﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class CameraMgrWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(CameraMgr), typeof(System.Object));
		L.RegFunction("Follow", Follow);
		L.RegFunction("ChangeFollow", ChangeFollow);
		L.RegFunction("ChangeFight", ChangeFight);
		L.RegFunction("New", _CreateCameraMgr);
		L.RegFunction("__tostring", ToLua.op_ToString);
		L.RegVar("camera", get_camera, set_camera);
		L.RegVar("followTarget", get_followTarget, set_followTarget);
		L.RegVar("state", get_state, set_state);
		L.RegVar("Instance", get_Instance, null);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateCameraMgr(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 0)
			{
				CameraMgr obj = new CameraMgr();
				ToLua.PushObject(L, obj);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to ctor method: CameraMgr.New");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Follow(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			CameraMgr obj = (CameraMgr)ToLua.CheckObject<CameraMgr>(L, 1);
			obj.Follow();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ChangeFollow(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			CameraMgr obj = (CameraMgr)ToLua.CheckObject<CameraMgr>(L, 1);
			obj.ChangeFollow();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ChangeFight(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			CameraMgr obj = (CameraMgr)ToLua.CheckObject<CameraMgr>(L, 1);
			obj.ChangeFight();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_camera(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			CameraMgr obj = (CameraMgr)o;
			UnityEngine.Transform ret = obj.camera;
			ToLua.Push(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index camera on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_followTarget(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			CameraMgr obj = (CameraMgr)o;
			UnityEngine.Transform ret = obj.followTarget;
			ToLua.Push(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index followTarget on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_state(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			CameraMgr obj = (CameraMgr)o;
			CameraState ret = obj.state;
			ToLua.Push(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index state on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_Instance(IntPtr L)
	{
		try
		{
			ToLua.PushObject(L, CameraMgr.Instance);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_camera(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			CameraMgr obj = (CameraMgr)o;
			UnityEngine.Transform arg0 = (UnityEngine.Transform)ToLua.CheckObject<UnityEngine.Transform>(L, 2);
			obj.camera = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index camera on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_followTarget(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			CameraMgr obj = (CameraMgr)o;
			UnityEngine.Transform arg0 = (UnityEngine.Transform)ToLua.CheckObject<UnityEngine.Transform>(L, 2);
			obj.followTarget = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index followTarget on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_state(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			CameraMgr obj = (CameraMgr)o;
			CameraState arg0 = (CameraState)ToLua.CheckObject(L, 2, typeof(CameraState));
			obj.state = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index state on a nil value");
		}
	}
}

