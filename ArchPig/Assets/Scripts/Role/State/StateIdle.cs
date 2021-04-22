/*
 * ===================================================================
 * 
 *          projectName:
 *              项目名称:拱猪
 *                  
 *          title:
 *              标题:站立状态
 *          
 *          description:
 *              功能描述:站立状态的进入，持续，退出的具体逻辑
 *              
 *          author:
 *              作者:
 * 
 * ===================================================================
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateIdle : StateBase
{
    public StateIdle(Animation animation,CharacterController controller,string roleId) : base(animation, controller,roleId)
    {

    }

    public override void StateEnter()
    {
        animation.CrossFade("stand");
        //animator.SetInteger("stateId", (int)RoleState.Idle);
    }
}
