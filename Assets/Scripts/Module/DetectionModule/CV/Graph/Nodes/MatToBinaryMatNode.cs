using UnityEngine;
using OpenCVForUnity;

namespace CellBig.Module.Detection.CV
{
    public class MatToBinaryMatNode : CVNodeBase<Output.BinaryMat, Mat, Mat>
    {
        private readonly Mat _binaryMat;

        public MatToBinaryMatNode()
        {
            Vector2Int imageResolution = Settings.OutputResolution;
            _binaryMat = new Mat(imageResolution.y, imageResolution.x, CvType.CV_8UC1);
        }

        ~MatToBinaryMatNode()
        {
            _binaryMat.Dispose();
        }

        protected override Mat RunImpl(Mat input, int deltaInterval)
        {
            Imgproc.cvtColor(input, _binaryMat, Imgproc.COLOR_RGB2GRAY);

            Imgproc.threshold(_binaryMat, _binaryMat, Settings.Threshold, Settings.MaxVal, Imgproc.THRESH_BINARY);
            if (Settings.MorphologyMode == MorphologyMode.Open)
            {
                for (int i = 0; i < Settings.ErodeIterations; i++)
                    Imgproc.erode(_binaryMat, _binaryMat, Settings.ErodeKernel);
                for (int i = 0; i < Settings.DilateIterations; i++)
                    Imgproc.dilate(_binaryMat, _binaryMat, Settings.DilateKernel);
            }
            else if (Settings.MorphologyMode == MorphologyMode.Close)
            {
                for (int i = 0; i < Settings.DilateIterations; i++)
                    Imgproc.dilate(_binaryMat, _binaryMat, Settings.DilateKernel);
                for (int i = 0; i < Settings.ErodeIterations; i++)
                    Imgproc.erode(_binaryMat, _binaryMat, Settings.ErodeKernel);
            }

            return _binaryMat;
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
