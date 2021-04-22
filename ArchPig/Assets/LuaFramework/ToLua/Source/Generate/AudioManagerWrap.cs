﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class AudioManagerWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(AudioManager), typeof(System.Object));
		L.RegFunction("Init", Init);
		L.RegFunction("SetBgmVolume", SetBgmVolume);
		L.RegFunction("GetBgmVolume", GetBgmVolume);
		L.RegFunction("SetEffectVolume", SetEffectVolume);
		L.RegFunction("GetEffectVolume", GetEffectVolume);
		L.RegFunction("PlayBgm", PlayBgm);
		L.RegFunction("PlayEffect", PlayEffect);
		L.RegFunction("PauseBgm", PauseBgm);
		L.RegFunction("PauseEffect", PauseEffect);
		L.RegFunction("StopBgm", StopBgm);
		L.RegFunction("StopEffect", StopEffect);
		L.RegFunction("New", _CreateAudioManager);
		L.RegFunction("__tostring", ToLua.op_ToString);
		L.RegVar("Instance", get_Instance, null);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateAudioManager(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 0)
			{
				AudioManager obj = new AudioManager();
				ToLua.PushObject(L, obj);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to ctor method: AudioManager.New");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Init(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			AudioManager obj = (AudioManager)ToLua.CheckObject<AudioManager>(L, 1);
			UnityEngine.GameObject arg0 = (UnityEngine.GameObject)ToLua.CheckObject(L, 2, typeof(UnityEngine.GameObject));
			obj.Init(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetBgmVolume(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			AudioManager obj = (AudioManager)ToLua.CheckObject<AudioManager>(L, 1);
			float arg0 = (float)LuaDLL.luaL_checknumber(L, 2);
			obj.SetBgmVolume(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetBgmVolume(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			AudioManager obj = (AudioManager)ToLua.CheckObject<AudioManager>(L, 1);
			float o = obj.GetBgmVolume();
			LuaDLL.lua_pushnumber(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int SetEffectVolume(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			AudioManager obj = (AudioManager)ToLua.CheckObject<AudioManager>(L, 1);
			float arg0 = (float)LuaDLL.luaL_checknumber(L, 2);
			obj.SetEffectVolume(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetEffectVolume(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			AudioManager obj = (AudioManager)ToLua.CheckObject<AudioManager>(L, 1);
			float o = obj.GetEffectVolume();
			LuaDLL.lua_pushnumber(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int PlayBgm(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			AudioManager obj = (AudioManager)ToLua.CheckObject<AudioManager>(L, 1);
			string arg0 = ToLua.CheckString(L, 2);
			obj.PlayBgm(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int PlayEffect(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			AudioManager obj = (AudioManager)ToLua.CheckObject<AudioManager>(L, 1);
			string arg0 = ToLua.CheckString(L, 2);
			obj.PlayEffect(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int PauseBgm(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			AudioManager obj = (AudioManager)ToLua.CheckObject<AudioManager>(L, 1);
			obj.PauseBgm();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int PauseEffect(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			AudioManager obj = (AudioManager)ToLua.CheckObject<AudioManager>(L, 1);
			obj.PauseEffect();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int StopBgm(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			AudioManager obj = (AudioManager)ToLua.CheckObject<AudioManager>(L, 1);
			obj.StopBgm();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int StopEffect(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			AudioManager obj = (AudioManager)ToLua.CheckObject<AudioManager>(L, 1);
			obj.StopEffect();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_Instance(IntPtr L)
	{
		try
		{
			ToLua.PushObject(L, AudioManager.Instance);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}
}

