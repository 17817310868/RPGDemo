/*
 * ===================================================================
 * 
 *          projectName:
 *              项目名称:拱猪
 *                  
 *          title:
 *              标题:战斗系统
 *          
 *          description:
 *              功能描述:管理所有战局并处理战斗数据
 *              
 *          author:
 *              作者:照着教程敲出bug的程序员
 * 
 * ===================================================================
 */

using GameDesigner;
using Net;
using Net.Entity;
using Server.Buff;
using Server.GameSys;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

namespace Server
{
    public enum ActorType
    {
        None = 0,
        Attack,  //普通攻击
        Skill,  //使用技能
        Item,  //使用道具
        Escape,  //逃跑
    }



    public enum BattleStateEnum
    {
        Init,  //等待所有客户端初始化完战局所有信息
        Receive,  //接收玩家战斗指令
        WaitAniEnd,  //等待所有客户端动画播放结束
        BattleEnd  //战局结束
    }

    public enum BattleTypeEnum
    {
        PVE,  //人机对战
        PVP  //玩家对战
    }

    public enum BattlerType
    {
        None = 0,
        Player = 1,
        AI = 2
    }

    /// <summary>
    /// 战斗系统
    /// </summary>
    class BattleSys
    {
        private static BattleSys instance;
        public static BattleSys Instance
        {
            get
            {
                if (instance == null)
                    instance = new BattleSys();
                return instance;
            }
        }

        //以战局Guid作为标识存储当前所有战局
        Dictionary<string, Battle> battlesDic = new Dictionary<string, Battle>();

        //存储每个客户端玩家对应的战局Guid
        Dictionary<Player, string> battlesGuid = new Dictionary<Player, string>();

        //角色站位顺序
        byte[] stationOrder = new byte[20] { 2, 3, 1, 4, 0, 7, 8, 6, 9, 5, 12, 13, 11, 14, 10, 17, 18, 16, 19, 15 };

        public void MeetMonster(Player player)
        {
            MonsterConfig[] monstersConfig = ConfigSys.Instance.GetAllConfig<MonsterConfig>();
            string scene = player.sceneID;
            int monsterId = 0;
            for(int i = 0; i < monstersConfig.Length; i++)
            {
                if (monstersConfig[i].scene == scene)
                    monsterId = monstersConfig[i].id;
            }
            if (monsterId == 0)
                return;
            List<Player> team;
            if (player.mainRole.TeamId == "-1")
                team = new List<Player>() { player };
            else
                team = TeamSys.Instance.GetTeam(player.mainRole.TeamId).GetAllPlayers().ToList();
            Console.WriteLine("队伍人数：" + team.Count);
            int monsterCount = Tools._Random.Next(team.Count, team.Count + 2);
            List<RoleBase> monsters = new List<RoleBase>();
            MonsterConfig monsterConfig = ConfigSys.Instance.GetConfig<MonsterConfig>(monsterId);
            for (int i = 0;i < monsterCount; i++)
            {
                RoleBase monster = new RoleBase(Guid.NewGuid().ToString(), RoleEnum.None,-1);
                monster.ChangeName(monsterConfig.name);
                monster.ChangeLevel(monsterConfig.level);
                monster.ChangeHp(monsterConfig.hp);
                monster.ChangeMp(monsterConfig.mp);
                monster.ChangeMaxHp(monsterConfig.hp);
                monster.ChangeMaxMp(monsterConfig.mp);
                monster.ChangePhysicalAttack(monsterConfig.physicalAttack);
                monster.ChangePhysicalDefense(monsterConfig.physicalDefense);
                monster.ChangeMagicAttack(monsterConfig.magicAttack);
                monster.ChangeMagicDefense(monsterConfig.magicDefense);
                monster.ChangeSpeed(monsterConfig.speed);
                monster.ChangeExperience(monsterConfig.experience);
                monster.ChangePoisoning(monsterConfig.poisonint);
                monster.ChangePoisoningResist(monsterConfig.poisoningResist);
                monster.ChangeBurn(monsterConfig.burn);
                monster.ChangeBurnResist(monsterConfig.burnResist);
                monster.ChangeContinueHit(monsterConfig.continueHit);
                monster.ChangeStrikeBack(monsterConfig.strikeBack);
                monsters.Add(monster);
            }
            AddBattle(team, monsters, monsterId);

        }

