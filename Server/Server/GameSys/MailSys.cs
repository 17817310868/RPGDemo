/*
 * ===================================================================
 * 
 *          projectName:
 *              项目名称:拱猪
 *                  
 *          title:
 *              标题:邮件系统
 *          
 *          description:
 *              功能描述:实现邮件处理
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
using UnityEngine.Experimental.UIElements;

namespace Server.GameSys
{
    class MailSys
    {

        private static MailSys instance;
        public static MailSys Instance
        {
            get
            {
                if (instance == null)
                    instance = new MailSys();
                return instance;
            }
        }

        public void InitMails(Player player)
        {
            List<MailData> mails = DBSys.Instance.GetAllDatas<MailData>("account", player.account);
            if (mails.Count == 0)
                return;
            bool isHaveNotRead = false;
            for(int i = 0; i < mails.Count; i++)
            {
                Mail mail = new Mail();
                mail.Guid = mails[i].Guid;
                mail.title = mails[i].title;
                mail.content = mails[i].content;
                mail.addresserName = mails[i].addresserName;
                mail.isRead = mails[i].isRead;
                mail.isExistItem = mails[i].isExistItem;
                mail.items = mails[i].items;
                mail.equips = mails[i].equips;
                mail.gold = mails[i].gold;
                player.mainRole.Mails.Add(mail);
                if (!isHaveNotRead && !mail.isRead)
                    isHaveNotRead = true;
            }
            ServerSys.Instance.Send(player, "CallLuaFunction", "MainUICtrl.ReceiveNewMail");
        }

        public void GetItems(Player player,C2S_GetMailItems C2S_getMailItems)
        {
            if (player.mainRole.Mails.Count == 0)
                return;
            Mail mail = null;
            for(int i = 0; i < player.mainRole.Mails.Count; i++)
            {
                if(player.mainRole.Mails[i].Guid == C2S_getMailItems.Guid)
                {
                    mail = player.mainRole.Mails[i];
                    break;
                }
            }
            if (mail == null)
                return;
            player.mainRole.AddGold(mail.gold);
            RoleSys.Instance.UpdatePlayerMainRole(player);
            mail.gold = 0;
            if (!mail.isExistItem)
                return;
            mail.isExistItem = false;
            if (mail.items.Count != 0)
            {
                List<S2C_AddItemInfo> S2C_addItems = new List<S2C_AddItemInfo>();
                foreach(ItemInfo item in mail.items)
                {
                    item.ChangePlayer(player.playerID);
                    player.mainRole.Inventory.AddItem(item);
                    S2C_addItems.Add(new S2C_AddItemInfo(item.Guid, item.ItemId, (int)item.ItemType,
                        item.Inventory, item.Count));
                }
                ServerSys.Instance.Send(player, "CallLuaFunction", "BagCtrl.AddItems",
                    new S2C_AddItemsInfo(S2C_addItems));
            }
            mail.items.Clear();
            if(mail.equips.Count != 0)
            {
                List<S2C_AddEquipInfo> S2C_addEquips = new List<S2C_AddEquipInfo>();
                foreach(EquipInfo equip in mail.equips)
                {
                    equip.ChangePlayer(player.playerID);
                    player.mainRole.Inventory.AddItem(equip);
                    S2C_addEquips.Add(new S2C_AddEquipInfo(equip.Guid, equip.ItemId, (int)equip.ItemType,
                        equip.Inventory, equip.gems));
                }
                ServerSys.Instance.Send(player, "CallLuaFunction", "BagCtrl.AddItems",
                    new S2C_AddEquipsInfo(S2C_addEquips));
                //添加装备
            }
            mail.equips.Clear();
            //刷新邮件信息
            GetMails(player);
        }

        public void ReadMail(Player player,C2S_ReadMail C2S_readMail)
        {
            if (player.mainRole.Mails.Count == 0)
                return;
            Mail mail = null;
            for (int i = 0; i < player.mainRole.Mails.Count; i++)
            {
                if (player.mainRole.Mails[i].Guid == C2S_readMail.Guid)
                {
                    mail = player.mainRole.Mails[i];
                    break;
                }
            }
            if (mail == null)
                return;
            if (mail.isRead == false)
                mail.isRead = true;
        }

        public void GetMails(Player player)
        {
            Console.WriteLine("邮件数:" + player.mainRole.Mails.Count);
            if (player.mainRole.Mails.Count == 0)
                return;
            List<S2C_ReceiveMail> S2C_receiveMails = new List<S2C_ReceiveMail>();
            for (int i = 0; i < player.mainRole.Mails.Count; i++)
            {
                S2C_ReceiveMail mail = new S2C_ReceiveMail();
                mail.Guid = player.mainRole.Mails[i].Guid;
                mail.addresserName = player.mainRole.Mails[i].addresserName;
                mail.title = player.mainRole.Mails[i].title;
                mail.content = player.mainRole.Mails[i].content;
                mail.isRead = player.mainRole.Mails[i].isRead;
                mail.isExistItem = player.mainRole.Mails[i].isExistItem;
                mail.gold = player.mainRole.Mails[i].gold;
                Console.WriteLine("金币：" + mail.gold);
                List<S2C_AddItemInfo> items = new List<S2C_AddItemInfo>();
                List<S2C_AddEquipInfo> equips = new List<S2C_AddEquipInfo>();

                if (player.mainRole.Mails[i].items.Count != 0)
                {
                    foreach (ItemInfo item in player.mainRole.Mails[i].items)
                    {
                        items.Add(new S2C_AddItemInfo(item.Guid, item.ItemId, (int)item.ItemType,
                            item.Inventory, item.Count));
                    }
                }
                if (player.mainRole.Mails[i].equips.Count != 0)
                {
                    foreach (EquipInfo equip in player.mainRole.Mails[i].equips)
                    {
                        equips.Add(new S2C_AddEquipInfo(equip.Guid, equip.ItemId, (int)equip.ItemType,
                            equip.Inventory, equip.gems));
                    }
                }

                //player.mainRole.AddGold(mail.gold);

                mail.items = items;
                mail.equips = equips;

                S2C_receiveMails.Add(mail);

            }

            ServerSys.Instance.Send(player, "CallLuaFunction", "MailCtrl.UpdateMails", new S2C_ReceiveMails(
                S2C_receiveMails));

        }

        /// <summary>
        /// 系统发送邮件的方法
        /// </summary>
        /// <param name="receiveName"></param>
        /// <param name="addresserName"></param>
        /// <param name="isExistItem"></param>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <param name="items"></param>
        /// <param name="equips"></param>
        /// <param name="gold"></param>
        public void SendMail(string receiveName,string addresserName, bool isExistItem, string title, 
            string content,List<ItemInfo> items,List<EquipInfo> equips,int gold)
        {
            Player target = null;
            bool isOnline = false;
            foreach (Player p in ServerSys.Instance.Players.Values)
            {
                if (p.mainRole.Name == receiveName)
                {
                    target = p;
                    isOnline = true;
                    break;
                }
            }
            if (DBSys.Instance.GetData<MainRoleData>("name",receiveName) == null && isOnline == false)
            {
                return;
            }

            Mail mail = new Mail();
            mail.addresserName = addresserName;
            mail.Guid = Guid.NewGuid().ToString();
            mail.isExistItem = isExistItem;
            mail.isRead = false;
            mail.title = title;
            mail.content = content;
            mail.items = items;
            mail.equips = equips;
            mail.gold = gold;

            switch (isOnline)
            {
                case true:
                    target.mainRole.Mails.Add(mail);
                    ServerSys.Instance.Send(target, "CallLuaFunction", "MainUICtrl.ReceiveNewMail");
                    break;
                case false:
                    string account = DBSys.Instance.GetData<MainRoleData>("name",receiveName).account;
                    DBSys.Instance.InsertData<MailData>(new MailData(mail.Guid, account, mail.addresserName,
                        mail.title, mail.content, mail.isRead, mail.isExistItem, mail.items, mail.equips,
                        mail.gold));
                    break;
            }
        }

        /// <summary>
        /// 玩家之间发送邮件的方法
        /// </summary>
        /// <param name="player"></param>
        /// <param name="C2S_sendMail"></param>
        public void SendMail(Player player, C2S_SendMail C2S_sendMail)
        {
            Console.WriteLine("发送邮件");
            if (player.mainRole.Gold < C2S_sendMail.gold)
                return;
            Player target = null;
            bool isOnline = false;
            foreach (Player p in ServerSys.Instance.Players.Values)
            {
                if (p.mainRole.Name == C2S_sendMail.receiveName)
                {
                    target = p;
                    isOnline = true;
                    break;
                }
            }
            if (DBSys.Instance.GetData<MainRoleData>("name", C2S_sendMail.receiveName) == null && isOnline == false)
            {
                return;
            }

            Console.WriteLine("该玩家是否在线：" + isOnline);

            List<ItemInfo> items = new List<ItemInfo>();
            List<EquipInfo> equips = new List<EquipInfo>();
            bool isExistItem = false;
            if (C2S_sendMail.itemsGuid.Count != 0)
            {
                isExistItem = true;
                for (int i = 0; i < C2S_sendMail.itemsGuid.Count; i++)
                {
                    if (player.mainRole.Inventory.GetItem(C2S_sendMail.itemsGuid[i]) == null)
                    {
                        return;
                    }
                    ItemBase item = player.mainRole.Inventory.GetItem(C2S_sendMail.itemsGuid[i]);
                    switch (item.ItemType)
                    {
                        case ItemEunm.Equip:
                            equips.Add(item as EquipInfo);
                            break;
                        case ItemEunm.Gem:
                            items.Add(item as ItemInfo);
                            break;
                        case ItemEunm.Item:
                            items.Add(item as ItemInfo);
                            break;
                    }
                }
            }

            Mail mail = new Mail();
            mail.addresserName = player.mainRole.Name;
            mail.Guid = Guid.NewGuid().ToString();
            mail.isExistItem = isExistItem;
            mail.isRead = false;
            mail.title = C2S_sendMail.title;
            mail.content = C2S_sendMail.content;
            mail.items = items;
            mail.equips = equips;
            mail.gold = C2S_sendMail.gold;

            switch (isOnline)
            {
                case true:
                    target.mainRole.Mails.Add(mail);
                    ServerSys.Instance.Send(target, "CallLuaFunction", "MainUICtrl.ReceiveNewMail");
                    break;
                case false:
                    string account = DBSys.Instance.GetData<MainRoleData>("name", C2S_sendMail.receiveName).account;
                    DBSys.Instance.InsertData<MailData>(new MailData(mail.Guid, account, mail.addresserName,
                        mail.title, mail.content, mail.isRead, mail.isExistItem, mail.items, mail.equips,
                        mail.gold));
                    break;
            }
        }

        /// <summary>
        /// 保存邮件信息
        /// </summary>
        /// <param name="player"></param>
        public void SaveMail(Player player)
        {
            DBSys.Instance.DeleteAllDatas<MailData>("account", player.account);
            for(int i = 0; i < player.mainRole.Mails.Count; i++)
            {
                Mail mail = player.mainRole.Mails[i];
                MailData mailData = new MailData(mail.Guid, player.account, mail.addresserName, mail.title,
                    mail.content, mail.isRead, mail.isExistItem, mail.items, mail.equips, mail.gold);
            }
        }
    }

    public class Mail
    {
        public string Guid;
        public string addresserName;
        public string title;
        public string content;
        public bool isRead;
        public bool isExistItem;
        public List<ItemInfo> items;
        public List<EquipInfo> equips;
        public int gold;
    }
}
