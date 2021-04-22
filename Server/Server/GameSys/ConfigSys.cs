/*
 * ===================================================================
 * 
 *          projectName:
 *              项目名称:拱猪
 *                  
 *          title:
 *              标题:配置表系统
 *          
 *          description:
 *              功能描述:管理游戏所有配置表
 *              
 *          author:
 *              作者:
 * 
 * ===================================================================
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class ConfigSys
    {
        private static ConfigSys instance;
        public static ConfigSys Instance
        {
            get
            {
                if (instance == null)
                    instance = new ConfigSys();
                return instance;
            }
        }

        Dictionary<string, List<ConfigClass>> dic = new Dictionary<string, List<ConfigClass>>();
        Dictionary<string, Dictionary<int, ConfigClass>> configsDic = new Dictionary<string, Dictionary<int, ConfigClass>>();

        public void Init()
        {
            ReadConfig("SchoolConfig");
            ReadConfig("ProfessionConfig");
            ReadConfig("BuffConfig");
            ReadConfig("SkillConfig");
            ReadConfig("EquipConfig");
            ReadConfig("ItemConfig");
            ReadConfig("FormulaConfig");
            ReadConfig("GemConfig");
            ReadConfig("TaskConfig");
            ReadConfig("MonsterConfig");
            ReadConfig("ExperienceConfig");
        }

        public void ExcelToByte()
        {
            ExcelTool.LoadData("SchoolConfig");
            ExcelTool.LoadData("ProfessionConfig");
            ExcelTool.LoadData("BuffConfig");
            ExcelTool.LoadData("SkillConfig");
            ExcelTool.LoadData("EquipConfig");
            ExcelTool.LoadData("ItemConfig");
            ExcelTool.LoadData("FormulaConfig");
            ExcelTool.LoadData("GemConfig");
            ExcelTool.LoadData("TaskConfig");
            ExcelTool.LoadData("MonsterConfig");
            ExcelTool.LoadData("ExperienceConfig");
        }

        public int GetConfigLength(string configName)
        {
            if (!configsDic.TryGetValue(configName, out Dictionary<int, ConfigClass> configs))
            {
                //Debugger.LogError($"----------------找不到ProfessionConfig表----------");
                return 0;
            }
            return configs.Count;
        }

        public T[] GetAllConfig<T>() where T : ConfigClass
        {
            string configName = typeof(T).Name;
            if (!configsDic.TryGetValue(configName, out Dictionary<int, ConfigClass> configs))
            {
                //Debugger.LogError($"----------------找不到ProfessionConfig表----------");
                return null;
            }

            List<T> allConfigs = new List<T>();
            foreach(ConfigClass cfig in configs.Values)
            {
                allConfigs.Add(cfig as T);
            }

            return allConfigs.ToArray();
        }

        public T GetConfig<T>(int id) where T: ConfigClass
        {
            string configName = typeof(T).Name;
            if (!configsDic.TryGetValue(configName, out Dictionary<int, ConfigClass> configs))
            {
                //Debugger.LogError($"----------------找不到ProfessionConfig表----------");
                return null;
            }

            if (!configsDic[configName].TryGetValue(id, out ConfigClass config))
            {
                //Debugger.LogError($"----------------找不到ProfessionConfig表----------");
                return null;
            }
            //ProfessionConfig professionConfig = configList[id] as ProfessionConfig;
            T realConfig = config as T;
            if (realConfig == null)
            {
                //Debugger.LogError($"----------------找不到{id}对应的职业配置----------");
                return null;
            }
            return realConfig;
        }

        //public ProfessionConfig GetProfessionConfig(int id)
        //{

        //    if (!configsDic.TryGetValue("ProfessionConfig", out Dictionary<int, ConfigClass> configs))
        //    {
        //        //Debugger.LogError($"----------------找不到ProfessionConfig表----------");
        //        return null;
        //    }

        //    if (!configsDic["ProfessionConfig"].TryGetValue(id, out ConfigClass config))
        //    {
        //        //Debugger.LogError($"----------------找不到ProfessionConfig表----------");
        //        return null;
        //    }
        //    //ProfessionConfig professionConfig = configList[id] as ProfessionConfig;
        //    ProfessionConfig professionConfig = config as ProfessionConfig;
        //    if (professionConfig == null)
        //    {
        //        //Debugger.LogError($"----------------找不到{id}对应的职业配置----------");
        //        return null;
        //    }

        //    return professionConfig;

        //}

        //public SchoolConfig GetSchoolConfig(int id)
        //{

        //    if (!configsDic.TryGetValue("SchoolConfig", out Dictionary<int, ConfigClass> configs))
        //    {
        //        //Debugger.LogError($"----------------找不到SchoolConfig表----------");
        //        return null;
        //    }

        //    if (!configsDic["SchoolConfig"].TryGetValue(id, out ConfigClass config))
        //    {
        //        //Debugger.LogError($"----------------找不到SchoolConfig表----------");
        //        return null;
        //    }
        //    //ProfessionConfig professionConfig = configList[id] as ProfessionConfig;
        //    SchoolConfig schoolConfig = config as SchoolConfig;
        //    if (schoolConfig == null)
        //    {
        //        //Debugger.LogError($"----------------找不到{id}对应的门派配置----------");
        //        return null;
        //    }

        //    return schoolConfig;

        //}




        public void ReadConfig(string ConfigName)
        {
            //string filePath = @"..\..\ConfigTable\" + ConfigName + ".msconfig";
            string filePath = @"..\..\ConfigTable\" + ConfigName;
            FileStream stream = new FileStream(filePath, FileMode.Open);
            BinaryReader binary = new BinaryReader(stream);
            byte[] bytes = binary.ReadBytes((int)stream.Length);
            MemoryStream memory = new MemoryStream(bytes);
            BinaryFormatter formatter = new BinaryFormatter();
            List<ConfigClass> datalist = (List<ConfigClass>)formatter.Deserialize(memory);
            dic.Add(ConfigName, datalist);
            foreach (ConfigClass config in datalist)
            {
                if (!configsDic.TryGetValue(ConfigName, out Dictionary<int, ConfigClass> configs))
                    configsDic.Add(ConfigName, new Dictionary<int, ConfigClass>());
                if (configsDic[ConfigName].TryGetValue(config.id, out ConfigClass newConfig))
                    Console.WriteLine($"-------------重复添加索引为{config.id}的数据------------");
                configsDic[ConfigName].Add(config.id, config);
            }
            Console.WriteLine($"{ConfigName}该表加载完成");
        }
    }
}
