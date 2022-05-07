using UnityEngine;
using UnityEngine.UI;
using System;

namespace CellBig.Constants
{
    public static class SceneConstants
    {
    }

    public enum ContentType
    {
        WaterPlay = 2,
        BallPlay = 4,
        TestGame1 = 6,
        TestGame2 = 7,
        TestGame3 = 8,
    }

    [Serializable]
    public class GameData
    {
        public Constants.SceneName GameName;
        public String GameNameShow;
        public Sprite GameThumnail;
        public bool GameSelectToggle;
        public int GameTime;
        public GameObject BackGround;
    }

    public enum BallBGState
    {
        Change = 0,
        Earth,
        Moon,
        Space,
    }

    public enum SceneName
    {
        None,
        SelectGameScene,
        WaterPlayTutorialScene,
        WaterPlayScene,
        BallPlayTutorialScene,
        BallPlayScene,
        TestGame1,
        TestGame2,
        TestGame3,
    }

    public enum ModuleName
    {
        None,
        BaseCamera,
        MRCamera,
        ProjectionCamera,
        MediaZen,
        Bluetooth,

        VideoDevice = 10,
        Detection,

    }
}
