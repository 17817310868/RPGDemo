using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using LuaInterface;
using System;
using GameDesigner;

namespace LuaFramework {
    public static class LuaHelper {

        /// <summary>
        /// getType
        /// </summary>
        /// <param name="classname"></param>
        /// <returns></returns>
        public static System.Type GetType(string classname) {
            Assembly assb = Assembly.GetExecutingAssembly();  //.GetExecutingAssembly();
            System.Type t = null;
            t = assb.GetType(classname); ;
            if (t == null) {
                t = assb.GetType(classname);
            }
            return t;
        }

        /// <summary>
        /// 面板管理器
        /// </summary>
        public static PanelManager GetPanelManager() {
            return AppFacade.Instance.GetManager<PanelManager>(ManagerName.Panel);
        }

        /// <summary>
        /// 事件管理器
        /// </summary>
        /// <returns></returns>
        public static EventManager GetEventManager()
        {
            return EventManager.Instance;
        }

        /// <summary>
        /// 场景管理器
        /// </summary>
        /// <returns></returns>
        public static ScenesManager GetScenesManager()
        {
            return AppFacade.Instance.GetManager<ScenesManager>(ManagerName.Scenes);
        }

        /// <summary>
        /// 游戏管理器
        /// </summary>
        /// <returns></returns>
        public static GameManager GetGameManager()
        {
            return AppFacade.Instance.GetManager<GameManager>(ManagerName.Game);
        }

        /// <summary>
        /// 客户端管理器
        /// </summary>
        /// <returns></returns>
        public static ClientManager GetClientManager()
        {
            return AppFacade.Instance.GetManager<ClientManager>(ManagerName.Client);
        }

        /// <summary>
        /// 资源管理器
        /// </summary>
        public static ResourceManager GetResManager() {
            return AppFacade.Instance.GetManager<ResourceManager>(ManagerName.Resource);
        }

        /// <summary>
        /// 网络管理器
        /// </summary>
        public static NetworkManager GetNetManager() {
            return AppFacade.Instance.GetManager<NetworkManager>(ManagerName.Network);
        }

        /// <summary>
        /// 音乐管理器
        /// </summary>
        public static SoundManager GetSoundManager() {
            return AppFacade.Instance.GetManager<SoundManager>(ManagerName.Sound);
        }

        /// <summary>
        /// 对象池管理器
        /// </summary>
        /// <returns></returns>
        public static ObjectPoolManager GetObjectPoolManager()
        {
            return AppFacade.Instance.GetManager<ObjectPoolManager>(ManagerName.ObjectPool);
        }

        /// <summary>
        /// 配置表管理器
        /// </summary>
        /// <returns></returns>
        public static ConfigManager GetConfigManager()
        {
            return ConfigManager.Instance;
        }

        /// <summary>
        /// 角色管理器
        /// </summary>
        /// <returns></returns>
        public static RoleMgr GetRoleMgr()
        {
            return RoleMgr.Instance;
        }

        public static StateMgr GetStateMgr()
        {
            return StateMgr.Instance;
        }

        public static BattleMgr GetBattleMgr()
        {
            return AppFacade.Instance.GetManager<BattleMgr>(ManagerName.Battle);
        }

        public static CameraMgr GetCameraMgr()
        {
            return CameraMgr.Instance;
        }

        public static NPCManager GetNPCMgr()
        {
            return NPCManager.Instance;
        }

        public static AudioManager GetAudioMgr()
        {
            return AudioManager.Instance;
        }

        public static RoleCtrl GetRoleCtrl()
        {
            return RoleCtrl.Instance;
        }

        /// <summary>
        /// pbc/pblua函数回调
        /// </summary>
        /// <param name="func"></param>
        public static void OnCallLuaFunc(LuaByteBuffer data, LuaFunction func) {
            if (func != null) func.Call(data);
            Debug.LogWarning("OnCallLuaFunc length:>>" + data.buffer.Length);
        }

        /// <summary>
        /// cjson函数回调
        /// </summary>
        /// <param name="data"></param>
        /// <param name="func"></param>
        public static void OnJsonCallFunc(string data, LuaFunction func) {
            Debug.LogWarning("OnJsonCallback data:>>" + data + " lenght:>>" + data.Length);
            if (func != null) func.Call(data);
        }
    }
}