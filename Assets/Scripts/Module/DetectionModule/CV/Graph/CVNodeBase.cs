using CellBig.Models;

namespace CellBig.Module.Detection.CV
{
    public abstract class CVNodeBase<TMessage, TInput, TOutput> : DetectionNodeBase<TMessage, TInput, TOutput>
        where TMessage : OutputMessageBase<TOutput>, new()
    {
        public CVNodeBase()
        {
            Settings = Model.First<DetectionInfoModel>().CVSettings;
        }

        public CVSettings Settings { get; }
    }
}