using LuaFramework;
using LuaInterface;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public enum NPCType
{
    None = 0,
    Panel,
    Task
}

public class NPCManager 
{
    private static NPCManager instance;
    public static NPCManager Instance
    {
        get
        {
            if (instance == null)
                instance = new NPCManager();
            return instance;
        }
    }

    Dictionary<int, GameObject> NPCsDic = new Dictionary<int, GameObject>();
    Dictionary<GameObject, int> idsDic = new Dictionary<GameObject, int>();

    public int NPCCount = 2;

    public void Init()
    {
        for(int i = 1; i <= NPCCount; i++)
        {
            NPCConfig config = ConfigManager.Instance.GetConfig<NPCConfig>(i);
            LuaHelper.GetObjectPoolManager().Get(config.model, (model) =>
            {
                NPCsDic.Add(config.id, model);
                idsDic.Add(model, config.id);
                model.transform.position = new Vector3(config.posX, config.posY, config.posZ);
            });
        }
    }

    public int GetNpcId(GameObject gameObject)
    {
        if (idsDic.TryGetValue(gameObject, out int npcId))
            return npcId;
        return 0;
    }

    public void ClickNpc(GameObject gameObject)
    {
        LuaManager luaMgr = AppFacade.Instance.GetManager<LuaManager>(ManagerName.Lua);
        luaMgr.GetFunction("TalkCtrl.ShowTask").LazyCall(luaMgr.GetTable("TalkCtrl"),
             GetNpcId(gameObject));
        if (ConfigManager.Instance.GetConfig<NPCConfig>(GetNpcId(gameObject)).panelName == "-1")
            return;
        AppFacade.Instance.GetManager<PanelManager>(ManagerName.Panel).OpenPanel(
            ConfigManager.Instance.GetConfig<NPCConfig>(GetNpcId(gameObject)).panelName, 2, 2);
    }

    public void ClearTaskIcon()
    {
        for (int i = 1; i <= NPCCount; i++)
        {
            NPCConfig config = ConfigManager.Instance.GetConfig<NPCConfig>(i);
            if (config.type == (int)NPCType.Task)
                NPCsDic[i].transform.Find("Canvas/Auto_task").gameObject.SetActive(false);
        }
    }

    public void HideTaskIcon(int npcId)
    {
        //LuaInterface.Debugger.Log("不显示");
        NPCsDic[npcId].transform.Find("Canvas/Auto_task").gameObject.SetActive(false);
    }

    public void AddTaskIcon(int npcId, string icon)
    {
        //LuaInterface.Debugger.Log("显示");
        AppFacade.Instance.GetManager<ResourceManager>(ManagerName.Resource).
            LoadAsset<Sprite>("taskicon", new string[] { icon }, (obj) =>
             {
                 NPCsDic[npcId].transform.Find("Canvas/Auto_task").gameObject.SetActive(true);
                 NPCsDic[npcId].transform.Find("Canvas/Auto_task").GetComponent<Image>().sprite =
                 obj[0] as Sprite;
             });
    }

    //public void UpdateTaskIcon(Dictionary<int,int> conductTasks,List<int> acceptableTasks)
    //{
    //    //Dictionary<int,int> acceptableTasks =  AcceptableTasks.ToDictTable<int,int>().ToDictionary();
    //    for(int i = 0; i < acceptableTasks.Count; i++)
    //    {
    //        LuaInterface.Debugger.Log(acceptableTasks[i]);
    //    }
         
    //}

}