        /// <summary>
        /// 添加一个人机对战的战局
        /// </summary>
        /// <param name="team"></param>
        /// <param name="roles"></param>
        public void AddBattle(List<Player> team, List<RoleBase> roles,int monsterId)
        {
            //Console.WriteLine(roles.Count);
            ChangeBattleState(ref team);
            List<S2C_UpdateBattle> S2C_updateBattles = new List<S2C_UpdateBattle>();
            Console.WriteLine("队伍人数：" + team.Count);
            foreach (Player player in team)
            {
                S2C_updateBattles.Add(new S2C_UpdateBattle(player.account, true));
            }
            Console.WriteLine("人数：" + (team[0].Scene as Scene).Players);
            foreach (Player player in (team[0].Scene as Scene).Players)
            {
                if (player.mainRole.State != RoleState.Battle)
                {
                    Console.WriteLine(player.mainRole.Name + "发送战斗图标");
                    ServerSys.Instance.Send(player, "CallLuaFunction",
                        "HeadMgrCtrl.OtherRoleBattle", new S2C_UpdateBattles(S2C_updateBattles));
                }
            }

            //战局Guid
            string battleGuid = Guid.NewGuid().ToString();
            //存放参战者数据
            Battler[] battlers = new Battler[20];
            //存放参战客户端
            List<Player> players = new List<Player>();
            //存放第一支队伍存活索引
            List<byte> teamOneSurvivals = new List<byte>();
            //存放第二支队伍存活索引
            List<byte> teamTwoSurvivals = new List<byte>();

            //添加参战客户端
            AddPlayers(ref players, team);
            //添加参战者数据
            AddBattlers(team, ref battlers, battleGuid, ref teamOneSurvivals, 0);

            //遍历ai角色
            for (int i = 0; i < roles.Count; i++)
            {
                RoleBase monster = roles[i];
                //计算角色站位索引
                byte positionIndex = stationOrder[i + 10];
                //给站位索引对应的参战者数据赋值
                battlers[positionIndex] = new Battler(positionIndex, monster,BattlerType.AI,monsterId);
                //将站位索引添加进队伍存活列表
                teamTwoSurvivals.Add(positionIndex);
            }
            //创建一个PVE战局
            Battle battle = new Battle(battlers, battleGuid, players, BattleTypeEnum.PVE,
                teamOneSurvivals, teamTwoSurvivals, monsterId);
            //将战局添加进字典
            battlesDic.Add(battleGuid, battle);
        }

        /// <summary>
        /// 从战局字典中删除该Guid对应的战局
        /// </summary>
        /// <param name="Guid"></param>
        public void RemoveBattle(string Guid)
        {
            if (!battlesDic.TryGetValue(Guid, out Battle battler))
            {
                Console.WriteLine(Guid + "该id对应的战局不存在，无法删除");
                return;
            }
            battlesDic.Remove(Guid);

        }

        /// <summary>
        /// 从战局参战者字典中移除参战者玩家
        /// </summary>
        /// <param name="players"></param>
        public void RemovePlayer(List<Player> players)
        {
            foreach(Player player in players)
            {
                if(!battlesGuid.TryGetValue(player, out string Guid))
                {
                    Console.WriteLine(player.account + "该id对应的玩家没有所属战局");
                    continue;
                }
                battlesGuid.Remove(player);
            }
        }

        /// <summary>
        /// 添加一个玩家对战的战局
        /// </summary>
        /// <param name="teamOne"></param>
        /// <param name="teamTwo"></param>
        public void AddBattle(List<Player> teamOne, List<Player> teamTwo)
        {
            ChangeBattleState(ref teamOne);
            ChangeBattleState(ref teamTwo);
            Console.WriteLine("队伍1的人数:" + teamOne.Count);
            Console.WriteLine("队伍2的人数:" + teamTwo.Count);
            List<S2C_UpdateBattle> S2C_updateBattles = new List<S2C_UpdateBattle>();
            foreach(Player player in teamOne)
            {
                S2C_updateBattles.Add(new S2C_UpdateBattle(player.account, true));
            }
            foreach (Player player in teamTwo)
            {
                S2C_updateBattles.Add(new S2C_UpdateBattle(player.account, true));
            }
            foreach (Player player in (teamOne[0].Scene as Scene).Players)
            {
                if(player.mainRole.State != RoleState.Battle)
                {
                    ServerSys.Instance.Send(player,"CallLuaFunction",
                        "HeadMgrCtrl.OtherRoleBattle", S2C_updateBattles);
                }
            }
            //战局Guid
            string battleGuid = Guid.NewGuid().ToString();
            //存放参战客户端
            List<Player> players = new List<Player>();
            //存放第一支队伍的存活索引
            List<byte> teamOneSurvivals = new List<byte>();
            //存放第二支队伍的存活索引
            List<byte> teamTwoSurvivals = new List<byte>();

            //添加参战客户端
            AddPlayers(ref players, teamOne);
            AddPlayers(ref players, teamTwo);
            //存放参战者数据
            Battler[] battlers = new Battler[20];
            //添加参战者数据
            AddBattlers(teamOne, ref battlers, battleGuid, ref teamOneSurvivals, 0);
            AddBattlers(teamTwo, ref battlers, battleGuid, ref teamTwoSurvivals, 10);

            //创建一个PVP战局
            Battle battle = new Battle(battlers, battleGuid, players, BattleTypeEnum.PVP, teamOneSurvivals, teamTwoSurvivals);
            //将战局存进字典
            battlesDic.Add(battleGuid, battle);
        }

