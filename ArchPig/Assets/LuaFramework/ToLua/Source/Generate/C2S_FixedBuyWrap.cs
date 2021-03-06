//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class C2S_FixedBuyWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(C2S_FixedBuy), typeof(System.Object));
		L.RegFunction("New", _CreateC2S_FixedBuy);
		L.RegFunction("__tostring", ToLua.op_ToString);
		L.RegVar("Guid", get_Guid, set_Guid);
		L.RegVar("itemType", get_itemType, set_itemType);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateC2S_FixedBuy(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 2)
			{
				string arg0 = ToLua.CheckString(L, 1);
				byte arg1 = (byte)LuaDLL.luaL_checknumber(L, 2);
				C2S_FixedBuy obj = new C2S_FixedBuy(arg0, arg1);
				ToLua.PushObject(L, obj);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to ctor method: C2S_FixedBuy.New");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_Guid(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			C2S_FixedBuy obj = (C2S_FixedBuy)o;
			string ret = obj.Guid;
			LuaDLL.lua_pushstring(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index Guid on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_itemType(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			C2S_FixedBuy obj = (C2S_FixedBuy)o;
			byte ret = obj.itemType;
			LuaDLL.lua_pushnumber(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index itemType on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_Guid(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			C2S_FixedBuy obj = (C2S_FixedBuy)o;
			string arg0 = ToLua.CheckString(L, 2);
			obj.Guid = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index Guid on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_itemType(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			C2S_FixedBuy obj = (C2S_FixedBuy)o;
			byte arg0 = (byte)LuaDLL.luaL_checknumber(L, 2);
			obj.itemType = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index itemType on a nil value");
		}
	}
}

