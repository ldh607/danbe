using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Text;
using CellBig.Common;

public class Log : MonoSingleton<Log>
{
    static FileStream _fileLog;
    static StreamWriter _writer;
    string _logPath;
    const string _logFilename = "Log";

    static FileStream _coinFileLog;
    static StreamWriter _coinWriter;
 
    const string _coinLogFilename = "Money";

    bool _bInitialize = false;

    public bool IsInitializeComplete { get { return _bInitialize; } }

    //-----------------------------------------------------------------------------------------------
    protected override void Init()
    {
        if(!_bInitialize)
        {
            _logPath = Application.dataPath.Replace("/Assets", "/Log/Curling_Log");

            if (!System.IO.Directory.Exists(_logPath))
                System.IO.Directory.CreateDirectory(_logPath);

            string fullpath = MakeFullpath(_logFilename, DateTime.Now.ToString("yyyyMMdd_HHmmss"));
            _fileLog = new FileStream(fullpath, FileMode.Append);
            _writer = new StreamWriter(_fileLog);

            fullpath = MakeFullpath(_coinLogFilename, DateTime.Now.ToString("yyyyMMdd_HHmmss"));
            _coinFileLog = new FileStream(fullpath, FileMode.Append);
            _coinWriter = new StreamWriter(_coinFileLog);

            _bInitialize = true;
        }
    }
    //-----------------------------------------------------------------------------------------------
    protected override void Release()
    {
        if(_writer != null)
            _writer.Close();
        if (_fileLog != null)
            _fileLog.Close();
        if (_coinWriter != null)
            _coinWriter.Close();
        if (_coinFileLog != null)
            _coinFileLog.Close();

        _bInitialize = false;
    }
    //-----------------------------------------------------------------------------------------------
    public void logAndEditor(string logmsg)
    {
        _writer.WriteLine("[" + DateTime.Now + "] - " + logmsg);
#if UNITY_EDITOR
        Debug.LogError(string.Format("(Log.cs/logAndEditor) -[ msg : {0} ]", logmsg));
#endif
    }
    //-----------------------------------------------------------------------------------------------
    public void log(string logmsg)
    {
        _writer.WriteLine("["+DateTime.Now+ "/" + DateTime.Now.Millisecond + "] - "+logmsg);
    }
    //-----------------------------------------------------------------------------------------------
    public void logCoin(string logmsg)
    {
        _coinWriter.WriteLine(logmsg);
#if UNITY_EDITOR
        Debug.LogError(string.Format("(Log.cs/logCoin) -[ msg : {0} ]", logmsg));
#endif
    }
    //-----------------------------------------------------------------------------------------------
    string MakeFullpath(string fileName, string dateTimeString)
    {
        return string.Format("{0}/{1}_{2}.txt", _logPath, fileName, dateTimeString);
    }
    //-----------------------------------------------------------------------------------------------
}