        /// <summary>
        /// 添加参战队伍的信息
        /// </summary>
        /// <param name="team"></param>
        /// <param name="battlers"></param>
        /// <param name="guid"></param>
        void AddBattlers(List<Player> team, ref Battler[] battlers, string guid, ref List<byte> teamSurvivals, int positionStart)
        {
            //遍历队伍存活索引
            for (int i = 0; i < team.Count; i++)
            {
                //获取客户端对应的角色属性
                MainRole role = team[i].mainRole;
                //计算角色站位索引
                byte positionIndex = stationOrder[i + positionStart];
                //给站位索引对应的参战者数据赋值
                battlers[positionIndex] = new Battler(positionIndex, role,BattlerType.Player);
                //将站位索引添加进队伍存活列表
                teamSurvivals.Add(positionIndex);
            }
            //遍历参战客户端
            foreach (Player player in team)
            {
                //将参战客户端添加进字典

                battlesGuid.Add(player, guid);
            }
        }

        /// <summary>
        /// 添加参战客户端
        /// </summary>
        /// <param name="players"></param>
        /// <param name="p"></param>
        void AddPlayers(ref List<Player> players, List<Player> p)
        {
            //遍历队伍客户端
            foreach (Player pTemp in p)
            {
                //将队伍客户端加入参战客户端列表
                players.Add(pTemp);
            }
        }

        /// <summary>
        /// 添加战斗指令
        /// </summary>
        /// <param name="player"></param>
        /// <param name="C2S_battleCommand"></param>
        public void AddBattleCommand(Player player, C2S_BattleCommand C2S_battleCommand)
        {

            Battle battle = GetPlayerBattle(player);
            if (battle == null)
                return;

            if (battle.BattleState != BattleStateEnum.Receive)
            {
                Console.WriteLine($"{battle.Guid}该战局不处于接收战斗指令阶段");
                return;
            }

            battle.AddBattleCommand(C2S_battleCommand);
        }

        /// <summary>
        /// 往客户端对应的战局添加客户端响应
        /// </summary>
        /// <param name="player"></param>
        public void AddBattleCallback(Player player)
        {
            Battle battle = GetPlayerBattle(player);
            if (battle == null)
                return;

            battle.AddCallback();
            
        }

        /// <summary>
        /// 获取玩家对应的战局
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public Battle GetPlayerBattle(Player player)
        {
            if (!battlesGuid.TryGetValue(player, out string battleGuid))
            {
                Console.WriteLine($"{player.account}该玩家没有所属的战局");
                return null;
            }
            
            if (!battlesDic.TryGetValue(battleGuid, out Battle battle))
            {
                Console.WriteLine($"{battleGuid}该战局Guid对应的战局不存在");
                return null;
            }

            return battle;
        }

        /// <summary>
        /// 切磋请求
        /// </summary>
        /// <param name="player"></param>
        /// <param name="C2S_battleRequest"></param>
        public void BattleRequest(Player player, C2S_BattleRequest C2S_battleRequest)
        {
            string targetId = C2S_battleRequest.playerId;
            Player targetClient;
            string teamId = ServerSys.Instance.Players[targetId].mainRole.TeamId;
            if (teamId == "-1")
            {
                targetClient = ServerSys.Instance.Players[targetId];
            }
            else
            {
                targetClient = TeamSys.Instance.GetTeam(teamId).GetLeader();
            }
            //创建一条用于发送给目标客户端的请求
            S2C_BattleRequest S2C_battleRequest = new S2C_BattleRequest();
            //更新请求信息
            S2C_battleRequest.playerId = player.playerID;
            //发送给客户端，调用客户端的申请入伍函数
            ServerSys.Instance.Send(targetClient,"CallLuaFunction",
                "MessageCtrl.BattleRequest", S2C_battleRequest);

        }

        /// <summary>
        /// 切磋回复
        /// </summary>
        /// <param name="player"></param>
        /// <param name="C2S_battleReply"></param>
        public void BattleReply(Player player, C2S_BattleReply C2S_battleReply)
        {
            string playerId = C2S_battleReply.playerId;
            //获取发起切磋请求的客户端
            Player requestPlayer = ServerSys.Instance.Players[playerId];
            //创建一条用于答复请求客户端的消息
            S2C_BattleReply S2C_battleReply = new S2C_BattleReply();
            //更新答复请求信息
            S2C_battleReply.playerId = player.playerID;
            S2C_battleReply.reply = C2S_battleReply.reply;
            //向请求客户端发送，调用该客户端的答复函数
            ServerSys.Instance.Send(requestPlayer,"CallLuaFunction",
                "MessageCtrl.BattleReply", S2C_battleReply);
            //如果玩家拒绝切磋，则直接跳出方法
            if (!C2S_battleReply.reply)
            {
                return;
            }
            //获取双方队伍的所有成员
            List<Player> teamOne = new List<Player>();
            List<Player> teamTwo = new List<Player>();
            GetBattlePlayer(ref teamOne, requestPlayer);
            GetBattlePlayer(ref teamTwo, player);
            //添加一个PVP(玩家对战)的战局
            AddBattle(teamOne, teamTwo);

        }

