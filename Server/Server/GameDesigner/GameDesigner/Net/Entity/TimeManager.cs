using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Net.Entity
{
    public class TimeManager
    {
        static bool run;
        struct TimeAct
        {
            public DateTime time;
            public Action action;
        }
        static List<TimeAct> timeActs = new List<TimeAct>();

        public static void Delay(int millisecondsTimeout, Action action)
        {
            if(run){
                timeActs.Add(new TimeAct(){
                    time = DateTime.Now.AddMilliseconds(millisecondsTimeout),
                    action = action,
                });
                return;
            }
            run = true;
            Task.Run(()=> {
                while(true){
                    Thread.Sleep(1);
                    for(int i = 0; i < timeActs.Count; i++) {
                        if(DateTime.Now > timeActs[i].time) {
                            timeActs[i].action();
                            timeActs.RemoveAt(i);
                        }
                    }
                }
            });
        }
    }
}
