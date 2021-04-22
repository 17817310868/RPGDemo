using JetBrains.Annotations;
using Server.GameSys;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class TeamSys
    {
        private static TeamSys instance;
        public static TeamSys Instance
        {
            get
            {
                if (instance == null)
                    instance = new TeamSys();
                return instance;
            }
        }

        public TeamSys()
        {
            EventSys.Instance.AddListener("UpdateTeamFollow", (objs) =>
            {
                //Console.WriteLine("刷新队伍");
                UpdateFollowTarget(objs[0] as Team);
                //UpdateTeamInfo(objs[0] as Team);
            });
        }

        //以队伍的Guid为键存储当前存在的所有队伍
        Dictionary<string, Team> teamsDic = new Dictionary<string, Team>();


        /// <summary>
        /// 创建队伍
        /// </summary>
        /// <param name="player"></param>
        public void CreateTeam(Player player)
        {
            Team team = new Team(Guid.NewGuid().ToString());
            if (teamsDic.TryGetValue(team.Guid,out Team teamTemp))
            {
                Console.WriteLine($"{team.Guid}该id对应的队伍已存在");
                return;
            }
            teamsDic.Add(team.Guid, team);
            //RoleSys.Instance.GetMainRole(player).ChangeTeam(team.Guid);
            //team.ChangeLeader(player);
            team.AddPlayer(player);
            //更新客户端信息
        }

        /// <summary>
        /// 获取队伍信息
        /// </summary>
        /// <param name="player"></param>
        public void GetTeamInfo(Player player)
        {
            if (player.mainRole.TeamId == "-1")
                return;
            Team team = GetTeam(player.mainRole.TeamId);
            UpdateTeamInfo(team);
            //Player[] players = team.GetAllPlayers();
            //List<string> playersGuid = new List<string>();
            //for(int i = 0; i < players.Length; i++)
            //{
            //    playersGuid.Add(players[i].account);
            //}
            //bool isLoader = false;
            //if (player.account == team.GetLeader().account)
            //    isLoader = true;
            //ServerSys.Instance.Send(player, "UpdateTeamInfo", new S2C_TeamInfo(playersGuid, isLoader));
        }

        /// <summary>
        /// 向队伍成员客户端更新队伍信息
        /// </summary>
        /// <param name="team"></param>
        public void UpdateTeamInfo(Team team)
        {
            Player[] players = team.GetAllPlayers();
            List<string> Guids = new List<string>();
            //向所有队伍成员客户端发送并刷新队伍成员信息
            for (int i = 0; i < players.Length; i++)
            {
                Guids.Add(players[i].account);
            }
            foreach (Player p in players)
            {
                if (p.account == team.GetLeader().account)
                {
                    ServerSys.Instance.Send(p,"CallLuaFunction",
                        "MainUICtrl.UpdateTeamMember", new S2C_TeamInfo(Guids, true));
                }
                else
                {
                    ServerSys.Instance.Send(p,"CallLuaFunction",
                        "MainUICtrl.UpdateTeamMember", new S2C_TeamInfo(Guids, false));
                }
            }
        }

        /// <summary>
        /// 获取Guid对应的队伍
        /// </summary>
        /// <param name="Guid"></param>
        /// <returns></returns>
        public Team GetTeam(string Guid)
        {
            if(!teamsDic.TryGetValue(Guid,out Team team))
            {
                Console.WriteLine($"{Guid}该id对应的队伍不存在");
                return null;
            }
            return team;
        }

        /// <summary>
        /// 申请进入队伍
        /// </summary>
        /// <param name="player"></param>
        /// <param name="teamId"></param>
        public void JoinTeamRequest(Player player,C2S_JoinTeamRequest C2S_joinTeamRequest)
        {
            string teamId = ServerSys.Instance.Players[C2S_joinTeamRequest.playerId].mainRole
                .TeamId;
            Player leader = ServerSys.Instance.Players[C2S_joinTeamRequest.playerId];
            if (teamId != "-1")
            {
                if (!teamsDic.TryGetValue(teamId, out Team team))
                {
                    Console.WriteLine($"{teamId}该id对应的队伍不存在");
                    return;
                }
                if (team.PlayerCount >= 5)
                {
                    Console.WriteLine($"{teamId}该id对应的队伍人数已满");
                    return;
                }
                leader = teamsDic[teamId].GetLeader();
            }

            //创建一条用于发送给队长客户端得消息
            S2C_JoinTeamRequest S2C_joinTeamRequest = new S2C_JoinTeamRequest(player.playerID);
            //向该队伍队长客户端发送加入队伍申请
            ServerSys.Instance.Send(leader,"CallLuaFunction",
                "MessageCtrl.JoinRequest", S2C_joinTeamRequest);
        }

        /// <summary>
        /// 进队请求回复
        /// </summary>
        /// <param name="player"></param>
        /// <param name="result"></param>
        /// <param name="playerId"></param>
        public void JoinTeamReply(Player player,C2S_JoinTeamReply C2S_joinTeamReply)
        {
            S2C_JoinTeamReply S2C_joinTeamReply = new S2C_JoinTeamReply(C2S_joinTeamReply.playerId,
                C2S_joinTeamReply.result);

            ServerSys.Instance.Send(ServerSys.Instance.Players[C2S_joinTeamReply.playerId],
                "CallLuaFunction", "MessageCtrl.JoinReply", S2C_joinTeamReply);

            if (!C2S_joinTeamReply.result)
                return;
            if(player.mainRole.TeamId == "-1")
            {
                CreateTeam(player);
                UpdateLeader(player, true);

            }
            Team team = GetTeam(player.mainRole.TeamId);
            team.AddPlayer(ServerSys.Instance.Players[C2S_joinTeamReply.playerId]);
            UpdateFollowTarget(team);
            Console.WriteLine("成功入伍");
            UpdateTeamInfo(team);
        }

        /// <summary>
        /// 邀请目标进入队伍
        /// </summary>
        /// <param name="player"></param>
        /// <param name="C2S_visiteRequest"></param>
        public void VisiteTeamRequest(Player player, C2S_VisiteRequest C2S_visiteRequest)
        {
            Player target = ServerSys.Instance.Players[C2S_visiteRequest.playerId];
            ServerSys.Instance.Send(target,"CallLuaFunction", "MessageCtrl.VisiteRequest", 
                new S2C_VisiteRequest(player.account));
        }

        public void VisiteTeamReply(Player player, C2S_VisiteReply C2S_visiteReply)
        {
            Player target = ServerSys.Instance.Players[C2S_visiteReply.playerId];
            ServerSys.Instance.Send(target,"CallLuaFunction", "MessageCtrl.VisiteReply",
                new S2C_VisiteReply(player.account, C2S_visiteReply.result));
            if (!C2S_visiteReply.result)
                return;

            if (target.mainRole.TeamId == "-1")
            {
                CreateTeam(target);
                UpdateLeader(target, true);
            }
            Team team = GetTeam(target.mainRole.TeamId);
            team.AddPlayer(player);
            UpdateFollowTarget(team);
            Console.WriteLine("成功入伍");
            UpdateTeamInfo(team);

        }

        /// <summary>
        /// 退出队伍
        /// </summary>
        /// <param name="player"></param>
        public void ExitTeam(Player player)
        {

            if(!teamsDic.TryGetValue(player.mainRole.TeamId,out Team team))
            {
                Console.WriteLine($"{player.mainRole.TeamId}该id对应的队伍不存在");
                return;
            }

            if (player.account == team.GetLeader().account)
                UpdateLeader(player, false);
            team.RemovePlayer(player);
            ServerSys.Instance.Send(player, "CallLuaFunction", "MainUICtrl.UpdateTeamMember"
                , new S2C_TeamInfo(new List<string>(), false));
            player.mainRole.ChangeState(RoleState.Idle);
            foreach(Player p in (player.Scene as Scene).Players)
            {
                ServerSys.Instance.Send(p, "ChangeIdle", player.account);
            }
            if (team.PlayerCount == 1)
            {
                UpdateLeader(team.GetLeader(), false);
                ServerSys.Instance.Send(team.GetLeader(), "CallLuaFunction",
                        "MainUICtrl.UpdateTeamMember", new S2C_TeamInfo(new List<string>(),false));
                team.GetLeader().mainRole.ChangeState(RoleState.Idle);
                foreach (Player p in (team.GetLeader().Scene as Scene).Players)
                {
                    ServerSys.Instance.Send(p, "ChangeIdle", team.GetLeader().account);
                }
                team.Clear();
                teamsDic.Remove(player.mainRole.TeamId);
                return;
            }
            
            UpdateLeader(team.GetLeader(), true);
            UpdateFollowTarget(team);
            UpdateTeamInfo(team);

        }

        /// <summary>
        /// 切换传入的客户端为队长
        /// </summary>
        /// <param name="player"></param>
        public void ChangeLeader(Player player)
        {
            Team team = teamsDic[player.mainRole.TeamId];
            UpdateLeader(team.GetLeader(),false);
            Console.WriteLine(team.GetLeader().account);
            team.ChangeLeader(player);
            Console.WriteLine(team.GetLeader().account);
            UpdateLeader(team.GetLeader(), true);
            UpdateFollowTarget(team);
            UpdateTeamInfo(team);
        }

        /// <summary>
        /// 向场景内玩家更新队长图标
        /// </summary>
        /// <param name="player"></param>
        /// <param name="isLoader"></param>
        public void UpdateLeader(Player player,bool isLeader)
        {
            foreach(Player p in (player.Scene as Scene).Players)
            {
                ServerSys.Instance.Send(p,"CallLuaFunction", "HeadMgrCtrl.UpdateLeader",
                    new S2C_UpdateLeader(player.account,
                    isLeader));
            }
        }

        /// <summary>
        /// 向场景内所有玩家更新队伍中成员的跟随目标及状态
        /// 队长为自动活动，队员跟随前一名队员
        /// </summary>
        /// <param name="team"></param>
        public void UpdateFollowTarget(Team team)
        {
            Player[] players = team.GetAllPlayers();
            Console.WriteLine("新队长" + players[0].account);
            foreach (Player player in (players[0].Scene as Scene).Players)
            {
                ServerSys.Instance.Send(player, "ChangeIdle", players[0].account);
                for(int i = 1; i < players.Length; i++)
                {
                    ServerSys.Instance.Send(player, "ChangeFollowState",
                        new S2C_FollowInfo(players[i].account, players[i]
                        .mainRole.MoveSpeed, players[i - 1].account));
                }
            }
            players[0].mainRole.ChangeState(RoleState.Idle);
            for (int i = 1; i< players.Length; i++)
            {
                players[i].mainRole.ChangeState(RoleState.Follow);
            }
        }
    }

    class Team
    {
        public Team(string guid)
        {
            this.guid = guid;
            players = new List<Player>();
        }
        private string guid;
        private int playerCount = 0;
        private int maxCount = 5;
        private List<Player> players;

        public string Guid { get { return guid; } }
        public int PlayerCount { get { return playerCount; } }
        public int MaxCount { get { return MaxCount; } }

        /// <summary>
        /// 往队伍中添加玩家
        /// </summary>
        /// <param name="player"></param>
        public void AddPlayer(Player player)
        {
            if(playerCount >= maxCount)
            {
                Console.WriteLine($"{Guid}该id对应的队伍人数已满");
            }
            //更新人物得所属队伍Id
            player.mainRole.ChangeTeam(guid);
            players.Add(player);
            playerCount++;
            //如果队伍中只有自身一人，则设置自身为队长
            if (playerCount == 1)
            {
                player.mainRole.ChangeLeader(true);
            }
        }

        /// <summary>
        /// 往队伍中删除玩家
        /// </summary>
        /// <param name="player"></param>
        public void RemovePlayer(Player player)
        {
            if (playerCount < 1)
            {
                Console.WriteLine($"{guid}该id对应的队伍不存在");
                return;
            }
            if (playerCount > 1)
            {
                //如果该玩家是队长(也就是索引为0得玩家)，则把队长顺位给索引为1得玩家
                if (player.playerID == players[0].playerID)
                {
                    players[0].mainRole.ChangeLeader(false);
                    players[1].mainRole.ChangeLeader(true);
                }
            }
            if (!players.Remove(player))
            {
                Console.WriteLine($"{player.playerID}该id对应的玩家不存在该队伍");
                return;
            }
            //更新队伍成员数量
            playerCount--;
            //将该玩家得所属队伍id设置为无效
            player.mainRole.ChangeTeam("-1");
            player.mainRole.ChangeLeader(false);
        }

        /// <summary>
        /// 获取队伍中的所有玩家
        /// </summary>
        /// <returns></returns>
        public Player[] GetAllPlayers()
        {
            if (playerCount <= 0)
            {
                Console.WriteLine($"{Guid}该id对应的队伍不存在");
            }
            return players.ToArray();
        }

        /// <summary>
        /// 获取队长玩家
        /// </summary>
        /// <returns></returns>
        public Player GetLeader()
        {
            if (players.Count < 1)
            {
                Console.WriteLine($"{Guid}该id对应的队伍不存在");
                return null;
            }
            return players[0];
        }

        /// <summary>
        /// 切换队长，队长为队伍成员列表的首位
        /// </summary>
        /// <param name="player"></param>
        public void ChangeLeader(Player player)
        {
            if(PlayerCount == 1)
            {
                players[0].mainRole.ChangeLeader(true);
                return;
            }
            if (!players.Remove(player))
            {
                Console.WriteLine($"{player.id}该id不属于该队伍");
                return;
            }
            players[0].mainRole.ChangeLeader(false);
            players.Insert(0,player);
            player.mainRole.ChangeLeader(true);

        }

        public void Clear()
        {
            if(players.Count > 0)
            {
                players[0].mainRole.ChangeLeader(false);
                foreach(Player player in players)
                {
                    player.mainRole.ChangeTeam("-1");
                }
                players.Clear();
            }
            playerCount = 0;
        }
    }
}
