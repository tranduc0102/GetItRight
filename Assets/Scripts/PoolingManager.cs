using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace pooling
{
    public static class PoolingManager
    {
        private static Dictionary<int, Pool> listPools;

        private static void Init(GameObject prefab = null)
        {
            if (listPools == null)
            {
                listPools = new Dictionary<int, Pool>();
            }

            if (prefab != null && !listPools.ContainsKey(prefab.GetInstanceID()))
            {
                listPools[prefab.GetInstanceID()] = new Pool(prefab);
            }
        }
        public static GameObject Spawn(GameObject prefab)
        {
            Init(prefab);
            return listPools[prefab.GetInstanceID()].Spawn(Vector3.zero, Quaternion.identity);
        }
        public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion quaternion, Transform parent = null)
        {
            Init(prefab);
            return listPools[prefab.GetInstanceID()].Spawn(position, quaternion, parent);
        }
        public static T Spawn<T>(T prefab) where T : Component
        {
            Init(prefab.gameObject);
            return listPools[prefab.GetInstanceID()].Spawn<T>(Vector3.zero, Quaternion.identity);
        }

        public static T Spawn<T>(T prefab, Vector3 position, Quaternion quaternion, Transform parent = null) where T : Component
        {
            Init(prefab.gameObject);
            return listPools[prefab.gameObject.GetInstanceID()].Spawn<T>(position, quaternion, parent);
        }
        public static void Despawn(GameObject prefab, Action action = null)
        {
            Pool p = null;
            foreach (var pool in listPools.Values)
            {
                if (pool.idObject.Contains(prefab.GetInstanceID()))
                {
                    p = pool;
                    break;
                }
            }

            if (p == null)
            {
                Object.Destroy(prefab);
            }
            else
            {
                p.Despawn(prefab);
            }
        }

        public static void ClearPool()
        {
            if (listPools.Count > 0)
            {
                listPools.Clear();
            }
        }
}
    public class Pool
    {
        private readonly Queue<GameObject> pools;
        public readonly HashSet<int> idObject;
        private readonly GameObject prefabObject;
        private int id = 0;

        public Pool(GameObject gameObject)
        {
            prefabObject = gameObject;
            pools = new Queue<GameObject>();
            idObject = new HashSet<int>();
        }
        public GameObject Spawn(Vector3 position, Quaternion quaternion, Transform parent = null)
        {
            while (true)
            {
                GameObject newObject;
                if (pools.Count == 0)
                {
                    newObject = Object.Instantiate(prefabObject,position ,quaternion,parent);
                    id += 1;
                    idObject.Add(newObject.GetInstanceID());
                    newObject.name = prefabObject.name + "_" + id;
                    return newObject;
                }
                newObject = pools.Dequeue();
                if (newObject == null)
                {
                    continue;
                }
                newObject.transform.SetPositionAndRotation(position, quaternion);
                newObject.transform.SetParent(parent);
                /*
                newObject.name = prefabObject.name;
                */
                newObject.SetActive(true);
                return newObject;
            }
        }
        public T Spawn<T>(Vector3 position, Quaternion quaternion, Transform parent = null)
        {
            return Spawn(position, quaternion, parent).GetComponent<T>();
        }
        public void Despawn(GameObject gameObject)
        {
            if (!gameObject.activeSelf)
            {
                return;
            }
            gameObject.SetActive(false);
            pools.Enqueue(gameObject);
        }
    }
}