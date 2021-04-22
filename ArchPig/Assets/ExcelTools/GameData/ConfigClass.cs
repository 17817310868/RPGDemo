using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime;
using System;

[Serializable]
public class ConfigClass
{
    public int id;
}

[Serializable]
public class ProfessionConfig:ConfigClass 
{
    public string name;
    public string model;
    public string headIcon;
    public string professionIconFalse;
    public string professionIconTrue;
    public int firstMenpai;
    public int secondMenpai;
    public string weapon;
    public int level;
    public float hp;
    public float mp;
    public float physicalAttack;
    public float physicalDefense;
    public float magicAttack;
    public float magicDefense;
    public float speed;
    public float experience;
    public float moveSpeed;
    public float positionX;
    public float positionY;
    public float positionZ;
    public float poisoning;
    public float poisoningResist;
    public float burn;
    public float burnResist;
    public float continueHit;
    public float strikeBack;
    public int silver;
    public int gold;
    public int yuanBao;

}

[Serializable]
public class SchoolConfig:ConfigClass
{
    public string name;
    public string schoolIconFalse;
    public string schoolIconTrue;
    public string schoolInfo;
}

[Serializable]
public class EquipConfig : ConfigClass
{
    public int _class;
    public int _type;
    public string name;
    public int level;
    public int profession;
    public string icon;
    public int suit;
    public string model;
    public float buyPrice;
    public float sellPrice;
    public string priceType;
    public int score;
    public int quality;
    public string typeInfo;
    public int firstAttr;
    public int firstAttrValue;
    public int secondAttr;
    public int secondAttrValue;
    public int thirdAttr;
    public int thirdAttrValue;
    public int advanceGold;
}

[Serializable]
public class SkillConfig : ConfigClass
{
    public string name;
    public string icon;
    public string effect;
    public int count;
    public int skillDistanceType;
    public int skillLevel;
    public int CD;
    public int consume;
    public string introduce;
    public int upgradeLevel;
    public int upgradeMoney;
    public string upgradeInfo;
    public float skillTime;
    public string skillAnim;
    public float skillStart;
    public float skillEnd;
    public string effectName;
    public float effectStart;
    public float effectEnd;
    public string hurtAnim;
    public float hurtStart;
    public float hurtEnd;
    public string skillAudio;
    public float audioTime;
    public int effectType;
}

[Serializable]
public class BuffConfig : ConfigClass
{
    public string name;
    public string info;
    public string icon;
    public int type;
    public int rounds;
}

[Serializable]
public class NPCConfig : ConfigClass
{
    public string name;
    public string model;
    public int type;
    public string panelName;
    public string headIcon;
    public float posX;
    public float posY;
    public float posZ;
}

[Serializable]
public class MonsterConfig : ConfigClass
{
    public string name;
    public string model;
    public string scene;
    public int level;
    public int skill;
    public int hp;
    public int mp;
    public int physicalAttack;
    public int physicalDefense;
    public int magicAttack;
    public int magicDefense;
    public int speed;
    public int experience;
    public float poisonint;
    public float poisoningResist;
    public float burn;
    public float burnResist;
    public float continueHit;
    public float strikeBack;
    public int silver;
    public int gold;
    public int yuanBao;
    public string item;
    public string itemChance;
    public string equip;
    public string equipChance;
    public string gem;
    public string gemChance;
}