namespace Net.Server
{
    using Net.Share;
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;

    /// <summary>
    /// Tcp网络服务器 2019.9.9  
    /// 作者:龙兄
    /// QQ：1752062104
    /// </summary>
    public class TcpServer<Player,Scene> : TcpServerBase<Player,Scene>
        where Player : NetPlayer, new() 
        where Scene : NetScene<Player>, new()
    {
        /// <summary>
        /// 启动服务器
        /// </summary>
        /// <param name="port">端口</param>
        /// <param name="workerThreads">处理线程数</param>
        public override void Start(int port = 666, int workerThreads = 0)
        {
            if (Server != null)//如果服务器套接字已创建
                throw new Exception("服务器已经运行，不可重新启动，请先关闭后在重启服务器");

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
            Server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//---TCP协议
            IPEndPoint ip = new IPEndPoint(IPAddress.Any, port);//IP端口设置
            Server.Bind(ip);//绑定 IP端口
            Server.Listen(LineUp);

            new Thread(AcceptConnect) { IsBackground = true, Name = "AcceptConnect" }.Start();//创建接受连接线程
            new Thread(HeartUpdate) { IsBackground = true, Name = "HeartUpdate" }.Start();//创建心跳包线程
            new Thread(UnHeartUpdate) { IsBackground = true, Name = "UnHeartUpdate" }.Start();//创建未知客户端心跳包线程
            new Thread(DebugThread) { IsBackground = true, Name = "DebugThread" }.Start();
            new Thread(DebugLogThread) { IsBackground = true, Name = "DebugLogThread" }.Start();

            var scene = OnAddDefaultScene();
            DefaultScene = scene.Key;
            Scenes.TryAdd(scene.Key, scene.Value);
            OnStartupCompletedHandle();
            logList.Add("服务器启动成功!");
        }

        //接受客户端连接
        void AcceptConnect()
        {
            try
            {
                if (!IsRunServer)
                    return;
                Server.BeginAccept(ac => {
                    try
                    {
                        var client = Server.EndAccept(ac);
                        AddUnClient(client);
                        ReceiveBuffer(client);
                    }
                    catch
                    {
                    }
                    finally
                    {
                        AcceptConnect();
                    }
                }, null);
            } catch { }
        }

        //异步接受数据缓冲区
        void ReceiveBuffer(Socket client)
        {
            try
            {
                if (!client.Connected)
                    return;
                var buffer = new byte[50000];
                client.BeginReceive(buffer, 0, 50000, 0, (ar) =>
                {
                    var player = (Socket)ar.AsyncState;
                    try
                    {
                        int count = client.EndReceive(ar);
                        TcpHandle(buffer, count, player);
                    }
                    catch (Exception ex)
                    {
                        Disconnected(player);
                        return;
                    }
                    //finally
                    //{
                        ReceiveBuffer(player);
                    //}
                }, client);
            }
            catch (Exception ex)
            {
                Disconnected(client);
            }
        }

        //tcp数据处理
        void TcpHandle(byte[] buffer, int count, Socket client)
        {
            if (Clients.ContainsKey(client.RemoteEndPoint))//在线客户端
            {
                ResolveBuffer(Clients[client.RemoteEndPoint], buffer, count);//处理缓冲区数据
                return;
            }
            if (UnClients.ContainsKey(client.RemoteEndPoint))//未知客户端
            {
                int removeUnClient = ResolveUnBuffer(UnClients[client.RemoteEndPoint], buffer, count);
                if (removeUnClient == -1)//如果允许未知客户端进入服务器，则可以将此客户端从未知客户端字典种移除，并且添加此客户端到在线玩家集合中
                    UnClients.TryRemove(client.RemoteEndPoint, out Player unClient1);
                return;
            }
            ResolveBuffer(client, buffer, count);
            if (UnClients.Count >= LineUp)//排队人数
            {
                exceededNumber++;
                OnExceededNumber(client);
                return;
            }
            if (Clients.Count >= OnlineLimit)//服务器最大在线人数
            {
                blockConnection++;
                OnBlockConnection(client);
                return;
            }
            AddUnClient(client);
        }

        /// <summary>
        /// 解析网络数据包
        /// </summary>
        private void ResolveBuffer(Socket client, byte[] buffer, int count)
        {
            int index = 0;
            while (index < count)
            {
                byte cmd = buffer[index];//[0] = 网络命令整形数据
                int size = BitConverter.ToInt32(buffer, index + 1);// {[1],[2]} 网络数据长度大小
                if (cmd == NetCmd.QuitGame)//退出程序指令
                    return;
                if (cmd == NetCmd.SendHeartbeat)//buffer[0]=5:连接或心跳Ping指令 count=1:连接指令 count=5：心跳指令
                    client.Send(new byte[] { 6, 0, 0, 0, 0 });//心跳回应 或 连接回应
                index = index + frame + size;
            }
        }

        //添加客户端到无知客户端列表里
        void AddUnClient(Socket client)
        {
            exceededNumber = 0;
            blockConnection = 0;
            Player unClient = new Player();
            unClient.Client = client;
            unClient.StackStream = new MemoryStream();
            unClient.RemotePoint = client.RemoteEndPoint;
            unClient.LastTime = DateTime.Now.AddMinutes(5);
            OnHasConnectHandle?.Invoke(unClient);
            logList.Add("有客户端连接:" + client.RemoteEndPoint.ToString());
            UnClients.TryAdd(client.RemoteEndPoint, unClient);
        }

        /// <summary>
        /// 当服务器连接人数溢出时调用
        /// </summary>
        /// <param name="remotePoint"></param>
        private void OnExceededNumber(Socket client)
        {
            logList.Add("未知客户端排队爆满,阻止连接人数: " + exceededNumber);
            client.Send(new byte[] { NetCmd.ExceededNumber, 0, 0, 0, 0 });
        }

        /// <summary>
        /// 当服务器爆满时调用
        /// </summary>
        /// <param name="remotePoint"></param>
        private void OnBlockConnection(Socket client)
        {
            logList.Add("服务器爆满,阻止连接人数: " + blockConnection);
            client.Send(new byte[] { NetCmd.BlockConnection, 0, 0, 0, 0 });
        }
        
        //解析未知客户端数据缓冲区， 返回1：未知客户端表达没有通过， 返回-1：允许未知客户端进服务器，与其他客户端进行互动
        private int ResolveUnBuffer(Player unPlayer, byte[] buffer, int count)
        {
            int index = 0;
            if (unPlayer.stack > StackNumberMax)//不能一直叠包
            {
                unPlayer.stack = 0;
                unPlayer.StackStream.Flush();
            }
            if (unPlayer.stack >= 1)
            {
                unPlayer.StackStream.Write(buffer, index, count);
                buffer = unPlayer.StackStream.ToArray();
                count = (int)unPlayer.StackStream.Length;
            }
            while (index < count)
            {
                byte cmd = buffer[index];//[0] = 网络命令整形数据
                int size = BitConverter.ToInt32(buffer, index + 1);// {[1],[2]} 网络数据长度大小
                if (size < 0 | size > StackBufferSize) {//如果出现解析的数据包大小有问题，则不处理
                    unPlayer.stack = 0;
                    return 1;
                }
                if (index + frame + size <= count)
                {
                    unPlayer.stack = 0;
                    if (UnClientHandle(unPlayer, cmd, buffer, size) == -1)
                        return -1;
                }
                else
                {
                    unPlayer.StackStream.SetLength(0);
                    unPlayer.StackStream.Write(buffer, index, count - index);
                    unPlayer.stack++;
                    break;
                }
                index = index + frame + size;
            }
            return 1;
        }

        //接受未知客户端进入服务器许可 -1:接受客户端，并添加到客户端在线字典(集合) 0:无动作
        private int UnClientHandle(Player unPlayer, byte cmd, byte[] buffer, int count)
        {
            Player unClient = null;
            switch (cmd)
            {
                case NetCmd.SendHeartbeat:
                    Send(unPlayer, new byte[] { 6, 0, 0, 0, 0 }, 0, frame);//心跳回应 或 连接回应
                    return 0;
                case NetCmd.RevdHeartbeat:
                    unPlayer.heart = 0;
                    return 0;
                case NetCmd.EntityRpc:
                    unClient = unPlayer.OnUnClientRequest(cmd, buffer, frame, count) as Player;
                    break;
                case NetCmd.CallRpc:
                    unClient = OnUnClientRequest(unPlayer, cmd, buffer, frame, count);
                    break;
                case NetCmd.SafeCall:
                    unClient = OnUnClientRequest(unPlayer, cmd, buffer, frame, count);
                    break;
                case NetCmd.QuitGame:
                    return -1;
            }
            if (unClient != null)//当有客户端连接时,如果允许用户添加此客户端
            {
                unClient.Client = unPlayer.Client;
                unClient.StackStream = new MemoryStream();
                if (!Scenes.ContainsKey(unClient.sceneID))//如果非法场景ID则使用默认场景ID
                    unClient.sceneID = DefaultScene;
                unClient.RemotePoint = unPlayer.Client.RemoteEndPoint;//防止旧的端口号
                unClient.heart = 0;//心跳初始化
                Clients.TryAdd(unPlayer.Client.RemoteEndPoint, unClient);//将网络玩家添加到集合中
                if (unClient.playerID == string.Empty)
                    unClient.playerID = Share.Random.Range(1000000, 9999999).ToString();
                Players.TryAdd(unClient.playerID, unClient);//将网络玩家添加到集合中
                Scenes[DefaultScene].Players.Add(unClient);//将网络玩家添加到主场景集合中
                unClient.Scene = Scenes[DefaultScene];//赋值玩家所在的场景实体
                unClient.AddRpc(unClient);
                OnAddClientHandle?.Invoke(unClient);
                return -1;
            }
            return 0;
        }

        /// <summary>
        /// 解析网络数据包
        /// </summary>
        private void ResolveBuffer(Player client, byte[] buffer, int count)
        {
            int index = 0;
            if (client.stack > StackNumberMax)//不能一直叠包
            {
                client.stack = 0;
                client.StackStream.Flush();
                logList.Add("数据错乱，叠包数量达到25次以上");
            }
            if (client.stack >= 1)
            {
                client.StackStream.Write(buffer, index, count);
                buffer = client.StackStream.ToArray();
                count = (int)client.StackStream.Length;
            }
            while (index < count)
            {
                byte cmd = buffer[index];//[0] = 网络命令整形数据
                int size = BitConverter.ToInt32(buffer, index + 1);// {[1],[2]} 网络数据长度大小
                if (size < 0 | size > StackBufferSize) {//如果出现解析的数据包大小有问题，则不处理
                    client.stack = 0;
                    return;
                }
                if (index + frame + size <= count)
                {
                    OnRevdBufferHandle?.Invoke(client, cmd, buffer, index + frame, size);
                    client.stack = 0;
                }
                else
                {
                    client.StackStream.SetLength(0);
                    client.StackStream.Write(buffer, index, count - index);
                    client.stack ++;
                    break;
                }
                index = index + frame + size;
            }
        }

        /// <summary>
        /// 发送封装完成后的网络数据
        /// </summary>
        /// <param name="client">发送到的客户端</param>
        /// <param name="buffer">发送字节数组缓冲区</param>
        /// <param name="index">字节数组开始位置</param>
        /// <param name="count">字节数组长度</param>
        protected override void Send(Player client, byte[] buffer, int index, int count)
        {
            try {
                sendAmount++;
                sendCount += count;
                client.Client.Send(buffer, index, count, 0);
            } catch (Exception ex){
                Disconnected(client.Client);
            }
        }

        /// <summary>
        /// 未知客户端心跳处理
        /// </summary>
        protected override void UnHeartHandle()
        {
            foreach (var client in UnClients)
            {
                if (client.Value == null)
                    goto GO;
                try
                {
                    if (DateTime.Now > client.Value.LastTime)
                        throw new Exception("未知客户端占用连接通道!");
                    Send(client.Value, new byte[] { 5, 0, 0, 0, 0 }, 0, frame);
                    continue;
                }
                catch (Exception ex)
                {
                    logList.Add("移除未知客户端:" + ex);
                }
            GO: UnClients.TryRemove(client.Key, out Player recClient);
            }
        }

        /// <summary>
        /// 客户端心跳处理
        /// </summary>
        protected override void HeartHandle()
        {
            foreach (var client in Clients)
            {
                if (client.Value == null)
                    goto GO;
                try
                {
                    Send(client.Value, new byte[] { 5, 0, 0, 0, 0 }, 0, frame);
                    continue;
                }
                catch (Exception ex)
                {
                    logList.Add("移除客户端: 玩家ID:" + client.Value?.playerID + " 玩家终端: " + client.Key.ToString() + " Code: " + ex.Message);
                }
                GO: if(Clients.TryRemove(client.Key, out Player recClient))
                    OnRemoveClientEvent?.Invoke(recClient);
                if (client.Value != null)
                {
                    Players.TryRemove(client.Value.playerID, out Player recClient1);
                    if (Scenes.ContainsKey(client.Value.sceneID))
                        Scenes[client.Value.sceneID].Players.Remove(client.Value);
                }
            }
        }

        /// <summary>
        /// 从客户端字典中移除客户端
        /// </summary>
        /// <param name="client"></param>
        public override void RemoveClient(Player client)
        {
            if (Clients.TryRemove(client.RemotePoint, out Player reClient))
                OnRemoveClientEvent?.Invoke(reClient);
            Players.TryRemove(client.playerID, out Player reClient1);
            if (Scenes.ContainsKey(client.sceneID))
            {
                var players = Scenes[client.sceneID].Players;
                players.Remove(client);
            }
            client.Dispose();
        }

        /// <summary>
        /// 关闭服务器
        /// </summary>
        public override void Close()
        {
            base.Close();
            Player player;
            foreach (var unClient in UnClients)
            {
                UnClients.TryRemove(unClient.Key, out player);
                player.Dispose();
                player = null;
            }
            foreach (var client in Clients)
            {
                Clients.TryRemove(client.Key, out player);
                player.Dispose();
                player = null;
            }
        }
    }
}