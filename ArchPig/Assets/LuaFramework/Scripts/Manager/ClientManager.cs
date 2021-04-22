using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Net.Client;
using Net.Share;
using LuaInterface;
using LuaFramework;
using System;
using GameDesigner;
using System.ComponentModel;

public class ClientManager : NetBehaviour , INetworkHandle ,IDebugHandle
{
    /// <summary>
    /// 声明客户端
    /// </summary>
    public UdpClient client = new UdpClient(true);
    private LuaManager lua;

    // Start is called before the first frame update
    void Start()
    {
        //获取lua管理器
        lua = AppFacade.Instance.GetManager<LuaManager>(ManagerName.Lua);
        //默认连接本地服务器;
        client.Connect("192.168.1.106", 666, (result) =>
          {

          });
      
        
        //绑定当前的网络状态处理接口
        client.BindNetworkHandle(this);
        //绑定当前的网络调试信息处理接口
        client.BindLogHandle(this);
        client.AddRpcHandle(RoleMgr.Instance);
        client.AddRpcHandle(StateMgr.Instance);
        client.AddRpcHandle(AppFacade.Instance.GetManager<BattleMgr>(ManagerName.Battle));
        client.AddRpcHandle(ActorMgr.Instrance);
        
    }

    void Update()
    {
        client.FixedUpdate();
    }

    public static void MySend(string func)
    {
        Send(func);
    }

    #region 统一管理发送至lua脚本的信息，这里进行转发

    [Rpc]
    void CallLuaFunction(string func, MessageModel result)
    {
        Debugger.Log(func);
        string tableName = func.Split('.')[0];
        LuaTable table = lua.GetTable(tableName);
        LuaFunction function = lua.GetFunction(func);
        
        function.LazyCall(table, result);
    }

    [Rpc]
    void CallLuaFunction(string func)
    {
        LuaFunction function = lua.GetFunction(func);
        function.Call(lua.GetTable(func.Split('.')[0]));
    }

    void OnDestroy() 
    {
        client.AbortedThread();
    }

