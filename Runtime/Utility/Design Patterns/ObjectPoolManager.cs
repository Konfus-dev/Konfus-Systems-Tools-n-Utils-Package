﻿using System.Collections.Generic;
using UnityEngine;

namespace Konfus.Utility.Design_Patterns
{
    [System.Serializable]
    public class Pool
    {
        /// <summary>
        /// key to associate with objects in a pool.
        /// </summary>
        public string key;
        /// <summary>
        /// size of pool.
        /// </summary>
        public int size;
        /// <summary>
        /// prefab to instantiate objects from for a pool.
        /// </summary>
        public GameObject prefab;
        /// <summary>
        /// place to parent and store objects in a pool.
        /// </summary>
        public Transform storage;
    }

    public class ObjectPoolManager : Singleton<ObjectPoolManager>
    {
        [Tooltip("The pools of GameObjects to spawn from.")]
        public List<Pool> pools;

        /// <summary>
        /// stores queues for pools by cref="MartianChild.Utility.Pool.tag".
        /// </summary>
        private Dictionary<string, Queue<GameObject>> _poolDict;

        protected override void OnAwake()
        {
            CreatePools();
        }

        /// <summary>
        /// <para> Spawns object from pool at a specified gridPosition and rotation. </para>
        /// <param name="key"> <see cref="Pool.key"/> given to item in <see cref="Pool"/>. </param>
        /// <param name="position"> Position to spawn object. </param>
        /// <param name="rotation"> Rotation to spawn object. </param>
        /// </summary>
        public GameObject SpawnFromPool(string key, Vector3 position, Quaternion rotation)
        {
            if (!_poolDict.ContainsKey(key))
            {
                Debug.LogError("No pool with key: " + key + " found!", this);
                return null;
            }
            
            GameObject gameObj = _poolDict[key].Dequeue();
            gameObj.transform.position = position;
            gameObj.transform.rotation = rotation;
            gameObj.SetActive(true);
            _poolDict[key].Enqueue(gameObj);
            
            return gameObj;
        }

        private void CreatePools()
        {
            _poolDict = new Dictionary<string, Queue<GameObject>>();
            
            foreach (Pool pool in pools)
            {
                Queue<GameObject> objPool = new Queue<GameObject>();

                for (int i = 0; i < pool.size; i++)
                {
                    GameObject obj = Instantiate(pool.prefab, pool.storage, true);
                    obj.SetActive(false);
                    objPool.Enqueue(obj);
                }

                _poolDict.Add(pool.key, objPool);
            }
        }
    }
}