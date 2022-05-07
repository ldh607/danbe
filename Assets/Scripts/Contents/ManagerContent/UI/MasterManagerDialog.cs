using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CellBig.Models;
using CellBig.Common;
using CellBig.UI.Event;

namespace CellBig.UI
{
    public class MasterManagerDialog : IManagerDialog
    {
        InputField password;

        protected override void OnLoad()
        {
            if (cm == null)
                cm = Model.First<PlayContentModel>();
            base.OnLoad();
        }

        protected override void OnEnter()
        {
            itemDataList = cm.RootData;
            base.OnEnter();
        }

        protected override void ToggleChange(ContentToggleChange msg)
        {
            if (itemDataList.ContainsKey(msg.index))
            {
                itemDataList[msg.index].isActive = msg.isValue;
            }
        }

        protected override void SetContnet(ContentItem root, PlayContentModel.ContentData data, int index)
        {
            Sprite tempImage;
            iconList.TryGetValue(data.ContentName, out tempImage);
            root.SetContent(data.ContentName, tempImage, data.isActive , index);
        }

        protected override void OnExit()
        {
            base.OnExit();
        }

        protected override void OnUnload()
        {
            base.OnUnload();
        }

        protected override IEnumerator UISet()
        {
            yield return StartCoroutine( base.UISet() );

            var LPanel = this.transform.Find("GameObject").Find("L_Panel");
            password = LPanel.Find("BOX01").Find("Password").GetComponent<InputField>();
            password.onValueChanged.AddListener(delegate {
                cm.Password = password.text;
            });
            password.text = cm.Password;
            yield return null;
        }

        protected override void EndGame()
        {
            if (!cm.ResetData(true))
            {
                SelectError.SetActive(true);
                return;
            }

            cm.SaveContentsSetting();
            base.EndGame();
            IDialog.RequestDialogExit<MasterManagerDialog>();
            IDialog.RequestDialogEnter<CommonManagerDialog>();            
        }
    }

}