using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CellBig.UI;

public class BallPlayTutorialDialog : IDialog
{
    Canvas _canvas;
    
    protected override void OnLoad()
    {
    }

    protected override void OnEnter()
    {
        _canvas = this.transform.parent.GetComponent<Canvas>();
        _canvas.renderMode = RenderMode.ScreenSpaceCamera;
    }

    protected override void OnExit()
    {
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
    }

}
