using System;
using System.IO;
using UnityEngine;
using CellBig.Models;

namespace CellBig.Module.Detection
{
    public class SettingsManager : MonoBehaviour
    {
        [SerializeField]
        private string _filePath = "Setting/detection_settings.json";
        [SerializeField]
        private DetectionSettings _settings;
        [SerializeField]
        private KeyCode _saveKey;

        private void Awake()
        {
            Message.AddListener<SaveSettings>(OnSaveMessageReceived);
        }

        private void OnDestroy()
        {
            Message.RemoveListener<SaveSettings>(OnSaveMessageReceived);
        }

        private void Update()
        {
            if (Input.GetKeyDown(_saveKey))
            {
                Save();
            }

            _settings.Update();
        }
        
        private void OnSaveMessageReceived(SaveSettings msg)
        {
            Save();
        }

        private bool CheckPath()
        {
            if (string.IsNullOrEmpty(_filePath))
            {
                Debug.LogError($"[{nameof(SettingsManager)}] FilePath 경로가 입력되지 않았습니다.");
                return false;
            }

            return true;
        }

        public void Save()
        {
            if (!CheckPath())
                return;

            string path = Application.streamingAssetsPath + '/' + _filePath;
            string json = JsonUtility.ToJson(_settings, true);
            File.WriteAllText(path, json);
        }

        public DetectionSettings Load()
        {
            if (!CheckPath())
                return _settings;

            string path = Application.streamingAssetsPath + '/' + _filePath;
            if (!File.Exists(path))
                Save();

            string json = File.ReadAllText(path);
            JsonUtility.FromJsonOverwrite(json, _settings);

            return _settings;
        }
    }
}