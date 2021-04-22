/*
 * ===================================================================
 * 
 *          projectName:
 *              项目名称:拱猪
 *                  
 *          title:
 *              标题:签到系统
 *          
 *          description:
 *              功能描述:管理玩家签到信息
 *              
 *          author:
 *              作者:照着教程敲出bug的程序员
 * 
 * ===================================================================
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameSys
{
    class SignSys
    {
        private static SignSys instance;
        public static SignSys Instance
        {
            get
            {
                if (instance == null)
                    instance = new SignSys();
                return instance;
            }
        }

        Dictionary<string, Sign> signsDic = new Dictionary<string, Sign>();

        /// <summary>
        /// 获取签到信息
        /// </summary>
        /// <param name="player"></param>
        public void GetSignInfo(Player player)
        {
            if (!signsDic.TryGetValue(player.playerID, out Sign sign))
                signsDic.Add(player.playerID, new Sign());

            bool isCanSign;  //是否可以签到
            if(sign.time == null)  //若为空，表示玩家从未签到，即可签到
            {
                isCanSign = true;
            }
            else
            {
                //若上次签到的时间为当前时间的前一天，即可签到
                //否则不可签到
                TimeSpan span = DateTime.Now.Date - sign.time.Date;
                if (span.Days > 0 && span.Days < 1)
                {
                    if (sign.number == 7)
                        sign.number = 0;
                    isCanSign = true;
                }
                else if (span.Days > 1)
                {
                    isCanSign = true;
                    sign.number = 0;
                }
                else
                {
                    isCanSign = false;
                }
            }

            ServerSys.Instance.Send(player, "UpdateSign", new S2C_SignInfo(sign.number, isCanSign));

        }

        public void Sign(Player player)
        {
            if (!signsDic.TryGetValue(player.playerID, out Sign sign))
                return;

            TimeSpan span = DateTime.Now.Date - sign.time.Date;
            if (span.Days < 1)
                return;

            sign.number++;
            sign.time = DateTime.Now.Date;

            ServerSys.Instance.Send(player, "UpdateSign", new S2C_SignInfo(sign.number, false));
        }

    }

    public class Sign
    {
        public Sign()
        {
            number = 0;
        }
        public byte number;  //连续签到的次数
        public DateTime time;  //上次签到的时间
    }
}
