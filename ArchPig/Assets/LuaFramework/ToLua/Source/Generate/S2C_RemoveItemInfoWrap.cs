//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class S2C_RemoveItemInfoWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(S2C_RemoveItemInfo), typeof(System.Object));
		L.RegFunction("New", _CreateS2C_RemoveItemInfo);
		L.RegFunction("__tostring", ToLua.op_ToString);
		L.RegVar("Guid", get_Guid, set_Guid);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateS2C_RemoveItemInfo(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 1)
			{
				string arg0 = ToLua.CheckString(L, 1);
				S2C_RemoveItemInfo obj = new S2C_RemoveItemInfo(arg0);
				ToLua.PushObject(L, obj);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to ctor method: S2C_RemoveItemInfo.New");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_Guid(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			S2C_RemoveItemInfo obj = (S2C_RemoveItemInfo)o;
			string ret = obj.Guid;
			LuaDLL.lua_pushstring(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index Guid on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_Guid(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			S2C_RemoveItemInfo obj = (S2C_RemoveItemInfo)o;
			string arg0 = ToLua.CheckString(L, 2);
			obj.Guid = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index Guid on a nil value");
		}
	}
}

