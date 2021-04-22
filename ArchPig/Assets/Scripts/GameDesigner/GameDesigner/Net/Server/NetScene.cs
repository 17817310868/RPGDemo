namespace Net.Server
{
    using Net.Share;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 网络场景
    /// </summary>
    public class NetScene<Player> where Player : NetPlayer
    {
        /// <summary>
        /// 场景容纳人数
        /// </summary>
        public int sceneCapacity = 0;
        /// <summary>
        /// 当前网络场景的玩家
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoIgnore]
        public HashSet<Player> Players { get; set; } = new HashSet<Player>();
        /// <summary>
        /// 当前网络场景状态
        /// </summary>
        public NetState state = NetState.Idle;

        /// <summary>
        /// 获取场景当前人数
        /// </summary>
        public int SceneNumber {
            get { return Players.Count; }
        }

        /// <summary>
        /// 构造网络场景
        /// </summary>
        public NetScene() { }

        /// <summary>
        /// 添加网络主场景并增加主场景最大容纳人数
        /// </summary>
        /// <param name="number">主创建最大容纳人数</param>
        public NetScene(int number)
        {
            sceneCapacity = number;
        }

        /// <summary>
        /// 添加网络场景并增加当前场景人数
        /// </summary>
        /// <param name="client">网络玩家</param>
        /// <param name="number">创建场景容纳人数</param>
        public NetScene(Player client, int number)
        {
            Players.Add(client);
            sceneCapacity = number;
        }

        /// <summary>
        /// 获取场景内的玩家到一维集合里
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Player[] GetPlayers()
        {
            Player[] players = new Player[Players.Count];
            Players.CopyTo(players, 0);
            return players;
        }

        /// <summary>
        /// 添加玩家
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        public void AddPlayerRange(IEnumerable<Player> collection)
        {
            foreach (var p in collection)
                Players.Add(p);
        }

        /// <summary>
        /// 当场景每一帧更新
        /// </summary>
        public virtual void Update(IServerSendHandle<Player> server)
        {
        }
    }
}