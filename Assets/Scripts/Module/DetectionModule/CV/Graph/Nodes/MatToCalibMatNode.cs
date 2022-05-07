using UnityEngine;
using OpenCVForUnity;

namespace CellBig.Module.Detection.CV
{
    public class MatToCalibMatNode : CVNodeBase<Output.CalibMat, Mat, Mat>
    {
        private readonly Mat _calibMat;

        public MatToCalibMatNode()
        {
            Vector2Int resolution = Settings.OutputResolution;
            _calibMat = new Mat(resolution.y, resolution.x, CvType.CV_8UC3);
        }

        ~MatToCalibMatNode()
        {
            _calibMat.Dispose();
        }
        
        protected override Mat RunImpl(Mat input, int deltaInterval)
        {
            bool calib = false;
            if (!string.IsNullOrEmpty(Settings.CameraMatrixPath) && !string.IsNullOrEmpty(Settings.DistortionPath))
            {
                calib = true;
                input.copyTo(_calibMat);
                Imgproc.undistort(input, _calibMat, Settings.CameraMatrix, Settings.DistortionMatrix);
            }
            return calib ? _calibMat : input;
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
