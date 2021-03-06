//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class S2C_RolePowerRankWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(S2C_RolePowerRank), typeof(System.Object));
		L.RegFunction("New", _CreateS2C_RolePowerRank);
		L.RegFunction("__tostring", ToLua.op_ToString);
		L.RegVar("ranks", get_ranks, set_ranks);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateS2C_RolePowerRank(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 1)
			{
				System.Collections.Generic.List<RolePowerRank> arg0 = (System.Collections.Generic.List<RolePowerRank>)ToLua.CheckObject(L, 1, typeof(System.Collections.Generic.List<RolePowerRank>));
				S2C_RolePowerRank obj = new S2C_RolePowerRank(arg0);
				ToLua.PushObject(L, obj);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to ctor method: S2C_RolePowerRank.New");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_ranks(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			S2C_RolePowerRank obj = (S2C_RolePowerRank)o;
			System.Collections.Generic.List<RolePowerRank> ret = obj.ranks;
			ToLua.PushSealed(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index ranks on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_ranks(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			S2C_RolePowerRank obj = (S2C_RolePowerRank)o;
			System.Collections.Generic.List<RolePowerRank> arg0 = (System.Collections.Generic.List<RolePowerRank>)ToLua.CheckObject(L, 2, typeof(System.Collections.Generic.List<RolePowerRank>));
			obj.ranks = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index ranks on a nil value");
		}
	}
}