        /// <summary>
        /// 获取队伍中的所有人进行参战
        /// </summary>
        /// <param name="team"></param>
        /// <param name="player"></param>
        void GetBattlePlayer(ref List<Player> team,Player player)
        {
            string teamOneId = player.mainRole.TeamId;
            if (teamOneId != "-1")
            {
                Player[] players = TeamSys.Instance.GetTeam(teamOneId).GetAllPlayers();
                foreach (Player client in players)
                {
                    team.Add(client);
                }
            }
            else
            {
                team.Add(player);
            }
        }

        void ChangeBattleState(ref List<Player> team)
        {
            for(int i = 0; i < team.Count; i++)
            {
                team[i].mainRole.ChangeState(RoleState.Battle);
            }
        }
    }

    /// <summary>
    /// 战局
    /// </summary>
    public class Battle
    {
        public Battle(Battler[] roles, string guid, List<Player> players, BattleTypeEnum battleType,
            List<byte> teamOneSurvivals, List<byte> teamTwoSurvivals,int monsterId = 0)
        {
            this.battlers = roles;
            this.guid = guid;
            this.players = players;
            this.battleType = battleType;
            this.teamOneSurvivals = teamOneSurvivals;
            this.teamTwoSurvivals = teamTwoSurvivals;
            teamOneCount = (byte)teamOneSurvivals.Count;
            teamTwoCount = (byte)teamTwoSurvivals.Count;
            this.monsterId = monsterId;
            //存储存活索引对应的队伍列表
            indexsDic = new Dictionary<byte, List<byte>>();
            foreach(byte index in teamOneSurvivals)
            {
                indexsDic.Add(index, teamOneSurvivals);
            }
            //Console.WriteLine(teamTwoSurvivals.Count);
            foreach(byte index in teamTwoSurvivals)
            {
                indexsDic.Add(index, teamTwoSurvivals);
            }
            //初始化战局
            InitBattle();
        }
        private string guid;
        private BattleTypeEnum battleType;
        private BattleStateEnum battleState;
        private byte round;
        private byte maxRound = 20;
        private byte teamOneCount;
        private byte teamTwoCount;
        private int monsterId;
        public string Guid { get { return guid; } }  //战局唯一标识
        public BattleTypeEnum BattleType { get { return battleType; } }  //战局类型
        public BattleStateEnum BattleState { get { return battleState; } }  //战局状态
        public byte Round { get { return round; } }  //当前回合数
        public byte MaxRound { get { return maxRound; } }  //最大回合数

        List<Player> players;  //所有参战客户端，用于发送战斗消息

        public List<byte> teamOneSurvivals;  //保存队伍1的所有存活角色的站位索引
        public List<byte> teamTwoSurvivals;  //保存队伍2的所有存活角色的站位索引

        Dictionary<byte, List<byte>> indexsDic;  //用于保存各个站位索引对应的队伍
        public Battler[] battlers;  //以站位索引为唯一标识存储参战角色

        List<S2C_BattleMessage> messages;  //存放该回合得所有战斗消息，用于发送战斗消息给客户端
        Dictionary<byte, C2S_BattleCommand> battleCommandsDic;  //以站位索引为唯一标识存储战斗指令
        public byte callbackCount;  //客户端响应数量


