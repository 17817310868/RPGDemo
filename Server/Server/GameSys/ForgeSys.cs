/*
 * ===================================================================
 * 
 *          projectName:
 *              项目名称:拱猪
 *                  
 *          title:
 *              标题:锻造系统
 *          
 *          description:
 *              功能描述:实现打造，镶嵌摘除和进阶功能逻辑
 *              
 *          author:
 *              作者:照着教程敲出bug的程序员
 * 
 * ===================================================================
 */

using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Server.GameSys
{
    class ForgeSys
    {

        private static ForgeSys instance;
        public static ForgeSys Instance
        {
            get
            {
                if (instance == null)
                    instance = new ForgeSys();
                return instance;
            }
        }

        /// <summary>
        /// 打造装备
        /// </summary>
        /// <param name="player"></param>
        /// <param name="Guid"></param>
        public void Make(Player player, string Guid)
        {
            if (player.mainRole.Inventory.GetItem(Guid) == null)
                return;
            ItemInfo book = player.mainRole.Inventory.GetItem(Guid) as ItemInfo;
            int makeBookId = book.ItemId;
            int formulaId = ConfigSys.Instance.GetConfig<ItemConfig>(makeBookId).formulaId;
            ItemBase[] items = player.mainRole.Inventory.GetTypeItems((int)InventoryEnum.Bag,
                (int)ItemEunm.Item);
            ItemInfo materialOne = null;
            ItemInfo materialTwo = null;
            ItemInfo materialThree = null;
            ItemInfo materialFour = null;

            List<S2C_ReduceItem> S2C_reduceItems = new List<S2C_ReduceItem>();
            List<S2C_AddItemInfo> S2C_addItems = new List<S2C_AddItemInfo>();

            foreach (ItemBase item in items)
            {
                if (item.ItemId == ConfigSys.Instance.GetConfig<FormulaConfig>(formulaId).materialOne)
                    materialOne = item as ItemInfo;
                if (item.ItemId == ConfigSys.Instance.GetConfig<FormulaConfig>(formulaId).materialTwo)
                    materialTwo = item as ItemInfo;
                if (item.ItemId == ConfigSys.Instance.GetConfig<FormulaConfig>(formulaId).materialThree)
                    materialThree = item as ItemInfo;
                if (item.ItemId == ConfigSys.Instance.GetConfig<FormulaConfig>(formulaId).materialFour)
                    materialFour = item as ItemInfo;
            }

            if ((materialOne == null) || (materialTwo == null) || (materialThree == null)
                || (materialFour == null))
            {
                return;
            }

            S2C_reduceItems.Add(new S2C_ReduceItem(book.Guid,1));
            S2C_reduceItems.Add(new S2C_ReduceItem(materialOne.Guid,1));
            S2C_reduceItems.Add(new S2C_ReduceItem(materialTwo.Guid,1));
            S2C_reduceItems.Add(new S2C_ReduceItem(materialThree.Guid,1));
            S2C_reduceItems.Add(new S2C_ReduceItem(materialFour.Guid,1));
            ServerSys.Instance.Send(player, "CallLuaFunction", "BagCtrl.ReduceItems",
                new S2C_ReduceItems(S2C_reduceItems));

            player.mainRole.Inventory.ReduceItem(book.Guid, 1);
            player.mainRole.Inventory.ReduceItem(materialOne.Guid, 1);
            player.mainRole.Inventory.ReduceItem(materialTwo.Guid, 1);
            player.mainRole.Inventory.ReduceItem(materialThree.Guid, 1);
            player.mainRole.Inventory.ReduceItem(materialFour.Guid, 1);
            EquipInfo equip = new EquipInfo(player.account, ItemEunm.Equip, ConfigSys.Instance.
                GetConfig<ItemConfig>(makeBookId).composeId, System.Guid.NewGuid().ToString(),
                (int)InventoryEnum.Bag, new int[2]);
            player.mainRole.Inventory.AddItem(equip);

            ServerSys.Instance.Send(player, "CallLuaFunction", "BagCtrl.AddItems",
                new S2C_AddEquipsInfo(new List<S2C_AddEquipInfo>() { new S2C_AddEquipInfo(
                    equip.Guid,equip.ItemId,(int)equip.ItemType,equip.Inventory,equip.gems)}));

        }

        /// <summary>
        /// 镶嵌宝石
        /// </summary>
        /// <param name="player"></param>
        /// <param name="C2S_inlayGem"></param>
        public void InlayGem(Player player,C2S_InlayGem C2S_inlayGem)
        {
            if(player.mainRole.Inventory.GetItem(C2S_inlayGem.equipGuid) == null)
            {
                return;
            }

            List<S2C_ReduceItem> S2C_reduceItems = new List<S2C_ReduceItem>();
            List<S2C_AddItemInfo> S2C_addItems = new List<S2C_AddItemInfo>();

            EquipInfo equip = player.mainRole.Inventory.GetItem(C2S_inlayGem.equipGuid) as EquipInfo;

            if(player.mainRole.Inventory.GetItem(C2S_inlayGem.gemGuid) == null)
            {
                return;
            }

            ItemInfo gem = player.mainRole.Inventory.GetItem(C2S_inlayGem.gemGuid) as ItemInfo;

            
            if(equip.gems[C2S_inlayGem.hole] != 0)
            {
                return;
            }

            equip.gems[C2S_inlayGem.hole] = gem.ItemId;

            ServerSys.Instance.Send(player, "CallLuaFunction", "BagCtrl.UpdateItem",
                new S2C_AddEquipInfo(equip.Guid, equip.ItemId, (int)equip.ItemType,
                (int)equip.Inventory, equip.gems));

            S2C_reduceItems.Add(new S2C_ReduceItem( gem.Guid,1));

            ServerSys.Instance.Send(player, "CallLuaFunction", "BagCtrl.ReduceItems",
                new S2C_ReduceItems(S2C_reduceItems));

            player.mainRole.Inventory.ReduceItem(gem.Guid, 1);

            //UpdateItem(player, player.mainRole.Inventory.ReduceItem(gem.Guid, 1));

            if(equip.Inventory == (int)InventoryEnum.Equip)
            {
                AttrSys.Instance.AddGemAttr(player, gem.ItemId);
                RoleSys.Instance.UpdatePlayerMainRole(player);
            }

        }

        /// <summary>
        /// 摘除宝石
        /// </summary>
        /// <param name="player"></param>
        /// <param name="C2S_removeGem"></param>
        public void RemoveGem(Player player,C2S_RemoveGem C2S_removeGem)
        {
            if (player.mainRole.Inventory.GetItem(C2S_removeGem.equipGuid) == null)
            {
                return;
            }

            EquipInfo equip = player.mainRole.Inventory.GetItem(C2S_removeGem.equipGuid) as EquipInfo;

            if (equip.gems[C2S_removeGem.hole] == 0)
                return;

            if (equip.Inventory == (int)InventoryEnum.Equip)
            {
                AttrSys.Instance.ReduceGemAttr(player, equip.gems[0]);
                RoleSys.Instance.UpdatePlayerMainRole(player);
            }

            string[] gemsGuid = player.mainRole.Inventory.GetGemsGuid(equip.gems[C2S_removeGem.hole]);
            int gemId = equip.gems[C2S_removeGem.hole];
            ItemInfo gem = new ItemInfo(player.playerID, ItemEunm.Gem, gemId, Guid.NewGuid().ToString(),
                (byte)InventoryEnum.Bag,1);
            player.mainRole.Inventory.AddItem(gem);
            ServerSys.Instance.Send(player, "CallLuaFunction", "BagCtrl.AddItems",
                new S2C_AddItemsInfo(new List<S2C_AddItemInfo>() { new S2C_AddItemInfo(
                gem.Guid,gem.ItemId,(int)gem.ItemType,gem.Inventory,gem.Count)}));
            //for (int i = 0; i < gemsGuid.Length; i++)
            //{
            //    string Guid = gemsGuid[i];
            //    ItemInfo gem = player.mainRole.Inventory.GetItem(Guid) as ItemInfo;
            //    if(gem.Count < ConfigSys.Instance.GetConfig<GemConfig>(gem.ItemId).maxCount)
            //    {
            //        gem.AddCount(1);
            //        ServerSys.Instance.Send(player, "CallLuaFunction", "BagCtrl.UpdateItem",
            //        new S2C_AddItemInfo(gem.Guid, gem.ItemId, (int)gem.ItemType,
            //        (int)gem.Inventory, gem.Count));
            //        break;
            //    }
            //    if(i == gemsGuid.Length - 1)
            //    {
            //        ItemInfo item = new ItemInfo(player.playerID,
            //            ItemEunm.Gem, gem.ItemId, System.Guid.NewGuid().ToString(),
            //            (int)InventoryEnum.Bag, 1);
            //        player.mainRole.Inventory.AddItem(item);
            //        ServerSys.Instance.Send(player, "CallLuaFunction", "BagCtrl.AddItems",
            //        new S2C_AddItemsInfo(new List<S2C_AddItemInfo>() { new S2C_AddItemInfo(
            //        item.Guid,item.ItemId,(int)item.ItemType,item.Inventory,item.Count)}));
            //    }
            //}

            equip.gems[C2S_removeGem.hole] = 0;

            player.mainRole.Inventory.UpdateItem(new EquipInfo(equip.PlayerId, equip.ItemType,
                equip.ItemId, equip.Guid, equip.Inventory, equip.gems));

            ServerSys.Instance.Send(player, "CallLuaFunction", "BagCtrl.UpdateItem",
                new S2C_AddEquipInfo(equip.Guid, equip.ItemId, (int)equip.ItemType,
                 (int)equip.Inventory, equip.gems));

            

        }

        /// <summary>
        /// 装备进阶
        /// </summary>
        /// <param name="player"></param>
        /// <param name="C2S_advanceEquip"></param>
        public void AdvanceEquip(Player player,C2S_AdvanceEquip C2S_advanceEquip)
        {
            if(player.mainRole.Inventory.GetItem(C2S_advanceEquip.Guid) == null)
            {
                return;
            }
            EquipInfo equip = player.mainRole.Inventory.GetItem(C2S_advanceEquip.Guid) as EquipInfo;
            if(ConfigSys.Instance.GetConfig<EquipConfig>(equip.ItemId).quality > 4)
            {
                return;
            }
            if(player.mainRole.Gold < ConfigSys.Instance.GetConfig<EquipConfig>(equip.ItemId).
                advanceGold)
            {
                return;
            }

            player.mainRole.Inventory.UpdateItem(new EquipInfo(equip.PlayerId, equip.ItemType,
                equip.ItemId+1, equip.Guid, equip.Inventory, equip.gems));

            AttrSys.Instance.ReduceEquipAttr(player, equip.ItemId);
            AttrSys.Instance.AddEquipAttr(player, equip.ItemId + 1);

            ServerSys.Instance.Send(player, "CallLuaFunction", "BagCtrl.UpdateItem",
                new S2C_AddEquipInfo(equip.Guid, equip.ItemId + 1, (int)equip.ItemType,
                 (int)equip.Inventory, equip.gems));

            player.mainRole.ReduceGold(ConfigSys.Instance.GetConfig<EquipConfig>(equip.ItemId).
                advanceGold);

            RoleSys.Instance.UpdatePlayerMainRole(player);

        }
    }
}
