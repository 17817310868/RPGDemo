/*
 * ===================================================================
 * 
 *          projectName:
 *              项目名称:拱猪
 *                  
 *          title:
 *              标题:玩家信息模型
 *          
 *          description:
 *              功能描述:设计玩家信息
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
using Net.Server;
using Net.Share;
using MongoDB.Bson;
using Net;

namespace Server
{
    public class Player:NetPlayer
    {

        public ObjectId id;
        public string account;
        public string password;

        public MainRole mainRole;

    }
}
