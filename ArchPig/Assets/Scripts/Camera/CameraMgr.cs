using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraState { 
    None,
    Follow,
    Fight
}

public class CameraMgr 
{
    private static CameraMgr instance;
    public static CameraMgr Instance
    {
        get
        {
            if (instance == null)
                instance = new CameraMgr();
            return instance;
        }
    }

    public Transform camera;

    private Vector3 followDelta;

    public Transform followTarget;

    public CameraState state = CameraState.None;

    public void Follow()
    {
        if (state != CameraState.Follow)
            return;
        if (followTarget == null)
            return;
        if (camera == null)
            return;
        camera.position = followTarget.position + followDelta;

    }

    
    void Fight()
    {
        if (camera == null)
            return;
        //此坐标应从配置表读取
        //camera.position = new Vector3(0, 0, 0);
    }

    public void ChangeFollow()
    {
        if (state == CameraState.Follow)
            return;
        state = CameraState.Follow;
        camera.GetComponent<Camera>().orthographic = false;
        //从配置表读取跟随插值
        followDelta = new Vector3(4.74f, 5.6f, -0.21f);
        //设置角度向量
        camera.localEulerAngles = new Vector3(35.96f, -90.825f, 0f);
    }

    public void ChangeFight()
    {
        if (state == CameraState.Fight)
            return;
        state = CameraState.Fight;
        camera.position = new Vector3(-4.62f, 9.88f, 91.74f);
        camera.localEulerAngles = new Vector3(40.33f, 39.418f, -1.615f);
        camera.GetComponent<Camera>().orthographic = true;
    }

    //public void Init(Vector3 dir)
    //{
    //    switch (state)
    //    {
    //        case CameraState.Follow:
    //            camera.GetComponent<Camera>().orthographic = false;
    //            //从配置表读取跟随插值
    //            followDelta = new Vector3(4.74f, 5.6f, -0.21f);
    //            //设置角度向量
    //            camera.localEulerAngles = dir;
    //            Follow();
    //            break;
    //        case CameraState.Fight:
    //            //设置角度向量
    //            camera.position = new Vector3(-4.62f, 9.88f, 91.74f);
    //            camera.localEulerAngles = new Vector3(40.33f, 39.418f, -1.615f);
    //            camera.GetComponent<Camera>().orthographic = true;
    //            Fight();
    //            break;
    //        default:
    //            break;

    //    }
    //}
}
