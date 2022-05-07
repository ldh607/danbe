using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace CellBig.UI
{
    public class ReadyDialog : IDialog
    {
        
        public Animator mAnimator;
        protected override void OnEnter()
        {
          
            mAnimator.Play(0);
        }

        protected override void OnExit()
        {
            
        }

    }
}

