/*
 * ===================================================================
 * 
 *          projectName:
 *              项目名称:拱猪
 *                  
 *          title:
 *              标题:事件系统
 *          
 *          description:
 *              功能描述:实现事件触发和监听
 *              
 *          author:
 *              作者:照着教程敲出bug的程序员
 * 
 * ===================================================================
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameSys
{
    class EventSys
    {
        private static EventSys instance;
        public static EventSys Instance
        {
            get
            {
                if (instance == null)
                    instance = new EventSys();
                return instance;
            }
        }

        Dictionary<string, Action<Object[]>> eventsParamDic = new Dictionary<string, Action<Object[]>>();
        Dictionary<string, Action> eventsDic = new Dictionary<string, Action>();

        public void AddListener(string eventName, Action<Object[]> action)
        {
            //Console.WriteLine("添加监听");
            if (!eventsParamDic.TryGetValue(eventName, out Action<Object[]> _event))
                eventsParamDic.Add(eventName, action);
            else
                _event += action;
        }

        public void AddListener(string eventName,Action action)
        {
            if (!eventsDic.TryGetValue(eventName, out Action _event))
                eventsDic.Add(eventName, action);
            else
                _event += action;
        }

        public void Trigger(string eventName,Object[] _params)
        {
            //Console.WriteLine("触发"+ eventsDic.ContainsKey(eventName));
            if (!eventsParamDic.TryGetValue(eventName, out Action<Object[]> action))
                return;
            action?.Invoke(_params);
        }

        public void Trigger(string eventName)
        {
            if (!eventsDic.TryGetValue(eventName,out Action action))
                return;
            action?.Invoke();
        }

        public void RemoveListener(string eventName)
        {
            if (eventsParamDic.ContainsKey(eventName))
                eventsParamDic.Remove(eventName);
            if (eventsDic.ContainsKey(eventName))
                eventsDic.Remove(eventName);
        }

        public void ClearListener()
        {
            eventsParamDic.Clear();
            eventsDic.Clear();
        }
    
    }
}
