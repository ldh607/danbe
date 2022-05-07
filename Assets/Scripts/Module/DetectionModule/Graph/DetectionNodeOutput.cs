
namespace CellBig.Module.Detection
{
    public readonly struct DetectionNodeOutput<T>
    {
        public DetectionNodeOutput(T value, bool success, bool sendable)
        {
            Value = value;
            Success = success;
            Sendable = sendable;
        }

        public T Value { get; }
        public bool Success { get; }
        public bool Sendable { get; }
    }
}