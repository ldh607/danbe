using System;
using System.Threading;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using UnityEngine;
using OpenCVForUnity;

namespace CellBig.Module.Detection.CV
{
    public class MatToWaterplayMatNode : CVNodeBase<Output.WaterplayMat, Mat, Mat>
    {
        private struct ContourData
        {
            public long Addr;
            public int Total;
        }

        private Mat _curMat;
        private readonly Mat _prevMat;
        private readonly Mat _lineMat;
        private readonly Mat _coveredLineMat;
        private readonly Mat _blurredCoveredLineMat;
        private readonly List<ContourData> _contourData;
        private readonly List<MatOfPoint> _contours;
        private readonly Mat _hierarchy;
        private readonly Mat _maskMat;
        private readonly Mat _retMat;
        private readonly byte[] _linePixels;
        private readonly Mat _sharpKernel;

        private readonly Thread _lineThread;
        private readonly Thread _coveredLineThread;
        private readonly ManualResetEvent _processSync;
        private readonly CountdownEvent _lineSync;

        public MatToWaterplayMatNode()
        {
            Vector2Int resolution = Settings.OutputResolution;
            _curMat = new Mat(resolution.y, resolution.x, CvType.CV_8UC1);
            _prevMat = new Mat(resolution.y, resolution.x, CvType.CV_8UC1);
            _lineMat = new Mat(resolution.y, resolution.x, CvType.CV_8UC1);
            _coveredLineMat = new Mat(resolution.y, resolution.x, CvType.CV_8UC1);
            _blurredCoveredLineMat = new Mat(resolution.y, resolution.x, CvType.CV_8UC1);
            _retMat = new Mat(resolution.y, resolution.x, CvType.CV_8UC1);
            _maskMat = new Mat(resolution.y, resolution.x, CvType.CV_8UC1, Scalar.all(0));
            _linePixels = new byte[resolution.x * resolution.y];
            _sharpKernel = new Mat(3, 3, CvType.CV_32F);
            _sharpKernel.put(0, 0, new float[]
            {
                -1, 1, -1,
                1, 2, 1,
                -1, 1, -1
            });

            _contourData = new List<ContourData>();
            _contours = new List<MatOfPoint>();
            _hierarchy = new Mat();

            _lineThread = new Thread(ProcessLine);
            _coveredLineThread = new Thread(ProcessCoveredLine);

            _processSync = new ManualResetEvent(false);
            _lineSync = new CountdownEvent(2);

            _lineThread.Start();
            _coveredLineThread.Start();
        }

        ~MatToWaterplayMatNode()
        {
            _curMat.Dispose();
            _prevMat.Dispose();
            _lineMat.Dispose();
            _coveredLineMat.Dispose();
            _retMat.Dispose();
            _sharpKernel.Dispose();

            _hierarchy.Dispose();
            _maskMat.Dispose();

            _lineThread.Abort();
            _coveredLineThread.Abort();

            _processSync.Dispose();
            _lineSync.Dispose();
        }

        protected override Mat RunImpl(Mat input, int deltaInterval)
        {
            Imgproc.cvtColor(input, _curMat, Imgproc.COLOR_RGB2GRAY);
            Imgproc.cvtColor(Settings.DiffTarget, _prevMat, Imgproc.COLOR_RGB2GRAY);

            _lineSync.Reset();
            _processSync.Set();
            _processSync.Reset();
            _lineSync.Wait();

            //안가려진 선 + 가려진 선 병합
            Core.add(_lineMat, _coveredLineMat, _retMat);
            for (int i = 0; i < Settings.FinalErodeIterations; i++)
                Imgproc.erode(_retMat, _retMat, Imgproc.getStructuringElement(Imgproc.MORPH_ELLIPSE, new Size(Settings.FinalErodeSize, Settings.FinalErodeSize)));

            return _retMat;
        }

        protected override void Copy(Mat from, ref Mat to)
        {
            if (to == null)
                to = from.clone();
            else
                from.copyTo(to);
        }

