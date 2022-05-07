using System;
using UnityEngine;
using Windows.Kinect;
using OpenCVForUnity;

namespace CellBig.Module.VideoDevice
{
    public class Kinect : MonoBehaviour, IVideoDevice
    {
        private KinectSensor _sensor;
        private FrameDescription _depthDesc;
        private FrameDescription _colorDesc;
        private MultiSourceFrameReader _multiReader;
        private ColorFrameReader _colorReader;
        private DepthFrameReader _depthReader;
        private ushort[] _depthData;
        private byte[] _colorData;
        private byte[] _pixelData;
        private ColorSpacePoint[] _colorSpacePoints;
        private Mat _depthToColorMat;
        private Detection.CV.Output.CalibMat _rgbMessage;
        private VideoDeviceSettings _settings;
        private bool _rgbActive;

        private void Awake()
        {
            _sensor = KinectSensor.GetDefault();
            _rgbMessage = new Detection.CV.Output.CalibMat();
            Message.AddListener<SetActiveKinectRGB>(SetActiveRGB);

            this.enabled = false;
        }

        private void OnDestroy()
        {
            Message.RemoveListener<SetActiveKinectRGB>(SetActiveRGB);
            Disconnect();
        }
        
        private void SetActiveRGB(SetActiveKinectRGB msg)
        {
            _rgbActive = msg.Active;
        }

        private bool CheckDeviceConnected()
        {
            if (!IsConnected)
                Debug.LogError($"[{nameof(Kinect)}] 키넥트에 연결된 상태가 아닙니다.");
            return IsConnected;
        }

        private bool CheckDevicePlaying()
        {
            if (!IsPlaying)
                Debug.LogError($"[{nameof(Kinect)}] 키넥트가 재생 상태가 아닙니다.");
            return IsPlaying;
        }

        public void Connect(Vector2Int resolution, double fps)
        {
            bool connected = false;

            if (_sensor != null)
            {
                _settings = Model.First<VideoDeviceInfoModel>().Settings;

                _sensor.Open();
                _depthDesc = _sensor.DepthFrameSource.FrameDescription;
                _colorDesc = _sensor.ColorFrameSource.CreateFrameDescription(ColorImageFormat.Rgba);
                _depthData = new ushort[_depthDesc.LengthInPixels];
                _colorData = new byte[_colorDesc.LengthInPixels * _colorDesc.BytesPerPixel];
                _colorSpacePoints = new ColorSpacePoint[_depthDesc.LengthInPixels];
                _pixelData = new byte[_depthDesc.LengthInPixels * 3];
                _depthToColorMat = new Mat(_depthDesc.Height, _depthDesc.Width, CvType.CV_8UC3);
                _rgbMessage.Value = _depthToColorMat;
                //_multiReader = _sensor.OpenMultiSourceFrameReader(FrameSourceTypes.Color | FrameSourceTypes.Depth);
                //_multiReader.IsPaused = true;
                _colorReader = _sensor.ColorFrameSource.OpenReader();
                _colorReader.IsPaused = true;
                _depthReader = _sensor.DepthFrameSource.OpenReader();
                _depthReader.IsPaused = true;

                connected = true;
                this.enabled = true;
            }

            if (!connected)
            {
                Debug.LogError($"[{nameof(Kinect)}] 키넥트 연결에 실패했습니다.");
            }
        }
        
        public void Disconnect()
        {
            if (IsConnected)
            {
                _colorReader.Dispose();
                _depthReader.Dispose();
                //_multiReader.Dispose();
            }
            if (_sensor != null)
                _sensor.Close();
            this.enabled = false;
        }

        public void CopyBuffer(byte[] dst)
        {
            if (!CheckDevicePlaying())
                return;

            //MultiSourceFrame frame = _multiReader.AcquireLatestFrame();
            //if (frame != null)
            {
                DepthFrame depthFrame = _depthReader.AcquireLatestFrame();
                if (depthFrame != null)
                {
                    depthFrame.CopyFrameDataToArray(_depthData);

                    ushort minDepth = depthFrame.DepthMinReliableDistance;
                    ushort maxDepth = depthFrame.DepthMaxReliableDistance;
                    int idx = 0;
                    int len = Resolution.x * Resolution.y;
                    for (int i = len - 1; i >= 0; i--)
                    {
                        ushort depth = _depthData[i];
                        byte intensity = (byte)(depth >= minDepth && depth <= maxDepth ? 255 - ((float)depth / (maxDepth - minDepth)) * 255f : 0);

                        dst[idx++] = intensity;
                        dst[idx++] = intensity;
                        dst[idx++] = intensity;
                    }
                }

                if (_rgbActive)
                {
                    ColorFrame colorFrame = _colorReader.AcquireLatestFrame();
                    if (colorFrame != null)
                    {
                        colorFrame.CopyConvertedFrameDataToArray(_colorData, ColorImageFormat.Rgba);

                        _sensor.CoordinateMapper.MapDepthFrameToColorSpace(_depthData, _colorSpacePoints);
                        for (int i = 0; i < _colorSpacePoints.Length; i++)
                        {
                            int x = (int)Mathf.Floor(_colorSpacePoints[i].X + 0.5f);
                            int y = (int)Mathf.Floor(_colorSpacePoints[i].Y + 0.5f);
                            int colorIdx = ((_colorDesc.Width * y) + x) * (int)_colorDesc.BytesPerPixel;
                            int pixelIdx = i * 3;
                            if (x >= 0 && x < _colorDesc.Width && y >= 0 && y < _colorDesc.Height)
                            {
                                _pixelData[pixelIdx] = _colorData[colorIdx];
                                _pixelData[pixelIdx + 1] = _colorData[colorIdx + 1];
                                _pixelData[pixelIdx + 2] = _colorData[colorIdx + 2];
                            }
                            else
                            {
                                _pixelData[pixelIdx] = 0;
                                _pixelData[pixelIdx + 1] = 0;
                                _pixelData[pixelIdx + 2] = 0;
                            }
                        }
                        _depthToColorMat.put(0, 0, _pixelData);
                        Core.flip(_depthToColorMat, _depthToColorMat, 1);
                        if (_settings.FlipX && _settings.FlipY)
                            Core.flip(_depthToColorMat, _depthToColorMat, -1);
                        else if (_settings.FlipX)
                            Core.flip(_depthToColorMat, _depthToColorMat, 1);
                        else if (_settings.FlipY)
                            Core.flip(_depthToColorMat, _depthToColorMat, 0);
                        
                        Message.Send(_rgbMessage);
                    }

                    if (colorFrame != null)
                        colorFrame.Dispose();
                }

                if (depthFrame != null)
                    depthFrame.Dispose();
            }
        }

        public void Play()
        {
            if (!CheckDeviceConnected())
                return;

            _colorReader.IsPaused = false;
            _depthReader.IsPaused = false;
            //_multiReader.IsPaused = false;
        }

        public void Stop()
        {
            if (!CheckDeviceConnected())
                return;

            _colorReader.IsPaused = true;
            _depthReader.IsPaused = true;
            //_multiReader.IsPaused = true;
        }

        public Vector2Int Resolution
        {
            get
            {
                if (!CheckDeviceConnected())
                    return new Vector2Int();

                return new Vector2Int(_depthDesc.Width, _depthDesc.Height);
            }
        }

        public bool IsConnected => _sensor != null && _sensor.IsOpen;
        public bool IsPlaying => _depthReader != null && !_depthReader.IsPaused;

        public VideoCapability Capability => VideoCapability.None;

        public double Exposure
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public double AutoExposure
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public double Focus
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public double AutoFocus
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }
    }
}