using LuaFramework;
using LuaInterface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.Networking;
using Net.Share;

public enum EquipEnum
{
    None = 0,
    Helmet,
    Necklace,
    Weapon,
    Clothes,
    Belt,
    Shoes
}

public class ActorMgr
{
    private static ActorMgr instance;
    public static ActorMgr Instrance
    {
        get
        {
            if (instance == null)
                instance = new ActorMgr();
            return instance;
        }
    }

    Dictionary<string, Actor> actorsDic = new Dictionary<string, Actor>();

    public void AddActor(string Guid,Actor actor)
    {
        if(actorsDic.TryGetValue(Guid,out Actor a))
        {
            Debugger.LogError($"{Guid}该id对应的Actor已存在，无法添加");
            return;
        }

        actorsDic.Add(Guid, actor);
    }

    public void RemoveActor(string Guid)
    {
        if (!actorsDic.TryGetValue(Guid, out Actor actor))
        {
            Debugger.LogError($"{Guid}该id对应的Actor不存在，无法删除");
            return;
        }
        actorsDic.Remove(Guid);
    }

    public Actor GetActor(string Guid)
    {
        if (!actorsDic.TryGetValue(Guid, out Actor actor))
        {
            Debugger.LogError($"{Guid}该id对应的Actor不存在，无法获取");
            return null;
        }
        return actor;
    }

    [Rpc]
    public void TakeoffEquip(S2C_TakeoffEquip S2C_takeoffEquip)
    {
        if (GetActor(S2C_takeoffEquip.Guid) == null)
            return;
        GetActor(S2C_takeoffEquip.Guid).DropBind(S2C_takeoffEquip.type);
    }

    [Rpc]
    public void DressEquip(S2C_DressEquip S2C_dressEquip)
    {
        if (GetActor(S2C_dressEquip.Guid) == null)
            return;
        GetActor(S2C_dressEquip.Guid).UpdateBind(S2C_dressEquip.equipId);
    }
}


public class Actor
{
    private Transform go; //模型预制体
    public Actor(GameObject go)
    {
        this.go = go.transform;
        weaponBind = Util.GetChild(this.go, "Slot_R_Hand");
        Init();
    }

    private Transform weaponBind;  //武器绑定点
    /*  待扩展
    private Transform helmetBind;  //头盔绑定点
    private Transform beltBind;  //腰带绑定点
    private Transform necklaceBind;  //项链绑定点
    private Transform clothesBind;  //衣服绑定点
    private Transform shoesBind;  //鞋子绑定点
    */


    public void Init()
    {
        ClearBind(weaponBind);
    }
    

    public void DropBind(int equipType)
    {
        switch ((EquipEnum)equipType)
        {
            case EquipEnum.Helmet:
                break;
            case EquipEnum.Necklace:
                break;
            case EquipEnum.Weapon:
                DropWeapon();
                break;
            case EquipEnum.Clothes:
                break;
            case EquipEnum.Belt:
                break;
            case EquipEnum.Shoes:
                break;
        }
    }


    public void DropWeapon()
    {
        ClearBind(weaponBind);
    }

    void ClearBind(Transform bind)
    {
        Transform[] transforms = bind.GetComponentsInChildren<Transform>();
        if (transforms.Length > 1)
        {
            LuaHelper.GetObjectPoolManager().Set(transforms[1].name, transforms[1].gameObject);
        }
    }

    public void UpdateBind(int equipId)
    {
        int equipType = ConfigManager.Instance.GetConfig<EquipConfig>(equipId)._type;
        switch ((EquipEnum)equipType)
        {
            case EquipEnum.Helmet:
                break;
            case EquipEnum.Necklace:
                break;
            case EquipEnum.Weapon:
                UpdateWeapon(equipId);
                break;
            case EquipEnum.Clothes:
                break;
            case EquipEnum.Belt:
                break;
            case EquipEnum.Shoes:
                break;
        }
    }

    public void UpdateWeapon(int equipId)
    {
        DropWeapon();
        LuaHelper.GetObjectPoolManager().Get(ConfigManager.Instance.GetConfig<EquipConfig>(equipId).model, (go) =>
        {
            go.transform.position = weaponBind.position;
            go.transform.SetParent(weaponBind);
            go.transform.localPosition = Vector3.zero;
            go.transform.localEulerAngles = Vector3.zero;
            //int professionId = ConfigManager.Instance.GetConfig<EquipConfig>(equipId).profession;
        });
    }
}