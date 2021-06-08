using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;

public class ObjectPoller : NetworkBehaviour
{
    public GameObject PoolingObjParent;

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }
    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    #region Singleton
    public static ObjectPoller ObjPollInstance;
    private void Awake()
    {
        ObjPollInstance = this;
    }
    #endregion

    private void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        foreach(Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for(int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab,Vector3.zero,Quaternion.identity);
                obj.transform.parent = PoolingObjParent.transform;
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if(!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Not have tag in this pool");
            return null;
        }
        else
        {
            GameObject objToSpawn = poolDictionary[tag].Dequeue();
            objToSpawn.transform.position = position;
            objToSpawn.transform.rotation = rotation;
            objToSpawn.SetActive(true);

            poolDictionary[tag].Enqueue(objToSpawn);

            return objToSpawn;
        }
    }
    public GameObject GetFromPool(string tag)
    {
        if(!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Not have tag in this pool");
            return null;
        }
        else
        {
            GameObject objToSpawn = poolDictionary[tag].Dequeue();

            poolDictionary[tag].Enqueue(objToSpawn);

            return objToSpawn;
        }
    }
}
