using Net.Server;
using System.Collections.Concurrent;

namespace Net.Share
{
    /// <summary>
    /// 用户对接基类服务器
    /// </summary>
    public interface IServerHandle<Player, Scene> : 
        IServerSendHandle<Player>, 
        INetworkSceneHandle<Player, Scene>, 
        IServerEventHandle<Player>
        where Player : NetPlayer where Scene : NetScene<Player>
    {
        ConcurrentDictionary<string, Scene> Scenes { get; set; }
    }
}
