﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class C2S_UpgradeSkillWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(C2S_UpgradeSkill), typeof(System.Object));
		L.RegFunction("New", _CreateC2S_UpgradeSkill);
		L.RegFunction("__tostring", ToLua.op_ToString);
		L.RegVar("skillId", get_skillId, set_skillId);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateC2S_UpgradeSkill(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 1)
			{
				int arg0 = (int)LuaDLL.luaL_checknumber(L, 1);
				C2S_UpgradeSkill obj = new C2S_UpgradeSkill(arg0);
				ToLua.PushObject(L, obj);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to ctor method: C2S_UpgradeSkill.New");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_skillId(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			C2S_UpgradeSkill obj = (C2S_UpgradeSkill)o;
			int ret = obj.skillId;
			LuaDLL.lua_pushinteger(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index skillId on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_skillId(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			C2S_UpgradeSkill obj = (C2S_UpgradeSkill)o;
			int arg0 = (int)LuaDLL.luaL_checknumber(L, 2);
			obj.skillId = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index skillId on a nil value");
		}
	}
}

