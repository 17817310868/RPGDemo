/*
 * ===================================================================
 * 
 *          projectName:
 *              项目名称:拱猪
 *                  
 *          title:
 *              标题:技能信息系统
 *          
 *          description:
 *              功能描述:管理所有玩家的技能信息
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
    class SkillInfoSys
    {

        private static SkillInfoSys instance;
        public static SkillInfoSys Instance
        {
            get
            {
                if (instance == null)
                    instance = new SkillInfoSys();
                return instance;
            }
        }

        /// <summary>
        /// 初始化技能
        /// </summary>
        /// <param name="player"></param>
        public void InitSkill(Player player)
        {
            SkillData skillDatas = DBSys.Instance.GetData<SkillData>("account", player.account);
            if (skillDatas == null)
                return;
            List<S2C_AddSkill> skills = new List<S2C_AddSkill>();
            for(int i = 0; i < skillDatas.skills.Length; i++)
            {
                skills.Add(new S2C_AddSkill(skillDatas.skills[i]));
            }
            player.mainRole.Skills.skills = skillDatas.skills;
            ServerSys.Instance.Send(player, "CallLuaFunction", "SkillCtrl.AddSkills", new S2C_AddSkills(skills));
        }

        /// <summary>
        /// 学习技能
        /// </summary>
        /// <param name="player"></param>
        /// <param name="C2S_learnSkill"></param>
        public void LearnSkill(Player player,C2S_LearnSkill C2S_learnSkill)
        {
            if (player.mainRole.Skills.IsExist(C2S_learnSkill.skillId))
                return;
            int skillId = (C2S_learnSkill.skillId / 100 * 100) + (C2S_learnSkill.skillId / 10 % 10 * 10);
            SkillConfig config = ConfigSys.Instance.GetConfig<SkillConfig>(skillId);
            if (player.mainRole.Silver < config.upgradeMoney)
                return;
            if (player.mainRole.Level < config.upgradeLevel)
                return;
            player.mainRole.ReduceSilver(config.upgradeMoney);
            player.mainRole.Skills.AddSkill(skillId+1);
            RoleSys.Instance.UpdatePlayerMainRole(player);
            ServerSys.Instance.Send(player, "CallLuaFunction", "SkillCtrl.AddSkill", new S2C_AddSkill(skillId+1));
        }

        /// <summary>
        /// 升级技能
        /// </summary>
        /// <param name="player"></param>
        /// <param name="C2S_upgradeSkill"></param>
        public void UpgradeSkill(Player player,C2S_UpgradeSkill C2S_upgradeSkill)
        {
            if (!player.mainRole.Skills.IsExistSkillLevel(C2S_upgradeSkill.skillId))
                return;
            SkillConfig config = ConfigSys.Instance.GetConfig<SkillConfig>(C2S_upgradeSkill.skillId);
            if (player.mainRole.Silver < config.upgradeMoney)
                return;
            if (player.mainRole.Level < config.upgradeLevel)
                return;

            player.mainRole.ReduceSilver(config.upgradeMoney);
            player.mainRole.Skills.UpdateSkill(C2S_upgradeSkill.skillId+1);
            RoleSys.Instance.UpdatePlayerMainRole(player);
            ServerSys.Instance.Send(player, "CallLuaFunction", "SkillCtrl.UpdateSkill", new S2C_UpdateSkill(
                C2S_upgradeSkill.skillId+1));

        }

        public void SaveSkill(Player player)
        {
            DBSys.Instance.DeleteAllDatas<SkillData>("account", player.account);
            DBSys.Instance.InsertData<SkillData>(new SkillData(player.account, player.mainRole.Skills.skills));
        }
    }

    public class SkillInfo
    {
        public SkillInfo()
        {
            skills = new int[8];
        }
        public int[] skills;

        /// <summary>
        /// 添加技能
        /// </summary>
        /// <param name="player"></param>
        /// <param name="skillId"></param>
        public void AddSkill(int skillId)
        {
            int shi = skillId / 10 % 10;
            if (skills[shi] != 0)
                return;
            skills[shi] = skillId;
        }

        /// <summary>
        /// 移除技能
        /// </summary>
        /// <param name="skillId"></param>
        void RemoveSkill(int skillId)
        {
            int shi = skillId / 10 % 10;
            skills[shi] = 0;
        }

        public void UpdateSkill(int skillId)
        {
            RemoveSkill((skillId));
            AddSkill(skillId);
        }

        /// <summary>
        /// 添加多个技能
        /// </summary>
        /// <param name="skillsId"></param>
        public void AddSkills(List<int> skillsId)
        {
            foreach(int skillId in skillsId)
            {
                AddSkill(skillId);
            }
        }

        /// <summary>
        /// 判断该玩家是否拥有此技能
        /// </summary>
        /// <param name="player"></param>
        /// <param name="skillId"></param>
        /// <returns></returns>
        public bool IsExist(int skillId)
        {
            int shi = skillId / 10 % 10;
            if (skills[shi] == 0)
                return false;
            return true;
        }

        /// <summary>
        /// 判断玩家是否拥有改技能等级
        /// </summary>
        /// <param name="skillId"></param>
        /// <returns></returns>
        public bool IsExistSkillLevel(int skillId)
        {
            for (int i = 0;i < skills.Length; i++)
            {
                if (skills[i] == skillId)
                    return true;
            }
            return false;
        }
    }
}
