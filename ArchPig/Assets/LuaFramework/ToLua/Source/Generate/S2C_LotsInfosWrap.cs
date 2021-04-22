﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class S2C_LotsInfosWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(S2C_LotsInfos), typeof(System.Object));
		L.RegFunction("New", _CreateS2C_LotsInfos);
		L.RegFunction("__tostring", ToLua.op_ToString);
		L.RegVar("itemType", get_itemType, set_itemType);
		L.RegVar("lotsInfos", get_lotsInfos, set_lotsInfos);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateS2C_LotsInfos(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 2)
			{
				byte arg0 = (byte)LuaDLL.luaL_checknumber(L, 1);
				System.Collections.Generic.List<S2C_LotsInfo> arg1 = (System.Collections.Generic.List<S2C_LotsInfo>)ToLua.CheckObject(L, 2, typeof(System.Collections.Generic.List<S2C_LotsInfo>));
				S2C_LotsInfos obj = new S2C_LotsInfos(arg0, arg1);
				ToLua.PushObject(L, obj);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to ctor method: S2C_LotsInfos.New");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_itemType(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			S2C_LotsInfos obj = (S2C_LotsInfos)o;
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
	static int get_lotsInfos(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			S2C_LotsInfos obj = (S2C_LotsInfos)o;
			System.Collections.Generic.List<S2C_LotsInfo> ret = obj.lotsInfos;
			ToLua.PushSealed(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index lotsInfos on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_itemType(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			S2C_LotsInfos obj = (S2C_LotsInfos)o;
			byte arg0 = (byte)LuaDLL.luaL_checknumber(L, 2);
			obj.itemType = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index itemType on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_lotsInfos(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			S2C_LotsInfos obj = (S2C_LotsInfos)o;
			System.Collections.Generic.List<S2C_LotsInfo> arg0 = (System.Collections.Generic.List<S2C_LotsInfo>)ToLua.CheckObject(L, 2, typeof(System.Collections.Generic.List<S2C_LotsInfo>));
			obj.lotsInfos = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index lotsInfos on a nil value");
		}
	}
}