        /// <summary>
        /// 初始化战局
        /// </summary>
        public void InitBattle()
        {
            //初始化当前回合数;
            round = 0;
            //将战局状态设置为初始状态
            battleState = BattleStateEnum.Init;
            //将客户端响应数归零
            callbackCount = 0;
            //存储所有战斗指令
            battleCommandsDic = new Dictionary<byte, C2S_BattleCommand>();
            //存储所有发送给客户端得动画消息
            messages = new List<S2C_BattleMessage>();
            //向客户端发送战局内的所有角色站位及信息
            //存储玩家guid对应的站位索引
            Dictionary<string, byte> rolesDic = new Dictionary<string, byte>();
            //存储每个参战者需要显示及发送给客户端的信息
            List<S2C_BattlerInfo> battlersInfo = new List<S2C_BattlerInfo>();
            //遍历参战者站位索引
            foreach(byte index in indexsDic.Keys)
            {
                RoleBase role = battlers[index].Role;
                //将站位索引以角色guid为键存进字典
                rolesDic.Add(role.Guid, index);
                //创建一条客户端需要显示的信息
                S2C_BattlerInfo battleInfo = new S2C_BattlerInfo();
                //更新信息
                //Console.WriteLine(battlers[index].BattlerType);
                switch (battlers[index].BattlerType)
                {
                    case BattlerType.Player:
                        battleInfo.battlerType = (int)BattlerType.Player;
                        battleInfo.profession = role.Profession;  //职业类型(模型索引)
                        break;
                    case BattlerType.AI:
                        battleInfo.battlerType = (int)BattlerType.AI;
                        battleInfo.monsterId = battlers[index].MonsterId;
                        break;
                }
                battleInfo.positionIndex = index;  //站位索引
                battleInfo.roleName = role.Name;  //角色名称
                //判断该角色装备栏是否装备武器，若有，则给武器id赋值
                //ItemBase[] equips = (role as MainRole).Inventory.GetInventoryItems((byte)InventoryEnum.Equip);
                int weaponId = -1;
                if ((role as MainRole) != null)
                {
                    MainRole mainRole = (role as MainRole);
                    if(mainRole.Inventory.GetEquipbarEquip((byte)EquipDress.Weapon) != null)
                    {
                        weaponId = mainRole.Inventory.GetEquipbarEquip((byte)EquipDress.Weapon).ItemId;
                    }
                    //if (mainRole.Inventory.GetInventoryItems((byte)InventoryEnum.Equip) != null)
                    //{
                    //    ItemBase[] equips = mainRole.Inventory.GetInventoryItems((byte)InventoryEnum.Equip);
                    //    for (int i = 0; i < equips.Length; i++)
                    //    {
                    //        if (ConfigSys.Instance.GetConfig<EquipConfig>(equips[i].ItemId)._type == (byte)EquipDress.Weapon)
                    //            weaponId = equips[i].ItemId;
                    //    }
                    //}
                }
                battleInfo.weapon = weaponId;
                battleInfo.maxHp = role.MaxHp;
                battleInfo.hp = role.Hp;
                battleInfo.maxMp = role.MaxMp;
                battleInfo.mp = role.Mp;
                //将信息添加进信息列表
                battlersInfo.Add(battleInfo);
            }
            //创建一条初始化战局的消息
            S2C_InitBattlers initBattlers = new S2C_InitBattlers();
            initBattlers.rolesDic = rolesDic;
            initBattlers.battlersInfo = battlersInfo;
            //将该消息发送给所有参战客户端
            foreach(Player player in players)
            {
                //调用所有参战客户端的InitBattle方法
                ServerSys.Instance.Send(player, "InitBattle", initBattlers);
            }
        }

        /// <summary>
        /// 客户端响应
        /// </summary>
        public void AddCallback()
        {
            //增加客户端响应数
            callbackCount++;
            //判断战局类型
            switch (battleType)
            {
                //若为人机对战，则当客户端响应数不小于第一支队伍(玩家队伍)的人数时，继续往下执行，否则直接跳出
                case BattleTypeEnum.PVE:
                    if (callbackCount != teamOneSurvivals.Count)
                        return;
                    break;
                //若为玩家对战，则当客户端响应数不小于两支队伍人数之和时，继续往下执行，否则退出
                case BattleTypeEnum.PVP:
                    if (callbackCount != (teamOneSurvivals.Count + teamTwoSurvivals.Count))
                        return;
                    break;
            }
            //将战局状态改为接收战斗指令状态
            battleState = BattleStateEnum.Receive;
            //客户端响应数归零
            callbackCount = 0;
            //更新当前回合数
            round++;
            //遍历所有参战客户端
            foreach (Player player in players)
            {
                //发送进入下一回合的信号(开始接收战斗指令)
                ServerSys.Instance.Send(player, "NewRound",new S2C_Round(round));
            }
        }

        /// <summary>
        /// 添加战斗指令
        /// </summary>
        /// <param name="C2S_battleCommand"></param>
        public void AddBattleCommand(C2S_BattleCommand C2S_battleCommand)
        {
            //若战局状态不处于接收战斗指令状态，则直接跳出
            if (BattleState != BattleStateEnum.Receive)
                return;
            //若该行动角色索引不存在于两支队伍的存活索引列表，直接跳出
            if (!teamOneSurvivals.Contains(C2S_battleCommand.actorIndex) && !teamTwoSurvivals.Contains(C2S_battleCommand.actorIndex))
            {
                Console.WriteLine($"{C2S_battleCommand}该站位的角色已死亡，无法添加操作指令");
                return;
            }
            //若战斗指令字典中已存在该索引，则直接跳出
            if (battleCommandsDic.TryGetValue(C2S_battleCommand.actorIndex, out C2S_BattleCommand command))
            {
                Console.WriteLine($"{C2S_battleCommand.actorIndex}该站位索引本回合已存在战斗指令，请勿重复添加");
                return;
            }
            //将该战斗指令添加进战斗指令字典
            battleCommandsDic.Add(C2S_battleCommand.actorIndex, C2S_battleCommand);
            //判断战局类型
            switch (BattleType)
            {
                //若为人机对战，则当战斗指令的数量等于第一支队伍(玩家队伍)的角色数量时，处理战斗指令
                //将战局状态设置为等待动画播放结束
                case BattleTypeEnum.PVE:
                    if (battleCommandsDic.Count == teamOneSurvivals.Count)
                    {
                        for(int i = 0;i < teamTwoSurvivals.Count; i++)
                        {
                            Battler monster = battlers[teamTwoSurvivals[i]];
                            byte victim = teamOneSurvivals[Tools._Random.Next(0, teamOneSurvivals.Count)];
                            C2S_BattleCommand battlerCommand = new C2S_BattleCommand(monster.PositionIndex,
                                victim,(int)ActorType.Attack,0);
                            battleCommandsDic.Add(monster.PositionIndex, battlerCommand);

                        }
                        battleState = BattleStateEnum.WaitAniEnd;
                        HandleBattleCommand();
                    }
                    break;
                //若为玩家对战，则当战斗指令的数量等于两支队伍的角色数量之和时，处理战斗指令
                //将战局状态设置为等待动画播放结束
                case BattleTypeEnum.PVP:
                    if (battleCommandsDic.Count == (teamOneSurvivals.Count + teamTwoSurvivals.Count))
                    {
                        battleState = BattleStateEnum.WaitAniEnd;
                        HandleBattleCommand();
                    }
                    break;
            }
        }

