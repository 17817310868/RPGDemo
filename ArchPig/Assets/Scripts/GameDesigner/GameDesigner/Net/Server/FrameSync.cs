using Net.Share;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Net.Server
{
    /// <summary>
    /// 帧同步案例
    /// </summary>
    public class FrameSync : NetBehaviour<NetPlayer, NetScene<NetPlayer>>
    {
        public bool replay = false;
        public DateTime time;
        public int frameID;
        public List<FrameData> frameDatas = new List<FrameData>();

        /// <summary>
        /// 开始游戏
        /// </summary>
        [RPCFun]
        public void Start()
        {
            time = DateTime.Now;//记录时间帧
        }

        /// <summary>
        /// 记录玩家操作帧,这里以位置为例
        /// </summary>
        [RPCFun(NetCmd.SafeCall)]
        private void InputFrame(NetPlayer player, UnityEngine.Vector3 position)
        {
            var data = new FrameData(frameID, DateTime.Now.Subtract(time).Milliseconds, "InputFrame", position);
            time = DateTime.Now;
            frameID++;
            frameDatas.Add(data);
        }

        /// <summary>
        /// 回放帧
        /// </summary>
        /// <param name="player"></param>
        [RPCFun(NetCmd.SafeCall)]
        public void Replay(NetPlayer player)
        {
            if (replay)
                return;
            replay = true;
            Task.Run(()=> 
            {
                int frameID = 0;
                DateTime time = DateTime.Now;
                while (frameID < frameDatas.Count)
                {
                    if (DateTime.Now >= time)
                    {
                        time = DateTime.Now.AddMilliseconds(frameDatas[frameID].frameTime);
                        NetServerBase<NetPlayer, NetScene<NetPlayer>>.Instance.Send(player, frameDatas[frameID].func, frameDatas[frameID].pars);
                        frameID++;
                    }
                }
                replay = false;
                frameDatas.Clear();
            });
        }
    }
}
