namespace Net.Share
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    /// <summary>
    /// 网络转换二进制数据
    /// </summary>
    public class NetConvertBinary : NetConvertOld
    {
        private static ConcurrentDictionary<short, Type> networkTypes = new ConcurrentDictionary<short, Type>();
        private static ConcurrentDictionary<Type, short> networkType1s = new ConcurrentDictionary<Type, short>();

        private static readonly bool MyInit = Init();

        /// <summary>
        /// 序列化处理
        /// </summary>
        private static SerializeHandle SerializeHandle = MySerializeHandle;
        /// <summary>
        /// 反序列化委托类型，返回解析后的对象实例
        /// </summary>
        private static DeserializeHandle DeserializeHandle = MyDeserializeHandle;

        /// <summary>
        /// 初始化网络转换类型
        /// </summary>
        public static bool Init()
        {
            networkTypes = new ConcurrentDictionary<short, Type>();
            networkType1s = new ConcurrentDictionary<Type, short>();
            SerializeHandle = MySerializeHandle;
            DeserializeHandle = MyDeserializeHandle;
            AddNetworkBaseType();
            return true;
        }

        /// <summary>
        /// 添加网络基本类型， int，float，bool，string......
        /// </summary>
        public static void AddNetworkBaseType()
        {
            AddNetworkType(typeof(short));
            AddNetworkType(typeof(int));
            AddNetworkType(typeof(long));
            AddNetworkType(typeof(ushort));
            AddNetworkType(typeof(uint));
            AddNetworkType(typeof(ulong));
            AddNetworkType(typeof(float));
            AddNetworkType(typeof(double));
            AddNetworkType(typeof(bool));
            AddNetworkType(typeof(char));
            AddNetworkType(typeof(string));
            AddNetworkType(typeof(byte));
            AddNetworkType(typeof(sbyte));
            AddNetworkType(typeof(UnityEngine.Vector2));
            AddNetworkType(typeof(UnityEngine.Vector3));
            AddNetworkType(typeof(UnityEngine.Vector4));
            AddNetworkType(typeof(UnityEngine.Quaternion));
            AddNetworkType(typeof(UnityEngine.Rect));
            AddNetworkType(typeof(UnityEngine.Color));
            AddNetworkType(typeof(UnityEngine.Color32));
        }

        /// <summary>
        /// 默认序列化
        /// </summary>
        private static bool MySerializeHandle(Type type, object value, out byte[] buffer)
        {
            buffer = null;
            return false;
        }

        /// <summary>
        /// 默认反序列化
        /// </summary>
        private static bool MyDeserializeHandle(byte[] buffer, ref int index, Type type, out object value)
        {
            value = null;
            return false;
        }

        /// <summary>
        /// 添加序列化和反序列化监听器
        /// </summary>
        /// <param name="serializeHandle">序列化处理</param>
        /// <param name="deserializeHandle">反序列化处理</param>
        public static void AddListener(SerializeHandle serializeHandle, DeserializeHandle deserializeHandle)
        {
            SerializeHandle = serializeHandle ?? throw new Exception("不允许序列化为空");
            DeserializeHandle = deserializeHandle ?? throw new Exception("不允许反序列化为空");
        }

        /// <summary>
        /// 移除序列化和反序列化监听器
        /// </summary>
        public static void RemoveListener()
        {
            SerializeHandle = MySerializeHandle;
            DeserializeHandle = MyDeserializeHandle;
        }

        /// <summary>
        /// 添加可序列化的参数类型, 网络参数类型 如果不进行添加将不会被序列化,反序列化
        /// </summary>
        /// <typeparam name="T">要添加的网络类型</typeparam>
        public static void AddNetworkType<T>()
        {
            AddNetworkType(typeof(T));
        }

        /// <summary>
        /// 添加可序列化的参数类型, 网络参数类型 如果不进行添加将不会被序列化,反序列化
        /// </summary>
        /// <param name="type">要添加的网络类型</param>
        public static void AddNetworkType(Type type)
        {
            if (type.IsArray)
                throw new IOException("数组类型自动识别，无需注册，请取消数组注册！");
            if (type.IsEnum)
                throw new IOException("枚举类型自动识别，无需注册，请取消枚举注册！");
            if (networkType1s.ContainsKey(type))
                return;
            networkTypes.TryAdd((short)networkTypes.Count, type);
            networkType1s.TryAdd(type, (short)networkType1s.Count);
        }

        /// <summary>
        /// 添加可序列化的参数类型, 网络参数类型 如果不进行添加将不会被序列化,反序列化
        /// </summary>
        /// <param name="types">要添加的网络类型</param>
        public static void AddNetworkType(params Type[] types)
        {
            foreach (var type in types) {
                AddNetworkType(type);
            }
        }

        /// <summary>
        /// 添加网络程序集，此方法将会添加获取传入的类的程序集并全部添加
        /// </summary>
        /// <param name="value">传入的类</param>
        [Obsolete("不再建议使用此方法，请使用AddNetworkType方法来代替", true)]
        public static void AddNetworkTypeAssembly(Type value)
        {
            foreach (var type in value.Assembly.GetTypes().Where((t) => {
                return !t.IsAbstract & !t.IsInterface & !t.IsGenericType & !t.IsGenericTypeDefinition & t.IsPublic;
            })) {
                if (networkType1s.ContainsKey(type))
                    continue;
                networkTypes.TryAdd((short)networkTypes.Count, type);
                networkType1s.TryAdd(type, (short)networkType1s.Count);
            }
        }

        /// <summary>
        /// 添加网络传输程序集， 注意：添加客户端的程序集必须和服务器的程序集必须保持一致， 否则将会出现问题
        /// </summary>
        /// <param name="assemblies">程序集</param>
        [Obsolete("不再建议使用此方法，请使用AddNetworkType方法来代替", true)]
        public static void AddNetworkAssembly(Assembly[] assemblies)
        {
            foreach (var assemblie in assemblies) {
                foreach (var type in assemblie.GetTypes().Where((t) => {
                    return !t.IsAbstract & !t.IsInterface & !t.IsGenericType & !t.IsGenericTypeDefinition & t.IsPublic;
                })) {
                    if (networkType1s.ContainsKey(type))
                        continue;
                    networkTypes.TryAdd((short)networkTypes.Count, type);
                    networkType1s.TryAdd(type, (short)networkType1s.Count);
                }
            }
        }

        /// <summary>
        /// 索引取类型
        /// </summary>
        /// <param name="typeIndex"></param>
        /// <returns></returns>
        private static Type IndexToType(short typeIndex)
        {
            if (!networkTypes.ContainsKey(typeIndex))
                return null;
            return networkTypes[typeIndex];
        }

        /// <summary>
        /// 类型取索引
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static short TypeToIndex(Type type)
        {
            if (type.IsArray) {
                var typeName = type.ToString().TrimEnd(']', '[');
                type = GetType(typeName);
            }
            if (!networkType1s.ContainsKey(type))
                throw new KeyNotFoundException($"网络类中没有注册[{type.ToString()}]类");
            return networkType1s[type];
        }

        /// <summary>
        /// 序列化基本类型
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static byte[] WriteValue(object value)
        {
            return WriteValue(value.GetType(), value);
        }

        /// <summary>
        /// 序列化基本类型
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static byte[] WriteValue(Type type, object value)
        {
            switch (type.ToString()) {
                case "System.Int32":
                    return BitConverter.GetBytes((int)value);
                case "System.Single":
                    return BitConverter.GetBytes((float)value);
                case "System.Boolean":
                    return BitConverter.GetBytes((bool)value);
                case "System.Char":
                    return BitConverter.GetBytes((char)value);
                case "System.Int16":
                    return BitConverter.GetBytes((short)value);
                case "System.Int64":
                    return BitConverter.GetBytes((long)value);
                case "System.UInt16":
                    return BitConverter.GetBytes((ushort)value);
                case "System.UInt32":
                    return BitConverter.GetBytes((uint)value);
                case "System.UInt64":
                    return BitConverter.GetBytes((ulong)value);
                case "System.Double":
                    return BitConverter.GetBytes((double)value);
                case "System.Byte":
                    return new byte[] { (byte)value };
                case "System.SByte":
                    return BitConverter.GetBytes((sbyte)value);
            }
            throw new IOException("试图写入的类型不是基本类型!");
        }

        /// <summary>
        /// 反序列化基本类型
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private static object ToBaseValue(byte[] buffer, ref int index)
        {
            var type = IndexToType(BitConverter.ToInt16(buffer, index));
            index += 2;
            return ToBaseValue(type, buffer, ref index);
        }

        /// <summary>
        /// 反序列化基本类型
        /// </summary>
        /// <param name="type"></param>
        /// <param name="buffer"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private static object ToBaseValue(Type type, byte[] buffer, ref int index)
        {
            object obj = null;
            switch (type.ToString()) {
                case "System.Int32":
                    obj = BitConverter.ToInt32(buffer, index);
                    index += 4;
                    break;
                case "System.Single":
                    obj = BitConverter.ToSingle(buffer, index);
                    index += 4;
                    break;
                case "System.Boolean":
                    obj = BitConverter.ToBoolean(buffer, index);
                    index += 1;
                    break;
                case "System.Char":
                    obj = BitConverter.ToChar(buffer, index);
                    index += 2;
                    break;
                case "System.Int16":
                    obj = BitConverter.ToInt16(buffer, index);
                    index += 2;
                    break;
                case "System.Int64":
                    obj = BitConverter.ToInt64(buffer, index);
                    index += 8;
                    break;
                case "System.UInt16":
                    obj = BitConverter.ToUInt16(buffer, index);
                    index += 2;
                    break;
                case "System.UInt32":
                    obj = BitConverter.ToUInt32(buffer, index);
                    index += 4;
                    break;
                case "System.UInt64":
                    obj = BitConverter.ToUInt64(buffer, index);
                    index += 8;
                    break;
                case "System.Double":
                    obj = BitConverter.ToDouble(buffer, index);
                    index += 8;
                    break;
                case "System.Byte":
                    obj = buffer[index];
                    index += 1;
                    break;
                case "System.SByte":
                    obj = buffer[index];
                    index += 1;
                    break;
            }
            return obj;
        }

        /// <summary>
        /// 序列化数组实体
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="array"></param>
        private static void WriteArray(ref MemoryStream stream, Array array)
        {
            var count = BitConverter.GetBytes(array.Length);//数组长度
            stream.Write(count, 0, 4);//写入数组长度
            foreach (object arr in array) {
                if (arr == null)//如果数组值为空
                {
                    stream.WriteByte(0);//写入0代表空值
                    continue;
                }
                stream.WriteByte(1);//写入1代表有值

                Type type = arr.GetType();
                var typeBytes = BitConverter.GetBytes(TypeToIndex(type));
                if (type.IsPrimitive)//基本类型
                {
                    var value = WriteValue(type, arr);//转字节数组
                    stream.Write(typeBytes, 0, 2);//写入类型索引
                    stream.Write(value, 0, value.Length);//写入值
                } else if (type.IsEnum)//枚举类型
                  {
                    stream.Write(typeBytes, 0, 2);//写入类型索引
                    stream.WriteByte((byte)arr.GetHashCode());//写入值
                } else if (type == typeof(string))//字符串类型
                  {
                    byte[] strBytes = Encoding.Unicode.GetBytes(arr.ToString());//获取字节数组
                    stream.Write(typeBytes, 0, 2);//写入类型索引
                    stream.Write(BitConverter.GetBytes(strBytes.Length), 0, 4);//写入字符串长度
                    stream.Write(strBytes, 0, strBytes.Length);//写入字符串
                } else if (type.IsArray)//数组类型
                  {
                    Array array1 = (Array)arr;
                    stream.Write(typeBytes, 0, 2);//写入类型索引
                    stream.Write(BitConverter.GetBytes(array1.Length), 0, 4);//写入数组长度
                    WriteArray(ref stream, array1);//写入值
                } else if (networkType1s.ContainsKey(type))//如果是序列化类型才进行序列化
                  {
                    if (type.IsGenericType)//泛型类型
                    {
                        stream.Write(typeBytes, 0, 2);//写入类型索引
                        if (type.ToString().Contains("Dictionary")) {
                            object dicKeys = type.GetProperty("Keys").GetValue(arr);
                            Type keyType = dicKeys.GetType();
                            int count1 = (int)keyType.GetProperty("Count").GetValue(dicKeys);
                            Array keys = Array.CreateInstance(type.GenericTypeArguments[0], count1);
                            keyType.GetMethod("CopyTo").Invoke(dicKeys, new object[] { keys, 0 });
                            object dicValues = type.GetProperty("Values").GetValue(arr);
                            Type valuesType = dicValues.GetType();
                            Array values = Array.CreateInstance(type.GenericTypeArguments[1], count1);
                            valuesType.GetMethod("CopyTo").Invoke(dicValues, new object[] { values, 0 });
                            WriteArray(ref stream, keys);
                            WriteArray(ref stream, values);
                            continue;
                        }
                        Array array1 = (Array)type.GetMethod("ToArray").Invoke(arr, null);
                        WriteArray(ref stream, array1);
                        continue;
                    }
                    if (SerializeHandle(type, arr, out byte[] buffer)) {
                        stream.Write(typeBytes, 0, 2);//写入参数类型索引
                        stream.Write(buffer, 0, buffer.Length);//写入参数值
                        continue;
                    }
                    stream.Write(typeBytes, 0, 2);//写入类型索引
                    WriteObject(ref stream, type, arr);//写入实体类型
                }
            }
        }

        /// <summary>
        /// 反序列化数组
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="index"></param>
        /// <param name="type"></param>
        private static Array ToArray(byte[] buffer, ref int index, Type type)
        {
            var arrCount = BitConverter.ToInt16(buffer, index);
            index += 2;
            Array array = Array.CreateInstance(type, arrCount);
            ToArray(buffer, ref index, ref array);
            return array;
        }

        /// <summary>
        /// 反序列化数组
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="index"></param>
        /// <param name="array"></param>
        private static void ToArray(byte[] buffer, ref int index, ref Array array)
        {
            for (int i = 0; i < array.Length; i++) {
                index += 1;
                if (buffer[index - 1] == 0)
                    continue;

                var type = IndexToType(BitConverter.ToInt16(buffer, index));
                index += 2;
                if (type.IsPrimitive) {
                    array.SetValue(ToBaseValue(type, buffer, ref index), i);
                } else if (type.IsEnum) {
                    array.SetValue(ToEnum(buffer, ref index), i);
                } else if (type == typeof(string)) {
                    array.SetValue(ToString(buffer, ref index), i);
                } else if (networkType1s.ContainsKey(type)) {
                    if (type.IsGenericType) {
                        var arrCount1 = BitConverter.ToInt32(buffer, index);
                        index += 4;
                        object list = Activator.CreateInstance(type);
                        if (type.ToString().Contains("Dictionary")) {
                            Type dicType = list.GetType();
                            Type keysT = type.GenericTypeArguments[0];
                            Type valuesT = type.GenericTypeArguments[1];
                            Array keys = Array.CreateInstance(keysT, arrCount1);
                            Array values = Array.CreateInstance(valuesT, arrCount1);
                            ToArray(buffer, ref index, ref keys);
                            index += 4;
                            ToArray(buffer, ref index, ref values);
                            for (int a = 0; a < keys.Length; a++) {
                                dicType.GetMethod("Add").Invoke(list, new object[] { keys.GetValue(a), values.GetValue(a) });
                            }
                        } else {
                            Type itemType = type.GenericTypeArguments[0];
                            Array array1 = Array.CreateInstance(itemType, arrCount1);
                            ToArray(buffer, ref index, ref array1);
                            var met = type.GetMethod("AddRange");
                            met.Invoke(list, new object[] { array1 });
                        }
                        array.SetValue(list, i);
                    } else
                        array.SetValue(ToObject(buffer, ref index, type), i);
                }
            }
        }

        /// <summary>
        /// 反序列化枚举类型
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private static object ToEnum(byte[] buffer, ref int index)
        {
            var type = IndexToType(BitConverter.ToInt16(buffer, index));
            index += 2;
            object obj = Enum.ToObject(type, buffer[index]);
            index += 1;
            return obj;
        }

        /// <summary>
        /// 反序列化字符串
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private static string ToString(byte[] buffer, ref int index)
        {
            var count = BitConverter.ToInt32(buffer, index);
            index += 4;
            string str = Encoding.Unicode.GetString(buffer, index, count);
            index += count;
            return str;
        }

        /// <summary>
        /// 新版网络序列化
        /// </summary>
        /// <param name="pars"></param>
        /// <returns></returns>
        public new static byte[] Serialize(params object[] pars)
        {
            return Serialize(string.Empty, pars);
        }

        [ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllFields)]
        internal class Data
        {
            public string type;
            public byte[] buffer;
        }

        private static readonly Assembly assembly = typeof(UnityEngine.Vector2).Assembly;

        /// <summary>
        /// 新版网络序列化
        /// </summary>
        /// <param name="funcName">函数名</param>
        /// <param name="pars">参数</param>
        /// <returns></returns>
        public new static byte[] Serialize(string funcName, params object[] pars)
        {
            using (MemoryStream stream = new MemoryStream()) {
                try {
                    ProtoBuf.Serializer.Serialize(stream, funcName);
                    List<Data> datas = new List<Data> {
                        new Data() { type = "System.String", buffer = stream.ToArray() }
                    };
                    stream.SetLength(0);
                    foreach (var obj in pars) {
                        var type = obj.GetType();
                        Data data = new Data() { type = type.ToString() };
                        if (type.Assembly == assembly) {
                            var bts = SerializeComplex(obj);
                            data.buffer = bts;
                            datas.Add(data);
                        } else {
                            ProtoBuf.Serializer.Serialize(stream, obj);
                            data.buffer = stream.ToArray();
                            datas.Add(data);
                        }
                        stream.SetLength(0);
                    }
                    ProtoBuf.Serializer.Serialize(stream, datas);
                    return stream.ToArray();
                } catch (Exception e) {
                    DeBug?.Invoke(e.ToString());
                    return new byte[0];
                }
            }
        }

        public static event Action<string> DeBug;

        //序列化复杂类型的, 如unity的类型
        private static byte[] SerializeComplex(object par)
        {
            MemoryStream stream = new MemoryStream();
            byte[] buffers = new byte[0];
            try {
                if (par == null)
                    return stream.ToArray();
                Type type = par.GetType();
                var typeBytes = BitConverter.GetBytes(TypeToIndex(type));
                if (type.IsPrimitive)//基本类型
                {
                    var parValue = WriteValue(type, par);//获取基本类型字节数组
                    stream.WriteByte(0);//记录类型为基类
                    stream.Write(typeBytes, 0, 2);//写入类型索引
                    stream.Write(parValue, 0, parValue.Length);//写入值
                } else if (type.IsEnum)//枚举类型
                  {
                    stream.WriteByte(1);//记录类型为枚举类
                    stream.Write(typeBytes, 0, 2);//写入类型索引
                    stream.WriteByte((byte)par.GetHashCode());//写入值
                } else if (type.IsArray)//数组类型
                  {
                    stream.WriteByte(2);//记录类型为数组
                    stream.Write(typeBytes, 0, 2);//写入类型索引
                    WriteArray(ref stream, (Array)par);//写入值
                } else if (par is string)//字符串类型
                  {
                    stream.WriteByte(3);//记录类型为字符串
                    stream.Write(typeBytes, 0, 2);//写入数类型索引
                    byte[] strBytes = Encoding.Unicode.GetBytes(par.ToString());//获取字节数组
                    stream.Write(BitConverter.GetBytes(strBytes.Length), 0, 4);//记录字符串长度
                    stream.Write(strBytes, 0, strBytes.Length);//写入字符串
                } else if (networkType1s.ContainsKey(type))//序列化的类型
                  {
                    if (type.IsGenericType)//泛型类型 只支持List
                    {
                        stream.WriteByte(4);//记录类型为泛型
                        stream.Write(typeBytes, 0, 2);//写入类型索引
                        if (type.ToString().Contains("Dictionary")) {
                            object dicKeys = type.GetProperty("Keys").GetValue(par);
                            Type keyType = dicKeys.GetType();
                            int count = (int)keyType.GetProperty("Count").GetValue(dicKeys);
                            Array keys = Array.CreateInstance(type.GenericTypeArguments[0], count);
                            keyType.GetMethod("CopyTo").Invoke(dicKeys, new object[] { keys, 0 });
                            object dicValues = type.GetProperty("Values").GetValue(par);
                            Type valuesType = dicValues.GetType();
                            Array values = Array.CreateInstance(type.GenericTypeArguments[1], count);
                            valuesType.GetMethod("CopyTo").Invoke(dicValues, new object[] { values, 0 });
                            WriteArray(ref stream, keys);
                            WriteArray(ref stream, values);
                            goto JUMP;
                        }
                        Array array = (Array)type.GetMethod("ToArray").Invoke(par, null);
                        WriteArray(ref stream, array);
                        goto JUMP;
                    }
                    if (SerializeHandle(type, par, out byte[] buffer)) {
                        stream.WriteByte(5);//记录类型为自定义类
                        stream.Write(typeBytes, 0, 2);//写入参数类型索引
                        stream.Write(buffer, 0, buffer.Length);//写入参数值
                        goto JUMP;
                    }
                    stream.WriteByte(6);//记录类型为自定义类
                    stream.Write(typeBytes, 0, 2);//写入类型索引
                    WriteObject(ref stream, type, par);//写入实体类型
                }
                JUMP: buffers = stream.ToArray();
            } finally {
                stream.Dispose();
                stream.Close();
            }
            return buffers;
        }

        /// <summary>
        /// 序列化实体类型
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="type"></param>
        /// <param name="target"></param>
        private static void WriteObject(ref MemoryStream stream, Type type, object target)
        {
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            for (int i = 0; i < fields.Length; i++) {
                var value = fields[i].GetValue(target);
                if (value == null)//如果实体字段为空，不记录
                    continue;

                if (fields[i].FieldType.IsPrimitive)//如果是基础类型
                {
                    var parValue = WriteValue(fields[i].FieldType, value);
                    stream.WriteByte((byte)i);//写入字段索引
                    stream.Write(parValue, 0, parValue.Length);//写入字段值
                } else if (fields[i].FieldType.IsEnum)//枚举类型
                  {
                    stream.WriteByte((byte)i);//写入字段索引
                    stream.WriteByte((byte)value.GetHashCode());//写入字段值
                } else if (fields[i].FieldType == typeof(string))//字符串类型
                  {
                    var parValue = Encoding.Unicode.GetBytes(value.ToString());//获取字符串字节数组
                    stream.WriteByte((byte)i);//写入字段索引
                    stream.Write(BitConverter.GetBytes(parValue.Length), 0, 4);//记录字符串长度
                    stream.Write(parValue, 0, parValue.Length);//写入字段值
                } else if (fields[i].FieldType.IsArray)//如果是数组
                  {
                    Array array = value as Array;
                    stream.WriteByte((byte)i);//写入字段索引
                    stream.Write(BitConverter.GetBytes(TypeToIndex(fields[i].FieldType)), 0, 2);//写入数组类型
                    WriteArray(ref stream, array);
                } else if (networkType1s.ContainsKey(fields[i].FieldType))//如果是序列化类型才进行序列化
                  {
                    stream.WriteByte((byte)i);//写入字段索引
                    if (fields[i].FieldType.IsGenericType)//泛型类型
                    {
                        if (fields[i].FieldType.ToString().Contains("Dictionary")) {
                            object dicKeys = fields[i].FieldType.GetProperty("Keys").GetValue(value);
                            Type keyType = dicKeys.GetType();
                            int count = (int)keyType.GetProperty("Count").GetValue(dicKeys);
                            Array keys = Array.CreateInstance(fields[i].FieldType.GenericTypeArguments[0], count);
                            keyType.GetMethod("CopyTo").Invoke(dicKeys, new object[] { keys, 0 });
                            object dicValues = fields[i].FieldType.GetProperty("Values").GetValue(value);
                            Type valuesType = dicValues.GetType();
                            Array values = Array.CreateInstance(fields[i].FieldType.GenericTypeArguments[1], count);
                            valuesType.GetMethod("CopyTo").Invoke(dicValues, new object[] { values, 0 });
                            WriteArray(ref stream, keys);
                            WriteArray(ref stream, values);
                            continue;
                        }
                        Array array = (Array)fields[i].FieldType.GetMethod("ToArray").Invoke(value, null);
                        WriteArray(ref stream, array);
                        continue;
                    }
                    if (SerializeHandle(fields[i].FieldType, value, out byte[] buffer))
                        stream.Write(buffer, 0, buffer.Length);//写入字段值
                    else
                        WriteObject(ref stream, fields[i].FieldType, value);
                }
            }
            stream.WriteByte(255);//写入字段结束值
        }

        /// <summary>
        /// 新版反序列化
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static FuncData Deserialize(byte[] buffer)
        {
            return Deserialize(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// 新版反序列化
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="func"></param>
        public static void Deserialize(byte[] buffer, Action<string, object[]> func)
        {
            Deserialize(buffer, 0, buffer.Length, func);
        }

        /// <summary>
        /// 新版反序列化
        /// </summary>
        /// <param name="buffer">字节数据</param>
        /// <param name="index">字节从哪个位置开始</param>
        /// <param name="count">字节大小</param>
        /// <returns></returns>
        public static FuncData Deserialize(byte[] buffer, int index, int count)
        {
            FuncData func = new FuncData();
            Deserialize(buffer, index, count, (funname, pars) => {
                func = new FuncData(funname, pars);
            });
            return func;
        }

        /// <summary>
        /// 新版反序列化
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <param name="func"></param>
        public static void Deserialize(byte[] buffer, int index, int count, Action<string, object[]> func)
        {
            using (MemoryStream stream = new MemoryStream(buffer, index, count)) {
                var datas = ProtoBuf.Serializer.Deserialize<List<Data>>(stream);
                List<object> pars = new List<object>();
                foreach (var data in datas) {
                    using (MemoryStream ms = new MemoryStream(data.buffer)) {
                        var type = GetTypeAll(data.type);
                        if (type == null) {
                            pars.Add(null);
                            continue;
                        }
                        if (type.Assembly == assembly) {
                            DeserializeComplex(data.buffer, 0, data.buffer.Length, (obj1) => {
                                pars.Add(obj1);
                            });
                        } else {
                            var obj = ProtoBuf.Serializer.Deserialize(type, ms);
                            pars.Add(obj);
                        }
                    }
                }
                var funName = pars[0].ToString();
                pars.RemoveAt(0);
                func?.Invoke(funName, pars.ToArray());
            }
        }

        /// <summary>
        /// 反序列化复杂类型
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <param name="func"></param>
        private static void DeserializeComplex(byte[] buffer, int index, int count, Action<object> par)
        {
            object obj = null;
            while (index < count) {
                var pro = buffer[index];
                index += 1;
                var type = IndexToType(BitConverter.ToInt16(buffer, index));
                if (type == null)
                    break;
                index += 2;
                switch (pro) {
                    case 0:
                        obj = ToBaseValue(type, buffer, ref index);
                        break;
                    case 1:
                        obj = Enum.ToObject(type, buffer[index]);
                        index += 1;
                        break;
                    case 2:
                        var arrCount = BitConverter.ToInt32(buffer, index);
                        index += 4;
                        Array array = Array.CreateInstance(type, arrCount);
                        ToArray(buffer, ref index, ref array);
                        obj = array;
                        break;
                    case 3:
                        var strCount = BitConverter.ToInt32(buffer, index);
                        index += 4;
                        obj = Encoding.Unicode.GetString(buffer, index, strCount);
                        index += strCount;
                        break;
                    case 4:
                        var arrCount1 = BitConverter.ToInt32(buffer, index);
                        index += 4;
                        object list = Activator.CreateInstance(type);
                        if (type.ToString().Contains("Dictionary")) {
                            Type dicType = list.GetType();
                            Type keysT = type.GenericTypeArguments[0];
                            Type valuesT = type.GenericTypeArguments[1];
                            Array keys = Array.CreateInstance(keysT, arrCount1);
                            Array values = Array.CreateInstance(valuesT, arrCount1);
                            ToArray(buffer, ref index, ref keys);
                            index += 4;
                            ToArray(buffer, ref index, ref values);
                            for (int i = 0; i < keys.Length; i++) {
                                dicType.GetMethod("Add").Invoke(list, new object[] { keys.GetValue(i), values.GetValue(i) });
                            }
                        } else {
                            Type itemType = type.GenericTypeArguments[0];
                            Array array1 = Array.CreateInstance(itemType, arrCount1);
                            ToArray(buffer, ref index, ref array1);
                            var met = type.GetMethod("AddRange");
                            met.Invoke(list, new object[] { array1 });
                        }
                        obj = list;
                        break;
                    case 5:
                        DeserializeHandle(buffer, ref index, type, out object obj2);
                        obj = obj2;
                        break;
                    case 6:
                        var obj1 = ToObject(buffer, ref index, type);
                        obj = obj1;
                        break;
                }
            }
            par(obj);
        }

        /// <summary>
        /// 反序列化实体对象
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private static object ToObject(byte[] buffer, ref int index)
        {
            var type = IndexToType(BitConverter.ToInt16(buffer, index));
            index += 2;
            return ToObject(buffer, ref index, type);
        }

        /// <summary>
        /// 反序列化实体对象
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="index"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static object ToObject(byte[] buffer, ref int index, Type type)
        {
            object obj = Activator.CreateInstance(type);
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            for (int i = 0; i < fields.Length; i++) {
                if (buffer[index] == 255)//字段结束符
                    break;

                if (i != buffer[index])//如果字段匹配不对，跳过
                    continue;

                index += 1;

                if (fields[i].FieldType.IsPrimitive)//如果是基础类型
                {
                    fields[i].SetValue(obj, ToBaseValue(fields[i].FieldType, buffer, ref index));
                } else if (fields[i].FieldType.IsEnum)//如果是枚举类型
                  {
                    fields[i].SetValue(obj, Enum.ToObject(fields[i].FieldType, buffer[index]));
                    index += 1;
                } else if (fields[i].FieldType == typeof(string))//如果是字符串
                  {
                    var count = BitConverter.ToInt32(buffer, index);
                    index += 4;
                    fields[i].SetValue(obj, Encoding.Unicode.GetString(buffer, index, count));
                    index += count;
                } else if (fields[i].FieldType.IsArray)//如果是数组类型
                  {
                    var arrType = IndexToType(BitConverter.ToInt16(buffer, index));
                    index += 2;
                    var arrCount = BitConverter.ToInt32(buffer, index);
                    index += 4;
                    Array array = Array.CreateInstance(arrType, arrCount);
                    ToArray(buffer, ref index, ref array);
                    fields[i].SetValue(obj, array);
                } else if (networkType1s.ContainsKey(fields[i].FieldType))//如果是序列化类型
                  {
                    if (fields[i].FieldType.IsGenericType) {
                        if (fields[i].FieldType.GenericTypeArguments.Length == 2)
                            fields[i].SetValue(obj, ToDictionary(buffer, ref index, fields[i].FieldType));
                        else
                            fields[i].SetValue(obj, ToList(buffer, ref index, fields[i].FieldType));
                        continue;
                    }
                    if (DeserializeHandle(buffer, ref index, fields[i].FieldType, out object value))
                        fields[i].SetValue(obj, value);
                    else
                        fields[i].SetValue(obj, ToObject(buffer, ref index, fields[i].FieldType));
                }
            }
            index += 1;
            return obj;
        }

        /// <summary>
        /// 反序列化泛型
        /// </summary>
        private static object ToList(byte[] buffer, ref int index, Type type)
        {
            var arrCount1 = BitConverter.ToInt32(buffer, index);
            index += 4;
            object list = Activator.CreateInstance(type);
            Type itemType = type.GenericTypeArguments[0];
            Array array1 = Array.CreateInstance(itemType, arrCount1);
            ToArray(buffer, ref index, ref array1);
            var met = type.GetMethod("AddRange");
            met.Invoke(list, new object[] { array1 });
            return list;
        }

        /// <summary>
        /// 反序列化字典类型
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="index"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static object ToDictionary(byte[] buffer, ref int index, Type type)
        {
            var arrCount1 = BitConverter.ToInt32(buffer, index);
            index += 4;
            object dic = Activator.CreateInstance(type);
            Type keysT = type.GenericTypeArguments[0];
            Type valuesT = type.GenericTypeArguments[1];
            Array keys = Array.CreateInstance(keysT, arrCount1);
            Array values = Array.CreateInstance(valuesT, arrCount1);
            ToArray(buffer, ref index, ref keys);
            index += 4;
            ToArray(buffer, ref index, ref values);
            for (int i = 0; i < keys.Length; i++) {
                type.GetMethod("Add").Invoke(dic, new object[] { keys.GetValue(i), values.GetValue(i) });
            }
            return dic;
        }
    }
}