// -------------------------------------------------------------------------------------------------
// Filename: Logger.cs
// Author: Song Ji Hun. [aka. CraZy GolMae] <jihun.song@pocatcom>
// Date: 2015.04.23
//
// Desc: 
//
// Copyright (c) 2015 Pocatcom. All rights reserved.
// -------------------------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;


//#if !UNITY_EDITOR
//public static class Debug
//{
//	[System.Diagnostics.Conditional("UNITY_EDITOR")]
//	public static void Log(object message) {}
//
//	[System.Diagnostics.Conditional("UNITY_EDITOR")]
//	public static void Log(object message, UnityEngine.Object context) {}
//
//	[System.Diagnostics.Conditional("UNITY_EDITOR")]
//	public static void LogWarning(object message) {}
//
//	[System.Diagnostics.Conditional("UNITY_EDITOR")]
//	public static void LogWarning(object message, UnityEngine.Object context) {}
//
//	[System.Diagnostics.Conditional("UNITY_EDITOR")]
//	public static void LogError(object message) {}
//
//	[System.Diagnostics.Conditional("UNITY_EDITOR")]
//	public static void LogError(object message, UnityEngine.Object context) {}
//
//	[System.Diagnostics.Conditional("UNITY_EDITOR")]
//	public static void LogException(Exception exception) {}
//
//	[System.Diagnostics.Conditional("UNITY_EDITOR")]
//	public static void LogException(Exception exception, UnityEngine.Object context) {}
//
//	[System.Diagnostics.Conditional("UNITY_EDITOR")]
//	public static void DrawLine(Vector3 start, Vector3 end) {}
//
//	[System.Diagnostics.Conditional("UNITY_EDITOR")]
//	public static void DrawLine(Vector3 start, Vector3 end, Color color) {}
//
//	[System.Diagnostics.Conditional("UNITY_EDITOR")]
//	public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration) {}
//
//	[System.Diagnostics.Conditional("UNITY_EDITOR")]
//	public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration, bool depthTest) {}
//
//	[System.Diagnostics.Conditional("UNITY_EDITOR")]
//	public static void DrawRay(Vector3 start, Vector3 dir) {}
//
//	[System.Diagnostics.Conditional("UNITY_EDITOR")]
//	public static void DrawRay(Vector3 start, Vector3 dir, Color color) {}
//
//	[System.Diagnostics.Conditional("UNITY_EDITOR")]
//	public static void DrawRay(Vector3 start, Vector3 dir, Color color, float duration) {}
//
//	[System.Diagnostics.Conditional("UNITY_EDITOR")]
//	public static void DrawRay(Vector3 start, Vector3 dir, Color color, float duration, bool depthTest) {}
//
//	[System.Diagnostics.Conditional("UNITY_EDITOR")]
//	public static void Break() { Debug.Break(); }
//
//	[System.Diagnostics.Conditional("UNITY_EDITOR")]
//	public static void DebugBreak() { Debug.DebugBreak(); }
//
//	public static bool isDebugBuild { get; set; }
//}
//#endif


namespace CellBig.Common
{
	public class Logger : MonoSingleton<Logger>
	{
		public static bool isActive = true;

		static readonly string _logPath = Application.persistentDataPath + "/Log";
		const string _logFilename = "Log";

		FileStream _logFileStream = null;
		StreamWriter _logWriter = null;

		StringBuilder _builder = null;


		protected override void Init()
		{
			StartLogs();
		}

		protected override void Release()
		{
			EndLogs();
		}

		void StartLogs()
		{
			_builder = new StringBuilder();

//			if (Application.platform == RuntimePlatform.Android ||
//			    Application.platform == RuntimePlatform.IPhonePlayer)
//			{
//				isActive = false;
//			}

			if (isActive)
			{
				OpenLogFile(_logFileStream, _logWriter);
			}
		}

		void EndLogs()
		{
			CloseLogFile(_logFileStream, _logWriter);
		}

		public void SystemLog(params object[] messages)
		{
			if (!isActive)
				return;

			_builder.Append(DateTime.Now.ToString("[yyyy.MM.dd HH:mm:ss.ffff] [SYSTEM]"));

			for (int i = 0; i < messages.Length; ++i)
			{
				object o = messages[i];

				if (o is IList)
				{
					IList collection = (IList)o;

					foreach (object obj in collection)
					{
						_builder.Append("\t");
						_builder.Append(obj);
					}
				}
				else
				{
					_builder.Append("\t");
					_builder.Append(o);
				}
			}

			Log(_logFileStream, _logWriter, _builder.ToString());

			_builder.Length = 0;
		}

