using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CellBig;
using CellBig.Models;

namespace CellBig.Contents
{
    public class ManagerContent : IContent
    {
        Coroutine inputCor;
        PlayContentModel cm;

        protected override void OnEnter()
        {
            cm = Model.First<PlayContentModel>();
            
            inputCor = StartCoroutine(SettingInput());
        }

        protected override void OnExit()
        {
            if (inputCor != null)
                StopCoroutine(inputCor);
        }

        IEnumerator SettingInput()
        {
            while (true)
            {
#if UNITY_EDITOR
                if (Input.GetKey(KeyCode.LeftShift))
#else
                if (Input.GetKey(KeyCode.LeftAlt))
#endif
                {
                    if (Input.GetKeyDown(KeyCode.M))
                    {
                        // 씬 Additive 방식이면 씬 언로드. (예외처리 있어야될거 같긴한데↗~ 없이도 에러 안뜨는거 확인..)
                        UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(cm.GetCurrentContent().ContentName);

                        Message.Send<CellBig.Contents.Event.ExitContentMsg>(cm.GetCurrentContent().ContentName, new CellBig.Contents.Event.ExitContentMsg());
                        IContent.RequestContentExit<PlayTimeContent>();
                        UI.IDialog.RequestDialogEnter<UI.CommonManagerDialog>();

                        yield return new WaitForEndOfFrame();

                        // 모든 사운드 종료.
                        SoundManager.Instance.StopAllSound();
                    }
                    if( cm.isDirectSensor)
                    {
                        if(Input.GetKeyDown(KeyCode.H))
                        {
                            UI.IDialog.RequestDialogEnter<UI.SensorPopupDialog>();
                        }
                    }
                }

                yield return null;
            }
        }
    }
}