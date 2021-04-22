/*
 * ===================================================================
 * 
 *          projectName:
 *              项目名称:拱猪
 *                  
 *          title:
 *              标题:客户端与服务器通讯消息模型
 *          
 *          description:
 *              功能描述:设计客户端与服务器通讯的消息体
 *              
 *          author:
 *              作者:
 * 
 * ===================================================================
 */

using Net.Share;
using Server.Buff;
using Server.GameSys;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Principal;
using UnityEngine;

public interface MessageModel
{

}
public class S2C_LoginOrRegisterCallback : MessageModel
{
    public bool result;
    public string message;

    public S2C_LoginOrRegisterCallback(bool result, string message)
    {
        this.result = result;
        this.message = message;
    }
}

public class C2S_CreateMainRole : MessageModel
{
    public C2S_CreateMainRole(string name, int professionId, int schoolId)
    {
        this.name = name;
        this.professionId = professionId;
        this.schoolId = schoolId;
    }
    public string name;
    public int professionId;
    public int schoolId;
}

public class S2C_OtherRoleInfo : MessageModel
{
    public S2C_OtherRoleInfo(string Guid,int roleType,int professionId,int schoolId, string name,
        bool isLeader, bool isBattle,int level, int moveSpeed,int power,int weaponId, Vector3 position)
    {
        this.Guid = Guid;
        this.roleType = roleType;
        this.professionId = professionId;
        this.schoolId = schoolId;
        this.name = name;
        this.isLeader = isLeader;
        this.isBattle = isBattle;
        this.level = level;
        this.moveSpeed = moveSpeed;
        this.power = power;
        this.weaponId = weaponId;
        this.position = position;
    }
    public int roleType;
    public int professionId;
    public int schoolId;
    public string Guid;
    public string name;
    public bool isLeader;
    public bool isBattle;
    public int level;
    public int moveSpeed;
    public int power;
    public int weaponId;
    public Vector3 position;
}

public class S2C_MainRoleInfo : S2C_OtherRoleInfo
{
    public S2C_MainRoleInfo(string Guid,int roleType,int profeesionId,int schoolId, string name,
        bool isLeader, bool isBattle, int level, int moveSpeed,int power,int weaponId, Vector3 position,
        int hp,int maxHp, int mp, int maxMp, int physicalAttack, int physicalDefense,
        int magicAttack, int magicDefense, int speed, int experience,int silver,
        int gold,int yuanBao) : base(Guid,roleType, profeesionId, schoolId, name, isLeader, 
            isBattle, level, moveSpeed,power,weaponId, position)
    {
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
        this.silver = silver;
        this.gold = gold;
        this.yuanBao = yuanBao;
    }
    public int hp;
    public int maxHp;
    public int mp;
    public int maxMp;
    public int physicalAttack;
    public int physicalDefense;
    public int magicAttack;
    public int magicDefense;
    public int speed;
    public int experience;
    public int silver;
    public int gold;
    public int yuanBao;
}

public class S2C_SceneInfo : MessageModel
{
    public S2C_SceneInfo(string sceneId)
    {
        this.sceneId = sceneId;
    }
    public string sceneId;
}

public class S2C_MoveInfo: MessageModel
{
    public S2C_MoveInfo(string Guid,float speed,Vector3 target)
    {
        this.Guid = Guid;
        this.speed = speed;
        this.target = target;
    }
    public string Guid;
    public float speed;
    public Vector3 target;
}

public class C2S_MoveInfo: MessageModel
{
    public C2S_MoveInfo(Vector3 target)
    {
        this.target = target;
    }
    public Vector3 target;
}

public class S2C_FollowInfo : MessageModel
{
    public S2C_FollowInfo(string Guid, float speed, string targetId)
    {
        this.Guid = Guid;
        this.speed = speed;
        this.targetId = targetId;
    }
    public string Guid;
    public float speed;
    public string targetId;
}

public class S2C_AddItemsInfo : MessageModel
{
    public S2C_AddItemsInfo(List<S2C_AddItemInfo> items)
    {
        this.items = items;
    }
    public List<S2C_AddItemInfo> items;

}

