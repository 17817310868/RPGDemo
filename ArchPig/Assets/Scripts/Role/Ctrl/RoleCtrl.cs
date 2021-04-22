/*
 * ===================================================================
 * 
 *          projectName:
 *              项目名称:拱猪
 *                  
 *          title:
 *              标题:控制系统
 *          
 *          description:
 *              功能描述:控制角色，实现鼠标与角色交互，向服务端发送控制请求
 *              
 *          author:
 *              作者:
 * 
 * ===================================================================
 */

//using Lua;
using LuaFramework;
using LuaInterface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RoleCtrl
{
    private static RoleCtrl instance;
    public static RoleCtrl Instance
    {
        get
        {
            if (instance == null)
                instance = new RoleCtrl();
            return instance;
        }
    }

    public bool isBattle = false;
    //bool isCtrl = false;
    public float runTime = 5;

    public void Update()
    {
        if(runTime < 0)
        {
            isBattle = true;
            Net.Client.NetBehaviour.Send("MeetMonster");
            runTime = 5;
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            //if (!isCtrl)
            //    return;
            //if (EventSystem.current.IsPointerOverGameObject())
                //return;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Application.isEditor)
            {
                if (EventSystem.current.IsPointerOverGameObject())
                    return;
            }
            else
            {
                if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                    return;
            }
            if (isBattle)
                return;
            LuaManager luaMgr = AppFacade.Instance.GetManager<LuaManager>(ManagerName.Lua);
            
            Physics.Raycast(ray, out RaycastHit hitInfo);
            Debug.Log(hitInfo.collider.tag);
            //Debugger.Log(hitInfo.collider.tag);
            switch (hitInfo.collider.tag)
            {
                case "Npc":
                    if (Vector3.Distance(RoleMgr.Instance.GetMainRole().transform.position, hitInfo.collider
                        .gameObject.transform.position) > 1)
                    {
                        Net.Client.NetBehaviour.Send("Move", new C2S_MoveInfo(hitInfo.point));
                    }
                    else
                    {
                        NPCManager.Instance.ClickNpc(hitInfo.collider.gameObject);
                    }
                    break;
                case "Player":
                    luaMgr.GetFunction("UIMgr.Trigger").LazyCall(luaMgr.GetTable("UIMgr"), "ShowOtherRole",
                        RoleMgr.Instance.GetRoleGuid(hitInfo.collider.gameObject));
                    break;
                case "Ground":
                    if ((Vector3.Distance(RoleMgr.Instance.GetMainRole().transform.position, hitInfo.point) > 0.5f))
                        Net.Client.NetBehaviour.Send("Move", new C2S_MoveInfo(hitInfo.point));
                    break;
            }
            //if (Physics.Raycast(ray, out RaycastHit Info, 100f, 1 << LayerMask.NameToLayer("Npc")))
            //{
                
            //    return;
            //}
            //if (Physics.Raycast(ray, out RaycastHit hInfo, 100f , 1 << LayerMask.NameToLayer("Player"))){
            //    LuaManager luaMgr = AppFacade.Instance.GetManager<LuaManager>(ManagerName.Lua);
            //    luaMgr.GetFunction("UIMgr.Trigger").LazyCall(luaMgr.GetTable("UIMgr"),"ShowOtherRole",
            //        RoleMgr.Instance.GetRoleGuid(hInfo.collider.gameObject));
            //    return;
            //}

            //if (!Physics.Raycast(ray, out RaycastHit hitInfo, 100f,1 << LayerMask.NameToLayer("Ground")))
            //    return;
            //if (!(Vector3.Distance(RoleMgr.Instance.GetMainRole().transform.position, hitInfo.point) > 0.5f))
            //    return;
            //BaseDataMgr.Instance.GetBaseData(RoleMgr.Instance.MainRoleId).ChangeTargetPosition(hitInfo.point);

            //向服务端发送移动请求
            
            //StateMgr.Instance.ChangeState(RoleMgr.Instance.MainRoleId, (int)RoleState.Run);
            
        }

        
    }
    //public void ChangeCanCtrl()
    //{
    //    isCtrl = true;
    //}
}
