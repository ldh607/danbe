using UnityEngine;
using OpenCVForUnity;

namespace CellBig.Module.Detection.CV
{
    public class BytesToMatNode : CVNodeBase<Output.Mat, byte[], Mat>
    {
        private readonly Mat _inputMat;
        private readonly Mat _outputMat;
        private readonly Size _size;

        public BytesToMatNode()
        {
            _inputMat = new Mat(Settings.InputResolution.y, Settings.InputResolution.x, CvType.CV_8UC3);
            _outputMat = new Mat(Settings.OutputResolution.y, Settings.OutputResolution.x, CvType.CV_8UC3);
            _size = _outputMat.size();
        }
            
        ~BytesToMatNode()
        {
            _inputMat.Dispose();
            _outputMat.Dispose();
        }
        
        protected override Mat RunImpl(byte[] input, int deltaInterval)
        {
            bool resized = false;
            
            _inputMat.put(0, 0, input);
            Core.flip(_inputMat, _inputMat, 0);
            if (Settings.InputResolution != Settings.OutputResolution)
            {
                Imgproc.resize(_inputMat, _outputMat, _size, 0, 0, Imgproc.INTER_AREA);
                resized = true;
            }

            return resized ? _outputMat : _inputMat;
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
