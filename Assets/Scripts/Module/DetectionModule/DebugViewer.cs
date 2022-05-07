using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using OpenCVForUnity;
using CellBig.Module.VideoDevice;

namespace CellBig.Module.Detection
{
    public class DebugViewer : MonoBehaviour
    {
        [SerializeField]
        private RawImage _image;

        private Texture2D _texture;
        private Mat _originalMat;
        private Mat _processingMat;
        private KeyCode _prevKey;

        private void Start()
        {
            _image.enabled = false;

            DetectionInfoModel info = Model.First<DetectionInfoModel>();
            Vector2Int resolution = info.CVSettings.OutputResolution;
            _originalMat = new Mat(resolution.y, resolution.x, CvType.CV_8UC3, new Scalar(0, 0, 0));
            _processingMat = new Mat(resolution.y, resolution.x, CvType.CV_8UC3);
            _texture = new Texture2D(resolution.x, resolution.y, TextureFormat.RGB24, false);
            _image.texture = _texture;
        }

        private void Update()
        {
            KeyCode keyCode = KeyCode.None;

            if (Input.GetKeyDown(KeyCode.F1)) // Warp Mat
            {
                if (_prevKey != KeyCode.F1)
                {
                    SetupCallback<CV.Output.WarpMat>(WarpMat);
                }

                keyCode = KeyCode.F1;
            }
            else if (Input.GetKeyDown(KeyCode.F2)) // Diff Mat
            {
                if (_prevKey != KeyCode.F2)
                {
                    SetupCallback<CV.Output.DiffMat>(DiffMat);
                }

                keyCode = KeyCode.F2;
            }
            else if (Input.GetKeyDown(KeyCode.F3)) // Binary Mat
            {
                if (_prevKey != KeyCode.F3)
                {
                    SetupCallback<CV.Output.WaterplayMat>(BinaryMat);
                }

                keyCode = KeyCode.F3;
            }
            else if (Input.GetKeyDown(KeyCode.F4)) // Contours
            {
                if (_prevKey != KeyCode.F4)
                {
                    SetupCallback<CV.Output.ImageContours>(Contours);
                }

                keyCode = KeyCode.F4;
            }
            else if (Input.GetKeyDown(KeyCode.F5)) // Rects
            {
                if (_prevKey != KeyCode.F5)
                {
                    SetupCallback<CV.Output.ImageRects>(Rects);
                }

                keyCode = KeyCode.F5;
            }
            else if (Input.GetKeyDown(KeyCode.F6)) // Circles
            {
                if (_prevKey != KeyCode.F6)
                {
                    SetupCallback<CV.Output.ImageCircles>(Circles);
                }

                keyCode = KeyCode.F6;
            }
            else if (Input.GetKeyDown(KeyCode.F7)) // Quads
            {
                if (_prevKey != KeyCode.F7)
                {
                    SetupCallback<CV.Output.ImageQuads>(Quads);
                }

                keyCode = KeyCode.F7;
            }

            if (keyCode != KeyCode.None)
            {
                if (_image.enabled && _prevKey == keyCode)
                {
                    UnsetupCallbacks();

                    _prevKey = KeyCode.None;
                    _image.enabled = false;
                }
                else
                {
                    _prevKey = keyCode;
                    _image.enabled = true;
                }
            }
        }

        private void SetupCallback<T>(Action<T> callback) where T : Message
        {
            UnsetupCallbacks();
            Message.AddListener(callback);
        }

        private void UnsetupCallbacks()
        {
            Message.RemoveListener<CV.Output.WarpMat>(WarpMat);
            Message.RemoveListener<CV.Output.DiffMat>(DiffMat);
            Message.RemoveListener<CV.Output.WaterplayMat>(BinaryMat);
            Message.RemoveListener<CV.Output.ImageContours>(Contours);
            Message.RemoveListener<CV.Output.ImageRects>(Rects);
            Message.RemoveListener<CV.Output.ImageCircles>(Circles);
            Message.RemoveListener<CV.Output.ImageQuads>(Quads);
        }

        private void WarpMat(CV.Output.WarpMat output)
        {
            output.Value.copyTo(_processingMat);
            Utils.matToTexture2D(_processingMat, _texture);
        }

        private void DiffMat(CV.Output.DiffMat output)
        {
            output.Value.copyTo(_processingMat);
            Utils.matToTexture2D(_processingMat, _texture);
        }

        private void BinaryMat(CV.Output.WaterplayMat output)
        {
            Imgproc.cvtColor(output.Value, _processingMat, Imgproc.COLOR_GRAY2RGB);
            Utils.matToTexture2D(_processingMat, _texture);
        }

        private void Contours(CV.Output.ImageContours output)
        {
            _originalMat.copyTo(_processingMat);

            Scalar color = new Scalar(255, 0, 0);
            for (int i = 0; i < output.Value.Count; i++)
                Imgproc.drawContours(_processingMat, output.Value, i, color, 3);
            Utils.matToTexture2D(_processingMat, _texture);
        }

        private void Rects(CV.Output.ImageRects output)
        {
            _originalMat.copyTo(_processingMat);

            Scalar color = new Scalar(255, 0, 0);
            foreach (var rect in output.Value)
                Imgproc.rectangle(_processingMat, rect, color, 3);
            Utils.matToTexture2D(_processingMat, _texture);
        }

        private void Circles(CV.Output.ImageCircles output)
        {
            _originalMat.copyTo(_processingMat);

            Scalar color = new Scalar(255, 0, 0);
            foreach (var point in output.Value)
                Imgproc.circle(_processingMat, point, 10, color);
            Utils.matToTexture2D(_processingMat, _texture);
        }

        private void Quads(CV.Output.ImageQuads output)
        {
            _originalMat.copyTo(_processingMat);

            Scalar color = new Scalar(255, 0, 0);
            foreach (var point in output.Value)
                Imgproc.circle(_processingMat, point, 10, color);
            Utils.matToTexture2D(_processingMat, _texture);
        }
    }
}
