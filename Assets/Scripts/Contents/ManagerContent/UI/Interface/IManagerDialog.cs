using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CellBig.Models;
using CellBig.Common;

namespace CellBig.UI.Event
{
    public class ContentListChange : Message
    {
        public int TargetIndex;
        public int RootIndex;
        public ContentListChange(int root, int target)
        {
            RootIndex = root;
            TargetIndex = target;
        }
    }

    public class ContentToggleChange : Message
    {
        public int index;
        public bool isValue;
        public ContentToggleChange(int _index, bool _var)
        {
            index = _index;
            isValue = _var;
        }
    }
}

namespace CellBig.UI
{
    public class IManagerDialog : IDialog
    {
        protected PlayContentModel cm;
        protected SettingModel sm;
        protected Button SelectAll;
        protected Button GameEnd;
        protected ObjectPool itemData = new ObjectPool();

        protected List<ContentItem> itemList = new List<ContentItem>();
        protected Dictionary<int, PlayContentModel.ContentData> itemDataList = new Dictionary<int, PlayContentModel.ContentData>();
        protected Dictionary<string, Sprite> iconList = new Dictionary<string, Sprite>();

        protected ScrollRect ContentListUI;

        protected bool isUseAll = false;

        protected GameObject SelectError;

        protected override void OnLoad()
        {
            if (cm == null)
                cm = Model.First<PlayContentModel>();

            if (sm == null)
                sm = Model.First<SettingModel>();


        }

        protected override void OnUnload()
        {
        }

        protected override void OnEnter()
        {
            StartCoroutine(UISet());
            Message.AddListener<UI.Event.ContentToggleChange>(ToggleChange);
            Message.AddListener<UI.Event.ContentListChange>(ListChange);

        }

        protected override void OnExit()
        {
            Message.RemoveListener<UI.Event.ContentToggleChange>(ToggleChange);
            Message.RemoveListener<UI.Event.ContentListChange>(ListChange);

            foreach (var item in itemList)
            {
                itemData.PoolObject(item.gameObject);
            }
            itemList.Clear();

            iconList.Clear();
            itemDataList = null;
            if (SelectAll != null)
                SelectAll.onClick.RemoveListener(ContentsSelectAll);
            if (GameEnd != null)
                GameEnd.onClick.RemoveListener(EndGame);
        }

        protected virtual IEnumerator UISet()
        {
            foreach (var temp in itemDataList)
            {
                string path = string.Format("UIs/ICON/{0}", temp.Value.ContentName);
                string name = temp.Value.ContentName;
                StartCoroutine(ResourceLoader.Instance.Load<Sprite>(path, o =>
                {
                    if (o != null)
                        iconList.Add(name, (Sprite)o);
                }
                ));
            }

            SelectError = dialogView.transform.Find("SelectError").gameObject;
            SelectError.SetActive(false);
            ContentListUI = dialogView.transform.Find("R_Panel").Find("Scroll View").GetComponent<ScrollRect>();

            SelectAll = dialogView.transform.Find("DownButtons").Find("SelectAll").GetComponent<Button>();
            SelectAll.onClick.AddListener(ContentsSelectAll);

            GameEnd = dialogView.transform.Find("DownButtons").Find("GameEnd").GetComponent<Button>();
            GameEnd.onClick.AddListener(EndGame);

            yield return StartCoroutine(ResourceLoader.Instance.Load<GameObject>("Prefab/1.Manager/ContentItem", o =>
            {
                itemData = Util.Instance.CreateObjectPool(this.gameObject, (GameObject)o, 10);
            }));

            yield return null;
            yield return StartCoroutine(SetActiveContentList());
        }

        protected virtual void EndGame()
        {
            cm.SavePlayContentList();
        }

        void ContentsSelectAll()
        {
            isUseAll = !isUseAll;
            foreach (var item in itemDataList)
            {
                item.Value.isUse = isUseAll;
            }
            foreach (var item in itemList)
            {
                item.SetCheck(isUseAll);
            }
        }

        protected virtual void ToggleChange(UI.Event.ContentToggleChange msg)
        {
        }

        protected virtual void ListChange(Event.ContentListChange msg)
        {
            if (msg.TargetIndex < 0 || msg.TargetIndex > itemDataList.Count)
            {
                Debug.LogError("ListChange Index Out of range");
                return;
            }
            PlayContentModel.ContentData root;
            itemDataList.TryGetValue(msg.RootIndex, out root);
            PlayContentModel.ContentData target;
            itemDataList.TryGetValue(msg.TargetIndex, out target);

            if (root == null || target == null)
                return;

            itemDataList.Remove(msg.RootIndex);
            itemDataList.Remove(msg.TargetIndex);

            itemDataList.Add(msg.RootIndex, target);
            itemDataList.Add(msg.TargetIndex, root);

            SetContnet(itemList[msg.RootIndex], target, msg.RootIndex);
            SetContnet(itemList[msg.TargetIndex], root, msg.TargetIndex);
        }

        protected virtual IEnumerator SetActiveContentList()
        {
            for (int i = 0; i < itemDataList.Count; i++)
            {
                var item = itemData.GetObject();
                item.transform.parent = ContentListUI.content;
                var trans = item.GetComponent<RectTransform>();
                trans.localPosition = Vector3.zero;
                trans.localScale = Vector3.one;
                Debug.LogWarning("max ========== " + trans.offsetMax + "////////// min ==========" + trans.offsetMin);
                trans.offsetMin = new Vector2(0, trans.offsetMin.y);
                trans.offsetMax = new Vector2(0, trans.offsetMax.y);
                var pos = trans.localPosition;
                pos.y = -(i * trans.rect.height + trans.rect.height / 2);
                trans.localPosition = pos;
                var temp = item.GetComponent<ContentItem>();

                SetContnet(temp, itemDataList[i], i);

                itemList.Add(temp);
                yield return null;
            }
        }

        protected virtual void SetContnet(ContentItem root, PlayContentModel.ContentData data, int index)
        {
            Sprite tempImage;
            iconList.TryGetValue(data.ContentName, out tempImage);
            root.SetContent(data.ContentName, tempImage, data.isUse, index);
        }

    }
}