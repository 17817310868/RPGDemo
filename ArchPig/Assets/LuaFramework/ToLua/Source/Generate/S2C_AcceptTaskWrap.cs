﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class S2C_AcceptTaskWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(S2C_AcceptTask), typeof(System.Object));
		L.RegFunction("New", _CreateS2C_AcceptTask);
		L.RegFunction("__tostring", ToLua.op_ToString);
		L.RegVar("taskId", get_taskId, set_taskId);
		L.RegVar("progress", get_progress, set_progress);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateS2C_AcceptTask(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 2)
			{
				int arg0 = (int)LuaDLL.luaL_checknumber(L, 1);
				int arg1 = (int)LuaDLL.luaL_checknumber(L, 2);
				S2C_AcceptTask obj = new S2C_AcceptTask(arg0, arg1);
				ToLua.PushObject(L, obj);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to ctor method: S2C_AcceptTask.New");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_taskId(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			S2C_AcceptTask obj = (S2C_AcceptTask)o;
			int ret = obj.taskId;
			LuaDLL.lua_pushinteger(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index taskId on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_progress(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			S2C_AcceptTask obj = (S2C_AcceptTask)o;
			int ret = obj.progress;
			LuaDLL.lua_pushinteger(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index progress on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_taskId(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			S2C_AcceptTask obj = (S2C_AcceptTask)o;
			int arg0 = (int)LuaDLL.luaL_checknumber(L, 2);
			obj.taskId = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index taskId on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_progress(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			S2C_AcceptTask obj = (S2C_AcceptTask)o;
			int arg0 = (int)LuaDLL.luaL_checknumber(L, 2);
			obj.progress = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index progress on a nil value");
		}
	}
}