        private void ProcessLine()
        {
            while (true)
            {
                _processSync.WaitOne();

                Core.subtract(_prevMat, _curMat, _lineMat);
                Core.multiply(_lineMat, Scalar.all(Settings.DiffMultiplier), _lineMat);
                for (int i = 0; i < Settings.DiffBlurIterations; i++)
                    Imgproc.blur(_lineMat, _lineMat, new Size(Settings.DiffBlurSize, Settings.DiffBlurSize));
                Imgproc.filter2D(_lineMat, _lineMat, -1, _sharpKernel);

                // 선 인식
                for (int i = 0; i < Settings.LineMedianBlurIterations; i++)
                    Imgproc.medianBlur(_lineMat, _lineMat, Settings.LineMedianBlurSize);
                Imgproc.threshold(_lineMat, _lineMat, Settings.LineThreshold, 255, Imgproc.THRESH_BINARY);
                for (int i = 0; i < Settings.LineMorphIterations; i++)
                {
                    if (Settings.LineDilateSize > 0)
                        Imgproc.dilate(_lineMat, _lineMat, Settings.LineDilateKernel);
                    if (Settings.LineErodeSize > 0)
                        Imgproc.erode(_lineMat, _lineMat, Settings.LineErodeKernel);
                }

                // blob 노이즈 제거
                _contours.Clear();
                Imgproc.findContours(_lineMat, _contours, _hierarchy, Imgproc.RETR_EXTERNAL, Imgproc.CHAIN_APPROX_SIMPLE);
                Marshal.Copy((IntPtr)_lineMat.dataAddr(), _linePixels, 0, _linePixels.Length);

                int contourCnt = _contours.Count;
                double[] contourPt = new double[2];
                double[] noisePixel = new double[1];
                for (int i = 0; i < contourCnt; i++)
                {
                    MatOfPoint contour = _contours[i];
                    if (Imgproc.contourArea(contour) > Settings.BlobNoiseContourSize)
                    {
                        _maskMat.setTo(Scalar.all(0));
                        Imgproc.drawContours(_maskMat, _contours, i, Scalar.all(255), -1);
                        Core.findNonZero(_maskMat, contour);

                        int total = 0;
                        unsafe
                        {
                            int* ptr = (int*)contour.dataAddr();
                            int num = (int)contour.total();
                            int w = _lineMat.width();
                            for (int j = 0; j < num; j++)
                            {
                                int index = j * 2; // x, y

                                // 이미지 좌표계 포인트 획득
                                contourPt[0] = *(ptr + index);
                                contourPt[1] = *(ptr + index + 1);

                                total += _linePixels[(int)(contourPt[1] * w + contourPt[0])];
                            }
                        }

                        if (total * 0.001d >= Settings.BlobNoiseAreaSize)
                        {
                            // dilate?
                            for (int k = 0; k < Settings.BlobNoiseDilateIterations; k++)
                            {
                                if (Settings.BlobNoiseDilateSize > 0)
                                {
                                    Mat kernel = Imgproc.getStructuringElement(Imgproc.MORPH_ELLIPSE, new Size(Settings.BlobNoiseDilateSize, Settings.BlobNoiseDilateSize));
                                    Imgproc.dilate(_maskMat, _maskMat, kernel);
                                }
                            }
                            Core.subtract(_lineMat, _maskMat, _lineMat);
                        }
                    }
                }

                _lineSync.Signal();
            }
        }

        private void ProcessCoveredLine()
        {
            while (true)
            {
                _processSync.WaitOne();

                Core.subtract(_curMat, _prevMat, _coveredLineMat);

                // 가려진 선 인식 (Threshold)
                Imgproc.GaussianBlur(_coveredLineMat, _blurredCoveredLineMat, new Size(Settings.CoveredLineGaussBlurSize, Settings.CoveredLineGaussBlurSize), 0);
                Core.subtract(_blurredCoveredLineMat, _coveredLineMat, _coveredLineMat);
                Imgproc.medianBlur(_coveredLineMat, _coveredLineMat, Settings.CoveredLineMedianBlurSize);
                Imgproc.threshold(_coveredLineMat, _coveredLineMat, Settings.CoveredLineThreshold, 255, Imgproc.THRESH_BINARY);
                for (int i = 0; i < Settings.CoveredLineMorphIterations; i++)
                {
                    Imgproc.dilate(_coveredLineMat, _coveredLineMat, Settings.CoveredLineDilateKernel);
                    Imgproc.erode(_coveredLineMat, _coveredLineMat, Settings.CoveredLineErodeKernel);
                }

                // 가려진 선 인식 (Edge)


                _lineSync.Signal();
            }
        }
    }
}
