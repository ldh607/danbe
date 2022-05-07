using UnityEngine;

namespace CellBig.Module.Detection
{
    public abstract class DetectionNodeBase<TMessage, TInput, TOutput> : IDetectionNode
        where TMessage : OutputMessageBase<TOutput>, new()
    {
        private readonly object _lock = new object();
        private readonly TMessage _message;
        private TOutput _msgOutput;
        private bool _swapped = true;

        public DetectionNodeBase()
        {
            _message = new TMessage();
        }

        ~DetectionNodeBase()
        {
            
        }
        
        protected abstract TOutput RunImpl(TInput input, int deltaInterval);
        protected abstract void Copy(TOutput from, ref TOutput to);
        protected virtual void Clear(TOutput output) { }

        public DetectionNodeOutput<object> Run(object input, int deltaInterval)
        {
#if UNITY_EDITOR
            if (input.GetType() != typeof(TInput))
            {
                Debug.LogError($"[{GetType().Name}] 인풋 타입이 일치하지않습니다.");
                return new DetectionNodeOutput<object>(default, false, false);
            }
#endif

            bool sendable = true;
            TOutput output = RunImpl((TInput)input, deltaInterval);

            if (sendable)
            {
                sendable = false;

                sendable = _swapped;
                if (sendable)
                {
                    lock (_lock)
                    {
                        if (_msgOutput != null)
                            Clear(_msgOutput);
                        Copy(output, ref _msgOutput);
                        _swapped = false;
                    }
                }
            }

            return new DetectionNodeOutput<object>(output, true, sendable);
        }

        public void SendOutput()
        {
            lock (_lock)
            {
                TOutput temp = _message.Value;
                _message.Value = _msgOutput;
                _msgOutput = temp;
                _swapped = true;
            }

            // Message를 받는 콜백 함수가 없을경우 로그를 남기기때문에 비활성화 한 후에 보낸다.
            Debug.unityLogger.logEnabled = false;
            Message.Send<TMessage>(_message);
            Debug.unityLogger.logEnabled = true;

            Clear(_message.Value);
        }

        public void Reset()
        {
            _swapped = true;
        }
    }
}