/*
 * ===================================================================
 * 
 *          projectName:
 *              项目名称:拱猪
 *                  
 *          title:
 *              标题:物品栏系统
 *          
 *          description:
 *              功能描述:管理所有玩家的所有物品栏中的所有物品
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
using UnityEditor;
using UnityEngine;
using Net.Share;
using Net.Server;

namespace Server.GameSys
{

    public enum ItemEunm
    {
        None = 0,
        Item,
        Equip,
        Gem
    }

    public enum InventoryEnum
    {
        None = 0,
        Bag,
        Equip,
        Shop,
        Auction
    }

    public enum EquipDress
    {
        None = 0,
        Helmet,
        Necklace,
        Weapon,
        Clothes,
        Belt,
        Shoes
    }

   

    class InventorySys
    {
        private static InventorySys instance;
        public static InventorySys Instance
        {
            get
            {
                if (instance == null)
                    instance = new InventorySys();
                return instance;
            }
        }

        Dictionary<int, Dictionary<int, List<int>>> commoditysDic =
            new Dictionary<int, Dictionary<int, List<int>>>();

        Dictionary<int, int> typesDic = new Dictionary<int, int>();
        Dictionary<int, int> shopsDic = new Dictionary<int, int>();

        /// <summary>
        /// 添加商品
        /// </summary>
        /// <param name="Guid"></param>
        /// <param name="type"></param>
        /// <param name="shop"></param>
        public void AddCommodity(int itemId, int type, int shop)
        {
            if (!commoditysDic.TryGetValue(shop, out Dictionary<int, List<int>> types))
                commoditysDic.Add(shop, new Dictionary<int, List<int>>());
            if (!commoditysDic[shop].TryGetValue(type, out List<int> commoditys))
                commoditysDic[shop].Add(type, new List<int>());
            if (commoditysDic[shop][type].Contains(itemId))
                return;
            commoditysDic[shop][type].Add(itemId);
        }

        /// <summary>
        /// 初始化商品
        /// </summary>
        /// <param name="player"></param>
        public void InitCommodity(Player player)
        {
            List<S2C_AddItemInfo> S2C_addItems = new List<S2C_AddItemInfo>();
            List<S2C_AddEquipInfo> S2C_addEquips = new List<S2C_AddEquipInfo>();
            foreach(int shop in commoditysDic.Keys)
            {
                foreach(int type in commoditysDic[shop].Keys)
                {
                    foreach(int itemId in commoditysDic[shop][type])
                    {
                        switch ((ItemEunm)type)
                        {
                            case ItemEunm.Item:
                                S2C_addItems.Add(new S2C_AddItemInfo(Guid.NewGuid().ToString(),
                                itemId, type, shop, 1));
                                break;
                            case ItemEunm.Equip:
                                S2C_addEquips.Add(new S2C_AddEquipInfo(Guid.NewGuid().ToString(),
                                itemId, type, shop, new int[2]));
                                break;
                            case ItemEunm.Gem:
                                S2C_addItems.Add(new S2C_AddItemInfo(Guid.NewGuid().ToString(),
                                itemId, type, shop, 1));
                                break;
                        }
                    }
                }
            }
            ServerSys.Instance.Send(player, "CallLuaFunction", "BagCtrl.AddItems",
                new S2C_AddItemsInfo(S2C_addItems));
            ServerSys.Instance.Send(player, "CallLuaFunction", "BagCtrl.AddItems",
                new S2C_AddEquipsInfo(S2C_addEquips));

        }

        /// <summary>
        /// 初始化物品
        /// </summary>
        /// <param name="player"></param>
        public void InitItem(Player player)
        {
            List<ItemInfoData> itemDatas = DBSys.Instance.GetAllDatas<ItemInfoData>
                ("account", player.account);
            List<ItemInfo> items = new List<ItemInfo>();
            List<S2C_AddItemInfo> S2C_addItems = new List<S2C_AddItemInfo>();
            if(itemDatas.Count > 0)
            {
                foreach(ItemInfoData itemInfo in itemDatas)
                {
                    ItemInfo item = new ItemInfo(player.account, (ItemEunm)itemInfo.itemType,
                        itemInfo.itemId, itemInfo.Guid, itemInfo.inventory,
                        itemInfo.count);
                    items.Add(item);
                    S2C_addItems.Add(new S2C_AddItemInfo(item.Guid, item.ItemId,
                        (int)item.ItemType,item.Inventory, item.Count));
                }
            }

            List<EquipInfoData> equipDatas = DBSys.Instance.GetAllDatas<EquipInfoData>
                ("account", player.account);
            Console.WriteLine("装备数量"+equipDatas.Count);
            List<S2C_AddEquipInfo> S2C_addEquips = new List<S2C_AddEquipInfo>();
            List<EquipInfo> equips = new List<EquipInfo>();
            if (equipDatas.Count > 0)
            {
                foreach (EquipInfoData itemInfo in equipDatas)
                {
                    EquipInfo equip = new EquipInfo(player.account, (ItemEunm)itemInfo.itemType,
                        itemInfo.itemId, itemInfo.Guid, itemInfo.inventory,
                        itemInfo.gems);
                    equips.Add(equip);
                    S2C_addEquips.Add(new S2C_AddEquipInfo(equip.Guid, equip.ItemId,
                        (int)equip.ItemType, (int)equip.Inventory, equip.gems));
                }
            }
            if (items.Count > 0)
            {
                player.mainRole.Inventory.AddItems(items);
                Console.WriteLine("发送物品信息");
                ServerSys.Instance.Send(player, "CallLuaFunction", "BagCtrl.AddItems",
                    new S2C_AddItemsInfo(S2C_addItems));
            }
            if(equips.Count > 0)
            {
                player.mainRole.Inventory.AddEquips(equips);
                ServerSys.Instance.Send(player, "CallLuaFunction", "BagCtrl.AddItems",
                    new S2C_AddEquipsInfo(S2C_addEquips));
            }
            InitCommodity(player);

        }

        /// <summary>
        /// 购买物品
        /// </summary>
        /// <param name="player"></param>
        /// <param name="C2S_buyItem"></param>
        public void BuyItem(Player player,C2S_BuyItem C2S_buyItem)
        {
            int itemId = C2S_buyItem.itemId;
            int price = ConfigSys.Instance.GetConfig<ItemConfig>(itemId).buyPrice;
            if (player.mainRole.Gold < price)
                return;
            player.mainRole.ReduceGold(price);
            RoleSys.Instance.UpdatePlayerMainRole(player);
            ItemInfo item = new ItemInfo(player.playerID, ItemEunm.Item, itemId, Guid.NewGuid().ToString(),
                (byte)InventoryEnum.Bag, 1);
            EventSys.Instance.Trigger("AddItem", new object[] { player, itemId, 1 });
            player.mainRole.Inventory.AddItem(item);
            ServerSys.Instance.Send(player, "CallLuaFunction", "BagCtrl.AddItems", new S2C_AddItemsInfo(
                new List<S2C_AddItemInfo>(){ new S2C_AddItemInfo(item.Guid,item.ItemId,
                (byte)item.ItemType,item.Inventory,item.Count)}));

        }

        /// <summary>
        /// 购买装备
        /// </summary>
        /// <param name="player"></param>
        /// <param name="C2S_buyItem"></param>
        public void BuyEquip(Player player, C2S_BuyItem C2S_buyItem)
        {
            int itemId = C2S_buyItem.itemId;
            int price = ConfigSys.Instance.GetConfig<EquipConfig>(itemId).buyPrice;
            if (player.mainRole.Gold < price)
                return;
            player.mainRole.ReduceGold(price);
            RoleSys.Instance.UpdatePlayerMainRole(player);
            EquipInfo equip = new EquipInfo(player.playerID, ItemEunm.Equip, itemId, Guid.NewGuid().ToString(),
                (byte)InventoryEnum.Bag, new int[2]);
            player.mainRole.Inventory.AddItem(equip);
            ServerSys.Instance.Send(player, "CallLuaFunction", "BagCtrl.AddItems", new S2C_AddEquipsInfo(
                new List<S2C_AddEquipInfo>(){ new S2C_AddEquipInfo(equip.Guid,equip.ItemId,
                (byte)equip.ItemType,equip.Inventory,equip.gems)}));
        }

        /// <summary>
        /// 购买宝石
        /// </summary>
        /// <param name="player"></param>
        /// <param name="C2S_buyItem"></param>
        public void BuyGem(Player player, C2S_BuyItem C2S_buyItem)
        {
            int itemId = C2S_buyItem.itemId;
            int price = ConfigSys.Instance.GetConfig<GemConfig>(itemId).buyPrice;
            if (player.mainRole.Gold < price)
                return;
            player.mainRole.ReduceGold(price);
            RoleSys.Instance.UpdatePlayerMainRole(player);
            ItemInfo gem = new ItemInfo(player.playerID, ItemEunm.Gem, itemId, Guid.NewGuid().ToString(),
                (byte)InventoryEnum.Bag, 1);
            player.mainRole.Inventory.AddItem(gem);
            ServerSys.Instance.Send(player, "CallLuaFunction", "BagCtrl.AddItems", new S2C_AddItemsInfo(
                new List<S2C_AddItemInfo>(){ new S2C_AddItemInfo(gem.Guid,gem.ItemId,
                (byte)gem.ItemType,gem.Inventory,gem.Count)}));
        }

        /// <summary>
        /// 穿戴装备
        /// </summary>
        /// <param name="player"></param>
        /// <param name="C2S_dressEquip"></param>
        public void DressEquip(Player player, C2S_DressEquip C2S_dressEquip)
        {
            //判断该装备是否存在
            //List<S2C_AddEquipInfo> S2C_addEquips = new List<S2C_AddEquipInfo>();
            EquipInfo equip = player.mainRole.Inventory.GetItem(C2S_dressEquip.Guid) as EquipInfo;
            if(equip == null)
            {
                return;
            }
            if (ConfigSys.Instance.GetConfig<EquipConfig>(equip.ItemId).profession != -1)
            {
                if (ConfigSys.Instance.GetConfig<EquipConfig>(equip.ItemId).profession != player.mainRole.Profession)
                    return;
            }
            //获得装备类型
            EquipDress dress = (EquipDress)ConfigSys.Instance.GetConfig<EquipConfig>
                (equip.ItemId)._type;
            //查看该装备位置是否已装备了装备
            ItemBase[] equips = player.mainRole.Inventory.GetInventoryItems((int)InventoryEnum.Equip);
            if (equips != null)
            {
                for (int i = 0; i < equips.Length; i++)
                {
                    //若已有装备，则从装备栏移除该装备，并添加至背包
                    //向客户端发送对应的消息
                    //将该装备提供的属性加成移除
                    EquipInfo equipTemp = equips[i] as EquipInfo;
                    if ((EquipDress)ConfigSys.Instance.GetConfig<EquipConfig>(equipTemp.ItemId)._type
                        == dress)
                    {
                        TakeoffEquip(player, new C2S_TakeoffEquip(equipTemp.Guid));
                        //player.mainRole.Inventory.RemoveItem(equipTemp.Guid);
                        //ServerSys.Instance.Send(player, "CallLuaFunction", "InventoryMgr.RemoveItem",
                        //    new S2C_RemoveItemInfo(equipTemp.Guid));
                        //player.mainRole.Inventory.AddItem(new EquipInfo(equipTemp.PlayerId,
                        //    equipTemp.ItemType, equipTemp.ItemId, equipTemp.Guid,
                        //    (int)InventoryEnum.Bag, equipTemp.gems));
                        //S2C_addEquips.Add(new S2C_AddEquipInfo(equipTemp.Guid, equipTemp.ItemId,
                        //    (int)equipTemp.ItemType, (int)InventoryEnum.Bag, equipTemp.gems));

                        //AttrSys.Instance.ReduceEquipAttr(player, equipTemp.ItemId);
                        break;
                    }
                }
            }

            //将该装备从背包移除，并添加至装备栏
            //向客户端发送对应消息
            //对人物增加装备所提供的属性
            //向客户端发送刷新人物属性的消息
            player.mainRole.Inventory.RemoveItem(equip.Guid);
            //ServerSys.Instance.Send(player, "CallLuaFunction", "InventoryMgr.RemoveItem",
            //            new S2C_RemoveItemInfo(equip.Guid));
            player.mainRole.Inventory.AddItem(new EquipInfo(equip.PlayerId, equip.ItemType,
                equip.ItemId, equip.Guid, (int)InventoryEnum.Equip, equip.gems));
            //S2C_addEquips.Add(new S2C_AddEquipInfo(equip.Guid, equip.ItemId,
                        //(int)equip.ItemType, (int)InventoryEnum.Equip, equip.gems));

            //ServerSys.Instance.Send(player, "CallLuaFunction", "InventoryMgr.AddItems",
            //    new S2C_AddEquipsInfo(S2C_addEquips));
            ServerSys.Instance.Send(player, "CallLuaFunction", "BagCtrl.UpdateItem",
                new S2C_AddEquipInfo(equip.Guid,equip.ItemId,(int)equip.ItemType,
                (int)InventoryEnum.Equip, equip.gems));

            AttrSys.Instance.AddEquipAttr(player, equip.ItemId);

            for(int i = 0;i < equip.gems.Length; i++)
            {
                if (equip.gems[i] == 0)
                    break;
                AttrSys.Instance.AddGemAttr(player, equip.gems[i]);
            }
            RoleSys.Instance.UpdatePlayerMainRole(player);
            S2C_DressEquip S2C_dressEquip = new S2C_DressEquip();
            S2C_dressEquip.Guid = player.playerID;
            S2C_dressEquip.equipId = equip.ItemId;
            foreach (Player client in (player.Scene as Scene).Players)
            {
                ServerSys.Instance.Send(client, "DressEquip", S2C_dressEquip);
            }
            

        }



        /// <summary>
        /// 卸下装备
        /// </summary>
        /// <param name="player"></param>
        /// <param name="C2S_takeoffEquip"></param>
        public void TakeoffEquip(Player player, C2S_TakeoffEquip C2S_takeoffEquip)
        {
            EquipInfo equip = player.mainRole.Inventory.GetItem(C2S_takeoffEquip.Guid)
                as EquipInfo;
            List<S2C_AddEquipInfo> S2C_addEquips = new List<S2C_AddEquipInfo>();
            if (equip == null)
            {

                return;
            }
            player.mainRole.Inventory.RemoveItem(equip.Guid);
            ServerSys.Instance.Send(player, "CallLuaFunction", "BagCtrl.RemoveItems",
                new S2C_RemoveItems(new List<S2C_RemoveItemInfo>(){
                        new S2C_RemoveItemInfo(equip.Guid) }));
            player.mainRole.Inventory.AddItem(new EquipInfo(equip.PlayerId, equip.ItemType,
                equip.ItemId, equip.Guid, (int)InventoryEnum.Bag, equip.gems));
            S2C_addEquips.Add(new S2C_AddEquipInfo(equip.Guid, equip.ItemId, (int)equip.ItemType,
                (int)InventoryEnum.Bag, equip.gems));
            ServerSys.Instance.Send(player, "CallLuaFunction", "BagCtrl.AddItems",
                new S2C_AddEquipsInfo(S2C_addEquips));

            AttrSys.Instance.ReduceEquipAttr(player, equip.ItemId);

            for (int i = 0; i < equip.gems.Length; i++)
            {
                if (equip.gems[i] == 0)
                    break;
                AttrSys.Instance.ReduceGemAttr(player, equip.gems[i]);
            }

            S2C_TakeoffEquip takeoffEuip = new S2C_TakeoffEquip();
            takeoffEuip.Guid = player.playerID;
            takeoffEuip.type = ConfigSys.Instance.GetConfig<EquipConfig>(equip.ItemId)._type;
            foreach (Player client in (player.Scene as Scene).Players)
            {
                ServerSys.Instance.Send(client, "TakeoffEquip", takeoffEuip);
            }
            RoleSys.Instance.UpdatePlayerMainRole(player);

        }

        /// <summary>
        /// 丢弃物品
        /// </summary>
        /// <param name="player"></param>
        /// <param name="C2S_discardItem"></param>
        public void DiscardItem(Player player, C2S_DiscardItem C2S_discardItem)
        {
            ItemBase item = player.mainRole.Inventory.GetItem(C2S_discardItem.Guid);
            if(item == null)
            {
                return;
            }
            player.mainRole.Inventory.RemoveItem(item.Guid);
            ServerSys.Instance.Send(player, "CallLuaFunction", "BagCtrl.RemoveItems",
                        new S2C_RemoveItems(new List<S2C_RemoveItemInfo>(){
                            new S2C_RemoveItemInfo(item.Guid) }));

        }

        public void SaveItem(Player player)
        {
            DBSys.Instance.DeleteAllDatas<ItemInfoData>("account", player.account);
            DBSys.Instance.DeleteAllDatas<EquipInfoData>("account", player.account);

            ItemBase[] items = player.mainRole.Inventory.GetAllItems();
            for (int i = 0;i < items.Length; i++)
            {
                switch (items[i].ItemType)
                {
                    case ItemEunm.Item:
                        ItemInfo item = items[i] as ItemInfo;
                        DBSys.Instance.InsertData<ItemInfoData>(new ItemInfoData(item.Guid, item.PlayerId,
                            (int)item.ItemType, item.ItemId, item.Inventory, item.Count));
                        break;
                    case ItemEunm.Equip:
                        EquipInfo equip = items[i] as EquipInfo;
                        DBSys.Instance.InsertData<EquipInfoData>(new EquipInfoData(equip.Guid, equip.PlayerId,
                            (int)equip.ItemType, equip.ItemId, equip.Inventory, equip.gems));
                        break;
                    case ItemEunm.Gem:
                        ItemInfo gem = items[i] as ItemInfo;
                        DBSys.Instance.InsertData<ItemInfoData>(new ItemInfoData(gem.Guid, gem.PlayerId,
                            (int)gem.ItemType, gem.ItemId, gem.Inventory, gem.Count));
                        break;
                }
            }
        }

        /// <summary>
        /// 新增物品
        /// </summary>
        /// <param name="player"></param>
        /// <param name="itemsId"></param>
        /// <param name="counts"></param>
        public void AddItems(Player player,int[] itemsId,int[] counts)
        {
            List<S2C_AddItemInfo> S2C_addItems = new List<S2C_AddItemInfo>();
            for (int i = 0; i < itemsId.Length; i++)
            {
                ItemInfo itemInfo = new ItemInfo(player.account, ItemEunm.Item, itemsId[i], Guid.NewGuid()
                    .ToString(), (int)InventoryEnum.Bag, counts[i]);
                player.mainRole.Inventory.AddItem(itemInfo);
                S2C_addItems.Add(new S2C_AddItemInfo(itemInfo.Guid, itemInfo.ItemId, (int)itemInfo.ItemType,
                    itemInfo.Inventory, itemInfo.Count));
            }
            ServerSys.Instance.Send(player, "CallLuaFunction", "BagCtrl.AddItems",
                new S2C_AddItemsInfo(S2C_addItems));
        }

        /// <summary>
        /// 新增装备
        /// </summary>
        /// <param name="player"></param>
        /// <param name="equipsId"></param>
        public void AddEquips(Player player,int[] equipsId)
        {
            List<S2C_AddEquipInfo> S2C_addEquips = new List<S2C_AddEquipInfo>();
            for(int i = 0;i < equipsId.Length; i++)
            {
                EquipInfo equipInfo = new EquipInfo(player.account, ItemEunm.Equip, equipsId[i], Guid.NewGuid()
                    .ToString(), (int)InventoryEnum.Bag, new int[2]);
                S2C_addEquips.Add(new S2C_AddEquipInfo(equipInfo.Guid, equipInfo.ItemId, (int)equipInfo.ItemType,
                    equipInfo.Inventory, equipInfo.gems));
            }
            ServerSys.Instance.Send(player, "CallLuaFunction", "BagCtrl.AddItems",
                new S2C_AddEquipsInfo(S2C_addEquips));
        }

        /// <summary>
        /// 新增宝石
        /// </summary>
        /// <param name="player"></param>
        /// <param name="gemsId"></param>
        /// <param name="counts"></param>
        public void AddGems(Player player,int[] gemsId,int[] counts)
        {
            List<S2C_AddItemInfo> S2C_addGems = new List<S2C_AddItemInfo>();
            for(int i =0;i < gemsId.Length; i++)
            {
                ItemInfo gemInfo = new ItemInfo(player.account, ItemEunm.Gem, gemsId[i], Guid.NewGuid()
                    .ToString(), (int)InventoryEnum.Bag, counts[i]);
                player.mainRole.Inventory.AddItem(gemInfo);
                S2C_addGems.Add(new S2C_AddItemInfo(gemInfo.Guid, gemInfo.ItemId, (int)gemInfo.ItemType,
                    gemInfo.Inventory, gemInfo.Count));
            }
            ServerSys.Instance.Send(player, "CallLuaFunction", "BagCtrl.AddItems",
                new S2C_AddItemsInfo(S2C_addGems));
        }



        ////<玩家id<物品栏类型<物品栏页签<物品类型<物品id<物品Guid,物品信息>>>>>>  存储所有玩家的所有物品信息
        //Dictionary<string, Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<string, ItemBase>>>>>> itemsDic =
        //    new Dictionary<string, Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<string, ItemBase>>>>>>();

        ////存储物品Guid对应的itemId
        //Dictionary<string, int> idsDic = new Dictionary<string, int>();
        ////存储物品Guid对应的itemType
        //Dictionary<string, int> typesDic = new Dictionary<string, int>();
        ////存储物品Guid对应的inventoryPage
        //Dictionary<string, int> pagesDic = new Dictionary<string, int>();
        ////存储物品Guid对应的belongInventory
        //Dictionary<string, int> inventorysDic = new Dictionary<string, int>();
        ////存储物品Guid对应的玩家Id
        //Dictionary<string, string> playerIdsDic = new Dictionary<string, string>();

        //public void InitItem(Player player)
        //{
        //    List<S2C_AddItemInfo> S2C_items = new List<S2C_AddItemInfo>();
        //    List<ItemInfoData> itemDatas = DBSys.Instance.GetAllDatas<ItemInfoData>("account", player.account);
        //    if (itemDatas.Count > 0)
        //    {
        //        foreach (ItemInfoData itemData in itemDatas)
        //        {
        //            ItemInfo item = new ItemInfo(itemData.account, (ItemEunm)itemData.itemType, itemData.itemId,
        //                Guid.NewGuid().ToString(), itemData.belongInventory, itemData.inventoryPage, itemData.count);
        //            AddItem(item);
        //            S2C_items.Add (new S2C_AddItemInfo(item.Guid, item.ItemId, (int)item.ItemType, item.BelongInventory,
        //                item.InventoryPage, item.Count));
        //        }
        //    }
        //    //向客户端发送玩家的所有物品信息
        //    ServerSys.Instance.Send(player, "CallLuaFunction", "InventoryMgr.AddItems", new S2C_AddItemsInfo(S2C_items));
            
        //    //List<EquipInfo> equips = new List<EquipInfo>();
        //    //List<EquipInfoData> equipDatas = DBSys.Instance.GetAllDatas<EquipInfoData>("account", player.account);
        //    //if(equipDatas.Count > 0)
        //    //{
        //    //    foreach(EquipInfoData equipData in equipDatas)
        //    //    {
        //    //        EquipInfo equip = new EquipInfo(equipData.account, (ItemEunm)equipData.itemType, equipData.itemId,
        //    //            equipData.Guid, equipData.belongInventory, equipData.inventoryPage);
        //    //    }
        //    //}

        //    //向客户端发送玩家的所有装备信息
        //}


        ///// <summary>
        ///// 使用物品
        ///// </summary>
        //public void UseItem(string Guid)
        //{
        //    if (!IsExist(Guid))
        //    {
        //        Console.WriteLine($"{Guid}该唯一标识对应的物品不存在");
        //        return;
        //    }

        //    ItemBase item = GetItem(Guid);

        //    switch (item.ItemType)
        //    {
        //        case ItemEunm.Item:
        //            break;
        //        case ItemEunm.Equip:
        //            break;
        //        case ItemEunm.Gem:
        //            break;
        //    }
            
        //}

        ///// <summary>
        ///// 增加物品
        ///// </summary>
        ///// <param name="item"></param>
        //public void AddItem(ItemBase item)
        //{
        //    if(!itemsDic.TryGetValue(item.PlayerId,out Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<string, ItemBase>>>>> player))
        //    {
        //        itemsDic.Add(item.PlayerId, new Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<string, ItemBase>>>>>());
        //    }
        //    if (!itemsDic[item.PlayerId].TryGetValue(item.BelongInventory, out Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<string, ItemBase>>>> inventory))
        //    {
        //        itemsDic[item.PlayerId].Add(item.BelongInventory, new Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<string, ItemBase>>>>());
        //    }
        //    if (!itemsDic[item.PlayerId][item.BelongInventory].TryGetValue(item.InventoryPage, out Dictionary<int, Dictionary<int, Dictionary<string, ItemBase>>> page))
        //    {
        //        itemsDic[item.PlayerId][item.BelongInventory].Add(item.InventoryPage, new Dictionary<int, Dictionary<int, Dictionary<string, ItemBase>>>());
        //    }
        //    if (!itemsDic[item.PlayerId][item.BelongInventory][item.InventoryPage].TryGetValue((int)item.ItemType, out Dictionary<int, Dictionary<string, ItemBase>> type))
        //    {
        //        itemsDic[item.PlayerId][item.BelongInventory][item.InventoryPage].Add((int)item.ItemType, new Dictionary<int, Dictionary<string, ItemBase>>());
        //    }
        //    if (!itemsDic[item.PlayerId][item.BelongInventory][item.InventoryPage][(int)item.ItemType].TryGetValue(item.ItemId, out Dictionary<string, ItemBase> id))
        //    {
        //        itemsDic[item.PlayerId][item.BelongInventory][item.InventoryPage][(int)item.ItemType].Add(item.ItemId, new Dictionary<string, ItemBase>());
        //    }
        //    if (itemsDic[item.PlayerId][item.BelongInventory][item.InventoryPage][(int)item.ItemType][item.ItemId].TryGetValue(item.Guid, out ItemBase itemInfo))
        //    {
        //        Console.WriteLine($"{item.Guid}该id对应的物品已存在");
        //        return;
        //    }

        //    //将物品信息添加进字典
        //    itemsDic[item.PlayerId][item.BelongInventory][item.InventoryPage][(int)item.ItemType][item.ItemId].Add(item.Guid, item);
        //    if(!idsDic.TryGetValue(item.Guid,out int itemId))
        //    {
        //        idsDic.Add(item.Guid, itemId);
        //    }
        //    if(!typesDic.TryGetValue(item.Guid,out int itemType))
        //    {
        //        typesDic.Add(item.Guid, itemType);
        //    }
        //    if(!pagesDic.TryGetValue(item.Guid,out int itemPage))
        //    {
        //        pagesDic.Add(item.Guid, itemPage);
        //    }
        //    if(!inventorysDic.TryGetValue(item.Guid,out int itemInventory))
        //    {
        //        inventorysDic.Add(item.Guid, itemInventory);
        //    }
        //    if (!playerIdsDic.TryGetValue(item.Guid, out string playerId))
        //    {
        //        playerIdsDic.Add(item.Guid, playerId);
        //    }

        //}


        ///// <summary>
        ///// 移除物品
        ///// </summary>
        ///// <param name="Guid"></param>
        //public void RemoveItem(string Guid)
        //{
        //    if (!IsExist(Guid))
        //    {
        //        Console.WriteLine($"{Guid}该唯一标识对应的物品不存在");
        //        return;
        //    }

        //    itemsDic[playerIdsDic[Guid]][inventorysDic[Guid]][pagesDic[Guid]][typesDic[Guid]][idsDic[Guid]].Remove(Guid);

        //    if (idsDic.TryGetValue(Guid, out int itemId))
        //    {
        //        idsDic.Remove(Guid);
        //    }
        //    if (!typesDic.TryGetValue(Guid, out int itemType))
        //    {
        //        typesDic.Remove(Guid);
        //    }
        //    if (!pagesDic.TryGetValue(Guid, out int itemPage))
        //    {
        //        pagesDic.Remove(Guid);
        //    }
        //    if (!inventorysDic.TryGetValue(Guid, out int itemInventory))
        //    {
        //        inventorysDic.Remove(Guid);
        //    }
        //    if (!playerIdsDic.TryGetValue(Guid, out string itemPlayerId))
        //    {
        //        playerIdsDic.Remove(Guid);
        //    }

        //}


        ///// <summary>
        ///// 获取一个物品
        ///// </summary>
        ///// <param name="Guid"></param>
        ///// <returns></returns>
        //public ItemBase GetItem(string Guid)
        //{
        //    if (!IsExist(Guid))
        //    {
        //        Console.WriteLine($"{Guid}该唯一标识对应的物品不存在");
        //        return null;
        //    }
        //    return itemsDic[playerIdsDic[Guid]][inventorysDic[Guid]][pagesDic[Guid]][typesDic[Guid]][idsDic[Guid]][Guid];

        //}


        ///// <summary>
        ///// 刷新物品信息
        ///// </summary>
        ///// <param name="newItem"></param>
        //public void UpdateItem(ItemBase newItem)
        //{
        //    ItemBase item = GetItem(newItem.Guid);
        //    if(item == null)
        //    {
        //        Console.WriteLine($"{newItem.Guid}该唯一标识对应的物品不存在");
        //        return;
        //    }
        //    item = newItem;
        //}


        ///// <summary>
        ///// 获取指定类型的一个物品
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="Guid"></param>
        ///// <returns></returns>
        //public T GetItem<T>(string Guid)where T: ItemBase
        //{
        //    T item = GetItem(Guid) as T;
        //    if(item == null)
        //    {
        //        Console.WriteLine($"{Guid}该唯一标识对应的物品不存在");
        //        return null;
        //    }

        //    return item;
        //}

        ///// <summary>
        ///// 增加多个物品
        ///// </summary>
        ///// <param name="items"></param>
        //public void AddItems(List<ItemBase> items)
        //{
        //    foreach(ItemBase item in items)
        //    {
        //        AddItem(item);
        //    }
        //}


        ///// <summary>
        ///// 移除多个物品
        ///// </summary>
        ///// <param name="Guids"></param>
        //public void RemoveItems(List<string> Guids)
        //{
        //    foreach(string Guid in Guids)
        //    {
        //        RemoveItem(Guid);
        //    }
        //}


        ///// <summary>
        ///// 获取一个玩家的所有物品
        ///// </summary>
        ///// <param name="playerId"></param>
        ///// <returns></returns>
        //public ItemBase[] GetPlayerAllItems(string playerId)
        //{
        //    if (!itemsDic.ContainsKey(playerId)){
        //        Console.WriteLine($"{playerId}该玩家没有物品");
        //        return null;
        //    }
        //    List<ItemBase> items = new List<ItemBase>();

        //    foreach(var inventory in itemsDic[playerId].Values)
        //    {
        //        foreach(var page in inventory.Values)
        //        {
        //            foreach(var type in page.Values)
        //            {
        //                foreach(var id in type.Values)
        //                {
        //                    foreach(ItemBase item in id.Values)
        //                    {
        //                        items.Add(item);
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    if(items.Count < 1)
        //    {
        //        return null;
        //    }

        //    return items.ToArray();

        //}

        ///// <summary>
        ///// 获取指定玩家指定物品栏的所有物品
        ///// </summary>
        ///// <param name="playerId"></param>
        ///// <param name="inventory"></param>
        ///// <returns></returns>
        //public ItemBase[] GetPlayerInventoryItems(string playerId,int inventory)
        //{
        //    if (!itemsDic.ContainsKey(playerId))
        //    {
        //        Console.WriteLine($"{playerId}该唯一标识对应的玩家没有物品");
        //        return null;
        //    }
        //    if (!itemsDic[playerId].ContainsKey(inventory))
        //    {
        //        Console.WriteLine($"{inventory}该唯一标识对应的物品栏不存在");
        //        return null;
        //    }

        //    List<ItemBase> items = new List<ItemBase>();

        //    foreach(var page in itemsDic[playerId][inventory].Values)
        //    {
        //        foreach(var type in page.Values)
        //        {
        //            foreach(var id in type.Values)
        //            {
        //                foreach(ItemBase item in id.Values)
        //                {
        //                    items.Add(item);
        //                }
        //            }
        //        }
        //    }

        //    if(items.Count < 1)
        //    {
        //        return null;
        //    }

        //    return items.ToArray();
        //}

        ///// <summary>
        ///// 获取某个物品栏的某种装备
        ///// </summary>
        ///// <param name="playerId"></param>
        ///// <param name="DressType"></param>
        ///// <returns></returns>
        //public ItemBase[] GetEquips(string playerId, InventoryEnum inventory,EquipDress DressType)
        //{
        //    ItemBase[] equips = GetPlayerInventoryItems(playerId, (int)inventory);
        //    List<ItemBase> reallyEquips = new List<ItemBase>();
        //    if (equips == null || equips.Length < 1)
        //        return null;
        //    foreach(ItemBase equip in equips)
        //    {
        //        if(ConfigSys.Instance.GetConfig<EquipConfig>(equip.ItemId)._type == (int)DressType)
        //        {
        //            reallyEquips.Add(equip);
        //        }
        //    }
        //    if(reallyEquips.Count < 1)
        //        return null;

        //    return reallyEquips.ToArray();
        //}


        ///// <summary>
        ///// 判断物品是否存在
        ///// </summary>
        ///// <param name="Guid"></param>
        ///// <returns></returns>
        //bool IsExist(string Guid)
        //{
        //    if (!idsDic.TryGetValue(Guid, out int id))
        //    {
        //        Console.WriteLine($"{Guid}该唯一标识对应的物品id不存在");
        //        return false;
        //    }
        //    if (!typesDic.TryGetValue(Guid, out int type))
        //    {
        //        Console.WriteLine($"{Guid}该唯一标识对应的物品类型不存在");
        //        return false;
        //    }
        //    if (!pagesDic.TryGetValue(Guid, out int page))
        //    {
        //        Console.WriteLine($"{Guid}该唯一标识对应的物品栏页签不存在");
        //        return false;
        //    }
        //    if (!inventorysDic.TryGetValue(Guid, out int inventory))
        //    {
        //        Console.WriteLine($"{Guid}该唯一标识对应的物品栏不存在");
        //        return false;
        //    }
        //    if (!playerIdsDic.TryGetValue(Guid, out string playerId))
        //    {
        //        Console.WriteLine($"{Guid}该唯一标识对应的玩家Id不存在");
        //        return false;
        //    }
        //    if (!itemsDic[playerId][inventory][page][type][id].ContainsKey(Guid))
        //    {
        //        Console.WriteLine($"{Guid}该唯一标识对应的物品不存在");
        //        return false;
        //    }

        //    return true;
        //}

    }
    public class Inventory
    {
        Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<string ,ItemBase>>>> itemsDic;
        Dictionary<string, int> inventorysDic;
        Dictionary<string, int> typesDic;
        Dictionary<string, int> idsDic;
        public Inventory()
        {
            itemsDic = new Dictionary<int, Dictionary<int, Dictionary<int, Dictionary<string, ItemBase>>>>();
            inventorysDic = new Dictionary<string, int>();
            typesDic = new Dictionary<string, int>();
            idsDic = new Dictionary<string, int>();
        }

        /// <summary>
        /// 添加物品
        /// </summary>
        /// <param name="item"></param>
        public void AddItem(ItemBase item)
        {
            if (!itemsDic.ContainsKey(item.Inventory))
                itemsDic.Add(item.Inventory, new Dictionary<int, Dictionary<int, Dictionary<string, ItemBase>>>());
            if (!itemsDic[item.Inventory].ContainsKey((int)item.ItemType))
                itemsDic[item.Inventory].Add((int)item.ItemType, new Dictionary<int, Dictionary<string, ItemBase>>());
            if (!itemsDic[item.Inventory][(int)item.ItemType].TryGetValue(item.ItemId,out Dictionary<string,ItemBase> items))
                itemsDic[item.Inventory][(int)item.ItemType].Add(item.ItemId, new Dictionary<string, ItemBase>());
            else
            {
                if(items.Values.Count > 0)
                {
                    switch (item.ItemType)
                    {
                        case ItemEunm.Item:
                            (items.Values.ToArray()[0] as ItemInfo).AddCount((item as ItemInfo).Count);
                            return;
                        case ItemEunm.Equip:
                            break;
                        case ItemEunm.Gem:
                            (items.Values.ToArray()[0] as ItemInfo).AddCount((item as ItemInfo).Count);
                            return;
                    }
                }
            }

            if (itemsDic[item.Inventory][(int)item.ItemType][item.ItemId].TryGetValue(item.Guid, out ItemBase Item))
            {
                return;
            }
            itemsDic[item.Inventory][(int)item.ItemType][item.ItemId].Add(item.Guid, item);
            inventorysDic.Add(item.Guid, item.Inventory);
            typesDic.Add(item.Guid, (int)item.ItemType);
            idsDic.Add(item.Guid, item.ItemId);
        }

        /// <summary>
        /// 添加多个物品
        /// </summary>
        /// <param name="items"></param>
        public void AddItems(List<ItemInfo> items)
        {
            foreach (ItemInfo item in items)
            {
                AddItem(item);
            }
        }

        /// <summary>
        /// 添加多个装备
        /// </summary>
        /// <param name="equips"></param>
        public void AddEquips(List<EquipInfo> equips)
        {
            foreach(EquipInfo equip in equips)
            {
                AddItem(equip);
            }
        }

        /// <summary>
        /// 删除物品
        /// </summary>
        /// <param name="Guid"></param>
        public void RemoveItem(string Guid)
        {
            if (!itemsDic.TryGetValue(inventorysDic[Guid], out Dictionary<int, Dictionary<int, Dictionary<string, ItemBase>>> types))
                return;
            if (!types.TryGetValue(typesDic[Guid], out Dictionary<int, Dictionary<string, ItemBase>> ids))
                return;
            if (!ids.TryGetValue(idsDic[Guid], out Dictionary<string, ItemBase> Guids))
                return;
            if (!Guids.TryGetValue(Guid,out ItemBase item))
                return;
            inventorysDic.Remove(Guid);
            typesDic.Remove(Guid);
            idsDic.Remove(Guid);
            Guids.Remove(Guid);

        }

        /// <summary>
        /// 删除物品
        /// </summary>
        /// <param name="Guid"></param>
        public void ReduceItem(string Guid, int effect)
        {
            if (!itemsDic.TryGetValue(inventorysDic[Guid], out Dictionary<int, Dictionary<int, Dictionary<string, ItemBase>>> types))
                return;
            if (!types.TryGetValue(typesDic[Guid], out Dictionary<int, Dictionary<string, ItemBase>> ids))
                return;
            if (!ids.TryGetValue(idsDic[Guid], out Dictionary<string, ItemBase> Guids))
                return;
            if (!Guids.TryGetValue(Guid, out ItemBase item))
                return;

            switch (item.ItemType)
            {
                case ItemEunm.Item:
                    (item as ItemInfo).ReduceCount(effect);
                    if ((item as ItemInfo).Count > 0)
                        return;
                    break;
                case ItemEunm.Equip:
                    break;
                case ItemEunm.Gem:
                    (item as ItemInfo).ReduceCount(effect);
                    if ((item as ItemInfo).Count > 0)
                        return;
                    break;
            }

            inventorysDic.Remove(Guid);
            typesDic.Remove(Guid);
            idsDic.Remove(Guid);
            Guids.Remove(Guid);
        }

        /// <summary>
        /// 获取物品
        /// </summary>
        /// <param name="Guid"></param>
        /// <returns></returns>
        public ItemBase GetItem(string Guid)
        {
            if (!itemsDic.TryGetValue(inventorysDic[Guid], out Dictionary<int, Dictionary<int, Dictionary<string, ItemBase>>> types))
                return null;
            if (!types.TryGetValue(typesDic[Guid], out Dictionary<int, Dictionary<string, ItemBase>> ids))
                return null;
            if (!ids.TryGetValue(idsDic[Guid], out Dictionary<string, ItemBase> Guids))
                return null;
            if (!Guids.TryGetValue(Guid, out ItemBase item))
                return null;
            return item;
        }

        /// <summary>
        /// 获得同种类的所有物品的Guid
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public string[] GetItemsGuid(int itemId)
        {
            if (!itemsDic[(int)InventoryEnum.Bag][(int)ItemEunm.Item].
                TryGetValue(itemId, out Dictionary<string, ItemBase> items))
                return null;
            return items.Keys.ToArray();
        }

        /// <summary>
        /// 获得同种类的所有宝石的Guid
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public string[] GetGemsGuid(int itemId)
        {
            if (!itemsDic[(int)InventoryEnum.Bag][(int)ItemEunm.Gem].
                TryGetValue(itemId, out Dictionary<string, ItemBase> items))
                return null;
            return items.Keys.ToArray();
        }

        /// <summary>
        /// 更新物品
        /// </summary>
        /// <param name="newItem"></param>
        /// <returns></returns>
        public bool UpdateItem(ItemBase newItem)
        {
            if (!itemsDic.TryGetValue(inventorysDic[newItem.Guid], out Dictionary<int, Dictionary<int, Dictionary<string, ItemBase>>> types))
                return false;
            if (!types.TryGetValue(typesDic[newItem.Guid], out Dictionary<int, Dictionary<string, ItemBase>> ids))
                return false;
            if (!ids.TryGetValue(idsDic[newItem.Guid], out Dictionary<string, ItemBase> Guids))
                return false;
            if (!Guids.TryGetValue(newItem.Guid, out ItemBase item))
                return false;
            RemoveItem(newItem.Guid);
            AddItem(newItem);
            return true;
        }

        /// <summary>
        /// 获取某个物品栏的某种类型的所有物品
        /// </summary>
        /// <param name="inventory"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public ItemBase[] GetTypeItems(int inventory,int type)
        {
            if (!itemsDic.TryGetValue(inventory, out Dictionary<int, Dictionary<int, Dictionary<string, ItemBase>>> types))
                return null;
            if (!types.TryGetValue(type, out Dictionary<int, Dictionary<string, ItemBase>> ids))
                return null;
            List<ItemBase> items = new List<ItemBase>();
            foreach(int id in ids.Keys)
            {
                foreach(ItemBase item in ids[id].Values)
                {
                    items.Add(item);
                }
            }
            if (items.Count == 0)
                return null;
            return items.ToArray();
        }

        /// <summary>
        /// 获取装备栏上的所有装备
        /// </summary>
        /// <returns></returns>
        public EquipInfo[] GetEquipbarEquips()
        {
            if (GetInventoryItems((byte)InventoryEnum.Equip) == null)
                return null;
            ItemBase[] items = GetInventoryItems((byte)InventoryEnum.Equip);
            List<EquipInfo> equips = new List<EquipInfo>();
            for(int i = 0;i < items.Length; i++)
            {
                equips.Add(items[i] as EquipInfo);
            }
            return equips.ToArray();
        }

        /// <summary>
        /// 获取装备栏上某个部位的装备
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public EquipInfo GetEquipbarEquip(int type)
        {
            if (GetEquipbarEquips() == null)
                return null;
            EquipInfo[] equips = GetEquipbarEquips();
            for(int i = 0; i < equips.Length; i++)
            {
                if (ConfigSys.Instance.GetConfig<EquipConfig>(equips[i].ItemId)._type == type)
                    return equips[i];
            }
            return null;
        }

        /// <summary>
        /// 减少物品
        /// </summary>
        /// <param name="Guid"></param>
        /// <param name="effect"></param>
        /// <returns></returns>
        //public ItemInfo ReduceItem(string Guid,int effect)
        //{
        //    ItemInfo item =  GetItem(Guid) as ItemInfo;
        //    if (item.Count < effect)
        //        return null;
        //    item.ReduceCount(effect);
        //    if (item.Count == 0)
        //        RemoveItem(item.Guid);
        //    return item;
        //}

        /// <summary>
        /// 获取某个物品栏的所有物品
        /// </summary>
        /// <param name="inventory"></param>
        /// <returns></returns>
        public ItemBase[] GetInventoryItems(int inventory)
        {
            if (!itemsDic.TryGetValue(inventory, out Dictionary<int, Dictionary<int, Dictionary<string, ItemBase>>> types))
                return null;
            List<ItemBase> items = new List<ItemBase>();
            foreach(int type in types.Keys)
            {
                foreach(int id in types[type].Keys)
                {
                    foreach(ItemBase item in types[type][id].Values)
                    {
                        items.Add(item);
                    }
                }
            }
            if (items.Count == 0)
                return null;
            return items.ToArray();
        }

        /// <summary>
        /// 获取所有物品栏的所有物品
        /// </summary>
        /// <returns></returns>
        public ItemBase[] GetAllItems()
        {
            List<ItemBase> items = new List<ItemBase>();
            int[] inventorys = itemsDic.Keys.ToArray();
            for(int i = 0;i < inventorys.Length; i++)
            {
                if(GetInventoryItems(inventorys[i]) != null)
                {
                    ItemBase[] inventoryItems = GetInventoryItems(inventorys[i]);
                    if(inventoryItems.Length != 0)
                    {
                        for(int j = 0; j < inventoryItems.Length; j++)
                        {
                            items.Add(inventoryItems[j]);
                        }
                    }
                }
            }
            if (items.Count == 0)
                return null;
            return items.ToArray();
        }
    }
}

