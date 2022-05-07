using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CellBig;
using CellBig.Common;
using System.Text;

namespace CellBig
{
    public class StreamingCSVLoader
    {
        Dictionary<string, List<string>> loadedDatas = new Dictionary<string, List<string>>();
        List<string> ValueNames = new List<string>();
        public bool isLoading;

        /// <summary>
        /// csv 파일을 로드 하는 함수 
        /// 코루틴으로 동작함 onComplete 함수를 통해 로드완료시 콜백을 받을수 있다. 
        /// </summary>
        /// <param name="resourceName"></param>
        /// <param name="onComplete"></param>
        /// <returns></returns>
		public void Load(string resourceName, System.Action onComplete = null)
        {
            //Debug.LogFormat("Start to load {0} at frame {1}", resourceName, Time.frameCount);
            if (isLoading == true)
                return;

            string line;
            string pathBasic = Application.dataPath + "/StreamingAssets/Setting/";
            string path = resourceName;
            using (System.IO.StreamReader file = new System.IO.StreamReader(@pathBasic + path))//, Encoding.Unicode))
            {
                isLoading = true;
                ValueNames.Clear();
                foreach (var item in loadedDatas)
                {
                    item.Value.Clear();
                }
                loadedDatas.Clear();
                List<int> headIndex = new List<int>();

                if ((line = file.ReadLine()) != null)
                {
                    var data = line.Split(',');
                    for (int i = 0; i < data.Length; ++i)
                    {
                        if (data[i].Contains(";") || string.IsNullOrEmpty(data[i]))
                            continue;
                        headIndex.Add(i);
                        loadedDatas.Add(data[i], new List<string>());
                        ValueNames.Add(data[i]);
                    }
                }
                while ((line = file.ReadLine()) != null)
                {
                    string[] value = line.Split(',');

                    for (int i = 0; i < headIndex.Count; i++)
                    {
                        loadedDatas[ValueNames[i]].Add(value[headIndex[i]]);

                    }
                }

                file.Close();
                line = string.Empty;
                isLoading = false;
                if (onComplete != null)
                    onComplete();
            }
        }

        public void Unload(string resourceName)
        {
            foreach (var item in loadedDatas)
            {
                item.Value.Clear();
            }
            loadedDatas.Clear();
            ValueNames.Clear();

        }

        /// <summary>
        /// 변수 이름(csv의 열값)과 인덱스(csv의 행값)로 단일 데이터를 가져온다
        /// </summary>
        /// <param name="valueName"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public string GetValue(string valueName, int index)
        {
            if (loadedDatas.ContainsKey(valueName))
                return loadedDatas[valueName][index];
            return null;
        }

        /// <summary>
        /// 변수 이름에 해당하는 모든값을 가져온다.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public List<string> GetValue(string name)
        {
            if (loadedDatas.ContainsKey(name))
                return loadedDatas[name];
            return null;
        }

        /// <summary>
        /// 인덱스를 통해 행값(가로)을 가져온다
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public List<string> GetValue(int index)
        {
            List<string> result = new List<string>();
            foreach (var name in ValueNames)
            {
                result.Add(loadedDatas[name][index]);
            }
            return result;
        }

        /// <summary>
        /// 이름을 가져온다 (csv의 첫번째 줄의 값을 이름으로 함)
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string GetValueName(int index)
        {
            if (index < ValueNames.Count)
                return ValueNames[index];
            return null;
        }

        /// <summary>
        /// 모든 이름을 가져온다
        /// </summary>
        /// <returns></returns>
        public List<string> GetValueNames()
        {
            return ValueNames;
        }

        /// <summary>
        /// 특정 변수 데이터에 값이 있으면 해당하는 인덱스 번호를 가져옴
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public int GetEqualsIndex(string name, string value)
        {
            for (int i = 0; i < GetValue(name).Count; i++)
            {
                if (GetValue(name)[i].Equals(value))
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
