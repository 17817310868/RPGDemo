namespace Net.Share
{
    /// <summary>
    /// 网络调式日志信息处理接口
    /// </summary>
    public interface IDebugHandle
    {
        /// <summary>
        /// 当输出信息
        /// </summary>
        /// <param name="msg"></param>
        void DebugLog(string msg);
        /// <summary>
        /// 当远程过程调用函数
        /// </summary>
        /// <param name="msg"></param>
        void RpcLog(string msg);
        /// <summary>
        /// 当网络错误输出
        /// </summary>
        /// <param name="msg"></param>
        void ErrorLog(string msg);
    }
}
