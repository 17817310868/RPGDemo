namespace Net.Share
{
    /// <summary>
    /// 发送处理程序接口 
    /// 2019.9.23
    /// </summary>
    public interface ISendHandle
    {
        /// <summary>
        /// 发送自定义网络数据 默认使用UDP发送方式
        /// </summary>
        /// <param name="buffer">数据缓冲区</param>
        void Send(byte[] buffer);

        /// <summary>
        /// 发送自定义网络数据 可使用TCP或UDP进行发送
        /// </summary>
        /// <param name="cmd">网络命令</param>
        /// <param name="buffer">发送字节数组缓冲区</param>
        void Send(byte cmd, byte[] buffer);

        /// <summary>
        /// 发送RPCFun网络数据 默认使用UDP发送方式
        /// </summary>
        /// <param name="fun">RPCFun函数</param>
        /// <param name="pars">RPCFun参数</param>
        void Send(string fun, params object[] pars);

        /// <summary>
        /// 发送带有网络命令的RPCFun网络数据
        /// </summary>
        /// <param name="cmd">网络命令</param>
        /// <param name="fun">RPCFun函数</param>
        /// <param name="pars">RPCFun参数</param>
        void Send(byte cmd, string fun, params object[] pars);

        /// <summary>
        /// 远程过程调用 同Send方法
        /// </summary>
        /// <param name="fun">Call名</param>
        /// <param name="pars">Call函数</param>
        void CallRpc(string fun, params object[] pars);

        /// <summary>
        /// 远程过程调用 同Send方法
        /// </summary>
        /// <param name="cmd">网络命令，请看NetCmd类定义</param>
        /// <param name="fun">Call名</param>
        /// <param name="pars">Call函数</param>
        void CallRpc(byte cmd, string fun, params object[] pars);

        /// <summary>
        /// 网络请求 同Send方法
        /// </summary>
        /// <param name="fun">Call名</param>
        /// <param name="pars">Call函数</param>
        void Request(string fun, params object[] pars);

        /// <summary>
        /// 网络请求 同Send方法
        /// </summary>
        /// <param name="cmd">网络命令，请看NetCmd类定义</param>
        /// <param name="fun">Call名</param>
        /// <param name="pars">Call函数</param>
        void Request(byte cmd, string fun, params object[] pars);

        /// <summary>
        /// 发送可靠网络传输, 可以发送大型文件数据
        /// 如果不是大数据, 不推荐使用此方法
        /// </summary>
        /// <param name="cmd">网络命令</param>
        /// <param name="func">函数名</param>
        /// <param name="pars">参数</param>
        void SendRT(RTCmd cmd, string func, params object[] pars);

        /// <summary>
        /// 发送可靠网络传输, 可以发送大型文件数据
        /// 如果不是大数据, 不推荐使用此方法
        /// </summary>
        /// <param name="cmd">网络命令</param>
        /// <param name="func">函数名</param>
        /// <param name="progress">当前数据进度</param>
        /// <param name="pars">参数</param>
        void SendRT(RTCmd cmd, string func, SendRTProgress progress, params object[] pars);

        /// <summary>
        /// 发送可靠网络传输, 可发送大数据流
        /// 如果不是大数据, 不推荐使用此方法
        /// </summary>
        /// <param name="cmd">网络命令</param>
        /// <param name="progress">当前数据进度</param>
        /// <param name="buffers"></param>
        void SendRT(RTCmd cmd, SendRTProgress progress, byte[] buffers);

        /// <summary>
        /// 发送解释型可靠传输流, 带有命令, 解释对象, 大型数据 注意:此方法跟前面两种方法不同
        /// 使用此方法发送数据, 在服务器声明的rpc方法必须有(byte[] buffer)参数放在后面
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="progress"></param>
        /// <param name="buffer"></param>
        /// <param name="func"></param>
        /// <param name="pars"></param>
        void SendERT(RTCmd cmd, SendRTProgress progress, byte[] buffer, string func, params object[] pars);
    }
}
