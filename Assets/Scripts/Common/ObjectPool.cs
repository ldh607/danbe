// -------------------------------------------------------------------------------------------------
// Filename: ObjectPool.cs
// Author: Song Ji Hun. [aka. CraZy GolMae] <jihun.song@pocatcom.com>
// Date: 2013.8.22
//
// Desc:
//
// Copyright (c) 2013 Pocatcom. All rights reserved.
// -------------------------------------------------------------------------------------------------
using UnityEngine;
using System;
using System.Collections.Generic;


namespace CellBig.Common
{
    public class ObjectPool : MonoBehaviour
    {
        public GameObject prefab;

        Queue<GameObject> _pool = new Queue<GameObject>();
        public event Action<GameObject> onCreateObject = null;

        public void PreloadObject(int count, GameObject prefab)
        {
            this.prefab = prefab;

            for (int i = 0; i < count; ++i)
            {
                _pool.Enqueue(InstantiateObject());
            }
        }


        //풀에서 오브젝트 가져오기
        public GameObject GetObject(Transform parent = null)
        {
            GameObject go = null;

            if (_pool.Count == 0)
            {
                go = InstantiateObject();
                _pool.Enqueue(go);

            }
            else
            {
                go = _pool.Dequeue();
            }

            go.SetActive(true);
            go.transform.SetParent(parent);

            return go;
        }

        //풀에 오브젝트 저장하기
        public void PoolObject(GameObject go)
        {
            if(this != null) {
            go.transform.SetParent(transform);
            go.SetActive(false);
            _pool.Enqueue(go);
            }
            else
            {
                Destroy(go);
            }
        }

        //풀에 오브젝트 갯수
        public int PoolCount()
        {
            return _pool.Count;
        }

        //풀 초기화
        public void ClearAll()
        {
            for (int i = 0; i < transform.childCount; ++i)
                Destroy(transform.GetChild(i).gameObject);

            _pool.Clear();
        }

        GameObject InstantiateObject()
        {
            GameObject go;
            try
            {
                go = Instantiate<GameObject>(prefab);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
            }
            go.SetActive(false);
            go.transform.SetParent(transform);

            if (onCreateObject != null)
                onCreateObject(go);
 
            return go;
        }
    }


    // 풀에 가져올 데이터가 없으면 null 을 반환하는 형태
    public class ObjectPool<T> where T : UnityEngine.Object
    {
        readonly Queue<T> _pool = new Queue<T>();


        public T GetObject()
        {
            T o;

            if (_pool.Count == 0)
            {
                return null;
            }
            else
            {
                o = _pool.Dequeue();
            }

            return o;
        }

        public void PoolObject(T o)
        {
            _pool.Enqueue(o);
        }

        public void ClearAll()
        {
            while (_pool.Count > 0)
            {
                var o = _pool.Dequeue();
                UnityEngine.Object.Destroy(o);
            }
        }
    }


    public class ObjectPool<TKey, TType> where TType : UnityEngine.Object
    {
        readonly Dictionary<TKey, ObjectPool<TType>> _pool = new Dictionary<TKey, ObjectPool<TType>>();


        public TType GetObject(TKey key)
        {
            ObjectPool<TType> op;
            if (_pool.TryGetValue(key, out op))
            {
                return op.GetObject();
            }

            return null;
        }

        public void PoolObject(TKey key, TType o)
        {
            ObjectPool<TType> op;
            if (!_pool.TryGetValue(key, out op))
            {
                op = new ObjectPool<TType>();
                _pool.Add(key, op);
            }

            op.PoolObject(o);
        }

        public void Clear(TKey key)
        {
            ObjectPool<TType> op;
            if (_pool.TryGetValue(key, out op))
            {
                op.ClearAll();
                _pool.Remove(key);
            }
        }

        public void ClearAll()
        {
            foreach (var op in _pool)
            {
                op.Value.ClearAll();
            }

            _pool.Clear();
        }
    }
}
