namespace Net.Share
{
    /// <summary>
    /// 网络流量数据统计
    /// </summary>
    /// <param name="sendNumber">一秒发送数据次数</param>
    /// <param name="sendCount">一秒发送字节长度</param>
    /// <param name="receiveNumber">一秒接收数据次数</param>
    /// <param name="receiveCount">一秒接收到的字节大小</param>
    /// <param name="resolveNumber">解析RPC函数次数</param>
    public delegate void NetworkDataTraffic(int sendNumber, int sendCount, int receiveNumber, int receiveCount, int resolveNumber);

    /// <summary>
    /// 当处理缓冲区数据
    /// </summary>
    /// <param name="client">处理此客户端的数据请求</param>
    /// <param name="cmd">网络命令</param>
    /// <param name="buffer">缓冲区数据</param>
    /// <param name="index">数据开始索引</param>
    /// <param name="count">数据长度</param>
    public delegate void RevdBufferHandle<Player>(Player client, byte cmd, byte[] buffer, int index, int count);
}