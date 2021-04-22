namespace Net.Server
{
    using Net.Share;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 网络服务器核心类 2019.11.22
    /// Player为你自己定义的玩家类型, 玩家类型针对每个用户的数据存储用到, 
    /// Scene为服务器场景类型, 你的游戏中场景里面需要有什么功能, 则写在Scene类里面
    /// </summary>
    [Serializable]
    public class NetServer<Player, Scene> : NetServerBase<Player, Scene> where Player : NetPlayer, new() where Scene : NetScene<Player>, new()
    {
        /// <summary>
        /// 添加网络Rpc
        /// </summary>
        /// <param name="target">注册的对象实例</param>
        public void AddRpcHandle(object target)
        {
            NetBehaviour<Player,Scene>.AddRpcs(this, target);
        }

        /// <summary>
        /// 添加网络Rpc
        /// </summary>
        /// <param name="rpcs"></param>
        public void AddRpcHandle(List<NetDelegate> rpcs)
        {
            Rpcs.AddRange(rpcs);
        }
    }

    /// <summary>
    /// 网络服务器核心类 2019.10.4
    /// </summary>
    [Serializable]
    public class UdpServer<Player, Scene> : NetServer<Player, Scene> where Player : NetPlayer, new() where Scene : NetScene<Player>, new()
    {
    }
}