        /// <summary>
        /// 处理战斗指令
        /// </summary>
        void HandleBattleCommand()
        {
            //新建列表，用于存储排序后的战斗指令 
            List<C2S_BattleCommand> battleCommands = new List<C2S_BattleCommand>();
            //遍历战斗指令字典
            foreach(C2S_BattleCommand command in battleCommandsDic.Values)
            {
                //当战斗列表为空时，直接将战斗指令添加
                if (battleCommands.Count == 0)
                {
                    battleCommands.Add(command);
                    continue;
                }

                //遍历战斗列表，将战斗指令插入战斗列表的对应位置，索引越大，速度越慢
                for(int i = 0;i < battleCommands.Count; i++)
                {
                    if (battlers[command.actorIndex].Role.Speed > battlers[battleCommands[i].actorIndex].Role.Speed)
                    {
                        battleCommands.Insert(i, command);
                        break;
                    }
                    if (i == battleCommands.Count - 1)
                    {
                        battleCommands.Add(command);
                        break;
                    }
                }
            }

            //清空战斗指令字典
            battleCommandsDic.Clear();

            //遍历排序完的战斗指令列表
            for(int i = 0; i < battleCommands.Count; i++)
            {
                S2C_BattleMessage message = new S2C_BattleMessage();
                C2S_BattleCommand command = battleCommands[i];
                //该指令的行动角色索引
                byte index = command.actorIndex;
                message.actorIndex = index;
                //判断该索引对应的角色是否死亡
                if (IsDead(index))
                    continue;

                //获取该玩家的战斗数据
                Battler battler = battlers[index];
                Dictionary<int,BuffBase> staticBuffs = battler.staticBuffs;
                List<int> staticBuffsId = staticBuffs.Keys.ToList();
                //处理玩家所拥有的所有静态buff
                foreach (int buffId in staticBuffsId)
                {
                    staticBuffs[buffId].Calculate(ref battler);
                    if (staticBuffs[buffId].rounds == 0)
                        battler.staticBuffs.Remove(staticBuffs[buffId].buffId);
                }

                Dictionary<int,BuffBase> actionBuffs = battler.actionBuffs;
                List<int> actionBuffsId = actionBuffs.Keys.ToList();
                foreach(int buffId in actionBuffsId)
                {
                    actionBuffs[buffId].Calculate(ref battler);
                    if (actionBuffs[buffId].rounds == 0)
                        battler.actionBuffs.Remove(actionBuffs[buffId].buffId);
                    //判断角色hp，小于零去死
                    if(battler.Role.Hp <= 1)
                    {
                        BattlerDead(ref battler);
                    }
                }

                if (isBattleEnd())
                    break;

                //更新行动者数据
                UpdataBattlerAttr(ref battler);

                if((command.actorType == (byte)ActorType.Attack) || (command.actorType == (byte)ActorType.Skill))
                {
                    //Console.WriteLine(command.victim+ "是否挂掉" + IsDead(command.victim));
                    if (IsDead(command.victim))
                        continue;
                }

                //判断行动者行动类型
                switch (command.actorType)
                {
                    //如果为普通攻击
                    case (byte)ActorType.Attack:
                        //更新战斗消息
                        message.actorType = (byte)ActorType.Attack;
                        message.paramId = 1001;
                        message.actorIndex = command.actorIndex;
                        if (IsDead(command.victim))
                            continue;
                        message.victims = new byte[] { command.victim };
                        //获得目标
                        Battler victim = battlers[command.victim];
                        //更新目标数据
                        UpdataBattlerAttr(ref victim);
                        //释放技能（普通攻击）
                        SkillSys.Instance.Release(1000, this, battler, new Battler[] { victim }, ref message);
                        break;
                    //如果为使用技能
                    case (byte)ActorType.Skill:
                        //更新战斗消息
                        message.actorType = (byte)ActorType.Skill;
                        message.paramId = command.paramId;
                        if (!(battler.Role as MainRole).Skills.IsExistSkillLevel(message.paramId))
                            continue;
                        SkillConfig config = ConfigSys.Instance.GetConfig<SkillConfig>(message.paramId);
                        if (battler.Role.Mp < config.consume)
                            continue;
                        battler.Role.ReduceMp(config.consume);
                        if (IsDead(command.victim))
                            continue;
                        //根据技能目标个数获取技能所有目标的站位索引
                        byte[] targets = GetTargets(command.victim, (byte)config.count);
                        //获取所有目标数据
                        Battler[] victims = new Battler[targets.Length];
                        for(int z = 0;z < victims.Length; z++)
                        {
                            victims[z] = battlers[targets[z]];
                        }
                        //更新所有目标数据
                        for(int x = 0;x <victims.Length; x++)
                        {
                            UpdataBattlerAttr(ref victims[x]);
                        }
                        //释放技能
                        SkillSys.Instance.Release(command.paramId, this, battler, victims, ref message);
                        break;
                    //如果为使用物品
                    case (byte)ActorType.Item:
                        message.actorType = (byte)ActorType.Item;
                        message.paramId = command.paramId;
                        if ((battler.Role as MainRole).Inventory.GetItemsGuid(message.paramId) == null)
                            continue;
                        if (IsDead(command.victim))
                            continue;
                        ItemSys.Instance.Battle(message.paramId, this, battler, battlers[command.victim],ref message);
                        break;
                    //如果是逃跑
                    case (byte)ActorType.Escape:
                        //message.actorType = (byte)ActorType.Escape;
                        //Player player = ServerSys.Instance.Players[(battler.Role as MainRole).Guid];
                        //players.Remove(player);
                        //indexsDic[battler.PositionIndex].Remove(battler.PositionIndex);
                        //TeamSys.Instance.ExitTeam(player);
                        break;
                }

                messages.Add(message);

                if (isBattleEnd())
                    break;
            }
            foreach (Player player in players)
            {
                ServerSys.Instance.Send(player, "ShowBattleAnim", messages);
            }
            messages.Clear();

            if(battleState == BattleStateEnum.BattleEnd)
            {
                Console.WriteLine("战斗结束");
                List<Team> teams = new List<Team>();
                S2C_BattleSettle S2C_battleSettle = new S2C_BattleSettle();
                List<S2C_UpdateBattle> S2C_updateBattles = new List<S2C_UpdateBattle>();
                
                if (teamOneSurvivals.Count == 0)
                {
                    S2C_battleSettle.winTeam = 2;
                }
                else
                {
                    S2C_battleSettle.winTeam = 1;
                }
                foreach (Player player in players)
                {
                    ServerSys.Instance.Send(player, "BattleEnd", S2C_battleSettle);
                    player.mainRole.ChangeState(RoleState.Idle);
                    if (player.mainRole.TeamId != "-1")
                        if (!teams.Contains(TeamSys.Instance.GetTeam(player.mainRole.TeamId)))
                            teams.Add(TeamSys.Instance.GetTeam(player.mainRole.TeamId));
                    RoleSys.Instance.UpdatePlayerMainRole(player);
                    S2C_updateBattles.Add(new S2C_UpdateBattle(player.account, false));
                }
                foreach (Player player in (players[0].Scene as Scene).Players)
                {
                    if (player.mainRole.State != RoleState.Battle)
                    {
                        ServerSys.Instance.Send(player, "CallLuaFunction",
                            "HeadMgrCtrl.OtherRoleBattle", S2C_updateBattles);
                    }
                }
                if (teams.Count != 0)
                {
                    foreach(Team team in teams)
                    {
                        EventSys.Instance.Trigger("UpdateTeamFollow", new System.Object[] { team });
                    }
                }
                switch (battleType)
                {
                    case BattleTypeEnum.PVE:
                        if (S2C_battleSettle.winTeam == 1)
                        {
                            foreach (Player player in players)
                            {
                                EventSys.Instance.Trigger("KillMonster", new System.Object[] {player,
                                monsterId,teamTwoCount});
                            }
                        }
                        break;
                    case BattleTypeEnum.PVP:
                        break;
                }

                BattleSys.Instance.RemoveBattle(this.Guid);
                BattleSys.Instance.RemovePlayer(players);
                
            }
        }

