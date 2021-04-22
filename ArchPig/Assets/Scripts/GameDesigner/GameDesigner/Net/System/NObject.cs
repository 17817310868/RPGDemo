using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Entity
{
    /// <summary>
    /// 游戏对象基类
    /// </summary>
    public abstract class NObject : IDisposable
    {
        public string name;
        [ProtoBuf.ProtoIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public NScene scene;

        public T FindObjectOfType<T>() where T : NComponent
        {
            var ts = scene.FindObjectsOfType<T>();
            if (ts.Length == 0)
                return null;
            return ts[0];
        }

        public T[] FindObjectsOfType<T>() where T : NComponent
        {
            return scene.FindObjectsOfType<T>();
        }

        public static void Destroy(NGameObject gameObject)
        {
            gameObject.Destroy();
        }

        public void Dispose()
        {
            
        }
    }
}
