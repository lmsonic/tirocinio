using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{



    static public ObjectPooler Instance = null;
    public GameObject exitPrefab;
    public GameObject hexPrefab;

    List<GameObject> pooledExits;
    List<GameObject> pooledHexes;

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            pooledExits  = InitializePool(pooledExits, exitPrefab, 12);
            pooledHexes  = InitializePool(pooledHexes, hexPrefab, 6);
        }
        else
        {
            Destroy(gameObject);
        }

        
    }

    public void AddCentralHex (GameObject centralHex){
        pooledHexes.Add(centralHex);
    }
    

    public GameObject GetPooledExit(Vector3 position, Quaternion rotation, Transform parent)
    {
        GameObject pooledExit = SearchObject(pooledExits, exitPrefab);
        pooledExit.SetActive(true);
        pooledExit.transform.position=position;
        pooledExit.transform.rotation=rotation;
        pooledExit.transform.parent=parent;
        return pooledExit;
    }


    public GameObject GetPooledHex(Vector3 position, Quaternion rotation, Transform parent)
    {
        GameObject pooledHex = SearchObject(pooledHexes, hexPrefab);
        pooledHex.SetActive(true);
        pooledHex.transform.position=position;
        pooledHex.transform.rotation=rotation;
        pooledHex.transform.parent=parent;
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
        GameObject foundObj = pool.Find(obj => !obj.activeInHierarchy);
        if (foundObj) return foundObj;

        GameObject newObject = Instantiate(prefab);
        pool.Add(newObject);
        return newObject;
    }

   
}


