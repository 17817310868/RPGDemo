using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using LuaFramework;
using System.Diagnostics;

/// <summary>
/// 场景管理类
/// 封装加载场景api
/// </summary>
public class ScenesManager : Manager
{
    /// <summary>
    /// 开辟异步加载场景的协程
    /// </summary>
    /// <param name="name">场景名称</param>
    /// <param name="action">加载完所要执行的行为</param>
    public void ScenesLoadAsync(string name,UnityAction action = null)
    {
        if(action == null)
        {
            StartCoroutine(Loading(name));
            return;
        }
        StartCoroutine(Loading(name,action));
        //ManagerController.instance.StartCoroutine("Loading");
    }


    /// <summary>
    /// 异步加载场景，并触发加载场景事件
    /// </summary>
    /// <param name="name">场景名称</param>
    /// <param name="action">加载完所要执行的行为</param>
    /// <returns></returns>
    private IEnumerator Loading(string name,UnityAction action = null)
    {
        LuaManager lua = AppFacade.Instance.GetManager<LuaManager>(ManagerName.Lua);
        AsyncOperation async = SceneManager.LoadSceneAsync(name);
        //async.allowSceneActivation = false;
        while (!async.isDone)
        {
            lua.GetFunction("UIMgr.Trigger").LazyCall(lua.GetTable("UIMgr"),"UpdateLoading", async.progress);
            yield return 0;
                //async.progress;
            //EventManager.Instance.EventTrigger<float>("LoadScene", async.progress);
        }
        //async.allowSceneActivation = true;
        action?.Invoke();
    }

}

