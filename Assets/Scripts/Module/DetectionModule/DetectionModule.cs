using System;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;
using UnityEngine;
using CellBig.Module.VideoDevice;
using CellBig.Models;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CellBig.Module.Detection
{
    [RequireComponent(typeof(SettingsManager))]
    public class DetectionModule : IModule
    {
        private Vector2Int _resolution;
        private DetectionSettings _detectionSettings;
        private BaseSettings _baseSettings;
        private DetectionGraph _graph;
        private Thread _graphProcessor;
        private byte[] _frameBuffer;
        private byte[] _inputBuffer;
        private readonly object _lock = new object();
        private bool _enabled = false;

#if UNITY_EDITOR
        private void OnPauseChanged(PauseState state)
        {
            Enabled = state == PauseState.Unpaused && (_baseSettings != null && _baseSettings.Enabled);
        }
#endif

        protected override void OnLoadStart()
        {
            SetResourceLoadComplete();

            InitSettings();
            InitModels();
            RegisterCallbacks();

            Message.Send(new VideoDeviceConnectRequest(this, OnVideoDeviceConnected));

#if UNITY_EDITOR
            EditorApplication.pauseStateChanged += OnPauseChanged;
#endif
        }

        private void OnDestroy()
        {
            UnregisterCallbacks();

            SettingsManager settingsMgr = GetComponent<SettingsManager>();
            DetectionSettings settings = settingsMgr.Load();
            settings.Unsetup();

            if (Enabled)
                Enabled = false;
            Message.Send(new VideoDeviceDisconnectRequest(this));

#if UNITY_EDITOR
            EditorApplication.pauseStateChanged -= OnPauseChanged;
#endif
        }
        
        private void OnVideoDeviceConnected(Vector2Int resolution, bool reconnection)
        {
            _resolution = resolution;
            _detectionSettings.Unsetup();
            _detectionSettings.Setup(_resolution);
            InitBuffers();
            InitGraph();

            //if (reconnection)
            //    Message.Send(new VideoDeviceStopRequest(this));
            Enabled = _baseSettings.Enabled;
        }

        private void RegisterCallbacks()
        {
            if (_baseSettings != null)
                _baseSettings.OnSettingsChanged += OnBaseSettingsChanged;
            Message.AddListener<SetActive>(OnActiveChanged);
        }

        private void UnregisterCallbacks()
        {
            if (_baseSettings != null)
                _baseSettings.OnSettingsChanged -= OnBaseSettingsChanged;
            Message.RemoveListener<SetActive>(OnActiveChanged);
        }
        
        private void OnActiveChanged(SetActive msg)
        {
            Enabled = msg.Active;
        }

        private void OnBaseSettingsChanged(BaseSettings settings)
        {
            Enabled = settings.Enabled;
            _graph.Timeout = settings.Timeout;
        }

        private void InitSettings()
        {
            SettingsManager settingsMgr = GetComponent<SettingsManager>();
            _detectionSettings = settingsMgr.Load();

            _baseSettings = _detectionSettings.Base;
        }

        private void InitModels()
        {
            new DetectionInfoModel(_detectionSettings);
        }

        private void InitBuffers()
        {
            int bufferLen = _resolution.x * _resolution.y * 3;
            _frameBuffer = new byte[bufferLen]; // Main Thread Buffer
            _inputBuffer = new byte[bufferLen]; // Module Thread Buffer
        }

        private void InitGraph()
        {
            _graph = DetectionGraphFactory.Build(_baseSettings.ContentType);
            _graph.Timeout = _baseSettings.Timeout;
        }

        private void StartGraph()
        {
            _graphProcessor = new Thread(ProcessGraph);
            _graphProcessor.Start();
        }

        private void AbortGraph()
        {
            if (_graphProcessor != null && _graphProcessor.IsAlive)
            {
                _graphProcessor.Abort();
                _graphProcessor = null;
            }
        }

        private void Update()
        {
            UpdateGraph();
        }

        private void UpdateGraph()
        {
            if (_graph != null)
            {
                _graph.Update();
            }
        }

        private void ProcessGraph()
        {
            while (_graph == null) ;

            Stopwatch stopwatch = new Stopwatch();
            int deltaInterval = 0;

            while (true)
            {
                int elapsedDiff = (int)(_baseSettings.Interval - deltaInterval);
                if (elapsedDiff > 0)
                {
                    // Sleep 시간까지 계산하여 그래프에 전달
                    deltaInterval = _baseSettings.Interval;
                    Thread.Sleep(elapsedDiff);
                }

                stopwatch.Reset();
                stopwatch.Start();
                {
                    lock (_frameBuffer)
                    {
                        _frameBuffer.CopyTo(_inputBuffer, 0);
                    }

                    _graph.Run(_inputBuffer, deltaInterval);
                }
                stopwatch.Stop();
                deltaInterval = (int)stopwatch.ElapsedMilliseconds;

                //UnityEngine.Debug.Log(deltaInterval);
            }
        }

        private bool Enabled
        {
            get
            {
                return _enabled;
            }

            set
            {
                //if (_enabled != value)
                {
                    if (value)
                    {
                        Message.Send(new VideoDeviceStopRequest(this));
                        Message.Send(new VideoDevicePlayRequest(this, _frameBuffer));
                        AbortGraph();
                        StartGraph();
                    }
                    else
                    {
                        Message.Send(new VideoDeviceStopRequest(this));
                        AbortGraph();
                    }
                }

                _enabled = value;
            }
        }
    }
}