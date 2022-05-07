using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundData
{
	public List<Param> list = new List<Param>();

	public class Param
	{
		public int Index;
		public string FilePath;
		public string FileName;
		public float Volum;
		public bool Loop;
	}
}