		public void PacketLog(bool isSend, string cmd, params object[] packet)
		{
			if (!isActive)
				return;

			_builder.Append(DateTime.Now.ToString("[yyyy.MM.dd HH:mm:ss.ffff]"));
			_builder.Append(isSend ? " [GAME]\t[SEND]\t" : " [GAME]\t[RECV]\t");
			_builder.Append(cmd);

			for (int i = 0; i < packet.Length; ++i)
			{
				object o = packet[i];

				if (o is IList)
				{
					IList collection = (IList)o;

					foreach (object obj in collection)
					{
						_builder.Append("\t");
						_builder.Append(obj);
					}
				}
				else
				{
					_builder.Append("\t");
					_builder.Append(o);
				}
			}

			//if (isSend)
			//{
			//	_builder.Append("\t");
			//	_builder.Append(AfGlobalData.Instance.m_kCharInfo.ticket);
			//}

			Log(_logFileStream, _logWriter, _builder.ToString());

			_builder.Length = 0;
		}

		public void SocketLog(bool isSend, string name, params object[] packet)
		{
			if (!isActive)
				return;

			_builder.Append(DateTime.Now.ToString("[yyyy.MM.dd HH:mm:ss.ffff]"));
			_builder.Append(isSend ? " [SOCK]\t[SEND]\t" : " [SOCK]\t[RECV]\t");
			_builder.Append(name);

			for (int i = 0; i < packet.Length; ++i)
			{
				object o = packet[i];

				if (o is IList)
				{
					IList collection = (IList)o;

					foreach (object obj in collection)
					{
						_builder.Append("\t");
						_builder.Append(obj);
					}
				}
				else
				{
					_builder.Append("\t");
					_builder.Append(o);
				}
			}

			Log(_logFileStream, _logWriter, _builder.ToString());

			_builder.Length = 0;
		}

		void Log(FileStream fs, StreamWriter writer, string message)
		{
			if (fs == null || writer == null)
				return;

			if (fs.Length > 10485760 /* 10MB */)
			{
				CloseLogFile(fs, writer);
				OpenLogFile(fs, writer);

				if (fs == null || writer == null)
					return;
			}

			writer.WriteLine(message);
			Debug.Log(message);
		}

		void OpenLogFile(FileStream fs, StreamWriter writer)
		{
			if (fs != null || writer != null)
				return;

			if (!Directory.Exists(_logPath))
			{
				Directory.CreateDirectory(_logPath);
			}

			CleanUpLogFiles();

			string fullpath = MakeFullpath(DateTime.Now.ToString("yyyyMMdd_HHmmss"));
			Debug.Log("PacketLog Start: " + fullpath);

			fs = new FileStream(fullpath, FileMode.Append);
			writer = new StreamWriter(fs);
			writer.AutoFlush = true;

			writer.WriteLine("//------------------------------------------------------------");
			writer.WriteLine("//                         START LOG                          ");
			writer.WriteLine("//------------------------------------------------------------");
			writer.WriteLine("// Type: {0}, Date: {1}", _logFilename, DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss"));
			writer.WriteLine("//------------------------------------------------------------");
		}

		void CloseLogFile(FileStream fs, StreamWriter writer)
		{
			if (writer != null)
			{
				writer.WriteLine("//------------------------------------------------------------");
				writer.WriteLine("// Date: {0}", DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss"));
				writer.WriteLine("//------------------------------------------------------------");
				writer.WriteLine("//                          END LOG                           ");
				writer.WriteLine("//------------------------------------------------------------");

				writer.Close();
			}

			if (fs != null)
			{
				fs.Close();
			}
		}

		string MakeFullpath(string dateTimeString)
		{
			return string.Format("{0}/{1}_{2}.txt", _logPath, _logFilename, dateTimeString);
		}

		void CleanUpLogFiles()
		{
			var di = new DirectoryInfo(_logPath);
			var fis = di.GetFiles("*.txt");

			long totalSize = 0;
			for (int i = 0; i < fis.Length; ++i)
				totalSize += fis[i].Length;

			if (totalSize > 20971520 /* 20MB */)
			{
				List<DateTime> dts = new List<DateTime>();

				for (int i = 0; i < fis.Length; ++i)
				{
					var dt = DateTime.ParseExact(fis[i].Name.Substring(_logFilename.Length + 1, 15), "yyyyMMdd_HHmmss", System.Globalization.CultureInfo.InvariantCulture);
					dts.Add(dt);
				}

				var earliest = dts.Min();
				File.Delete(MakeFullpath(earliest.ToString("yyyyMMdd_HHmmss")));
			}
		}
	}
}
