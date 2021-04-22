using Net.Share;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Net.Entity
{
    /// <summary>
    /// 游戏场景
    /// </summary>
    public class NScene
    {
        /// <summary>
        /// 唤醒的物体
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoIgnore]
        public List<NGameObject> awakeObjs = new List<NGameObject>();
        /// <summary>
        /// 每帧更新的物体
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoIgnore]
        public List<NGameObject> updateObjs = new List<NGameObject>();
        /// <summary>
        /// 启用的物体
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoIgnore]
        public List<NGameObject> enableObjs = new List<NGameObject>();
        /// <summary>
        /// 关闭的物体
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [ProtoBuf.ProtoIgnore]
        public List<NGameObject> disableObjs = new List<NGameObject>();

        public T[] FindObjectsOfType<T>() where T : NComponent
        {
            List<T> ts = new List<T>();
            List<NGameObject> gameObjects = new List<NGameObject>(awakeObjs);
            gameObjects.AddRange(updateObjs);
            gameObjects.AddRange(enableObjs);
            gameObjects.AddRange(disableObjs);
            HashSet<NGameObject> components = new HashSet<NGameObject>(gameObjects);
            foreach (var o in components) {
                foreach (var c in o.components) {
                    if (c is T)
                        ts.Add((T)c);
                }
            }
            return ts.ToArray();
        }

        public NGameObject CreateGameObject()
        {
            NGameObject gameObject = new NGameObject();
            gameObject.scene = this;
            awakeObjs.Add(gameObject);
            return gameObject;
        }

        public NGameObject AddGameObject(NGameObject gameObject)
        {
            gameObject.scene = this;
            awakeObjs.Add(gameObject);
            return gameObject;
        }

        internal void Awake()
        {
            for (int i = 0; i < updateObjs.Count; i++) {
                updateObjs[i].Awake();
            }
        }

        internal void OnEnable()
        {
            for (int i = 0; i < updateObjs.Count; i++) {
                updateObjs[i].OnEnable();
            }
        }

        internal void Start()
        {
            for (int i = 0; i < updateObjs.Count; i++) {
                updateObjs[i].Start();
            }
        }

        internal void Update()
        {
            for (int i = 0; i < updateObjs.Count; i++) {
                updateObjs[i].Update();
            }
        }

        internal void FixedUpdate()
        {
            for (int i = 0; i < updateObjs.Count; i++) {
                updateObjs[i].FixedUpdate();
            }
        }

        internal void OnDisable()
        {
            for (int i = 0; i < updateObjs.Count; i++) {
                updateObjs[i].OnDisable();
            }
        }

        internal void OnDestroy()
        {
            for (int i = 0; i < updateObjs.Count; i++) {
                updateObjs[i].OnDestroy();
            }
        }

        internal void Destroy(NGameObject gameObject)
        {
            for (var i = 0; i < updateObjs.Count; i++) {
                if (updateObjs[i] == gameObject) {
                    updateObjs[i].OnDestroy();
                    updateObjs.RemoveAt(i);
                    return;
                }
            }
        }

        public void Execute()
        {
            int count = awakeObjs.Count;
            while (count > 0) {
                if(awakeObjs[0].enabled)
                    awakeObjs[0].Awake();
                count--;
            }
            count = awakeObjs.Count;
            while (count > 0) {
                if (awakeObjs[0].enabled)
                    awakeObjs[0].OnEnable();
                count--;
            }
            count = enableObjs.Count;
            while (count > 0) {
                if (enableObjs[0].enabled){
                    enableObjs[0].OnEnable();
                    updateObjs.Add(enableObjs[0]);
                    enableObjs.RemoveAt(0);
                }
                count--;
            }
            count = awakeObjs.Count;
            while (count > 0) {
                if (awakeObjs[0].enabled){
                    awakeObjs[0].Start();
                    updateObjs.Add(awakeObjs[0]);
                    awakeObjs.RemoveAt(0);
                }
                count--;
            }
            Parallel.ForEach(updateObjs, gameObj=>{
                if (gameObj.enabled)
                    gameObj.FixedUpdate();
            });
            Parallel.ForEach(updateObjs, gameObj => {
                if (gameObj.enabled)
                    gameObj.Update();
            });
            Parallel.ForEach(updateObjs, gameObj => {
                if (gameObj.enabled)
                    gameObj.LateUpdate();
            });
            count = disableObjs.Count;
            while (count > 0) {
                disableObjs[0].OnDisable();
                disableObjs.RemoveAt(0);
                count--;
            }
        }
    }
}