using System;
using System.Runtime.InteropServices;
using UnityEngine;
using OpenCVForUnity;

namespace CellBig.Module.VideoDevice
{
    public class WaterplayTest : MonoBehaviour, IVideoDevice
    {
        private Mat _mat;
        private Point _previousPoint;
        private int _eraserRadius = 10;

        public void Connect(Vector2Int resolution, double fps)
        {
            IsConnected = true;
            Resolution = resolution;
            _mat = new Mat(Resolution.y, Resolution.x, CvType.CV_8UC3, new Scalar(0, 0, 0));
        }

        public void Disconnect()
        {
            Stop();
            IsConnected = false;
        }

        public void Play()
        {
            IsPlaying = true;
        }

        public void Stop()
        {
            IsPlaying = false;
        }

        public void CopyBuffer(byte[] dst)
        {
            Marshal.Copy((IntPtr)_mat.dataAddr(), dst, 0, dst.Length);
        }

        private void Update()
        {
            double x = Input.mousePosition.x * (Resolution.x / (double)Screen.width);
            double y = Input.mousePosition.y * (Resolution.y / (double)Screen.height);

            if (Input.GetMouseButtonDown(0))
            {
                _previousPoint = new Point(x, y);
            }
            else if (Input.GetMouseButton(0))
            {
                Point currentPoint = new Point(x, y);
                Imgproc.line(_mat, _previousPoint, currentPoint, new Scalar(255, 255, 255), 20);
                _previousPoint = currentPoint;
            }
            else if (Input.GetMouseButton(1))
            {
                Point currentPoint = new Point(x, y);
                Imgproc.circle(_mat, currentPoint, _eraserRadius, Scalar.all(0), -1);
            }

            if (Input.GetKeyDown(KeyCode.Plus) || Input.GetKeyDown(KeyCode.KeypadPlus))
            {
                _eraserRadius += 10;
            }
            else if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus))
            {
                _eraserRadius -= 10;
                if (_eraserRadius < 10)
                    _eraserRadius = 10;
            }

            if (Input.GetKeyDown(KeyCode.Delete))
            {
                _mat.setTo(Scalar.all(0));
            }
        }

        public bool IsConnected { get; private set; }
        public bool IsPlaying { get; private set; }
        public Vector2Int Resolution { get; private set; }

        public VideoCapability Capability => VideoCapability.None;
        public double Exposure { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public double AutoExposure { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public double Focus { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public double AutoFocus { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    }
}
