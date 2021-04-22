namespace Net
{
    using Net.Share;
    using System.Reflection;

    /// <summary>
    /// 克隆工具类
    /// </summary>
    public sealed class Clone
    {
        /// <summary>
        /// 克隆对象, 脱离引用对象的地址
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <returns></returns>
        public static T Instance<T>(object target) where T : class
        {
            return (T)NetConvert.Deserialize(NetConvert.Serialize(target)).pars[0];
        }

        /// <summary>
        /// 克隆对象, 脱离引用对象的地址
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <returns></returns>
        public static T Copy<T>(object target) where T : class
        {
            var obj = NetConvert.Deserialize(NetConvert.Serialize(target)).pars[0];
            var obj1 = System.Activator.CreateInstance<T>();
            var type1 = target.GetType();
            var fields = type1.GetFields(BindingFlags.Instance | BindingFlags.Public);
            foreach(var field in fields){
                var value = field.GetValue(obj);
                if (value != null)
                    field.SetValue(obj1, value);
            }
            return obj1;
        }
    }
}
