namespace Net.Client
{
    using Net.Share;
    using System.Reflection;
    using UnityEngine;

    /// <summary>
    /// 网络行为，此类负责网络增删RPCFun远程过程调用函数，使用到网络通讯功能，需要继承此类
    /// </summary>
    public abstract class NetBehaviour : MonoBehaviour
    {
        /// <summary>
        /// 添加远程过程调用函数的委托
        /// </summary>
        /// <param name="target">远程过程调用指定的对象</param>
        public static void AddRpc(object target, bool append = false)
        {
            AddRpc(NetClientBase.Instance, target, append);
        }

        /// <summary>
        /// 添加远程过程调用函数的委托
        /// </summary>
        /// <param name="client">添加RPC到此客户端</param>
        /// <param name="target">远程过程调用指定的对象</param>
        public static void AddRpc(NetClientBase client, object target, bool append = false)
        {
            if (client == null)
                return;
            if (!append)
                foreach (var o in client.Rpcs)
                    if (o.target == target)
                        return;

            foreach (MethodInfo info in target.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                var rpc = info.GetCustomAttribute<RPCFun>();
                if (rpc != null)
                {
                    NetDelegate item = new NetDelegate(target, info, rpc.cmd);
                    if (item.cmd == NetCmd.ThreadRpc)
                        client.TRpcs.Add(item);
                    else
                        client.Rpcs.Add(item);
                }
            }
        }

        /// <summary>
        /// 移除RPCFun函数
        /// </summary>
        /// <param name="target">将此对象的所有带有RPCFun特性的函数移除</param>
        public static void RemoveRpc(object target)
        {
            RemoveRpc(NetClientBase.Instance, target);
        }

        /// <summary>
        /// 移除子客户端的RPCFun函数
        /// </summary>
        /// <param name="client">子客户端对象</param>
        /// <param name="target">将此对象的所有带有RPCFun特性的函数移除</param>
        public static void RemoveRpc(NetClientBase client, object target)
        {
            for (int i = 0; i < client.Rpcs.Count; i++)
            {
                if (client.Rpcs[i].target.Equals(target) | client.Rpcs[i].method.Equals(null))
                {
                    client.Rpcs.RemoveAt(i);
                    i--;
                }
            }
        }

        /// <summary>
        /// 当游戏开始初始化此对象时，搜索继承此类的方法中带有RPCFun或Rpc特性的方法，并添加到远程过程调用委托集合变量里
        /// </summary>
        protected virtual void Awake()
        {
            NetEvent.AddTaskEvent(()=> 
            {
                if (NetClientBase.Instance == null)
                    return false;
                if (!NetClientBase.Instance.Connected)
                    return false;
                AddRpc(this);
                return true;
            });
        }

        /// <summary>
        /// 发送自定义网络数据 默认使用UDP发送方式
        /// </summary>
        /// <param name="buffer">数据缓冲区</param>
        public static void Send(byte[] buffer)
        {
            NetClientBase.Instance.Send(buffer);
        }

        /// <summary>
        /// 发送自定义网络数据 可使用TCP或UDP进行发送
        /// </summary>
        /// <param name="cmd">网络命令</param>
        /// <param name="buffer">发送字节数组缓冲区</param>
        public static void Send(byte cmd, byte[] buffer)
        {
            NetClientBase.Instance.Send(cmd, buffer);
        }

        /// <summary>
        /// 发送RPCFun网络数据 默认使用UDP发送方式
        /// </summary>
        /// <param name="fun">RPCFun函数</param>
        /// <param name="pars">RPCFun参数</param>
        public static void Send(string fun, params object[] pars)
        {
            NetClientBase.Instance.Send(fun, pars);
        }

        /// <summary>
        /// 发送带有网络命令的RPCFun网络数据 默认使用UDP发送方式
        /// </summary>
        /// <param name="cmd">网络命令</param>
        /// <param name="fun">RPCFun函数</param>
        /// <param name="pars">RPCFun参数</param>
        public static void Send(byte cmd, string fun, params object[] pars)
        {
            NetClientBase.Instance.Send(cmd, fun, pars);
        }
    }
}