        /// <summary>
        /// 参战者死亡所需调用的方法
        /// </summary>
        /// <param name="battler"></param>
        public void BattlerDead(ref Battler battler)
        {
            battler.staticBuffs.Clear();
            battler.actionBuffs.Clear();
            indexsDic[battler.PositionIndex].Remove(battler.PositionIndex);
        }

        /// <summary>
        /// 判断该索引单位是否死亡
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool IsDead(byte index)
        {
            return !indexsDic[index].Contains(index);
        }

        /// <summary>
        /// 判断战局是否结束
        /// </summary>
        /// <returns></returns>
        public bool isBattleEnd()
        {
            if ((teamOneSurvivals.Count == 0) || (teamTwoSurvivals.Count == 0))
            {
                battleState = BattleStateEnum.BattleEnd;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 更新参战者临时数据
        /// </summary>
        /// <param name="battler"></param>
        public void UpdataBattlerAttr(ref Battler battler)
        {
            RoleBase role = battler.Role;
            battler.hp = role.Hp;
            battler.maxHp = role.MaxHp;
            battler.mp = role.Mp;
            battler.maxMp = role.MaxMp;
            battler.physicalAttack = role.PhysicalAttack;
            battler.physicalDefense = role.PhysicalDefense;
            battler.magicAttack = role.MagicAttack;
            battler.magicDefense = role.MagicDefense;
            battler.frozen = role.Frozen;
            battler.frozenResist = role.FrozenResist;
            battler.poisoning = role.Poisoning;
            battler.poisoningResist = role.PoisoningResist;
            battler.burn = role.Burn;
            battler.burnResist = role.BurnResist;
            battler.continueHit = role.ContinueHit;
            battler.strikeBack = role.StrikeBack;
            Dictionary<int, BuffBase> staticBuffs = battler.staticBuffs;
            foreach(BuffBase buff in staticBuffs.Values)
            {
                buff.Calculate(ref battler);
            }
        }

        /// <summary>
        /// 添加静态buff
        /// </summary>
        /// <param name="buff"></param>
        /// <param name="battler"></param>
        public void AddStaticBuff(BuffBase buff,ref Battler battler)
        {

            if (battler.staticBuffs.ContainsKey(buff.buffId))
                battler.staticBuffs.Remove(buff.buffId);

            battler.staticBuffs.Add(buff.buffId, buff);

        }

        /// <summary>
        /// 添加动态buff
        /// </summary>
        /// <param name="buff"></param>
        /// <param name="battler"></param>
        public void AddActionBuff(BuffBase buff,ref Battler battler)
        {
            if (battler.actionBuffs.ContainsKey(buff.buffId))
                battler.actionBuffs.Remove(buff.buffId);

            battler.actionBuffs.Add(buff.buffId, buff);
            
        }



        /// <summary>
        /// 根据技能目标数获取横排的与目标最近的所有目标索引
        /// </summary>
        /// <param name="target">技能首个目标索引</param>
        /// <param name="count">技能目标个数</param>
        /// <param name="team">目标所属队伍</param>
        /// <returns></returns>
        byte[] GetTargets(byte target,byte count)
        {
            Console.WriteLine("目标个数：" + count);
            
            byte start = (byte)((target / 5) * 5);
            byte end = (byte)((target / 5) * 5 + 4);
            Console.WriteLine("min" + start);
            Console.WriteLine("max" + end);
            List<byte> targets = new List<byte>();
            byte[] team = indexsDic[target].ToArray();
            for (int i = 0; i < 8; i++)
            {
                byte index = (byte)(i + 1);
                if (index % 2 != 0)
                {
                    index = (byte)(target - (index / 2));
                }
                else
                {
                    index = (byte)(target + (index / 2));
                }
                Console.WriteLine("index:" + index);
                if (!(index >= start && index <= end))
                    continue;
                
                if (team.Contains(index))
                    targets.Add(index);

                if (targets.Count == count)
                    return targets.ToArray();
            }

            return targets.ToArray();
        }
    }

    /// <summary>
    /// 参战者
    /// </summary>
    public class Battler
    {
        public Battler(byte positionIndex, RoleBase role,BattlerType battlerType,int monsterId = 0)
        {
            this.positionIndex = positionIndex;
            this.role = role;
            this.battlerType = battlerType;
            this.monsterId = monsterId;
            staticBuffs = new Dictionary<int, BuffBase>();
            actionBuffs = new Dictionary<int, BuffBase>();
        }

        public Dictionary<int,BuffBase> staticBuffs;  //用于存储静态buff
        public Dictionary<int,BuffBase> actionBuffs;  //用于存储动态buff

        //参战者临时数据，最终结果为原始角色属性+所拥有的buff属性
        public int hp;
        public int maxHp;
        public int mp;
        public int maxMp;
        public int physicalAttack;
        public int physicalDefense;
        public int magicAttack;
        public int magicDefense;
        public float dizzy;  //晕眩命中率
        public float dizzyResist;  //晕眩抗性
        public float frozen;  //冰冻命中率
        public float frozenResist;  //冰冻抗性
        public float poisoning;  //中毒命中率
        public float poisoningResist;  //中毒抗性
        public float burn;  //烧伤命中率
        public float burnResist;  //烧伤抗性
        public float continueHit;  //连击率
        public float strikeBack;  //反击率

        private int monsterId;

        private byte positionIndex;
        private RoleBase role;
        private BattlerType battlerType;
        public int MonsterId { get { return monsterId; } }
        public BattlerType BattlerType { get { return battlerType; } }
        public byte PositionIndex { get { return positionIndex; } }  //站位索引
        public RoleBase Role { get { return role; } }  //角色原始属性

    }
}
