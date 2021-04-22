﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class S2C_JoinTeamReplyWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(S2C_JoinTeamReply), typeof(System.Object));
		L.RegFunction("New", _CreateS2C_JoinTeamReply);
		L.RegFunction("__tostring", ToLua.op_ToString);
		L.RegVar("playerId", get_playerId, set_playerId);
		L.RegVar("result", get_result, set_result);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateS2C_JoinTeamReply(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 2)
			{
				string arg0 = ToLua.CheckString(L, 1);
				bool arg1 = LuaDLL.luaL_checkboolean(L, 2);
				S2C_JoinTeamReply obj = new S2C_JoinTeamReply(arg0, arg1);
				ToLua.PushObject(L, obj);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to ctor method: S2C_JoinTeamReply.New");
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
			S2C_JoinTeamReply obj = (S2C_JoinTeamReply)o;
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
	static int get_result(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			S2C_JoinTeamReply obj = (S2C_JoinTeamReply)o;
			bool ret = obj.result;
			LuaDLL.lua_pushboolean(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index result on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_playerId(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			S2C_JoinTeamReply obj = (S2C_JoinTeamReply)o;
			string arg0 = ToLua.CheckString(L, 2);
			obj.playerId = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index playerId on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_result(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			S2C_JoinTeamReply obj = (S2C_JoinTeamReply)o;
			bool arg0 = LuaDLL.luaL_checkboolean(L, 2);
			obj.result = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index result on a nil value");
		}
	}
}

