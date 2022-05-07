//#define LOAD_FROM_ASSETBUNDLE

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using CellBig;
using CellBig.Common;
using System.IO;


namespace CellBig
{
    public class TableManager : MonoSingleton<TableManager>
    {
        bool _alreadyLoading = false;
        bool _loadComplete = false;

        readonly Dictionary<System.Type, object> _tables = new Dictionary<System.Type, object>();

        public bool IsComplete { get { return _loadComplete; } }

        public T GetTableClass<T>() where T : class
        {
            object table;

            if (_tables.TryGetValue(typeof(T), out table))
                return (T)table;

            Debug.LogErrorFormat("{0} is null", typeof(T).Name);
            return null;
        }

        public IEnumerator Load()
        {
            if (_loadComplete)
                yield break;

            if (_alreadyLoading)
            {
                while (!_loadComplete)
                    yield return null;

                yield break;
            }

            _alreadyLoading = true;
            _loadComplete = false;

            while (!Log.Instance.IsInitializeComplete)
                yield return new WaitForSeconds(0.2f);

            yield return ResourceLoader.Instance.Load<BT_Sound>("Tables/BT_Sound", o =>
             {
                 _tables.Add(Type.GetType("BT_Sound"), o);
             });

            //yield return AssetBundleLoader.Instance.Load<BT_Sound>("Table/BT_Sound", "BT_Sound", o =>
            //{
            //    _tables.Add(Type.GetType("BT_Sound"), o);
            //});

            _alreadyLoading = false;
            _loadComplete = true;
        }

        public void Clear()
        {
            //ResourceLoader.Instance.Unload("Tables/Table/BT_Sound");

            _tables.Clear();
            Debug.Log("Clear Tables. - " + GetInstanceID());
        }

        protected override void Release()
        {
            Clear();
        }
    }
}
