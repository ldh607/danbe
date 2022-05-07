using System.Collections.Generic;
using UnityEngine;
using OpenCVForUnity;

namespace CellBig.Module.Detection.CV
{
    public class ImageQuadsToViewportQuadsNode : CVNodeBase<Output.ViewportQuads, List<Point>, List<Vector2>>
    {
        private readonly List<Vector2> _points = new List<Vector2>();
        private MatOfPoint2f _pointMat = new MatOfPoint2f(new Point());
        private float[] _point = new float[2];

        public ImageQuadsToViewportQuadsNode()
        {

        }

        protected override List<Vector2> RunImpl(List<Point> input, int deltaInterval)
        {
            _points.Clear();

            Vector2Int resolution = Settings.OutputResolution;
            Mat transform = Settings.InnerPerspectiveTransform * Settings.OuterPerspectiveTransform.inv();
            foreach (var p in input)
            {
                _point[0] = (float)p.x;
                _point[1] = (float)p.y;

                _pointMat.put(0, 0, _point);
                Core.perspectiveTransform(_pointMat, _pointMat, transform);
                _pointMat.get(0, 0, _point);

                Vector2 point = new Vector2()
                {
                    x = (float)_point[0] / resolution.x,
                    y = 1f - (float)_point[1] / resolution.y
                };

                _points.Add(point);
            }

            return _points;
        }

        protected override void Copy(List<Vector2> from, ref List<Vector2> to)
        {
            if (to == null)
                to = new List<Vector2>(from);            
            else
                to.AddRange(from);
        }

        protected override void Clear(List<Vector2> output)
        {
            output.Clear();
        }
    }
}
