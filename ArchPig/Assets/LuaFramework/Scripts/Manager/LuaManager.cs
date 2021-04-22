using UnityEngine;
using System.Collections;
using LuaInterface;

namespace LuaFramework {
    public class LuaManager : Manager  {
        private LuaState lua;
        private LuaLoader loader;
        private LuaLooper loop = null;

        //public bool isRequireLuaCodeEnd = false;

        // Use this for initialization

        //public void InitLuaManager()
        //{
        //    loader = new LuaLoader();
        //    lua = new LuaState();
        //    this.OpenLibs();
        //    lua.LuaSetTop(0);

        //    LuaBinder.Bind(lua);
        //    DelegateFactory.Init();
        //    LuaCoroutine.Register(lua, this);
        //    //LuaCoroutine.Register(lua, AppFacade.Instance.GetManager<GameManager>(ManagerName.Game).GetComponent<MonoBehaviour>());
        //}

        void Awake()
        {
            loader = new LuaLoader();
            lua = new LuaState();
            this.OpenLibs();
            lua.LuaSetTop(0);

            LuaBinder.Bind(lua);
            DelegateFactory.Init();
            LuaCoroutine.Register(lua, this);
            //LuaCoroutine.Register(lua, AppFacade.Instance.GetManager<GameManager>(ManagerName.Game).GetComponent<MonoBehaviour>());
        }

        void Update()
        {
            
        }

        public void InitStart() {
            InitLuaPath();
            InitLuaBundle();
            this.lua.Start();    //启动LUAVM
            this.StartMain();
            this.StartLooper();
        }

        void StartLooper() {
            //loop = AppFacade.Instance.GetManager<LuaLooper>(ManagerName.LuaLoop);
            loop = gameObject.AddComponent<LuaLooper>();
            loop.luaState = lua;
        }

        //cjson 比较特殊，只new了一个table，没有注册库，这里注册一下
        protected void OpenCJson() {
            lua.LuaGetField(LuaIndexes.LUA_REGISTRYINDEX, "_LOADED");
            lua.OpenLibs(LuaDLL.luaopen_cjson);
            lua.LuaSetField(-2, "cjson");

            lua.OpenLibs(LuaDLL.luaopen_cjson_safe);
            lua.LuaSetField(-2, "cjson.safe");
        }

        void StartMain() {
            lua.DoFile("Main.lua");
            
            LuaFunction main = lua.GetFunction("Main");
            main.Call();
            main.Dispose();
            main = null;    
        }
        
        /// <summary>
        /// 初始化加载第三方库
        /// </summary>
        void OpenLibs() {
            lua.OpenLibs(LuaDLL.luaopen_pb);      
            lua.OpenLibs(LuaDLL.luaopen_sproto_core);
            lua.OpenLibs(LuaDLL.luaopen_protobuf_c);
            lua.OpenLibs(LuaDLL.luaopen_lpeg);
            lua.OpenLibs(LuaDLL.luaopen_bit);
            lua.OpenLibs(LuaDLL.luaopen_socket_core);

            this.OpenCJson();
        }

        /// <summary>
        /// 初始化Lua代码加载路径
        /// </summary>
        void InitLuaPath() {
            if (AppConst.DebugMode) {
                string rootPath = AppConst.FrameworkRoot;
                lua.AddSearchPath(rootPath + "/Lua");
                lua.AddSearchPath(rootPath + "/ToLua/Lua");
            } else {
                lua.AddSearchPath(Util.DataPath + "lua");
                if (Application.isEditor)
                {
                    lua.AddSearchPath(LuaConst.luaDir);
                    lua.AddSearchPath(LuaConst.toluaDir);
                }
            }
        }

        /// <summary>
        /// 初始化LuaBundle
        /// </summary>
        void InitLuaBundle() {
            if (loader.beZip) {
                loader.AddBundle("lua/lua.unity3d");
                loader.AddBundle("lua/lua_math.unity3d");
                loader.AddBundle("lua/lua_system.unity3d");
                loader.AddBundle("lua/lua_system_reflection.unity3d");
                loader.AddBundle("lua/lua_unityengine.unity3d");
                loader.AddBundle("lua/lua_common.unity3d");
                loader.AddBundle("lua/lua_logic.unity3d");
                loader.AddBundle("lua/lua_view.unity3d");
                loader.AddBundle("lua/lua_enum.unity3d");
                loader.AddBundle("lua/lua_config.unity3d");
                loader.AddBundle("lua/lua_tools.unity3d");
                loader.AddBundle("lua/lua_model.unity3d");
                loader.AddBundle("lua/lua_controller.unity3d");
                loader.AddBundle("lua/lua_misc.unity3d");

                loader.AddBundle("lua/lua_protobuf.unity3d");
                loader.AddBundle("lua/lua_3rd_cjson.unity3d");
                loader.AddBundle("lua/lua_3rd_luabitop.unity3d");
                loader.AddBundle("lua/lua_3rd_pbc.unity3d");
                loader.AddBundle("lua/lua_3rd_pblua.unity3d");
                loader.AddBundle("lua/lua_3rd_sproto.unity3d");
            }
        }

        public void DoFile(string filename) {
            lua.DoFile(filename);
        }

        public void DoString(string file)
        {
            lua.DoString(file);
        }

        public void Require(string filename)
        {
            lua.Require(filename);
        }

        public LuaTable GetTable(string tableName)
        {
            return lua.GetTable(tableName);
        }

        public LuaFunction GetFunction(string func)
        {

            LuaFunction function = lua.GetFunction(func);
            return function;
        }

        public LuaFunction GetRoleFunction(string func)
        {
            string name = func.Split('.')[0];
            if (func == null || name == null)
                Debugger.LogError("----------------获取不到lua方法名--------------");
            lua.Require("Role/" + name);
            LuaFunction function = lua.GetFunction(func);
            return function;
        }

        // Update is called once per frame
        public object[] CallFunction(string funcName, params object[] args) {
            LuaFunction func = lua.GetFunction(funcName);
            if (func != null)
            {
                return func.LazyCall(args);
            }
            return null;
        }

        public void LuaGC() {
            lua.LuaGC(LuaGCOptions.LUA_GCCOLLECT);
        }

        public void Close() {
            loop.Destroy();
            loop = null;

            lua.Dispose();
            lua = null;
            loader = null;
        }
    }
}