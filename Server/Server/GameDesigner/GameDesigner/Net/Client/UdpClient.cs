namespace Net.Client
{
    using Net.Share;
    using System;

    /// <summary>
    /// Udp网络客户端
    /// </summary>
    [Serializable]
    public class UdpClient : NetClientBase
    {
        /// <summary>
        /// 构造不可靠传输客户端
        /// </summary>
        public UdpClient() { }
        
        /// <summary>
        /// 构造不可靠传输客户端
        /// </summary>
        /// <param name="useUnityThread">使用unity多线程?</param>
        public UdpClient(bool useUnityThread)
        {
            UseUnityThread = useUnityThread;
        }
    }

    /// <summary>
    /// 网络客户端
    /// </summary>
    [Serializable]
    public class NetClient : NetClientBase
    {
        /// <summary>
        /// 构造客户端
        /// </summary>
        public NetClient() { }

        /// <summary>
        /// 构造客户端
        /// </summary>
        /// <param name="useUnityThread">使用unity多线程?</param>
        public NetClient(bool useUnityThread)
        {
            UseUnityThread = useUnityThread;
        }
    }
}
