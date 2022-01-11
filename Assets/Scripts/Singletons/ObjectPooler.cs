using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Tirocinio
{

    public class ObjectPooler : MonoBehaviour
    {
        //This class is used for pooling all the objects in the hex generation (exits, hexes, and chunks)

        public GameObject exitPrefab;
        public GameObject hexPrefab;
        public GameObject chunkPrefab;

        List<GameObject> pooledExits;
        List<GameObject> pooledHexes;
        List<GameObject> pooledChunks;

        
        void Awake()
        {

            pooledExits = InitializePool(pooledExits, exitPrefab, 30);
            pooledHexes = InitializePool(pooledHexes, hexPrefab, 20);
            pooledChunks = InitializePool(pooledChunks, chunkPrefab, 6);

        }

        //used to recycle the hex that starts the first generation
        public void AddCentralHex(GameObject centralHex) => pooledHexes.Add(centralHex);
        
        //same as AddCentralHex, for chunks
        public void AddCentralChunk(GameObject centralChunk) => pooledChunks.Add(centralChunk);
        

        public GameObject GetPooledChunk(Vector3 position, Quaternion rotation, Transform parent)
        {
            GameObject pooledChunk = SearchObject(pooledChunks, chunkPrefab);
            pooledChunk.SetActive(true);
            pooledChunk.transform.position = position;
            pooledChunk.transform.rotation = rotation;
            pooledChunk.transform.parent = parent;
            pooledChunk.transform.localScale = Vector3.one;
            return pooledChunk;
        }

        public GameObject GetPooledExit(Vector3 position, Quaternion rotation, Transform parent)
        {
            GameObject pooledExit = SearchObject(pooledExits, exitPrefab);
            pooledExit.SetActive(true);
            pooledExit.transform.position = position;
            pooledExit.transform.rotation = rotation;
            pooledExit.transform.parent = parent;
            pooledExit.transform.localScale = Vector3.one;
            return pooledExit;
        }


        public GameObject GetPooledHex(Vector3 position, Quaternion rotation, Transform parent)
        {
            GameObject pooledHex = SearchObject(pooledHexes, hexPrefab);
            pooledHex.SetActive(true);
            pooledHex.transform.position = position;
            pooledHex.transform.rotation = rotation;
            pooledHex.transform.parent = parent;
            pooledHex.transform.localScale = Vector3.one;
            return pooledHex;
        }

        List<GameObject> InitializePool(List<GameObject> pool, GameObject prefab, int count)
        {
            pool = new List<GameObject>(count);
            for (int i = 0; i < count; i++)
            {
                GameObject obj = Instantiate(prefab);
                obj.transform.parent = transform;
                obj.SetActive(false);
                pool.Add(obj);

            }

            return pool;
        }


        GameObject SearchObject(List<GameObject> pool, GameObject prefab)
        {
            //searches for an inactive object, if it doesn't find it, it instantiates it
            GameObject foundObj = pool.Find(obj => !obj.activeInHierarchy);
            if (foundObj) return foundObj;

            GameObject newObject = Instantiate(prefab);
            pool.Add(newObject);
            return newObject;
        }


    }
}


