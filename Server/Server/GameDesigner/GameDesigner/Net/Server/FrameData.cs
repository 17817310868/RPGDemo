using Net.Share;

namespace Net.Server
{
    /// <summary>
    /// 帧数据
    /// </summary>
    public struct FrameData
    {
        /// <summary>
        /// 帧索引
        /// </summary>
        public int frameID;
        /// <summary>
        /// 帧时间ms
        /// </summary>
        public int frameTime;
        /// <summary>
        /// 帧函数名
        /// </summary>
        public string func;
        /// <summary>
        /// 帧函数参数
        /// </summary>
        //public object[] pars;
        public byte[][] pars;

        /// <summary>
        /// 构造帧数据
        /// </summary>
        /// <param name="frameID"></param>
        /// <param name="frameTime"></param>
        /// <param name="func"></param>
        /// <param name="pars"></param>
        public FrameData(int frameID, int frameTime, string func, params object[] pars)
        {
            this.frameID = frameID;
            this.frameTime = frameTime;
            this.func = func;
            this.pars = new byte[pars.Length][];
            for (int i = 0; i < pars.Length; i++) {
                var parBts = NetConvert.Serialize(pars[i]);
                this.pars[i] = parBts;
            }
            //this.pars = pars;
        }

        public object[] GetPars {
            get {
                object[] pars1 = new object[pars.Length];
                for (int i = 0; i < pars.Length; i++)
                {
                    var parBts = NetConvert.Deserialize(pars[i]);
                    pars1[i] = parBts.pars[0];
                }
                return pars1;
            }
        }
    }
}