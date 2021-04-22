using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Net.Entity
{
    [Serializable]
    public class NGameObject : NObject
    {
        public List<NComponent> components = new List<NComponent>();
        public NTransform transform = new NTransform();

        [SerializeField]
        [ProtoBuf.ProtoIgnore]
        [Newtonsoft.Json.JsonIgnore]
        private bool _enabled = true;
        [ProtoBuf.ProtoIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public bool enabled {
            get { return _enabled; }
            set {
                if (value & !_enabled) {
                    if (!scene.enableObjs.Contains(this)) {
                        scene.enableObjs.Add(this);
                    }
                }
                if (!value & _enabled) {
                    if (!scene.disableObjs.Contains(this)) {
                        scene.disableObjs.Add(this);
                    }
                }
                _enabled = value;
            }
        }

        public T GetComponent<T>() where T : NComponent
        {
            foreach (var t in components) {
                if (t is T)
                    return (T)t;
            }
            return null;
        }

        public T[] GetComponentsInParent<T>() where T : NComponent
        {
            List<T> ts = new List<T>();
            foreach (var t in components) {
                if (t is T)
                    ts.Add((T)t);
            }
            return ts.ToArray();
        }

        public T AddComponent<T>() where T : NComponent, new()
        {
            return AddComponent(new T());
        }

        public T AddComponent<T>(T t) where T : NComponent, new()
        {
            t.gameObject = this;
            t.transform = transform;
            t.scene = scene;
            components.Add(t);
            return t;
        }

        public void DestroyComponent(NComponent component)
        {
            for (int i = 0; i < components.Count; i++) {
                if (components[i] == component) {
                    components[i].OnDestroy();
                    components.RemoveAt(i);
                    return;
                }
            }
        }

        internal void Awake()
        {
            for (int i = 0; i < components.Count; i++) {
                if (components[i].enabled)
                    components[i].Awake();
            }
        }

        internal void OnEnable()
        {
            for (int i = 0; i < components.Count; i++) {
                if (components[i].enabled)
                    components[i].OnEnable();
            }
        }

        internal void Start()
        {
            for (int i = 0; i < components.Count; i++) {
                if (components[i].enabled)
                    components[i].Start();
            }
        }

        internal void FixedUpdate()
        {
            for (int i = 0; i < components.Count; i++) {
                if (components[i].enabled)
                    components[i].FixedUpdate();
            }
        }

        internal void Update()
        {
            for (int i = 0; i < components.Count; i++) {
                if (components[i].enabled)
                    components[i].Update();
            }
        }

        internal void LateUpdate()
        {
            for (int i = 0; i < components.Count; i++) {
                if(components[i].enabled)
                    components[i].LateUpdate();
            }
        }

        internal void OnDisable()
        {
            for (int i = 0; i < components.Count; i++) {
                components[i].OnDisable();
            }
        }

        internal void OnDestroy()
        {
            for (int i = 0; i < components.Count; i++) {
                components[i].OnDestroy();
            }
        }

        public void Destroy()
        {
            scene.Destroy(this);
        }
    }
}