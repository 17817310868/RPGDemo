//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class C2S_BattleCommandWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(C2S_BattleCommand), typeof(System.Object));
		L.RegFunction("New", _CreateC2S_BattleCommand);
		L.RegFunction("__tostring", ToLua.op_ToString);
		L.RegVar("actorIndex", get_actorIndex, set_actorIndex);
		L.RegVar("victim", get_victim, set_victim);
		L.RegVar("actorType", get_actorType, set_actorType);
		L.RegVar("paramId", get_paramId, set_paramId);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateC2S_BattleCommand(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 4)
			{
				byte arg0 = (byte)LuaDLL.luaL_checknumber(L, 1);
				byte arg1 = (byte)LuaDLL.luaL_checknumber(L, 2);
				byte arg2 = (byte)LuaDLL.luaL_checknumber(L, 3);
				int arg3 = (int)LuaDLL.luaL_checknumber(L, 4);
				C2S_BattleCommand obj = new C2S_BattleCommand(arg0, arg1, arg2, arg3);
				ToLua.PushObject(L, obj);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to ctor method: C2S_BattleCommand.New");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_actorIndex(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			C2S_BattleCommand obj = (C2S_BattleCommand)o;
			byte ret = obj.actorIndex;
			LuaDLL.lua_pushnumber(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index actorIndex on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_victim(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			C2S_BattleCommand obj = (C2S_BattleCommand)o;
			byte ret = obj.victim;
			LuaDLL.lua_pushnumber(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index victim on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_actorType(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			C2S_BattleCommand obj = (C2S_BattleCommand)o;
			byte ret = obj.actorType;
			LuaDLL.lua_pushnumber(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index actorType on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_paramId(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			C2S_BattleCommand obj = (C2S_BattleCommand)o;
			int ret = obj.paramId;
			LuaDLL.lua_pushinteger(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index paramId on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_actorIndex(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			C2S_BattleCommand obj = (C2S_BattleCommand)o;
			byte arg0 = (byte)LuaDLL.luaL_checknumber(L, 2);
			obj.actorIndex = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index actorIndex on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_victim(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			C2S_BattleCommand obj = (C2S_BattleCommand)o;
			byte arg0 = (byte)LuaDLL.luaL_checknumber(L, 2);
			obj.victim = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index victim on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_actorType(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			C2S_BattleCommand obj = (C2S_BattleCommand)o;
			byte arg0 = (byte)LuaDLL.luaL_checknumber(L, 2);
			obj.actorType = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index actorType on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_paramId(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			C2S_BattleCommand obj = (C2S_BattleCommand)o;
			int arg0 = (int)LuaDLL.luaL_checknumber(L, 2);
			obj.paramId = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index paramId on a nil value");
		}
	}
}

