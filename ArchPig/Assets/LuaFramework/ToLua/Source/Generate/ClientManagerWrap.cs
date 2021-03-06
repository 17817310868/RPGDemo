//this source code was auto-generated by tolua#, do not modify it
using System;
using LuaInterface;

public class ClientManagerWrap
{
	public static void Register(LuaState L)
	{
		L.BeginClass(typeof(ClientManager), typeof(Net.Client.NetBehaviour));
		L.RegFunction("MySend", MySend);
		L.RegFunction("OnBlockConnection", OnBlockConnection);
		L.RegFunction("OnCloseConnect", OnCloseConnect);
		L.RegFunction("OnConnected", OnConnected);
		L.RegFunction("OnConnectFailed", OnConnectFailed);
		L.RegFunction("OnConnectLost", OnConnectLost);
		L.RegFunction("OnDisconnect", OnDisconnect);
		L.RegFunction("OnReconnect", OnReconnect);
		L.RegFunction("OnTryToConnect", OnTryToConnect);
		L.RegFunction("DebugLog", DebugLog);
		L.RegFunction("RpcLog", RpcLog);
		L.RegFunction("ErrorLog", ErrorLog);
		L.RegFunction("__eq", op_Equality);
		L.RegFunction("__tostring", ToLua.op_ToString);
		L.RegVar("client", get_client, set_client);
		L.EndClass();
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int MySend(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			string arg0 = ToLua.CheckString(L, 1);
			ClientManager.MySend(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int OnBlockConnection(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			ClientManager obj = (ClientManager)ToLua.CheckObject<ClientManager>(L, 1);
			obj.OnBlockConnection();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int OnCloseConnect(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			ClientManager obj = (ClientManager)ToLua.CheckObject<ClientManager>(L, 1);
			obj.OnCloseConnect();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int OnConnected(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			ClientManager obj = (ClientManager)ToLua.CheckObject<ClientManager>(L, 1);
			obj.OnConnected();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int OnConnectFailed(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			ClientManager obj = (ClientManager)ToLua.CheckObject<ClientManager>(L, 1);
			obj.OnConnectFailed();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int OnConnectLost(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			ClientManager obj = (ClientManager)ToLua.CheckObject<ClientManager>(L, 1);
			obj.OnConnectLost();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int OnDisconnect(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			ClientManager obj = (ClientManager)ToLua.CheckObject<ClientManager>(L, 1);
			obj.OnDisconnect();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int OnReconnect(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			ClientManager obj = (ClientManager)ToLua.CheckObject<ClientManager>(L, 1);
			obj.OnReconnect();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int OnTryToConnect(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 1);
			ClientManager obj = (ClientManager)ToLua.CheckObject<ClientManager>(L, 1);
			obj.OnTryToConnect();
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int DebugLog(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			ClientManager obj = (ClientManager)ToLua.CheckObject<ClientManager>(L, 1);
			string arg0 = ToLua.CheckString(L, 2);
			obj.DebugLog(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int RpcLog(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			ClientManager obj = (ClientManager)ToLua.CheckObject<ClientManager>(L, 1);
			string arg0 = ToLua.CheckString(L, 2);
			obj.RpcLog(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int ErrorLog(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			ClientManager obj = (ClientManager)ToLua.CheckObject<ClientManager>(L, 1);
			string arg0 = ToLua.CheckString(L, 2);
			obj.ErrorLog(arg0);
			return 0;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int op_Equality(IntPtr L)
	{
		try
		{
			ToLua.CheckArgsCount(L, 2);
			UnityEngine.Object arg0 = (UnityEngine.Object)ToLua.ToObject(L, 1);
			UnityEngine.Object arg1 = (UnityEngine.Object)ToLua.ToObject(L, 2);
			bool o = arg0 == arg1;
			LuaDLL.lua_pushboolean(L, o);
			return 1;
		}
		catch (Exception e)
		{
			return LuaDLL.toluaL_exception(L, e);
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_client(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			ClientManager obj = (ClientManager)o;
			Net.Client.UdpClient ret = obj.client;
			ToLua.PushObject(L, ret);
			return 1;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index client on a nil value");
		}
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_client(IntPtr L)
	{
		object o = null;

		try
		{
			o = ToLua.ToObject(L, 1);
			ClientManager obj = (ClientManager)o;
			Net.Client.UdpClient arg0 = (Net.Client.UdpClient)ToLua.CheckObject<Net.Client.UdpClient>(L, 2);
			obj.client = arg0;
			return 0;
		}
		catch(Exception e)
		{
			return LuaDLL.toluaL_exception(L, e, o, "attempt to index client on a nil value");
		}
	}
}

