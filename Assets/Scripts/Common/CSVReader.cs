using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class CSVReader
{
    public static List<Dictionary<string, string>> Read(string fileName)
    {
        CB.Log("Try Read " + fileName + "...");
        var list = new List<Dictionary<string, string>>();

        string pathBasic = string.Format(Application.streamingAssetsPath);
        string path = string.Format("/CSVData/");
        string fullPath = string.Format(pathBasic + path + fileName);
        string[] header;

#if UNITY_ANDROID

			WWW www = new WWW(fullPath);
			while (!www.isDone) ;

			string[] dataLine = www.text.Split('\n');

			if (string.IsNullOrEmpty(dataLine[0]))
				return null;

			header = dataLine[0].Split(',');

			for (int l = 1; l < dataLine.Length; l++)
			{
				if (string.IsNullOrEmpty(dataLine[l]))
					break;
				var data_values = dataLine[l].Split(',');
				var data = new Dictionary<string, string>();
				for (int i = 0; i < data_values.Length; i++)
				{
					data.Add(header[i].Trim(), data_values[i].Trim());
				}
				list.Add(data);
			}

			www.Dispose();
#else
        using (StreamReader sr = new StreamReader(fullPath, System.Text.Encoding.Default))
        {
            //첫줄 읽어서 헤드로 지정
            string headerLine = sr.ReadLine().Trim();
            if (string.IsNullOrEmpty(headerLine)) return null;

            header = headerLine.Split(',');

            while (!sr.EndOfStream)
            {
                string data_String = sr.ReadLine().Trim();
                var data_values = data_String.Split(',');
                var data = new Dictionary<string, string>();
                for (int i = 0; i < data_values.Length; i++)
                {
                    data.Add(header[i].Trim(), data_values[i].Trim());
                }
                list.Add(data);
            }
            sr.Close();
        }
#endif
        CB.Log("Read " + fileName + " is Ended");

        return list;

    }
}
