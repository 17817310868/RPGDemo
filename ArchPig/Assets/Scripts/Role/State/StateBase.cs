/*
 * ===================================================================
 * 
 *          projectName:
 *              项目名称:拱猪
 *                  
 *          title:
 *              标题:状态基类
 *          
 *          description:
 *              功能描述:状态基类，定义状态进入，持续，退出的虚方法，等待子类重写，
 *              
 *          author:
 *              作者:
 * 
 * ===================================================================
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateBase 
{
    protected Animation animation;
    protected CharacterController controller;
    private string roleId;
    protected string RoleId
    {
        get
        {
            return roleId;
        }
    }
    public StateBase(Animation animator,CharacterController controller,string roleId)
    {
        this.animation = animator;
        this.controller = controller;
        this.roleId = roleId;
    }
    public virtual void StateEnter()
    {

    }

    public virtual void StateStay()
    {

    }

    public virtual void StateEnd()
    {

    }
}
