using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CellBig.Constants;
using System.Text;
using System.IO;
using CellBig.Common;

namespace CellBig.Models
{
	public class SoundTableSettingModel : Model
	{
		GameModel _owner;
		SoundData _soundData = new SoundData();
		
		public void Setup(GameModel owner)
		{
			_owner = owner;
			LoadSettingFile();
		}

		void LoadSettingFile()
		{
			var csvData = CSVReader.Read("SoundTableSetting.csv");
			for(int i=0; i< csvData.Count; i++)
			{
				SoundData.Param p = new SoundData.Param();
				p.Index = int.Parse(csvData[i]["Index"]);
				p.FilePath = csvData[i]["FilePath"];
				p.FileName = csvData[i]["FileName"];
				p.Volum = float.Parse(csvData[i]["Volum"]);
				p.Loop = bool.Parse(csvData[i]["Loop"]);
				_soundData.list.Add(p);
			}
		}
        
		public SoundData GetSoundData()
		{
			return _soundData;
		}
	}

}
