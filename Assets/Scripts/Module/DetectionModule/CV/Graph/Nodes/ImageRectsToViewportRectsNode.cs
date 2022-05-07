using System.Collections.Generic;
using UnityEngine;
using OpenCVForUnity;

namespace CellBig.Module.Detection.CV
{
    public class ImageRectsToViewportRects : CVNodeBase<Output.ViewportRects, List<OpenCVForUnity.Rect>, List<UnityEngine.Rect>>
    {
        private readonly List<UnityEngine.Rect> _rects = new List<UnityEngine.Rect>();
        private MatOfPoint2f _minMat = new MatOfPoint2f(new Point());
        private MatOfPoint2f _maxMat = new MatOfPoint2f(new Point());
        private float[] _minPoint = new float[2];
        private float[] _maxPoint = new float[2];

        public ImageRectsToViewportRects()
        {

        }

        protected override List<UnityEngine.Rect> RunImpl(List<OpenCVForUnity.Rect> input, int deltaInterval)
        {
            _rects.Clear();

            Vector2Int resolution = Settings.OutputResolution;
            Mat transform = Settings.InnerPerspectiveTransform * Settings.OuterPerspectiveTransform.inv();
            foreach (var imageRect in input)
            {
                _minPoint[0] = imageRect.x;
                _minPoint[1] = imageRect.y;
                _maxPoint[0] = imageRect.x + imageRect.width;
                _maxPoint[1] = imageRect.y + imageRect.height;

                _minMat.put(0, 0, _minPoint);
                _maxMat.put(0, 0, _maxPoint);
                Core.perspectiveTransform(_minMat, _minMat, transform);
                Core.perspectiveTransform(_maxMat, _maxMat, transform);
                _minMat.get(0, 0, _minPoint);
                _maxMat.get(0, 0, _maxPoint);

                UnityEngine.Rect viewportRect = new UnityEngine.Rect();
                viewportRect.x = (float)(_minPoint[0] / resolution.x);
                viewportRect.y = 1f - (float)(_maxPoint[1] / resolution.y);
                viewportRect.xMax = (float)(_maxPoint[0] / resolution.x);
                viewportRect.yMax = 1f - (float)(_minPoint[1] / resolution.y);
                _rects.Add(viewportRect);
            }

            return _rects;
        }

        protected override void Copy(List<UnityEngine.Rect> from, ref List<UnityEngine.Rect> to)
        {
            if (to == null)
                to = new List<UnityEngine.Rect>(from);
            else
                to.AddRange(from);
        }

        protected override void Clear(List<UnityEngine.Rect> output)
        {
            output.Clear();
        }
    }
}
