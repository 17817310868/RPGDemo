﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class Net_Share_RPCFunWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(Net.Share.RPCFun), typeof(System.Attribute));
		L.RegFunction("New", _CreateNet_Share_RPCFun);
		L.RegFunction("__tostring", ToLua.op_ToString);
		L.RegVar("cmd", get_cmd, set_cmd);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateNet_Share_RPCFun(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 0)
			{
				Net.Share.RPCFun obj = new Net.Share.RPCFun();
				ToLua.PushObject(L, obj);
				return 1;
			}
			else if (count == 1)
			{
				byte arg0 = (byte)LuaDLL.luaL_checknumber(L, 1);
				Net.Share.RPCFun obj = new Net.Share.RPCFun(arg0);
				ToLua.PushObject(L, obj);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to ctor method: Net.Share.RPCFun.New");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_cmd(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			Net.Share.RPCFun obj = (Net.Share.RPCFun)o;
			byte ret = obj.cmd;
			LuaDLL.lua_pushnumber(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index cmd on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_cmd(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			Net.Share.RPCFun obj = (Net.Share.RPCFun)o;
			byte arg0 = (byte)LuaDLL.luaL_checknumber(L, 2);
			obj.cmd = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index cmd on a nil value");
		}
	}
}

