using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gosu.Pool
{

	[System.Serializable]
	public class Pool 
	{
		[SerializeField] new string name;
		[SerializeField] int maxSize;
		[SerializeField] int instanceNumber;
		[SerializeField] GameObject prefab;

		private Queue<GameObject> items = new Queue<GameObject>();

		public string GetName() => name;
		public GameObject GetPrefab() => prefab;
		public int GetPrefabID => prefab.GetInstanceID();
		private Transform holder;


		public void Init(Transform parent)
		{
			holder = new GameObject(name + "_pool").transform;
			holder.SetParent(parent);
			if (instanceNumber > maxSize) instanceNumber = maxSize;

			for (int i = 0; i < instanceNumber; i++)
			{
				var item = GameObject.Instantiate(prefab);
				item.name += "_pool";
				item.transform.SetParent(holder);
				items.Enqueue(item);
				item.gameObject.setActive(false);
			}
		}

		/// <summary>
		/// 	Returns the instance back to the pool.
		/// </summary>
		public void Release()
		{
			foreach (var item in items)
			{
				item.gameObject.SetActive(false);
				item.transform.SetParent(holder);
			}
		}
		/// <summary>
		/// 	Get an instance from the pool. If the pool is empty then a new instance will be created.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T Get<T>() where T : Component
		{
			// exist a deactive object
			foreach (var item in items)
			{
				if (!item.gameObject.activeInHierarchy)
				{
					return item.GetComponent<T>();
				}
			}

			// not exist a deactive object, but not max size
			if (items.Count < maxSize)
			{
				var item = GameObject.Instantiate(prefab);
				items.Enqueue(item);
				item.transform.SetParent(holder);
				return item.GetComponent<T>();
			}

			// other
			var _item = items.Dequeue();
			items.Enqueue(_item);
			return _item.GetComponent<T>();

		}

	}
}
