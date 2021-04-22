/*
 * ===================================================================
 * 
 *          projectName:
 *              项目名称:拱猪
 *                  
 *          title:
 *              标题:物品系统
 *          
 *          description:
 *              功能描述:物品的具体处理逻辑
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
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

namespace Server.GameSys
{

    public enum ProfessionType
    {
        None = 0,
        NanQiang,
        NvGong,
        NanShan,
        NvZhang,
        NanJian
    }

    public class ItemSys
    {
        private static ItemSys instance;
        public static ItemSys Instance
        {
            get
            {
                if (instance == null)
                    instance = new ItemSys();
                return instance;
            }
        }

        Dictionary<int, ItemAction> itemAciontsDic = new Dictionary<int, ItemAction>();

        public void Init()
        {
            itemAciontsDic.Add(11010001, new XiuYD());
            itemAciontsDic.Add(11010002, new QiXD());
            itemAciontsDic.Add(11010003, new YuanQd());
            itemAciontsDic.Add(12020010, new Lingjlb());
            itemAciontsDic.Add(12020011, new ShiJLB());
            itemAciontsDic.Add(12020012, new ErSJLB());
            itemAciontsDic.Add(11110001, new CaiLiao());
            itemAciontsDic.Add(11110002, new CaiLiao());
            itemAciontsDic.Add(11120001, new CaiLiao());
            itemAciontsDic.Add(11120002, new CaiLiao());
            itemAciontsDic.Add(11120101, new CaiLiao());
            itemAciontsDic.Add(11120102, new CaiLiao());
            itemAciontsDic.Add(11120201, new CaiLiao());
            itemAciontsDic.Add(11120202, new CaiLiao());
            itemAciontsDic.Add(11190001, new CaiLiao());
            itemAciontsDic.Add(11190002, new CaiLiao());
            itemAciontsDic.Add(11190101, new CaiLiao());
            itemAciontsDic.Add(11190102, new CaiLiao());
            itemAciontsDic.Add(11190201, new CaiLiao());
            itemAciontsDic.Add(11190202, new CaiLiao());
            itemAciontsDic.Add(11190301, new CaiLiao());
            itemAciontsDic.Add(11190302, new CaiLiao());
            itemAciontsDic.Add(11190401, new CaiLiao());
            itemAciontsDic.Add(11190402, new CaiLiao());
            itemAciontsDic.Add(11190501, new CaiLiao());
            itemAciontsDic.Add(11190502, new CaiLiao());
            itemAciontsDic.Add(11190701, new CaiLiao());
            itemAciontsDic.Add(11190702, new CaiLiao());
            itemAciontsDic.Add(11191001, new CaiLiao());
            itemAciontsDic.Add(11191002, new CaiLiao());
            itemAciontsDic.Add(11191101, new CaiLiao());
            itemAciontsDic.Add(11191102, new CaiLiao());
        }

        public void Exe(Player player,string Guid)
        {
            if(player.mainRole.Inventory.GetItem(Guid) == null)
            {
                return;
            }

            ItemInfo item = player.mainRole.Inventory.GetItem(Guid) as ItemInfo;
            itemAciontsDic[item.ItemId].Exe(player, Guid);

        }

        public void Battle(int itemId,Battle battle, Battler actor, Battler victim, ref S2C_BattleMessage S2C_battleMessage)
        {
            if (!itemAciontsDic.TryGetValue(itemId, out ItemAction action))
                return;
            action.Battle(battle, actor, victim, ref S2C_battleMessage);
        }

    }


    public class ItemAction
    {
        public virtual void Exe(Player player,string Guid)
        {

        }

        public virtual void Battle(Battle battle, Battler actor, Battler victim, ref S2C_BattleMessage S2C_battleMessage)
        {

        }
    }

    public class XiuYD : ItemAction
    {
        public override void Exe(Player player, string Guid)
        {
            player.mainRole.AddExperience(500);
            player.mainRole.Inventory.ReduceItem(Guid, 1);
            ServerSys.Instance.Send(player, "CallLuaFunction", "BagCtrl.ReduceItems",
                new S2C_ReduceItems(new List<S2C_ReduceItem>() {
                new S2C_ReduceItem(Guid,1)}));
            RoleSys.Instance.UpdatePlayerMainRole(player);
        }
    }

    public class QiXD: ItemAction
    {
        public override void Exe(Player player, string Guid)
        {
            player.mainRole.AddHp(100);
            if (player.mainRole.Hp > player.mainRole.MaxHp)
                player.mainRole.ChangeHp(player.mainRole.MaxHp);
            player.mainRole.Inventory.ReduceItem(Guid, 1);
            ServerSys.Instance.Send(player, "CallLuaFunction", "BagCtrl.ReduceItems",
                new S2C_ReduceItems(new List<S2C_ReduceItem>() {
                new S2C_ReduceItem(Guid,1)}));
            RoleSys.Instance.UpdatePlayerMainRole(player);
        }

        public override void Battle(Battle battle, Battler actor, Battler victim, ref S2C_BattleMessage S2C_battleMessage)
        {
            string[] Guids = (actor.Role as MainRole).Inventory.GetItemsGuid(S2C_battleMessage.paramId);
            string Guid = Guids[0];
            ItemInfo itemInfo = (actor.Role as MainRole).Inventory.GetItem(Guid) as ItemInfo;
            (actor.Role as MainRole).Inventory.ReduceItem(Guid, 1);
            ServerSys.Instance.Send(ServerSys.Instance.Players[(actor.Role as MainRole).Guid],
                "CallLuaFunction", "BagCtrl.ReduceItems",
                new S2C_ReduceItems(new List<S2C_ReduceItem>() {
                new S2C_ReduceItem(Guid,1)}));
            S2C_battleMessage.actorIndex = actor.PositionIndex;
            S2C_battleMessage.victims = new byte[] {victim.PositionIndex };
            S2C_battleMessage.targetsHpEffect.Add(victim.PositionIndex, 100);
            actor.Role.AddHp(100);
            if (actor.Role.Hp > actor.Role.MaxHp)
                actor.Role.ChangeHp(actor.Role.MaxHp);
        }
    }

    public class YuanQd : ItemAction
    {
        public override void Exe(Player player, string Guid)
        {
            player.mainRole.AddMp(100);
            if (player.mainRole.Mp > player.mainRole.MaxMp)
                player.mainRole.ChangeMp(player.mainRole.MaxMp);
            player.mainRole.Inventory.ReduceItem(Guid, 1);
            ServerSys.Instance.Send(player, "CallLuaFunction", "BagCtrl.ReduceItems",
                new S2C_ReduceItems(new List<S2C_ReduceItem>() {
                new S2C_ReduceItem(Guid,1)}));
            RoleSys.Instance.UpdatePlayerMainRole(player);
        }
    }

    public class Lingjlb : ItemAction
    {
        public override void Exe(Player player, string Guid)
        {
            //删除该礼包
            player.mainRole.Inventory.ReduceItem(Guid,1);
            ServerSys.Instance.Send(player, "CallLuaFunction", "BagCtrl.ReduceItems",
                new S2C_ReduceItems(new List<S2C_ReduceItem>() {
                new S2C_ReduceItem(Guid,1)}));
            List<ItemInfo> items = new List<ItemInfo>();
            //根据职业添加0级武器制造书物品
            switch (player.mainRole.Profession)
            {
                case (int)ProfessionType.NanQiang:
                    items.Add(new ItemInfo(player.account, ItemEunm.Item, 11190001,
                System.Guid.NewGuid().ToString(), (int)InventoryEnum.Bag, 1));
                    break;
                case (int)ProfessionType.NvGong:
                    items.Add(new ItemInfo(player.account, ItemEunm.Item, 11190101,
                System.Guid.NewGuid().ToString(), (int)InventoryEnum.Bag, 1));
                    break;
                case (int)ProfessionType.NanShan:
                    items.Add(new ItemInfo(player.account, ItemEunm.Item, 11190201,
                System.Guid.NewGuid().ToString(), (int)InventoryEnum.Bag, 1));
                    break;
                case (int)ProfessionType.NvZhang:
                    items.Add(new ItemInfo(player.account, ItemEunm.Item, 11190301,
                System.Guid.NewGuid().ToString(), (int)InventoryEnum.Bag, 1));
                    break;
                case (int)ProfessionType.NanJian:
                    items.Add(new ItemInfo(player.account, ItemEunm.Item, 11190401,
                System.Guid.NewGuid().ToString(), (int)InventoryEnum.Bag, 1));
                    break;
            }

            //添加各个部位的装备0级制造书
            items.Add(new ItemInfo(player.account, ItemEunm.Item, 11190501,
                System.Guid.NewGuid().ToString(), (int)InventoryEnum.Bag, 1));
            items.Add(new ItemInfo(player.account, ItemEunm.Item, 11190701,
                System.Guid.NewGuid().ToString(), (int)InventoryEnum.Bag, 1));
            items.Add(new ItemInfo(player.account, ItemEunm.Item, 11191001,
                System.Guid.NewGuid().ToString(), (int)InventoryEnum.Bag, 1));
            items.Add(new ItemInfo(player.account, ItemEunm.Item, 11191101,
                System.Guid.NewGuid().ToString(), (int)InventoryEnum.Bag, 1));
            items.Add(new ItemInfo(player.account, ItemEunm.Item, 11191201,
                System.Guid.NewGuid().ToString(), (int)InventoryEnum.Bag, 1));
            //添加各种0级打造材料六个
            items.Add(new ItemInfo(player.account, ItemEunm.Item, 11110001,
                System.Guid.NewGuid().ToString(), (int)InventoryEnum.Bag, 6));
            items.Add(new ItemInfo(player.account, ItemEunm.Item, 11120001,
                System.Guid.NewGuid().ToString(), (int)InventoryEnum.Bag, 6));
            items.Add(new ItemInfo(player.account, ItemEunm.Item, 11120101,
                System.Guid.NewGuid().ToString(), (int)InventoryEnum.Bag, 6));
            items.Add(new ItemInfo(player.account, ItemEunm.Item, 11120201,
                System.Guid.NewGuid().ToString(), (int)InventoryEnum.Bag, 6));
            //添加一个10级礼包
            items.Add(new ItemInfo(player.account, ItemEunm.Item, 12020011,
                System.Guid.NewGuid().ToString(), (int)InventoryEnum.Bag, 1));

            player.mainRole.Inventory.AddItems(items);

            List<S2C_AddItemInfo> S2C_addItems = new List<S2C_AddItemInfo>();
            for(int i = 0; i < items.Count; i++)
            {
                ItemInfo item = items[i];
                S2C_addItems.Add(new S2C_AddItemInfo(item.Guid, item.ItemId, (int)item.ItemType,
                    (int)item.Inventory, item.Count));
            }

            ServerSys.Instance.Send(player, "CallLuaFunction", "BagCtrl.AddItems",
                new S2C_AddItemsInfo(S2C_addItems));
        }
    }

    public class ShiJLB : ItemAction
    {
        public override void Exe(Player player, string Guid)
        {
            
        }
    }

    public class ErSJLB : ItemAction
    {
        public override void Exe(Player player, string Guid)
        {
            
        }
    }

    public class CaiLiao : ItemAction
    {
        public override void Exe(Player player, string Guid)
        {
            
        }
    }
}
