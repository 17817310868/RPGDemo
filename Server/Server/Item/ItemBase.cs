/*
 * ===================================================================
 * 
 *          projectName:
 *              项目名称:拱猪
 *                  
 *          title:
 *              标题:物品基类
 *          
 *          description:
 *              功能描述:设计物品基本信息
 *              
 *          author:
 *              作者:
 * 
 * ===================================================================
 */

using Server.GameSys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Server
{
    public class ItemBase
    {
        public ItemBase(string playerId,ItemEunm itemType,int itemId,string guid,int inventory)
        {
            this.playerId = playerId;
            this.itemType = itemType;
            this.itemId = itemId;
            this.guid = guid;
            this.inventory = inventory;
        }

        private string playerId;
        private ItemEunm itemType;
        private int itemId;
        private string guid;

        private int inventory;
        public string PlayerId { get { return playerId; } }
        public ItemEunm ItemType { get { return itemType; } }
        public int ItemId { get { return itemId; } }
        public string Guid { get { return guid; } }

        public int Inventory { get { return inventory; } }

        public void ChangePlayer(string playerId)
        {
            this.playerId = playerId;
        }
        
    }
}
