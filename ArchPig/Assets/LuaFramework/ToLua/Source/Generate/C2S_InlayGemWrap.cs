//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class C2S_InlayGemWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(C2S_InlayGem), typeof(System.Object));
		L.RegFunction("New", _CreateC2S_InlayGem);
		L.RegFunction("__tostring", ToLua.op_ToString);
		L.RegVar("equipGuid", get_equipGuid, set_equipGuid);
		L.RegVar("hole", get_hole, set_hole);
		L.RegVar("gemGuid", get_gemGuid, set_gemGuid);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateC2S_InlayGem(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 3)
			{
				string arg0 = ToLua.CheckString(L, 1);
				int arg1 = (int)LuaDLL.luaL_checknumber(L, 2);
				string arg2 = ToLua.CheckString(L, 3);
				C2S_InlayGem obj = new C2S_InlayGem(arg0, arg1, arg2);
				ToLua.PushObject(L, obj);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to ctor method: C2S_InlayGem.New");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_equipGuid(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			C2S_InlayGem obj = (C2S_InlayGem)o;
			string ret = obj.equipGuid;
			LuaDLL.lua_pushstring(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index equipGuid on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_hole(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			C2S_InlayGem obj = (C2S_InlayGem)o;
			int ret = obj.hole;
			LuaDLL.lua_pushinteger(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index hole on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_gemGuid(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			C2S_InlayGem obj = (C2S_InlayGem)o;
			string ret = obj.gemGuid;
			LuaDLL.lua_pushstring(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index gemGuid on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_equipGuid(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			C2S_InlayGem obj = (C2S_InlayGem)o;
			string arg0 = ToLua.CheckString(L, 2);
			obj.equipGuid = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index equipGuid on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_hole(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			C2S_InlayGem obj = (C2S_InlayGem)o;
			int arg0 = (int)LuaDLL.luaL_checknumber(L, 2);
			obj.hole = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index hole on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_gemGuid(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			C2S_InlayGem obj = (C2S_InlayGem)o;
			string arg0 = ToLua.CheckString(L, 2);
			obj.gemGuid = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index gemGuid on a nil value");
		}
	}
}

