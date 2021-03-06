//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class StateMgrWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(StateMgr), typeof(System.Object));
		L.RegFunction("AddStateManager", AddStateManager);
		L.RegFunction("GetStateManager", GetStateManager);
		L.RegFunction("RemoveStateManager", RemoveStateManager);
		L.RegFunction("ChangeState", ChangeState);
		L.RegFunction("ChangeIdle", ChangeIdle);
		L.RegFunction("ChangeMoveState", ChangeMoveState);
		L.RegFunction("ChangeFollowState", ChangeFollowState);
		L.RegFunction("Clear", Clear);
		L.RegFunction("UpdateState", UpdateState);
		L.RegFunction("ShowPose", ShowPose);
		L.RegFunction("New", _CreateStateMgr);
		L.RegFunction("__tostring", ToLua.op_ToString);
		L.RegVar("Instance", get_Instance, null);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateStateMgr(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 0)
			{
				StateMgr obj = new StateMgr();
				ToLua.PushObject(L, obj);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to ctor method: StateMgr.New");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int AddStateManager(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 3);
			StateMgr obj = (StateMgr)ToLua.CheckObject<StateMgr>(L, 1);
			string arg0 = ToLua.CheckString(L, 2);
			StateManager arg1 = (StateManager)ToLua.CheckObject<StateManager>(L, 3);
			obj.AddStateManager(arg0, arg1);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetStateManager(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			StateMgr obj = (StateMgr)ToLua.CheckObject<StateMgr>(L, 1);
			string arg0 = ToLua.CheckString(L, 2);
			StateManager o = obj.GetStateManager(arg0);
			ToLua.PushObject(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int RemoveStateManager(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			StateMgr obj = (StateMgr)ToLua.CheckObject<StateMgr>(L, 1);
			string arg0 = ToLua.CheckString(L, 2);
			obj.RemoveStateManager(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ChangeState(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 3);
			StateMgr obj = (StateMgr)ToLua.CheckObject<StateMgr>(L, 1);
			string arg0 = ToLua.CheckString(L, 2);
			int arg1 = (int)LuaDLL.luaL_checknumber(L, 3);
			obj.ChangeState(arg0, arg1);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ChangeIdle(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			StateMgr obj = (StateMgr)ToLua.CheckObject<StateMgr>(L, 1);
			string arg0 = ToLua.CheckString(L, 2);
			obj.ChangeIdle(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ChangeMoveState(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			StateMgr obj = (StateMgr)ToLua.CheckObject<StateMgr>(L, 1);
			S2C_MoveInfo arg0 = (S2C_MoveInfo)ToLua.CheckObject<S2C_MoveInfo>(L, 2);
			obj.ChangeMoveState(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ChangeFollowState(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			StateMgr obj = (StateMgr)ToLua.CheckObject<StateMgr>(L, 1);
			S2C_FollowInfo arg0 = (S2C_FollowInfo)ToLua.CheckObject<S2C_FollowInfo>(L, 2);
			obj.ChangeFollowState(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Clear(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			StateMgr obj = (StateMgr)ToLua.CheckObject<StateMgr>(L, 1);
			obj.Clear();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int UpdateState(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			StateMgr obj = (StateMgr)ToLua.CheckObject<StateMgr>(L, 1);
			obj.UpdateState();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ShowPose(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			StateMgr obj = (StateMgr)ToLua.CheckObject<StateMgr>(L, 1);
			UnityEngine.GameObject arg0 = (UnityEngine.GameObject)ToLua.CheckObject(L, 2, typeof(UnityEngine.GameObject));
			System.Collections.IEnumerator o = obj.ShowPose(arg0);
			ToLua.Push(L, o);
			return 1;
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
			ToLua.PushObject(L, StateMgr.Instance);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}
}

