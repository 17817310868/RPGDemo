using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using System;
using LuaInterface;

namespace LuaFramework {
    /// <summary>
    /// 对象池管理器，分普通类对象池+资源游戏对象池
    /// </summary>
    public class ObjectPoolManager : Manager {
        private Transform m_PoolRootObject = null;
        private Dictionary<string, object> m_ObjectPools = new Dictionary<string, object>();

        //管理所有对象池，key为对象池名字，value为对象池
        private Dictionary<string, GameObjectPool> m_GameObjectPools = new Dictionary<string, GameObjectPool>();

        //private uint goCount;
        //public uint GoCount
        //{
        //    get {
        //        goCount++;
        //        return goCount;
        //    }
        //}

        Transform PoolRootObject {
            get {
                //创建一个所有对象池所挂在的根节点
                if (m_PoolRootObject == null) {
                    var objectPool = new GameObject("ObjectPool");
                    objectPool.transform.SetParent(transform);
                    objectPool.transform.localScale = Vector3.one;
                    objectPool.transform.localPosition = Vector3.zero;
                    m_PoolRootObject = objectPool.transform;
                }
                return m_PoolRootObject;
            }
        }

        //创建一个对象池
        private GameObjectPool CreatePool(string prefabName, int initSize, int maxSize) {
            var pool = new GameObjectPool(prefabName, initSize, maxSize, PoolRootObject);
            m_GameObjectPools[prefabName] = pool;
            return pool;
        }


        /// <summary>
        /// 从对象池获取一个对象实例
        /// </summary>
        /// <param name="poolName">对象池名称</param>
        /// <param name="prefabName">对象池实例的名称</param>
        /// <returns></returns>
        public string Get(string prefabName , Action<GameObject> action) {

            //若对象池存在，则直接提取对象实例，若不存在，则创建对象池
            if(!m_GameObjectPools.TryGetValue(prefabName,out GameObjectPool pool))
            {
                pool = CreatePool(prefabName, 0, 500);
            }
            return pool.GetObjectToPool(prefabName, action);
        }

        /// <summary>
        /// 往指定对象池添加对象实例
        /// </summary>
        /// <param name="poolName">对象池名称</param>
        /// <param name="go">对象实例</param>
        public void Set(string poolName, GameObject go) {

            if(!m_GameObjectPools.TryGetValue(poolName,out GameObjectPool pool))
            {
                Debugger.LogError($"-------------找不到{go}对应的对象池------------");
            }

            pool.SetObjectToPool(go);
        }

        ///-----------------------------------------------------------------------------------------------

        public ObjectPool<T> CreatePool<T>(UnityAction<T> actionOnGet, UnityAction<T> actionOnRelease) where T : class
        {
            var type = typeof(T);
            var pool = new ObjectPool<T>(actionOnGet, actionOnRelease);
            m_ObjectPools[type.Name] = pool;
            return pool;
        }

        public ObjectPool<T> GetPool<T>() where T : class
        {
            var type = typeof(T);
            ObjectPool<T> pool = null;
            if (m_ObjectPools.ContainsKey(type.Name))
            {
                pool = m_ObjectPools[type.Name] as ObjectPool<T>;
            }
            return pool;
        }

        public T Get<T>() where T : class
        {
            var pool = GetPool<T>();
            if (pool != null)
            {
                return pool.Get();
            }
            return default(T);
        }

        public void Release<T>(T obj) where T : class
        {
            var pool = GetPool<T>();
            if (pool != null)
            {
                pool.Release(obj);
            }
        }

        //--------------------------------------------------------------------------------------


        /// <summary>
        /// 获取指定子物体
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public Transform GetChild(Transform transform,string name)
        {
            Transform[] childs = transform.GetComponentsInChildren<Transform>();
            for(int i = 0; i < childs.Length; i++)
            {
                if (childs[i].name == name)
                    return childs[i];
            }

            Debugger.Log($"----------------------找不到{name}物体----------------------");
            return null;
        }
    }
}