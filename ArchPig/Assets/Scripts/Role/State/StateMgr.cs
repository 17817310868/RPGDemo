/*
 * ===================================================================
 * 
 *          projectName:
 *              项目名称:拱猪
 *                  
 *          title:
 *              标题:全局的状态管理器
 *          
 *          description:
 *              功能描述:管理所有玩家的状态
 *              
 *          author:
 *              作者:
 * 
 * ===================================================================
 */

using GameDesigner;
using LuaInterface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Net.Share;
using System.Data;

public enum RoleState
{
    Idle,
    Run,
    Follow,
    Fight,


    Length
}

/// <summary>
/// 用于管理所有角色对应的状态管理器
/// </summary>
public class StateMgr 
{
    private static StateMgr instance;
    public static StateMgr Instance
    {
        get
        {
            if (instance == null)
                instance = new StateMgr();
            return instance;
        }
    }

    //存储所有状态管理，以玩家id作为唯一标识
    Dictionary<string, StateManager> stateManangersDic = new Dictionary<string, StateManager>();

    /// <summary>
    /// 添加状态管理器
    /// </summary>
    /// <param name="roleId"></param>
    /// <param name="stateManager"></param>
    public void AddStateManager(string roleId,StateManager stateManager)
    {
        if(stateManangersDic.TryGetValue(roleId,out StateManager manager))
        {
            Debugger.LogError($"--------------{roleId}对应的角色已拥有状态管理器，请勿重复添加-------------");
            return;
        }
        stateManangersDic.Add(roleId, stateManager);
    }

    /// <summary>
    /// 获取对应角色id的状态管理器
    /// </summary>
    /// <param name="roleId"></param>
    public StateManager GetStateManager(string roleId)
    {
        if (!stateManangersDic.TryGetValue(roleId, out StateManager manager))
        {
            Debugger.LogError($"--------------{roleId}对应的角色没有状态管理器-------------");
            return null;
        }
        return stateManangersDic[roleId];
    }

    /// <summary>
    /// 移除状态管理器
    /// </summary>
    /// <param name="roleId"></param>
    public void RemoveStateManager(string roleId)
    {
        if (!stateManangersDic.TryGetValue(roleId, out StateManager manager))
        {
            Debugger.LogError($"--------------{roleId}对应的角色没有状态管理器-------------");
            return;
        }

        stateManangersDic.Remove(roleId);
    }

    /// <summary>
    /// 切换对应id的玩家的状态
    /// </summary>
    /// <param name="roleId"></param>
    /// <param name="stateId"></param>
    public void ChangeState(string roleId,int stateId)
    {
        if(!stateManangersDic.TryGetValue(roleId,out StateManager stateManager))
        {
            Debugger.LogError($"--------------{roleId}对应的角色没有状态管理器-------------");
            return;
        }
        stateManager.StateChange(stateId);
        
    }

    [Rpc]
    public void ChangeIdle(string roleId)
    {
        if (!stateManangersDic.TryGetValue(roleId, out StateManager stateManager))
        {
            Debugger.LogError($"--------------{roleId}对应的角色没有状态管理器-------------");
            return;
        }
        stateManager.StateChange((int)RoleState.Idle);
    }

    /// <summary>
    /// 切换id对应的玩家至移动状态
    /// </summary>
    /// <param name="roleId"></param>
    /// <param name="speed"></param>
    /// <param name="target"></param>
    [Rpc]
    public void ChangeMoveState(S2C_MoveInfo moveInfo)
    {
        if(!stateManangersDic.TryGetValue(moveInfo.Guid,out StateManager stateManager))
        {
            Debugger.LogError($"--------------{moveInfo.Guid}对应的角色没有状态管理器-------------");
            return;
        }
        //stateManager.StateChange((int)RoleState.Run);
        stateManager.ChangeMoveState(moveInfo.target, moveInfo.speed);
    }

    /// <summary>
    /// 切换id对应的玩家至跟随状态
    /// </summary>
    /// <param name="roleId"></param>
    /// <param name="speed"></param>
    /// <param name="targetId"></param>
    [Rpc]
    public void ChangeFollowState(S2C_FollowInfo S2C_followInfo)
    {
        if (!stateManangersDic.TryGetValue(S2C_followInfo.Guid, out StateManager stateManager))
        {
            Debugger.LogError($"--------------{S2C_followInfo.Guid}对应的角色没有状态管理器-------------");
            return;
        }
        stateManager.ChangeFollowState(RoleMgr.Instance.GetRole(S2C_followInfo.targetId).transform,
            S2C_followInfo.speed);
    }

    /// <summary>
    /// 清空状态管理器
    /// </summary>
    public void Clear()
    {
        stateManangersDic.Clear();
    }

    /// <summary>
    /// 更新所有状态管理器中的状态
    /// </summary>
    public void UpdateState()
    {
        if (stateManangersDic == null)
            return;
        foreach(StateManager stateManager in stateManangersDic.Values)
        {
            stateManager.StateStay();
        }
    }

    public IEnumerator ShowPose(GameObject obj)
    {
        Animation animation = obj.GetComponent<Animation>();
        animation.CrossFade("att01");
        while (animation.isPlaying == true)
        {
            yield return 0;
        }
        animation.CrossFade("stand",2f,PlayMode.StopAll);
    }
}
