namespace Net.Server
{
    using Net.Share;
    using System;
    using System.Collections.Generic;
    using System.Net;

    /// <summary>
    /// 主服务器
    /// </summary>
    public class MainServer<Server, Player, Scene> : NetServerBase<Player, Scene> 
        where Player : NetPlayer, new() 
        where Scene : NetScene<Player>, new()
        where Server : NetServerBase<Player, Scene>, new()
    {
        /// <summary>
        /// 服务器集群
        /// </summary>
        public List<Server> ServerArea = new List<Server>();
        /// <summary>
        /// 当分区服务器被创建
        /// </summary>
        public Action<Server> OnCreateServer;

        /// <summary>
        /// 
        /// </summary>
        protected override void OnStartupCompleted()
        {
            int port = NetPort.GetFirstAvailablePort();
            if (port == -1)
            {
                logList.Add("服务器无可用端口!");
                return;
            }
            Server newServer = new Server();
            newServer.Start(port);
            newServer.OnlineLimit = 500;
            ServerArea.Add(newServer);
            OnCreateServer?.Invoke(newServer);
        }
       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="unPlayer"></param>
        /// <param name="remotePoint"></param>
        /// <param name="buffer"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        protected override int ResolveUnBuffer(Player unPlayer, EndPoint remotePoint, byte[] buffer, int count)
        {
            int port;
            foreach (var server in ServerArea)
            {
                if (server.OnlinePlayers < server.OnlineLimit)
                {
                    port = server.Port;
                    Send(unPlayer, NetCmd.SwitchPort, BitConverter.GetBytes(port));
                    return 2;
                }
            }
            port = NetPort.GetFirstAvailablePort();
            if (port == -1)
            {
                logList.Add("服务器无可用端口!");
                return 2;
            }
            Server newServer = new Server();
            newServer.Start(port);
            newServer.OnlineLimit = 500;
            ServerArea.Add(newServer);
            Send(unPlayer, NetCmd.SwitchPort, BitConverter.GetBytes(port));
            OnCreateServer?.Invoke(newServer);
            return 2;
        }
    }
}
