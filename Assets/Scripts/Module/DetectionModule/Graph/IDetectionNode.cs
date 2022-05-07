
namespace CellBig.Module.Detection
{
    public interface IDetectionNode
    {
        DetectionNodeOutput<object> Run(object input, int deltaInterval);
        void SendOutput();
        void Reset();
    }
}