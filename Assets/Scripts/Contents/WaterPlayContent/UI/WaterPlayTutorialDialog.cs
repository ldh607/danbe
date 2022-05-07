using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CellBig.UI;

public class WaterPlayTutorialDialog : IDialog
{
    protected override void OnEnter()
    {
        this.transform.parent.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
    }

    protected override void OnExit()
    {
        this.transform.parent.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
    }

}
