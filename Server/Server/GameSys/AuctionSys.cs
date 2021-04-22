/*
 * ===================================================================
 * 
 *          projectName:
 *              项目名称:拱猪
 *                  
 *          title:
 *              标题:拍卖系统
 *          
 *          description:
 *              功能描述:管理拍卖场里的物品
 *              
 *          author:
 *              作者:照着教程敲出bug的程序员
 * 
 * ===================================================================
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Server.GameSys
{
    public class AuctionSys
    {

        private static AuctionSys instance;
        public static AuctionSys Instance
        {
            get
            {
                if (instance == null)
                    instance = new AuctionSys();
                return instance;
            }
        }

        public AuctionSys()
        {
            EventSys.Instance.AddListener("UpdateAuction", () =>
            {
                UpdateLots();
            });

        }

        

        //<物品类型,<Guid,lots>>
        Dictionary<byte, Dictionary<string, Lots>> lotsDic = new Dictionary<byte, Dictionary<string, Lots>>();

        public void Init()
        {
            List<LotsData> lotsData = DBSys.Instance.GetAllDatas<LotsData>();
            if (lotsData.Count == 0)
                return;
            for(int i = 0;i < lotsData.Count; i++)
            {
                Lots lots = new Lots();
                lots.Guid = lotsData[i].Guid;
                lots.account = lotsData[i].account;
                lots.auctionPrice = lotsData[i].auctionPrice;
                lots.bidderGuid = lotsData[i].bidderGuid;
                lots.bidderName = lotsData[i].bidderName;
                lots.fixedPrice = lotsData[i].fixedPrice;
                lots.itemInfo = lotsData[i].itemInfo;
                lots.remainTime = lotsData[i].remainTime;
                if (!lotsDic.TryGetValue((byte)lotsData[i].type, out Dictionary<string, Lots> dic))
                    lotsDic.Add((byte)lotsData[i].type, new Dictionary<string, Lots>());
                lotsDic[(byte)lotsData[i].type].Add(lots.Guid, lots);

            }
        }

        /// <summary>
        /// 拍卖物品
        /// </summary>
        /// <param name="player"></param>
        /// <param name="C2S_auction"></param>
        public void Auction(Player player,C2S_Auction C2S_auction)
        {

            if (player.mainRole.Inventory.GetItem(C2S_auction.Guid) == null)
                return;
            //根据拍卖时长扣除金币，判断玩家金币是否足够
            if (player.mainRole.Gold < C2S_auction.time)
                return;

            Lots lots = new Lots();
            lots.Guid = Guid.NewGuid().ToString();
            lots.account = player.account;
            lots.itemInfo = player.mainRole.Inventory.GetItem(C2S_auction.Guid);
            lots.auctionPrice = C2S_auction.auctionPrice;
            lots.fixedPrice = C2S_auction.fixedPrice;
            lots.remainTime = C2S_auction.time;

            //扣除玩家金币和物品
            player.mainRole.ReduceGold(C2S_auction.time);
            player.mainRole.Inventory.RemoveItem(C2S_auction.Guid);
            ServerSys.Instance.Send(player, "CallLuaFunction", "BagCtrl.RemoveItems",
                new S2C_RemoveItems(new List<S2C_RemoveItemInfo>() { new S2C_RemoveItemInfo(C2S_auction.Guid) }));

            //将物品上架到拍卖场字典
            byte itemType = (byte)lots.itemInfo.ItemType;
            if (!lotsDic.TryGetValue(itemType, out Dictionary<string, Lots> dic))
                lotsDic.Add(itemType, new Dictionary<string, Lots>());
            if (lotsDic[itemType].ContainsKey(lots.Guid))
                return;
            lotsDic[itemType].Add(lots.Guid, lots);

            ServerSys.Instance.Send(player, "CallLuaFunction", "AuctionCtrl.UpdateSellInfo");

        }

        /// <summary>
        /// 竞价
        /// </summary>
        /// <param name="player"></param>
        /// <param name="C2S_bidding"></param>
        public void Bidding(Player player,C2S_Bidding C2S_bidding)
        {
            if (!lotsDic.TryGetValue(C2S_bidding.itemType, out Dictionary<string, Lots> dic))
                return;
            if (!dic.TryGetValue(C2S_bidding.Guid, out Lots lots))
                return;

            if (C2S_bidding.biddingPrice <= lots.auctionPrice)
                return;

            if (player.mainRole.Gold < C2S_bidding.biddingPrice)
                return;

            if(lots.bidderGuid != null)
            {
                MailSys.Instance.SendMail(lots.bidderName, "拍卖场", false, "竞拍失败", "其他玩家以更高的价格" +
                    "竞拍了这件物品,在此返还金币", new List<ItemInfo>(), new List<EquipInfo>(),
                    lots.auctionPrice);
                //向玩家发送邮件告知并返回银币和金币

            }

            //扣除玩家银币和金币
            player.mainRole.ReduceGold(C2S_bidding.biddingPrice);
            RoleSys.Instance.UpdatePlayerMainRole(player);

            //更新该拍卖品信息
            lots.bidderGuid = player.account;
            lots.bidderName = player.mainRole.Name;
            lots.auctionPrice = C2S_bidding.biddingPrice;
            GetLots(player, new C2S_GetLots(C2S_bidding.itemType));

        }

        /// <summary>
        /// 一口价购买
        /// </summary>
        public void FixedBuy(Player player,C2S_FixedBuy C2S_fixedBuy)
        {
            if (!lotsDic.TryGetValue(C2S_fixedBuy.itemType, out Dictionary<string, Lots> dic))
                return;
            if (!dic.TryGetValue(C2S_fixedBuy.Guid, out Lots lots))
                return;

            if (player.mainRole.Gold < lots.fixedPrice)
                return;

            if (lots.bidderGuid != null)
            {
                MailSys.Instance.SendMail(lots.bidderName, "拍卖场", false, "竞拍失败", "其他玩家以更高的价格" +
                    "竞拍了这件物品,在此返还金币", new List<ItemInfo>(), new List<EquipInfo>(),
                    lots.auctionPrice);
                //向玩家发送邮件告知并返回银币和金币

            }

            MailSys.Instance.SendMail(ServerSys.Instance.Players[lots.account].mainRole.Name, "拍卖场",
                false, "拍卖成功", "这是宁获得的金币",new List<ItemInfo>(), new List<EquipInfo>(),
                lots.fixedPrice);

            player.mainRole.ReduceGold(lots.fixedPrice);
            RoleSys.Instance.UpdatePlayerMainRole(player);

            //扣除玩家银币和金币
            List<ItemInfo> items = new List<ItemInfo>();
            List<EquipInfo> equips = new List<EquipInfo>();
            switch (lots.itemInfo.ItemType)
            {
                case ItemEunm.Item:
                    items.Add(lots.itemInfo as ItemInfo);
                    break;
                case ItemEunm.Equip:
                    equips.Add(lots.itemInfo as EquipInfo);
                    break;
                case ItemEunm.Gem:
                    items.Add(lots.itemInfo as ItemInfo);
                    break;
            }

            MailSys.Instance.SendMail(player.mainRole.Name, "拍卖场", true, "购买成功", "恭喜宁成功以一口价拍下了" +
                "该物品", items, equips, 0);
            //发送邮件添加该物品给玩家
            lotsDic[C2S_fixedBuy.itemType].Remove(C2S_fixedBuy.Guid);
            GetLots(player, new C2S_GetLots(C2S_fixedBuy.itemType));

        }

        /// <summary>
        /// 更新商品信息
        /// </summary>
        public void UpdateLots()
        {
            List<byte> types = lotsDic.Keys.ToList();
            for(int i = 0;i < types.Count; i++)
            {
                byte type = types[i];
                if (lotsDic[type].Count == 0)
                    continue;
                List<string> Guids = lotsDic[types[i]].Keys.ToList();
                for(int index = 0;index < Guids.Count; index++)
                {
                    string Guid = Guids[index];
                    Lots lots = lotsDic[type][Guid];
                    lots.remainTime--;
                    if (lots.remainTime > 0)
                        continue;
                    List<ItemInfo> items = new List<ItemInfo>();
                    List<EquipInfo> equips = new List<EquipInfo>();
                    switch (lots.itemInfo.ItemType)
                    {
                        case ItemEunm.Item:
                            items.Add(lots.itemInfo as ItemInfo);
                            break;
                        case ItemEunm.Equip:
                            equips.Add(lots.itemInfo as EquipInfo);
                            break;
                        case ItemEunm.Gem:
                            items.Add(lots.itemInfo as ItemInfo);
                            break;
                    }
                    if (lots.bidderGuid == null)
                    {
                        //向该拍卖品所属玩家返还物品和银币金币
                        
                        MailSys.Instance.SendMail(ServerSys.Instance.Players[lots.account].mainRole.Name,
                            "拍卖场",true, "拍卖失败", "在此返回宁出售的物品", items,equips,0);
                    }
                    else
                    {
                        MailSys.Instance.SendMail(lots.bidderName,"拍卖场", true, "拍卖成功", 
                            "这是宁获得的物品", items, equips, 0);
                        MailSys.Instance.SendMail(ServerSys.Instance.Players[lots.account].mainRole.Name,
                            "拍卖场",false, "拍卖成功", "这是宁获得的金币", new List<ItemInfo>(),
                            new List<EquipInfo>(),lots.auctionPrice);
                        //向竞拍者发送邮件添加该拍卖品物品信息
                        //向拍卖这发送邮件添加竞拍银币和金币
                    }
                    lotsDic[type].Remove(lots.Guid);
                    
                }
            }
        }

        /// <summary>
        /// 获取某种类型的所有拍卖品
        /// </summary>
        /// <param name="player"></param>
        /// <param name="C2S_getLots"></param>
        public void GetLots(Player player,C2S_GetLots C2S_getLots)
        {
            if (!lotsDic.TryGetValue(C2S_getLots.itemType, out Dictionary<string, Lots> dic))
            {
                ServerSys.Instance.Send(player, "CallLuaFunction", "AuctionCtrl.UpdateAuctionBox",
                new S2C_LotsInfos(C2S_getLots.itemType, new List<S2C_LotsInfo>()));
                return;
            }
            if (dic.Count == 0)
            {
                ServerSys.Instance.Send(player, "CallLuaFunction", "AuctionCtrl.UpdateAuctionBox",
                new S2C_LotsInfos(C2S_getLots.itemType, new List<S2C_LotsInfo>()));
                return;
            }

            List<S2C_LotsInfo> lotsInfos = new List<S2C_LotsInfo>();
            foreach(Lots lots in dic.Values)
            {
                S2C_LotsInfo lotsInfo = new S2C_LotsInfo();
                lotsInfo.Guid = lots.Guid;
                lotsInfo.auctionPrice = lots.auctionPrice;
                lotsInfo.fixedPrice = lots.fixedPrice;
                lotsInfo.remainTime = lots.remainTime;
                if (lots.bidderGuid != null)
                    lotsInfo.bidderGuid = lots.bidderGuid;
                if (lots.bidderName != null)
                    lotsInfo.bidderName = lots.bidderName;
                switch (lots.itemInfo.ItemType)
                {
                    case ItemEunm.Item:
                        ItemInfo item = lots.itemInfo as ItemInfo;
                        lotsInfo.itemInfo = new S2C_AddItemInfo(item.Guid, item.ItemId, (int)item.ItemType,
                            item.Inventory, item.Count);
                        break;
                    case ItemEunm.Equip:
                        EquipInfo equip = lots.itemInfo as EquipInfo;
                        lotsInfo.equipInfo = new S2C_AddEquipInfo(equip.Guid, equip.ItemId, (int)equip.ItemType,
                            equip.Inventory, equip.gems);
                        break;
                    case ItemEunm.Gem:
                        ItemInfo gem = lots.itemInfo as ItemInfo;
                        lotsInfo.itemInfo = new S2C_AddItemInfo(gem.Guid, gem.ItemId, (int)gem.ItemType,
                            gem.Inventory, gem.Count);
                        break;
                }

                lotsInfos.Add(lotsInfo);

            }
            ServerSys.Instance.Send(player, "CallLuaFunction", "AuctionCtrl.UpdateAuctionBox",
                new S2C_LotsInfos(C2S_getLots.itemType,lotsInfos));

        }

        /// <summary>
        /// 获得某种类型的指定名称的所有拍卖品
        /// </summary>
        /// <param name="player"></param>
        /// <param name="C2S_searchLots"></param>
        public void SearchLots(Player player, C2S_SearchLots C2S_searchLots)
        {
            if (!lotsDic.TryGetValue(C2S_searchLots.itemType, out Dictionary<string, Lots> dic))
                return;
            if (dic.Count == 0)
                return;

            List<S2C_LotsInfo> lotsInfos = new List<S2C_LotsInfo>();
            List<string> Guids = dic.Keys.ToList();
            for(int i = 0;i < Guids.Count; i++)
            {
                Lots lots = dic[Guids[i]];
                switch (lots.itemInfo.ItemType)
                {
                    case ItemEunm.Item:
                        if (ConfigSys.Instance.GetConfig<ItemConfig>(lots.itemInfo.ItemId).name !=
                            C2S_searchLots.name)
                            continue;
                        break;
                    case ItemEunm.Equip:
                        if (ConfigSys.Instance.GetConfig<EquipConfig>(lots.itemInfo.ItemId).name !=
                            C2S_searchLots.name)
                            continue;
                        break;
                    case ItemEunm.Gem:
                        if (ConfigSys.Instance.GetConfig<GemConfig>(lots.itemInfo.ItemId).name !=
                            C2S_searchLots.name)
                            continue;
                        break;
                }

                S2C_LotsInfo lotsInfo = new S2C_LotsInfo();
                lotsInfo.Guid = lots.Guid;
                lotsInfo.auctionPrice = lots.auctionPrice;
                lotsInfo.fixedPrice = lots.fixedPrice;
                lotsInfo.remainTime = lots.remainTime;
                if (lots.bidderGuid != null)
                    lotsInfo.bidderGuid = lots.bidderGuid;
                if (lots.bidderName != null)
                    lotsInfo.bidderName = lots.bidderName;
                switch (lots.itemInfo.ItemType)
                {
                    case ItemEunm.Item:
                        ItemInfo item = lots.itemInfo as ItemInfo;
                        lotsInfo.itemInfo = new S2C_AddItemInfo(item.Guid, item.ItemId, (int)item.ItemType,
                            item.Inventory, item.Count);
                        break;
                    case ItemEunm.Equip:
                        EquipInfo equip = lots.itemInfo as EquipInfo;
                        lotsInfo.equipInfo = new S2C_AddEquipInfo(equip.Guid, equip.ItemId, (int)equip.ItemType,
                            equip.Inventory, equip.gems);
                        break;
                    case ItemEunm.Gem:
                        ItemInfo gem = lots.itemInfo as ItemInfo;
                        lotsInfo.itemInfo = new S2C_AddItemInfo(gem.Guid, gem.ItemId, (int)gem.ItemType,
                            gem.Inventory, gem.Count);
                        break;
                }

                lotsInfos.Add(lotsInfo);
            }
            ServerSys.Instance.Send(player, "CallLuaFunction", "AuctionCtrl.UpdateAuctionBox",
                new S2C_LotsInfos(C2S_searchLots.itemType, lotsInfos));
        }

        /// <summary>
        /// 保存拍卖品信息
        /// </summary>
        public void SaveAuction()
        {
            DBSys.Instance.DeleteAllDatas<LotsData>();
            if (lotsDic.Count == 0)
                return;
            byte[] types = lotsDic.Keys.ToArray();
            for (int i = 0; i < types.Length; i++)
            {
                foreach (Lots lots in lotsDic[types[i]].Values)
                {
                    DBSys.Instance.InsertData<LotsData>(new LotsData(lots.Guid, lots.account,
                        types[i], lots.itemInfo, lots.auctionPrice, lots.bidderGuid, lots.bidderName,
                        lots.fixedPrice, lots.remainTime));
                }
            }
        }
    }

    /// <summary>
    /// 拍卖品
    /// </summary>
    public class Lots
    {
        public string Guid;  //拍卖品Guid
        public string account;
        public ItemBase itemInfo;  //物品信息
        public int auctionPrice;  //竞拍价金币
        public string bidderGuid;  //竞拍者Guid
        public string bidderName;  //竞拍者名称
        public int fixedPrice;  //一口价金币
        public int remainTime;  //剩余时长（一小时为单位）

    }

}
