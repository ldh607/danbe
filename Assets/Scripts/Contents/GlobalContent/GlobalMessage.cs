using UnityEngine;
using CellBig.Constants;


namespace CellBig.UI.Event
{
    public class FadeInMsg : Message
    {
        public bool kiosk = true;
        public bool screen = true;
        public bool all = true;
        public float time = 0.5f;

        public FadeInMsg(bool kiosk = true, bool screen = true, float time = 0.5f)
        {
            this.kiosk = kiosk;
            this.screen = screen;
            this.time = time;

            if (kiosk && screen)
                all = true;
        }
    }

    public class FadeOutMsg : Message
    {
        public bool kiosk = true;
        public bool screen = true;
        public bool all = true;
        public float time = 0.5f;

        public FadeOutMsg(bool kiosk = true, bool screen = true, float time = 0.5f)
        {
            this.kiosk = kiosk;
            this.screen = screen;
            this.time = time;

            if (kiosk && screen)
                all = true;
        }
    }
}