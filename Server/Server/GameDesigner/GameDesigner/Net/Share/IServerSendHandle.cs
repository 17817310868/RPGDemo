namespace Net.Share
{
    using Net.Server;
    using System.Collections.Generic;

    /// <summary>
    /// 服务器发送处理接口
    /// </summary>
    public interface IServerSendHandle<Player> where Player:NetPlayer
    {
        /// <summary>
        /// 发送网络数据 UDP发送方式
        /// </summary>
        /// <param name="client">发送数据到的客户端</param>
        /// <param name="buffer">数据缓冲区</param>
        void Send(Player client, byte[] buffer);

        /// <summary>
        /// 发送网络数据 UDP发送方式
        /// </summary>
        /// <param name="client">发送数据到的客户端</param>
        /// <param name="fun">RPCFun函数</param>
        /// <param name="pars">RPCFun参数</param>
        void Send(Player client, string fun, params object[] pars);

        /// <summary>
        /// 网络多播, 发送数据到clients集合的客户端（并发, 有可能并行发送）
        /// </summary>
        /// <param name="clients">客户端集合</param>
        /// <param name="func">本地客户端rpc函数</param>
        /// <param name="pars">本地客户端rpc参数</param>
        void Multicast(HashSet<Player> clients, string func, params object[] pars);

        /// <summary>
        /// 网络多播, 发送自定义数据到clients集合的客户端（并发, 有可能并行发送）
        /// </summary>
        /// <param name="clients">客户端集合</param>
        /// <param name="buffer">自定义字节数组</param>
        void Multicast(HashSet<Player> clients, byte[] buffer);

        /// <summary>
        /// 发送自定义网络数据
        /// </summary>
        /// <param name="client">发送到客户端</param>
        /// <param name="cmd">网络命令</param>
        /// <param name="buffer">数据缓冲区</param>
        void Send(Player client, byte cmd, byte[] buffer);

        /// <summary>
        /// 发送网络数据
        /// </summary>
        /// <param name="client">发送到的客户端</param>
        /// <param name="cmd">网络命令</param>
        /// <param name="func">RPCFun函数</param>
        /// <param name="pars">RPCFun参数</param>
        void Send(Player client, byte cmd, string func, params object[] pars);

        /// <summary>
        /// 网络多播, 发送数据到clients集合的客户端（并发, 有可能并行发送）
        /// </summary>
        /// <param name="clients">客户端集合</param>
        /// <param name="cmd">网络命令</param>
        /// <param name="func">本地客户端rpc函数</param>
        /// <param name="pars">本地客户端rpc参数</param>
        void Multicast(HashSet<Player> clients, byte cmd, string func, params object[] pars);

        /// <summary>
        /// 网络多播, 发送自定义数据到clients集合的客户端（并发, 有可能并行发送）
        /// </summary>
        /// <param name="clients">客户端集合</param>
        /// <param name="cmd">网络命令</param>
        /// <param name="buffer">自定义字节数组</param>
        void Multicast(HashSet<Player> clients, byte cmd, byte[] buffer);

        /// <summary>
        /// 发送可靠网络传输, 可以发送大型文件数据
        /// 调用此方法通常情况下是一定把数据发送成功为止, (尽量少用)
        /// </summary>
        /// <param name="client">发送到客户端</param>
        /// <param name="func">函数名</param>
        /// <param name="pars">参数</param>
        void SendRT(Player client, string func, params object[] pars);

        /// <summary>
        /// (Reliable Transport)
        /// 发送可靠网络传输, 可以发送大型文件数据
        /// 调用此方法通常情况下是一定把数据发送成功为止, 此方法如果丢帧会自动补帧, 当对方收到当前帧索引和组包后会回调取下一帧索引, 如果检测丢失帧则重发前一帧数据 (尽量少用)
        /// </summary>
        /// <param name="client">发送到客户端</param>
        /// <param name="func">函数名</param>
        /// <param name="progress">当前传输进度</param>
        /// <param name="pars">参数</param>
        void SendRT(Player client, string func, SendRTProgress progress, params object[] pars);

        /// <summary>
        /// 发送可靠网络传输, 可以发送大型文件数据
        /// 调用此方法通常情况下是一定把数据发送成功为止, (尽量少用)
        /// </summary>
        /// <param name="client">发送到客户端</param>
        /// <param name="cmd">网络命令</param>
        /// <param name="func">函数名</param>
        /// <param name="pars">参数</param>
        void SendRT(Player client, RTCmd cmd, string func, params object[] pars);

        /// <summary>
        /// 发送可靠网络传输, 可以发送大型文件数据
        /// 调用此方法通常情况下是一定把数据发送成功为止, (尽量少用)
        /// </summary>
        /// <param name="client">发送到客户端</param>
        /// <param name="cmd">网络命令</param>
        /// <param name="func">函数名</param>
        /// <param name="progress">当前数据进度</param>
        /// <param name="pars">参数</param>
        void SendRT(Player client, RTCmd cmd, string func, SendRTProgress progress, params object[] pars);

        /// <summary>
        /// 发送可靠网络传输, 可以发送大型文件数据
        /// 调用此方法通常情况下是一定把数据发送成功为止, (尽量少用)
        /// </summary>
        /// <param name="client"></param>
        /// <param name="cmd">网络命令</param>
        /// <param name="progress">当前数据进度</param>
        /// <param name="buffers"></param>
        void SendRT(Player client, RTCmd cmd, SendRTProgress progress, byte[] buffers);

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
        void SendERT(Player client, RTCmd cmd, SendRTProgress progress, byte[] buffer, string func, params object[] pars);

        /// <summary>
        /// (Entity Reliable Transmission Progress)
        /// 发送解释型可靠传输流, 带有命令, 解释对象, 大型数据 注意:此方法跟前面两种方法不同
        /// 使用此方法发送数据, 在服务器声明的rpc方法必须有(byte[] buffer)参数放在后面
        /// </summary>
        /// <param name="client"></param>
        /// <param name="cmd">网络命令: 可选择在实体类中进行调用 或 在公共服务器类调用</param>
        /// <param name="progress">发送大型数据需要的进度事件委托, 可以监听当前发送的进度</param>
        /// <param name="buffer">大型数据比特流</param>
        /// <param name="func">当发送完成后在对方调用的函数名, 而不是在本地调用的函数哦</param>
        /// <param name="progressFunc">这个函数会在对方每次接收数大数据时调用, 对方可以定义这个函数进行查看当前下载进度</param>
        /// <param name="progressItem">对方的 progressFunc(object progressItem, int progressValue) 函数必须定义两个参数, progressItem参数用来标识用的, progressValue参数是当前下载的进度值</param>
        /// <param name="pars"></param>
        void SendERTP(Player client, RTCmd cmd, SendRTProgress progress, byte[] buffer, string func, string progressFunc, object progressItem, params object[] pars);
    }
}