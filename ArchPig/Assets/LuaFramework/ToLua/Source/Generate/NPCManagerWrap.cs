//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class NPCManagerWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(NPCManager), typeof(System.Object));
		L.RegFunction("Init", Init);
		L.RegFunction("GetNpcId", GetNpcId);
		L.RegFunction("ClickNpc", ClickNpc);
		L.RegFunction("ClearTaskIcon", ClearTaskIcon);
		L.RegFunction("HideTaskIcon", HideTaskIcon);
		L.RegFunction("AddTaskIcon", AddTaskIcon);
		L.RegFunction("New", _CreateNPCManager);
		L.RegFunction("__tostring", ToLua.op_ToString);
		L.RegVar("NPCCount", get_NPCCount, set_NPCCount);
		L.RegVar("Instance", get_Instance, null);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateNPCManager(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 0)
			{
				NPCManager obj = new NPCManager();
				ToLua.PushObject(L, obj);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to ctor method: NPCManager.New");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Init(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			NPCManager obj = (NPCManager)ToLua.CheckObject<NPCManager>(L, 1);
			obj.Init();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetNpcId(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			NPCManager obj = (NPCManager)ToLua.CheckObject<NPCManager>(L, 1);
			UnityEngine.GameObject arg0 = (UnityEngine.GameObject)ToLua.CheckObject(L, 2, typeof(UnityEngine.GameObject));
			int o = obj.GetNpcId(arg0);
			LuaDLL.lua_pushinteger(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ClickNpc(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			NPCManager obj = (NPCManager)ToLua.CheckObject<NPCManager>(L, 1);
			UnityEngine.GameObject arg0 = (UnityEngine.GameObject)ToLua.CheckObject(L, 2, typeof(UnityEngine.GameObject));
			obj.ClickNpc(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ClearTaskIcon(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			NPCManager obj = (NPCManager)ToLua.CheckObject<NPCManager>(L, 1);
			obj.ClearTaskIcon();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int HideTaskIcon(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			NPCManager obj = (NPCManager)ToLua.CheckObject<NPCManager>(L, 1);
			int arg0 = (int)LuaDLL.luaL_checknumber(L, 2);
			obj.HideTaskIcon(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int AddTaskIcon(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 3);
			NPCManager obj = (NPCManager)ToLua.CheckObject<NPCManager>(L, 1);
			int arg0 = (int)LuaDLL.luaL_checknumber(L, 2);
			string arg1 = ToLua.CheckString(L, 3);
			obj.AddTaskIcon(arg0, arg1);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_NPCCount(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			NPCManager obj = (NPCManager)o;
			int ret = obj.NPCCount;
			LuaDLL.lua_pushinteger(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index NPCCount on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_Instance(IntPtr L)
	{
		try
		{
			ToLua.PushObject(L, NPCManager.Instance);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_NPCCount(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			NPCManager obj = (NPCManager)o;
			int arg0 = (int)LuaDLL.luaL_checknumber(L, 2);
			obj.NPCCount = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index NPCCount on a nil value");
		}
	}
}

