using System;
using System.Collections.Generic;
using UnityEngine;
using OpenCVForUnity;

namespace CellBig.Module.Detection.CV
{
    public class MatToImageCircles : CVNodeBase<Output.ImageCircles, Mat, List<OpenCVForUnity.Point>>
    {
        private readonly List<OpenCVForUnity.Point> _circles = new List<OpenCVForUnity.Point>();
        private readonly float[] _circleData = new float[3];
        private Mat _mat;
        private readonly Mat _circleMat = new Mat(0, 0, CvType.CV_32F);
        
        public MatToImageCircles()
        {
            Vector2Int imageResolution = Settings.OutputResolution;
            _mat = new Mat(imageResolution.y, imageResolution.x, CvType.CV_8UC1);
        }

        ~MatToImageCircles()
        {
            _mat.Dispose();
            _circleMat.Dispose();
        }

        protected override List<OpenCVForUnity.Point> RunImpl(Mat input, int deltaInterval)
        {
            _circleMat.create(0, 0, CvType.CV_32F);
            _circles.Clear();

            input.copyTo(_mat);
            
            //Imgproc.medianBlur(_mat, _mat, Settings.BlurSize);
            //Imgproc.HoughCircles(_mat, _circleMat, Imgproc.CV_HOUGH_GRADIENT, Settings.DP, Settings.MinDist, Settings.Param1, Settings.Param2, Settings.MinRadius, Settings.MaxRadius);

            for (int i = 0; i < _circleMat.cols(); i++)
            {
                _circleMat.get(0, i, _circleData);

                int x = (int)_circleData[0];
                int y = (int)_circleData[1];
                int rad = (int)_circleData[2];

                _circles.Add(new Point(x, y));
            }

            return _circles;
        }

        protected override void Copy(List<OpenCVForUnity.Point> from, ref List<OpenCVForUnity.Point> to)
        {
            if (to == null)
                to = new List<OpenCVForUnity.Point>(from);
            else
                to.AddRange(from);
        }

        protected override void Clear(List<OpenCVForUnity.Point> output)
        {
            output.Clear();
        }
    }
}
