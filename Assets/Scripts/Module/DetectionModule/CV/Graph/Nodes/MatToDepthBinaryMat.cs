using System;
using System.Runtime.InteropServices;
using UnityEngine;
using OpenCVForUnity;

namespace CellBig.Module.Detection.CV
{
    public class MatToDepthBinaryMatNode : CVNodeBase<Output.DepthBinaryMat, Mat, Mat>
    {
        private readonly byte[] _inputBuffer;
        private readonly byte[] _diffTargetBuffer;
        private readonly Mat _clearMat;
        private readonly Mat _filterMat;
        private readonly Mat _binaryMat;
        private readonly byte[] _depthBytes = new byte[1];

        public MatToDepthBinaryMatNode()
        {
            Vector2Int resolution = Settings.OutputResolution;
            _inputBuffer = new byte[resolution.x * resolution.y * 3];
            _diffTargetBuffer = new byte[resolution.x * resolution.y * 3];
            _clearMat = new Mat(resolution.y, resolution.x, CvType.CV_8UC1, new Scalar(0, 0, 0));
            _filterMat = new Mat(resolution.y, resolution.x, CvType.CV_8UC1);
            _binaryMat = new Mat(resolution.y, resolution.x, CvType.CV_8UC1);
        }

        ~MatToDepthBinaryMatNode()
        {
            _clearMat.Dispose();
            _filterMat.Dispose();
            _binaryMat.Dispose();
        }

        protected override Mat RunImpl(Mat input, int deltaInterval)
        {
            _clearMat.copyTo(_binaryMat);
            _clearMat.copyTo(_filterMat);
            
            Marshal.Copy((IntPtr)input.dataAddr(), _inputBuffer, 0, _inputBuffer.Length);
            if (Settings.DiffTarget != null)
                Marshal.Copy((IntPtr)Settings.DiffTarget.dataAddr(), _diffTargetBuffer, 0, _diffTargetBuffer.Length);

            ushort reliableMinDepth = 500;
            ushort reliableMaxDepth = 4500;
            ushort reliableDepthDiff = (ushort)(reliableMaxDepth - reliableMinDepth);
            Vector2Int resolution = Settings.OutputResolution;
            for (int y = 0; y < resolution.y; y++)
            {
                for (int x = 0; x < resolution.x; x++)
                {
                    int idx = y * resolution.x * 3 + x * 3;
                    ushort depth = (ushort)((1f - _inputBuffer[idx] / 255f) * reliableDepthDiff);

                    if (Settings.DiffTarget != null)
                    {
                        ushort diffDepth = (ushort)((1f - _diffTargetBuffer[idx] / 255f) * reliableDepthDiff);
                        if (depth > diffDepth)
                        {
                            depth = 0;
                        }
                        else
                        {
                            depth = (ushort)((diffDepth - depth));
                        }
                    }

                    if (depth >= Settings.MinDepth && depth <= Settings.MaxDepth)
                    {
                        _depthBytes[0] = 255;
                        _binaryMat.put(y, x, _depthBytes);
                    }
                    else
                    {
                        _depthBytes[0] = 255;
                        _filterMat.put(y, x, _depthBytes);
                    }
                }
            }

            for (int i = 0; i < Settings.FilterDilateIterations; i++)
                Imgproc.dilate(_filterMat, _filterMat, Settings.FilterDilateKernel);
            Core.subtract(_binaryMat, _filterMat, _binaryMat);

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
