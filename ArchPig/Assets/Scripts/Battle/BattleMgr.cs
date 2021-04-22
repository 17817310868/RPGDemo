using GameDesigner;
using LuaFramework;
using LuaInterface;
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using Net.Share;
using UnityEngine.Networking;
using System;

public enum ActorType
{
    None = 0,
    Attack,  //普通攻击
    Skill,  //使用技能
    Item,  //使用道具
    Escape,  //逃跑
}

public enum SkillDistanceType
{
    None = 0,
    NearSkill,  //近程攻击
    FarSkill  //远程攻击
}

public enum BuffType
{
    None = 0,
    Buff,  //增益buff
    Debuff  //减益buff
}

public enum EffectType
{
    None = 0,
    OddNumber = 1,  //单个粒子特效
    ComplexNumber = 2  //多个粒子特效
}

public enum BattlerType
{
    None = 0,
    Player = 1,
    AI = 2
}

public class BattleMgr : Manager
{
    byte clientIndex;  //本玩家站位索引
    //记录角色预制体对应的站位索引
    Dictionary<GameObject, byte> GOsDic;
    Dictionary<byte, Animation> animsDic;
    Dictionary<byte, Battler> battlersDic;
    S2C_BattleSettle battleSettle;
    ObjectPoolManager poolMgr;
    public float time;
    public bool isTiming = false;
    byte teamId;
    bool isBattleEnd = false;
    bool isBattle = false;

    Dictionary<byte, List<Vector3>> teamPos = new Dictionary<byte, List<Vector3>>();
    Dictionary<byte, List<Vector3>> teamAngle = new Dictionary<byte, List<Vector3>>();

    void Awake()
    {
        teamPos.Add(1, new List<Vector3>
        {
            new Vector3(0f,0,95f),
            new Vector3(1.5f,0,95f),
            new Vector3(3f,0,95f),
            new Vector3(4.5f,0,95f),
            new Vector3(6f,0,95f),
            new Vector3(0f,0,96.5f),
            new Vector3(1.5f,0,96.5f),
            new Vector3(3f,0,96.5f),
            new Vector3(4.5f,0,96.5f),
            new Vector3(6f,0,96.5f),
            new Vector3(-3f,0,105f),
            new Vector3(-1.5f,0,105f),
            new Vector3(0f,0,105f),
            new Vector3(1.5f,0,105f),
            new Vector3(3f,0,105f),
            new Vector3(-3f,0,103.5f),
            new Vector3(-1.5f,0,103.5f),
            new Vector3(0f,0,103.5f),
            new Vector3(1.5f,0,103.5f),
            new Vector3(3f,0,103.5f)
        });
        teamPos.Add(2, new List<Vector3>
        {
            new Vector3(-3f,0,105f),
            new Vector3(-1.5f,0,105f),
            new Vector3(0f,0,105f),
            new Vector3(1.5f,0,105f),
            new Vector3(3f,0,105f),
            new Vector3(-3f,0,103.5f),
            new Vector3(-1.5f,0,103.5f),
            new Vector3(0f,0,103.5f),
            new Vector3(1.5f,0,103.5f),
            new Vector3(3f,0,103.5f),
            new Vector3(0f,0,95f),
            new Vector3(1.5f,0,95f),
            new Vector3(3f,0,95f),
            new Vector3(4.5f,0,95f),
            new Vector3(6f,0,95f),
            new Vector3(0f,0,96.5f),
            new Vector3(1.5f,0,96.5f),
            new Vector3(3f,0,96.5f),
            new Vector3(4.5f,0,96.5f),
            new Vector3(6f,0,96.5f)

        });

        teamAngle.Add(1, new List<Vector3>
        {
            new Vector3(0,0,0),
            new Vector3(0,0,0),
            new Vector3(0,0,0),
            new Vector3(0,0,0),
            new Vector3(0,0,0),
            new Vector3(0,0,0),
            new Vector3(0,0,0),
            new Vector3(0,0,0),
            new Vector3(0,0,0),
            new Vector3(0,0,0),
            new Vector3(0, 180, 0),
            new Vector3(0, 180, 0),
            new Vector3(0, 180, 0),
            new Vector3(0, 180, 0),
            new Vector3(0, 180, 0),
            new Vector3(0, 180, 0),
            new Vector3(0, 180, 0),
            new Vector3(0, 180, 0),
            new Vector3(0, 180, 0),
            new Vector3(0, 180, 0)
        });
        teamAngle.Add(2, new List<Vector3>
        {
            new Vector3(0, 180, 0),
            new Vector3(0, 180, 0),
            new Vector3(0, 180, 0),
            new Vector3(0, 180, 0),
            new Vector3(0, 180, 0),
            new Vector3(0, 180, 0),
            new Vector3(0, 180, 0),
            new Vector3(0, 180, 0),
            new Vector3(0, 180, 0),
            new Vector3(0, 180, 0),
            new Vector3(0,0,0),
            new Vector3(0,0,0),
            new Vector3(0,0,0),
            new Vector3(0,0,0),
            new Vector3(0,0,0),
            new Vector3(0,0,0),
            new Vector3(0,0,0),
            new Vector3(0,0,0),
            new Vector3(0,0,0),
            new Vector3(0,0,0)
        });
    }
    // Start is called before the first frame update
    void Start()
    {
        //luaMgr = AppFacade.Instance.GetManager<LuaManager>(ManagerName.Lua);
        poolMgr = AppFacade.Instance.GetManager<ObjectPoolManager>(ManagerName.ObjectPool);
        GOsDic = new Dictionary<GameObject, byte>();
        animsDic = new Dictionary<byte, Animation>();
        battlersDic = new Dictionary<byte, Battler>();
        //LuaManager.Require("Model/UIMgr");
    }

