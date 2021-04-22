using Net.Server;
using Net.Share;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Net.Entity
{
    public sealed class GameManager
    {
        private static bool run;

        public static void Run<Player,Scene>(IServerHandle<Player,Scene> server, int time = 20) where Player:NetPlayer where Scene:NetScene<Player>
        {
            if (run)
                throw new Exception("场景管理器已经运行!");
            run = true;
            //server.OnRevdCustomBufferHandle += OnRevdCustomBufferHandle;
            Task.Run(()=> {
                while (true) {
                    Thread.Sleep(time);
                    var scenes = server.Scenes;
                    foreach (var scene in scenes) {
                        scene.Value.Update(server);
                    }
                }
            });
        }

        /*static void OnRevdCustomBufferHandle<Player>(Player player, byte cmd, byte[] buffer, int index, int count) where Player:NetPlayer
        {
            switch (cmd) {
                case Command.Operation:
                    NetConvert.Deserialize(buffer, index, count, (func, pars) => {
                        player.OnOperationHandle((UnityEngine.Vector3)pars[0]);
                    });
                    break;
            }
        }*/
    }
}