    /*
    [Rpc]
    void InitMainRole(S2C_MainRoleInfo S2C_mainRoleInfo)
    {
        RoleMgr.Instance.CreateMainRole(S2C_mainRoleInfo);
        LuaFunction function = lua.GetFunction("RoleInfoMgr.AddMainRole");
        function.Call(lua.GetTable("RoleInfoMgr"), S2C_mainRoleInfo);
        lua.GetFunction("MainUICtrl.UpdateRoleInfo").Call(lua.GetTable("MainUICtrl"));
    }

    [Rpc]
    void CreateOtherRole(S2C_OtherRoleInfo S2C_otherRoleInfo)
    {
        RoleMgr.Instance.CreateOtherRole(S2C_otherRoleInfo);
        LuaFunction function = lua.GetFunction("RoleInfoMgr.AddRole");
        function.Call(lua.GetTable("RoleInfoMgr"), S2C_otherRoleInfo);
        
    }

    [Rpc]
    void CreateAllOtherRoles(List<S2C_OtherRoleInfo> otherRoleInfos)
    {
        //RoleMgr.Instance.CreateAllOtherRoles(otherRoleInfos);
        foreach (S2C_OtherRoleInfo otherRole in otherRoleInfos)
        {
            CreateOtherRole(otherRole);
        }
    }

    
    [Rpc]
    void ChangeMoveState(S2C_MoveInfo moveInfo)
    {
        StateMgr.Instance.ChangeMoveState(moveInfo);
    }

    //[Rpc]
    //void UpdateTeamInfo(S2C_TeamInfo S2C_teamInfo)
    //{
    //    Debugger.Log(S2C_teamInfo.playersGuid.Count);
    //    lua.GetFunction("ClientMgr.UpdateTeamInfo").LazyCall(lua.GetTable("ClientMgr"), S2C_teamInfo);
    //}

    
    [Rpc]
    void JoinTeamRequest(S2C_JoinTeamRequest S2C_joinTeamRequest)
    {
        lua.GetFunction("ClientMgr.JoinRequest").LazyCall(lua.GetTable("ClientMgr"), S2C_joinTeamRequest);
    }

    [Rpc]
    void JoinTeamReply(S2C_JoinTeamReply S2C_joinTeamReply)
    {
        lua.GetFunction("ClientMgr.JoinReply").LazyCall(lua.GetTable("ClientMgr"), S2C_joinTeamReply);
    }

    [Rpc]
    void VisiteTeamRequest(S2C_VisiteRequest S2C_visiteRequest)
    {
        lua.GetFunction("ClientMgr.VisiteRequest").LazyCall(lua.GetTable("ClientMgr"), S2C_visiteRequest);
    }

    [Rpc]
    void VisiteTeamReply(S2C_VisiteReply S2C_visiteReply)
    {
        lua.GetFunction("ClientMgr.VisiteReply").LazyCall(lua.GetTable("ClientMgr"), S2C_visiteReply);
    }

    

    [Rpc]
    void BattleRequest(S2C_BattleRequest S2C_battleRequest)
    {
        lua.GetFunction("ClientMgr.BattleRequest").LazyCall(lua.GetTable("ClientMgr"), S2C_battleRequest);
    }

    [Rpc]
    void BattleReply(S2C_BattleReply S2C_battleReply)
    {
        lua.GetFunction("ClientMgr.BattleReply").LazyCall(lua.GetTable("ClientMgr"), S2C_battleReply);
    }

    

    [Rpc]
    void InitBattle(S2C_InitBattlers S2C_initBattlers)
    {
        Debugger.Log("初始化战斗");
        
        
        AppFacade.Instance.GetManager<BattleMgr>(ManagerName.Battle).InitBattle(S2C_initBattlers);
        
    }

    

    [Rpc]
    void ShowBattleAnim(List<S2C_BattleMessage> S2C_battleMessages)
    {
        AppFacade.Instance.GetManager<BattleMgr>(ManagerName.Battle).ShowBattleAnim(S2C_battleMessages);
    }

        

    [Rpc]
    void BattleEnd(S2C_BattleSettle S2C_battleSettle)
    {
        AppFacade.Instance.GetManager<BattleMgr>(ManagerName.Battle).BattleEnd(S2C_battleSettle);
    }

        

    [Rpc]
    void NewRound(S2C_Round S2C_round)
    {
        lua.GetFunction("UIMgr.Trigger").LazyCall(lua.GetTable("UIMgr"), "NewRound", S2C_round.round);
        AppFacade.Instance.GetManager<BattleMgr>(ManagerName.Battle).time = 30;
        AppFacade.Instance.GetManager<BattleMgr>(ManagerName.Battle).isTiming = true;
    }

        
    [Rpc]
    void OtherRoleBattle(List< S2C_UpdateBattle> S2C_updateBattles)
    {
        lua.GetFunction("ClientMgr.OtherRoleBattle").LazyCall(lua.GetTable("ClientMgr"), S2C_updateBattles);
    }

    /*
    [Rpc]
    void UpdateLeader(S2C_UpdateLeader S2C_updateLoader)
    {
        lua.GetFunction("UIMgr.Trigger").LazyCall(lua.GetTable("UIMgr"), "UpdateHeadLeader",
            S2C_updateLoader.Guid, S2C_updateLoader.result);
    }

    
    [Rpc]
    void UpdateFollow(S2C_FollowInfo S2C_followInfo)
    {
        StateMgr.Instance.ChangeFollowState(S2C_followInfo.Guid, S2C_followInfo.speed,
            S2C_followInfo.targetId);
    }

    [Rpc]
    void UpdateIdle(string Guid)
    {
        StateMgr.Instance.ChangeState(Guid, (int)RoleState.Idle);
    }

    */
    ////登录回调
    //[Rpc]
    //void LoginCallback(string func , MessageModel result)
    //{
    //    LuaFunction function = GetLuaFunc(func);
    //    function.Call(result);
    //}

    ////注册回调
    //[Rpc]
    //void RegisterCallback(string func, MessageModel result)
    //{
    //    LuaFunction function = GetLuaFunc(func);
    //    function.Call(result);
    //}

    /// <summary>
    /// 获取对应的lua方法
    /// </summary>
    /// <param name="func"></param>
    /// <returns></returns>
    //LuaFunction GetLuaFunc(string func)
    //{

    //}

    #endregion
    // Update is called once per frame

    #region 网络状态处理接口
    public void OnBlockConnection()
    {
         
    }

    public void OnCloseConnect()
    {
        
    }

    public void OnConnected()
    {
         
    }

    public void OnConnectFailed()
    {
         
    }

    public void OnConnectLost()
    {
         
    }

    public void OnDisconnect()
    {
         
    }

    public void OnReconnect()
    {
         
    }

    public void OnTryToConnect()
    {
         
    }

    #endregion
    #region 网络调试信息处理接口
    public void DebugLog(string msg)
    {
        Debug.Log(msg);
    }

    public void RpcLog(string msg)
    {
        //Debug.Log(msg);
    }

    public void ErrorLog(string msg)
    {
        //Debug.Log(msg);
    }
    #endregion
}
