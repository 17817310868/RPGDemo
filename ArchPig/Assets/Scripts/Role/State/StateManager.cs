/*
 * ===================================================================
 * 
 *          projectName:
 *              项目名称:拱猪
 *                  
 *          title:
 *              标题:单一状态管理器
 *          
 *          description:
 *              功能描述:管理玩家的状态
 *              
 *          author:
 *              作者:照着教程敲出bug的程序员
 * 
 * ===================================================================
 */

using LuaInterface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 管理单一角色的所有状态的状态管理器
/// </summary>
public class StateManager
{

    private string roleId;
    public string RoleId
    {
        get
        {
            return roleId;
        }
    }

    //存储该角色的所有状态
    StateBase[] StateArray;
    //玩家状态总数
    int stateCount;
    public StateManager(string roleId,int stateCount)
    {
        this.stateCount = stateCount;
        StateArray = new StateBase[this.stateCount];
    }

    //当前状态id
    int currentStateId = -1;

    /// <summary>
    /// 切换状态
    /// </summary>
    /// <param name="index"></param>
    public void StateChange(int index)
    {
        if (index >= stateCount || index < 0 || StateArray[index] == null)
        {
            Debugger.LogError($"--------------该角色没有第{index}个状态---------");
            return;
        }
        //if (currentStateId != -1)
            //StateArray[currentStateId].StateEnd();
        currentStateId = index;
        StateArray[currentStateId].StateEnter();
    }

    /// <summary>
    /// 切换至移动状态
    /// </summary>
    /// <param name="target"></param>
    /// <param name="speed"></param>
    public void ChangeMoveState(Vector3 target,float speed)
    {
        if(StateArray[(int)RoleState.Run] == null)
        {
            Debugger.LogError($"--------------该角色没有第{RoleState.Run}状态---------");
            return;
        }
        StateRun run = StateArray[(int)RoleState.Run] as StateRun;
        run.Init(target, speed);
        currentStateId = (int)RoleState.Run;
        StateArray[currentStateId].StateEnter();
    }

    /// <summary>
    /// 切换至跟随状态
    /// </summary>
    /// <param name="target"></param>
    /// <param name="speed"></param>
    public void ChangeFollowState(Transform target,float speed)
    {
        if(StateArray[(int)RoleState.Follow] == null)
        {
            Debugger.LogError($"--------------该角色没有第{RoleState.Follow}状态---------");
            return;
        }
        StateFollow follow = StateArray[(int)RoleState.Follow] as StateFollow;
        follow.Init(target, speed);
        currentStateId = (int)RoleState.Follow;
        StateArray[currentStateId].StateEnter();
    }

    /// <summary>
    /// 处于状态中调用
    /// </summary>
    public void StateStay()
    {
        if (currentStateId == -1)
            return;
        StateArray[currentStateId].StateStay();
    }

    /// <summary>
    /// 添加状态
    /// </summary>
    /// <param name="index"></param>
    /// <param name="state"></param>
    public void AddState(int index,StateBase state)
    {
        if(index >= stateCount || index < 0)
        {
            Debugger.LogError($"---------------{index}此状态id不合法------------");
            return;
        }
        if(StateArray[index] != null)
        {
            Debugger.LogError($"-----------{index}对应的状态已存在，请勿重复添加------------");
            return;
        }
        StateArray[index] = state;
    }

    /// <summary>
    /// 获取当前状态
    /// </summary>
    /// <returns></returns>
    public StateBase GetCurrentState()
    {
        if (StateArray[currentStateId] == null)
        {
            Debugger.LogError($"-----------{currentStateId}对应的状态不存在------------");
            return null;
        }
        return StateArray[currentStateId];
    }

    /// <summary>
    /// 移除状态
    /// </summary>
    /// <param name="index"></param>
    public void RemoveState(int index)
    {
        if (index >= stateCount || index < 0)
        {
            Debugger.LogError($"---------------{index}此状态id不合法------------");
            return;
        }
        if (StateArray[index] == null)
        {
            Debugger.LogError($"-----------{index}对应的状态不存在------------");
            return;
        }

        StateArray[index] = null;
    }

    /// <summary>
    /// 清空状态
    /// </summary>
    public void Clear()
    {
        StateArray = null;
    }
}