public class S2C_AddItemInfo : MessageModel
{
    public S2C_AddItemInfo(string Guid,int itemId,int itemType,int inventory,int count)
    {
        this.Guid = Guid;
        this.itemId = itemId;
        this.itemType = itemType;
        this.inventory = inventory;
        this.count = count;
    }
    public string Guid;
    public int itemId;
    public int itemType;
    public int inventory;
    public int count;
}

public class S2C_AddEquipInfo : MessageModel
{
    public S2C_AddEquipInfo(string Guid,int itemId,int itemType,int inventory,int[] gems)
    {
        this.Guid = Guid;
        this.itemId = itemId;
        this.itemType = itemType;
        this.inventory = inventory;
        this.gems = gems;
    }
    public string Guid;
    public int itemId;
    public int itemType;
    public int inventory;
    public int[] gems;
}

public class S2C_AddEquipsInfo : MessageModel
{
    public S2C_AddEquipsInfo(List<S2C_AddEquipInfo> items)
    {
        this.items = items;
    }
    public List<S2C_AddEquipInfo> items;

}

public class C2S_DressEquip : MessageModel
{
    public C2S_DressEquip(string Guid)
    {
        this.Guid = Guid;
    }
    public string Guid;
}

public class C2S_MakeEquip : MessageModel
{
    public C2S_MakeEquip(string Guid)
    {
        this.Guid = Guid;
    }
    public string Guid;
}

public class C2S_InlayGem : MessageModel
{
    public C2S_InlayGem(string equipGuid, int hole, string gemGuid)
    {
        this.equipGuid = equipGuid;
        this.hole = hole;
        this.gemGuid = gemGuid;
    }
    public string equipGuid;
    public int hole;
    public string gemGuid;
}

public class C2S_RemoveGem : MessageModel
{
    public C2S_RemoveGem(string equipGuid, int hole)
    {
        this.equipGuid = equipGuid;
        this.hole = hole;
    }
    public string equipGuid;
    public int hole;
}

public class C2S_TakeoffEquip : MessageModel
{
    public C2S_TakeoffEquip(string Guid)
    {
        this.Guid = Guid;
    }
    public string Guid;
}

public class C2S_AdvanceEquip : MessageModel
{
    public C2S_AdvanceEquip(string Guid)
    {
        this.Guid = Guid;
    }

    public string Guid;
}

public class C2S_DiscardItem : MessageModel
{
    public C2S_DiscardItem(string Guid)
    {
        this.Guid = Guid;
    }
    public string Guid;
}

public class C2S_UseItem : MessageModel
{
    public C2S_UseItem(string Guid)
    {
        this.Guid = Guid;
    }
    public string Guid;
}

public class S2C_ReduceItem : MessageModel
{
    public S2C_ReduceItem(string Guid, int effect)
    {
        this.Guid = Guid;
        this.effect = effect;
    }
    public string Guid;
    public int effect;
}

public class S2C_ReduceItems : MessageModel
{
    public S2C_ReduceItems(List<S2C_ReduceItem> items)
    {
        this.items = items;
    }
    public List<S2C_ReduceItem> items;
}

public class S2C_RemoveItemInfo : MessageModel
{
    public S2C_RemoveItemInfo(string Guid)
    {
        this.Guid = Guid;
    }
    public string Guid;
}

public class S2C_UpdateItemInfo: MessageModel
{
    public S2C_UpdateItemInfo(string Guid,int count)
    {
        this.Guid = Guid;
        this.count = count;
    }
    public string Guid;
    public int count;
}

public class S2C_AddSkill : MessageModel
{
    public S2C_AddSkill(int skillId)
    {
        this.skillId = skillId;
    }
    public int skillId;
}

public class S2C_AddSkills : MessageModel
{
    public S2C_AddSkills(List<S2C_AddSkill> skills)
    {
        this.skills = skills;
    }
    public List<S2C_AddSkill> skills;
}

public class S2C_RemoveSkill : MessageModel
{
    public S2C_RemoveSkill(int skillId)
    {
        this.skillId = skillId;
    }
    public int skillId;
}

public class S2C_UpdateSkill : MessageModel
{
    public S2C_UpdateSkill(int skillId)
    {
        this.skillId = skillId;
    }
    public int skillId;
}

public class C2S_LearnSkill : MessageModel
{
    public C2S_LearnSkill(int skillId)
    {
        this.skillId = skillId;
    }
    public int skillId;
}

