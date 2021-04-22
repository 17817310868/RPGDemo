/*
 * ===================================================================
 * 
 *          projectName:
 *              项目名称:拱猪
 *                  
 *          title:
 *              标题:装备信息类
 *          
 *          description:
 *              功能描述:设计装备信息
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

namespace Server
{
    public class EquipInfo:ItemBase
    {
        public EquipInfo(string playerId,ItemEunm itemType,int itemId,string guid,int inventory,
            int[] gems)
            : base(playerId, itemType, itemId, guid, inventory)
        {
            this.gems = gems;
        }

        public int[] gems;
    }
}
