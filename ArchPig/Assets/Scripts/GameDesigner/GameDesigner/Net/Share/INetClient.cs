namespace Net.Share
{
    using System;

    /// <summary>
    /// 网络客户端接口处理 2019.7.9
    /// </summary>
    public interface INetClient
    {
        /// <summary>
        /// 输出调用网络函数
        /// </summary>
        event Action<string> LogRpc;
        /// <summary>
        /// 输出调用RPC错误,白色警告
        /// </summary>
        event Action<string> LogBug;
        /// <summary>
        /// 输出提示信息
        /// </summary>
        event Action<string> Log;
        /// <summary>
        /// 当连接服务器成功事件
        /// </summary>
        event Action OnConnectedHandle;
        /// <summary>
        /// 当连接失败事件
        /// </summary>
        event Action OnConnectFailedHandle;
        /// <summary>
        /// 当尝试连接服务器事件
        /// </summary>
        event Action OnTryToConnectHandle;
        /// <summary>
        /// 当连接中断 (异常) 事件
        /// </summary>
        event Action OnConnectLostHandle;
        /// <summary>
        /// 当断开连接事件
        /// </summary>
        event Action OnDisconnectHandle;
        /// <summary>
        /// 当接收到网络数据处理事件
        /// </summary>
        event Client.RevdBufferHandle OnRevdBufferHandle;
        /// <summary>
        /// 当接收到自定义数据触发的事件,注意：此事件是多线程在调用
        /// </summary>
        event Client.RevdBufferHandle OnRevdCustomBufferHandle;
        /// <summary>
        /// 当断线重连成功触发事件
        /// </summary>
        event Action OnReconnectHandle;
        /// <summary>
        /// 当关闭连接事件
        /// </summary>
        event Action OnCloseConnectHandle;
        /// <summary>
        /// 当统计网络流量时触发
        /// </summary>
        event NetworkDataTraffic OnNetworkDataTraffic;
    }
}