public class C2S_UpgradeSkill : MessageModel
{
    public C2S_UpgradeSkill(int skillId)
    {
        this.skillId = skillId;
    }
    public int skillId;
}

public class C2S_BattleCommand : MessageModel
{
    public C2S_BattleCommand(byte actorIndex,byte victim,byte actorType,int paramId)
    {
        this.actorIndex = actorIndex;
        this.victim = victim;
        this.actorType = actorType;
        this.paramId = paramId;
    }

    public byte actorIndex;  //行动者站位索引
    public byte victim;  //受害者站位索引
    public byte actorType;  //行动类型
    public int paramId;  //参数索引,根据行动类型而言(如果是技能类型，则为技能id，使用物品则为物品id)
}

public class S2C_BattleMessage : MessageModel
{
    public S2C_BattleMessage()
    {
        staticBuffs = new Dictionary<byte, int>();
        actionBuffs = new Dictionary<byte, int>();
        actionBuffsEffect = new Dictionary<int, float>();
        targetsHpEffect = new Dictionary<byte, float>();
        otherMessages = new List<S2C_BattleMessage>();
    }
    public byte actorIndex;  //行动者站位索引
    public byte[] victims;  //受害者站位索引
    public Dictionary<byte, int> staticBuffs;
    public Dictionary<byte, int> actionBuffs;
    public Dictionary<int, float> actionBuffsEffect;
    public byte actorType = 0;  //行动类型
    public int paramId;  //参数索引,根据行动类型而言(如果是技能类型，则为技能id，使用物品则为物品id)
    public Dictionary<byte, float> targetsHpEffect;
    public List<S2C_BattleMessage> otherMessages;  //反击连击
}



public class S2C_BattleSettle : MessageModel
{
    public byte winTeam;

}

public class S2C_BattlerInfo : MessageModel
{
    public byte battlerType;  //参战者类型
    public int monsterId;  //怪物id
    public byte positionIndex;  //站位索引
    public string roleName;  //角色名称
    public int profession;  //职业类型(模型id)
    public int weapon;  //武器id
    public float hp;  //当前气血
    public float maxHp;  //气血上限
    public float mp;  //当前元气值
    public float maxMp;  //元气值上限
}

public class S2C_InitBattlers: MessageModel
{
    public Dictionary<string, byte> rolesDic;  //存储玩家guid对应的站位索引
    public List<S2C_BattlerInfo> battlersInfo;  //存储参战的所有客户端需要用于显示的信息
}

public class C2S_JoinTeamRequest : MessageModel
{
    public C2S_JoinTeamRequest(string playerId)
    {
        this.playerId = playerId;
    }
    public string playerId;  //申请加入队伍的某一成员id
}

public class S2C_JoinTeamRequest : MessageModel
{
    public S2C_JoinTeamRequest(string playerId)
    {
        this.playerId = playerId;
    }
    public string playerId;  //申请加入得玩家id
}

public class C2S_JoinTeamReply : MessageModel
{
    public C2S_JoinTeamReply(string playerId, bool result)
    {
        this.playerId = playerId;
        this.result = result;
    }
    public string playerId;  //申请加入得玩家id
    public bool result;
}

public class S2C_JoinTeamReply : MessageModel
{
    public S2C_JoinTeamReply(string playerId, bool result)
    {
        this.playerId = playerId;
        this.result = result;
    }
    public string playerId;  //申请加入队伍的队长Id
    public bool result;
}

public class C2S_VisiteRequest : MessageModel
{
    public C2S_VisiteRequest(string playerId)
    {
        this.playerId = playerId;
    }
    public string playerId;  //被邀请的目标id

}

public class S2C_VisiteRequest : MessageModel
{
    public S2C_VisiteRequest(string playerId)
    {
        this.playerId = playerId;
    }
    public string playerId;  //发出邀请的目标id
}

public class C2S_VisiteReply : MessageModel
{
    public C2S_VisiteReply(string playerId, bool result)
    {
        this.playerId = playerId;
        this.result = result;
    }
    public string playerId; //发出邀请的目标id
    public bool result; //结果
}

public class S2C_VisiteReply : MessageModel
{
    public S2C_VisiteReply(string playerId, bool result)
    {
        this.playerId = playerId;
        this.result = result;
    }
    public string playerId;  //被邀请的目标id
    public bool result;  //结果
}

