/*
 * ===================================================================
 * 
 *          projectName:
 *              项目名称:拱猪
 *                  
 *          title:
 *              标题:数据信息模型
 *          
 *          description:
 *              功能描述:设计数据信息模型
 *              
 *          author:
 *              作者:
 * 
 * ===================================================================
 */

using MongoDB.Bson;
using Net;
using Server.GameSys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Tilemaps;

namespace Server
{
    public interface Data
    {
    }
    public class PlayerData :Data
    {
        public PlayerData(string account,string password,string sceneId)
        {
            this.account = account;
            this.password = password;
            this.sceneId = sceneId;
        }
        public ObjectId id;
        public string sceneId;
        public string account;
        public string password;
    }

    public class MainRoleData : RoleBaseData
    {

        public MainRoleData(string Guid,string account,string name,int model,int level, int hp,
            int maxHp, int mp, int maxMp, int physicalAttack, int physicalDefense,
            int magicAttack, int magicDefense, int speed, int experience,float poisoning,
            float poisoningResist, float burn, float burnResist, float continueHit,
            float strikeBack, int professionId,int schoolId, int moveSpeed,float positionX,
            float positionY,float positionZ,int silver,int gold,int yuanBao,int power):base(Guid,account,
                name,model,level,hp,maxHp,mp,maxMp,physicalAttack,physicalDefense,magicAttack,
                magicDefense,speed,experience,poisoning,poisoningResist,burn,burnResist,
                continueHit,strikeBack)
        {
            this.level = level;
            this.professionId = professionId;
            this.schoolId = schoolId;
            this.moveSpeed = moveSpeed;
            this.positionX = positionX;
            this.positionY = positionY;
            this.positionZ = positionZ;
            this.silver = silver;
            this.gold = gold;
            this.yuanBao = yuanBao;
            this.power = power;
        }

        public int professionId;  //职业id
        public int schoolId;  //门派id
        public int moveSpeed;  //移动速度
        public float positionX;  //坐标的x
        public float positionY;  //坐标的y
        public float positionZ;  //坐标的z
        public int silver;  //银币
        public int gold;  //金币
        public int yuanBao;  //元宝
        public int power;
    }

    public class PetRoleData : RoleBaseData
    {
        public PetRoleData(string Guid,string account,string name,int model,int level, int hp,
            int maxHp, int mp, int maxMp, int physicalAttack, int physicalDefense,
            int magicAttack, int magicDefense, int speed, int experience,float poisoning, 
            float poisoningResist, float burn, float burnResist,float continueHit,
            float strikeBack) : base(Guid,account, name,model, level, hp,maxHp,mp, maxMp,
                physicalAttack, physicalDefense, magicAttack, magicDefense, speed,experience,
                poisoning, poisoningResist, burn, burnResist,continueHit, strikeBack)
        {

        }
    }

    public class PartnerRoleData : RoleBaseData
    {
        public PartnerRoleData(string Guid,string account, string name,int model, int level, int hp,
            int maxHp, int mp, int maxMp, int physicalAttack, int physicalDefense,
            int magicAttack, int magicDefense, int speed, int experience,float poisoning,
            float poisoningResist, float burn, float burnResist, float continueHit,
            float strikeBack) : base(Guid,account, name,model, level, hp,maxHp, mp, maxMp,
                physicalAttack, physicalDefense, magicAttack,magicDefense, speed, experience,
                poisoning, poisoningResist, burn, burnResist, continueHit, strikeBack)
        {

        }
    }

