using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using LuaInterface;

namespace LuaFramework {

	[Serializable]
	public class PoolInfo {
		public string poolName;
		//public GameObject prefab;
		public int poolSize;
		public bool fixedSize;
	}

	public class GameObjectPool {
        private int maxSize;  //对象池最大容量
		private int poolSize;  //对象池目前所拥有的对象数量
		private string poolName;  //对象池名称
        private Transform poolRoot;  //该对象池中的对象所挂载的根节点
        private GameObject poolObjectPrefab;  //对象池中的某一实例
        private Stack<GameObject> availableObjStack = new Stack<GameObject>();  //存放该对象池所有对象的栈

        public GameObjectPool(string poolName, int initCount, int maxSize, Transform pool) {
			this.poolName = poolName;
			this.poolSize = initCount;
            this.maxSize = maxSize;
            this.poolRoot = pool;
            //this.poolObjectPrefab = poolObjectPrefab;

		}

		/// <summary>
		/// 添加一个对象进入对象池
		/// </summary>
		/// <param name="go">添加的对象实例</param>
        public void SetObjectToPool(GameObject go) {
		    if(poolSize >= maxSize)
			{
				Debugger.LogError($"-------------{go}对应的对象池已满-----------");
			}
			//add to pool
            go.SetActive(false);
            availableObjStack.Push(go);
            go.transform.SetParent(poolRoot, false);
			poolSize++;
		}


		/// <summary>
		/// 提取一个对象实例
		/// </summary>
		/// <returns>提取的对象实例</returns>
		public string GetObjectToPool(string prefabName,Action<GameObject> action) {

			if (availableObjStack.Count < 1)
			{
				if (poolObjectPrefab != null)
				{
					GameObject go = GameObject.Instantiate(poolObjectPrefab);
					poolSize--;
					go.SetActive(true);
					go.name = prefabName;
					action(go);
					return Guid.NewGuid().ToString();
				}
				else
				{
					string assetName = prefabName;
					string tempString = prefabName;
					if (prefabName.EndsWith("Panel"))
					{
						tempString = tempString.Remove(tempString.Length - "Panel".Length);
					}

					string abName = tempString.ToLower() + AppConst.ExtName;

					LuaHelper.GetResManager().LoadPrefab(abName, assetName, (objs) =>
					{
						if (objs.Length == 0)
						{
							Debugger.LogError($"--------------加载{prefabName}失败----------");
							return;
						}

						poolObjectPrefab = objs[0] as GameObject;
						if (poolObjectPrefab == null)
						{
							Debugger.LogError($"--------------加载{prefabName}失败----------");
							return;
						}
						GameObject go = GameObject.Instantiate(poolObjectPrefab);

						go.name = prefabName;
						action(go);

					});
					return Guid.NewGuid().ToString();
				}
			}
			else
			{
				GameObject go = availableObjStack.Pop();
				poolSize--;
				go.SetActive(true);
				action(go);
				return string.Empty;
			}
		} 
		

		public void Clear()
		{
			availableObjStack.Clear();
			poolSize = 0;
		}
		/// <summary>
		/// 往对象池添加一个对象实例
		/// </summary>
		/// <param name="pool"></param>
		/// <param name="po"></param>
  //      public void ReturnObjectToPool(string pool, GameObject po) {
  //          if (poolName.Equals(pool)) {
  //              SetObjectToPool(po);
		//	} else {
		//		Debug.LogError(string.Format("Trying to add object to incorrect pool {0} ", poolName));
		//	}
		//}
	}
}
