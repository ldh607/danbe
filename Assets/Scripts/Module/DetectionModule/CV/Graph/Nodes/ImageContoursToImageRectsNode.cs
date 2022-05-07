using System.Collections.Generic;
using OpenCVForUnity;

namespace CellBig.Module.Detection.CV
{
    public class ImageContoursToImageRectsNode : CVNodeBase<Output.ImageRects, List<MatOfPoint>, List<OpenCVForUnity.Rect>>
    {
        private readonly List<OpenCVForUnity.Rect> _rects = new List<OpenCVForUnity.Rect>();

        public ImageContoursToImageRectsNode()
        {

        }

        protected override List<OpenCVForUnity.Rect> RunImpl(List<MatOfPoint> input, int deltaInterval)
        {
            _rects.Clear();

            foreach (var contour in input)
            {
                var rect = Imgproc.boundingRect(contour);
                _rects.Add(rect);
            }

            return _rects;
        }

        protected override void Copy(List<Rect> from, ref List<Rect> to)
        {
            if (to == null)
                to = new List<Rect>(from);            
            else
                to.AddRange(from);
        }

        protected override void Clear(List<Rect> output)
        {
            output.Clear();
        }
    }
}
