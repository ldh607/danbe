using System;
using System.Collections.Generic;
using OpenCVForUnity;

namespace CellBig.Module.Detection.CV
{
    public class ImageContoursToImageQuadsNode : CVNodeBase<Output.ImageQuads, List<MatOfPoint>, List<Point>>
    {
        private readonly float[] _point = new float[2];
        private readonly List<Point> _points = new List<Point>();
        private MatOfPoint2f _contourMat = new MatOfPoint2f();
        private MatOfPoint2f _approxMat = new MatOfPoint2f();

        public ImageContoursToImageQuadsNode()
        {
        
        }

        ~ImageContoursToImageQuadsNode()
        {

        }

        protected override List<Point> RunImpl(List<MatOfPoint> input, int deltaInterval)
        {
            _approxMat.create(0, 0, CvType.CV_32F);
            _points.Clear();

            foreach (var contour in input)
            {
                contour.convertTo(_contourMat, CvType.CV_32F); // MatOfPoint -> MatOfPoint2f
                double arcLen = Imgproc.arcLength(_contourMat, true);
                Imgproc.approxPolyDP(_contourMat, _approxMat, arcLen * Settings.QuadEpsilonMultiplier, true);

                if (_approxMat.total() == 4)
                {
                    Point center = new Point(0, 0);
                    for (int i = 0; i < 4; i++)
                    {
                        _approxMat.get(i, 0, _point);
                        center.x += _point[0];
                        center.y += _point[1];
                    }

                    center /= 4;
                    _points.Add(center);
                }
            }

            return _points;
        }

        protected override void Copy(List<Point> from, ref List<Point> to)
        {
            if (to == null)
                to = new List<Point>(from);
            else
                to.AddRange(from);
        }

        protected override void Clear(List<Point> output)
        {
            output.Clear();
        }
    }
}