    // Update is called once per frame
    void Update()
    {
        
        if (isTiming == true)
        {
            time -= Time.deltaTime;
            LuaManager.GetFunction("UIMgr.Trigger").LazyCall(LuaManager.GetTable("UIMgr"),"Time",
                (int)time);
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hInfo, 100f, 1 << LayerMask.NameToLayer("Player")))
                {
                    byte index = GOsDic[hInfo.collider.gameObject];
                    LuaManager.GetFunction("UIMgr.Trigger").LazyCall(LuaManager.GetTable("UIMgr"), "Action",
                        clientIndex, index);
                    return;
                }
                else
                {
                    LuaManager.GetFunction("UIMgr.Trigger").LazyCall(LuaManager.GetTable("UIMgr"), "Action",
                        clientIndex, -1);
                    return;
                }
            }

            if (time < 1)
                isTiming = false;
        }

        if(isBattle)
            LuaManager.GetFunction("UIMgr.Trigger").LazyCall(LuaManager.GetTable("UIMgr"),
            "UpdateHeadPosition");


    }

    [Rpc]
    public void InitBattle(S2C_InitBattlers S2C_initBattlers)
    {
        CameraMgr.Instance.ChangeFight();
        RoleCtrl.Instance.isBattle = true;
        isBattle = true;
        LuaManager.GetFunction("BattleCtrl.StartBattle").Call(LuaManager.GetTable("BattleCtrl"));
        Debugger.Log("初始化模型");
        clientIndex = S2C_initBattlers.rolesDic[RoleMgr.Instance.MainRoleId];

        if (clientIndex < 10)
            teamId = 1;
        else
            teamId = 2;

        List<S2C_BattlerInfo> battlersInfo = S2C_initBattlers.battlersInfo;
        foreach (S2C_BattlerInfo battleInfo in battlersInfo)
        {
            string model = string.Empty;
            Debugger.Log((BattlerType)battleInfo.battlerType);
            switch ((BattlerType)battleInfo.battlerType)
            {
                case BattlerType.Player:
                    model = ConfigManager.Instance.GetConfig<ProfessionConfig>(battleInfo.profession).model;
                    break;
                case BattlerType.AI:
                    model = ConfigManager.Instance.GetConfig<MonsterConfig>(battleInfo.monsterId).model;
                    break;
            }
            LuaHelper.GetObjectPoolManager().Get(model, (go) =>
             {
                 byte index = battleInfo.positionIndex;
                 Battler battler = new Battler(index);
                 GOsDic.Add(go, index);
                 animsDic.Add(index, go.transform.GetComponent<Animation>());
                 animsDic[index].Play("jingjie");
                 //Debugger.Log(index);
                 //Debugger.Log(teamPos[teamId][index]);
                 battler.position = teamPos[teamId][index];
                 battler.angle = teamAngle[teamId][index];
                 go.transform.position = battler.position;
                 go.transform.localEulerAngles = battler.angle;
                 battler.hp = battleInfo.hp;
                 battler.maxHp = battleInfo.maxHp;
                 battler.mp = battleInfo.mp;
                 battler.maxMp = battleInfo.maxMp;
                 battlersDic.Add(index, battler);
                 LuaManager.GetFunction("UIMgr.Trigger").LazyCall(LuaManager.GetTable("UIMgr"),
                     "AddDetailHead",battleInfo.positionIndex,Vector2.zero,false,false,
                     battleInfo.roleName,battleInfo.hp,battleInfo.maxHp,battleInfo.mp,
                     battleInfo.maxMp);

                 Actor actor = new Actor(go);
                 if (battleInfo.weapon != -1)
                     actor.UpdateBind(battleInfo.weapon);
             });
        }
    }

    [Rpc]
    public void ShowBattleAnim(List<S2C_BattleMessage> S2C_battleMessages)
    {
        StartCoroutine(PlayAnims(S2C_battleMessages));
    }

    IEnumerator PlayAnims(List<S2C_BattleMessage> battleMessages)
    {
        LuaManager.GetFunction("UIMgr.Trigger").LazyCall(LuaManager.GetTable("UIMgr"),
            "ShowBattleAnim");

        List <S2C_BattleMessage> messages = battleMessages;
        SkillDistanceType distanceType = default;
        while(messages.Count > 0)
        {
            S2C_BattleMessage message = messages[0];

            messages.RemoveAt(0);

            Battler actor = battlersDic[message.actorIndex];

            if(actor.staticBuffs.Count > 0)
            {
                List<int> keys = actor.staticBuffs.Keys.ToList();
                foreach(int buffId in keys)
                {
                    actor.staticBuffs[buffId]--;
                    if (actor.staticBuffs[buffId] == 0)
                        actor.staticBuffs.Remove(buffId);
                }
            }

            if(actor.actionBuffs.Count > 0)
            {
                List<int> keys = actor.actionBuffs.Keys.ToList();
                foreach (int buffId in keys)
                {
                    switch (ConfigManager.Instance.GetConfig<BuffConfig>(buffId).type)
                    {
                        case (int)BuffType.Buff:
                            yield return StartCoroutine(PlayBuff(animsDic[actor.index],actor,
                                actor.actionBuffsEffect[buffId]));
                            break;
                        case (int)BuffType.Debuff:
                            yield return StartCoroutine(PlayDebuff(animsDic[actor.index],actor,
                                actor.actionBuffsEffect[buffId]));
                            break;
                    }
                    actor.actionBuffs[buffId]--;
                    if (actor.actionBuffs[buffId] == 0)
                    {
                        actor.actionBuffs.Remove(buffId);
                        actor.actionBuffsEffect.Remove(buffId);
                    }
                }
            }

            buff[] buffs = GetAllBuffs(actor);

            LuaManager.GetFunction("UIMgr.Trigger").LazyCall(LuaManager.GetTable("UIMgr"),
                        "UpdateBuff",actor.index ,buffs);

            if (message.actorType == (byte)ActorType.None)
                continue;

            distanceType = default;
            switch (message.actorType)
            {
                case (byte)ActorType.Attack:
                    distanceType = SkillDistanceType.NearSkill;
                    break;
                case (byte)ActorType.Skill:
                    distanceType = (SkillDistanceType)ConfigManager.Instance.
                        GetConfig<SkillConfig>(message.paramId).skillDistanceType;
                    break;
                default:
                    break;
            }

            if (distanceType == SkillDistanceType.NearSkill)
                yield return StartCoroutine(PlayMove(animsDic[message.actorIndex],
                    animsDic[message.victims[0]]));

            switch (message.actorType)
            {
                case (byte)ActorType.Attack:
                    break;
                case (byte)ActorType.Skill:
                    SkillConfig config = ConfigManager.Instance.GetConfig<SkillConfig>(message.paramId);
                    battlersDic[message.actorIndex].mp = battlersDic[message.actorIndex].mp - config.consume;
                    yield return StartCoroutine(PlaySkill(message,config.skillTime,config.skillAnim,
                        config.skillStart,config.skillEnd,config.effectName, config.effectStart,
                        config.effectEnd,config.hurtAnim,config.hurtStart,config.hurtEnd,config.skillAudio,
                        config.audioTime,(EffectType)config.effectType));
                    break;
                case (byte)ActorType.Item:
                    yield return StartCoroutine(PlayItem(message));
                    break;
                case (byte)ActorType.Escape:
                    //yield return StartCoroutine(PlayEscape(message));
                    break;
            }

            for (int i = 0; i < message.otherMessages.Count; i++)
            {
                yield return StartCoroutine(PlaySkill(message.otherMessages[i], 1.6f, "att01", 0,
                    1.05f,"attackeffect", 0.6f, 1.5f, "atted", 0.6f, 1.2f, "attack",0.55f,EffectType.ComplexNumber));
            }

            if (distanceType == SkillDistanceType.NearSkill)
                yield return StartCoroutine(PlayBack(message.actorIndex ,animsDic[message.actorIndex]));

            if (battlersDic[message.actorIndex].hp == 0)
                animsDic[message.actorIndex].Play("dead");
            
        }
        Net.Client.NetBehaviour.Send("AddBattleCallback");

        if (isBattleEnd)
        {
            bool isWin;
            if (battleSettle.winTeam == teamId)
                isWin = true;
            else
                isWin = false;
            LuaManager.GetFunction("UIMgr.Trigger").LazyCall(LuaManager.GetTable("UIMgr"),
                "BattleEnd",isWin);

            RoleCtrl.Instance.isBattle = false;

            clientIndex = 0;
            List<GameObject> gos = GOsDic.Keys.ToList();
            for(int i =0;i < gos.Count; i++)
            {
                poolMgr.Set(gos[i].name, gos[i]);
                LuaManager.GetFunction("UIMgr.Trigger").LazyCall(LuaManager.GetTable("UIMgr"),
                     "RemoveHead", GOsDic[gos[i]]);
            }
            GOsDic.Clear();
            animsDic.Clear();
            battlersDic.Clear();
            battleSettle = null;
            isTiming = false;
            teamId = 0;
            isBattleEnd = false;
            isBattle = false;
        }
    }

    IEnumerator PlayBuff(Animation actor,Battler battler,float hurt)
    {
        float time = 0;
        HurtAnim(battler, hurt);
        battler.hp += hurt;
        UpdateInfo(battler);
        while (time < 0.6f)
        {
            time += Time.deltaTime;
            yield return 0;
        }
    }

    IEnumerator PlayDebuff(Animation actor,Battler battler,float hurt)
    {
        float time = 0;
        actor.Play("atted");
        HurtAnim(battler, hurt);
        battler.hp += hurt;
        UpdateInfo(battler);
        while (time < 0.6f)
        {
            time += Time.deltaTime;
            yield return 0;
        }
        actor.Play("jingjie");
    }

    IEnumerator PlayMove(Animation attackAnim,Animation victimAnim)
    {
        attackAnim.Play("run");
        attackAnim.transform.LookAt(victimAnim.transform);
        while (Vector3.Distance(attackAnim.transform.position, victimAnim.transform.position) > 1.5f)
        {
            attackAnim.transform.position += attackAnim.transform.forward * Time.deltaTime * 10;
            yield return 0;
        }
        attackAnim.transform.LookAt(victimAnim.transform);
    }

    IEnumerator PlayBack(byte index, Animation anim)
    {
        Vector3 pos = battlersDic[index].position;
        Vector3 angle = battlersDic[index].angle;
        anim.transform.LookAt(pos);
        anim.Play("run");
        while (Vector3.Distance(anim.transform.position, pos) > 0.2f)
        {
            anim.transform.position += anim.transform.forward * Time.deltaTime * 10;
            yield return 0;
        }
        anim.transform.position = pos;
        anim.transform.localEulerAngles = angle;
        anim.Play("jingjie");
    }

    IEnumerator PlaySkill(S2C_BattleMessage message,float skillTime,string skillAnim,float skillStart,
        float skillEnd,string effectName,float effectStart,float effectEnd,string hurtAnim,
        float hurtStart,float hurtEnd,string skillAudio,float audioTime,EffectType effectType)
    {

        List<Battler> victims = new List<Battler>();

        foreach(byte index in message.victims)
        {
            victims.Add(battlersDic[index]);
        }

        for(int index = 0;index < victims.Count; index++)
        {
            Battler victim = victims[index];
            victim.hp += message.targetsHpEffect[victim.index];
            if (victim.hp < 0)
                victim.hp = 0;
        }

        Animation attackAnim = animsDic[message.actorIndex];
        List<Animation> victimsAnim = new List<Animation>();

        for(int i = 0;i < message.victims.Length; i++)
        {
            victimsAnim.Add(animsDic[message.victims[i]]);
        }

        float time = 0f;

        bool isSkillStart = false;
        bool isSkillEnd = false;
        bool isHurt = false;
        bool isHurtEnd = false;
        bool isEffect = false;
        bool isEffectEnd = false;
        bool isAudio = false;

        List<GameObject> effectsObj = new List<GameObject>();
        List<ParticleSystem> effects = new List<ParticleSystem>();

        while (time < skillTime)
        {

            time += Time.deltaTime;

            if(time > skillStart && !isSkillStart)
            {
                attackAnim.Stop();
                attackAnim.Play(skillAnim);
                isSkillStart = true;
            }

            if(time > skillEnd && !isSkillEnd)
            {
                attackAnim.Play("jingjie");
                isSkillEnd = true;
            }

            if(time > audioTime && !isAudio)
            {
                AudioManager.Instance.PlayEffect(skillAudio);
                isAudio = true;
            }

            if(time > effectStart && !isEffect)
            {
                switch (effectType)
                {
                    case EffectType.OddNumber:
                        poolMgr.Get(effectName, (effect) =>
                        {
                            effectsObj.Add(effect);
                            effect.transform.position = victimsAnim[0].transform.position;
                            ParticleSystem[] tempEffects = effect.transform.GetComponentsInChildren<ParticleSystem>();
                            foreach (ParticleSystem particle in tempEffects)
                            {
                                effects.Add(particle);
                                particle.Play();
                            }
                        });
                        break;
                    case EffectType.ComplexNumber:
                        foreach (Animation victim in victimsAnim)
                        {
                            poolMgr.Get(effectName, (effect) =>
                            {
                                effectsObj.Add(effect);
                                effect.transform.position = victim.transform.position;
                                ParticleSystem[] tempEffects = effect.transform.GetComponentsInChildren<ParticleSystem>();
                                foreach (ParticleSystem particle in tempEffects)
                                {
                                    effects.Add(particle);
                                    particle.Play();
                                }
                            });
                        }
                        break;
                }
                
                isEffect = true;
            }

            if(time > effectEnd && !isEffectEnd)
            {
                if (effects.Count > 0)
                {
                    foreach (ParticleSystem particle in effects)
                    {
                        particle.Stop();
                    }
                }
                if(effectsObj.Count > 0)
                {
                    foreach(GameObject obj in effectsObj)
                    {
                        poolMgr.Set(obj.name, obj);
                    }
                }
                isEffectEnd = true;
            }

            if(time > hurtStart && !isHurt)
            {
                Camera.main.DOShakePosition(0.1f, new Vector3(0.1f, 0.1f, 0));

                foreach(Animation victim in victimsAnim)
                {
                    victim.Play("atted");
                }

                if (message.staticBuffs.Count > 0)
                {
                    UpdateStaticBuff(message.staticBuffs);
                }

                if (message.actionBuffs.Count > 0)
                {
                    UpdateActionBuff(message.actionBuffs,message.actionBuffsEffect);
                }
                UpdateInfo(battlersDic[message.actorIndex]);
                foreach (Battler victim in victims)
                {
                    //更新UI血条蓝条
                    UpdateInfo(victim);

                    HurtAnim(victim, message.targetsHpEffect[victim.index]);

                    buff[] buffs = GetAllBuffs(victim);

                    LuaManager.GetFunction("UIMgr.Trigger").LazyCall(LuaManager.GetTable("UIMgr"),
                        "UpdateBuff", victim.index, buffs);
                }

                isHurt = true;
            }

            if(time > hurtEnd && !isHurtEnd)
            {
                foreach(Battler victim in victims)
                {
                    if(victim.hp > 0)
                    {
                        animsDic[victim.index].Play("jingjie");
                    }
                    else
                    {
                        animsDic[victim.index].Play("dead");
                    }
                }
                
                isHurtEnd = true;
            }

            yield return 0;
        }
    }

    IEnumerator PlaySkill(S2C_BattleMessage message, float skillTime, string skillAnim, float skillStart,
        float skillEnd, string effectName, float effectStart, float effectEnd)
    {

        List<Battler> victims = new List<Battler>();

        foreach (byte index in message.victims)
        {
            victims.Add(battlersDic[index]);
        }

        for (int index = 0; index < victims.Count; index++)
        {
            Battler victim = victims[index];
            victim.hp += message.targetsHpEffect[victim.index];
            if (victim.hp < 0)
                victim.hp = 0;
        }

        Animation attackAnim = animsDic[message.actorIndex];
        List<Animation> victimsAnim = new List<Animation>();

        for (int i = 0; i < message.victims.Length; i++)
        {
            victimsAnim.Add(animsDic[message.victims[i]]);
        }

        float time = 0f;

        bool isSkillStart = false;
        bool isSkillEnd = false;
        bool isHurt = false;
        bool isEffect = false;
        bool isEffectEnd = false;

        List<GameObject> effectsObj = new List<GameObject>();
        List<ParticleSystem> effects = new List<ParticleSystem>();

        while (time < skillTime)
        {

            time += Time.deltaTime;

            if (time > skillStart && !isSkillStart)
            {
                attackAnim.Stop();
                attackAnim.Play(skillAnim);
                isSkillStart = true;
            }

            if (time > skillEnd && !isSkillEnd)
            {
                attackAnim.Play("jingjie");
                isSkillEnd = true;
            }

            if (time > effectStart && !isEffect)
            {
                foreach (Animation victim in victimsAnim)
                {
                    poolMgr.Get(effectName, (effect) =>
                    {
                        effectsObj.Add(effect);
                        effect.transform.position = victim.transform.position;
                        ParticleSystem[] tempEffects = effect.transform.GetComponentsInChildren<ParticleSystem>();
                        foreach (ParticleSystem particle in tempEffects)
                        {
                            effects.Add(particle);
                            particle.Play();
                        }
                    });
                }
                isEffect = true;
            }

            if (time > effectEnd && !isEffectEnd)
            {
                if (effects.Count > 0)
                {
                    foreach (ParticleSystem particle in effects)
                    {
                        particle.Stop();
                    }
                }
                if (effectsObj.Count > 0)
                {
                    foreach (GameObject obj in effectsObj)
                    {
                        poolMgr.Set(obj.name, obj);
                    }
                }
                isEffectEnd = true;
            }

            if (time > effectStart + 0.2f && !isHurt)
            {
                Camera.main.DOShakePosition(0.1f, new Vector3(0.2f, 0.2f, 0));

                foreach (Animation victim in victimsAnim)
                {
                    victim.Play("atted");
                }

                if (message.staticBuffs.Count > 0)
                {
                    UpdateStaticBuff(message.staticBuffs);
                }
                if (message.actionBuffs.Count > 0)
                {
                    UpdateActionBuff(message.actionBuffs,message.actionBuffsEffect);
                }

                foreach (Battler victim in victims)
                {
                    //更新UI血条蓝条
                    LuaManager.GetFunction("UIMgr.Trigger").LazyCall(LuaManager.GetTable("UIMgr"),
                        "UpdateHeadOtherInfo", victim.index, victim.hp, victim.maxHp, victim.mp,
                        victim.maxMp);

                    //获取伤害值百分比视图坐标
                    Vector3 point = Camera.main.WorldToViewportPoint(animsDic[victim.index]
                        .transform.position);

                    //播放UI伤害动画
                    LuaManager.GetFunction("UIMgr.Trigger").LazyCall(LuaManager.GetTable("UIMgr"),
                        "HurtAnim", new Vector2(point.x * Screen.width, (point.y * Screen.height)
                        + 150), new Vector2(point.x * Screen.width, (point.y * Screen.height) + 200),
                        message.targetsHpEffect[victim.index]);

                    buff[] buffs = GetAllBuffs(victim);

                    LuaManager.GetFunction("UIMgr.Trigger").LazyCall(LuaManager.GetTable("UIMgr"),
                        "UpdateBuff", victim.index, buffs);
                }

                isHurt = true;
            }
            yield return 0;
        }
    }

    IEnumerator PlayItem(S2C_BattleMessage message)
    {
        Battler victim = battlersDic[message.victims[0]];
        victim.hp += message.targetsHpEffect[victim.index];
        if (victim.hp > victim.maxHp)
            victim.hp = victim.maxHp;
        float time = 0;
        //bool isSkillStart = false;
        //bool isSkillEnd = false;
        bool isReply = false;
        Animation actor = animsDic[message.actorIndex];
        actor.Play("att01");
        while(time < 1.05f)
        {
            time += Time.deltaTime;
            if(time > 0.7f && !isReply)
            {
                UpdateInfo(victim);
                HurtAnim(victim, message.targetsHpEffect[victim.index]);
                isReply = true;
            }
            yield return 0;
        }
        actor.Play("jingjie");
    }

    //IEnumerator PlayEscape(S2C_BattleMessage message)
    //{
    //    GameObject actor = animsDic[message.actorIndex].gameObject;
    //    poolMgr.Set(actor.name, actor);
    //    LuaManager.GetFunction("UIMgr.Trigger").LazyCall(LuaManager.GetTable("UIMgr"),
    //        "RemoveHead",message.actorIndex);
    //    GOsDic.Remove(actor);
    //    if(clientIndex == message.actorIndex)
    //        LuaManager.GetFunction("UIMgr.Trigger").LazyCall(LuaManager.GetTable("UIMgr"),
    //        "Escape");
    //    yield return 0;
    //}

    public void UpdateInfo(Battler battler)
    {
        LuaManager.GetFunction("UIMgr.Trigger").LazyCall(LuaManager.GetTable("UIMgr"),
            "UpdateHeadOtherInfo", battler.index, battler.hp, battler.maxHp, battler.mp,
            battler.maxMp);
    }

    public void HurtAnim(Battler victim,float hpEffect)
    {
        //获取伤害值百分比视图坐标
        Vector3 point = Camera.main.WorldToViewportPoint(animsDic[victim.index]
            .transform.position);

        //播放UI伤害动画
        LuaManager.GetFunction("UIMgr.Trigger").LazyCall(LuaManager.GetTable("UIMgr"),
            "HurtAnim", new Vector2(point.x * Screen.width, (point.y * Screen.height)
            + 150), new Vector2(point.x * Screen.width, (point.y * Screen.height) + 200),
            hpEffect);
    }

    [Rpc]
    public void NewRound(S2C_Round S2C_round)
    {
        if (battlersDic[clientIndex].hp == 0)
            return;
        LuaManager.GetFunction("UIMgr.Trigger").LazyCall(LuaManager.GetTable("UIMgr"), 
            "NewRound", S2C_round.round);
        time = 30;
        isTiming = true;
    }

    [Rpc]
    public void BattleEnd(S2C_BattleSettle S2C_battleSettle)
    {
        battleSettle = S2C_battleSettle;
        isBattleEnd = true;

    }

    public void UpdateStaticBuff(Dictionary<byte,int> buffsDic)
    {
        foreach (byte index in buffsDic.Keys)
        {
            Battler victim = battlersDic[index];
            int buffId = buffsDic[index];
            byte rounds = (byte)ConfigManager.Instance.GetConfig<BuffConfig>(buffId)
                .rounds;
            if (victim.staticBuffs.ContainsKey(buffId))
                victim.staticBuffs[buffId] = rounds;
            else
                victim.staticBuffs.Add(buffId, rounds);
        }
    }

    public void UpdateActionBuff(Dictionary<byte,int> buffsDic,Dictionary<int,float> buffsEffect)
    {
        foreach (byte index in buffsDic.Keys)
        {
            Battler victim = battlersDic[index];
            int buffId = buffsDic[index];
            byte rounds = (byte)ConfigManager.Instance.GetConfig<BuffConfig>(buffId)
                .rounds;
            if (victim.actionBuffs.ContainsKey(buffId))
            {
                victim.actionBuffs[buffId] = rounds;
                victim.actionBuffsEffect[buffId] = buffsEffect[victim.index];
            }
            else
            {
                victim.actionBuffs.Add(buffId, rounds);
                victim.actionBuffsEffect.Add(buffId, buffsEffect[victim.index]);
            }
        }
    }

    public buff[] GetAllBuffs(Battler battler)
    {
        List<buff> buffs = new List<buff>();
        if(battler.staticBuffs.Count > 0)
        {
            List<int> buffsId = battler.staticBuffs.Keys.ToList();
            foreach(int buffId in buffsId)
            {
                buffs.Add(new buff(buffId, battler.staticBuffs[buffId]));
            }
        }
        if(battler.actionBuffs.Count > 0)
        {
            List<int> buffsId = battler.actionBuffs.Keys.ToList();
            foreach (int buffId in buffsId)
            {
                buffs.Add(new buff(buffId, battler.actionBuffs[buffId]));
            }
        }
        if (buffs.Count < 1)
            return null;
        return buffs.ToArray();
    }

    public Vector2 GetBattlerPosToUI(byte index)
    {
        GameObject go = animsDic[index].gameObject;
        Vector3 point = Camera.main.WorldToViewportPoint(go.transform.position);
        return new Vector2(point.x * Screen.width, (point.y * Screen.height) + 100);
    }
}

public class Battler
{
    public Battler(byte index)
    {
        this.index = index;
        staticBuffs = new Dictionary<int, byte>();
        actionBuffs = new Dictionary<int, byte>();
        actionBuffsEffect = new Dictionary<int, float>();
    }

    public float hp;
    public float maxHp;
    public float mp;
    public float maxMp;
    public Vector3 position;
    public Vector3 angle;
    public byte index;

    public Dictionary<int, byte> staticBuffs;  //所拥有的静态buffId对应的所剩回合数
    public Dictionary<int, byte> actionBuffs;  //所拥有的动态buffId对应的所剩回合数
    public Dictionary<int, float> actionBuffsEffect;  //所拥有的动态buffId对应的hp影响

}

public class buff
{
    public buff(int buffId,int rounds)
    {
        this.buffId = buffId;
        this.rounds = rounds;
    }
    public int buffId;
    public int rounds;
}