public class C2S_BattleRequest: MessageModel
{
    public C2S_BattleRequest(string playerId)
    {
        this.playerId = playerId;
    }
    public string playerId;  //切磋得目标id
}

public class S2C_BattleRequest: MessageModel
{
    public string playerId;  //发起切磋请求的玩家id
}

public class C2S_BattleReply: MessageModel
{
    public C2S_BattleReply(string playerId,bool reply)
    {
        this.playerId = playerId;
        this.reply = reply;
    }
    public string playerId;  //发起切磋请求的玩家id(等待回复的玩家)
    public bool reply;  //是否同意切磋
}

public class S2C_BattleReply: MessageModel
{
    public string playerId;  //切磋的目标玩家id
    public bool reply;  //是否同意切磋
}

public class S2C_Round : MessageModel
{
    public S2C_Round(byte round)
    {
        this.round = round;
    }
    public byte round;  //第几回合
}

public class S2C_UpdateLeader : MessageModel
{
    public S2C_UpdateLeader(string Guid, bool result)
    {
        this.Guid = Guid;
        this.result = result;
    }
    public string Guid;
    public bool result;
}

public class S2C_UpdateBattles : MessageModel
{
    public S2C_UpdateBattles(List<S2C_UpdateBattle> updateBattles)
    {
        this.updateBattles = updateBattles;
    }
    public List<S2C_UpdateBattle> updateBattles;
}

public class S2C_UpdateBattle : MessageModel
{
    public S2C_UpdateBattle(string Guid, bool result)
    {
        this.Guid = Guid;
        this.result = result;
    }
    public string Guid;
    public bool result;
}

public class S2C_TeamInfo : MessageModel
{
    public S2C_TeamInfo(List<string> playersGuid,bool isLeader)
    {
        this.playersGuid = playersGuid;
        this.isLeader = isLeader;
    }
    public List<string> playersGuid;
    public bool isLeader;
}

public class S2C_SignInfo : MessageModel
{
    public S2C_SignInfo(byte number,bool isCanSign)
    {
        this.number = number;
        this.isCanSign = isCanSign;
    }
    public byte number;
    public bool isCanSign;
}

public class S2C_AcceptableTasks : MessageModel
{
    public S2C_AcceptableTasks(List<int> acceptableTasks)
    {
        this.acceptableTasks = acceptableTasks;
    }
    public List<int> acceptableTasks;
}

public class S2C_ConductTasks : MessageModel
{
    public S2C_ConductTasks(Dictionary<int,int> conductTasks)
    {
        this.conductTasks = conductTasks;
    }
    public Dictionary<int, int> conductTasks;
}

public class S2C_AllTaskInfo : MessageModel
{
    public S2C_AllTaskInfo(Dictionary<int,int> conductTasks,List<int> acceptableTasks)
    {
        this.conductTasks = conductTasks;
        this.acceptableTasks = acceptableTasks;
    }
    public Dictionary<int, int> conductTasks;
    public List<int> acceptableTasks;
}

public class S2C_TaskProgress : MessageModel
{
    public S2C_TaskProgress(int taskId,int progress)
    {
        this.taskId = taskId;
        this.progress = progress;
    }
    public int taskId;
    public int progress;
}

public class S2C_AcceptTask : MessageModel
{
    public S2C_AcceptTask(int taskId,int progress)
    {
        this.taskId = taskId;
        this.progress = progress;
    }
    public int taskId;
    public int progress;
}

public class S2C_CompleteTask : MessageModel
{
    public S2C_CompleteTask(int taskId)
    {
        this.taskId = taskId;
    }
    public int taskId;
}

public class C2S_AcceptTask : MessageModel
{
    public C2S_AcceptTask(int taskId)
    {
        this.taskId = taskId;
    }
    public int taskId;
}

public class C2S_CompleteTask : MessageModel
{
    public C2S_CompleteTask(int taskId)
    {
        this.taskId = taskId;
    }
    public int taskId;
}


public class S2C_RemoveItems : MessageModel
{
    public S2C_RemoveItems(List<S2C_RemoveItemInfo> Guids)
    {
        this.Guids = Guids;
    }
    public List<S2C_RemoveItemInfo> Guids;
}

public class C2S_BuyItem : MessageModel
{
    public C2S_BuyItem(int itemId)
    {
        this.itemId = itemId;
    }
    public int itemId;
}

