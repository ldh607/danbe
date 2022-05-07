using CellBig.Models;
using CellBig.UI;
using UnityEngine;

namespace CellBig.Scene
{
    public class TitleScene : IScene
    {
        protected override void OnLoadStart()
        {
            SetResourceLoadComplete();
        }

        protected override void OnLoadComplete()
        {
        }
    }
}
