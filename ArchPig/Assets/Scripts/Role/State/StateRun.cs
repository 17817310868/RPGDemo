/*
 * ===================================================================
 * 
 *          projectName:
 *              项目名称:拱猪
 *                  
 *          title:
 *              标题:移动状态
 *          
 *          description:
 *              功能描述:移动状态的进入，持续，退出具体逻辑
 *              
 *          author:
 *              作者:
 * 
 * ===================================================================
 */

using System.Diagnostics;
using UnityEngine;
using UnityEngine.AI;

public class StateRun : StateBase
{
    private float speed;
    private Vector3 target;
    private NavMeshAgent navMesh;
    public StateRun(Animation animation,CharacterController controller,string roleId) : base(animation, controller,roleId)
    {
        
    }

    public void Init(Vector3 target,float speed)
    {
        this.speed = speed;
        this.target = target;
        navMesh = animation.transform.GetComponent<NavMeshAgent>();
        //navMesh.speed = speed;
    }

    public override void StateEnter()
    {
        animation.CrossFade("run");
        navMesh.enabled = true;
        navMesh.speed = speed/30;
        navMesh.acceleration = speed / 30;
        navMesh.SetDestination(target);
        //animation.SetInteger("stateId", (int)RoleState.Run);
        //animation.transform.LookAt(target);
        //animation.transform.forward = Vector3.ProjectOnPlane(target - animation.transform.position, Vector3.up);

    }

    public override void StateStay()
    {
        if (Vector3.Distance(animation.transform.position, target) < 1f)
            StateEnd();
        if (animation.gameObject != RoleMgr.Instance.GetMainRole())
            return;
        if (animation.transform.position.x > -50 && !RoleCtrl.Instance.isBattle )
        {
            RoleCtrl.Instance.runTime -= Time.deltaTime;
        }
        //controller.SimpleMove(controller.transform.forward * speed * Time.deltaTime);
    }

    public override void StateEnd()
    {
        navMesh.enabled = false;
        StateMgr.Instance.ChangeState(RoleId, (int)RoleState.Idle);
    }
}
