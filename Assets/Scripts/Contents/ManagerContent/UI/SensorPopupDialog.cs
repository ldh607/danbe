using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CellBig.Models;
using CellBig.Common;
using CellBig.UI.Event;

namespace CellBig.UI
{
    public class SensorPopupDialog : PasswordPopupDialog
    {
        protected override void StartGame()
        {
            Debug.Log(inputPassword.text.ToLower()+"/////////////" + cm.Password.ToLower());
            if (string.Compare(inputPassword.text, cm.Password,false) >= 0)
            {
                IDialog.RequestDialogExit<CommonManagerDialog>();
                IDialog.RequestDialogEnter<IRCameraSensorSettingDialog>();
                IDialog.RequestDialogExit<SensorPopupDialog>();
            }
            else
            {
                result.text = "비밀번호가 다릅니다. 다시 확인해주세요.";
            }
        }

        protected override void ExitGame()
        {
            IDialog.RequestDialogExit<SensorPopupDialog>();
        }
    }
}
