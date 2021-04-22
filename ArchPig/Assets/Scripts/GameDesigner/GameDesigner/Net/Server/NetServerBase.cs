namespace Net.Server
{
    using Net.Share;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using System.Collections.Concurrent;
    using System.Threading.Tasks;

    /// <summary>
    /// 网络服务器核心基类 2019.11.22
    /// 作者:龙兄 
    /// QQ：1752062104, 
    /// Player为你自己定义的玩家类型, 玩家类型针对每个用户的数据存储用到, 
    /// Scene为服务器场景类型, 你的游戏中场景里面需要有什么功能, 则写在Scene类里面
    /// </summary>
    public abstract class NetServerBase<Player, Scene> :
        IServerHandle<Player,Scene>
        where Player : NetPlayer, new()
        where Scene : NetScene<Player>, new()
    {
        /// <summary>
        /// 服务器套接字
        /// </summary>
        public Socket Server { get; protected set; }
        /// <summary>
        /// 远程过程调用委托
        /// </summary>
        public List<NetDelegate> Rpcs { get; set; } = new List<NetDelegate>();
        /// <summary>
        /// 所有在线的客户端 与Players为互助字典
        /// </summary>
        public ConcurrentDictionary<EndPoint, Player> Clients { get; private set; } = new ConcurrentDictionary<EndPoint, Player>();
        /// <summary>
        /// 所有在线的客户端 与Clients为互助字典 所添加的键值为NetPlayer.playerID, 当未知客户端请求时返回的对象请把NetPlayer.playerID赋值好,方便后面用到
        /// </summary>
        public ConcurrentDictionary<string, Player> Players { get; private set; } = new ConcurrentDictionary<string, Player>();
        /// <summary>
        /// 未知客户端连接 或 刚连接服务器还未登录账号的IP 
        /// </summary>
        public ConcurrentDictionary<EndPoint, Player> UnClients { get; private set; } = new ConcurrentDictionary<EndPoint, Player>();
        /// <summary>
        /// 服务器场景，每个key都处于一个场景或房间，关卡，value是场景对象
        /// </summary>
        public ConcurrentDictionary<string, Scene> Scenes { get; set; } = new ConcurrentDictionary<string, Scene>();
        /// <summary>
        /// 网络服务器单例
        /// </summary>
        public static NetServerBase<Player,Scene> Instance { get; protected set; }
        /// <summary>
        /// 当前玩家在线人数
        /// </summary>
        public int OnlinePlayers { get { return Clients.Count; } }
        /// <summary>
        /// 服务器端口
        /// </summary>
        public int Port { get; protected set; }
        /// <summary>
        /// 服务器是否处于运行状态, 如果服务器套接字已经被释放则返回False, 否则返回True. 当调用Close方法后将改变状态
        /// </summary>
        public bool IsRunServer { get; private set; } = true;
        /// <summary>
        /// 获取或设置最大可排队人数， 如果未知客户端人数超出LineUp值将不处理超出排队的未知客户端数据请求 ， 默认排队1000人
        /// </summary>
        public int LineUp { get; set; } = 1000;
        /// <summary>
        /// 允许玩家在线人数最大值（玩家在线上限）默认2000人同时在线
        /// </summary>
        public int OnlineLimit { get; set; } = 2000;
        /// <summary>
        /// 是否允许接收大型数据传输, 
        /// true: 可接收大数据传输
        /// false: 只接收检验后的数据, 服务器防御一定的CC攻击
        /// </summary>
        public bool IsReceBigData { get; set; } = false;
        /// <summary>
        /// 超出的排队人数，不处理的人数
        /// </summary>
        protected int exceededNumber = 0;
        /// <summary>
        /// 服务器爆满, 阻止连接人数 与OnlineLimit属性有关
        /// </summary>
        protected int blockConnection = 0;
        /// <summary>
        /// 服务器主大厅默认场景名称
        /// </summary>
        protected string DefaultScene { get; set; } = "MainScene";
        /// <summary>
        /// 网络统计发送数据长度/秒
        /// </summary>
        protected int sendCount = 0;
        /// <summary>
        /// 网络统计发送次数/秒
        /// </summary>
        protected int sendAmount = 0;
        /// <summary>
        /// 网络统计解析次数/秒
        /// </summary>
        protected int resolveAmount = 0;
        /// <summary>
        /// 网络统计接收次数/秒
        /// </summary>
        protected int receiveAmount = 0;
        /// <summary>
        /// 网络统计接收长度/秒
        /// </summary>
        protected int receiveCount = 0;
        /// <summary>
        /// 命令(1) + 帧尾或叫数据长度(4) = 5
        /// </summary>
        protected readonly int frame = 5;

        #region 服务器事件处理
        /// <summary>
        /// 开始运行服务器事件
        /// </summary>
        public Action OnStartingHandle { get; set; }
        /// <summary>
        /// 服务器启动成功事件
        /// </summary>
        public Action OnStartupCompletedHandle { get; set; }
        /// <summary>
        /// 当前有客户端连接触发事件
        /// </summary>
        public Action<Player> OnHasConnectHandle { get; set; }
        /// <summary>
        /// 当添加客户端到所有在线的玩家集合中触发的事件
        /// </summary>
        public Action<Player> OnAddClientHandle { get; set; }
        /// <summary>
        /// 当接收到网络数据处理事件
        /// </summary>
        public RevdBufferHandle<Player> OnRevdBufferHandle { get; set; }
        /// <summary>
        /// 当接收自定义网络数据事件
        /// </summary>
        public RevdBufferHandle<Player> OnRevdCustomBufferHandle { get; set; }
        /// <summary>
        /// 当移除客户端时触发事件
        /// </summary>
        public Action<Player> OnRemoveClientEvent { get; set; }
        /// <summary>
        /// 当统计网络流量时触发
        /// </summary>
        public NetworkDataTraffic OnNetworkDataTraffic { get; set; }
        /// <summary>
        /// 输出日志
        /// </summary>
        public Action<string> Log { get; set; }
        /// <summary>
        /// 调式输出日志
        /// </summary>
        public List<string> logList = new List<string>();
        #endregion
        
        /// <summary>
        /// 构造网络服务器函数
        /// </summary>
        public NetServerBase() { }

        /// <summary>
        /// 玩家索引
        /// </summary>
        /// <param name="remotePoint"></param>
        /// <returns></returns>
        public Player this[EndPoint remotePoint] => Clients[remotePoint];

        /// <summary>
        /// 场景索引
        /// </summary>
        /// <param name="sceneID"></param>
        /// <returns></returns>
        public Scene this[string sceneID] => Scenes[sceneID];

        /// <summary>
        /// 获得所有在线的客户端对象
        /// </summary>
        /// <returns></returns>
        public List<Player> GetClients()
        {
            return GetClients(this);
        }

        /// <summary>
        /// 获得所有在线的客户端对象
        /// </summary>
        /// <returns></returns>
        public List<Player> GetClients(NetServerBase<Player,Scene> server)
        {
            return new List<Player>(server.Clients.Values);
        }

        /// <summary>
        /// 获得所有服务器场景
        /// </summary>
        /// <returns></returns>
        public List<Scene> GetScenes()
        {
            return new List<Scene>(Scenes.Values);
        }

        /// <summary>
        /// 当未知客户端发送数据请求，返回null，不允许unClient进入服务器!，返回对象，允许unClient客户端进入服务器
        /// 客户端玩家的入口点，在这里可以控制客户端是否可以进入服务器与其他客户端进行网络交互
        /// 在这里可以用来判断客户端登录和注册等等进站许可
        /// </summary>
        /// <param name="unClient">客户端终端</param>
        /// <param name="buffer">缓冲区</param>
        /// <param name="cmd">命令</param>
        /// <param name="index">字节开始索引</param>
        /// <param name="count">字节长度</param>
        /// <returns></returns>
        protected virtual Player OnUnClientRequest(Player unClient, byte cmd, byte[] buffer, int index, int count)
        {
            return unClient;
        }

        /// <summary>
        /// 当开始启动服务器
        /// </summary>
        protected virtual void OnStarting() { }

        /// <summary>
        /// 当服务器启动完毕
        /// </summary>
        protected virtual void OnStartupCompleted() { }

        /// <summary>
        /// 当添加默认网络场景，服务器初始化后会默认创建一个主场景，供所有玩家刚登陆成功分配的临时场景，默认初始化场景人数为1000人
        /// </summary>
        /// <returns>返回值string：网络玩家所在的场景名称 , 返回值NetScene：网络玩家的场景对象</returns>
        protected virtual KeyValuePair<string, Scene> OnAddDefaultScene()
        {
            return new KeyValuePair<string, Scene>(DefaultScene, new Scene { sceneCapacity = 1000 } );
        }

        /// <summary>
        /// 当添加玩家到默认场景， 如果不想添加刚登录游戏成功的玩家进入主场景，可重写此方法让其失效
        /// </summary>
        /// <param name="client"></param>
        protected virtual void OnAddPlayerToScene(Player client)
        {
            Scenes[DefaultScene].Players.Add(client);//将网络玩家添加到主场景集合中
            client.Scene = Scenes[DefaultScene];//赋值玩家所在的场景实体
        }

        /// <summary>
        /// 当有客户端连接
        /// </summary>
        /// <param name="client">客户端套接字</param>
        protected virtual void OnHasConnect(Player client) { }

        /// <summary>
        /// 当服务器判定客户端为断线或连接异常时，移除客户端时调用
        /// </summary>
        /// <param name="client">要移除的客户端</param>
        protected virtual void OnRemoveClient(Player client) { }

        /// <summary>
        /// 当开始调用服务器RPCFun函数 或 开始调用自定义网络命令时 可设置请求客户端的client为全局字段，可方便在服务器RPCFun函数内引用!!!
        /// 在多线程时有1%不安全，当出现client赋值错误诡异时，可在网络函数加上 RPCFunc(NetCmd.SafeCall) 命令
        /// </summary>
        /// <param name="client">发送请求数据的客户端</param>
        protected virtual void OnInvokeRpc(Player client) { }

        /// <summary>
        /// 当接收到客户端自定义数据请求,在这里可以使用你自己的网络命令，系列化方式等进行解析网络数据。（你可以在这里使用ProtoBuf或Json来解析网络数据）
        /// IOS平台支持
        /// </summary>
        /// <param name="client">当前客户端</param>
        /// <param name="cmd">网络命令</param>
        /// <param name="buffer">数据缓冲区</param>
        /// <param name="index">数据的开始索引</param>
        /// <param name="count">数据长度</param>
        protected virtual void OnReceiveBuffer(Player client, byte cmd, byte[] buffer, int index, int count) { }

        /// <summary>
        /// 运行服务器
        /// </summary>
        /// <param name="port">服务器端口号</param>
        /// <param name="workerThreads">处理线程数</param>
        public void Run(int port = 666, int workerThreads = 0) => Start(port, workerThreads);

        /// <summary>
        /// 启动服务器
        /// </summary>
        /// <param name="port">端口</param>
        /// <param name="workerThreads">处理线程数</param>
        public virtual void Start(int port = 666, int workerThreads = 0)
        {
            if (Server != null)//如果服务器套接字已创建
                throw new Exception("服务器已经运行，不可重新启动，请先关闭后在重启服务器");

            Port = port;
            OnStartingHandle += OnStarting;
            OnStartupCompletedHandle += OnStartupCompleted;
            OnHasConnectHandle += OnHasConnect;
            OnRevdCustomBufferHandle += OnReceiveBuffer;
            OnRemoveClientEvent += OnRemoveClient;
            OnRevdBufferHandle += ReceiveBufferHandle;

            OnStartingHandle();
            logList.Add("服务器开始运行...");

            if (Instance == null)
                Instance = this;

            Rpcs.AddRange(NetBehaviour<Player,Scene>.GetRpcs(this));
            Server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);//---UDP协议
            IPEndPoint ip = new IPEndPoint(IPAddress.Any, port);//IP端口设置
            Server.Bind(ip);//绑定UDP IP端口

            try {//在安卓启动服务器时忽略此错误
                uint IOC_IN = 0x80000000;
                uint IOC_VENDOR = 0x18000000;
                uint SIO_UDP_CONNRESET = IOC_IN | IOC_VENDOR | 12;
                Server.IOControl((int)SIO_UDP_CONNRESET, new byte[] { Convert.ToByte(false) }, null);//udp远程关闭现有连接方案
            } catch { }
            
            new Thread(UdpUpdate) { IsBackground = true, Name = "UdpUpdate" }.Start();//创建UDP每一帧线程
            new Thread(HeartUpdate) { IsBackground = true, Name = "HeartUpdate" }.Start();//创建心跳包线程
            new Thread(UnHeartUpdate) { IsBackground = true, Name = "UnHeartUpdate" }.Start();//创建未知客户端心跳包线程
            new Thread(DebugThread) { IsBackground = true, Name = "DebugThread" }.Start();
            new Thread(DebugLogThread) { IsBackground = true, Name = "DebugLogThread" }.Start();
            new Thread(RTHandle) { IsBackground = true, Name = "DebugLogThread" }.Start();

            while (workerThreads > 0)
            {
                workerThreads--;
                new Thread(UdpUpdate) { IsBackground = true, Name = "UdpUpdate " + workerThreads }.Start();//创建UDP每一帧线程
            }

            var scene = OnAddDefaultScene();
            DefaultScene = scene.Key;
            Scenes.TryAdd(scene.Key, scene.Value);
            OnStartupCompletedHandle();
            logList.Add("服务器启动成功!");
        }

        /// <summary>
        /// 流量统计线程
        /// </summary>
        protected void DebugThread()
        {
            while (IsRunServer)
            {
                Thread.Sleep(1000);
                try
                {
                    OnNetworkDataTraffic?.Invoke(sendAmount, sendCount, receiveAmount, receiveCount, resolveAmount);
                }
                finally
                {
                    sendCount = 0;
                    sendAmount = 0;
                    resolveAmount = 0;
                    receiveAmount = 0;
                    receiveCount = 0;
                }
            }
        }

        /// <summary>
        /// 调式输出信息线程
        /// </summary>
        protected void DebugLogThread()
        {
            while (IsRunServer)
            {
                Thread.Sleep(1);
                try
                {
                    while (logList.Count > 0)
                    {
                        Log?.Invoke(logList[0]);
                        logList.RemoveAt(0);
                    }
                }
                catch { }
            }
        }
        
        /// <summary>
        /// Udp每一帧
        /// </summary>
        private void UdpUpdate()
        {
            try
            {
                if (!IsRunServer)
                    return;
                EndPoint remotePoint = Server.LocalEndPoint;
                byte[] buffer = new byte[65507];
                Server.BeginReceiveFrom(buffer, 0, buffer.Length, 0, ref remotePoint, callback =>
                {
                    try
                    {
                        int count = Server.EndReceiveFrom(callback, ref remotePoint);
                        receiveAmount++;
                        receiveCount += count;
                        UdpHandle(buffer, count, remotePoint);
                    }
                    catch (Exception ex)
                    {
                        logList.Add($"接收异常:{ex}");
                    }
                    finally
                    {
                        UdpUpdate();
                    }
                }, null);
            }
            catch (Exception ex)
            {
                logList.Add($"接收异常:{ex}");
                UdpUpdate();
            }
        }
        
        //udp数据处理线程
        private void UdpHandle(byte[] buffer, int count, EndPoint remotePoint)
        {
            if (Clients.ContainsKey(remotePoint))//在线客户端
            {
                ResolveBuffer(Clients[remotePoint], buffer, 0, count);//处理缓冲区数据
                return;
            }
            if (buffer[0] == NetCmd.QuitGame & count == frame)//退出程序指令
                return;
            if (buffer[0] == NetCmd.SendHeartbeat & count == 1 | buffer[0] == NetCmd.SendHeartbeat & count == 5)//buffer[0]=5:连接或心跳Ping指令 count=1:连接指令 count=5：心跳指令
            {
                Server.SendTo(new byte[] { 6, 0, 0, 0, 0}, remotePoint);//心跳回应 或 连接回应
                return;
            }
            if (UnClients.ContainsKey(remotePoint))//未知客户端
            {
                int removeUnClient = ResolveUnBuffer(UnClients[remotePoint], remotePoint, buffer, count);
                if (removeUnClient == -1)//如果允许未知客户端进入服务器，则可以将此客户端从未知客户端字典种移除，并且添加此客户端到在线玩家集合中
                    UnClients.TryRemove(remotePoint, out Player unClient1);
                return;
            }
            if (UnClients.Count >= LineUp)//排队人数
            {
                exceededNumber++;
                OnExceededNumber(remotePoint);
                return;
            }
            if (Clients.Count >= OnlineLimit)//服务器最大在线人数
            {
                blockConnection++;
                OnBlockConnection(remotePoint);
                return;
            }
            exceededNumber = 0;
            blockConnection = 0;
            Player unClient = new Player();
            unClient.RemotePoint=remotePoint;
            unClient.LastTime = DateTime.Now.AddMinutes(5);
            OnHasConnectHandle?.Invoke(unClient);
            logList.Add("有客户端连接:" + remotePoint.ToString());
            int addUnClient = ResolveUnBuffer(unClient, remotePoint, buffer, count);//解析未知客户端数据
            if (addUnClient == 1)//如果需要添加未知客户端，则进行添加客户端， 为了下次访问时不需要重新创建对象， 性能优化
                UnClients.TryAdd(remotePoint, unClient);
        }

        /// <summary>
        /// 当服务器连接人数溢出时调用
        /// </summary>
        /// <param name="remotePoint"></param>
        private void OnExceededNumber(EndPoint remotePoint)
        {
            logList.Add("未知客户端排队爆满,阻止连接人数: " + exceededNumber);
            Server.SendTo(new byte[] { NetCmd.ExceededNumber, 0, 0, 0, 0 }, 0, remotePoint);
        }

        /// <summary>
        /// 当服务器爆满时调用
        /// </summary>
        /// <param name="remotePoint"></param>
        private void OnBlockConnection(EndPoint remotePoint)
        {
            logList.Add("服务器爆满,阻止连接人数: " + blockConnection);
            Server.SendTo(new byte[] { NetCmd.BlockConnection, 0, 0, 0, 0 }, 0, remotePoint);
        }

        /// <summary>
        /// 解析未知客户端数据缓冲区， 返回1：未知客户端表达没有通过，将此客户端添加到UnClients集合里. 返回-1：允许未知客户端进服务器，与其他客户端进行互动， 返回2：CC流量攻击行为
        /// </summary>
        /// <param name="unPlayer"></param>
        /// <param name="remotePoint"></param>
        /// <param name="buffer"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        protected virtual int ResolveUnBuffer(Player unPlayer, EndPoint remotePoint, byte[] buffer, int count)
        {
            byte cmd = buffer[0];//[0] = 网络命令整形数据
            int size = BitConverter.ToInt32(buffer, 1);// {[1],[2]} 网络数据长度大小
            if (size + frame != count)//如果数据协议不正确退出
                return 2;
            Player unClient = null;
            if (cmd == NetCmd.QuitGame)
                return -1;
            if (cmd == NetCmd.EntityRpc)
                unClient = unPlayer.OnUnClientRequest(cmd, buffer, frame, count - frame) as Player;
            else
                unClient = OnUnClientRequest(unPlayer, cmd, buffer, frame, count - frame);
            if (unClient != null)//当有客户端连接时,如果允许用户添加此客户端
            {
                if (!Scenes.ContainsKey(unClient.sceneID))//如果非法场景ID则使用默认场景ID
                    unClient.sceneID = DefaultScene;
                unClient.RemotePoint = remotePoint;//防止旧的端口号
                unClient.heart = 0;//心跳初始化
                Clients.TryAdd(remotePoint, unClient);//将网络玩家添加到集合中
                if (unClient.playerID == string.Empty)
                    unClient.playerID = Share.Random.Range(1000000, 9999999).ToString();
                Players.TryAdd(unClient.playerID, unClient);//将网络玩家添加到集合中
                OnAddPlayerToScene(unClient);
                unClient.AddRpc(unClient);
                OnAddClientHandle?.Invoke(unClient);
                return -1;
            }
            return 1;
        }
        
        /// <summary>
        /// 解析网络数据包
        /// </summary>
        /// <param name="client">当前客户端</param>
        /// <param name="buffer">数据缓冲区</param>
        /// <param name="index">数据开始位置</param>
        /// <param name="count">数据大小</param>
        private void ResolveBuffer(Player client, byte[] buffer, int index, int count)
        {
            if (IsReceBigData)
            {
                while (index < count)
                {
                    byte cmd = buffer[index];//0 = 网络命令整形数据
                    int size = BitConverter.ToInt32(buffer, index + 1);//1+4=5 帧头
                    index += frame;
                    OnRevdBufferHandle?.Invoke(client, cmd, buffer, index, size);
                    index += size;
                }
            }
            else
            {
                byte cmd = buffer[index];//0 = 网络命令整形数据
                int size = BitConverter.ToInt32(buffer, index + 1);//1+4=5 帧头
                index += frame;
                if (index + size == count)//如果数据完整 防止CC流量攻击
                    OnRevdBufferHandle?.Invoke(client, cmd, buffer, index, size);
            }
        }

        /// <summary>
        /// 当处理缓冲区数据
        /// </summary>
        /// <param name="client">处理此客户端的数据请求</param>
        /// <param name="cmd">网络命令</param>
        /// <param name="buffer">缓冲区数据</param>
        /// <param name="index">数据开始索引</param>
        /// <param name="count">数据长度</param>
        protected virtual void ReceiveBufferHandle(Player client, byte cmd, byte[] buffer, int index, int count)
        {
            resolveAmount++;
            try
            {
                switch (cmd)
                {
                    case NetCmd.EntityRpc:
                        NetConvert.Deserialize(buffer, index, count, (func, pars) => {
                            if (client.Rpcs.ContainsKey(func))
                                client.OnRpcExecute(func, pars);
                        });
                        break;
                    case NetCmd.CallRpc:
                        NetConvert.Deserialize(buffer, index, count, (func, pars) => {
                            OnRpcExecute(client, func, pars);
                        });
                        break;
                    case NetCmd.LocalCmd:
                        Send(client, buffer, index - frame, count + frame);//发送数据到这个客户端
                        break;
                    case NetCmd.SceneCmd:
                        Scene scene = client.Scene as Scene;
                        if (scene == null)
                            return;
                        Parallel.ForEach(scene.Players, player => //并行当前场景的客户端
                        {
                            if (player == null)
                                return;
                            Send(player, buffer, index - frame, count + frame);//发送数据到这个客户端
                        });
                        break;
                    case NetCmd.AllCmd:
                        Parallel.ForEach(Clients, player => //并行所有在线玩家
                        {
                            if (player.Value == null)
                                return;
                            Send(player.Value, buffer, index - frame, count + frame);//发送数据到这个客户端
                        });
                        break;
                    case NetCmd.SendHeartbeat:
                        Send(client, NetCmd.RevdHeartbeat, new byte[1]);
                        break;
                    case NetCmd.RevdHeartbeat:
                        client.heart = 0;
                        break;
                    case NetCmd.QuitGame:
                        if (Clients.TryRemove(client.RemotePoint, out Player quitClient))
                            OnRemoveClientEvent?.Invoke(quitClient);
                        Players.TryRemove(client.playerID, out Player quitClient1);
                        if (Scenes.ContainsKey(client.sceneID))
                            Scenes[client.sceneID].Players.Remove(client);
                        quitClient?.Dispose();
                        quitClient = null;
                        break;
                    case NetCmd.SafeCall:
                        NetConvert.Deserialize(buffer, index, count, (func, pars) => {
                            OnRpcExecute(client, func, pars);
                        });
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
                        ReliableTransport(client, cmd, kernel, rtID, rtIndex, rtLength, index, buffer, count + frame, pross, true, (buff) => {
                            if (kernel == 1) {
                                OnRevdRTDataBuffer?.Invoke(client, buff);
                                return;
                            }
                            if (kernel == 2 | kernel == 3) {
                                ResolvingRT(buff, (func, pars, buff1) => {
                                    List<object> list = new List<object>(pars) { buff1 };
                                    OnRpcExecute(client, func, list.ToArray());
                                });
                                return;
                            }
                            var func1 = NetConvert.Deserialize(buff);
                            OnRpcExecute(client, func1.name, func1.pars);
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
                        ReliableTransport(client, cmd, kernel1, rtID1, rtIndex1, rtLength1, index, buffer, count + frame, pross1, false, (buff) => {
                            if (kernel1 == 1) {
                                client.OnRevdRTDataBuffer(buff);
                                return;
                            }
                            if (kernel1 == 2 | kernel1 == 3) {
                                ResolvingRT(buff, (func, pars, buff1) => {
                                    if (client.Rpcs.ContainsKey(func)) {
                                        NetDelegate rpc = client.Rpcs[func];
                                        List<object> list = new List<object>(pars) { buff1 };
                                        rpc.method.Invoke(rpc.target, list.ToArray());
                                    }
                                });
                                return;
                            }
                            var func1 = NetConvert.Deserialize(buff);
                            if (client.Rpcs.ContainsKey(func1.name)) {
                                NetDelegate rpc = client.Rpcs[func1.name];
                                rpc.method.Invoke(rpc.target, func1.pars);
                            }
                        });
                        break;
                    case NetCmd.ReliableCallback:
                        NetConvert.Deserialize(buffer, index, count, (func, pars) => {
                            int key = (int)pars[0];
                            int downIndex = (int)pars[1];
                            bool done = (bool)pars[2];
                            byte rtCmd = (byte)pars[3];
                            if (!client.sendRTDic.ContainsKey(key))
                                return;
                            var rt = client.sendRTDic[key];
                            if (done)
                            {
                                rt.buffer.Clear();
                                rt.datas = null;
                                client.sendRTDic.Remove(key);
                                rt.progress?.Invoke(100, RTState.Complete);
                                GC.Collect();
                                return;
                            }
                            rt.index = downIndex;
                            rt.tryIndex = 0;
                            SendRT(client, rt);
                            var value = (float)downIndex / rt.buffer.Count * 100;
                            rt.progress?.Invoke(value, RTState.Sending);
                        });
                        break;
                    default:
                        client.OnRevdCustomBuffer(cmd, buffer, index, count);
                        OnRevdCustomBufferHandle?.Invoke(client, cmd, buffer, index, count);
                        break;
                }
            }
            catch (Exception ex)
            {
                logList.Add("解析异常:" + ex);
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

        //发送可靠数据
        void SendRT(Player client, SendRTBuffer sendRT)
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
                Send(client, buffer, 0, buffer.Length);
            }
            sendRT.time = DateTime.Now.AddSeconds(sendRT.buffer.Count);
        }

        /// <summary>
        /// 可靠传输数据缓冲区
        /// </summary>
        /// <param name="client"></param>
        /// <param name="buffer"></param>
        public delegate void RevdRTDataBuffer(Player client, byte[] buffer);
        /// <summary>
        /// 当接收可靠传输数据缓冲
        /// </summary>
        public RevdRTDataBuffer OnRevdRTDataBuffer;

        //可靠传输处理
        void ReliableTransport(Player client, byte cmd, int kernel, int rtID, int rtIndex, int rtLength, int index, byte[] buffer, int count, int pross, bool serOrEnty, Action<byte[]> call)
        {
            RevdRTBuffer revdRT;
            if (!client.revdRTDic.ContainsKey(rtID))
            {
                revdRT = new RevdRTBuffer();
                client.revdRTDic.Add(rtID, revdRT);
            }
            revdRT = client.revdRTDic[rtID];
            if (revdRT.index == rtIndex)
            {
                if (kernel == 3) {
                    NetConvert.Deserialize(buffer, index, pross, (func, objs) => {
                        var pross1 = (int)((float)revdRT.stream.Length / rtLength * 100);
                        List<object> list = new List<object>(objs) { pross1 };
                        if(serOrEnty) 
                            OnRpcExecute(client, func, list);
                        else if (client.Rpcs.ContainsKey(func))
                            client.Rpcs[func].method.Invoke(client, list.ToArray());
                    });
                }
                revdRT.stream.Write(buffer, index + pross, count - (index + pross));
                if (revdRT.stream.Length >= rtLength)
                {
                    Send(client, NetCmd.ReliableCallback, string.Empty, rtID, revdRT.index, true, cmd);
                    byte[] buff = revdRT.stream.ToArray();
                    revdRT.stream.Dispose();
                    client.revdRTDic.Remove(rtID);
                    call(buff);
                    GC.Collect();
                }
                else
                {
                    revdRT.index++;
                    Send(client, NetCmd.ReliableCallback, string.Empty, rtID, revdRT.index, false, cmd);
                }
            }
            else
            {
                Send(client, NetCmd.ReliableCallback, string.Empty, rtID, revdRT.index, false, cmd);
            }
        }

        /// <summary>
        /// 发送可靠网络传输, 可以发送大型文件数据
        /// 调用此方法通常情况下是一定把数据发送成功为止, (尽量少用)
        /// </summary>
        /// <param name="client">发送到客户端</param>
        /// <param name="func">函数名</param>
        /// <param name="pars">参数</param>
        public void SendRT(Player client, string func, params object[] pars)
        {
            SendRT(client, RTCmd.PublicRT, func, null, pars);
        }

        /// <summary>
        /// 发送可靠网络传输, 可以发送大型文件数据
        /// 调用此方法通常情况下是一定把数据发送成功为止, (尽量少用)
        /// </summary>
        /// <param name="client">发送到客户端</param>
        /// <param name="func">函数名</param>
        /// <param name="progress">当前传输进度</param>
        /// <param name="pars">参数</param>
        public void SendRT(Player client, string func, SendRTProgress progress, params object[] pars)
        {
            SendRT(client, RTCmd.PublicRT, func, progress, pars);
        }

        /// <summary>
        /// 发送可靠网络传输, 可以发送大型文件数据
        /// 调用此方法通常情况下是一定把数据发送成功为止, (尽量少用)
        /// </summary>
        /// <param name="client">发送到客户端</param>
        /// <param name="cmd">网络命令</param>
        /// <param name="func">函数名</param>
        /// <param name="pars">参数</param>
        public void SendRT(Player client, RTCmd cmd, string func, params object[] pars)
        {
            SendRT(client, cmd, func, null, pars);
        }

        /// <summary>
        /// 发送可靠网络传输, 可以发送大型文件数据
        /// 调用此方法通常情况下是一定把数据发送成功为止, (尽量少用)
        /// </summary>
        /// <param name="client">发送到客户端</param>
        /// <param name="cmd">网络命令</param>
        /// <param name="func">函数名</param>
        /// <param name="progress">当前数据进度</param>
        /// <param name="pars">参数</param>
        public void SendRT(Player client, RTCmd cmd, string func, SendRTProgress progress, params object[] pars)
        {
            var sendRT = new SendRTBuffer(cmd, func, progress, pars);
            SendRTData(client, sendRT);
        }

        /// <summary>
        /// 发送可靠网络传输, 可以发送大型文件数据
        /// 调用此方法通常情况下是一定把数据发送成功为止, (尽量少用)
        /// </summary>
        /// <param name="client"></param>
        /// <param name="cmd">网络命令</param>
        /// <param name="progress">当前数据进度</param>
        /// <param name="buffers"></param>
        public void SendRT(Player client, RTCmd cmd, SendRTProgress progress, byte[] buffers)
        {
            var sendRT = new SendRTBuffer(cmd, progress, buffers);
            SendRTData(client, sendRT);
        }

        /// <summary>
        /// 发送解释型可靠传输流, 带有命令, 解释对象, 大型数据 注意:此方法跟前面两种方法不同
        /// 使用此方法发送数据, 在客户端声明的rpc方法必须有(byte[] buffer)参数放在后面
        /// </summary>
        /// <param name="client"></param>
        /// <param name="cmd"></param>
        /// <param name="progress"></param>
        /// <param name="buffer"></param>
        /// <param name="func"></param>
        /// <param name="pars"></param>
        public void SendERT(Player client, RTCmd cmd, SendRTProgress progress, byte[] buffer, string func, params object[] pars)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                var entyBts = NetConvert.Serialize(func, pars);
                stream.Write(BitConverter.GetBytes(entyBts.Length), 0, 4);
                stream.Write(entyBts, 0, entyBts.Length);
                stream.Write(BitConverter.GetBytes(buffer.Length), 0, 4);
                stream.Write(buffer, 0, buffer.Length);
                var sendRT = new SendRTBuffer(cmd, progress, stream.ToArray()) { kernel = 2 };
                SendRTData(client, sendRT);
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
        public void SendERTP(Player client, RTCmd cmd, SendRTProgress progress, byte[] buffer, string func, string progressFunc, object progressItem, params object[] pars)
        {
            using (MemoryStream stream = new MemoryStream()) {
                var entyBts = NetConvert.Serialize(func, pars);
                stream.Write(BitConverter.GetBytes(entyBts.Length), 0, 4);
                stream.Write(entyBts, 0, entyBts.Length);
                var entyBts1 = NetConvert.Serialize(progressFunc, progressItem);
                stream.Write(BitConverter.GetBytes(buffer.Length), 0, 4);
                stream.Write(buffer, 0, buffer.Length);
                var sendRT = new SendRTBuffer(cmd, progress, stream.ToArray()) { kernel = 3, progressData = entyBts1 };
                SendRTData(client, sendRT);
            }
        }

        //发送可靠传输数据
        void SendRTData(Player client, SendRTBuffer sendRTBuffer)
        {
            new Task((asynSendRT) =>
            {
                try
                {
                    SendRTBuffer sendRT = asynSendRT as SendRTBuffer;
                    byte[] buffer;
                    if (sendRT.kernel == 0)
                        buffer = NetConvert.Serialize(sendRT.func, sendRT.pars);
                    else
                        buffer = sendRT.datas;
                    sendRT.count = buffer.Length;
                JUMP: int keyID = Share.Random.Range(100000000, 999999999);
                    if (client.sendRTDic.ContainsKey(keyID))
                        goto JUMP;
                    sendRT.keyID = keyID;
                    client.sendRTDic.Add(keyID, sendRT);
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
                    logList.Add("可靠发送异常:" + ex);
                }
            }, sendRTBuffer).Start();
        }

        //可靠传输检测线程
        void RTHandle()
        {
            while (IsRunServer)
            {
                Thread.Sleep(200);
                try
                {
                    foreach (var client in Clients.Values)
                    {
                        foreach (var rt in client.sendRTDic.Values)
                        {
                            if (DateTime.Now > rt.time & rt.tryIndex > 5)
                            {
                                rt.progress?.Invoke(0, RTState.FailSend);
                                rt.buffer.Clear();
                                rt.datas = null;
                                client.revdRTDic.Remove(rt.keyID);
                                GC.Collect();
                            }
                            else if (DateTime.Now > rt.time)
                            {
                                rt.tryIndex++;
                                SendRT(client, rt);
                                var value = (float)rt.index / rt.buffer.Count * 100;
                                rt.progress?.Invoke(value, RTState.Retransmission);
                            }
                            else if (rt.index <= rt.pasIndex)
                            {
                                SendRT(client, rt);
                            }
                            break;//注意:排帧发送,前帧不走后面帧不能走
                        }
                    }
                }
                catch (Exception ex)
                {
                    logList.Add("可靠异常:" + ex);
                }
            }
        }

        /// <summary>
        /// 当执行Rpc(远程过程调用函数)时, 如果IOS不支持反射, 需要重写此方法进行指定要调用的函数
        /// </summary>
        /// <param name="client">客户端</param>
        /// <param name="func">函数名</param>
        /// <param name="pars">参数</param>
        protected virtual void OnRpcExecute(Player client, string func, params object[] pars)
        {
            Parallel.For(0, Rpcs.Count, (i) =>
            {
                if (Rpcs[i].method.Name != func)
                    return;
                try
                {
                    if (Rpcs[i].cmd == NetCmd.SafeCall)
                    {
                        var pars1 = new List<object>(pars.Length + 1) { client };
                        pars1.AddRange(pars);
                        Rpcs[i].method.Invoke(Rpcs[i].target, pars1.ToArray());
                    }
                    else
                    {
                        OnInvokeRpc(client);
                        Rpcs[i].method.Invoke(Rpcs[i].target, pars);
                    }
                }
                catch (Exception e)
                {
                    string str = "方法:" + Rpcs[i].method + " 参数:";
                    if (pars == null)
                        str += "null";
                    else
                    {
                        foreach (var p in pars)
                        {
                            if (p == null)
                                str += "Null , ";
                            else
                                str += p + " , ";
                        }
                    }
                    logList.Add(str + " -> " + e);
                }
            });
        }

        /// <summary>
        /// 未知客户端心跳包线程
        /// </summary>
        protected void UnHeartUpdate()
        {
            while (IsRunServer)
            {
                Thread.Sleep(HeartTime);
                try
                {
                    UnHeartHandle();
                }
                catch (Exception ex)
                {
                    logList.Add("未知心跳异常: " + ex);
                }
            }
        }

        /// <summary>
        /// 未知客户端心跳处理
        /// </summary>
        protected virtual void UnHeartHandle()
        {
            foreach (var client in UnClients)
            {
                if (client.Value == null)
                    goto GO;
                if (client.Value.heart < 5 & DateTime.Now < client.Value.LastTime)//有5次确认心跳包
                {
                    client.Value.heart++;
                    Send(client.Value, NetCmd.SendHeartbeat, new byte[1]);
                    continue;
                }
            GO: if (UnClients.TryRemove(client.Key, out Player recClient))
                    recClient?.Dispose();
                logList.Add("移除未知客户端:" + client.Key.ToString());
                recClient = null;
            }
        }

        /// <summary>
        /// 心跳时间, 默认每5秒检查一次玩家是否离线, 玩家心跳确认为5次, 如果超出5次 则移除玩家客户端. 确认玩家离线总用时25秒, 
        /// 如果设置的值越小, 确认的速度也会越快. 但发送的数据也会增加.
        /// </summary>
        protected int HeartTime { get; set; } = 50000000;

        /// <summary>
        /// 心跳包线程
        /// </summary>
        protected void HeartUpdate()
        {
            while (IsRunServer)
            {
                Thread.Sleep(HeartTime);
                try
                {
                    HeartHandle();
                }
                catch (Exception ex)
                {
                    logList.Add("心跳异常: " + ex);
                }
            }
        }

        /// <summary>
        /// 心跳处理
        /// </summary>
        protected virtual void HeartHandle()
        {
            foreach (var client in Clients)
            {
                if (client.Value == null)
                    goto GO;
                if (client.Value.heart < 5)//有5次确认心跳包
                {
                    client.Value.heart++;
                    Send(client.Value, NetCmd.SendHeartbeat, new byte[1]);
                    continue;
                }
            GO: if (Clients.TryRemove(client.Key, out Player recClient))
                    OnRemoveClientEvent?.Invoke(recClient);
                if (client.Value != null)
                {
                    Players.TryRemove(client.Value.playerID, out Player recClient1);
                    if (Scenes.ContainsKey(client.Value.sceneID))
                        Scenes[client.Value.sceneID].Players.Remove(client.Value);
                    logList.Add("移除客户端: 玩家ID:" + client.Value?.playerID + " 玩家终端: " + client.Key.ToString());
                }
                recClient.OnRemoveClient();
                recClient?.Dispose();
                recClient = null;
            }
        }

        /// <summary>
        /// 创建网络场景, 退出当前场景,进入所创建的场景 - 创建场景成功返回场景对象， 创建失败返回null
        /// </summary>
        /// <param name="player">创建网络场景的玩家实体</param>
        /// <param name="sceneID">要创建的场景号或场景名称</param>
        /// <returns></returns>
        public Scene CreateScene(Player player, string sceneID)
        {
            return CreateScene(player, sceneID, new Scene());
        }

        /// <summary>
        /// 创建网络场景, 退出当前场景并加入所创建的场景 - 创建场景成功返回场景对象， 创建失败返回null
        /// </summary>
        /// <param name="player">创建网络场景的玩家实体</param>
        /// <param name="sceneID">要创建的场景号或场景名称</param>
        /// <param name="scene">创建场景的实体</param>
        /// <param name="exitCurrentSceneCall">即将退出当前场景的处理委托函数: 如果你需要对即将退出的场景进行一些事后处理, 则在此委托函数执行! 如:退出当前场景通知当前场景内的其他客户端将你的玩家对象移除等功能</param>
        /// <returns></returns>
        public Scene CreateScene(Player player, string sceneID, Scene scene, Action<Scene> exitCurrentSceneCall = null)
        {
            if (!Scenes.ContainsKey(sceneID))
            {
                if(Scenes.ContainsKey(player.sceneID)){
                    var exitScene = Scenes[player.sceneID];
                    exitScene.Players.Remove(player);
                    exitCurrentSceneCall?.Invoke(exitScene);
                }
                Scenes.TryAdd(sceneID, scene as Scene);
                if (!scene.Players.Contains(player))
                    scene.Players.Add(player);
                player.Scene = scene;
                player.sceneID = sceneID;
                return scene;
            }
            return null;
        }

        /// <summary>
        /// 退出当前场景,加入指定的场景 - 成功进入返回场景对象，进入失败返回null
        /// </summary>
        /// <param name="player">要进入sceneID场景的玩家实体</param>
        /// <param name="sceneID">场景ID，要切换到的场景号或场景名称</param>
        /// <param name="exitCurrentSceneCall">即将退出当前场景的处理委托函数: 如果你需要对即将退出的场景进行一些事后处理, 则在此委托函数执行! 如:退出当前场景通知当前场景内的其他客户端将你的玩家对象移除等功能</param>
        /// <returns></returns>
        public Scene JoinScene(Player player, string sceneID, Action<Scene> exitCurrentSceneCall = null) => SwitchScene(player, sceneID, exitCurrentSceneCall);

        /// <summary>
        /// 进入场景 - 成功进入返回true，进入失败返回false
        /// </summary>
        /// <param name="player">要进入sceneID场景的玩家实体</param>
        /// <param name="sceneID">场景ID，要切换到的场景号或场景名称</param>
        /// <param name="exitCurrentSceneCall">即将退出当前场景的处理委托函数: 如果你需要对即将退出的场景进行一些事后处理, 则在此委托函数执行! 如:退出当前场景通知当前场景内的其他客户端将你的玩家对象移除等功能</param>
        /// <returns></returns>
        public Scene EnterScene(Player player, string sceneID, Action<Scene> exitCurrentSceneCall = null) => SwitchScene(player, sceneID, exitCurrentSceneCall);

        /// <summary>
        /// 退出当前场景,切换到指定的场景 - 成功进入返回true，进入失败返回false
        /// </summary>
        /// <param name="player">要进入sceneID场景的玩家实体</param>
        /// <param name="sceneID">场景ID，要切换到的场景号或场景名称</param>
        /// <param name="exitCurrentSceneCall">即将退出当前场景的处理委托函数: 如果你需要对即将退出的场景进行一些事后处理, 则在此委托函数执行! 如:退出当前场景通知当前场景内的其他客户端将你的玩家对象移除等功能</param>
        /// <returns></returns>
        public Scene SwitchScene(Player player, string sceneID, Action<Scene> exitCurrentSceneCall = null)
        {
            if (Scenes.ContainsKey(sceneID))
            {
                if(Scenes.ContainsKey(player.sceneID)){
                    var scene = Scenes[player.sceneID];
                    scene.Players.Remove(player);
                    exitCurrentSceneCall?.Invoke(scene);
                }
                Scenes[sceneID].Players.Add(player);
                player.sceneID = sceneID;
                player.Scene = Scenes[sceneID];
                return player.Scene as Scene;
            }
            return null;
        }

        /// <summary>
        /// 退出场景
        /// </summary>
        /// <param name="player"></param>
        /// <param name="isEntMain">退出当前场景是否进入主场景: 默认进入主场景</param>
        /// <param name="exitCurrentSceneCall">即将退出当前场景的处理委托函数: 如果你需要对即将退出的场景进行一些事后处理, 则在此委托函数执行! 如:退出当前场景通知当前场景内的其他客户端将你的玩家对象移除等功能</param>
        public void ExitScene(Player player, bool isEntMain = true, Action<Scene> exitCurrentSceneCall = null)
        {
            RemoveScenePlayer(player, isEntMain);
        }

        /// <summary>
        /// 移除服务器场景. 从服务器总场景字典中移除指定的场景: 当你移除指定场景后,如果场景内有其他玩家在内, 则把其他玩家添加到主场景内
        /// </summary>
        /// <param name="sceneID">要移除的场景id</param>
        /// <param name="addToMainScene">允许即将移除的场景内的玩家添加到主场景?</param>
        /// <param name="exitCurrentSceneCall">即将退出当前场景的处理委托函数: 如果你需要对即将退出的场景进行一些事后处理, 则在此委托函数执行! 如:退出当前场景通知当前场景内的其他客户端将你的玩家对象移除等功能</param>
        /// <returns></returns>
        public bool RemoveScene(string sceneID, bool addToMainScene = true, Action<Scene> exitCurrentSceneCall = null)
        {
            if (sceneID == DefaultScene)
                throw new DirectoryNotFoundException("不允许删除主场景, 当你删除主场景后, 新的客户端将无法进入服务器了!");

            if (Scenes.ContainsKey(sceneID))
            {
                Scenes.TryRemove(sceneID, out Scene scene);
                exitCurrentSceneCall?.Invoke(scene);
                if (addToMainScene) {
                    foreach (var p in scene.Players) {
                        p.sceneID = DefaultScene;
                        p.Scene = Scenes[DefaultScene];
                    }
                }else{
                    foreach (var p in scene.Players) {
                        p.sceneID = "";
                        p.Scene = null;
                    }
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 将玩家从当前所在的场景移除掉， 移除之后此客户端将会进入默认主场景
        /// </summary>
        /// <param name="player">要执行的玩家实体</param>
        /// <param name="isEntMain">退出当前场景是否进入主场景: 默认进入主场景</param>
        /// <param name="exitCurrentSceneCall">即将退出当前场景的处理委托函数: 如果你需要对即将退出的场景进行一些事后处理, 则在此委托函数执行! 如:退出当前场景通知当前场景内的其他客户端将你的玩家对象移除等功能</param>
        /// <returns></returns>
        public bool RemoveScenePlayer(Player player, bool isEntMain = true, Action<Scene> exitCurrentSceneCall = null)
        {
            if (Scenes.ContainsKey(player.sceneID))
            {
                var scene = Scenes[player.sceneID];
                scene.Players.Remove(player);
                exitCurrentSceneCall?.Invoke(scene);
                if(isEntMain){
                    Scenes[DefaultScene].Players.Add(player);
                    player.sceneID = DefaultScene;
                    player.Scene = Scenes[DefaultScene];
                }else{
                    player.sceneID = "";
                    player.Scene = null;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 从所有在线玩家字典中删除(移除)玩家实体
        /// </summary>
        /// <param name="player"></param>
        public void DeletePlayer(Player player) => RemoveClient(player);

        /// <summary>
        /// 从所有在线玩家字典中移除玩家实体
        /// </summary>
        /// <param name="player"></param>
        public void RemovePlayer(Player player) => RemoveClient(player);

        /// <summary>
        /// 从客户端字典中移除客户端
        /// </summary>
        /// <param name="client"></param>
        public virtual void RemoveClient(Player client)
        {
            if (Clients.TryRemove(client.RemotePoint, out Player reClient))
                OnRemoveClientEvent?.Invoke(reClient);
            Players.TryRemove(client.playerID, out Player reClient1);
            if (Scenes.ContainsKey(client.sceneID))
            {
                var players = Scenes[client.sceneID].Players;
                players.Remove(client);
            }
            reClient?.Dispose();
            reClient = null;
        }

        /// <summary>
        /// 场景是否存在?
        /// </summary>
        /// <param name="sceneID"></param>
        /// <returns></returns>
        public bool IsHasScene(string sceneID)
        {
            return Scenes.ContainsKey(sceneID);
        }

        /// <summary>
        /// 关闭服务器
        /// </summary>
        public virtual void Close()
        {
            IsRunServer = false;
            Server?.Dispose();
            Server?.Close();
            Instance?.Server?.Close();
            Instance = null;
        }

        /// <summary>
        /// 发送网络数据 UDP发送方式
        /// </summary>
        /// <param name="client">发送数据到的客户端</param>
        /// <param name="buffer">数据缓冲区</param>
        public void Send(Player client, byte[] buffer)
        {
            Send(client, NetCmd.OtherCmd, buffer);
        }

        /// <summary>
        /// 发送网络数据 UDP发送方式
        /// </summary>
        /// <param name="client">发送数据到的客户端</param>
        /// <param name="fun">RPCFun函数</param>
        /// <param name="pars">RPCFun参数</param>
        public void Send(Player client, string fun, params object[] pars)
        {
            Send(client, NetCmd.CallRpc, fun, pars);
        }

        /// <summary>
        /// 网络多播, 发送数据到clients集合的客户端（并发, 有可能并行发送）
        /// </summary>
        /// <param name="clients">客户端集合</param>
        /// <param name="func">本地客户端rpc函数</param>
        /// <param name="pars">本地客户端rpc参数</param>
        public void Multicast(HashSet<Player> clients, string func, params object[] pars)
        {
            Multicast(clients, NetCmd.CallRpc, func, pars);
        }

        /// <summary>
        /// 网络多播, 发送自定义数据到clients集合的客户端（并发, 有可能并行发送）
        /// </summary>
        /// <param name="clients">客户端集合</param>
        /// <param name="buffer">自定义字节数组</param>
        public void Multicast(HashSet<Player> clients, byte[] buffer)
        {
            Multicast(clients, NetCmd.CallRpc, buffer);
        }

        /// <summary>
        /// 发送自定义网络数据
        /// </summary>
        /// <param name="client">发送到客户端</param>
        /// <param name="cmd">网络命令</param>
        /// <param name="buffer">数据缓冲区</param>
        public void Send(Player client, byte cmd, byte[] buffer)
        {
            buffer = Packing(cmd, buffer);
            Send(client, buffer, 0, buffer.Length);
        }

        /// <summary>
        /// 发送网络数据
        /// </summary>
        /// <param name="client">发送到的客户端</param>
        /// <param name="cmd">网络命令</param>
        /// <param name="func">RPCFun函数</param>
        /// <param name="pars">RPCFun参数</param>
        public void Send(Player client, byte cmd, string func, params object[] pars)
        {
            byte[] buffer = Packing(cmd, NetConvert.Serialize(func, pars));
            Send(client, buffer, 0, buffer.Length);
        }

        /// <summary>
        /// 网络多播, 发送数据到clients集合的客户端（并发, 有可能并行发送）
        /// </summary>
        /// <param name="clients">客户端集合</param>
        /// <param name="cmd">网络命令</param>
        /// <param name="func">本地客户端rpc函数</param>
        /// <param name="pars">本地客户端rpc参数</param>
        public void Multicast(HashSet<Player> clients, byte cmd, string func, params object[] pars)
        {
            byte[] buffer = Packing(cmd, NetConvert.Serialize(func, pars));
            Parallel.ForEach(clients, client =>
            {
                if (client == null)
                    return;
                Send(client, buffer, 0, buffer.Length);
            });
        }

        /// <summary>
        /// 网络多播, 发送自定义数据到clients集合的客户端（并发, 有可能并行发送）
        /// </summary>
        /// <param name="clients">客户端集合</param>
        /// <param name="cmd">网络命令</param>
        /// <param name="buffer">自定义字节数组</param>
        public void Multicast(HashSet<Player> clients, byte cmd, byte[] buffer)
        {
            buffer = Packing(cmd, buffer);
            Parallel.ForEach(clients, client =>
            {
                if (client == null)
                    return;
                Send(client, buffer, 0, buffer.Length);
            });
        }

        /// <summary>
        /// 网络统一包装
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        protected virtual byte[] Packing(byte cmd, byte[] buffer)
        {
            using (MemoryStream stream = new MemoryStream(buffer.Length + frame))
            {
                stream.WriteByte(cmd);
                stream.Write(BitConverter.GetBytes(buffer.Length), 0, 4);
                stream.Write(buffer, 0, buffer.Length);
                return stream.ToArray();
            }
        }

        /// <summary>
        /// 发送封装完成后的网络数据
        /// </summary>
        /// <param name="client">发送到的客户端</param>
        /// <param name="buffer">发送字节数组缓冲区</param>
        /// <param name="index">字节数组开始位置</param>
        /// <param name="count">字节数组长度</param>
        protected virtual void Send(Player client, byte[] buffer, int index, int count)
        {
            sendAmount++;
            sendCount += count;
            if (count < 65507 & index < count)
            {
                Server.SendTo(buffer, index, count, 0, client.RemotePoint);
            }
            else
            {
                logList.Add("数据太大!请使用SendERT或SendRT进行发送数据!");
            }
        }
    }
}