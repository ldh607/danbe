using System;
using System.Collections.Generic;
using UnityEngine;
using OpenCVForUnity;

namespace CellBig.Module.Detection.CV
{
    public class ImageContoursToImageCirclesNode : CVNodeBase<Output.ImageCircles, List<MatOfPoint>, List<Point>>
    {
        private readonly List<Point> _points = new List<Point>();
        private readonly int[] _point = new int[2];
        private readonly System.Random _random = new System.Random();

        public ImageContoursToImageCirclesNode()
        {
        
        }

        ~ImageContoursToImageCirclesNode()
        {

        }

        protected override List<Point> RunImpl(List<MatOfPoint> input, int deltaInterval)
        {
            _points.Clear();

            foreach (var contour in input)
            {
                var rect = Imgproc.boundingRect(contour);
                Point center = (rect.tl() + rect.br()) * 0.5;
                contour.get(0, 0, _point);
                float radius = Vector2.Distance(new Vector2((float)center.x, (float)center.y), new Vector2(_point[0], _point[1]));

                int samplingCnt = (int)(contour.total() * Settings.CircleSamplingRate);
                int validCnt = 0;
                for (int i = 0; i < samplingCnt; i++)
                {
                    int row = _random.Next(0, contour.rows());
                    int col = _random.Next(0, contour.cols());
                    contour.get(row, col, _point);
                    
                    float dist = Vector2.Distance(new Vector2((float)center.x, (float)center.y), new Vector2(_point[0], _point[1]));
                    if (Mathf.Abs(dist - radius) <= Settings.CircleErrorThreshold)
                    {
                        validCnt++;
                    }
                }

                float validRate = (float)validCnt / samplingCnt;
                if (validRate >= Settings.CircleValidRate)
                {
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
