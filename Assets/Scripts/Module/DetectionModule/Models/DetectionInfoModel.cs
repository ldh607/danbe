using UnityEngine;

namespace CellBig.Module.Detection
{
    public class DetectionInfoModel : Model
    {
        public DetectionInfoModel(DetectionSettings settings)
        {
            BaseSettings = settings.Base;
            CVSettings = settings.CV;
        }
        
        public BaseSettings BaseSettings { get; }
        public CV.CVSettings CVSettings { get; }
    }
}