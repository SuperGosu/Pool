using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gosu.Pool{
	[DefaultExecutionOrder(-100)]
	public class PoolManager : MonoBehaviour
	{
		public bool isDontDestroyOnLoad = true;

		private static PoolManager instance;

		public static PoolManager Instance
		{
			get
			{
				return instance;
			}
		}
		[SerializeField] Pool[] pools;
		private Dictionary<string, Pool> poolDic = new Dictionary<string, Pool>();
		private Dictionary<int, Pool> idToPoolDic = new Dictionary<int, Pool>();

		private void Awake()
		{
			if(instance == null)
			{
				Debug.Log("Pool Manager Init");

				instance = this;
		
				foreach (var pool in pools)
				{
					if (pool.GetPrefab())
					{
						Debug.Log("pool" + pool.GetName() + " init ", pool.GetPrefab());
					}
					else
					{
						Debug.LogError("pool" + pool.GetName() + " init fail  prefab not assign!!!");
					}

					pool.Init(this.transform);
					poolDic.Add(pool.GetName(), pool);
					idToPoolDic.Add(pool.GetPrefabID, pool);
				}

				if (isDontDestroyOnLoad)
				{
					DontDestroyOnLoad(this.gameObject);
				}
	
			}
			else
			{
				if(instance != this)
				{
					Destroy(this.gameObject);
				}
			}
		}

		public T GetObject<T>(string poolName) where T: Component
		{
			return poolDic[poolName].Get<T>();
		}
		public T GetObject<T>(T prefab) where T : Component
		{
			int id = prefab.gameObject.GetInstanceID();
			if (idToPoolDic.ContainsKey(id))
			{
				return idToPoolDic[id].Get<T>();
			}
			else
			{
				Debug.LogWarning("pool not setup, instance by Unity Default: " + prefab.name);
				return GameObject.Instantiate(prefab);
			}
		}

		/// <summary>
		/// Release all pools
		/// </summary>
		public void ReleaseAll()
		{
			foreach (var item in poolDic.Values)
			{
				Debug.Log(item.GetName());
				item.Release();
			}
		}


		private void OnDestroy()
		{
			if (!isDontDestroyOnLoad)
			{
				instance = null;
			}
		}


	}
}
