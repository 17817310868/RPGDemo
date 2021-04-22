/*
 * ===================================================================
 * 
 *          projectName:
 *              项目名称:拱猪
 *                  
 *          title:
 *              标题:控制系统
 *          
 *          description:
 *              功能描述:实现角色移动，更新角色坐标方向
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
using Net;
using Net.Share;

namespace Server.GameSys
{
    class CtrlSys
    {
        private static CtrlSys instance;
        public static CtrlSys Instance
        {
            get
            {
                if (instance == null)
                    instance = new CtrlSys();
                return instance;
            }
        }

        public void Move(Player player,C2S_MoveInfo C2S_moveInfo)
        {
            if (player.mainRole.State == RoleState.Battle)
                return;
            if (player.mainRole.State == RoleState.Follow)
                return;
            Console.WriteLine("移动消息");
            UnityEngine.Vector3 target = C2S_moveInfo.target;
            if (TeamSys.Instance.GetTeam(player.mainRole.TeamId) != null)
            {
                TeamMove(TeamSys.Instance.GetTeam(player.mainRole.TeamId), target);
            }
            player.mainRole.ChangePosition(target);
            //向当前场景的所有在线客户端广播该角色的移动目标点，并切换移动状态移动该角色
            ServerSys.Instance.Multicast((player.Scene as Scene).Players,"ChangeMoveState",
                new S2C_MoveInfo(player.mainRole.Guid, player.mainRole.MoveSpeed, target));
        }

        void TeamMove(Team team,Vector3 target)
        {
            Player[] players = team.GetAllPlayers();
            for(int i = 1;i < team.PlayerCount; i++)
            {
                players[i].mainRole.ChangePosition(target);
            }
        }
    }
}
