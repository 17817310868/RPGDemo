namespace Net.Share
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// 可靠传输状态
    /// </summary>
    public enum RTState
    {
        /// <summary>
        /// 发送中
        /// </summary>
        Sending,
        /// <summary>
        /// 下载中
        /// </summary>
        Download,
        /// <summary>
        /// 发送失败
        /// </summary>
        FailSend,
        /// <summary>
        /// 发送完成
        /// </summary>
        Complete,
        /// <summary>
        /// 尝试重传
        /// </summary>
        Retransmission
    }

    /// <summary>
    /// 可靠文件发送进度委托
    /// </summary>
    /// <param name="progress">当前进度</param>
    /// <param name="state">当前状态</param>
    public delegate void SendRTProgress(float progress, RTState state);

    /// <summary>
    /// 可靠网络传输命令
    /// </summary>
    public enum RTCmd : byte
    {
        /// <summary>
        /// 公共可靠传输, 在Server类Rpc进行调用
        /// </summary>
        PublicRT = NetCmd.ReliableTransport,
        /// <summary>
        /// 实体可靠传输, 在NetPlayer内部Rpc调用
        /// </summary>
        EntityRT = NetCmd.EntityReliableTransport
    }

    /// <summary>
    /// 可靠传输发送缓冲
    /// </summary>
    public sealed class SendRTBuffer
    {
        /// <summary>
        /// 函数名
        /// </summary>
        public string func;
        /// <summary>
        /// 参数
        /// </summary>
        public object[] pars;
        /// <summary>
        /// 网络命令
        /// </summary>
        public RTCmd cmd;
        /// <summary>
        /// 大型数据帧字典
        /// </summary>
        public Dictionary<int, byte[]> buffer = new Dictionary<int, byte[]>();
        /// <summary>
        /// 数据帧索引
        /// </summary>
        public int index;
        /// <summary>
        /// 控制时间
        /// </summary>
        public DateTime time;
        /// <summary>
        /// 数据大小
        /// </summary>
        public int count;
        /// <summary>
        /// 数据进度, 值1:当前进度 值2:是否完成 值3:当前状态
        /// </summary>
        public SendRTProgress progress;
        /// <summary>
        /// 所有要发送的数据
        /// </summary>
        public byte[] datas;
        /// <summary>
        /// 内部为0,  自定义为1, 解释型为2, 解释型带对方下载进度值的为3
        /// </summary>
        public byte kernel;
        /// <summary>
        /// 数据键
        /// </summary>
        public int keyID;
        /// <summary>
        /// 尝试重传
        /// </summary>
        public int tryIndex;
        /// <summary>
        /// 被动帧索引, 当客户端,服务器一直没有数据往来时用来检测
        /// </summary>
        public int pasIndex;
        /// <summary>
        /// 当 kernel=3 时用来存储对方进度函数
        /// </summary>
        public byte[] progressData = new byte[0];

        /// <summary>
        /// 构造可靠传输数据缓冲
        /// </summary>
        public SendRTBuffer()
        {
            time = DateTime.Now.AddSeconds(5);
        }

        /// <summary>
        /// 构造可靠传输数据缓冲
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="func"></param>
        /// <param name="progress"></param>
        /// <param name="pars"></param>
        public SendRTBuffer(RTCmd cmd, string func, SendRTProgress progress, object[] pars)
        {
            this.cmd = cmd;
            this.func = func;
            this.progress = progress;
            this.pars = pars;
            time = DateTime.Now.AddSeconds(pars.Length / 50000);
            kernel = 0;
        }

        /// <summary>
        /// 构造可靠传输数据缓冲
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="progress"></param>
        /// <param name="buffer"></param>
        public SendRTBuffer(RTCmd cmd, SendRTProgress progress, byte[] buffer)
        {
            this.cmd = cmd;
            this.func = string.Empty;
            this.progress = progress;
            datas = buffer;
            time = DateTime.Now.AddSeconds(buffer.Length / 50000);
            kernel = 1;
        }

        /// <summary>
        /// 解析
        /// </summary>
        ~SendRTBuffer()
        {
            buffer = null;
            datas = null;
        }
    }

    /// <summary>
    /// 可靠传输接收缓冲
    /// </summary>
    public sealed class RevdRTBuffer
    {
        /// <summary>
        /// 传输当前帧索引
        /// </summary>
        public int index;
        /// <summary>
        /// 数据内存流
        /// </summary>
        public MemoryStream stream;
        /// <summary>
        /// 可控制的时间范围, 如果超时可移除
        /// </summary>
        public DateTime time;

        /// <summary>
        /// 构造函数
        /// </summary>
        public RevdRTBuffer()
        {
            stream = new MemoryStream();
        }

        /// <summary>
        /// 解析
        /// </summary>
        ~RevdRTBuffer()
        {
            stream?.Dispose();
        }
    }
}