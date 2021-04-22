/*
 * ===================================================================
 * 
 *          projectName:
 *              项目名称:拱猪
 *                  
 *          title:
 *              标题:伙伴信息模型
 *          
 *          description:
 *              功能描述:设计伙伴信息
 *              
 *          author:
 *              作者:
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
    class PartnerRole:RoleBase
    {
        public PartnerRole(string guid,RoleEnum roleType,int profession) : base(guid,roleType, profession)
        {

        }
    }
}
