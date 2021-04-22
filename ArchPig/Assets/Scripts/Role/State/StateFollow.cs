using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateFollow : StateBase
{
    private Transform target;
    private float speed;
    public StateFollow(Animation animation,CharacterController controller,string roleId) : base(animation, controller,roleId)
    {

    }

    public void Init(Transform target,float speed)
    {
        this.target = target;
        this.speed = speed;
    }

    public override void StateEnter()
    {
        
    }

    public override void StateStay()
    {
        if(Vector3.Distance(controller.transform.position,target.position) > 0.8f)
        {
            animation.CrossFade("run");
            controller.transform.LookAt(target);
            controller.SimpleMove(controller.transform.forward * speed * Time.deltaTime);
        }
        else
        {
            animation.CrossFade("stand");
        }
    }
}
