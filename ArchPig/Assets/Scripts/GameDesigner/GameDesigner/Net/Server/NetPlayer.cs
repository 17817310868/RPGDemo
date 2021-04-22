namespace Net.Server
{
    using Net.Entity;
    using Net.Share;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Reflection;
    
    /// <summary>
    /// 网络玩家 - 当客户端连接服务器后都会为每个客户端生成一个网络玩家对象，(玩家对象由服务器管理) 2019.9.9
    /// </summary>
    public class NetPlayer : IDisposable
    {
        /// <summary>
        /// Tcp套接字
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoIgnore]
        public Socket Client { get; set; }
        /// <summary>
        /// 存储UDP客户端终端
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoIgnore]
        public EndPoint RemotePoint { get; set; }
        /// <summary>
        /// 此玩家所在的场景ID
        /// </summary>
        public string sceneID = "MainScene";
        /// <summary>
        /// 客户端玩家的标识
        /// </summary>
        public string playerID = string.Empty;
        /// <summary>
        /// 玩家所在的场景实体
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoIgnore]
        public virtual object Scene { get; set; }
        /// <summary>
        /// 玩家rpc
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoIgnore]
        public Dictionary<string, NetDelegate> Rpcs { get; set; } = new Dictionary<string, NetDelegate>();
        /// <summary>
        /// 临时客户端持续时间: (内核使用):
        /// 未知客户端连接服务器, 长时间未登录账号, 未知客户端临时内存对此客户端回收, 并强行断开此客户端连接
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoIgnore]
        public DateTime LastTime { get; set; }
        /// <summary>
        /// 跳动的心
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoIgnore]
        internal byte heart = 0;
        /// <summary>
        /// 发送可靠数据缓冲
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoIgnore]
        internal Dictionary<int, SendRTBuffer> sendRTDic = new Dictionary<int, SendRTBuffer>();
        /// <summary>
        /// 接收可靠数据缓冲
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoIgnore]
        internal Dictionary<int, RevdRTBuffer> revdRTDic = new Dictionary<int, RevdRTBuffer>();
        /// <summary>
        /// TCP叠包值， 0:正常 >1:叠包次数 >25:清空叠包缓存流
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoIgnore]
        internal int stack = 0;
        /// <summary>
        /// TCP叠包临时缓存流
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoIgnore]
        internal MemoryStream StackStream { get; set; }
        /// <summary>
        /// 临时命令
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoIgnore]
        public byte Cmd { get; set; }
        
        #region 创建网络客户端(玩家)
        /// <summary>
        /// 构造网络客户端
        /// </summary>
        public NetPlayer(){ }

        /// <summary>
        /// 构造网络客户端，Tcp
        /// </summary>
        /// <param name="serverID">服务器的实例ID， 如果你的服务器有场景服务器，大厅服务器，公告服务器，数据库服务器的时候，就会用到guid来识别要使用哪个服务器实例来发送的数据</param>
        /// <param name="client">客户端套接字</param>
        public NetPlayer(Socket client)
        {
            Client = client;
            RemotePoint = client.RemoteEndPoint;
        }

        /// <summary>
        /// 构造网络客户端
        /// </summary>
        /// <param name="serverID">服务器的实例ID， 如果你的服务器有场景服务器，大厅服务器，公告服务器，数据库服务器的时候，就会用到guid来识别要使用哪个服务器实例来发送的数据</param>
        /// <param name="remotePoint"></param>
        public NetPlayer(EndPoint remotePoint)
        {
            RemotePoint= remotePoint;
        }
        #endregion

        #region 客户端释放内存
        /// <summary>
        /// 析构网络客户端
        /// </summary>
        ~NetPlayer()
        {
        }

        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            Client?.Close();
            StackStream?.Close();
        }
        #endregion

        #region 客户端(玩家)Rpc(远程过程调用)处理
        /// <summary>
        /// 添加远程过程调用函数,从对象进行收集
        /// </summary>
        /// <param name="target"></param>
        /// <param name="append">可以重复添加rpc?</param>
        public void AddRpc(bool append = false)
        {
            AddRpc(this, append);
        }

        /// <summary>
        /// 添加远程过程调用函数,从对象进行收集
        /// </summary>
        /// <param name="target"></param>
        /// <param name="append">可以重复添加rpc?</param>
        public void AddRpc(object target, bool append = false)
        {
            if (!append)
                foreach (var o in Rpcs.Values)
                    if (o.target == target)
                        return;
            
            foreach (MethodInfo info in target.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                var rpc = info.GetCustomAttribute<RPCFun>();
                if (rpc == null)
                    continue;
                if (Rpcs.ContainsKey(info.Name))
                {
                    //Server.logList.Add($"添加客户端私有Rpc错误！Rpc方法{info.Name}使用同一函数名，这是不允许的，字典键值无法添加相同的函数名！");
                    throw new TargetException($"添加客户端私有Rpc错误！Rpc方法{info.Name}使用同一函数名，这是不允许的，字典键值无法添加相同的函数名！");
                    //continue;
                }
                Rpcs.Add(info.Name, new NetDelegate(target, info, rpc.cmd));
            }
        }

        /// <summary>
        /// 移除网络远程过程调用函数
        /// </summary>
        /// <param name="target">移除的rpc对象</param>
        public void RemoveRpc(object target)
        {
            NetDelegate[] rpcs = new NetDelegate[Rpcs.Count];
            Rpcs.Values.CopyTo(rpcs, 0);
            foreach (var rpc in rpcs)
            {
                if (rpc.target == target | rpc.target.Equals(target) | rpc.target.Equals(null) | rpc.method.Equals(null))
                {
                    Rpcs.Remove(rpc.method.Name);
                }
            }
        }
        #endregion
        
        #region 客户端数据处理函数
        /// <summary>
        /// 当未知客户端发送数据请求，返回null，不添加到clients，返回对象，添加到clients中
        /// 客户端玩家的入口点，在这里可以控制客户端是否可以进入服务器与其他客户端进行网络交互
        /// 在这里可以用来判断客户端登录和注册等等进站许可
        /// </summary>
        /// <param name="buffer">缓冲区</param>
        /// <param name="cmd">命令</param>
        /// <param name="index">字节开始索引</param>
        /// <param name="count">字节长度</param>
        /// <returns></returns>
        public virtual NetPlayer OnUnClientRequest(byte cmd, byte[] buffer, int index, int count)
        {
            return this;
        }

        /// <summary>
        /// 当接收到客户端自定义数据请求,在这里可以使用你自己的网络命令，系列化方式等进行解析网络数据。（你可以在这里使用ProtoBuf或Json来解析网络数据）
        /// </summary>
        /// <param name="cmd">网络命令</param>
        /// <param name="buffer">数据缓冲区</param>
        /// <param name="index">数据的开始索引</param>
        /// <param name="count">数据长度</param>
        public virtual void OnRevdCustomBuffer(byte cmd, byte[] buffer, int index, int count) { }

        /// <summary>
        /// 当接收客户端SendRT方法的可靠传输数据缓冲时调用
        /// </summary>
        /// <param name="buffer">可靠传输大数据</param>
        public virtual void OnRevdRTDataBuffer(byte[] buffer) { }

        /// <summary>
        /// 当服务器判定客户端为断线或连接异常时，移除客户端时调用
        /// </summary>
        public virtual void OnRemoveClient() { }

        /// <summary>
        /// 当执行Rpc(远程过程调用函数)时, 如果IOS不支持反射, 需要重写此方法进行指定要调用的函数
        /// </summary>
        /// <param name="func"></param>
        /// <param name="pars"></param>
        public virtual void OnRpcExecute(string func, object[] pars)
        {
            NetDelegate rpc = Rpcs[func];
            rpc.method.Invoke(rpc.target, pars);
        }
        #endregion
        
        /// <summary>
        /// 当玩家每一帧更新
        /// </summary>
        public virtual List<Operation> Update() 
        {
            return new List<Operation>();
        }
    }
}