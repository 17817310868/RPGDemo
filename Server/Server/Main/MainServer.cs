/*
 * ===================================================================
 * 
 *          projectName:
 *              项目名称:拱猪
 *                  
 *          title:
 *              标题:服务器入口
 *          
 *          description:
 *              功能描述:启动服务器
 *              
 *          author:
 *              作者:
 * 
 * ===================================================================
 */

using GameDesigner;
using Net.Entity;
using Server.GameSys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using UnityEngine;

namespace Server
{
    class MainServer
    {

        static void Main(string[] args)
        {

            ServerSys server = new ServerSys();
            server.Start();
            
            server.Log = server.Log + Console.WriteLine;
            ConfigSys.Instance.ExcelToByte();
            ConfigSys.Instance.Init();
            SkillSys.Instance.Init();
            ItemSys.Instance.Init();
            //DBSys.Instance.InsertData<ItemInfoData>(new ItemInfoData(Guid.NewGuid().ToString(),"123456",
            //    (int)ItemEunm.Item,11010001, (int)InventoryEnum.Bag, 1));
            //DBSys.Instance.InsertData<EquipInfoData>(new EquipInfoData(Guid.NewGuid().ToString(), "123456",
            //    (int)ItemEunm.Equip,10070004, (int)InventoryEnum.Bag, new int[2]));
            //DBSys.Instance.InsertData<EquipInfoData>(new EquipInfoData(Guid.NewGuid().ToString(), "123456", 
            //    (int)ItemEunm.Equip,10100002, (int)InventoryEnum.Bag, new int[2]));
            //DBSys.Instance.InsertData<EquipInfoData>(new EquipInfoData(Guid.NewGuid().ToString(), "123456",
            //    (int)ItemEunm.Equip,10110003, (int)InventoryEnum.Bag, new int[2]));
            //DBSys.Instance.InsertData<EquipInfoData>(new EquipInfoData(Guid.NewGuid().ToString(), "123456", 
            //    (int)ItemEunm.Equip,10120000, (int)InventoryEnum.Bag, new int[2]));
            //DBSys.Instance.InsertData<EquipInfoData>(new EquipInfoData(Guid.NewGuid().ToString(), "123456",
            //    (int)ItemEunm.Equip,10070001, (int)InventoryEnum.Bag, new int[2]));
            InventorySys.Instance.AddCommodity(10010000, (int)ItemEunm.Equip, (int)InventoryEnum.Shop);
            InventorySys.Instance.AddCommodity(10100003, (int)ItemEunm.Equip, (int)InventoryEnum.Shop);
            InventorySys.Instance.AddCommodity(10110003, (int)ItemEunm.Equip, (int)InventoryEnum.Shop);
            InventorySys.Instance.AddCommodity(11010001, (int)ItemEunm.Item, (int)InventoryEnum.Shop);
            InventorySys.Instance.AddCommodity(11010002, (int)ItemEunm.Item, (int)InventoryEnum.Shop);
            InventorySys.Instance.AddCommodity(11010003, (int)ItemEunm.Item, (int)InventoryEnum.Shop);
            //InventorySys.Instance.AddCommodity(11010001, (int)ItemEunm.Item, (int)InventoryEnum.Shop);
            //InventorySys.Instance.AddCommodity(11010001, (int)ItemEunm.Item, (int)InventoryEnum.Shop);
            InventorySys.Instance.AddCommodity(14010004, (int)ItemEunm.Gem, (int)InventoryEnum.Shop);
            InventorySys.Instance.AddCommodity(14020005, (int)ItemEunm.Gem, (int)InventoryEnum.Shop);
            InventorySys.Instance.AddCommodity(14040003, (int)ItemEunm.Gem, (int)InventoryEnum.Shop);
            RankingSys.Instance.InitRanking();
            
            Thread thread = new Thread(new ThreadStart(() =>
            {
                while (true)
                {
                    Thread.Sleep(100000000);
                    Console.WriteLine("刷新");
                    EventSys.Instance.Trigger("RoleDataSave");
                    EventSys.Instance.Trigger("UpdateRanking");
                    EventSys.Instance.Trigger("UpdateAuction");
                }
            }));

            thread.Start();

            while (true) {
                
            }
        }
    }
}
