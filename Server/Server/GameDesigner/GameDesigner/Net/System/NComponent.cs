using System;
using UnityEngine;

namespace Net.Entity
{
    public class NComponent : NObject
    {
        [ProtoBuf.ProtoIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public NGameObject gameObject;
        public NTransform transform;

        [SerializeField]
        private bool _enabled = true;
        public bool enabled {
            get { return _enabled; }
            set { _enabled = value; }
        }

        public virtual void Awake() { }

        public virtual void OnEnable() { }

        public virtual void Start(){ }

        public virtual void FixedUpdate() { }

        public virtual void Update() { }

        public virtual void LateUpdate() { }

        public virtual void OnDisable() { }

        public virtual void OnDestroy() { }

        public T GetComponent<T>() where T : NComponent
        {
            return gameObject.GetComponent<T>();
        }

        public T[] GetComponentsInParent<T>() where T : NComponent
        {
            return gameObject.GetComponentsInParent<T>();
        }
        public T[] GetComponentsInChildren<T>() where T : NComponent
        {
            return gameObject.GetComponentsInParent<T>();
        }

        public void Destroy()
        {
            gameObject.DestroyComponent(this);
        }

        public void Destroy(NComponent component)
        {
            gameObject.DestroyComponent(component);
        }
    }
}
