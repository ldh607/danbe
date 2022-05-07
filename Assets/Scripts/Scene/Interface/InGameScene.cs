using UnityEngine;
using System.Collections;
using CellBig.Models;
using CellBig.Constants;
using CellBig.Contents;
using CellBig.UI.Event;

namespace CellBig.Scene
{
	public class InGameScene : IScene
	{
        protected override void OnLoadComplete()
        {
        }

        IEnumerator ChangeContent()
        {
            Message.Send<FadeInMsg>(new FadeInMsg());
            yield return new WaitForSeconds(2.0f);
            var pcm = Model.First<PlayContentModel>();
            string nextContent = pcm.GetCurrentContent().ContentName;
          //  IContent.RequestContentEnter(nextContent);
            Message.Send<CellBig.Contents.Event.EnterContentMsg>(nextContent, new CellBig.Contents.Event.EnterContentMsg());
        }

    }
}
