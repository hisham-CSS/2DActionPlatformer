using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : Singleton<ObjectPooler>
{
    [System.Serializable]
    public class Pool
    {
        public GameObject prefab;
        public int size;
    }

    public List<Pool> pools;
    public Dictionary<int, Queue<GameObject>> poolDictionary;

    protected override void Awake()
    {
        base.Awake();
        poolDictionary = new Dictionary<int, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.prefab.GetInstanceID(), objectPool);
        }
    }

    public GameObject SpawnFromPool(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        int poolKey = prefab.GetInstanceID();

        if (poolDictionary.ContainsKey(poolKey))
        {
            GameObject objectToSpawn = poolDictionary[poolKey].Dequeue();

            objectToSpawn.SetActive(true);
            objectToSpawn.transform.position = position;
            objectToSpawn.transform.rotation = rotation;

            poolDictionary[poolKey].Enqueue(objectToSpawn);

            return objectToSpawn;
        }

        Debug.LogWarning("Pool with key " + poolKey + " doesn't exist.");
        return null;
    }
}
