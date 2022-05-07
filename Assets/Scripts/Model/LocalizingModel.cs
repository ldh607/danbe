using System;
using UnityEngine;
using System.Collections.Generic;
using CellBig.Constants;

namespace CellBig.Models
{
    public class LocalizingInfo
    {
        public int Index = 0;
        public string KR = "";
        public string JP = "";
        public string EN = "";
        public string CH = "";
    }

    public class LocalizingModel : Model
    {
        GameModel _owner;
        Dictionary<int, LocalizingInfo> localizingInfos = new Dictionary<int, LocalizingInfo>();

        public void Setup(GameModel owner)
        {
            _owner = owner;
            LoadFile();
        }

        void LoadFile()
        {
            // var data = Util.Instance.ReadCSV("Localizing");
            var data = CSVReader.Read(Application.streamingAssetsPath + "/Tables/Localizing.csv");
            for (int i = 0; i < data.Count; i++)
            {
                LocalizingInfo info = new LocalizingInfo();
                info.Index = int.Parse(data[i]["ID"].ToString());
                info.KR = data[i]["KR"].ToString();
                info.EN = data[i]["EN"].ToString();
                info.JP = data[i]["JP"].ToString();
                info.CH = data[i]["CH"].ToString();

                localizingInfos.Add(i, info);
            }
        }

        public string GetLocalizing(int stringIndex)
        {
            string localizing = "";

            if (localizingInfos.ContainsKey(stringIndex))
            {
                switch (_owner.setting.Model.LocalizingType)
                {
                    case LocalizingType.KR:
                        localizing = localizingInfos[stringIndex].KR;
                        break;
                    case LocalizingType.JP:
                        localizing = localizingInfos[stringIndex].JP;
                        break;
                    case LocalizingType.EN:
                        localizing = localizingInfos[stringIndex].EN;
                        break;
                    case LocalizingType.CH:
                        localizing = localizingInfos[stringIndex].CH;
                        break;
                }
            }
            else
            {
                Debug.LogWarning("Not String Index");
            }

            return localizing;
        }
    }
}
