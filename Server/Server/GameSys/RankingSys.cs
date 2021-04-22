/*
 * ===================================================================
 * 
 *          projectName:
 *              项目名称:拱猪
 *                  
 *          title:
 *              标题:排行榜系统
 *          
 *          description:
 *              功能描述:将所有玩家信息进行排序
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
using UnityEngine;

namespace Server.GameSys
{

    public enum RankType
    {
        None = 0,
        RoleLevel,
        RolePower

    }

    class RankingSys
    {

        private static RankingSys instance;
        public static RankingSys Instance
        {
            get
            {
                if (instance == null)
                    instance = new RankingSys();
                return instance;
            }
        }

        List<RoleLevelRank> roleLevelRanking = new List<RoleLevelRank>();
        List<RolePowerRank> rolePowerRanking = new List<RolePowerRank>();
        
        public RankingSys()
        {
            EventSys.Instance.AddListener("UpdateRanking", () =>
            {
                InitRanking();
            });
        }

        public void InitRanking()
        {
            List<MainRoleData> mainRoleDatas = DBSys.Instance.GetAllDatas<MainRoleData>();
            List<RoleLevelRank> mainRoleLevels = new List<RoleLevelRank>();
            List<RolePowerRank> mainRolePowers = new List<RolePowerRank>();
            for (int i = 0; i < mainRoleDatas.Count; i++)
            {
                MainRoleData mainRoleData = mainRoleDatas[i];
                mainRoleLevels.Add(new RoleLevelRank(mainRoleData.Guid,RankType.RoleLevel,
                    mainRoleData.name, mainRoleData.level));
                mainRolePowers.Add(new RolePowerRank(mainRoleData.Guid, RankType.RolePower,
                    mainRoleData.name, mainRoleData.power));
            }
            roleLevelRanking = mainRoleLevels.OrderByDescending(u => u.level).ToList();
            rolePowerRanking = mainRolePowers.OrderByDescending(u => u.power).ToList();
        }

        bool RemoveRank(string Guid,RankType type)
        {
            switch (type)
            {
                case RankType.RoleLevel:
                    for(int i = 0; i < roleLevelRanking.Count; i++)
                    {
                        if (roleLevelRanking[i].Guid != Guid)
                            continue;
                        roleLevelRanking.RemoveAt(i);
                        return true;
                    }
                    return false;
                case RankType.RolePower:
                    for (int i = 0; i < rolePowerRanking.Count; i++)
                    {
                        if (rolePowerRanking[i].Guid != Guid)
                            continue;
                        rolePowerRanking.RemoveAt(i);
                        return true;
                    }
                    return false;
            }
            return false;
        }

        public void UpdateRank(Rank rank)
        {
            RemoveRank(rank.Guid, rank.type);
            switch (rank.type)
            {
                case RankType.RoleLevel:
                    RoleLevelRank roleLevelRank = rank as RoleLevelRank;
                    for(int i = 0; i < roleLevelRanking.Count; i++)
                    {
                        if(roleLevelRank.level > roleLevelRanking[i].level)
                        {
                            roleLevelRanking.Insert(i, roleLevelRank);
                            return;
                        }
                    }
                    roleLevelRanking.Add(roleLevelRank);
                    break;
                case RankType.RolePower:
                    RolePowerRank rolePowerRank = rank as RolePowerRank;
                    for (int i = 0; i < rolePowerRanking.Count; i++)
                    {
                        if (rolePowerRank.power > rolePowerRanking[i].power)
                        {
                            rolePowerRanking.Insert(i, rolePowerRank);
                            return;
                        }
                    }
                    rolePowerRanking.Add(rolePowerRank);
                    break;
                default:
                    break;
            }
        }

        public void GetRanksInfo(Player player,C2S_CheckRank C2S_checkRank)
        {
            switch ((RankType)C2S_checkRank.rankType)
            {
                case RankType.RoleLevel:
                    ServerSys.Instance.Send(player, "CallLuaFunction", "RankCtrl.ShowRanksInfo",
                        new S2C_RoleLevelRank(roleLevelRanking));
                    break;
                case RankType.RolePower:
                    ServerSys.Instance.Send(player, "CallLuaFunction", "RankCtrl.ShowRanksInfo",
                        new S2C_RolePowerRank(rolePowerRanking));
                    break;
            }
        }
    }

    public class Rank
    {
        public Rank(string Guid,RankType type)
        {
            this.Guid = Guid;
            this.type = type;
        }
        public string Guid;
        public RankType type;
    }

    public class RoleLevelRank:Rank
    {
        public RoleLevelRank(string Guid,RankType type,string name,int level):base(Guid,type)
        {
            this.name = name;
            this.level = level;
        }
        public string name;
        public int level;
    }

    public class RolePowerRank : Rank
    {
        public RolePowerRank(string Guid,RankType type,string name,int power) : base(Guid, type)
        {
            this.name = name;
            this.power = power;
        }
        public string name;
        public int power;
    }
}
