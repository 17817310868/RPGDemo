using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using LuaFramework;
using LuaInterface;

public class ConfigManager
{

    private static ConfigManager instance;
    public static ConfigManager Instance
    {
        get
        {
            if (instance == null)
                instance = new ConfigManager();
            return instance;
        }
    }

    //key:表名 val 表数据列表
    Dictionary<string, List<ConfigClass>> dic = new Dictionary<string, List<ConfigClass>>();
    Dictionary<string, Dictionary<int, ConfigClass>> configsDic = new Dictionary<string, Dictionary< int, ConfigClass>>();
    public void InitConfig()
    {
        LuaHelper.GetGameManager().StartCoroutine(ReadConfigFile("ProfessionConfig.msconfig"));
        LuaHelper.GetGameManager().StartCoroutine(ReadConfigFile("SchoolConfig.msconfig"));
        LuaHelper.GetGameManager().StartCoroutine(ReadConfigFile("EquipConfig.msconfig"));
        LuaHelper.GetGameManager().StartCoroutine(ReadConfigFile("SkillConfig.msconfig"));
        LuaHelper.GetGameManager().StartCoroutine(ReadConfigFile("BuffConfig.msconfig"));
        LuaHelper.GetGameManager().StartCoroutine(ReadConfigFile("NPCConfig.msconfig"));
        LuaHelper.GetGameManager().StartCoroutine(ReadConfigFile("MonsterConfig.msconfig"));
    }

    //public ProfessionConfig GetProfessionConfig(int id)
    //{
        

    //}

    public SchoolConfig GetSchoolConfig(int id)
    {
        return GetConfig<SchoolConfig>(id);
    }

    public T GetConfig<T>(int id ) where T : ConfigClass
    {
        string keyName = typeof(T).Name + ".msconfig";
        if (!configsDic.TryGetValue(keyName, out Dictionary<int, ConfigClass> configs))
        {
            Debugger.LogError($"----------------找不到{typeof(T).Name}表----------");
            return null;
        }

        if (!configsDic[keyName].TryGetValue(id, out ConfigClass config))
        {
            Debugger.LogError($"----------------找不到{id}对应的配置----------");
            return null;
        }

        //ProfessionConfig professionConfig = configList[id] as ProfessionConfig;
        T tConfig = config as T;
        if (tConfig == null)
        {
            Debugger.LogError($"----------------找不到{id}对应的配置----------");
            return null;
        }
        return tConfig;
    }

    public EquipConfig GetEquipConfig(int id)
    {
        return GetConfig<EquipConfig>(id);
    }

    IEnumerator ReadConfigFile(string filename)
    {
        string filepath = ExcelTool.GetConfigFilePath(filename);

        WWW www = new WWW(filepath);
        yield return www;
        while (www.isDone == false) yield return null;
        if (www.error == null)
        {
            byte[] data = www.bytes;
            //List<ConfigClass> datalist = (List<ConfigClass>)ExcelTool.DeserializeObj(data);
            List<ConfigClass> datalist = (List<ConfigClass>)ExcelTool.DeserializeObj(data);
            dic.Add(filename, datalist);
            foreach(ConfigClass config in datalist)
            {
                if (!configsDic.TryGetValue(filename, out Dictionary<int, ConfigClass> configs))
                    configsDic.Add(filename, new Dictionary<int, ConfigClass>());
                if (configsDic[filename].TryGetValue(config.id, out ConfigClass newConfig))
                    Debugger.LogError($"-------------重复添加索引为{config.id}的数据------------");
                configsDic[filename].Add(config.id,config);
            }
            Debugger.Log($"----------------{filename}表加载完成----------------");
        }
        else
        {
            //GameLogTools.SetText("wwwError<<" + www.error + "<<" + filepath);
            Debug.Log("wwwError<<" + www.error + "<<" + filepath);
        }
        
    }
}


