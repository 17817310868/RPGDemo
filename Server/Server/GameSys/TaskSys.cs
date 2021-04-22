/*
 * ===================================================================
 * 
 *          projectName:
 *              项目名称:拱猪
 *                  
 *          title:
 *              标题:任务系统
 *          
 *          description:
 *              功能描述:处理任务
 *              
 *          author:
 *              作者:照着教程敲出bug程序员
 * 
 * ===================================================================
 */

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameSys
{

    public enum TaskType
    {
        None = 0,
        Talk,
        Goods,
        Fight
    }

    class TaskSys
    {

        private static TaskSys instance;
        public static TaskSys Instance
        {
            get
            {
                if (instance == null)
                    instance = new TaskSys();
                return instance;
            }
        }

        public TaskSys()
        {
            EventSys.Instance.AddListener("KillMonster", (objs) =>
            {
                Player player = objs[0] as Player;
                int monsterId = (int)objs[1];
                byte count = (byte)objs[2];
                List<int> conductTasks = player.mainRole.Tasks.conductTasks.Keys.ToList();
                foreach (int taskId in conductTasks)
                {
                    TaskConfig config = ConfigSys.Instance.GetConfig<TaskConfig>(taskId);
                    if (config.type != (int)TaskType.Fight)
                        continue;
                    if (config.paramId != monsterId)
                        continue;
                    player.mainRole.Tasks.conductTasks[taskId] += count;
                }
                UpdateConductTask(player);
            });
            EventSys.Instance.AddListener("AddItem", (objs) =>
             {
                 Player player = objs[0] as Player;
                 int itemId = (int)objs[1];
                 int count = (int)objs[2];
                 List<int> conductTasks = player.mainRole.Tasks.conductTasks.Keys.ToList();
                 foreach (int taskId in conductTasks)
                 {
                     TaskConfig config = ConfigSys.Instance.GetConfig<TaskConfig>(taskId);
                     if (config.type != (int)TaskType.Goods)
                         continue;
                     if (config.paramId != itemId)
                         continue;
                     player.mainRole.Tasks.conductTasks[taskId] += count;
                 }
                 UpdateConductTask(player);
             });
            EventSys.Instance.AddListener("ReduceItem", (objs) =>
            {
                Player player = objs[0] as Player;
                int itemId = (int)objs[1];
                int count = (int)objs[2];
                List<int> conductTasks = player.mainRole.Tasks.conductTasks.Keys.ToList();
                foreach (int taskId in conductTasks)
                {
                    TaskConfig config = ConfigSys.Instance.GetConfig<TaskConfig>(taskId);
                    if (config.type != (int)TaskType.Goods)
                        continue;
                    if (config.paramId != itemId)
                        continue;
                    player.mainRole.Tasks.conductTasks[taskId] -= count;
                }
                UpdateConductTask(player);
            });
        }

        int taskSum = 3;  //任务总数

        /// <summary>
        /// 获取该玩家所有任务信息
        /// </summary>
        /// <param name="player"></param>
        public void InitTaskInfo(Player player)
        {
            TaskInfoData taskInfoData = DBSys.Instance.GetData<TaskInfoData>("account", player.account);
            if(taskInfoData != null)
            {
                player.mainRole.Tasks.completeTasks = taskInfoData.completeTasks;
                player.mainRole.Tasks.conductTasks = taskInfoData.conductTasks;
            }

            //else
            //conductTask = player.mainRole.Tasks.conductTasks;
            //存储玩家目前可接受的任务
            List<int> acceptableTasks = new List<int>();
            for(int i = 1;i <= taskSum; i++)
            {
                //遍历任务配置表
                TaskConfig config = ConfigSys.Instance.GetConfig<TaskConfig>(i);
                //如果玩家未达到任务等级要求，跳过
                if (player.mainRole.Level < config.level)
                    continue;
                //如果玩家已经完成了该任务，跳过
                if (player.mainRole.Tasks.completeTasks.Contains(config.id))
                    continue;
                //如果玩家未完成该任务的前置任务，跳过
                if (config.premise != 0)
                    if (!player.mainRole.Tasks.completeTasks.Contains(config.premise))
                        continue;
                //将该任务添加至可接受任务列表
                acceptableTasks.Add(config.id);

            }

            ServerSys.Instance.Send(player, "CallLuaFunction", "MainUICtrl.UpdateTaskInfo",
                new S2C_AllTaskInfo(player.mainRole.Tasks.conductTasks, acceptableTasks));

        }

        /// <summary>
        /// 向客户端刷新当前进行的任务和进度
        /// </summary>
        /// <param name="player"></param>
        void UpdateConductTask(Player player)
        {
            ServerSys.Instance.Send(player, "CallLuaFunction", "MainUICtrl.UpdateConductTask",
                new S2C_ConductTasks(player.mainRole.Tasks.conductTasks));
        }

        /// <summary>
        /// 向客户端刷洗您当前可接受的任务
        /// </summary>
        /// <param name="player"></param>
        void UpdateAcceptableTasks(Player player)
        {
            List<int> acceptableTasks = new List<int>();
            for (int i = 1; i <= taskSum; i++)
            {
                //遍历任务配置表
                TaskConfig config = ConfigSys.Instance.GetConfig<TaskConfig>(i);
                //如果玩家未达到任务等级要求，跳过
                if (player.mainRole.Level < config.level)
                    continue;
                //如果玩家已经完成了该任务，跳过
                if (player.mainRole.Tasks.completeTasks.Contains(config.id))
                    continue;
                //如果玩家未完成该任务的前置任务，跳过
                if (config.premise != 0)
                    if (!player.mainRole.Tasks.completeTasks.Contains(config.premise))
                        continue;
                //将该任务添加至可接受任务列表
                acceptableTasks.Add(config.id);

            }
            //Console.WriteLine(acceptableTasks.Count);
            ServerSys.Instance.Send(player, "CallLuaFunction", "MainUICtrl.UpdateAcceptableTasks",
                new S2C_AcceptableTasks(acceptableTasks));
        }

        /// <summary>
        /// 接收任务
        /// </summary>
        /// <param name="player"></param>
        /// <param name="C2S_acceptTask"></param>
        public void AcceptTask(Player player,C2S_AcceptTask C2S_acceptTask)
        {
            int taskId = C2S_acceptTask.taskId;
            if (player.mainRole.Tasks.completeTasks.Contains(taskId))
                return;
            if (player.mainRole.Tasks.conductTasks.ContainsKey(taskId))
                return;
            TaskConfig config = ConfigSys.Instance.GetConfig<TaskConfig>(taskId);
            if (player.mainRole.Level < config.level)
                return;
            if (config.premise != 0)
                if (!player.mainRole.Tasks.completeTasks.Contains(config.premise))
                    return;
            int progress = 0;
            if (config.type == (int)TaskType.Goods)
            {
                if (player.mainRole.Inventory.GetItemsGuid(config.paramId) != null)
                {
                    string[] Guids = player.mainRole.Inventory.GetItemsGuid(config.paramId);
                    ItemInfo item = player.mainRole.Inventory.GetItem(Guids[0]) as ItemInfo;
                    progress = item.Count;
                }
            }

            player.mainRole.Tasks.conductTasks.Add(taskId, progress);
            ServerSys.Instance.Send(player, "CallLuaFunction", "MainUICtrl.DeleteAcceptableTask",
                new S2C_AcceptTask(taskId,progress));

        }

        /// <summary>
        /// 提交任务
        /// </summary>
        /// <param name="player"></param>
        /// <param name="C2S_completeTask"></param>
        public void SubmitTask(Player player, C2S_CompleteTask C2S_completeTask)
        {
            int taskId = C2S_completeTask.taskId;
            if (!player.mainRole.Tasks.conductTasks.ContainsKey(taskId))
                return;

            TaskConfig config = ConfigSys.Instance.GetConfig<TaskConfig>(taskId);

            if (player.mainRole.Tasks.conductTasks[taskId] < config.count)
                return;

            player.mainRole.Tasks.conductTasks.Remove(taskId);
            player.mainRole.Tasks.completeTasks.Add(taskId);

            ServerSys.Instance.Send(player, "CallLuaFunction", "MainUICtrl.DeleteConductTask",
                new S2C_CompleteTask(taskId));

            if (config.item != "-1")
            {
                string[] items = config.item.Split('|');
                int[] itemsId = new int[items.Length];
                int[] counts = new int[items.Length];
                for (int i = 0; i < items.Length; i++)
                {
                    itemsId[i] = int.Parse(items[i]);
                    counts[i] = 1;
                }
                InventorySys.Instance.AddItems(player, itemsId, counts);
            }
            if (config.equip != "-1")
            {
                string[] equips = config.equip.Split('|');
                int[] equipsId = new int[equips.Length];
                for (int i = 0; i < equips.Length; i++)
                {
                    equipsId[i] = int.Parse(equips[i]);
                }
                InventorySys.Instance.AddEquips(player, equipsId);
            }
            if (config.gem != "-1")
            {
                string[] gems = config.gem.Split('|');
                int[] gemsId = new int[gems.Length];
                int[] counts = new int[gems.Length];
                for (int i = 0; i < gems.Length; i++)
                {
                    gemsId[i] = int.Parse(gems[i]);
                    counts[i] = 1;
                }
                InventorySys.Instance.AddGems(player, gemsId, counts);
            }

            player.mainRole.AddSilver(config.silver);
            player.mainRole.AddGold(config.gold);
            player.mainRole.AddExperience(config.experience);
            RoleSys.Instance.UpdatePlayerMainRole(player);
            UpdateAcceptableTasks(player);

        }

        /// <summary>
        /// 保存任务数据
        /// </summary>
        /// <param name="player"></param>
        public void SaveTask(Player player)
        {
            DBSys.Instance.DeleteAllDatas<TaskInfoData>("account", player.account);
            DBSys.Instance.InsertData<TaskInfoData>(new TaskInfoData(
                player.account, player.mainRole.Tasks.completeTasks, player.mainRole.Tasks.conductTasks));
        }

    }

    public class TaskInfo
    {
        public TaskInfo()
        {
            completeTasks = new List<int>();
            conductTasks = new Dictionary<int, int>();
        }
        public List<int> completeTasks;  //用于存储已完成的任务的id
        public Dictionary<int, int> conductTasks;  //用于存储正在进行的任务所对应的任务进度

    }

}
