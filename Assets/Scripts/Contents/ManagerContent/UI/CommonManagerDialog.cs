using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CellBig.Models;
using CellBig.Common;
using CellBig.UI.Event;

namespace CellBig.UI
{
    public class CommonManagerDialog : IManagerDialog
    {

        InputField min;
        InputField sec;

        Toggle showScore;
        Toggle UnshowScore;
        Button GameStart;


        Coroutine KeyinputCor;

        protected override void OnLoad()
        {
            if (cm == null)
                cm = Model.First<PlayContentModel>();

            base.OnLoad();
        }

        protected override void OnEnter()
        {
            KeyinputCor = StartCoroutine(KeyInput());
            itemDataList = cm.ActiveData;
            cm.ResetData();
            base.OnEnter();
        }

        protected override void OnExit()
        {
            if (KeyinputCor != null)
                StopCoroutine(KeyinputCor);
            base.OnExit();
            if (GameStart != null)
                GameStart.onClick.RemoveListener(StartGame);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
        }

        protected override IEnumerator UISet()
        {
            yield return StartCoroutine(base.UISet());

            var time = cm.PlayTime;
            sm.PlayTime = cm.PlayTime;
            var minTime = time / 60;
            var secTime = time % 60;
            var LPanel = this.transform.Find("GameObject").Find("L_Panel");
            min = LPanel.Find("BOX01").Find("Minute").GetComponent<InputField>();
            min.onValueChanged.AddListener(delegate
            {
                int value = sm.PlayTime;
                int sec = value % 60;
                int result = int.Parse(min.text) * 60;
                result += sec;
                sm.PlayTime = result;
                cm.PlayTime = result;
            });
            min.text = minTime.ToString();

            sec = LPanel.Find("BOX01").Find("Second").GetComponent<InputField>();
            sec.onValueChanged.AddListener(delegate
            {
                int value = sm.PlayTime;
                int min = value / 60;
                int result = int.Parse(sec.text);
                result += (min * 60);
                sm.PlayTime = result;
                cm.PlayTime = result;
            });
            sec.text = secTime.ToString();

            showScore = LPanel.Find("BOX02").Find("On").GetComponent<Toggle>();
            UnshowScore = LPanel.Find("BOX02").Find("Off").GetComponent<Toggle>();
            showScore.onValueChanged.AddListener(delegate
            {
                sm.Score = showScore.isOn;
                cm.Score = showScore.isOn;
            });

            if (cm.Score)
            {
                showScore.isOn = true;
            }
            else
            {
                UnshowScore.isOn = true;
            }

            GameStart = this.transform.Find("GameObject").Find("DownButtons").Find("GameStart").GetComponent<Button>();
            GameStart.onClick.AddListener(StartGame);
            yield return null;
        }

        protected override void ListChange(ContentListChange msg)
        {
            base.ListChange(msg);

            int rootindex = cm.GetRootIndex(itemDataList[msg.RootIndex]);
            int targetindex = cm.GetRootIndex(itemDataList[msg.TargetIndex]);
            if (rootindex >= 0 && targetindex >= 0)
            {
                cm.ChangeContents(rootindex, targetindex);
            }
        }

        protected override void ToggleChange(ContentToggleChange msg)
        {
            if (itemDataList.ContainsKey(msg.index))
            {
                itemDataList[msg.index].isUse = msg.isValue;
            }
        }

        void StartGame()
        {
            if (!cm.ResetData())
            {
                SelectError.SetActive(true);
                return;
            }
            cm.SavePlayContentList();
            cm.SaveContentsSetting();
            string content = cm.GetCurrentContent().ContentName;
            Message.Send<CellBig.Contents.Event.EnterContentMsg>(content, new CellBig.Contents.Event.EnterContentMsg());
            // CellBig.Contents.IContent.RequestContentEnter(content);
            CellBig.Contents.IContent.RequestContentEnter<CellBig.Contents.PlayTimeContent>();
            IDialog.RequestDialogExit<CommonManagerDialog>();
        }

        protected override void EndGame()
        {
            if (!cm.ResetData())
            {
                SelectError.SetActive(true);
                return;
            }
            base.EndGame();
            cm.SaveContentsSetting();
            Application.Quit();
        }


        IEnumerator KeyInput()
        {
            while (true)
            {
                yield return null;
#if UNITY_EDITOR
                if (Input.GetKey(KeyCode.LeftShift))
#else
                if (Input.GetKey(KeyCode.LeftAlt))
#endif
                {
                    if (Input.GetKey(KeyCode.H))
                    {
                        IDialog.RequestDialogEnter<SensorPopupDialog>();
                    }
                    else if (Input.GetKey(KeyCode.O))
                    {
                        IDialog.RequestDialogEnter<MasterPopupDialog>();
                    }
                }
            }
        }
    }
}