using System;
using System.Collections.Generic;
using OpenCVForUnity;

namespace CellBig.Module.Detection.CV
{
    public class MatToImageContoursNode : CVNodeBase<Output.ImageContours, Mat, List<MatOfPoint>>
    {
        private readonly List<MatOfPoint> _allContours = new List<MatOfPoint>();
        private readonly List<MatOfPoint> _validContours = new List<MatOfPoint>();
        private readonly Mat _hierarchy = new Mat();

        public MatToImageContoursNode()
        {
            
        }

        ~MatToImageContoursNode()
        {
            _hierarchy.Dispose();
        }

        protected override List<MatOfPoint> RunImpl(Mat input, int deltaInterval)
        {
            _allContours.Clear();
            _validContours.Clear();

            Imgproc.findContours(input, _allContours, _hierarchy, (int)Settings.ContourRetrievalMode, (int)Settings.ContourApproxMethod);
            foreach (var points in _allContours)
            {
                // 사이즈 확인
                double area = Imgproc.contourArea(points);
                if (area >= Settings.MinContourSize && 
                    area <= Settings.MaxContourSize)
                {
                    _validContours.Add(points);
                }
            }
            return _validContours;
        }

        protected override void Copy(List<MatOfPoint> from, ref List<MatOfPoint> to)
        {
            if (to == null)
                to = new List<MatOfPoint>(from);
            else
                to.AddRange(from);
        }

        protected override void Clear(List<MatOfPoint> output)
        {
            output.Clear();
        }
    }
}
