using Net.Server;
using Net.Share;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Net.Entity
{
    public class GDFramework
    {
        public static bool IsRun = false;

        public static void Run<Player, Scene>(IServerHandle<Player, Scene> server, int time = 20) where Player : NetPlayer where Scene : NetScene<Player>
        {
            if (IsRun)
                throw new Exception("场景管理器已经运行!");
            IsRun = true;
            Task.Run(() => {
                while (true) {
                    Thread.Sleep(time);
                    var scenes = server.Scenes;
                    foreach (var scene in scenes) {
                        scene.Value.Update(server);
                    }
                }
            });
        }
    }
}