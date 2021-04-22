/*
 * ===================================================================
 * 
 *          projectName:
 *              项目名称:拱猪
 *                  
 *          title:
 *              标题:服务器系统
 *          
 *          description:
 *              功能描述:执行服务器启动完毕需要执行的事情,
 *              处理未知客户端的登录注册请求
 *              转发客户端向服务端发送得消息
 *              
 *          author:
 *              作者:照着教程敲出bug得程序员
 * 
 * ===================================================================
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameDesigner;
using Net.Entity;
using Net.Server;
using Net.Share;
using Server.GameSys;
using UnityEngine;

namespace Server
{
    class ServerSys : UdpServer<Player, Scene>
    {

        /// <summary>
        /// 当未知客户端连接时调用
        /// </summary>
        /// <param name="unClient"></param>
        /// <param name="cmd"></param>
        /// <param name="buffer"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        protected override Player OnUnClientRequest(Player unClient, byte cmd, byte[] buffer, int index, int count)
        {
            
            var func = NetConvert.Deserialize(buffer, index, count);
            switch (func.name)
            {
                case "Login":
                    return LoginSys.Instance.Login(unClient, func.pars[0].ToString(), func.pars[1].ToString());
                case "Register":
                    LoginSys.Instance.Register(unClient, func.pars[0].ToString(), func.pars[1].ToString(), func.pars[2].ToString());
                    break;
            }
            return null;
        }

        protected override void OnStarting()
        {
            HeartTime = 500;
        }

        protected override void OnAddPlayerToScene(Player client)
        {
            //默认登录成功会自动加入主场景， 所以player.scene对象是主场景对象
            //重写这个方法就可以
            //在这里写进入场景代码
            RoleSys.Instance.BeginGame(client);
            
        }

        protected override void OnRemoveClient(Player client)
        {
            Console.WriteLine(Players.ContainsKey(client.account));
            InventorySys.Instance.SaveItem(client);
            RoleSys.Instance.SaveRoleData(client);
            SkillInfoSys.Instance.SaveSkill(client);
            TaskSys.Instance.SaveTask(client);
            MailSys.Instance.SaveMail(client);
        }

        /// <summary>
        /// 初始化角色
        /// </summary>
        /// <param name="player"></param>
        [Rpc(NetCmd.SafeCall)]
        public void InitRole(Player player)
        {
            
            RoleSys.Instance.InitRole(player);
        }

        [Rpc(NetCmd.SafeCall)]
        public void InitMails(Player player)
        {
            MailSys.Instance.InitMails(player);
        }

        /// <summary>
        /// 创建主角
        /// </summary>
        /// <param name="player"></param>
        /// <param name="role"></param>
        [Rpc(NetCmd.SafeCall)]
        public void CreateMainRole(Player player, C2S_CreateMainRole role)
        {
            RoleSys.Instance.CreateMainRole(player, role);
        }

        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="player"></param>
        /// <param name="C2S_moveInfo"></param>
        [Rpc(NetCmd.SafeCall)]
        public void Move(Player player, C2S_MoveInfo C2S_moveInfo)
        {
            CtrlSys.Instance.Move(player, C2S_moveInfo);
        }

        /// <summary>
        /// 初始化玩家物品
        /// </summary>
        /// <param name="player"></param>
        //[Rpc(NetCmd.SafeCall)]
        //public void InitItem(Player player)
        //{
            
        //}

        [Rpc(NetCmd.SafeCall)]
        public void InitTask(Player player)
        {
            TaskSys.Instance.InitTaskInfo(player);
        }

        [Rpc(NetCmd.SafeCall)]
        public void AcceptTask(Player player,C2S_AcceptTask acceptTask)
        {
            TaskSys.Instance.AcceptTask(player, acceptTask);
        }

        [Rpc(NetCmd.SafeCall)]
        public void SubmitTask(Player player,C2S_CompleteTask completeTask)
        {
            TaskSys.Instance.SubmitTask(player, completeTask); 
        }

        [Rpc(NetCmd.SafeCall)]
        public void BattleRequest(Player player, C2S_BattleRequest C2S_battleRequest)
        {
            BattleSys.Instance.BattleRequest(player, C2S_battleRequest);
        }

        [Rpc(NetCmd.SafeCall)]
        public void BattleReply(Player player, C2S_BattleReply C2S_battleReply)
        {
            BattleSys.Instance.BattleReply(player, C2S_battleReply);
        }

        [Rpc(NetCmd.SafeCall)]
        public void AddBattleCommand(Player player, C2S_BattleCommand C2S_battleCommand)
        {
            BattleSys.Instance.AddBattleCommand(player, C2S_battleCommand);
        }

        [Rpc(NetCmd.SafeCall)]
        public void AddBattleCallback(Player player)
        {
            BattleSys.Instance.AddBattleCallback(player);
        }

        [Rpc(NetCmd.SafeCall)]
        public void InitSkill(Player player)
        {
            SkillInfoSys.Instance.InitSkill(player);
        }

        [Rpc(NetCmd.SafeCall)]
        public void JoinRequest(Player player, C2S_JoinTeamRequest C2S_joinTeamRequest)
        {
            TeamSys.Instance.JoinTeamRequest(player, C2S_joinTeamRequest);
        }

        [Rpc(NetCmd.SafeCall)]
        public void JoinReply(Player player, C2S_JoinTeamReply C2S_joinTeamReply)
        {
            TeamSys.Instance.JoinTeamReply(player, C2S_joinTeamReply);
        }

        [Rpc(NetCmd.SafeCall)]
        public void GetTeamInfo(Player player)
        {
            TeamSys.Instance.GetTeamInfo(player);
        }

        [Rpc(NetCmd.SafeCall)]
        public void ExitTeam(Player player,string Guid)
        {
            TeamSys.Instance.ExitTeam(ServerSys.Instance.Players[Guid]);
        }

        [Rpc(NetCmd.SafeCall)]
        public void ChangeLeader(Player player,string Guid)
        {
            TeamSys.Instance.ChangeLeader(ServerSys.Instance.Players[Guid]);
        }

        [Rpc(NetCmd.SafeCall)]
        public void VisiteRequest(Player player,C2S_VisiteRequest C2S_visiteRequest)
        {
            TeamSys.Instance.VisiteTeamRequest(player, C2S_visiteRequest);
        }

        [Rpc(NetCmd.SafeCall)]
        public void VisiteReply(Player player,C2S_VisiteReply C2S_visiteReply)
        {
            TeamSys.Instance.VisiteTeamReply(player, C2S_visiteReply);
        }

        [Rpc(NetCmd.SafeCall)]
        public void UseItem(Player player,C2S_UseItem C2S_useItem)
        {
            ItemSys.Instance.Exe(player, C2S_useItem.Guid);
        }

        [Rpc(NetCmd.SafeCall)]
        public void DressEquip(Player player, C2S_DressEquip C2S_dressEquip)
        {
            InventorySys.Instance.DressEquip(player, C2S_dressEquip);
        }

        [Rpc(NetCmd.SafeCall)]
        public void TakeoffEquip(Player player, C2S_TakeoffEquip C2S_takeoffEquip)
        {
            InventorySys.Instance.TakeoffEquip(player, C2S_takeoffEquip);
        }

        [Rpc(NetCmd.SafeCall)]
        public void DiscardItem(Player player,C2S_DiscardItem C2S_discardItem)
        {
            InventorySys.Instance.DiscardItem(player, C2S_discardItem);
        }

        [Rpc(NetCmd.SafeCall)]
        public void MakeEquip(Player player,C2S_MakeEquip C2S_makeEquip)
        {
            ForgeSys.Instance.Make(player, C2S_makeEquip.Guid);
        }

        [Rpc(NetCmd.SafeCall)]
        public void InlayGem(Player player,C2S_InlayGem C2S_inlayGem)
        {
            ForgeSys.Instance.InlayGem(player, C2S_inlayGem);
        }

        [Rpc(NetCmd.SafeCall)]
        public void AdvanceEquip(Player player,C2S_AdvanceEquip C2S_advanceEquip)
        {
            ForgeSys.Instance.AdvanceEquip(player, C2S_advanceEquip);
        }

        [Rpc(NetCmd.SafeCall)]
        public void RemoveGem(Player player,C2S_RemoveGem C2S_removeGem)
        {
            ForgeSys.Instance.RemoveGem(player, C2S_removeGem);
        }

        [Rpc(NetCmd.SafeCall)]
        public void MeetMonster(Player player)
        {
            Console.WriteLine(player.mainRole.Name + "触发战斗");
            BattleSys.Instance.MeetMonster(player);
        }

        [Rpc(NetCmd.SafeCall)]
        public void BuyItem(Player player,C2S_BuyItem C2S_buyItem)
        {
            InventorySys.Instance.BuyItem(player, C2S_buyItem);
        }

        [Rpc(NetCmd.SafeCall)]
        public void BuyEquip(Player player, C2S_BuyItem C2S_buyItem)
        {
            InventorySys.Instance.BuyEquip(player, C2S_buyItem);
        }

        [Rpc(NetCmd.SafeCall)]
        public void BuyGem(Player player, C2S_BuyItem C2S_buyItem)
        {
            InventorySys.Instance.BuyGem(player, C2S_buyItem);
        }

        [Rpc(NetCmd.SafeCall)]
        public void GetRanksInfo(Player player,C2S_CheckRank C2S_checkRank)
        {
            RankingSys.Instance.GetRanksInfo(player, C2S_checkRank);
        }

        [Rpc(NetCmd.SafeCall)]
        public void SendMail(Player player,C2S_SendMail C2S_sendMail)
        {
            MailSys.Instance.SendMail(player, C2S_sendMail);
        }

        [Rpc(NetCmd.SafeCall)]
        public void GetMailsInfo(Player player)
        {
            MailSys.Instance.GetMails(player);
        }

        [Rpc(NetCmd.SafeCall)]
        public void GetMailItems(Player player,C2S_GetMailItems C2S_getMailItems)
        {
            MailSys.Instance.GetItems(player, C2S_getMailItems);
        }

        [Rpc(NetCmd.SafeCall)]
        public void ReadMail(Player player,C2S_ReadMail C2S_readMail)
        {
            MailSys.Instance.ReadMail(player, C2S_readMail);
        }

        [Rpc(NetCmd.SafeCall)]
        public void Auction (Player player,C2S_Auction C2S_auction)
        {
            AuctionSys.Instance.Auction(player, C2S_auction);
        }

        [Rpc(NetCmd.SafeCall)]
        public void GetLotsInfos(Player player,C2S_GetLots C2S_getLots)
        {
            AuctionSys.Instance.GetLots(player, C2S_getLots);
        }

        [Rpc(NetCmd.SafeCall)]
        public void SearchLots(Player player, C2S_SearchLots C2S_searchLots)
        {
            AuctionSys.Instance.SearchLots(player, C2S_searchLots);
        }

        [Rpc(NetCmd.SafeCall)]
        public void Bidding(Player player,C2S_Bidding C2S_bidding)
        {
            AuctionSys.Instance.Bidding(player, C2S_bidding);
        }

        [Rpc(NetCmd.SafeCall)]
        public void FixedBuy(Player player,C2S_FixedBuy C2S_fixedBuy)
        {
            AuctionSys.Instance.FixedBuy(player, C2S_fixedBuy);
        }

        [Rpc(NetCmd.SafeCall)]
        public void CheckInfo(Player player,C2S_CheckInfo C2S_checkInfo)
        {
            RoleSys.Instance.CheckInfo(player, C2S_checkInfo);
        }

        [Rpc(NetCmd.SafeCall)]
        public void LearnSkill(Player player,C2S_LearnSkill C2S_learnSkill)
        {
            SkillInfoSys.Instance.LearnSkill(player, C2S_learnSkill);
        }

        [Rpc(NetCmd.SafeCall)]
        public void UpgradeSkill(Player player,C2S_UpgradeSkill C2S_upgradeSkill)
        {
            SkillInfoSys.Instance.UpgradeSkill(player, C2S_upgradeSkill);
        }

    } 
}
