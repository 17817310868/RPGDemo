using System;
using System.Reflection;

namespace GameDesigner
{
	[Serializable]
	public class CreateUtility
	{
		/// <summary>
		/// 创建类对象实例
		/// </summary>
		static public object CreateInstance( string typeName )
		{
            return SystemType.GetType(typeName).Assembly.CreateInstance(typeName);
        }

		/// <summary>
		/// 创建类对象实例
		/// </summary>
		static public object CreateInstance( Type type )
		{
			try{
				object obj = type.Assembly.CreateInstance(type.FullName);
				if( obj != null )
					return obj;
				return type;
			}catch{}
			return null;
		}

		/// <summary>
		/// 创建类对象实例
		/// </summary>
		static public object CreateInstance( Type typeName , object[] args )
		{
			try{
				object obj = typeName.Assembly.CreateInstance(typeName.FullName,false,BindingFlags.Default,null,args,null,null);
				if( obj != null )
					return obj;
				return typeName;
			}catch{}
			return null;
		}
	}
}