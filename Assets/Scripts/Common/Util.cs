using UnityEngine;
using CellBig.Common;
using System;

public static class CB // Custom Log
{
    public static void Log(string msg)
    {
#if dev || UNITY_EDITOR
        Debug.Log("<color=red> CB Log : </color>" + msg);
#endif
    }

    public static void Log(object obj)
    {
#if dev || UNITY_EDITOR
        try
        {
            string str = System.Convert.ToString(obj);
            Log(str);
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
        }
#endif
    }

    public static void LogError(string msg)
    {
#if dev || UNITY_EDITOR
        Debug.LogError("<color=red> CB Log : </color>" + msg);
#endif
    }

    public static void LogError(object obj)
    {
#if dev || UNITY_EDITOR
        try
        {
            string str = System.Convert.ToString(obj);
            LogError(str);
        }
        catch (System.Exception e)
        {
            LogError("Wrong Log... \n" + e);
        }
#endif
    }

}
public class Util : MonoSingleton<Util>
{
    public ObjectPool CreateObjectPool(GameObject root, GameObject target, int count)
    {
        var PoolObject = new GameObject();
        PoolObject.name = target.name + "Pool";
        PoolObject.transform.SetParent(root.transform);
        PoolObject.transform.localScale = Vector3.one;
        PoolObject.transform.localPosition = Vector3.zero;
        ObjectPool p = PoolObject.AddComponent<ObjectPool>();
        p.PreloadObject(count, target as GameObject);
        return p;
    }

    public static T[] FromJsonArray<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.array;
    }

    public static string ToJsonArray<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.array = array;
        return JsonUtility.ToJson(wrapper);
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] array;
    }
}
