using LuaFramework;
using LuaInterface;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Net.Share;
using OfficeOpenXml;


/// <summary>
/// 玩家对象管理器
/// </summary>
public class RoleMgr
{
    private static RoleMgr instance;
    public static RoleMgr Instance
    {
        get
        {
            if (instance == null)
                instance = new RoleMgr();
            return instance;
        }
    }

    //两个字典都用于存储角色对象，以玩家id作为唯一标识，后者用于获取对应对象的onlyId
    Dictionary<string, GameObject> rolesDic = new Dictionary<string, GameObject>();
    Dictionary<GameObject, string> onlyIdsDic = new Dictionary<GameObject, string>();

    /// <summary>
    /// 主角id
    /// </summary>
    private string mainRoleId;
    public string MainRoleId
    {
        get
        {
            return mainRoleId;
        }
    }

    /// <summary>
    /// 获取主角对象
    /// </summary>
    /// <returns></returns>
    public GameObject GetMainRole()
    {
        return rolesDic[mainRoleId];
    }

    public string GetRoleGuid(GameObject go)
    {
        if (!onlyIdsDic.TryGetValue(go, out string Guid))
        {
            Debugger.LogError($"{Guid}该id对应的角色不存在");
            return null;
        }
        return Guid;
    }

    /// <summary>
    /// 创建主角
    /// </summary>
    [Rpc]
    public void CreateMainRole(S2C_MainRoleInfo S2C_mainRoleInfo)
    {
        NPCManager.Instance.Init();
        Vector3 followDelta = new Vector3(4.74f, 5.6f, -0.21f);
        Vector3 dir = new Vector3(35.96f, -90.825f, 0f);
        Debugger.Log(S2C_mainRoleInfo.Guid);
        CameraMgr.Instance.camera.position = S2C_mainRoleInfo.position + followDelta;
        CameraMgr.Instance.camera.localEulerAngles = dir;
        
        Debugger.Log("创建主角");
        mainRoleId = S2C_mainRoleInfo.Guid;
        //加载角色预制体
        //GameObject go = new GameObject();
        LuaHelper.GetObjectPoolManager().Get(ConfigManager.Instance.GetConfig<ProfessionConfig>(S2C_mainRoleInfo.professionId).model, (go) =>
        {
            //此坐标应由服务端传过来
            go.transform.position = S2C_mainRoleInfo.position;
            //将角色加入角色管理器字典
            rolesDic.Add(mainRoleId, go);
            onlyIdsDic.Add(go, mainRoleId);

            Animation animation = go.GetComponent<Animation>();
            CharacterController controller = go.GetComponent<CharacterController>();
            //创建角色状态管理器
            StateManager state = new StateManager(mainRoleId, (int)RoleState.Length);
            state.AddState((int)RoleState.Idle, new StateIdle(animation, controller, (string)this.mainRoleId));
            state.AddState((int)RoleState.Run, new StateRun(animation, controller, (string)this.mainRoleId));
            state.AddState((int)RoleState.Follow, new StateFollow(animation, controller, (string)this.mainRoleId));
            //默认为站立状态
            state.StateChange((int)RoleState.Idle);
            //加入全局的状态管理器
            StateMgr.Instance.AddStateManager(mainRoleId, state);
            Actor actor = new Actor(go);
            if (S2C_mainRoleInfo.weaponId != -1)
            {
                actor.UpdateWeapon(S2C_mainRoleInfo.weaponId);
            }
            ActorMgr.Instrance.AddActor(S2C_mainRoleInfo.Guid, actor);

            //创建角色基本数据
            //BaseData baseData = new BaseData(id, "林小刀", 99, 150, 1, 1, 1, Vector3.zero, Vector3.zero);
            //BaseData baseData = new BaseData(id, 150, Vector3.zero, Vector3.zero);
            //加入全局的基本数据管理器
            //BaseDataMgr.Instance.AddBaseData(id, baseData);

            //Util.CallMethod("BaseDataMgr", "AddBaseData", id, baseData);


            //添加物品栏管理器，并将服务器传过来的物品添加进去
            /*
            AppFacade.Instance.GetManager<LuaManager>(ManagerName.Lua).GetFunction("InventoryMgr.AddNewItem").
            LazyCall(mainRoleId, 1, 1, 2, 10010005, 1);
            AppFacade.Instance.GetManager<LuaManager>(ManagerName.Lua).GetFunction("InventoryMgr.AddNewItem").
            LazyCall(mainRoleId, 1, 1, 2, 10010001, 2);
            AppFacade.Instance.GetManager<LuaManager>(ManagerName.Lua).GetFunction("InventoryMgr.AddNewItem").
            LazyCall(mainRoleId, 2, 1, 2, 10010006, 3);
            
            //添加角色其他属性，并加入其他属性管理器
            AppFacade.Instance.GetManager<LuaManager>(ManagerName.Lua).GetFunction("OtherDataMgr.AddNewOtherData").
            LazyCall(mainRoleId, 400, 1000, 300, 700, 500, 153, 79, 354, 154, 200, 1000, 500, 20);
            */


            //创建摄像机,并跟随主角

            CameraMgr.Instance.followTarget = go.transform;
            CameraMgr.Instance.ChangeFollow();
            //此坐标应再配置表读取
            //CameraMgr.Instance.Init(new Vector3(35.96f, -90.825f, 0f));
            LuaManager luaMgr = AppFacade.Instance.GetManager<LuaManager>(ManagerName.Lua);
            luaMgr.GetFunction("UIMgr.Trigger").LazyCall(luaMgr.GetTable("UIMgr"),
                "AddHead", S2C_mainRoleInfo.Guid, Vector2.zero, false, false,
                S2C_mainRoleInfo.name);

            luaMgr.GetFunction("MainUICtrl.InitMainRole").Call(luaMgr.GetTable("MainUICtrl"),S2C_mainRoleInfo);

        });
    }

