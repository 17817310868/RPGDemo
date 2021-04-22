/*
 * ===================================================================
 * 
 *          projectName:
 *              项目名称:拱猪
 *                  
 *          title:
 *              标题:登录系统
 *          
 *          description:
 *              功能描述:实现客户端登录和注册功能
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
using UnityEngine;
using Net.Server;
using Net.Share;
using Net.Entity;

namespace Server
{
    class LoginSys
    {
        private static LoginSys instance;
        public static LoginSys Instance {
            get
            {
                if (instance == null)
                    instance = new LoginSys();
                return instance;
            }
        }

        public Player Login(Player unClient,string account,string password)
        {
            PlayerData playerData = DBSys.Instance.GetData<PlayerData>("account", account, "password", password);
            if (playerData == null)
            {
                Console.WriteLine("账号或密码错误");
                ServerSys.Instance.Send(unClient, "CallLuaFunction", "LoginCtrl.LoginCallback",
                    new S2C_LoginOrRegisterCallback(false, "账号或密码错误"));
                return null;
            }
            if (ServerSys.Instance.Players.ContainsKey(account))
            {
                Console.WriteLine("账号已登录");
                ServerSys.Instance.Send(unClient, "CallLuaFunction", "LoginCtrl.LoginCallback",
                    new S2C_LoginOrRegisterCallback(false, "账号已登录"));
                return null;
            }
            Console.WriteLine("登录成功");
            ServerSys.Instance.Send(unClient, "CallLuaFunction", "LoginCtrl.LoginCallback",
                new S2C_LoginOrRegisterCallback(true, "登录成功"));
            unClient.playerID = account;
            unClient.account = account;
            unClient.password = password;
            //RoleSys.Instance.BeginGame(unClient);
            return unClient;
        }

        public void Register(Player unClient,string account,string password,string repassword)
        {
            int count = DBSys.Instance.GetAllDatas<PlayerData>("account", account).Count;
            if (count > 0)
            {
                ServerSys.Instance.Send(unClient, "CallLuaFunction", "RegisterCtrl.RegisterCallback",
                    new S2C_LoginOrRegisterCallback(false, "账号已存在"));
                return;
            }
            PlayerData playerData = new PlayerData(account,password,"MainScene");
            DBSys.Instance.InsertData<PlayerData>(playerData);
            playerData = null;
            ServerSys.Instance.Send(unClient, "CallLuaFunction", "RegisterCtrl.RegisterCallback",
                new S2C_LoginOrRegisterCallback(true, "注册成功"));
        }
    }
}
