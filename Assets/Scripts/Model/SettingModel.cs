using UnityEngine;
using CellBig.Constants;
using System.IO;

namespace CellBig.Models
{
    public class SettingModel : Model
    {
        GameModel _owner;
        public LocalizingType LocalizingType = LocalizingType.KR;
        public E_OPENCV_MOD SersonType = E_OPENCV_MOD.E_KINECT;
        public BallBGState BGState = BallBGState.Earth;


        public int MaxInputTick = 10;
        public int PlayTime;
        public int MinContourSize = 5;
        public int ActiveTick = 5;
        public bool Score;
        public bool LineShow;
        public bool BallPlayLineShow;

        public float TutorialTime = 2000;
        public int TutorialPassSensorCount = 2;
        public int TutorialPassSensorTick = 10;

        public int GimmicSoundCount = 20;
        public float ContentTime = 6000;
        public float ContentEndCount = 2;
        public float NoActiveTime = 3000;
        public float middleYpos = 0;




        //BallPlay
        public bool BallPlay_isTutorial = false;

        public float Ball_DynamicFriction = 0.6f;
        public float Ball_StaticFriction = 0.6f;
        public float Ball_Bounciness = 0.7f;
        public float BallLiveLimitTime = 15f;
        public float BallRespawnTime = 3.0f;
        public float BallLiveCheckTime = 3.0f;
        public int BallCount = 5;

        public float Item_SpaceShip_Catch_Time = 3.0f;
        public float Item_SpaceShip_Catch_Delay = 3.0f;
        public float Item_SpaceShip_MoveDistanceY = 5.0f;
        public float Item_SpaceShip_MoveDistanceX = 10.0f;

        public float Item_PortalLiveTime = 5.0f;
        public float Item_PortalReActiveTime = 10.0f;

        public bool Item_Reflection_InfinityStay = false;
        public float Item_Reflection_AddforceY = 300.0f;
        public float Item_Reflection_NewCreateDelayTime = 5.0f;

        public float Item_Spring_Addforce = 300.0f;
        public float Item_Spring_NewCreateDelayTime = 2.0f;

        public float Item_Shard_Reset_time = 2.0f;

        public float Item_Neon_DelayTime = 20.0f;

        public float Item_Goal_MoveTime = 5.0f;
        public float Item_Goal_DelayTime = 20.0f;
        public float Item_Goal_WaitTime = 15f;
        public float Item_GoalDistanceValue = 1.0f;

        public int[] BG_Order;
        public float[] BG_Time;
        public float VideoTransTime = 3.0f;

        public void Setup(GameModel owner)
        {
            _owner = owner;
            LoadSettingFile();
        }

