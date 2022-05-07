using System.Collections.Generic;
using UnityEngine;

namespace CellBig.Module.Detection.CV
{
    public class RefreshWarpingRect : Message
    {
        public NormalizedRect? Outer { get; set; } = null;
        public NormalizedRect? Inner { get; set; } = null;
    }

    namespace Output
    {
        public class Mat : OutputMessageBase<OpenCVForUnity.Mat> { }
        public class CalibMat : OutputMessageBase<OpenCVForUnity.Mat> { }
        public class WarpMat : OutputMessageBase<OpenCVForUnity.Mat> { }
        public class DiffMat : OutputMessageBase<OpenCVForUnity.Mat> { }
        public class BinaryDiffMat : OutputMessageBase<OpenCVForUnity.Mat> { }
        public class BinaryMat : OutputMessageBase<OpenCVForUnity.Mat> { }
        public class DepthBinaryMat : OutputMessageBase<OpenCVForUnity.Mat> { }
        public class WaterplayMat : OutputMessageBase<OpenCVForUnity.Mat> { }
        public class ImageContours : OutputMessageBase<List<OpenCVForUnity.MatOfPoint>> { }
        public class ViewportContours : OutputMessageBase<List<List<Vector2>>> { }
        public class ImageRects : OutputMessageBase<List<OpenCVForUnity.Rect>> { }
        public class ViewportRects : OutputMessageBase<List<UnityEngine.Rect>> { }
        public class ImageCircles : OutputMessageBase<List<OpenCVForUnity.Point>> { }
        public class ViewportCircles : OutputMessageBase<List<Vector2>> { }
        public class ImageQuads : OutputMessageBase<List<OpenCVForUnity.Point>> { }
        public class ViewportQuads : OutputMessageBase<List<Vector2>> { }
    }
}