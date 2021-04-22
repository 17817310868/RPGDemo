namespace Net.Client
{
    using UnityEngine;
    using Net.Share;

    /// <summary>
    /// 帧同步例子
    /// </summary>
    public class FrameSync : NetBehaviour
    {
        private string str = "开始游戏";
        private bool start = false;
        private int index;

        void Update()
        {
            if (NetClientBase.Instance == null)
                return;
            if (!NetClientBase.Instance.Connected)
                return;
            
            if (start)
            {
                if (Input.GetAxis("Horizontal") != 0 | Input.GetAxis("Vertical") != 0)
                {
                    var hor = Input.GetAxis("Horizontal");
                    var ver = Input.GetAxis("Vertical");
                    Send("InputFrame", transform.position);
                    transform.Translate(new Vector3(hor, 0, ver) * 5 * Time.deltaTime);
                }
            }
        }

        private void OnGUI()
        {
            if (GUI.Button(new Rect(50,50,200,100), str))
            {
                switch (index)
                {
                    case 0:
                        start = true;
                        Send("Start");
                        str = "结束游戏";
                        index = 1;
                        break;
                    case 1:
                        start = false;
                        str = "回放游戏";
                        index = 2;
                        break;
                    case 2:
                        Send("Replay");
                        str = "开始游戏";
                        index = 0;
                        break;
                }
            }
        }

        [RPCFun]
        private void InputFrame(Vector3 position)
        {
            transform.position = position;
        }
    }
}