public class C2S_CheckRank : MessageModel
{
    public C2S_CheckRank(int rankType)
    {
        this.rankType = rankType;
    }
    public int rankType;
}

public class S2C_RoleLevelRank : MessageModel
{
    public S2C_RoleLevelRank(List<RoleLevelRank> ranks)
    {
        this.ranks = ranks;
    }
    public List<RoleLevelRank> ranks;

}

public class S2C_RolePowerRank : MessageModel
{
    public S2C_RolePowerRank(List<RolePowerRank> ranks)
    {
        this.ranks = ranks;
    }
    public List<RolePowerRank> ranks;
}

public class C2S_SendMail : MessageModel
{
    public string receiveName;
    public string title;
    public string content;
    public List<string> itemsGuid;
    public int gold;
}

public class S2C_ReceiveMail : MessageModel
{
    public string Guid;
    public string addresserName;
    public string title;
    public string content;
    public bool isRead;
    public bool isExistItem;
    public List<S2C_AddItemInfo> items;
    public List<S2C_AddEquipInfo> equips;
    public int gold;
}

public class S2C_ReceiveMails : MessageModel
{
    public S2C_ReceiveMails(List<S2C_ReceiveMail> mails)
    {
        this.mails = mails;
    }
    public List<S2C_ReceiveMail> mails;
}

public class C2S_GetMailItems : MessageModel
{
    public C2S_GetMailItems(string Guid)
    {
        this.Guid = Guid;
    }
    public string Guid;
}

public class C2S_ReadMail : MessageModel
{
    public C2S_ReadMail(string Guid)
    {
        this.Guid = Guid;
    }
    public string Guid;
}

public class C2S_Auction: MessageModel
{
    public C2S_Auction(string Guid,int auctionPrice,int fixedPrice,int time)
    {
        this.Guid = Guid;
        this.auctionPrice = auctionPrice;
        this.fixedPrice = fixedPrice;
        this.time = time;
    }
    public string Guid;
    public int auctionPrice;
    public int fixedPrice;
    public int time;
}

public class C2S_Bidding: MessageModel
{
    public C2S_Bidding(string Guid,byte itemType,int biddingPrice)
    {
        this.Guid = Guid;
        this.itemType = itemType;
        this.biddingPrice = biddingPrice;
    }
    public string Guid;
    public byte itemType;
    public int biddingPrice;
}

public class C2S_FixedBuy : MessageModel
{
    public C2S_FixedBuy(string Guid,byte itemType)
    {
        this.Guid = Guid;
        this.itemType = itemType;
    }
    public string Guid;
    public byte itemType;
}

public class C2S_GetLots : MessageModel
{
    public C2S_GetLots(byte itemType)
    {
        this.itemType = itemType;
    }
    public byte itemType;
}

public class C2S_SearchLots : MessageModel
{
    public C2S_SearchLots(byte itemType,string name)
    {
        this.itemType = itemType;
        this.name = name;
    }
    public byte itemType;
    public string name;
}

public class S2C_LotsInfo : MessageModel
{
    public string Guid;  //拍卖品Guid
    public S2C_AddItemInfo itemInfo;
    public S2C_AddEquipInfo equipInfo;
    public int auctionPrice;  //竞拍价银币
    public string bidderGuid;  //竞拍者Guid
    public string bidderName;  //竞拍者名称
    public int fixedPrice;  //一口价银币
    public int remainTime;  //剩余时长（一小时为单位）
}

public class S2C_LotsInfos : MessageModel
{
    public S2C_LotsInfos(byte itemType,List<S2C_LotsInfo> lotsInfos)
    {
        this.itemType = itemType;
        this.lotsInfos = lotsInfos;
    }
    public byte itemType;
    public List<S2C_LotsInfo> lotsInfos;
}

public class C2S_CheckInfo : MessageModel
{
    public C2S_CheckInfo(string Guid)
    {
        this.Guid = Guid;
    }
    public string Guid;
}

public class S2C_CheckInfo : MessageModel
{
    public string Guid;
    public List<S2C_AddEquipInfo> equips;
}

public class S2C_DressEquip : MessageModel
{
    public string Guid;
    public int equipId;
}

public class S2C_TakeoffEquip : MessageModel
{
    public string Guid;
    public int type;
}