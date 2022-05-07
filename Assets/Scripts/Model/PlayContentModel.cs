using System;
using UnityEngine;
using System.Collections.Generic;
using CellBig.Constants;
using System.Text;
using System.IO;
using CellBig.Scene;


namespace CellBig.Models
{
    public class PlayContentModel : Model
    {
        public class ContentData
        {
            public string ContentName;
            public bool isActive;
            public bool isScore;
            public bool isMultiRay;
            public float Rayinterval;
            public float RayDistance;
            public bool isUse;
        }

        public Dictionary<int, ContentData> RootData = new Dictionary<int, ContentData>();
        public Dictionary<int, ContentData> ActiveData = new Dictionary<int, ContentData>();
        public List<ContentData> playableContnet = new List<ContentData>();

        GameModel _owner;
        int currentPlayContentIndex = 0;

        static string fileName = "PlayContents";

        string pathBasic = Application.dataPath + "/StreamingAssets/";
        string settingPath = "Setting/ContentsSetting.txt";

        public int PlayTime = 0;        // 컨텐츠 플레이 시간
        public bool Score = true;       // 스코어 On / Off
        public bool isDirectSensor;

        public string Password = "0000";

        public Camera mainCamera;

        public void Setup(GameModel owner)
        {
            _owner = owner;
            currentPlayContentIndex = 0;
           // LoadFile();
            LoadSettingFile();
        }

        void LoadFile()
        {
            fileName = "PlayContents";
            //var data = Util.Instance.ReadCSV(fileName);
            var data = CSVReader.Read(fileName);
            for (int i = 0; i < data.Count; i++)
            {
                ContentData temp = new ContentData();
                temp.ContentName = data[i]["Name"].ToString();
                temp.isActive = bool.Parse(data[i]["Active"].ToString());
                temp.isScore = bool.Parse(data[i]["Score"].ToString());
                temp.isMultiRay = bool.Parse(data[i]["MultiRay"].ToString());
                temp.Rayinterval = float.Parse(data[i]["RayInterval"].ToString());
                temp.RayDistance = float.Parse(data[i]["RayDistance"].ToString());
                temp.isUse = bool.Parse(data[i]["Use"].ToString());
                RootData.Add(i, temp);
            }
            ResetData();
        }

        void LoadSettingFile()
        {
            string line;

            using (StreamReader file = new StreamReader(@pathBasic + settingPath))
            {
                while ((line = file.ReadLine()) != null)
                {
                    if (line.Contains(";") || string.IsNullOrEmpty(line))
                        continue;

                    if (line.StartsWith("PlayTime"))
                        PlayTime = int.Parse(line.Split('=')[1]);
                    else if (line.StartsWith("Score"))
                        Score = bool.Parse(line.Split('=')[1]);
                    else if (line.StartsWith("Password"))
                        Password = (line.Split('=')[1]);
                    else if (line.StartsWith("isDirectSensor"))
                        isDirectSensor = bool.Parse(line.Split('=')[1]);
                }

                file.Close();
                line = string.Empty;
            }
        }

        public void SaveContentsSetting()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("PlayTime=" + PlayTime.ToString());
            sb.AppendLine("Score=" + Score.ToString());
            sb.AppendLine("Password=" + Password.ToString());
            StreamWriter outStream = System.IO.File.CreateText(pathBasic + settingPath);
            outStream.WriteLine(sb);
            outStream.Close();
        }

        public bool ResetData(bool isMaster = false)
        {
            currentPlayContentIndex = 0;
            int activeCount = 0;
            ActiveData.Clear();
            playableContnet.Clear();
            for (int i = 0; i < RootData.Count; i++)
            {
                if (RootData[i].isActive)
                {
                    ActiveData.Add(activeCount, RootData[i]);
                    activeCount++;
                    if (RootData[i].isUse)
                        playableContnet.Add(RootData[i]);
                }
            }
            if (isMaster)
            {
                if (ActiveData.Count == 0)
                {
                    return false;
                }
            }
            else
            {
                if (playableContnet.Count == 0)
                {
                    return false;
                }
            }
            return true;
        }

        public int GetRootIndex(ContentData data)
        {
            if (RootData.ContainsValue(data))
            {
                foreach (var item in RootData)
                {
                    if (item.Value == data)
                        return item.Key;
                }
            }
            return -1;
        }


        public void ChangeContents(int rootIndex, int targetIndex)
        {
            PlayContentModel.ContentData root;
            RootData.TryGetValue(rootIndex, out root);
            PlayContentModel.ContentData target;
            RootData.TryGetValue(targetIndex, out target);

            if (root == null || target == null)
                return;

            RootData.Remove(rootIndex);
            RootData.Remove(targetIndex);

            RootData.Add(rootIndex, target);
            RootData.Add(targetIndex, root);
        }

        public int GetCurrentIndex()
        {
            //int tempindex = _playContentIndex;

            //if (_playContents.Count <= _playContentIndex)
            //    _playContentIndex = 0;

            return currentPlayContentIndex;
        }

        public void Log()
        {
            Debug.LogWarning("---------------------------------------");
            Debug.LogWarning("Count : " + ActiveData.Count);
            Debug.LogWarning("GetCurrentContent : " + GetCurrentContent().ContentName);
        }

        public ContentData GetCurrentContent()
        {
            if (playableContnet.Count == 0)
                ResetData();
            var content = playableContnet[currentPlayContentIndex];
            return content;
        }


        public string GetNextContentName()
        {
            ++currentPlayContentIndex;
            if (currentPlayContentIndex >= playableContnet.Count)
                currentPlayContentIndex = 0;

            ContentData var = playableContnet[currentPlayContentIndex];
            if (var != null)
            {
                string returnContentName = var.ContentName;
                return returnContentName;
            }
            return "";
        }

        public void SavePlayContentList()
        {
            List<string[]> studentData = new List<string[]>();
            string[] data = new string[8];
            data[0] = "ID";
            data[1] = "Name";
            data[2] = "Active";
            data[3] = "RayInterval";
            data[4] = "MultiRay";
            data[5] = "RayDistance";
            data[6] = "Score";
            data[7] = "Use";
            studentData.Add(data);

            for (int i = 0; i < RootData.Count; i++)
            {
                data = new string[8];
                data[0] = (i + 1).ToString();
                data[1] = RootData[i].ContentName;
                data[2] = RootData[i].isActive.ToString();
                data[3] = RootData[i].Rayinterval.ToString();
                data[4] = RootData[i].isMultiRay.ToString();
                data[5] = RootData[i].RayDistance.ToString();
                data[6] = RootData[i].isScore.ToString();
                data[7] = RootData[i].isUse.ToString();
                studentData.Add(data);
            }

            string[][] output = new string[studentData.Count][];

            for (int i = 0; i < output.Length; i++)
            {
                output[i] = studentData[i];
            }

            int length = output.GetLength(0);
            string delimiter = ",";

            StringBuilder sb = new StringBuilder();

            for (int index = 0; index < length; index++)
                sb.AppendLine(string.Join(delimiter, output[index]));

            StreamWriter outStream = System.IO.File.CreateText(Application.dataPath + "/StreamingAssets/Tables/" + fileName + ".csv");
            outStream.WriteLine(sb);
            outStream.Close();

            currentPlayContentIndex = 0;
        }
    }
}
