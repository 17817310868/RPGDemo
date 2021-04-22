﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class C2S_BattleRequestWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(C2S_BattleRequest), typeof(System.Object));
		L.RegFunction("New", _CreateC2S_BattleRequest);
		L.RegFunction("__tostring", ToLua.op_ToString);
		L.RegVar("playerId", get_playerId, set_playerId);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateC2S_BattleRequest(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 1)
			{
				string arg0 = ToLua.CheckString(L, 1);
				C2S_BattleRequest obj = new C2S_BattleRequest(arg0);
				ToLua.PushObject(L, obj);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to ctor method: C2S_BattleRequest.New");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_playerId(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			C2S_BattleRequest obj = (C2S_BattleRequest)o;
			string ret = obj.playerId;
			LuaDLL.lua_pushstring(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index playerId on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_playerId(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			C2S_BattleRequest obj = (C2S_BattleRequest)o;
			string arg0 = ToLua.CheckString(L, 2);
			obj.playerId = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index playerId on a nil value");
		}
	}
}

