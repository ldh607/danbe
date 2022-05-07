using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using OpenCVForUnity;
using CellBig.Module.VideoDevice;

namespace CellBig.Module.Detection
{
    public class ProcessViewer : MonoBehaviour
    {
        [SerializeField]
        private RawImage _image;

        private Texture2D _texture;
        private Mat _originalMat;
        private Mat _processingMat;

        public Transform OptionRoot;
        Toggle WarpToggle;
        Toggle DiffToggle;
        Toggle BinaryToggle;
        Toggle KinectDiffToggle;
        Toggle KinectBinaryToggle;
        Toggle ContoursToggle;
        Toggle RectsToggle;
        Toggle CirclesToggle;
        Toggle QuardsToggle;

        private void Start()
        {
            DetectionInfoModel info = Model.First<DetectionInfoModel>();
            Vector2Int resolution = info.CVSettings.OutputResolution;
            _originalMat = new Mat(resolution.y, resolution.x, CvType.CV_8UC3, new Scalar(0, 0, 0));
            _processingMat = new Mat(resolution.y, resolution.x, CvType.CV_8UC3);
            _texture = new Texture2D(resolution.x, resolution.y, TextureFormat.RGB24, false);
            _image.texture = _texture;

            WarpToggle = OptionRoot.Find("Warp").GetComponent<Toggle>();
            WarpToggle.onValueChanged.AddListener(ChangeToggle);
            DiffToggle= OptionRoot.Find("WebCam").Find("Diff").GetComponent<Toggle>();
            DiffToggle.onValueChanged.AddListener(ChangeToggle);
            BinaryToggle = OptionRoot.Find("WebCam").Find("Binary").GetComponent<Toggle>();
            BinaryToggle.onValueChanged.AddListener(ChangeToggle);
            ContoursToggle = OptionRoot.Find("Contours").GetComponent<Toggle>();
            ContoursToggle.onValueChanged.AddListener(ChangeToggle);
            RectsToggle = OptionRoot.Find("Rects").GetComponent<Toggle>();
            RectsToggle.onValueChanged.AddListener(ChangeToggle);
            CirclesToggle = OptionRoot.Find("Circles").Find("Circles").GetComponent<Toggle>();
            CirclesToggle.onValueChanged.AddListener(ChangeToggle);
            QuardsToggle = OptionRoot.Find("Circles").Find("Quads").GetComponent<Toggle>();
            QuardsToggle.onValueChanged.AddListener(ChangeToggle);
            KinectDiffToggle = OptionRoot.Find("Kinect").Find("KinectDiff").GetComponent<Toggle>();
            KinectDiffToggle.onValueChanged.AddListener(ChangeToggle);
            KinectBinaryToggle = OptionRoot.Find("Kinect").Find("KinectBinary").GetComponent<Toggle>();
            KinectBinaryToggle.onValueChanged.AddListener(ChangeToggle);
            WarpToggle.isOn = true;
            DiffToggle.isOn = false;
            BinaryToggle.isOn = false;
            ContoursToggle.isOn = false;
            RectsToggle.isOn = false;
            CirclesToggle.isOn = false;
            QuardsToggle.isOn = false;
            KinectDiffToggle.isOn = false;
            KinectBinaryToggle.isOn = false;

            SetupCallback<CV.Output.WarpMat>(WarpMat);
        }

        private void ChangeToggle(bool isActive )
        {
            if (!isActive)
                return;

            if (WarpToggle.isOn)
                SetupCallback<CV.Output.WarpMat>(WarpMat);
            else if (DiffToggle.isOn)
                SetupCallback<CV.Output.DiffMat>(DiffMat);
            else if (BinaryToggle.isOn)
                SetupCallback<CV.Output.BinaryMat>(BinaryMat);
            else if (ContoursToggle.isOn)
                SetupCallback<CV.Output.ImageContours>(Contours);
            else if (RectsToggle.isOn)
                SetupCallback<CV.Output.ImageRects>(Rects);
            else if (CirclesToggle.isOn)
                SetupCallback<CV.Output.ImageCircles>(Circles);
            else if (QuardsToggle.isOn)
                SetupCallback<CV.Output.ImageQuads>(Quads);
            else if (KinectDiffToggle.isOn) 
                SetupCallback<CV.Output.BinaryDiffMat>(KinectDiff);
            else if (KinectBinaryToggle.isOn)
                SetupCallback<CV.Output.DepthBinaryMat>(KinectBinary);
        }

        private void SetupCallback<T>(Action<T> callback) where T : Message
        {
            UnsetupCallbacks();
            Message.AddListener(callback);
        }

        public void ResetToggle(bool value)
        {
            WarpToggle.isOn = true;
            DiffToggle.isOn = false;
            BinaryToggle.isOn = false;
            ContoursToggle.isOn = false;
            RectsToggle.isOn = false;
            CirclesToggle.isOn = false;
            QuardsToggle.isOn = false;
            KinectDiffToggle.isOn = false;
            KinectBinaryToggle.isOn = false;
        }

        private void UnsetupCallbacks()
        {
            Message.RemoveListener<CV.Output.WarpMat>(WarpMat);
            Message.RemoveListener<CV.Output.DiffMat>(DiffMat);
            Message.RemoveListener<CV.Output.BinaryMat>(BinaryMat);
            Message.RemoveListener<CV.Output.ImageContours>(Contours);
            Message.RemoveListener<CV.Output.ImageRects>(Rects);
            Message.RemoveListener<CV.Output.ImageCircles>(Circles);
            Message.RemoveListener<CV.Output.ImageQuads>(Quads);
            Message.RemoveListener<CV.Output.BinaryDiffMat>(KinectDiff);
            Message.RemoveListener<CV.Output.DepthBinaryMat>(KinectBinary);
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

        private void BinaryMat(CV.Output.BinaryMat output)
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

        private void KinectDiff(CV.Output.BinaryDiffMat output)
        {
            output.Value.copyTo(_processingMat);
            Utils.matToTexture2D(_processingMat, _texture);
        }

        private void KinectBinary(CV.Output.DepthBinaryMat output)
        {
            output.Value.copyTo(_processingMat);
            Utils.matToTexture2D(_processingMat, _texture);
        }
    }
}
