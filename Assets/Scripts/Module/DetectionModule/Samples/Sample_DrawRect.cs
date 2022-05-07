using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OpenCVForUnity;
using CellBig.Module.VideoDevice;

namespace CellBig.Module.Detection.Samples
{
    public class Sample_DrawRect : MonoBehaviour
    {
        [SerializeField]
        private RawImage _image;

        private Mat _originalMat;
        private Mat _processingMat;
        private Texture2D _texture;

        private void Start()
        {
            // 모델에서 해상도값 얻기
            VideoDeviceInfoModel vdInfo = Model.First<VideoDeviceInfoModel>();
            Vector2Int resolution = vdInfo.Resolution;

            // 메모리 초기화
            _originalMat = new Mat(resolution.y, resolution.x, CvType.CV_8UC3, new Scalar(0, 0, 0)); // 검정색으로 초기화
            _processingMat = new Mat(resolution.y, resolution.x, CvType.CV_8UC3);
            _texture = new Texture2D(resolution.x, resolution.y, TextureFormat.RGB24, false);
            _image.texture = _texture;

            // 이미지 좌표계 Rect를 얻어오기위한 메시지 콜백 등록
            Message.AddListener<CV.Output.ImageRects>(ProcessImageRects);
        }

        private void OnDestroy()
        {
            Message.RemoveListener<CV.Output.ImageRects>(ProcessImageRects);
            _originalMat?.Dispose();
            _processingMat?.Dispose();
        }
        
        private void ProcessImageRects(CV.Output.ImageRects output)
        {
            // 화면에 출력될 이미지 행렬을 검정색으로 초기화
            _originalMat.copyTo(_processingMat);

            // Rect 그리기
            List<OpenCVForUnity.Rect> rects = output.Value;
            Scalar color = new Scalar(255, 0, 0);
            foreach (var rect in rects)
            {
                Imgproc.rectangle(_processingMat, rect, color, 3);
            }

            // 이미지 행렬을 유니티 텍스처로 변환
            Utils.matToTexture2D(_processingMat, _texture);
        }
    }
}