    public class RoleBaseData : Data
    {
        public RoleBaseData(string Guid,string account,string name,int profession, int level, int hp,
            int maxHp, int mp, int maxMp, int physicalAttack, int physicalDefense,
            int magicAttack, int magicDefense, int speed, int experience,float poisoning,
            float poisoningResist,float burn,float burnResist,float continueHit,float strikeBack)
        {
            this.Guid = Guid;
            this.account = account;
            this.name = name;
            this.profession = profession;
            this.level = level;
            this.hp = hp;
            this.maxHp = maxHp;
            this.mp = mp;
            this.maxMp = maxMp;
            this.physicalAttack = physicalAttack;
            this.physicalDefense = physicalDefense;
            this.magicAttack = magicAttack;
            this.magicDefense = magicDefense;
            this.speed = speed;
            this.experience = experience;
            this.poisoning = poisoning;
            this.poisoningResist = poisoningResist;
            this.burn = burn;
            this.burnResist = burnResist;
            this.continueHit = continueHit;
            this.strikeBack = strikeBack;
        }
        public ObjectId id;
        public string Guid;
        public string account;  //账号
        public string name;  //角色名称
        public int profession;  //职业类型(模型id)
        public int level;  //等级
        public int hp;  //气血
        public int maxHp;  //气血上限
        public int mp;  //元气
        public int maxMp;  //元气上限
        public int physicalAttack;  //物理攻击
        public int physicalDefense;  //物理防御 
        public int magicAttack;  //元气攻击
        public int magicDefense;  //元气防御
        public int speed;  //速度
        public int experience;  //经验
        public float poisoning;  //中毒命中率
        public float poisoningResist;  //中毒抗性
        public float burn;  //烧伤命中率
        public float burnResist;  //烧伤抗性
        public float continueHit;  //连击率
        public float strikeBack;  //反击率
    }

    public class ItemBaseData : Data
    {
        public ItemBaseData(string Guid,string account,int itemType,int itemId,int inventory)
        {
            this.Guid = Guid;
            this.account = account;
            this.itemType = itemType;
            this.itemId = itemId;
            this.inventory = inventory;
        }
        public ObjectId id;
        public string Guid;
        public string account;
        public int itemType;
        public int itemId;
        public int inventory;
    }

    public class ItemInfoData : ItemBaseData
    {
        public ItemInfoData(string Guid,string account,int itemType,int itemId,int inventory,int count) : 
            base(Guid,account, itemType, itemId,inventory)
        {
            this.count = count;
        }
        public int count;
    }

    public class EquipInfoData : ItemBaseData
    {
        public EquipInfoData(string Guid,string account,int itemType,int itemId,int inventory, int[] gems)
            : base(Guid,account,itemType,itemId,inventory)
        {
            this.gems = gems;
        }
        public int[] gems;
    }

    public class SkillData : Data
    {
        public SkillData(string account,int[] skills)
        {
            this.account = account;
            this.skills = skills;
        }
        public ObjectId id;
        public string account;
        public int[] skills;
    }

    public class TaskInfoData : Data
    {
        public TaskInfoData(string account,List<int> completeTasks,Dictionary<int,int> conductTasks)
        {
            this.account = account;
            this.completeTasks = completeTasks;
            this.conductTasks = conductTasks;
        }
        public ObjectId id;
        public string account;
        public List<int> completeTasks;  //用于存储已完成的任务的id
        public Dictionary<int, int> conductTasks;  //用于存储正在进行的任务所对应的任务进度
    }

    public class MailData : Data
    {
        public MailData(string Guid,string account,string addresserName,string title,string content,
            bool isRead,bool isExistItem,List<ItemInfo> items,List<EquipInfo> equips,int gold)
        {
            this.Guid = Guid;
            this.account = account;
            this.addresserName = addresserName;
            this.title = title;
            this.content = content;
            this.isRead = isRead;
            this.isExistItem = isExistItem;
            this.items = items;
            this.equips = equips;
            this.gold = gold;
        }
        public ObjectId id;
        public string Guid;
        public string account;
        public string addresserName;
        public string title;
        public string content;
        public bool isRead;
        public bool isExistItem;
        public List<ItemInfo> items;
        public List<EquipInfo> equips;
        public int gold;
    }

    public class LotsData : Data
    {
        public LotsData(string Guid,string account,int type,ItemBase itemInfo,int auctionPrice,string bidderGuid,
            string bidderName,int fixedPrice,int remainTime)
        {
            this.Guid = Guid;
            this.account = account;
            this.type = type;
            this.itemInfo = itemInfo;
            this.auctionPrice = auctionPrice;
            this.bidderGuid = bidderGuid;
            this.bidderName = bidderName;
            this.fixedPrice = fixedPrice;
            this.remainTime = remainTime;
        }
        public ObjectId id;
        public string account;
        public string Guid;  //拍卖品Guid
        public int type;
        public ItemBase itemInfo;  //物品信息
        public int auctionPrice;  //竞拍价金币
        public string bidderGuid;  //竞拍者Guid
        public string bidderName;  //竞拍者名称
        public int fixedPrice;  //一口价金币
        public int remainTime;  //剩余时长（一小时为单位）
    }
}
