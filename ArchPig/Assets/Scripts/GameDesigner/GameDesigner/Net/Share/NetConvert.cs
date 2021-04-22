namespace Net.Share
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    /// <summary>
    /// 序列化数据标示
    /// </summary>
    [ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllFields)]
    public struct Data
    {
        public string type;
        public byte[] buffer;

        public Data(string type, byte[] buffer) 
        {
            this.type = type;
            this.buffer = buffer;
        }
    }

    /// <summary>
    /// 网络转换核心 2019.7.16
    /// </summary>
    public class NetConvert : NetConvertOld
    {
        private static readonly Assembly assembly = typeof(UnityEngine.Vector2).Assembly;
        private static Dictionary<string, List<string>> filter = new Dictionary<string, List<string>>();

        public class LimitPropsContractResolver : DefaultContractResolver
        {
            string[] props = null;
            readonly bool retain;

            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="props">传入的属性数组</param>
            /// <param name="retain">true:表示props是需要保留的字段  false：表示props是要排除的字段</param>
            public LimitPropsContractResolver(string[] props, bool retain = false)
            {
                //指定要序列化属性的清单
                this.props = props;
                this.retain = retain;
            }

            protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
            {
                IList<JsonProperty> list =
                base.CreateProperties(type, memberSerialization);
                //只保留清单有列出的属性
                return list.Where(p =>
                {
                    if (retain)
                        return props.Contains(p.PropertyName);
                    else
                        return !props.Contains(p.PropertyName);
                }).ToList();
            }
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
                    foreach (var obj in pars){
                        var type = obj.GetType();
                        Data data = new Data() { type = type.ToString() };
                        if(type.IsGenericType){
                            bool result = true;
                            foreach (var ga in type.GetGenericArguments()){
                                if ((!ga.IsPrimitive & type != typeof(string)) & ga.GetCustomAttribute<ProtoBuf.ProtoContractAttribute>() == null){
                                    result = false;
                                    break;
                                }
                            }
                            if(result){
                                ProtoBuf.Serializer.Serialize(stream, obj);
                                data.buffer = stream.ToArray();
                                datas.Add(data);
                            }else{
                                var bts = SerializeComplex(type, obj);
                                data.buffer = bts;
                                datas.Add(data);
                            }
                        }else{
                            var customAtt = type.GetCustomAttribute<ProtoBuf.ProtoContractAttribute>();
                            if (type.Assembly == assembly | (customAtt == null & !type.IsPrimitive & type != typeof(string))) {
                                var bts = SerializeComplex(type, obj);
                                data.buffer = bts;
                                datas.Add(data);
                            } else {
                                ProtoBuf.Serializer.Serialize(stream, obj);
                                data.buffer = stream.ToArray();
                                datas.Add(data);
                            }
                        }
                        stream.SetLength(0);
                    }
                    ProtoBuf.Serializer.Serialize(stream, datas);
                    return stream.ToArray();
                } catch (Exception e){
                    DeBug?.Invoke(e.ToString());
                    return new byte[0];
                }
            }
        }

        public static event Action<string> DeBug;

        //序列化复杂类型的, 如unity的类型
        private static byte[] SerializeComplex(Type type, object obj)
        {
            DateTime time = DateTime.Now.AddSeconds(5);
            JUMP: try {
                var jSetting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
                var typeName = type.ToString();
                if (filter.ContainsKey(typeName))
                    jSetting.ContractResolver = new LimitPropsContractResolver(filter[typeName].ToArray(), false);
                var jsonStr = JsonConvert.SerializeObject(obj, jSetting);
                return Encoding.Unicode.GetBytes(jsonStr);
            } catch (Exception e) {
                string ee = e.Message;
                //Error setting value to 'xxxxxxxxxxxxxxxxxx'xxxxxxxxxxxxx.
                if (ee.Contains("Error setting value to '")) {
                    var field = ee.Split(new string[] { "'" }, StringSplitOptions.None);
                    if (field.Length > 1)
                        ee = field[1].Split(new string[] { "." }, StringSplitOptions.None)[0];
                }
                //Error getting value from 'ScopeId' on 'System.Net.IPAddress'.
                if (ee.Contains("Error getting value from '")) {
                    var field = ee.Split(new string[] { "'" }, StringSplitOptions.None);
                    if (field.Length > 1)
                        ee = field[1].Split(new string[] { "." }, StringSplitOptions.None)[0];
                }
                //Self referencing loop detected for property 'normalized' with type 'UnityEngine.Vector3'. Path 'pos.normalized.normalized'.
                if (ee.Contains("Self referencing loop detected for property '")) {
                    var field = ee.Split(new string[] { "'" }, StringSplitOptions.None);
                    if (field.Length > 1)
                        ee = field[1].Split(new string[] { "." }, StringSplitOptions.None)[0];
                }
                //Self referencing loop detected with type 'bydrServer.BydrClient'. Path 'scene.Players'.
                if (ee.Contains("Self referencing loop detected with type '")) {
                    var field = ee.Split(new string[] { "'" }, StringSplitOptions.None);
                    if (field.Length > 3)
                        ee = field[3].Split(new string[] { "." }, StringSplitOptions.None)[0];
                }
                //Could not create an instance of type System.Net.EndPoint. Type is an interface or abstract class and cannot be instantiated. Path 'RemotePoint.e6881ad2-e201-3376-9c81-f3496100c170.AddressFamily', line 1, position 488.
                if (ee.Contains("Could not create an instance of type")) {
                    var field = ee.Split(new string[] { "'" }, StringSplitOptions.None);
                    if (field.Length > 1)
                        ee = field[1].Split(new string[] { "." }, StringSplitOptions.None)[0];
                }
                var typeName = type.ToString();
                if (!filter.ContainsKey(typeName))
                    filter.Add(typeName, new List<string>());
                filter[typeName].Add(ee);
                if (DateTime.Now > time)
                    return new byte[0];
                DeBug?.Invoke(e.ToString());
                goto JUMP;
            }
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
            Deserialize(buffer, index, count, (funname, pars)=>{
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
            if (index < 0 | count < 0 | count > buffer.Length)
                return;
            using (MemoryStream stream = new MemoryStream(buffer, index, count)) {
                var datas = ProtoBuf.Serializer.Deserialize<List<Data>>(stream);
                List<object> pars = new List<object>();
                foreach (var data in datas) {
                    using (MemoryStream ms = new MemoryStream(data.buffer)) {
                        var type = GetTypeAll(data.type);
                        if(type==null){
                            pars.Add(null);
                            continue;
                        }
                        if (type.IsGenericType) {
                            bool result = true;
                            foreach (var ga in type.GetGenericArguments()) {
                                if ((!ga.IsPrimitive & type != typeof(string)) & ga.GetCustomAttribute<ProtoBuf.ProtoContractAttribute>() == null) {
                                    result = false;
                                    break;
                                }
                            }
                            if (result) {
                                var obj = ProtoBuf.Serializer.Deserialize(type, ms);
                                pars.Add(obj);
                            } else {
                                var obj = DeserializeComplex(type, data.buffer, 0, data.buffer.Length);
                                pars.Add(obj);
                            }
                        }else{
                            var customAtt = type.GetCustomAttribute<ProtoBuf.ProtoContractAttribute>();
                            if (type.Assembly == assembly | (customAtt == null & !type.IsPrimitive & type != typeof(string))) {
                                var obj = DeserializeComplex(type, data.buffer, 0, data.buffer.Length);
                                pars.Add(obj);
                            } else {
                                var obj = ProtoBuf.Serializer.Deserialize(type, ms);
                                pars.Add(obj);
                            }
                        }
                    }
                }
                var funName = (string)pars[0];
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
        private static object DeserializeComplex(Type type, byte[] buffer, int index, int count)
        {
            DateTime time = DateTime.Now.AddSeconds(5);
            JUMP: try {
                var jSetting = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
                var typeName = type.ToString();
                if (filter.ContainsKey(typeName))
                    jSetting.ContractResolver = new LimitPropsContractResolver(filter[typeName].ToArray(), false);
                var jsonStr = Encoding.Unicode.GetString(buffer, index, count);
                return JsonConvert.DeserializeObject(jsonStr, type, jSetting);
            } catch (Exception e) {
                string ee = e.Message;
                //Error setting value to 'xxxxxxxxxxxxxxxxxx'xxxxxxxxxxxxx.
                if (ee.Contains("Error setting value to '")) {
                    var field = ee.Split(new string[] { "'" }, StringSplitOptions.None);
                    if (field.Length > 1)
                        ee = field[1].Split(new string[] { "." }, StringSplitOptions.None)[0];
                }
                //Error getting value from 'ScopeId' on 'System.Net.IPAddress'.
                if (ee.Contains("Error getting value from '")) {
                    var field = ee.Split(new string[] { "'" }, StringSplitOptions.None);
                    if (field.Length > 1)
                        ee = field[1].Split(new string[] { "." }, StringSplitOptions.None)[0];
                }
                //Self referencing loop detected for property 'normalized' with type 'UnityEngine.Vector3'. Path 'pos.normalized.normalized'.
                if (ee.Contains("Self referencing loop detected for property '")) {
                    var field = ee.Split(new string[] { "'" }, StringSplitOptions.None);
                    if (field.Length > 1)
                        ee = field[1].Split(new string[] { "." }, StringSplitOptions.None)[0];
                }
                //Self referencing loop detected with type 'bydrServer.BydrClient'. Path 'scene.Players'.
                if (ee.Contains("Self referencing loop detected with type '")) {
                    var field = ee.Split(new string[] { "'" }, StringSplitOptions.None);
                    if (field.Length > 3)
                        ee = field[3].Split(new string[] { "." }, StringSplitOptions.None)[0];
                }
                //Could not create an instance of type System.Net.EndPoint. Type is an interface or abstract class and cannot be instantiated. Path 'RemotePoint.e6881ad2-e201-3376-9c81-f3496100c170.AddressFamily', line 1, position 488.
                if (ee.Contains("Could not create an instance of type")) {
                    var field = ee.Split(new string[] { "'" }, StringSplitOptions.None);
                    if (field.Length > 1)
                        ee = field[1].Split(new string[] { "." }, StringSplitOptions.None)[0];
                }
                var typeName = type.ToString();
                if (!filter.ContainsKey(typeName))
                    filter.Add(typeName, new List<string>());
                filter[typeName].Add(ee);
                if (DateTime.Now > time)
                    return null;
                goto JUMP;
            }
        }
    }
}