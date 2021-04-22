/*
 * ===================================================================
 * 
 *          projectName:
 *              项目名称:拱猪
 *                  
 *          title:
 *              标题:物品信息类
 *          
 *          description:
 *              功能描述:设计普通物品信息
 *              
 *          author:
 *              作者:
 * 
 * ===================================================================
 */

using MongoDB.Driver.Core.Operations;
using Server.GameSys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class ItemInfo : ItemBase
    {
        public ItemInfo(string playerId,ItemEunm itemType,int itemId,string guid,int inventory,
            int count) : base(playerId, itemType, itemId, guid, inventory)
        {
            this.count = count;
        }
        private int count;
        public int Count { get { return count; } }

        public void ChangeCount(int count)
        {
            this.count = count;
        }

        public void AddCount(int offest)
        {
            count += offest;
        }

        public void ReduceCount(int offest)
        {
            count -= offest;
        }
    }
}
