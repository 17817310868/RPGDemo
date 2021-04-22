namespace Net.Entity
{
    public class Command : Share.NetCmd
    {
        /// <summary>
        /// 帧同步操作命令
        /// </summary>
        public const byte SyncOperations = 17;
        /// <summary>
        /// 客户端操作帧
        /// </summary>
        public const byte Operation = 18;
        /// <summary>
        /// 玩家运动命令
        /// </summary>
        public const byte Movement = 19;
        /// <summary>
        /// 创建玩家命令
        /// </summary>
        public const byte CreatePlayer = 20;
        /// <summary>
        /// 玩家攻击命令
        /// </summary>
        public const byte Attack = 21;
        /// <summary>
        /// 同步生命值
        /// </summary>
        public const byte SyncHealth = 22;
    }
}