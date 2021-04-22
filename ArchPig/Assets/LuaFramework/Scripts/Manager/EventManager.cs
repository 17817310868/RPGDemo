using LuaInterface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 空接口
/// </summary>
public interface IEventAction
{

}

// 给带有泛型参数的委托封装一层，继承没有泛型的空接口
// 在后面的使用中通过父类转子类转化为泛型类
// 封装含有参数的委托
public class EventAction<T> : IEventAction
{
    public UnityAction<T> actions;
    public EventAction(UnityAction<T> action)
    {
        actions += action;
    }
}

//  封装不含参数的委托
public class EventAction : IEventAction
{
    public UnityAction actions;
    public EventAction(UnityAction action)
    {
        actions += action;
    }
}


/// <summary>
/// 事件管理类
/// </summary>
public class EventManager 
{
    private static EventManager instance;
    public static EventManager Instance {
        get
        {
            if (instance == null)
                instance = new EventManager();
            return instance;
        }
    }
    /// <summary>
    /// 存储所有监听的事件及事件对应的所有行为
    /// </summary>
    public Dictionary<string, IEventAction> eventDic = new Dictionary<string, IEventAction>();

    /// <summary>
    /// 添加监听事件
    /// </summary>
    /// <param name="T">传递的参数类型</param>
    /// <param name="name">监听的事件的名字</param>
    /// <param name="action">所需要执行的行为</param>
    public void AddListener<T>(string name ,UnityAction<T> action)
    {
        IEventAction eventAction = null;
        if(eventDic.TryGetValue(name,out eventAction))
        {
            (eventAction as EventAction<T>).actions += action;
        }
        else
        {
            eventDic.Add(name, new EventAction<T>(action));
        }
    }

    /// <summary>
    /// 添加监听事件
    /// </summary>
    /// <param name="name">监听的事件的名字</param>
    /// <param name="action">所需要执行的行为</param>
    public void AddListener(string name, UnityAction action)
    {
        IEventAction eventAction = null;
        if(eventDic.TryGetValue(name,out eventAction))
        {
            (eventAction as EventAction).actions += action;
        }
        else
        {
            eventDic.Add(name, new EventAction(action));
        }
    }

    /// <summary>
    /// 添加监听事件
    /// </summary>
    /// <param name="name">监听的事件的名字</param>
    /// <param name="action">所需要执行的行为</param>
    public void AddListener(string name, UnityAction<string> action)
    {
        AddListener<string>(name, action);
    }


    /// <summary>
    /// 移除某个监听事件的某一个行为
    /// </summary>
    /// <typeparam name="T">传递的参数类型</typeparam>
    /// <param name="name"></param>
    /// <param name="action"></param>
    public void RemoveListener<T>(string name, UnityAction<T> action)
    {
        IEventAction eventAction = null;
        if(eventDic.TryGetValue(name,out eventAction))
        {
            if ((eventAction as EventAction<T>).actions == null)
            {
                Debugger.LogError($"-----------{name}监听的事件为空---------------");
                return;
            }
            (eventAction as EventAction<T>).actions -= action;
        }
    }

    /// <summary>
    /// 移除某个监听事件的某一个行为
    /// </summary>
    /// <param name="name"></param>
    /// <param name="action"></param>
    public void RemoveListener(string name, UnityAction action)
    {
        IEventAction eventAction = null;
        if(eventDic.TryGetValue(name,out eventAction))
        {
            if ((eventAction as EventAction).actions == null)
            {
                Debugger.LogError($"-----------{name}监听的事件为空---------------");
                return;
            }
            (eventAction as EventAction).actions -= action;
        }
    }

    /// <summary>
    /// 移除某个监听
    /// </summary>
    /// <param name="name"></param>
    public void RemoveListener(string name)
    {
        if (eventDic.ContainsKey(name))
            eventDic.Remove(name);
    }

    /// <summary>
    /// 清空事件管理器的所有事件及行为
    /// </summary>
    public void ClearEvent()
    {
        eventDic = null;
    }

    /// <summary>
    /// 触发事件
    /// </summary>
    /// <param name="T">传递的参数类型</param>
    /// <param name="name">监听的事件的名字</param>
    /// <param name="obj">向监听事件者传递的参数</param>
    public void EventTrigger<T>(string name , T obj)
    {
        IEventAction eventAction = null;
        if(eventDic.TryGetValue(name,out eventAction))
        {
            (eventAction as EventAction<T>).actions?.Invoke(obj);
        }
    }

    /// <summary>
    /// 触发事件
    /// </summary>
    /// <param name="name">监听的事件的名字</param>
    public void EventTrigger(string name)
    {
        IEventAction eventAction = null;
        if (eventDic.TryGetValue(name, out eventAction))
        {
            (eventAction as EventAction).actions?.Invoke();
        }
    }

    /// <summary>
    /// 触发事件
    /// </summary>
    /// <param name="name">监听的事件的名字</param>
    /// <param name="str">向监听事件者传递的参数</param>
    public void EventTrigger(string name,string str)
    {
        EventTrigger<string>(name, str);
    }
}