        void LoadSettingFile()
        {
            string line;
            string pathBasic = Application.dataPath + "/StreamingAssets/";
            string path = "Setting/Setting.txt";
            using (StreamReader file = new StreamReader(@pathBasic + path))
            {
                while ((line = file.ReadLine()) != null)
                {
                    if (line.Contains(";") || string.IsNullOrEmpty(line))
                        continue;

                    if (line.StartsWith("Localizing"))
                        LocalizingType = (LocalizingType)int.Parse(line.Split('=')[1]);
                    else if (line.StartsWith("SersonType"))
                        SersonType = (E_OPENCV_MOD)int.Parse(line.Split('=')[1]);
                    else if (line.StartsWith("MaxInputTick"))
                        MaxInputTick = int.Parse(line.Split('=')[1]);
                    else if (line.StartsWith("LineShow"))
                        LineShow = bool.Parse(line.Split('=')[1]);
                    else if (line.StartsWith("BallPlayLineShow"))
                        BallPlayLineShow = bool.Parse(line.Split('=')[1]);
                    else if (line.StartsWith("MinContourSize"))
                        MinContourSize = int.Parse(line.Split('=')[1]);
                    else if (line.StartsWith("ActiveTick"))
                        ActiveTick = int.Parse(line.Split('=')[1]);

                    else if (line.StartsWith("TutorialTime"))
                        TutorialTime = float.Parse(line.Split('=')[1]);
                    else if (line.StartsWith("TutorialPassSensorCount"))
                        TutorialPassSensorCount = int.Parse(line.Split('=')[1]);
                    else if (line.StartsWith("TutorialPassSensorTick"))
                        TutorialPassSensorTick = int.Parse(line.Split('=')[1]);

                    else if (line.StartsWith("GimmicSoundCount"))
                        GimmicSoundCount = int.Parse(line.Split('=')[1]);

                    else if (line.StartsWith("ContentEndCount"))
                        ContentEndCount = int.Parse(line.Split('=')[1]);

                    else if (line.StartsWith("ContentTime"))
                        ContentTime = float.Parse(line.Split('=')[1]);
                    else if (line.StartsWith("NoActiveTime"))
                        NoActiveTime = float.Parse(line.Split('=')[1]);
                    else if (line.StartsWith("middleYpos"))
                        middleYpos = float.Parse(line.Split('=')[1]);

                    //BallPlaySetting
                    else if (line.StartsWith("Ball_BG_Order"))
                    {
                        string bgDATA = line.Split('=')[1];
                        var length = bgDATA.Split(',').Length;
                        if (length > 0)
                        {
                            BG_Order = new int[length];
                            for (int i = 0; i < length; i++)
                            {
                                BG_Order[i] = int.Parse(bgDATA.Split(',')[i]);
                            }
                        }
                        else
                        {
                            BG_Order = new int[1];
                            BG_Order[0] = int.Parse(bgDATA);
                        }
                    }
                    else if (line.StartsWith("Ball_BG_Time"))
                    {
                        string bgDATA = line.Split('=')[1];
                        var length = bgDATA.Split(',').Length;
                        if (length > 0)
                        {
                            BG_Time = new float[length];
                            for (int i = 0; i < length; i++)
                            {
                                BG_Time[i] = float.Parse(bgDATA.Split(',')[i]);
                            }
                        }
                        else
                        {
                            BG_Time = new float[1];
                            BG_Time[0] = float.Parse(bgDATA);
                        }

                    }
                    else if (line.StartsWith("Ball_BG_VideoTransTime"))
                        VideoTransTime = float.Parse(line.Split('=')[1]);

                    else if (line.StartsWith("Ball_DynamicFriction"))
                        Ball_DynamicFriction = float.Parse(line.Split('=')[1]);
                    else if (line.StartsWith("Ball_StaticFriction"))
                        Ball_StaticFriction = float.Parse(line.Split('=')[1]);
                    else if (line.StartsWith("Ball_Bounciness"))
                        Ball_Bounciness = float.Parse(line.Split('=')[1]);

                    else if (line.StartsWith("BallLiveLimitTime"))
                        BallLiveLimitTime = float.Parse(line.Split('=')[1]);
                    else if (line.StartsWith("BallRespawnTime"))
                        BallRespawnTime = float.Parse(line.Split('=')[1]);
                    else if (line.StartsWith("BallCount"))
                        BallCount = int.Parse(line.Split('=')[1]);
                    else if (line.StartsWith("BallLiveCheckTime"))
                        BallLiveCheckTime = float.Parse(line.Split('=')[1]);

                    else if (line.StartsWith("Item_SpaceShip_Catch_Time"))
                        Item_SpaceShip_Catch_Time = float.Parse(line.Split('=')[1]);
                    else if (line.StartsWith("Item_SpaceShip_MoveDistanceY"))
                        Item_SpaceShip_MoveDistanceY = float.Parse(line.Split('=')[1]);
                    else if (line.StartsWith("Item_SpaceShip_MoveDistanceX"))
                        Item_SpaceShip_MoveDistanceX = float.Parse(line.Split('=')[1]);

                    else if (line.StartsWith("Item_Reflection_InfinityStay"))
                        Item_Reflection_InfinityStay = bool.Parse(line.Split('=')[1]);
                    else if (line.StartsWith("Item_Reflection_AddforceY"))
                        Item_Reflection_AddforceY = float.Parse(line.Split('=')[1]);
                    else if (line.StartsWith("Item_Reflection_NewCreateDelayTime"))
                        Item_Reflection_NewCreateDelayTime = float.Parse(line.Split('=')[1]);

                    else if (line.StartsWith("Item_Spring_Addforce"))
                        Item_Spring_Addforce = float.Parse(line.Split('=')[1]);
                    else if (line.StartsWith("Item_Spring_NewCreateDelayTime"))
                        Item_Spring_NewCreateDelayTime = float.Parse(line.Split('=')[1]);

                    else if (line.StartsWith("Item_Shard_Reset_time"))
                        Item_Shard_Reset_time = float.Parse(line.Split('=')[1]);

                    else if (line.StartsWith("Item_Neon_DelayTime"))
                        Item_Neon_DelayTime = float.Parse(line.Split('=')[1]);

                    else if (line.StartsWith("Item_PortalReActiveTime"))
                        Item_PortalReActiveTime = float.Parse(line.Split('=')[1]);
                    else if (line.StartsWith("Item_PortalLiveTime"))
                        Item_PortalLiveTime = float.Parse(line.Split('=')[1]);

                    else if (line.StartsWith("Item_Goal_MoveTime"))
                        Item_Goal_MoveTime = float.Parse(line.Split('=')[1]);
                    else if (line.StartsWith("Item_Goal_DelayTime"))
                        Item_Goal_DelayTime = float.Parse(line.Split('=')[1]);
                    else if (line.StartsWith("Item_GoalDistanceValue"))
                        Item_GoalDistanceValue = float.Parse(line.Split('=')[1]);
                    else if (line.StartsWith("Item_Goal_WaitTime"))
                        Item_Goal_WaitTime = float.Parse(line.Split('=')[1]);
                }
                file.Close();
                line = string.Empty;
            }
        }


        public string GetLocalizingPath()
        {
            return LocalizingType.ToString();
        }
    }
}