namespace Net.Client
{
    using System.Threading.Tasks;
    using System.Threading;
    using Net.Share;
    using System;

    /// <summary>
    /// 检测网络状况
    /// </summary>
    [Serializable]
    public class NetworkCondition
    {
        private int frame;
        /// <summary>
        /// 延时毫秒时间
        /// </summary>
        private int delayTime;
        private DateTime time;
        private bool start = false;
        /// <summary>
        /// 显示FPS和网络延迟 参数1:FPS和延迟字符串 参数2:帧率 参数2:网络延迟
        /// </summary>
        public Action<string, int, int> ShowFPS;

        public void Start()
        {
            Start(NetClientBase.Instance);
        }

        public void Start(NetClientBase client)
        {
            if (start)
                return;
            start = true;
            client.AddRpcHandle(this);
            Task.Run(() =>
            {
                while (this != null)
                {
                    frame = 0;
                    Thread.Sleep(1000);
                    time = DateTime.Now;
                    client.Send(NetCmd.LocalCmd, "NetworkDelay", frame);

                }
            });
        }

        //通过unity的Update方法调用
        public void Update()
        {
            frame++;
        }

        [Rpc]
        private void NetworkDelay(int frame)
        {
            delayTime = DateTime.Now.Subtract(time).Milliseconds;
            var text = "FPS:" + frame + "  " + delayTime + "ms";
            ShowFPS?.Invoke(text, frame, delayTime);
        }
    }
}