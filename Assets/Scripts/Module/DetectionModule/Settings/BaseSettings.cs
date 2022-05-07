using System;
using UnityEngine;

namespace CellBig.Module.Detection
{
    [Serializable]
    public class BaseSettings : ISerializationCallbackReceiver
    {
        [SerializeField]
        private bool _enabled = true;
        [SerializeField]
        private ContentType _contentType;
        [SerializeField]
        private int _interval;
        [SerializeField]
        private int _timeout;

        private event Action<BaseSettings> _onSettingsChanged;

        public void OnAfterDeserialize()
        {
            _onSettingsChanged?.Invoke(this);
        }

        public void OnBeforeSerialize()
        {

        }

        public event Action<BaseSettings> OnSettingsChanged
        {
            add => _onSettingsChanged += value;
            remove => _onSettingsChanged -= value;
        }

        public bool Enabled
        {
            get => _enabled;
            set { _enabled = value; _onSettingsChanged?.Invoke(this); }
        }

        public ContentType ContentType
        {
            get => _contentType;
            set => _contentType = value;
        }

        public int Interval
        {
            get => _interval;
            set => _interval = value;
        }

        public int Timeout
        {
            get => _timeout;
            set { _timeout = value; _onSettingsChanged?.Invoke(this); }
        }
    }
}