using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CellBig.Tool
{
    public class ActiveOneChildObject : MonoBehaviour
    {
        public bool AutoSet = false;
        public List<GameObject> childList = new List<GameObject>();
        public bool EnableRandomActive;


        void Start()
        {
            if (AutoSet)
            {
                childList.Clear();
                foreach (var item in GetComponentsInChildren<GameObject>(true))
                {
                    if (item != this.gameObject)
                    {
                        childList.Add(item);
                    }
                }
            }
        }

        private void OnEnable()
        {
            if( EnableRandomActive)
            {
                ActiveObj(Random.Range(0, childList.Count));
            }
        }

        private void OnDestroy()
        {
            childList.Clear();
        }

        public void ActiveObj(int index, bool Smooth = false, float timer = 0)
        {
            if( index < 0 || index > childList.Count)
            {
                Debug.LogError(string.Format( "ActiveOneChildObject Index Error \n input : {0} Size : {1}",index, childList.Count ));
                return;
            }
            if (Smooth)
            {
                StartCoroutine(SmoothChange(index, timer));
            }
            else
            {
                for (int i = 0; i < childList.Count; i++)
                {
                    if (i == index)
                        childList[i].SetActive(true);
                    else
                        childList[i].SetActive(false);
                }
            }
        }

        IEnumerator SmoothChange(int index, float timer)
        {
            childList[index].SetActive(true);
            yield return new WaitForSeconds(timer);
            for (int i = 0; i < childList.Count; i++)
            {
                if (i != index)
                    childList[i].SetActive(false);
            }
        }
    }
}