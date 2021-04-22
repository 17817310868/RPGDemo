/*
 * ===================================================================
 * 
 *          projectName:
 *              项目名称:拱猪
 *                  
 *          title:
 *              标题:buff系统
 *          
 *          description:
 *              功能描述:管理游戏中所有的buff
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

namespace Server.GameSys
{
    class BuffSys
    {
        private static BuffSys instance;
        public static BuffSys Instance
        {
            get
            {
                if (instance == null)
                    instance = new BuffSys();
                return instance;
            }
        }
    }
}
