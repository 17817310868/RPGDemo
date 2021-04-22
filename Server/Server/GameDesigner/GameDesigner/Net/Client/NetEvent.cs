namespace Net.Client
{
    using UnityEngine;
    using System.Collections;
    using Net.Share;
    using System.Collections.Generic;
    using System;
    using System.Threading;
    using System.Reflection;
    using System.Collections.Concurrent;

    /// <summary>
    /// 网络事件
    /// </summary>
    public sealed class NetEvent : MonoBehaviour
    {
        public class TaskEvent
        {
            public Func<bool> func;
            public DateTime time;
            public int period;
        }

        private static NetEvent ins;
        private static Thread thread, thread1, thread2;
        private static List<Func<bool>> updateEvents = new List<Func<bool>>();
        private static List<TaskEvent> taskEvents = new List<TaskEvent>();
        private static List<Func<bool>> taskEvents1 = new List<Func<bool>>();

        /// <summary>
        /// 添加任务事件，在Undate函数执行， 当任务返回true时结束任务， 返回flase时继续执行任务
        /// </summary>
        /// <param name="func">函数委托</param>
        public static void AddEvent(Func<bool> func)
        {
            updateEvents.Add(func);
            if (ins == null)
            {
                ins = FindObjectOfType<NetEvent>();
                ins = ins ?? (ins = new GameObject("NetworkEvent").AddComponent<NetEvent>());
            }
        }

        class Ev
        {
            public Action<object> func;
            public object obj;
        }

        private static List<Ev> updateEvents1 = new List<Ev>();

        /// <summary>
        /// 添加任务事件，在Undate函数执行， 当任务返回true时结束任务， 返回flase时继续执行任务
        /// </summary>
        /// <param name="func">函数委托</param>
        public static void AddUnityEvent(Action<object> func, object obj)
        {
            updateEvents1.Add(new Ev() { func = func, obj = obj });
            if (ins == null)
            {
                ins = FindObjectOfType<NetEvent>();
                ins = ins ?? (ins = new GameObject("NetworkEvent").AddComponent<NetEvent>());
            }
        }

        void Awake()
        {
            ins = this;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            for (int i = 0; i < updateEvents.Count; i++)
            {
                try
                {
                    if (updateEvents[i].Invoke())
                    {
                        updateEvents.RemoveAt(i);
                        i--;
                    }
                }
                catch
                {
                    updateEvents.RemoveAt(i);
                }
            }
            for (int i = 0; i < updateEvents1.Count; i++)
            {
                try
                {
                    updateEvents1[i].func(updateEvents1[i].obj);
                }
                finally
                {
                    updateEvents1.RemoveAt(i);
                    i = 0;
                }
            }
        }

        /// <summary>
        /// 添加任务到线程里,单个线程执行, 当条件到达可以返回true，否则你可以返回false继续执行任务
        /// </summary>
        /// <param name="func">函数委托</param>
        /// <param name="dueTime">开始调用时间(毫秒)</param>
        /// <param name="period">每隔毫秒执行一次</param>
        public static void AddTaskEvent(Func<bool> func, int dueTime, int period)
        {
            taskEvents.Add(new TaskEvent()
            {
                func = func,
                time = DateTime.Now.AddMilliseconds(dueTime),
                period = period
            });
            if (thread != null)
                return;
            thread = new Thread(() => {
                while (thread != null)
                {
                    Thread.Sleep(1);
                    TaskUpdate();
                }
            })
            { IsBackground = true };
            thread.Start();
        }

        //任务线程
        internal static void TaskUpdate()
        {
            for (int i = 0; i < taskEvents.Count; i++)
            {
                try
                {
                    if (taskEvents[i].time < DateTime.Now)
                    {
                        taskEvents[i].time = DateTime.Now.AddMilliseconds(taskEvents[i].period);
                        if (taskEvents[i].func.Target == null)
                        {
                            taskEvents[i] = null;
                            taskEvents.RemoveAt(i);
                            continue;
                        }
                        if (taskEvents[i].func.Target.Equals(null))
                        {
                            taskEvents[i] = null;
                            taskEvents.RemoveAt(i);
                            continue;
                        }
                        if (taskEvents[i].func.Invoke())
                        {
                            taskEvents[i] = null;
                            taskEvents.RemoveAt(i);
                            i--;
                        }
                    }
                }
                catch
                {
                    taskEvents[i] = null;
                    taskEvents.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// 添加任务到线程里,单个线程执行, 当条件到达可以返回true，否则你可以返回false继续执行任务
        /// </summary>
        /// <param name="func">函数委托</param>
        public static void AddTaskEvent(Func<bool> func)
        {
            taskEvents1.Add(func);
            if (thread1 != null)
                return;
            thread1 = new Thread(() => {
                while (thread1 != null)
                {
                    Thread.Sleep(1);
                    TaskUpdate01();
                }
            })
            { IsBackground = true };
            thread1.Start();
        }

        //任务线程
        internal static void TaskUpdate01()
        {
            for (int i = 0; i < taskEvents1.Count; i++)
            {
                try
                {
                    if (taskEvents1[i].Target == null)
                    {
                        taskEvents1[i] = null;
                        taskEvents1.RemoveAt(i);
                        continue;
                    }
                    if (taskEvents1[i].Target.Equals(null))
                    {
                        taskEvents1[i] = null;
                        taskEvents1.RemoveAt(i);
                        continue;
                    }
                    if (taskEvents1[i].Invoke())
                    {
                        taskEvents1[i] = null;
                        taskEvents1.RemoveAt(i);
                        i--;
                    }
                }
                catch
                {
                    taskEvents1[i] = null;
                    taskEvents1.RemoveAt(i);
                }
            }
        }

        private static ConcurrentQueue<Action> Tasks = new ConcurrentQueue<Action>();

        /// <summary>
        /// 运行单一线程池
        /// </summary>
        /// <param name="call"></param>
        public static void Run(Action call)
        {
            Tasks.Enqueue(call);
            if (thread2 != null)
            {
                string str = thread2.ThreadState.ToString();
                if (str.Contains("Abort") | str.Contains("Stop"))
                    goto GO;
                return;
            }
        GO: thread2 = new Thread(() => {
            while (thread2 != null)
            {
                Thread.Sleep(1);
                try
                {
                    if (Tasks.TryDequeue(out Action action))
                        action?.Invoke();
                }
                catch { }
            }
        })
        { IsBackground = true };
            thread2.Start();
        }

        internal static void AddEvent(Func<object, bool> p, List<Vector3> smoothNormals)
        {
            throw new NotImplementedException();
        }
    }
}