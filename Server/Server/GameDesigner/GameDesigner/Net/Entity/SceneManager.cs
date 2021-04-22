using Net.EventSystem;
using Net.Server;
using Net.Share;
using System.Collections.Generic;
using System.Linq;

namespace Net.Entity
{
    public class SceneManager<Player> : NetScene<Player>, IEntityComponent where Player: NetPlayer
    {
        internal long index;
        public List<Operation> operations = new List<Operation>();//备用操作, 当玩家被移除后速度比update更新要快而没有地方收集操作指令

        public override void Update(IServerSendHandle<Player> server)
        {
            index++;
            OperationList list = new OperationList() { frame = index };
            for (int i = 0; i < Players.Count; i++) {
                var player = Players.ElementAt(i);
                var ops = player.Update();
                list.AddRange(ops);
            }
            int count = operations.Count;
            if (count > 0) {
                var ops = operations.GetRange(0, count);
                operations.RemoveRange(0, count);
                list.AddRange(ops);
            }
            server.Multicast(Players, Command.SyncOperations, "", list);
        }
    }
}
