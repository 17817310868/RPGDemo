namespace Net.Share
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text;

    /// <summary>
    /// 函数数据
    /// </summary>
    [ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllFields)]
    public struct FuncData
    {
        /// <summary>
        /// 函数名称
        /// </summary>
        public string name;
        /// <summary>
        /// 参数数组
        /// </summary>
        public object[] pars;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="func"></param>
        /// <param name="pars"></param>
        public FuncData(string func, object[] pars)
        {
            this.name = func;
            this.pars = pars;
        }
    }

    /// <summary>
    /// 旧版网络转换，字符串转换
    /// </summary>
    public class NetConvertOld : NetConvertBase
    {
        private static Dictionary<string, Type> Types = new Dictionary<string, Type>();
        public static string Split = "\n---------------------------------\n";
        public static string FieldEnd = "\n----------GDNet:19.11.6----------\n";
        public static string NullValue = "\n----------QQGroup:825240544----------\n";
        public static string HasValue = "\n----------QQ:1752062104----------\n";

        /// <summary>
        /// 添加系列化类型,  当复杂类型时,如果不进行添加则系列化失败: 主要类型 Dictionary
        /// </summary>
        public static void AddSerializeType<T>()
        {
            AddSerializeType(typeof(T));
        }

        /// <summary>
        /// 添加系列化类型,  当复杂类型时,如果不进行添加则系列化失败: 主要类型 Dictionary
        /// </summary>
        /// <param name="type"></param>
        public static void AddSerializeType(Type type)
        {
            string typeString = type.ToString();
            if (!Types.ContainsKey(typeString))
                Types.Add(typeString, type);
        }

        /// <summary>
        /// 解释 : 获得应用程序当前已加载的所有程序集中查找typeName的类型
        /// </summary>
        public static Type GetType(string typeName)
        {
            //代码优化
            if (Types.ContainsKey(typeName))
                return Types[typeName];
            string typeName1 = typeName;
            typeName1 = typeName1.Replace("&", ""); // 反射参数的 out 标示
            typeName1 = typeName1.Replace("*", ""); // 反射参数的 int*(指针) 标示
            typeName1 = typeName1.Replace("[]", ""); // 反射参数的 object[](数组) 标示
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                Type type = assembly.GetType(typeName1);
                if (type != null) {
                    Types.Add(typeName, type);
                    return type;
                }
            }
            throw new Exception($"找不到类型:{typeName}, 类型太复杂时需要使用 NetConvertOld.AddSerializeType(type) 添加类型后再进行系列化!");
        }

        /// <summary>
        /// 解释 : 获得应用程序当前已加载的所有程序集中查找typeName的类型
        /// </summary>
        public static Type GetTypeAll(string typeName)
        {
            //代码优化
            if (Types.ContainsKey(typeName))
                return Types[typeName];
            var type1 = Type.GetType(typeName);
            if (type1 != null) {
                Types.Add(typeName, type1);
                return type1;
            }
            string typeName1 = typeName;
            typeName1 = typeName1.Replace("&", ""); // 反射参数的 out 标示
            typeName1 = typeName1.Replace("*", ""); // 反射参数的 int*(指针) 标示
            typeName1 = typeName1.Replace("[]", ""); // 反射参数的 object[](数组) 标示
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                Type type = assembly.GetType(typeName1);
                if (type != null) {
                    Types.Add(typeName, type);
                    return type;
                }
            }
            throw new Exception($"找不到类型:{typeName}, 类型太复杂时需要使用 NetConvertOld.AddSerializeType(type) 添加类型后再进行系列化!");
        }


        /// <summary>
        /// 反序列化基本类型
        /// </summary>
        /// <returns></returns>
        private static object ToBaseValue(Type type, string value)
        {
            object obj = null;
            switch (type.ToString()) {
                case "System.Int32":
                    obj = Convert.ToInt32(value);
                    break;
                case "System.Single":
                    obj = Convert.ToSingle(value);
                    break;
                case "System.Boolean":
                    obj = Convert.ToBoolean(value);

                    break;
                case "System.Char":
                    obj = Convert.ToChar(value);

                    break;
                case "System.Int16":
                    obj = Convert.ToInt16(value);

                    break;
                case "System.Int64":
                    obj = Convert.ToInt64(value);

                    break;
                case "System.UInt16":
                    obj = Convert.ToUInt16(value);

                    break;
                case "System.UInt32":
                    obj = Convert.ToUInt32(value);

                    break;
                case "System.UInt64":
                    obj = Convert.ToUInt64(value);

                    break;
                case "System.Double":
                    obj = Convert.ToDouble(value);

                    break;
                case "System.Byte":
                    obj = Convert.ToByte(value);

                    break;
                case "System.SByte":
                    obj = Convert.ToSByte(value);

                    break;
            }
            return obj;
        }

        /// <summary>
        /// 序列化数组实体
        /// </summary>
        private static void WriteArray(ref StringBuilder stream, Array array)
        {
            AppendLine(ref stream, array.Length.ToString());//写入数组长度
            foreach (object arr in array) {
                if (arr == null)//如果数组值为空
                {
                    AppendLine(ref stream, NullValue);//写入0代表空值
                    continue;
                }
                AppendLine(ref stream, HasValue);//写入1代表有值

                Type type = arr.GetType();
                if (type.IsPrimitive)//基本类型
                {
                    AppendLine(ref stream, type.ToString(), arr.ToString());
                } else if (type.IsEnum)//枚举类型
                  {
                    AppendLine(ref stream, type.ToString(), arr.ToString());
                } else if (type == typeof(string))//字符串类型
                  {
                    AppendLine(ref stream, type.ToString(), arr.ToString());
                } else if (type.IsArray)//数组类型
                  {
                    Array array1 = (Array)arr;
                    AppendLine(ref stream, type.ToString(), array1.Length.ToString());
                    WriteArray(ref stream, array1);//写入值
                } else {
                    AppendLine(ref stream, type.ToString());//写入参数类型索引
                    WriteObject(ref stream, type, arr);
                }
            }
        }

        /// <summary>
        /// 反序列化数组
        /// </summary>
        private static Array ToArray(string[] buffer, ref int index, Type type)
        {
            var arrCount = Convert.ToInt16(buffer[index]);
            index++;
            Array array = Array.CreateInstance(type, arrCount);
            ToArray(buffer, ref index, ref array);
            return array;
        }

        /// <summary>
        /// 反序列化数组
        /// </summary>
        private static void ToArray(string[] buffer, ref int index, ref Array array)
        {
            for (int i = 0; i < array.Length; i++) {
                index++;
                if (buffer[index - 1] == NullValue)
                    continue;
                var value = buffer[index];
                value = value.Trim('{', '}');
                var values = value.Split(new string[] { "}*{" }, StringSplitOptions.None);
                var type = GetType(values[0]);
                index++;
                if (type.IsPrimitive) {
                    array.SetValue(ToBaseValue(type, values[1]), i);
                } else if (type.IsEnum) {
                    array.SetValue(Enum.Parse(type, values[1]), i);
                } else if (type == typeof(string)) {
                    array.SetValue(values[1], i);
                } else {
                    array.SetValue(ToObject(buffer, ref index, type), i);
                }
            }
        }

        /// <summary>
        /// 反序列化枚举类型
        /// </summary>
        private static object ToEnum(string[] buffer, ref int index)
        {
            var type = GetType(buffer[index]);
            object obj = Enum.Parse(type, buffer[index]);
            index++;
            return obj;
        }

        static void AppendLine(ref StringBuilder stream, string str)
        {
            stream.Append(str + Split);
        }

        static void AppendLine(ref StringBuilder stream, string type, string value)
        {
            stream.Append("{" + type + "}*{" + value + "}" + Split);
        }

        /// <summary>
        /// 新版网络序列化
        /// </summary>
        /// <param name="pars"></param>
        /// <returns></returns>
        public static string Serialize(params object[] pars)
        {
            return Serialize(string.Empty, pars);
        }

        /// <summary>
        /// 新版网络序列化
        /// </summary>
        /// <param name="funcName">函数名</param>
        /// <param name="pars">参数</param>
        /// <returns></returns>
        public static string Serialize(string funcName, params object[] pars)
        {
            StringBuilder stream = new StringBuilder();
            try {
                AppendLine(ref stream, funcName);//写入函数名
                if (pars == null)
                    return stream.ToString();
                foreach (object par in pars) {
                    Type type = par.GetType();
                    if (type.IsPrimitive)//基本类型
                    {
                        AppendLine(ref stream, "基本类型");//记录类型为基类
                        AppendLine(ref stream, type.ToString());//写入类型索引
                        AppendLine(ref stream, par.ToString());//写入值
                    } else if (type.IsEnum)//枚举类型
                      {
                        AppendLine(ref stream, "枚举类型");//记录类型为枚举类
                        AppendLine(ref stream, type.ToString());//写入类型索引
                        AppendLine(ref stream, par.ToString());//写入值
                    } else if (type.IsArray)//数组类型
                      {
                        AppendLine(ref stream, "数组类型");//记录类型为数组
                        AppendLine(ref stream, type.ToString());//写入类型索引
                        WriteArray(ref stream, (Array)par);//写入值
                    } else if (par is string)//字符串类型
                      {
                        AppendLine(ref stream, "字符串类型");//记录类型为字符串
                        AppendLine(ref stream, type.ToString());//写入类型索引
                        AppendLine(ref stream, par.ToString());//写入字符串
                    } else {
                        if (type.IsGenericType)//泛型类型 只支持List
                        {
                            AppendLine(ref stream, "泛型");//记录类型为泛型
                            AppendLine(ref stream, type.ToString());//写入类型索引
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
                                continue;
                            }
                            Array array = (Array)type.GetMethod("ToArray").Invoke(par, null);
                            WriteArray(ref stream, array);
                            continue;
                        }
                        AppendLine(ref stream, "自定义类型");//记录类型为自定义类
                        AppendLine(ref stream, type.ToString());//写入类型索引
                        WriteObject(ref stream, type, par);//写入实体类型
                    }
                }
            } finally { }
            return stream.ToString();
        }

        /// <summary>
        /// 序列化实体类型
        /// </summary>
        private static void WriteObject(ref StringBuilder stream, Type type, object target)
        {
            var cons = type.GetConstructors();
            if (cons.Length == 0)
                return;
            if (cons[0].GetParameters().Length > 0 & !type.IsValueType)
                return;
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            for (int i = 0; i < fields.Length; i++) {
                var value = fields[i].GetValue(target);
                if (value == null)//如果实体字段为空，不记录
                    continue;
                if (fields[i].FieldType.IsPrimitive)//如果是基础类型
                {
                    AppendLine(ref stream, fields[i].Name, value.ToString());//写入字段名+字段值
                } else if (fields[i].FieldType.IsEnum)//枚举类型
                  {
                    AppendLine(ref stream, fields[i].Name, value.ToString());//写入字段名+字段值
                } else if (fields[i].FieldType == typeof(string))//字符串类型
                  {
                    AppendLine(ref stream, fields[i].Name, value.ToString());//写入字段名+字段值
                } else if (fields[i].FieldType.IsArray)//如果是数组
                  {
                    Array array = value as Array;
                    AppendLine(ref stream, fields[i].Name, fields[i].FieldType.ToString());//写入字段名+写入数组类型
                    WriteArray(ref stream, array);
                } else if (fields[i].FieldType.IsGenericType)//泛型类型 只支持List泛型类型
                  {
                    AppendLine(ref stream, fields[i].Name);//写入字段索引
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
                } else {
                    AppendLine(ref stream, fields[i].Name);//写入字段索引
                    WriteObject(ref stream, fields[i].FieldType, value);
                }
            }
            AppendLine(ref stream, FieldEnd);//写入字段结束值
        }

        /// <summary>
        /// 新版反序列化
        /// </summary>
        public static FuncData Deserialize(string buffer)
        {
            FuncData value = new FuncData();
            Deserialize(buffer, (func, pars) => {
                value = new FuncData(func, pars);
            });
            return value;
        }

        /// <summary>
        /// 新版反序列化
        /// </summary>
        public static void Deserialize(string buffer, Action<string, object[]> func)
        {
            string[] buffer1 = buffer.Split(new string[] { Split }, StringSplitOptions.None);
            Deserialize(buffer1, 0, buffer1.Length, func);
        }

        /// <summary>
        /// 新版反序列化
        /// </summary>
        public static FuncData Deserialize(string[] buffer, int index, int count)
        {
            FuncData value = new FuncData();
            Deserialize(buffer, index, count, (func, pars) => {
                value = new FuncData(func, pars);
            });
            return value;
        }

        /// <summary>
        /// 新版反序列化
        /// </summary>
        public static void Deserialize(string[] buffer, int index, int count, Action<string, object[]> func)
        {
            List<object> objs = new List<object>();
            var funcName = buffer[index];
            index++;
            while (index < count - 1) {
                var pro = buffer[index];
                index++;
                var type = GetType(buffer[index]);
                if (type == null)
                    break;
                index++;
                var value = buffer[index];
                value = value.TrimStart('{');
                value = value.TrimEnd('}');
                var values1 = value.Split(new string[] { "}*{" }, StringSplitOptions.None);
                switch (pro) {
                    case "基本类型":
                        objs.Add(ToBaseValue(type, values1[1]));
                        break;
                    case "枚举类型":
                        objs.Add(Enum.Parse(type, buffer[index]));
                        index++;
                        break;
                    case "数组类型":
                        var arrCount = Convert.ToInt32(buffer[index]);
                        index++;
                        Array array = Array.CreateInstance(type, arrCount);
                        ToArray(buffer, ref index, ref array);
                        objs.Add(array);
                        break;
                    case "字符串类型":
                        objs.Add(buffer[index]);
                        index++;
                        break;
                    case "泛型":
                        var arrCount1 = Convert.ToInt32(buffer[index]);
                        index++;
                        object list = Activator.CreateInstance(type);
                        if (type.ToString().Contains("Dictionary")) {
                            Type dicType = list.GetType();
                            Type keysT = type.GenericTypeArguments[0];
                            Type valuesT = type.GenericTypeArguments[1];
                            Array keys = Array.CreateInstance(keysT, arrCount1);
                            Array values = Array.CreateInstance(valuesT, arrCount1);
                            ToArray(buffer, ref index, ref keys);
                            index++;
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
                        objs.Add(list);
                        break;
                    case "自定义类型":
                        var obj1 = ToObject(buffer, ref index, type);
                        objs.Add(obj1);
                        break;
                }
            }
            func(funcName, objs.ToArray());
        }

        /// <summary>
        /// 反序列化实体对象
        /// </summary>
        private static object ToObject(string[] buffer, ref int index, Type type)
        {
            var cons = type.GetConstructors();
            if (cons.Length == 0)
                return null;
            if (cons[0].GetParameters().Length > 0 & !type.IsValueType)
                return null;
            object obj = Activator.CreateInstance(type);
            while (buffer[index] != FieldEnd) {
                string name = buffer[index];
                name = name.TrimStart('{');
                name = name.TrimEnd('}');
                var value = name.Split(new string[] { "}*{" }, StringSplitOptions.None);
                index++;
                var field = type.GetField(value[0]);
                if (field.FieldType.IsPrimitive)//如果是基础类型
                {
                    field.SetValue(obj, ToBaseValue(field.FieldType, value[1]));
                } else if (field.FieldType.IsEnum)//如果是枚举类型
                    {
                    field.SetValue(obj, Enum.Parse(field.FieldType, value[1]));
                } else if (field.FieldType == typeof(string))//如果是字符串
                  {
                    field.SetValue(obj, value[1]);
                } else if (field.FieldType.IsArray)//如果是数组类型
                  {
                    var arrType = GetType(buffer[index]);
                    index++;
                    var arrCount = Convert.ToInt32(buffer[index]);
                    index++;
                    Array array = Array.CreateInstance(arrType, arrCount);
                    ToArray(buffer, ref index, ref array);
                    field.SetValue(obj, array);
                } else {
                    if (field.FieldType.IsGenericType) {
                        if (field.FieldType.GenericTypeArguments.Length == 2)
                            field.SetValue(obj, ToDictionary(buffer, ref index, field.FieldType));
                        else
                            field.SetValue(obj, ToList(buffer, ref index, field.FieldType));
                        continue;
                    }
                    field.SetValue(obj, ToObject(buffer, ref index, field.FieldType));
                }
            }
            index++;
            return obj;
        }

        /// <summary>
        /// 反序列化泛型
        /// </summary>
        private static object ToList(string[] buffer, ref int index, Type type)
        {
            var arrCount1 = Convert.ToInt16(buffer[index]);
            index++;
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
        private static object ToDictionary(string[] buffer, ref int index, Type type)
        {
            var arrCount1 = Convert.ToInt16(buffer[index]);
            index++;
            object dic = Activator.CreateInstance(type);
            Type keysT = type.GenericTypeArguments[0];
            Type valuesT = type.GenericTypeArguments[1];
            Array keys = Array.CreateInstance(keysT, arrCount1);
            Array values = Array.CreateInstance(valuesT, arrCount1);
            ToArray(buffer, ref index, ref keys);
            index++;
            ToArray(buffer, ref index, ref values);
            for (int i = 0; i < keys.Length; i++) {
                type.GetMethod("Add").Invoke(dic, new object[] { keys.GetValue(i), values.GetValue(i) });
            }
            return dic;
        }
    }
}