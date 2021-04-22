using System;
using System.Net.Sockets;

namespace Net.Server
{
    /// <summary>
    /// Tcp服务器基类
    /// </summary>
    /// <typeparam name="Player"></typeparam>
    /// <typeparam name="Scene"></typeparam>
    public abstract class TcpServerBase<Player, Scene> : NetServerBase<Player,Scene> 
        where Player:NetPlayer, new() 
        where Scene:NetScene<Player>,new()
    {
        /// <summary>
        /// 允许叠包缓冲器最大值 默认可发送5242880(5M)的数据包
        /// </summary>
        public int StackBufferSize { get; set; } = 5242880;
        /// <summary>
        /// 允许叠包最大次数，如果数据包太大，接收数据的次数超出StackNumberMax值，则会清除叠包缓存器 默认可叠包25次
        /// </summary>
        public int StackNumberMax { get; set; } = 25;

        /// <summary>
        /// 发送或接收异常断开连接处理
        /// </summary>
        /// <param name="client"></param>
        protected void Disconnected(Socket client)
        {
            try {
                if (UnClients.ContainsKey(client.RemoteEndPoint)) {
                    UnClients.TryRemove(client.RemoteEndPoint, out Player player1);
                    logList.Add($"接收异常:未知客户端：{client.RemoteEndPoint.ToString()}被移除!");
                } else if (Clients.ContainsKey(client.RemoteEndPoint)) {
                    Clients.TryRemove(client.RemoteEndPoint, out Player player1);
                    logList.Add($"接收异常:客户端：{player1.playerID}:{client.RemoteEndPoint.ToString()}被移除!");
                    OnRemoveClient(player1);
                }
            } catch (Exception ex) { }
        }
    }
}
