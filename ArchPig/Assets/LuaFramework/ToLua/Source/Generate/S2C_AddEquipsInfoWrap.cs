//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class S2C_AddEquipsInfoWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(S2C_AddEquipsInfo), typeof(System.Object));
		L.RegFunction("New", _CreateS2C_AddEquipsInfo);
		L.RegFunction("__tostring", ToLua.op_ToString);
		L.RegVar("items", get_items, set_items);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateS2C_AddEquipsInfo(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 1)
			{
				System.Collections.Generic.List<S2C_AddEquipInfo> arg0 = (System.Collections.Generic.List<S2C_AddEquipInfo>)ToLua.CheckObject(L, 1, typeof(System.Collections.Generic.List<S2C_AddEquipInfo>));
				S2C_AddEquipsInfo obj = new S2C_AddEquipsInfo(arg0);
				ToLua.PushObject(L, obj);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to ctor method: S2C_AddEquipsInfo.New");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_items(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			S2C_AddEquipsInfo obj = (S2C_AddEquipsInfo)o;
			System.Collections.Generic.List<S2C_AddEquipInfo> ret = obj.items;
			ToLua.PushSealed(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index items on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_items(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			S2C_AddEquipsInfo obj = (S2C_AddEquipsInfo)o;
			System.Collections.Generic.List<S2C_AddEquipInfo> arg0 = (System.Collections.Generic.List<S2C_AddEquipInfo>)ToLua.CheckObject(L, 2, typeof(System.Collections.Generic.List<S2C_AddEquipInfo>));
			obj.items = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index items on a nil value");
		}
	}
}

