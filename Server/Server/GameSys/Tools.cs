/*
 * ===================================================================
 * 
 *          projectName:
 *              项目名称:拱猪
 *                  
 *          title:
 *              标题:工具类
 *          
 *          description:
 *              功能描述:封装一些工具
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

namespace Server
{
    public static class Tools
    {
        private static Random random;
        public static Random _Random
        {
            get
            {
                if (random == null)
                    random = new Random();
                return random;
            }
        }
    }
}
