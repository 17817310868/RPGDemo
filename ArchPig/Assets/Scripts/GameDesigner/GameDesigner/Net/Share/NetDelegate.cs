namespace Net.Share
{
    using System;
    using System.Reflection;
    using System.Runtime.InteropServices;

    /// <summary>
    /// 远程过程调用委托
    /// </summary>
    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct NetDelegate
    {
        /// <summary>
        /// 委托函数名
        /// </summary>
        public string Name;
        /// <summary>
        /// 委托对象
        /// </summary>
        public object target;
        /// <summary>
        /// 委托方法
        /// </summary>
        public MethodInfo method;
        /// <summary>
        /// 网络命令
        /// </summary>
        public byte cmd;
        
        /// <summary>
        /// 网络委托构造函数
        /// </summary>
        public NetDelegate(Action method)
        {
            Name = method.Method.ToString();
            cmd = 0;
            target = method.Target;
            this.method = method.Method;
        }

        /// <summary>
        /// 网络委托构造函数
        /// </summary>
        public NetDelegate(object target, MethodInfo method)
        {
            Name = method.ToString();
            cmd = 0;
            this.target = target;
            this.method = method;
        }

        /// <summary>
        /// 网络委托构造函数
        /// </summary>
        public NetDelegate(object target, MethodInfo method, byte cmd)
        {
            Name = method.ToString();
            this.cmd = cmd;
            this.target = target;
            this.method = method;
        }

        internal void Invoke()
        {
            method.Invoke(target, null);
        }

        internal void Invoke(params object[] pars)
        {
            method.Invoke(target, pars);
        }
    }
}

