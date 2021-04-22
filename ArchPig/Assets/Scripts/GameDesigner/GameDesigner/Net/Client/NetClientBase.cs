namespace Net.Client
{
    using Net.Share;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Sockets;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// 当处理缓冲区数据
    /// </summary>
    /// <param name="cmd">网络命令</param>
    /// <param name="buffer">缓冲区数据</param>
    /// <param name="index">数据开始索引</param>
    /// <param name="count">数据长度</param>
    public delegate void RevdBufferHandle(byte cmd, byte[] buffer, int index, int count);

    /// <summary>
    /// 网络客户端核心虚类 2019.3.3
    /// </summary>
    public abstract class NetClientBase : INetClient, ISendHandle
    {
        /// <summary>
        /// UDP客户端套接字
        /// </summary>
        public Socket Client { get; protected set; }
        /// <summary>
        /// IP地址
        /// </summary>
        public string host = "127.0.0.1";
        /// <summary>
        /// 端口号
        /// </summary>
        public int port = 666;
        /// <summary>
        /// 发送缓存器
        /// </summary>
        protected ConcurrentQueue<SendBuffer> sendBuffers = new ConcurrentQueue<SendBuffer>();
        /// <summary>
        /// 接收缓存器
        /// </summary>
        protected List<RevdBuffer> revdBuffers = new List<RevdBuffer>();
        /// <summary>
        /// 网络委托函数
        /// </summary>
        /// <returns></returns>
        public List<NetDelegate> Rpcs { get; set; } = new List<NetDelegate>();
        /// <summary>
        /// 多线程网络委托函数
        /// </summary>
        /// <returns></returns>
        public List<NetDelegate> TRpcs { get; set; } = new List<NetDelegate>();
        /// <summary>
        /// 线程字典
        /// </summary>
        protected Dictionary<string, Thread> threadDic = new Dictionary<string, Thread>();
        /// <summary>
        /// 网络连接状态
        /// </summary>
        public ConnectState ConnectState { get; protected set; } = ConnectState.None;
        /// <summary>
        /// 服务器与客户端是否是连接状态
        /// </summary>
        public bool Connected { get; protected set; } = false;
        /// <summary>
        /// 网络客户端实例
        /// </summary>
        public static NetClientBase Instance { get; set; }
        /// <summary>
        /// 输出调用RPC错误级别,红色警告
        /// </summary>
        public bool ThrowException = false;
        /// <summary>
        /// 是否使用unity主线程进行每一帧更新？  
        /// True：使用unity的Update之类的方法进行更新，unity的组建可以在Rpc函数内进行调用。
        /// False：使用多线程进行网络更新，使用多线程更新后unity的组件将不得在rpc函数内进行赋值，否则会无效
        /// </summary>
        public bool UseUnityThread { get; set; } = false;
        /// <summary>
        /// 当前网络客户端状态
        /// </summary>
        protected ConnectState connectState = ConnectState.None;
        /// <summary>
        /// 当前尝试重连次数
        /// </summary>
        protected int currFrequency = 0;
        /// <summary>
        /// 每秒发送数据长度
        /// </summary>
        protected int sendCount = 0;
        /// <summary>
        /// 每秒发送数据次数
        /// </summary>
        protected int sendAmount = 0;
        /// <summary>
        /// 每秒解析rpc函数次数
        /// </summary>
        protected int resolveAmount = 0;
        /// <summary>
        /// 每秒接收网络数据次数
        /// </summary>
        protected int receiveAmount = 0;
        /// <summary>
        /// 每秒接收网络数据大小
        /// </summary>
        protected int receiveCount = 0;
        /// <summary>
        /// 心跳次数
        /// </summary>
        protected int heart = 0;
        /// <summary>
        /// 当前客户端是否打开(运行)
        /// </summary>
        protected bool openClient = false;

        /// <summary>
        /// 输出调用网络函数
        /// </summary>
        public event Action<string> LogRpc;
        /// <summary>
        /// 输出调用RPC错误,白色警告
        /// </summary>
        public event Action<string> LogBug;
        /// <summary>
        /// 输出提示信息
        /// </summary>
        public event Action<string> Log;
        /// <summary>
        /// 当连接服务器成功事件
        /// </summary>
        public event Action OnConnectedHandle;
        /// <summary>
        /// 当连接失败事件
        /// </summary>
        public event Action OnConnectFailedHandle;
        /// <summary>
        /// 当尝试连接服务器事件
        /// </summary>
        public event Action OnTryToConnectHandle;
        /// <summary>
        /// 当连接中断 (异常) 事件
        /// </summary>
        public event Action OnConnectLostHandle;
        /// <summary>
        /// 当断开连接事件
        /// </summary>
        public event Action OnDisconnectHandle;
        /// <summary>
        /// 当接收到网络数据处理事件
        /// </summary>
        public event RevdBufferHandle OnRevdBufferHandle;
        /// <summary>
        /// 当接收到自定义数据触发的事件,注意：此事件是多线程在调用
        /// </summary>
	    public event RevdBufferHandle OnRevdCustomBufferHandle;
        /// <summary>
        /// 当断线重连成功触发事件
        /// </summary>
        public event Action OnReconnectHandle;
        /// <summary>
        /// 当关闭连接事件
        /// </summary>
        public event Action OnCloseConnectHandle;
        /// <summary>
        /// 当统计网络流量时触发
        /// </summary>
        public event NetworkDataTraffic OnNetworkDataTraffic;
        /// <summary>
        /// 当服务器连接人数溢出(未知客户端连接总数), 服务器忽略当前客户端的所有Rpc请求时调用
        /// </summary>
        public event Action OnExceededNumberHandle;
        /// <summary>
        /// 当服务器在线人数爆满, 服务器忽略当前客户端的所有Rpc请求时调用
        /// </summary>
        public event Action OnBlockConnectionHandle;

        /// <summary>
        /// 发送可靠传输数据队列
        /// </summary>
        private ConcurrentQueue<SendRTBuffer> sendRTBuffers = new ConcurrentQueue<SendRTBuffer>();
        /// <summary>
        /// 发送可靠传输缓冲
        /// </summary>
        private Dictionary<int, SendRTBuffer> sendRTDic = new Dictionary<int, SendRTBuffer>();
        /// <summary>
        /// 接收可靠传输缓冲
        /// </summary>
        private Dictionary<int, RevdRTBuffer> revdRTDic = new Dictionary<int, RevdRTBuffer>();

        /// <summary>
        /// 命令(1) + 帧尾或叫数据长度(4) = 5
        /// </summary>
        protected readonly int frame = 5;
        /// <summary>
        /// 心跳时间, 默认每5秒检查一次玩家是否离线, 玩家心跳确认为5次, 如果超出5次 则移除玩家客户端. 确认玩家离线总用时25秒, 
        /// 如果设置的值越小, 确认的速度也会越快. 但发送的数据也会增加.
        /// </summary>
        protected int HeartTime { get; set; } = 500;

        /// <summary>
        /// 添加信息输出
        /// </summary>
        /// <param name="msg"></param>
        internal void LogHandle(string msg)
        {
            Log?.Invoke(msg);
        }

        /// <summary>
        /// 接收网络数据缓冲区处理
        /// </summary>
        /// <param name="cmd">网络命令</param>
        /// <param name="buffer">网络数据缓冲</param>
        /// <param name="index">数据开始索引</param>
        /// <param name="count">数据长度(大小)</param>
        protected void RevdBufferHandle(byte cmd, byte[] buffer, int index, int count)
        {
            OnRevdBufferHandle?.Invoke(cmd, buffer, index, count);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public NetClientBase()
        {
            NetBehaviour.AddRpc(this, this);
            OnRevdBufferHandle += ReceiveData;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="useUnityThread">
        /// 是否使用unity主线程进行每一帧更新？  
        /// True：使用unity的Update之类的方法进行更新，unity的组建可以在Rpc函数内进行调用。
        /// False：使用多线程进行网络更新，使用多线程更新后unity的组件将不得在rpc函数内进行赋值，否则会无效
        /// </param>
        public NetClientBase(bool useUnityThread)
        {
            NetBehaviour.AddRpc(this, this);
            OnRevdBufferHandle += ReceiveData;
            UseUnityThread = useUnityThread;
        }

        /// <summary>
        /// 添加网络Rpc
        /// </summary>
        /// <param name="target">注册的对象实例</param>
        public void AddRpcHandle(object target)
        {
            NetBehaviour.AddRpc(target);
        }

        /// <summary>
        /// 添加网络Rpc
        /// </summary>
        /// <param name="client">网络客户端对象</param>
        /// <param name="target">注册的对象实例</param>
        public void AddRpcHandle(NetClientBase client, object target)
        {
            NetBehaviour.AddRpc(client, target);
        }

        /// <summary>
        /// 绑定Rpc函数
        /// </summary>
        /// <param name="target">注册的对象实例</param>
        public void BindRpc(object target) => NetBehaviour.AddRpc(this, target);

        /// <summary>
        /// 绑定Rpc函数
        /// </summary>
        /// <param name="client">网络客户端</param>
        /// <param name="target">要收集的rpc对象并绑定到网络客户端</param>
        public void BindRpc(NetClientBase client, object target) => NetBehaviour.AddRpc(client, target);

        /// <summary>
        /// 绑定网络调式信息处理接口
        /// </summary>
        /// <param name="debug"></param>
        public void BindLogHandle(IDebugHandle debug)
        {
            Log += debug.DebugLog;
            LogBug += debug.ErrorLog;
            LogRpc += debug.RpcLog;
        }

        /// <summary>
        /// 绑定网络状态处理接口
        /// </summary>
        /// <param name="network"></param>
        public void BindNetworkHandle(INetworkHandle network)
        {
            OnConnectedHandle += network.OnConnected;
            OnConnectFailedHandle += network.OnConnectFailed;
            OnConnectLostHandle += network.OnConnectLost;
            OnDisconnectHandle += network.OnDisconnect;
            OnReconnectHandle += network.OnReconnect;
            OnTryToConnectHandle += network.OnTryToConnect;
            OnCloseConnectHandle += network.OnCloseConnect;
            OnBlockConnectionHandle += network.OnBlockConnection;
        }

        /// <summary>
        /// 添加网络函数数据,添加后自动在rpc中调用
        /// </summary>
        /// <param name="func">指定的rpc函数名，函数名必须存在于rpcs属性中</param>
        /// <param name="pars">rpc参数：也就是函数参数</param>
        public void AddRpcBuffer(string func, object[] pars)
        {
            AddRevdBuffer(func, pars);
        }

        /// <summary>
        /// 开启心跳线程
        /// </summary>
        public void StartHeartHandle()
        {
            StartThread("HeartHandle", HeartHandle);
        }

        /// <summary>
        /// 开启线程
        /// </summary>
        /// <param name="threadKey">线程名称</param>
        /// <param name="start">线程函数</param>
        public void StartThread(string threadKey, ThreadStart start)
        {
            if (!threadDic.ContainsKey(threadKey))
            {
                var thread = new Thread(start)
                {
                    IsBackground = true,
                    Name = threadKey
                };
                thread.Start();
                threadDic.Add(threadKey, thread);
            }
            else
            {
                string str = threadDic[threadKey].ThreadState.ToString();
                if (str.Contains("Abort") | str.Contains("Stop"))
                {
                    threadDic.Remove(threadKey);
                    StartThread(threadKey, start);
                }
            }
        }

        /// <summary>
        /// 结束指定的线程
        /// </summary>
        /// <param name="threadKey">线程名称键值</param>
        public void AbortedThread(string threadKey)
        {
            if (threadDic.ContainsKey(threadKey))
                threadDic[threadKey].Abort();
        }

        /// <summary>
        /// 结束所有线程
        /// </summary>
        public void AbortedThread()
        {
            foreach (var thread in threadDic.Values)
            {
                thread?.Abort();
            }
        }

        //每一帧执行线程
        private void UpdateHandle()
        {
            while (openClient)
            {
                Thread.Sleep(1);
                try
                {
                    FixedUpdate();
                }
                catch { }
            }
        }

        /// <summary>
        /// 网络数据更新
        /// </summary>
        public void FixedUpdate()
        {
            int count = revdBuffers.Count;
            while (count > 0)
            {
                if (UseUnityThread & ThrowException)
                {
                    if (revdBuffers[0].method == null | revdBuffers[0].target == null)
                        goto REMOVEAT;
                    revdBuffers[0].method.Invoke(revdBuffers[0].target, revdBuffers[0].pars);
                REMOVEAT: count--;
                    revdBuffers.RemoveAt(0);
                    continue;
                }

                try
                {
                    LogRpc?.Invoke($"RPC:{revdBuffers[0].method.ToString()}");
                    revdBuffers[0].method.Invoke(revdBuffers[0].target, revdBuffers[0].pars);
                }
                catch (Exception e)
                {
                    if (LogBug == null)
                        continue;

                    string bug = $"BUG: Met:{revdBuffers[0].method} -> Tar:" + revdBuffers[0].target + " -> Pars:";
                    if (revdBuffers[0].pars == null)
                    {
                        bug += "null";
                        goto LOG;
                    }
                    foreach (var par in revdBuffers[0].pars)
                        bug += par + " , ";
                    LOG: LogBug(bug + " -> " + e);
                }
                finally
                {
                    count--;
                    revdBuffers.RemoveAt(0);
                }
            }

            StateHandle();
        }

        //状态处理
        private void StateHandle()
        {
            switch (connectState)
            {
                case ConnectState.Connected:
                    connectState = ConnectState.None;
                    OnConnectedHandle?.Invoke();
                    break;
                case ConnectState.ConnectFailed:
                    connectState = ConnectState.None;
                    OnConnectFailedHandle?.Invoke();
                    break;
                case ConnectState.TryToConnect:
                    connectState = ConnectState.None;
                    OnTryToConnectHandle?.Invoke();
                    break;
                case ConnectState.ConnectLost:
                    connectState = ConnectState.None;
                    OnConnectLostHandle?.Invoke();
                    break;
                case ConnectState.Disconnect:
                    connectState = ConnectState.None;
                    OnDisconnectHandle?.Invoke();
                    break;
                case ConnectState.ConnectClosed:
                    connectState = ConnectState.None;
                    OnCloseConnectHandle?.Invoke();
                    break;
                case ConnectState.Reconnect:
                    connectState = ConnectState.None;
                    OnReconnectHandle?.Invoke();
                    break;
                case ConnectState.ExceededNumber:
                    connectState = ConnectState.None;
                    OnExceededNumberHandle?.Invoke();
                    break;
                case ConnectState.BlockConnection:
                    connectState = ConnectState.None;
                    OnBlockConnectionHandle?.Invoke();
                    break;
            }
        }

        /// <summary>
        /// 连接服务器
        /// </summary>
        public void Connect()
        {
            Connect(connected => { });
        }

        /// <summary>
        /// 连接服务器
        /// </summary>
        /// <param name="result">连接结果</param>
        /// <returns></returns>
        public void Connect(Action<bool> result)
        {
            Connect(host, port, result);
        }

        /// <summary>
        /// 连接服务器
        /// </summary>
        /// <param name="host">IP地址</param>
        /// <param name="port">端口号</param>
        /// <param name="result">连接结果</param>
        public virtual void Connect(string host, int port, Action<bool> result)
        {
            openClient = true;
            if (Instance == null)
                Instance = this;
            if (Client == null) //如果套接字为空则说明没有连接上服务器
            {
                this.host = host;
                this.port = port;
                ConnectResult(host, port, result1 =>
                {
                    OnConnected(result1);
                    result(result1);
                });
            }
            else if (!Connected)
            {
                Client.Close();
                Client = null;
                ConnectState = connectState = ConnectState.ConnectLost;
                Log?.Invoke("服务器连接中断!");
                AbortedThread();
                result(false);
            }
            else
            {
                result(true);
            }
        }

        /// <summary>
        /// 连接服务器
        /// </summary>
        /// <param name="host">连接的服务器主机IP地址</param>
        /// <param name="port">连接的服务器主机端口号</param>
        /// <param name="result">连接结果</param>
        protected virtual void ConnectResult(string host, int port, Action<bool> result)
        {
            Client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);//创建套接字
            Client.Connect(host, port);
            bool isDone = false;
            Task.Run(() =>
            {
                while (!isDone)
                {
                    Client?.Send(new byte[] { 5 });
                    Thread.Sleep(1000);
                }
            });
            Task.Run(() =>
            {
                try
                {
                    Client.Receive(new byte[1024]);
                    isDone = true;
                    Connected = true;
                    StartupThread();
                    result(true);
                }
                catch
                {
                    isDone = true;
                    Client?.Close();
                    Client = null;
                    result(false);
                }
            });
            Task.Run(() =>
            {
                Thread.Sleep(5000);
                if (isDone)
                    return;
                isDone = true;
                Client?.Close();
                Client = null;
                result(false);
            });
        }

        /// <summary>
        /// 连接成功处理
        /// </summary>
        protected void StartupThread()
        {
            Connected = true;
            StartThread("SendHandle", SendHandle);
            StartThread("ReceiveHandle", ReceiveHandle);
            StartThread("DeBugHandle", DeBugHandle);
            StartThread("CheckRpcHandle", CheckRpcHandle);
            StartThread("HeartHandle", HeartHandle);
            StartThread("SendRTHandle", SendRTHandle);
            StartThread("RTHandle", RTHandle);
            if (!UseUnityThread)
                StartThread("UpdateHandle", UpdateHandle);
        }

        /// <summary>
        /// 连接结果处理
        /// </summary>
        /// <param name="result">结果</param>
        protected void OnConnected(bool result)
        {
            if (result)
            {
                ConnectState = connectState = ConnectState.Connected;
                Log?.Invoke("成功连接服务器...");
            }
            else
            {
                ConnectState = connectState = ConnectState.ConnectFailed;
                Log?.Invoke("服务器尚未开启或连接IP端口错误!");
                if (!UseUnityThread)
                    StartThread("UpdateHandle", UpdateHandle);
            }
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        /// <param name="reuseSocket">断开连接后还能重新使用？</param>
        public void Disconnect(bool reuseSocket)
        {
            ConnectState = connectState = ConnectState.Disconnect;
            Client.Disconnect(reuseSocket);
        }

        /// <summary>
        /// 调式输出网络流量信息
        /// </summary>
        private void DeBugHandle()
        {
            while (Connected)
            {
                Thread.Sleep(1000);
                try { OnNetworkDataTraffic?.Invoke(sendAmount, sendCount, receiveAmount, receiveCount, resolveAmount); } catch { }
                sendCount = 0;
                sendAmount = 0;
                resolveAmount = 0;
                receiveAmount = 0;
                receiveCount = 0;
            }
        }

        /// <summary>
        /// rpc检查处理线程
        /// </summary>
        private void CheckRpcHandle()
        {
            while (Connected)
            {
                Thread.Sleep(1);
                try
                {
                    RpcCheckUpdate(Rpcs);
                    RpcCheckUpdate(TRpcs);
                    CheckEventsUpdate();
                }
                catch (Exception e)
                {
                    LogBug?.Invoke(e.ToString());
                }
            }
        }

        //检查rpc函数
        private void RpcCheckUpdate(List<NetDelegate> rpcs)
        {
            for (int i = 0; i < rpcs.Count; i++)
            {
                if (rpcs[i].target == null | rpcs[i].method == null)
                {
                    rpcs.RemoveAt(i);
                    continue;
                }

                if (rpcs[i].target.Equals(null) | rpcs[i].method.Equals(null))
                {
                    rpcs.RemoveAt(i);
                }
            }
        }

        Type GetBaseType(Type type)
        {
            if (type == typeof(NetClientBase))
                return type;
            return GetBaseType(type.BaseType);
        }

        //检测事件委托函数
        private void CheckEventsUpdate()
        {
            Type type = GetBaseType(GetType());
            if (type == null)
                return;
            EventInfo[] es = type.GetEvents(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            for (int i = 0; i < es.Length; i++)
            {
                FieldInfo f = type.GetField(es[i].Name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                if (f == null)
                    continue;

                object value = f.GetValue(this);
                if (value == null)
                    continue;

                Delegate dele = (Delegate)value;
                Delegate[] ds = dele.GetInvocationList();
                for (int a = 0; a < ds.Length; a++)
                {
                    if (ds[a].Target == null | ds[a].Method == null)
                    {
                        es[i].RemoveEventHandler(this, ds[a]);
                        continue;
                    }
                    if (ds[a].Target.Equals(null) | ds[a].Method.Equals(null))
                    {
                        es[i].RemoveEventHandler(this, ds[a]);
                    }
                }
            }
        }

        /// <summary>
        /// 发包线程
        /// </summary>
        private void SendHandle()
        {
            MemoryStream stream = new MemoryStream();
            SendBuffer send = null;
            while (Connected)
            {
                Thread.Sleep(1);
                try
                {
                    SendHandle(stream, send);
                }
                catch (Exception e)
                {
                    if (e is SocketException)
                    {
                        ConnectState = connectState = ConnectState.ConnectLost;
                        Connected = false;
                        LogHandle("连接中断！");
                        AbortedThread();
                    }
                    Log?.Invoke("发送异常:" + e.Message);
                }
            }
            stream.Close();
            stream.Dispose();
        }

        /// <summary>
        /// 发送处理
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="send"></param>
        protected virtual void SendHandle(MemoryStream stream, SendBuffer send)
        {
            bool reulte = sendBuffers.TryDequeue(out send);
            if (!reulte)
                return;
            if (send.kernel)
                send.buffer = NetConvert.Serialize(send.fun, send.pars);
            stream.SetLength(0);
            stream.WriteByte(send.cmd);//网络命令 1字节
            stream.Write(BitConverter.GetBytes(send.buffer.Length), 0, 4);//1 - 4 = 5字节 记录封包长度
            stream.Write(send.buffer, 0, send.buffer.Length);
            if (stream.Length < 65507)
            {
                sendCount += (int)stream.Length;
                sendAmount++;
                Client.SendTo(stream.ToArray(), (int)stream.Length, 0, Client.RemoteEndPoint);
                return;
            }
            LogHandle("数据太大,发送被阻止!(可使用SendRT方法(可靠传输数据流)进行发送网络大数据)");
        }

        /// <summary>
        /// 后台线程接收数据
        /// </summary>
        protected virtual void ReceiveHandle()
        {
            try
            {
                if (!Connected | !Client.Connected)
                    return;
                byte[] buffer = new byte[65507];
                Client.BeginReceive(buffer, 0, 65507, 0, (ar) => {
                    try
                    {
                        int count = Client.EndReceive(ar);
                        receiveCount += count;
                        receiveAmount++;
                        ResolveBuffer(buffer, 0, count);
                    }
                    catch { }
                    finally
                    {
                        ReceiveHandle();
                    }
                }, null);
            }
            catch (Exception e)
            {
                if (e is SocketException)
                {
                    Connected = false;
                    AbortedThread();
                }
                LogHandle(e.ToString());
            }
        }

        /// <summary>
        /// 解析网络数据包
        /// </summary>
        protected virtual void ResolveBuffer(byte[] buffer, int index, int count)
        {
            byte cmd = buffer[index];//0 = 网络命令整形数据
            int size = BitConverter.ToInt32(buffer, index + 1);//1+4=5 网络数据长度大小
            if (index + frame + size == count)//如果数据完整
            {
                OnRevdBufferHandle?.Invoke(cmd, buffer, index + frame, size);
            }
        }

        /// <summary>
        /// 接收网络数据
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="cmd"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        private void ReceiveData(byte cmd, byte[] buffer, int index, int count)
        {
            resolveAmount++;
            switch (cmd)
            {
                case NetCmd.RevdHeartbeat:
                    heart = 0;
                    break;
                case NetCmd.SendHeartbeat:
                    Send(NetCmd.RevdHeartbeat, new byte[] { 6, 0, 0, 0, 0 });
                    break;
                case NetCmd.AllCmd:
                    NetConvert.Deserialize(buffer, index, count, (func, pars) => {
                        AddRevdBuffer(func, pars);
                    });
                    break;
                case NetCmd.CallRpc:
                    NetConvert.Deserialize(buffer, index, count, (func, pars) => {
                        AddRevdBuffer(func, pars);
                    });
                    break;
                case NetCmd.LocalCmd:
                    NetConvert.Deserialize(buffer, index, count, (func, pars) => {
                        AddRevdBuffer(func, pars);
                    });
                    break;
                case NetCmd.SceneCmd:
                    NetConvert.Deserialize(buffer, index, count, (func, pars) => {
                        AddRevdBuffer(func, pars);
                    });
                    break;
                case NetCmd.ThreadRpc:
                    NetConvert.Deserialize(buffer, index, count, (func, pars) => {
                        ThreadRpc(func, pars);
                    });
                    break;
                case NetCmd.ExceededNumber:
                    connectState = ConnectState.ExceededNumber;
                    break;
                case NetCmd.BlockConnection:
                    connectState = ConnectState.BlockConnection;
                    break;
                case NetCmd.ReliableTransport:
                    int kernel = buffer[index];
                    index++;
                    int rtID = BitConverter.ToInt32(buffer, index);
                    index += 4;
                    int rtIndex = BitConverter.ToInt32(buffer, index);
                    index += 4;
                    int rtLength = BitConverter.ToInt32(buffer, index);
                    index += 4;
                    int pross = BitConverter.ToInt32(buffer, index);
                    index += 4;
                    ReliableTransport(cmd, kernel, rtID, rtIndex, rtLength, index, buffer, count + frame, pross, (func, pars) => {
                        AddRevdBuffer(func, pars);
                    });
                    break;
                case NetCmd.EntityReliableTransport:
                    int kernel1 = buffer[index];
                    index++;
                    int rtID1 = BitConverter.ToInt32(buffer, index);
                    index += 4;
                    int rtIndex1 = BitConverter.ToInt32(buffer, index);
                    index += 4;
                    int rtLength1 = BitConverter.ToInt32(buffer, index);
                    index += 4;
                    int pross1 = BitConverter.ToInt32(buffer, index);
                    index += 4;
                    ReliableTransport(cmd, kernel1, rtID1, rtIndex1, rtLength1, index, buffer, count + frame, pross1, (func, pars) => {
                        AddRevdBuffer(func, pars);
                    });
                    break;
                case NetCmd.ReliableCallback:
                    NetConvert.Deserialize(buffer, index, count, (func, pars) => {
                        int key = (int)pars[0];
                        int downIndex = (int)pars[1];
                        bool done = (bool)pars[2];
                        byte rtCmd = (byte)pars[3];
                        if (!sendRTDic.ContainsKey(key))
                            return;
                        var rt = sendRTDic[key];
                        if (done)
                        {
                            rt.buffer.Clear();
                            rt.datas = null;
                            rt.progress?.Invoke(100, RTState.Complete);
                            sendRTDic.Remove(key);
                            return;
                        }
                        rt.index = downIndex;
                        rt.tryIndex = 0;
                        SendRT(rt);
                        var value = (float)downIndex / rt.buffer.Count * 100;
                        rt.progress?.Invoke(value, RTState.Sending);
                    });
                    break;
                case NetCmd.SwitchPort:
                    port = BitConverter.ToInt32(buffer, index);
                    Client?.Close();
                    Client = null;
                    Connect();
                    break;
                default:
                    OnRevdCustomBufferHandle?.Invoke(cmd, buffer, index, count);
                    break;
            }
        }

        //发送可靠数据
        void SendRT(SendRTBuffer sendRT)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                if (!sendRT.buffer.ContainsKey(sendRT.index))
                    return;
                var bytes = sendRT.buffer[sendRT.index];
                stream.WriteByte((byte)sendRT.cmd);
                stream.Write(BitConverter.GetBytes(bytes.Length + sendRT.progressData.Length + 17), 0, 4);
                stream.WriteByte(sendRT.kernel);
                stream.Write(BitConverter.GetBytes(sendRT.keyID), 0, 4);
                stream.Write(BitConverter.GetBytes(sendRT.index), 0, 4);
                stream.Write(BitConverter.GetBytes(sendRT.count), 0, 4);
                stream.Write(BitConverter.GetBytes(sendRT.progressData.Length), 0, 4);
                stream.Write(sendRT.progressData, 0, sendRT.progressData.Length);
                stream.Write(bytes, 0, bytes.Length);
                byte[] buffer = stream.ToArray();
                if (buffer.Length < 65507)
                    Client.Send(buffer, buffer.Length, 0);
            }
            sendRT.time = DateTime.Now.AddSeconds(sendRT.buffer.Count);
        }

        /// <summary>
        /// 可靠传输数据缓冲区
        /// </summary>
        /// <param name="buffer"></param>
        public delegate void RevdRTDataBuffer(byte[] buffer);
        /// <summary>
        /// 当接收可靠传输数据缓冲
        /// </summary>
        public event RevdRTDataBuffer OnRevdRTDataBuffer;

        //可靠传输处理
        void ReliableTransport(byte cmd, int kernel, int rtID, int rtIndex, int rtLength, int index, byte[] buffer, int count, int pross, Action<string, object[]> call)
        {
            RevdRTBuffer revdRT;
            if (!revdRTDic.ContainsKey(rtID))
            {
                revdRT = new RevdRTBuffer();
                revdRTDic.Add(rtID, revdRT);
            }
            revdRT = revdRTDic[rtID];
            if (revdRT.index == rtIndex)
            {
                if (kernel == 3)
                {
                    NetConvert.Deserialize(buffer, index, pross, (func, objs) => {
                        List<object> list = new List<object>(objs) { (int)((float)revdRT.stream.Length / rtLength * 100) };
                        AddRpcBuffer(func, list.ToArray());
                    });
                }
                revdRT.stream.Write(buffer, index + pross, count - (index + pross));
                if (revdRT.stream.Length >= rtLength)
                {
                    Send(NetCmd.ReliableCallback, string.Empty, rtID, revdRT.index, true, cmd);
                    byte[] buff = revdRT.stream.ToArray();
                    revdRT.stream.Dispose();
                    revdRTDic.Remove(rtID);
                    if (kernel == 1)
                        OnRevdRTDataBuffer?.Invoke(buff);
                    else if (kernel == 2 | kernel == 3)
                    {
                        ResolvingRT(buff, (func, pars, buff1) => {
                            List<object> list = new List<object>(pars) { buff1 };
                            AddRpcBuffer(func, list.ToArray());
                        });
                    }
                    else
                    {
                        NetConvert.Deserialize(buff, 0, buff.Length, (func, pars) => {
                            call(func, pars);
                        });
                    }
                }
                else
                {
                    revdRT.index++;
                    Send(NetCmd.ReliableCallback, string.Empty, rtID, revdRT.index, false, cmd);
                }
            }
            else
            {
                Send(NetCmd.ReliableCallback, string.Empty, rtID, revdRT.index, false, cmd);
            }
        }

        //解析可靠传输数据
        void ResolvingRT(byte[] buffer, Action<string, object[], byte[]> call)
        {
            int index = 0;
            int length = BitConverter.ToInt32(buffer, index);
            index += 4;
            var func = NetConvert.Deserialize(buffer, index, length);
            index += length;
            int dataLength = BitConverter.ToInt32(buffer, index);
            index += 4;
            byte[] data = new byte[dataLength];
            Array.Copy(buffer, index, data, 0, dataLength);
            call(func.name, func.pars, data);
        }

        /// <summary>
        /// 发送可靠网络传输, 可以发送大型文件数据
        /// 调用此方法通常情况下是一定把数据发送成功为止, (尽量少用)
        /// </summary>
        /// <param name="cmd">网络命令</param>
        /// <param name="func">函数名</param>
        /// <param name="pars">参数</param>
        public void SendRT(RTCmd cmd, string func, params object[] pars)
        {
            SendRT(cmd, func, null, pars);
        }

        /// <summary>
        /// 发送可靠网络传输, 可以发送大型文件数据
        /// 调用此方法通常情况下是一定把数据发送成功为止, (尽量少用)
        /// </summary>
        /// <param name="cmd">网络命令</param>
        /// <param name="func">函数名</param>
        /// <param name="progress">当前数据进度</param>
        /// <param name="pars">参数</param>
        public void SendRT(RTCmd cmd, string func, SendRTProgress progress, params object[] pars)
        {
            var buffer = new SendRTBuffer(cmd, func, progress, pars);
            sendRTBuffers.Enqueue(buffer);
        }

        /// <summary>
        /// 发送可靠网络传输, 可发送大数据流
        /// 调用此方法通常情况下是一定把数据发送成功为止, (尽量少用)
        /// </summary>
        /// <param name="cmd">网络命令</param>
        /// <param name="progress">当前数据进度</param>
        /// <param name="buffers"></param>
        public void SendRT(RTCmd cmd, SendRTProgress progress, byte[] buffers)
        {
            var buffer = new SendRTBuffer(cmd, progress, buffers);
            sendRTBuffers.Enqueue(buffer);
        }

        /// <summary>
        /// 发送解释型可靠传输流, 带有命令, 解释对象, 大型数据 注意:此方法跟前面两种方法不同
        /// 使用此方法发送数据, 在服务器声明的rpc方法必须有(byte[] buffer)参数放在后面
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="progress"></param>
        /// <param name="buffer"></param>
        /// <param name="func"></param>
        /// <param name="pars"></param>
        public void SendERT(RTCmd cmd, SendRTProgress progress, byte[] buffer, string func, params object[] pars)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                var entyBts = NetConvert.Serialize(func, pars);
                stream.Write(BitConverter.GetBytes(entyBts.Length), 0, 4);
                stream.Write(entyBts, 0, entyBts.Length);
                stream.Write(BitConverter.GetBytes(buffer.Length), 0, 4);
                stream.Write(buffer, 0, buffer.Length);
                var buffer1 = new SendRTBuffer(cmd, progress, stream.ToArray()) { kernel = 2 };
                sendRTBuffers.Enqueue(buffer1);
            }
        }

        /// <summary>
        /// 发送解释型可靠传输流, 带有命令, 解释对象, 大型数据 注意:此方法跟前面两种方法不同
        /// 使用此方法发送数据, 在服务器声明的rpc方法必须有(byte[] buffer)参数放在后面
        /// </summary>
        /// <param name="cmd">网络命令: 可选择在实体类中进行调用 或 在公共服务器类调用</param>
        /// <param name="progress">发送大型数据需要的进度事件委托, 可以监听当前发送的进度</param>
        /// <param name="buffer">大型数据比特流</param>
        /// <param name="func">当发送完成后在对方调用的函数名, 而不是在本地调用的函数哦</param>
        /// <param name="progressFunc">这个函数会在对方每次接收数大数据时调用, 对方可以定义这个函数进行查看当前下载进度</param>
        /// <param name="progressItem">对方的 progressFunc(object progressItem, int progressValue) 函数必须定义两个参数, progressItem参数用来标识用的, progressValue参数是当前下载的进度值</param>
        /// <param name="pars"></param>
        public void SendERTP(RTCmd cmd, SendRTProgress progress, byte[] buffer, string func, string progressFunc, object progressItem, params object[] pars)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                var entyBts = NetConvert.Serialize(func, pars);
                stream.Write(BitConverter.GetBytes(entyBts.Length), 0, 4);
                stream.Write(entyBts, 0, entyBts.Length);
                var entyBts1 = NetConvert.Serialize(progressFunc, progressItem);
                stream.Write(BitConverter.GetBytes(buffer.Length), 0, 4);
                stream.Write(buffer, 0, buffer.Length);
                var buffer1 = new SendRTBuffer(cmd, progress, stream.ToArray()) { kernel = 3, progressData = entyBts1 };
                sendRTBuffers.Enqueue(buffer1);
            }
        }

        //可靠传输线程
        void SendRTHandle()
        {
            while (openClient)
            {
                Thread.Sleep(1);
                try
                {
                    if (!sendRTBuffers.TryDequeue(out SendRTBuffer sendRT))
                        continue;
                    byte[] buffer;
                    if (sendRT.kernel == 0)
                        buffer = NetConvert.Serialize(sendRT.func, sendRT.pars);
                    else
                        buffer = sendRT.datas;
                    sendRT.count = buffer.Length;
                JUMP: int keyID = Share.Random.Range(100000000, 999999999);
                    if (sendRTDic.ContainsKey(keyID))
                        goto JUMP;
                    sendRT.keyID = keyID;
                    sendRTDic.Add(keyID, sendRT);
                    int index = 0, key = 0;
                    while (index < buffer.Length)
                    {
                        int count = 50000;
                        if (index + count >= buffer.Length)
                            count = buffer.Length - index;
                        using (MemoryStream stream = new MemoryStream(buffer, index, count))
                        {
                            byte[] buf1 = stream.ToArray();
                            sendRT.buffer.Add(key, buf1);
                            key++;
                        }
                        index += 50000;
                    }
                    sendRT.time = DateTime.Now.AddSeconds(sendRT.buffer.Count);
                }
                catch (Exception ex)
                {
                    LogHandle("发送可靠异常:" + ex.Message);
                }
            }
        }

        //可靠发送处理线程
        void RTHandle()
        {
            while (openClient)
            {
                Thread.Sleep(200);
                try
                {
                    foreach (var rt in sendRTDic.Values)
                    {
                        if (DateTime.Now > rt.time & rt.tryIndex > 5)
                        {
                            rt.buffer.Clear();
                            rt.datas = null;
                            rt.progress?.Invoke(0, RTState.FailSend);
                            sendRTDic.Remove(rt.keyID);
                            //GC.Collect();
                        }
                        else if (DateTime.Now > rt.time)
                        {
                            rt.tryIndex++;
                            SendRT(rt);
                            var value = (float)rt.index / rt.buffer.Count * 100;
                            rt.progress?.Invoke(value, RTState.FailSend);
                        }
                        else if (rt.index <= rt.pasIndex)
                        {
                            SendRT(rt);
                        }
                        break;//注意:排帧发送,前帧不走后面帧不能走
                    }
                }
                catch (Exception ex)
                {
                    LogHandle("可靠异常:" + ex.Message);
                };
            }
        }

        /// <summary>
        /// 调用网络封包数据
        /// </summary>
        /// <param name="funName"></param>
        /// <param name="pars"></param>
        private void AddRevdBuffer(string funName, object[] pars)
        {
            Parallel.For(0, Rpcs.Count, (i) =>//遍历远程过程调用的函数委托
            {
                if (funName == Rpcs[i].method.Name)
                {
                    revdBuffers.Add(new RevdBuffer(Rpcs[i].target, Rpcs[i].method, pars));
                }
            });
        }

        /// <summary>
        /// 多线程调用网络函数
        /// </summary>
        /// <param name="funName"></param>
        /// <param name="pars"></param>
        private void ThreadRpc(string funName, object[] pars)
        {
            Parallel.For(0, TRpcs.Count, (i) => //遍历远程过程调用的函数委托
            {
                if (funName == TRpcs[i].method.Name)
                {
                    TRpcs[i].method.Invoke(TRpcs[i].target, pars);
                }
            });
        }

        /// <summary>
        /// 后台线程发送心跳包
        /// </summary>
        protected virtual void HeartHandle()
        {
            while (openClient & currFrequency < 10)
            {
                Thread.Sleep(HeartTime);//5秒发送一个心跳包
                try
                {
                    if (Connected & heart < 3)
                    {
                        Send(NetCmd.SendHeartbeat, new byte[0]);
                        heart++;
                    }
                    else if (heart <= 3)//连接中断事件执行
                    {
                        ConnectState = connectState = ConnectState.ConnectLost;
                        heart = 4;
                        LogHandle("连接中断！");
                    }
                    else//尝试连接执行
                    {
                        Reconnection(10);
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// 断线重新连接
        /// </summary>
        /// <param name="maxFrequency">重连最大次数</param>
        protected virtual void Reconnection(int maxFrequency)
        {
            bool done = false;
            ConnectResult(host, port, result =>
            {
                done = true;
                currFrequency++;
                if (result)
                {
                    currFrequency = 0;
                    heart = 0;
                    ConnectState = connectState = ConnectState.Reconnect;
                    Log?.Invoke("重连成功...");
                }
                else if (currFrequency >= maxFrequency)//尝试maxFrequency次重连，如果失败则退出线程
                {
                    Close();
                    Log?.Invoke($"连接失败!请检查网络是否异常");
                }
                else
                {
                    ConnectState = connectState = ConnectState.TryToConnect;
                    Log?.Invoke($"尝试重连:{currFrequency}...");
                }
            });
            while (!done) { }
        }

        /// <summary>
        /// 关闭连接,释放线程以及所占资源
        /// </summary>
        public virtual void Close()
        {
            if (ConnectState != ConnectState.ConnectClosed)
                Client?.SendTo(new byte[] { 8, 0, 0, 0, 0 }, 5, 0, Client.RemoteEndPoint);
            Connected = false;
            openClient = false;
            ConnectState = connectState = ConnectState.ConnectClosed;
            Thread.Sleep(1000);//给update线程一秒的时间处理关闭事件
            AbortedThread("HeartHandle");
            AbortedThread("UpdateHandle");
            Client?.Close();
            Client?.Dispose();
            Client = null;
        }

        /// <summary>
        /// 发送自定义网络数据 默认使用UDP发送方式
        /// </summary>
        /// <param name="buffer">数据缓冲区</param>
        public void Send(byte[] buffer)
        {
            Send(NetCmd.CallRpc, buffer);
        }

        /// <summary>
        /// 发送自定义网络数据 可使用TCP或UDP进行发送
        /// </summary>
        /// <param name="cmd">网络命令</param>
        /// <param name="buffer">发送字节数组缓冲区</param>
        public void Send(byte cmd, byte[] buffer)
        {
            var sendbuff = new SendBuffer(cmd, buffer);
            sendBuffers.Enqueue(sendbuff);
        }

        /// <summary>
        /// 发送RPCFun网络数据 默认使用UDP发送方式
        /// </summary>
        /// <param name="fun">RPCFun函数</param>
        /// <param name="pars">RPCFun参数</param>
        public void Send(string fun, params object[] pars)
        {
            Send(NetCmd.CallRpc, fun, pars);
        }

        /// <summary>
        /// 发送带有网络命令的RPCFun网络数据
        /// </summary>
        /// <param name="cmd">网络命令</param>
        /// <param name="fun">RPCFun函数</param>
        /// <param name="pars">RPCFun参数</param>
        public void Send(byte cmd, string fun, params object[] pars)
        {
            var sendbuff = new SendBuffer(cmd, fun, pars);
            sendBuffers.Enqueue(sendbuff);
        }

        /// <summary>
        /// 远程过程调用 同Send方法
        /// </summary>
        /// <param name="fun">Call名</param>
        /// <param name="pars">Call函数</param>
        public void CallRpc(string fun, params object[] pars) => Send(fun, pars);

        /// <summary>
        /// 远程过程调用 同Send方法
        /// </summary>
        /// <param name="cmd">网络命令，请看NetCmd类定义</param>
        /// <param name="fun">Call名</param>
        /// <param name="pars">Call函数</param>
        public void CallRpc(byte cmd, string fun, params object[] pars) => Send(cmd, fun, pars);

        /// <summary>
        /// 网络请求 同Send方法
        /// </summary>
        /// <param name="fun">Call名</param>
        /// <param name="pars">Call函数</param>
        public void Request(string fun, params object[] pars) => Send(fun, pars);

        /// <summary>
        /// 网络请求 同Send方法
        /// </summary>
        /// <param name="cmd">网络命令，请看NetCmd类定义</param>
        /// <param name="fun">Call名</param>
        /// <param name="pars">Call函数</param>
        public void Request(byte cmd, string fun, params object[] pars) => Send(cmd, fun, pars);
    }
}