using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Tirocinio
{
    public class ObjectPool<T> : IPool<T> where T : MonoBehaviour, IPoolable<T>
    {
        public ObjectPool(GameObject pooledObject, int numToSpawn = 0)
        {
            this.prefab = pooledObject;
            Spawn(numToSpawn);
        }
        private Stack<T> pooledObjects = new Stack<T>();
        private GameObject prefab;
        public int pooledCount
        {
            get
            {
                return pooledObjects.Count;
            }
        }

        public T Pull()
        {
            T t;
            if (pooledCount > 0)
                t = pooledObjects.Pop();
            else
                t = GameObject.Instantiate(prefab).GetComponent<T>();

            t.gameObject.SetActive(true); //ensure the object is on
            t.Initialize(Push);



            return t;
        }

        public T Pull(Vector3 position)
        {
            T t = Pull();
            t.transform.position = position;
            t.transform.localScale = Vector3.one;

            return t;
        }

        public T Pull(Vector3 position, Quaternion rotation)
        {
            T t = Pull();
            t.transform.position = position;
            t.transform.rotation = rotation;
            t.transform.localScale = Vector3.one;            
            return t;
        }

        public T Pull(Vector3 position, Quaternion rotation, Transform parent)
        {
            T t = Pull();
            t.transform.position = position;
            t.transform.rotation = rotation;
            t.transform.parent = parent;
            t.transform.localScale = Vector3.one;
            return t;
        }


        public GameObject PullGameObject()
        {
            return Pull().gameObject;
        }

        public GameObject PullGameObject(Vector3 position)
        {
            GameObject go = Pull().gameObject;
            go.transform.position = position;
            go.transform.localScale = Vector3.one;
            return go;
        }

        public GameObject PullGameObject(Vector3 position, Quaternion rotation)
        {
            GameObject go = Pull().gameObject;
            go.transform.position = position;
            go.transform.rotation = rotation;
            go.transform.localScale = Vector3.one;
            return go;
        }
        public GameObject PullGameObject(Vector3 position, Quaternion rotation,Transform parent)
        {
            GameObject go = Pull().gameObject;
            go.transform.position = position;
            go.transform.rotation = rotation;
            go.transform.parent = parent;
            go.transform.localScale = Vector3.one;
            return go;
        }

        public void Push(T t)
        {
            pooledObjects.Push(t);


            t.gameObject.SetActive(false);
        }

        private void Spawn(int number)
        {
            T t;

            for (int i = 0; i < number; i++)
            {
                t = GameObject.Instantiate(prefab).GetComponent<T>();
                pooledObjects.Push(t);
                t.gameObject.SetActive(false);
            }
        }
    }

    public interface IPool<T>
    {
        T Pull();
        void Push(T t);
    }

    public interface IPoolable<T>
    {
        void Initialize(System.Action<T> returnAction);
        void ReturnToPool();
    }
}
