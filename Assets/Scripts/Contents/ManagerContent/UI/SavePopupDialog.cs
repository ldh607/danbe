using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using CellBig.Models;
using CellBig.Common;
using CellBig.UI.Event;

public class SavePopupDialog : MonoBehaviour
{
    protected Button Enter;
    protected Button Exit;
    public Action SaveFunc;
    public Action UnsaveFunc;
    bool isSet = false;
    private void Awake()
    {

        if (!isSet)
            UiSet();
    }

    protected void UiSet()
    {
        Enter = transform.Find("Image").Find("Save").GetComponent<Button>();
        Enter.onClick.AddListener(delegate { SaveFunc(); });
        Exit = transform.Find("Image").Find("UnSave").GetComponent<Button>();
        Exit.onClick.AddListener(delegate { UnsaveFunc(); });
        isSet = true;
    }
}
