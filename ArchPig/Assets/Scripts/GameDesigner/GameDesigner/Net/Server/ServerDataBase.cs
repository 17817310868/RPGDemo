namespace Net.Server
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// 服务器数据库 19.10.4
    /// </summary>
    public class ServerDataBase<Player> where Player : NetPlayer
    {
        /// <summary>
        /// 玩家数据保存路径
        /// </summary>
        public static string PlayersPath = "Data/";
        /// <summary>
        /// 所有玩家信息
        /// </summary>
        public ConcurrentDictionary<string, Player> PlayerInfos = new ConcurrentDictionary<string, Player>();

        /// <summary>
        /// 直接读取数据库玩家对象
        /// </summary>
        /// <param name="playerID"></param>
        /// <returns></returns>
        public Player this[string playerID]
        {
            get { return PlayerInfos[playerID]; }
            set { PlayerInfos[playerID] = value; }
        }

        /// <summary>
        /// 获得所有玩家帐号数据
        /// </summary>
        public List<Player> Players()
        {
            return new List<Player>(PlayerInfos.Values);
        }

        /// <summary>
        /// 加载数据库信息
        /// </summary>
        public Task Load()
        {
            return LoadAsync(null);
        }

        /// <summary>
        /// 加载数据库信息
        /// </summary>
        /// <param name="lastHandle">需要做最后的处理的, Player.playerID必须指定 </param>
        public Task Load(Action<Player> lastHandle)
        {
            return LoadAsync(lastHandle);
        }

        /// <summary>
        /// 异步加载数据库信息
        /// </summary>
        /// <param name="lastHandle">需要做最后的处理的, Player.playerID必须指定 </param>
        /// <returns></returns>
        public Task LoadAsync(Action<Player> lastHandle)
        {
            return Task.Run(() =>
            {
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                if (!Directory.Exists(baseDirectory + PlayersPath))
                    Directory.CreateDirectory(baseDirectory + PlayersPath);
                string[] playerDataPaths = Directory.GetFiles(baseDirectory + PlayersPath, "PlayerInfo.data", SearchOption.AllDirectories);
                foreach (var path in playerDataPaths)
                {
                    try
                    {
                        FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                        var buffer = new byte[fileStream.Length];
                        int count = fileStream.Read(buffer, 0, buffer.Length);
                        fileStream.Close();
                        var func = Share.NetConvert.Deserialize(buffer, 0, count);
                        if (func.pars.Length <= 0)
                            continue;
                        if (func.pars[0] is Player player){
                            lastHandle?.Invoke(player);
                            PlayerInfos.TryAdd(player.playerID, player);
                        }
                    }
                    catch (Exception e) { Console.WriteLine(e); }
                }
            });
        }

        /// <summary>
        /// 存储全部玩家数据到文件里
        /// </summary>
        public Task SaveAll()
        {
            return Task.Run(() =>
            {
                foreach (var p in PlayerInfos.Values)
                {
                    Save(p).Wait();
                }
            });
        }
        
        /// <summary>
        /// 存储单个玩家的数据到文件里
        /// </summary>
        public Task Save(Player player)
        {
            if (player.playerID == string.Empty)
                throw new Exception("NetPlayer的playerID字段必须赋值，playerID就是Account的基值");
            return Task.Run(() =>
            {
                string path = AppDomain.CurrentDomain.BaseDirectory;
                if (!Directory.Exists(path + PlayersPath))
                    Directory.CreateDirectory(path + PlayersPath);
                if (!Directory.Exists(path + PlayersPath + player.playerID))
                    Directory.CreateDirectory(path + PlayersPath + player.playerID);
                string path1 = path + PlayersPath + player.playerID + "/PlayerInfo.data";
                FileStream fileStream;
                if (!File.Exists(path1))
                    fileStream = new FileStream(path1, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                else
                    fileStream = new FileStream(path1, FileMode.Truncate, FileAccess.ReadWrite, FileShare.ReadWrite);
                var bytes = Share.NetConvert.Serialize(player);
                fileStream.Write(bytes, 0, bytes.Length);
                fileStream.Close();
            });
        }

        /// <summary>
        /// 删除磁盘里面的单个用户的全部数据
        /// </summary>
        public Task Delete(Player player)
        {
            return Task.Run(() => {
                string path = AppDomain.CurrentDomain.BaseDirectory;
                if (!Directory.Exists(path + PlayersPath))
                    return;
                if (!Directory.Exists(path + PlayersPath + player.playerID))
                    return;
                string path1 = path + PlayersPath + player.playerID;
                if (Directory.Exists(path1))
                    Directory.Delete(path1, true);
            });
        }

        /// <summary>
        /// 添加网络玩家到数据库
        /// </summary>
        /// <param name="player"></param>
        public void AddPlayer(Player player)
        {
            AddPlayer(player.playerID, player);
        }

        /// <summary>
        /// 添加网络玩家到数据库
        /// </summary>
        /// <param name="playerID"></param>
        /// <param name="player"></param>
        public void AddPlayer(string playerID, Player player)
        {
            PlayerInfos.TryAdd(playerID, player);
        }

        /// <summary>
        /// 是否包含玩家ID
        /// </summary>
        /// <param name="playerID"></param>
        /// <returns></returns>
        public bool Contains(string playerID)
        {
            return PlayerInfos.ContainsKey(playerID);
        }

        /// <summary>
        /// 数据库是否已经有这个playerID账号?
        /// </summary>
        /// <param name="playerID"></param>
        /// <returns></returns>
        public bool HasAccout(string playerID)
        {
            return PlayerInfos.ContainsKey(playerID);
        }

        /// <summary>
        /// 尝试移除网络玩家
        /// </summary>
        /// <param name="player"></param>
        public void Remove(Player player)
        {
            Remove(player.playerID);
        }

        /// <summary>
        /// 尝试移除网络玩家
        /// </summary>
        /// <param name="playerID"></param>
        public void Remove(string playerID)
        {
            PlayerInfos.TryRemove(playerID, out Player t);
            Delete(t);
        }
    }
}