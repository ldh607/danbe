using System.Collections.Generic;
using UnityEngine;
using OpenCVForUnity;

namespace CellBig.Module.Detection.CV
{
    public class ImageContoursToViewportContoursNode : CVNodeBase<Output.ViewportContours, List<MatOfPoint>, List<List<Vector2>>>
    {
        private readonly List<List<Vector2>> _contours = new List<List<Vector2>>();
        private readonly List<List<Vector2>> _contourPool = new List<List<Vector2>>();
        private MatOfPoint2f _pointMat = new MatOfPoint2f(new Point());
        private float[] _point = new float[2];

        public ImageContoursToViewportContoursNode()
        {

        }

        protected override List<List<Vector2>> RunImpl(List<MatOfPoint> input, int deltaInterval)
        {
            //foreach (var contour in _contours)
            //    contour.Clear();
            //_contourPool.AddRange(_contours);
            _contours.Clear();

            // Conversion MatOfPoint to Array of Vector2
            // MatOfPoint.toArray/toList를 통한 Garbage 생성을 막기위해 직접 변환한다.
            Vector2Int resolution = Settings.OutputResolution;
            Mat transform = Settings.InnerPerspectiveTransform * Settings.OuterPerspectiveTransform.inv();
            for (int i = 0; i < input.Count; i++)
            {
                MatOfPoint points = input[i];
                List<Vector2> contour = null;
                if (_contourPool.Count > 0)
                {
                    contour = _contourPool[0];
                    _contourPool.RemoveAt(0);
                }
                else
                {
                    contour = new List<Vector2>();
                }

                unsafe
                {
                    int* ptr = (int*)points.dataAddr();
                    int num = (int)points.total();
                    for (int j = 0; j < num; j++)
                    {
                        int index = j * 2; // x, y

                        // 이미지 좌표계 포인트 획득
                        _point[0] = *(ptr + index);
                        _point[1] = *(ptr + index + 1);

                        _pointMat.put(0, 0, _point);
                        Core.perspectiveTransform(_pointMat, _pointMat, Settings.OuterPerspectiveTransform.inv());
                        Core.perspectiveTransform(_pointMat, _pointMat, Settings.InnerPerspectiveTransform);
                        _pointMat.get(0, 0, _point);

                        // 뷰포트 좌표계로 변환
                        Vector2 point = new Vector2()
                        {
                            x = (float)_point[0] / resolution.x,
                            y = 1f - (float)_point[1] / resolution.y
                        };

                        contour.Add(point);
                    }
                }

                _contours.Add(contour);
            }

            return _contours;
        }

        protected override void Copy(List<List<Vector2>> from, ref List<List<Vector2>> to)
        {
            if (to == null)
                to = new List<List<Vector2>>(from);
            else
                to.AddRange(from);
        }

        protected override void Clear(List<List<Vector2>> output)
        {
            //foreach (var contour in output)
            //    contour.Clear();
            output.Clear();
        }
    }
}
