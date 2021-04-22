using Net.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Entity
{
    /// <summary>
    /// 帧同步实体组建
    /// </summary>
    [Serializable]
    public class FrameEntity
    {
        /// <summary>
        /// 操作帧列表
        /// </summary>
        public List<FrameData> frameDatas = new List<FrameData>();

        /// <summary>
        /// 添加玩家操作帧数据
        /// </summary>
        /// <param name="frameID"></param>
        /// <param name="frameTime"></param>
        /// <param name="func"></param>
        /// <param name="pars"></param>
        public void AddFrameData(int frameID, int frameTime, string func, params object[] pars) 
        {
            frameDatas.Add(new FrameData(frameID, frameTime, func, pars));
        }

        /// <summary>
        /// 添加玩家操作帧数据
        /// </summary>
        /// <param name="data"></param>
        public void AddFrameData(FrameData data)
        {
            frameDatas.Add(data);
        }
    }
}
