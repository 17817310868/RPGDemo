/*
 * ===================================================================
 * 
 *          projectName:
 *              项目名称:拱猪
 *                  
 *          title:
 *              标题:技能系统
 *          
 *          description:
 *              功能描述:管理所有技能
 *              
 *          author:
 *              作者:照着教程敲出bug的程序员
 * 
 * ===================================================================
 */

using MongoDB.Driver.Core.Misc;
using Net;
using Net.Share;
using Skill;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine.Rendering;

namespace Server.GameSys
{
    class SkillSys
    {

        private static SkillSys instance;
        public static SkillSys Instance
        {
            get
            {
                if (instance == null)
                    instance = new SkillSys();
                return instance;
            }
        }

        Dictionary<int, SkillBase> skillsDic = new Dictionary<int, SkillBase>();


        public void Init()
        {
            skillsDic.Add(1000, new NormalAttack(1000));
            skillsDic.Add(1001, new StrikeBack(1001));
            skillsDic.Add(100, new WeiZhenLingXiao(100));
            skillsDic.Add(101, new WeiZhenLingXiao(101));
            skillsDic.Add(102, new WeiZhenLingXiao(102));
            skillsDic.Add(103, new WeiZhenLingXiao(103));
            skillsDic.Add(104, new WeiZhenLingXiao(104));
            skillsDic.Add(500, new PaiShanDaoHai(500));
            skillsDic.Add(501, new PaiShanDaoHai(501));
            skillsDic.Add(502, new PaiShanDaoHai(502));
            skillsDic.Add(503, new PaiShanDaoHai(503));
            skillsDic.Add(504, new PaiShanDaoHai(504));
            skillsDic.Add(510, new HengSaoQianJun(510));
            skillsDic.Add(511, new HengSaoQianJun(511));
            skillsDic.Add(512, new HengSaoQianJun(512));
            skillsDic.Add(513, new HengSaoQianJun(513));
            skillsDic.Add(514, new HengSaoQianJun(514));
            skillsDic.Add(300, new FengHuoLianTian(300));
            skillsDic.Add(301, new FengHuoLianTian(301));
            skillsDic.Add(302, new FengHuoLianTian(302));
            skillsDic.Add(303, new FengHuoLianTian(303));
            skillsDic.Add(304, new FengHuoLianTian(304));
            skillsDic.Add(310, new FanYunFuYu(400));
            skillsDic.Add(311, new FanYunFuYu(401));
            skillsDic.Add(312, new FanYunFuYu(402));
            skillsDic.Add(313, new FanYunFuYu(403));
            skillsDic.Add(314, new FanYunFuYu(404));
        }

        public void Release(int skillId,Battle battle, Battler actor, Battler[] victims, ref S2C_BattleMessage S2C_battleMessage)
        {
            skillsDic[skillId].Release(battle, actor, victims, ref S2C_battleMessage);
        }
    }
}
