namespace Net.Share
{
    using System;

    /// <summary>
    /// 标注为远程过程调用函数
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class RPCFun : Attribute
    {
        /// <summary>
        /// 网络命令
        /// </summary>
        public byte cmd;

        /// <summary>
        /// 构造RPCFun函数特性
        /// </summary>
        public RPCFun() {}

        /// <summary>
        /// 构造RPCFun函数特性
        /// </summary>
        /// <param name="cmd">自定义的网络命令</param>
        public RPCFun(byte cmd)
        {
            this.cmd = cmd;
        }
    }

    /// <summary>
    /// 标注为远程过程调用函数 (简型)
    /// </summary>
    public class Rpc : RPCFun
    {
        /// <summary>
        /// 构造RPCFun函数特性
        /// </summary>
        public Rpc() { }

        /// <summary>
        /// 构造RPCFun函数特性
        /// </summary>
        /// <param name="cmd">自定义的网络命令</param>
        public Rpc(byte cmd)
        {
            this.cmd = cmd;
        }
    }
}