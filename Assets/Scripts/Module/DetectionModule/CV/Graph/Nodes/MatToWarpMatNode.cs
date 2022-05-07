using UnityEngine;
using OpenCVForUnity;

namespace CellBig.Module.Detection.CV
{
    public class MatToWarpMatNode : CVNodeBase<Output.WarpMat, Mat, Mat>
    {
        private readonly Mat _warpMat;
        private readonly Size _tfSize;

        public MatToWarpMatNode()
        {
            Vector2Int warpingResolution = Settings.OutputResolution;
            _warpMat = new Mat(warpingResolution.y, warpingResolution.x, CvType.CV_8UC3);
            _tfSize = new Size(warpingResolution.x, warpingResolution.y);
        }

        ~MatToWarpMatNode()
        {
            _warpMat.Dispose();
        }
        
        protected override Mat RunImpl(Mat input, int deltaInterval)
        {
            Imgproc.warpPerspective(input, _warpMat, Settings.OuterPerspectiveTransform, _tfSize);
            return _warpMat;
        }

        protected override void Copy(Mat from, ref Mat to)
        {
            if (to == null)
                to = from.clone();
            else
                from.copyTo(to);
        }
    }
}
