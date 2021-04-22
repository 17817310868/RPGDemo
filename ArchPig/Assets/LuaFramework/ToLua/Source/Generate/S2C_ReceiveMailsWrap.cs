﻿//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class S2C_ReceiveMailsWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(S2C_ReceiveMails), typeof(System.Object));
		L.RegFunction("New", _CreateS2C_ReceiveMails);
		L.RegFunction("__tostring", ToLua.op_ToString);
		L.RegVar("mails", get_mails, set_mails);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateS2C_ReceiveMails(IntPtr L)
	{
		try
		{
			int count = LuaDLL.lua_gettop(L);

			if (count == 1)
			{
				System.Collections.Generic.List<S2C_ReceiveMail> arg0 = (System.Collections.Generic.List<S2C_ReceiveMail>)ToLua.CheckObject(L, 1, typeof(System.Collections.Generic.List<S2C_ReceiveMail>));
				S2C_ReceiveMails obj = new S2C_ReceiveMails(arg0);
				ToLua.PushObject(L, obj);
				return 1;
			}
			else
			{
				return LuaDLL.luaL_throw(L, "invalid arguments to ctor method: S2C_ReceiveMails.New");
			}
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_mails(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			S2C_ReceiveMails obj = (S2C_ReceiveMails)o;
			System.Collections.Generic.List<S2C_ReceiveMail> ret = obj.mails;
			ToLua.PushSealed(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index mails on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_mails(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			S2C_ReceiveMails obj = (S2C_ReceiveMails)o;
			System.Collections.Generic.List<S2C_ReceiveMail> arg0 = (System.Collections.Generic.List<S2C_ReceiveMail>)ToLua.CheckObject(L, 2, typeof(System.Collections.Generic.List<S2C_ReceiveMail>));
			obj.mails = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index mails on a nil value");
		}
	}
}