    public GameObject GetRole(string roleId)
    {
        if (!rolesDic.TryGetValue(roleId, out GameObject role))
        {
            //Debugger.LogError($"---------------{roleId}该玩家id没有对应的对象--------------");
            return null;
        }
        return role;
    }

    public Vector2 GetRolePosToUI(string roleId)
    {
        Vector3 pos = GetRole(roleId).transform.position;
        Vector3 point = Camera.main.WorldToViewportPoint(pos);
        float effectX = point.x < 0.5f ? 0.5f - point.x : point.x - 0.5f;
        if (point.x < 0.5f)
            return new Vector2(point.x * Screen.width - Screen.width * effectX * 0.15f,
                (point.y * Screen.height) + 115);
        else
            return new Vector2(point.x * Screen.width + Screen.width * effectX * 0.15f,
                (point.y * Screen.height) + 115);
        //return new Vector2(point.x * Screen.width ,
        //        (point.y * Screen.height) + 115);
    }


    /// <summary>
    /// 创建其他玩家实例
    /// </summary>
    [Rpc]
    public void CreateOtherRole(S2C_OtherRoleInfo S2C_otherRoleInfo)
    {

        LuaHelper.GetObjectPoolManager().Get(ConfigManager.Instance.GetConfig<ProfessionConfig>(S2C_otherRoleInfo.professionId).model, (go) =>
        {
            LuaManager lua = AppFacade.Instance.GetManager<LuaManager>(ManagerName.Lua);
            lua.GetFunction("MainUICtrl.InitOtherRole").Call(lua.GetTable("MainUICtrl"), S2C_otherRoleInfo);

            go.transform.position = S2C_otherRoleInfo.position;
            rolesDic.Add(S2C_otherRoleInfo.Guid, go);
            onlyIdsDic.Add(go, S2C_otherRoleInfo.Guid);
            Animation animation = go.GetComponent<Animation>();
            CharacterController controller = go.GetComponent<CharacterController>();
            //创建角色状态管理器
            StateManager state = new StateManager(S2C_otherRoleInfo.Guid, (int)RoleState.Length);
            state.AddState((int)RoleState.Idle, new StateIdle(animation, controller, S2C_otherRoleInfo.Guid));
            state.AddState((int)RoleState.Run, new StateRun(animation, controller, S2C_otherRoleInfo.Guid));
            state.AddState((int)RoleState.Follow, new StateFollow(animation, controller, S2C_otherRoleInfo.Guid));
            //默认为站立状态
            state.StateChange((int)RoleState.Idle);
            //加入全局的状态管理器
            StateMgr.Instance.AddStateManager(S2C_otherRoleInfo.Guid, state);

            Actor actor = new Actor(go);
            if (S2C_otherRoleInfo.weaponId != -1)
            {
                actor.UpdateWeapon(S2C_otherRoleInfo.weaponId);
            }
            ActorMgr.Instrance.AddActor(S2C_otherRoleInfo.Guid, actor);

            LuaManager luaMgr = AppFacade.Instance.GetManager<LuaManager>(ManagerName.Lua);
            
            luaMgr.GetFunction("UIMgr.Trigger").LazyCall(luaMgr.GetTable("UIMgr"),
                "AddHead", S2C_otherRoleInfo.Guid, Vector2.zero, S2C_otherRoleInfo.isLeader,
                S2C_otherRoleInfo.isBattle, S2C_otherRoleInfo.name);
            //AppFacade.Instance.GetManager<LuaManager>(ManagerName.Lua).GetFunction("BaseDataMgr.AddNewBaseData").
            //LazyCall(S2C_otherRoleInfo.Guid, S2C_otherRoleInfo.name, S2C_otherRoleInfo.level, S2C_otherRoleInfo.professionId, S2C_otherRoleInfo.professionId,
            //S2C_otherRoleInfo.schoolId);
        });
    }

    [Rpc]
    public void CreateAllOtherRoles(List<S2C_OtherRoleInfo> otherRoleInfos)
    {
        foreach (S2C_OtherRoleInfo otherRoleInfo in otherRoleInfos)
        {
            CreateOtherRole(otherRoleInfo);
        }
    }

    void CreateMonster()
    {

    }

    void CreatePartner()
    {

    }

    public void ClearRoles()
    {
        ObjectPoolManager poolMgr = AppFacade.Instance.GetManager<ObjectPoolManager>(ManagerName.ObjectPool);
        foreach (GameObject role in rolesDic.Values)
        {
            poolMgr.Set(role.name, role);
        }
        rolesDic.Clear();
        onlyIdsDic.Clear();
    }
}
