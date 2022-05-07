using System;
using System.IO;
using UnityEngine;
using OpenCVForUnity;
using CellBig.Common;
using CellBig.Models;

namespace CellBig.Module.Detection.CV
{
    public enum DiffMethod
    {
        AbsDiff,
        Subtract
    }

    public enum DiffOrder
    {
        SubtractFromCurrentFrame,
        SubtractFromPreviousFrame
    }

    public enum MorphologyMode
    {
        Open,   // Erode -> Dilate
        Close   // Dilate -> Erode
    }

    public enum ContourRetrievalMode
    {
        EXTERNAL    = 0,
        LIST        = 1,
        CCOMP       = 2,
        TREE        = 3,
        FLOODFILL   = 4
    }

    public enum ContourApproxMethod
    {
        NONE        = 1,
        SIMPLE      = 2,
        TC89_L1     = 3,
        TC89_KCOS   = 4
    }

    [Serializable]
    public partial class CVSettings : ISerializationCallbackReceiver
    {
        [SerializeField]
        private Vector2Int _outputResolution;

        [Space]

        [SerializeField]
        private string _cameraMatrixPath = "";
        [SerializeField]
        private string _distortionPath = "";

        [Space]

        [SerializeField]
        private NormalizedRect _outerWarpingRect = new NormalizedRect()
        {
            LeftTop = new NormalizedPoint(0, 0),
            RightTop = new NormalizedPoint(1, 0),
            RightBottom = new NormalizedPoint(1, 1),
            LeftBottom = new NormalizedPoint(0, 1)
        };
        [SerializeField]
        private NormalizedRect _innerWarpingRect = new NormalizedRect()
        {
            LeftTop = new NormalizedPoint(0, 0),
            RightTop = new NormalizedPoint(1, 0),
            RightBottom = new NormalizedPoint(1, 1),
            LeftBottom = new NormalizedPoint(0, 1)
        };

        [Space]

        [SerializeField]
        private bool _useDiff = true;
        [SerializeField]
        private DiffMethod _diffMethod = DiffMethod.AbsDiff;
        [SerializeField]
        private DiffOrder _diffOrder = DiffOrder.SubtractFromCurrentFrame;
        [SerializeField]
        private int _diffInterval = 0;
        [SerializeField]
        private string _diffTargetPath;
        [SerializeField]
        private float _diffTargetAutoSaveDelay = 5f;
        [SerializeField]
        private bool _diffTargetAutoSave;

        [Space]

        [SerializeField]
        private MorphologyMode _morphologyMode = MorphologyMode.Open;
        [SerializeField]
        private int _erodeSize;
        [SerializeField]
        private int _erodeIterations;
        [SerializeField]
        private int _dilateSize;
        [SerializeField]
        private int _dilateIterations;
        [SerializeField]
        private int _threshold = 10;
        [SerializeField]
        private int _maxVal = 255;

        [Space]

        [SerializeField]
        private ContourRetrievalMode _contourRetrievalMode = ContourRetrievalMode.EXTERNAL;
        [SerializeField]
        private ContourApproxMethod _contourApproxMode = ContourApproxMethod.SIMPLE;
        [SerializeField]
        private double _minContourSize = 100;
        [SerializeField]
        private double _maxContourSize = 10000;

        [Space]

        [SerializeField]
        private float _circleSamplingRate = 0.3f;
        [SerializeField]
        private float _circleValidRate = 0.9f;
        [SerializeField]
        private float _circleErrorThreshold = 15f;

        [Space]

        [SerializeField]
        private double _quadEpsilonMultiplier = 0.05;

        [Space]

        // Kinect
        [SerializeField]
        private ushort _minDepth = 100;
        [SerializeField]
        private ushort _maxDepth = 3000;
        [SerializeField]
        private int _filterDilateSize;
        [SerializeField]
        private int _filterDilateIterations;

        [Space]

        // Waterplay
        [SerializeField]
        private double _diffMultiplier = 1;         // 차이값 픽셀 배수
        [SerializeField]
        private int _diffBlurSize;                  // 차이값 블러 크기
        [SerializeField]
        private int _diffBlurItertaions;            // 차이값 블러 횟수
        [SerializeField]
        private int _lineMedianBlurSize;            // 안가려진 선 노이즈 블러 크기
        [SerializeField]
        private int _lineMedianBlurIterations;      // 안가려진 선 노이즈 블러 횟수
        [SerializeField]
        private int _lineThreshold;                 // 안가려진 선 이진화 픽셀 임계값
        [SerializeField]
        private int _lineErodeSize;                 // 안가려진 선 노이즈 제거 크기
        [SerializeField]
        private int _lineDilateSize;                // 안가려진 선 확장 크기
        [SerializeField]
        private int _lineMorphIterations;           // 안가려진 선 Dilate/Erode 횟수
        [SerializeField]
        private int _sharpFactor = 2;               // 선명화 커널 계수
        [SerializeField]
        private int _blobNoiseContourSize;          // 재질 노이즈로 인식할 외곽선 크기
        [SerializeField]
        private int _blobNoiseAreaSize;             // 재질 노이즈로 인식할 영역 크기
        [SerializeField]
        private int _blobNoiseDilateSize;           // 재질 노이즈 마스크 확장 크기
        [SerializeField]
        private int _blobNoiseDilateIterations;     // 재질 노이즈 마스크 확장 횟수
        [SerializeField]
        private int _coveredLineGaussBlurSize;      // 가려진 선 엣지 따기 전 블러 크기
        [SerializeField]
        private int _coveredLinePreErodeSize;       // 가려진 선 엣지 따기 전 노이즈 제거 크기
        [SerializeField]
        private int _coveredLineMedianBlurSize;     // 가려진 선 엣지 노이즈 블러 크기
        [SerializeField]
        private int _coveredLineThreshold;          // 가려진 선 엣지 이진화 픽셀 임계값
        [SerializeField]
        private int _coveredLineErodeSize;          // 가려진 선 엣지 노이즈 제거 크기
        [SerializeField]
        private int _coveredLineDilateSize;         // 가려진 선 엣지 확장 크기
        [SerializeField]
        private int _coveredLineMorphIterations;    // 가려진 선 엣지 Dilate/Erode 횟수
        [SerializeField]
        private int _finalErodeSize = 1;            // 최종 픽셀 축소 크기
        [SerializeField]
        private int _finalErodeIterations;          // 최종 픽셀 축소 횟수

        private bool _setup = false;

        private Vector2Int _inputResolution;
        private Mat _cameraMatrix;
        private Mat _distortionMatrix;
        private Mat _erodeKernel;
        private Mat _dilateKernel;
        private Mat _outerPerspectiveTransform;
        private Mat _innerPerspectiveTransform;
        private Mat _diffTarget;
        private bool _diffTargetChanged;
        private readonly FileSystemWatcher _diffTargetWatcher = new FileSystemWatcher();
        private Mat _filterDilateKernel;
        private Mat _lineErodeKernel;
        private Mat _lineDilateKernel;
        private Mat _coveredLineErodeKernel;
        private Mat _coveredLineDilateKernel;

        public void Setup(Vector2Int inputResolution)
        {
            _setup = true;

            Message.AddListener<RefreshWarpingRect>(OnWarpingRectChanged);

            InputResolution = inputResolution;
            if (OutputResolution == Vector2Int.zero)
                OutputResolution = inputResolution;
            CameraMatrix = GetCameraMatrix();
            DistortionMatrix = GetDistortionMatrix();
            DiffTargetPath = _diffTargetPath;
            OuterPerspectiveTransform = GetPerspectiveTransform(OuterWarpingRect, OutputResolution);
            InnerPerspectiveTransform = GetPerspectiveTransform(InnerWarpingRect, OutputResolution);
            ErodeSize = _erodeSize;
            DilateSize = _dilateSize;
            FilterDilateSize = _filterDilateSize;
            LineErodeSize = _lineErodeSize;
            LineDilateSize = _lineDilateSize;
            CoveredLineErodeSize = _coveredLineErodeSize;
            CoveredLineDilateSize = _coveredLineDilateSize;
        }

        public void Unsetup()
        {
            if (!_setup)
                return;

            _setup = false;

            Message.RemoveListener<RefreshWarpingRect>(OnWarpingRectChanged);

            //_diffTargetWatcher.Created -= OnDiffTargetChanged;
            //_diffTargetWatcher.Changed -= OnDiffTargetChanged;
        }

        public void Update()
        {
            if (_diffTargetChanged)
            {
                DiffTarget = LoadDiffTarget();
                _diffTargetChanged = false;
            }
        }

        private void OnDiffTargetChanged(object sender, FileSystemEventArgs args)
        {
            _diffTargetChanged = true;
        }

        private void OnWarpingRectChanged(RefreshWarpingRect rect)
        {
            if (rect.Outer != null)
            {
                _outerWarpingRect = rect.Outer.Value;
                OuterPerspectiveTransform = GetPerspectiveTransform(OuterWarpingRect, OutputResolution);
            }
            if (rect.Inner != null)
            {
                _innerWarpingRect = rect.Inner.Value;
                InnerPerspectiveTransform = GetPerspectiveTransform(InnerWarpingRect, OutputResolution);
            }
        }

        private Mat GetCameraMatrix()
        {
            Mat m = new Mat(3, 3, CvType.CV_64F);
            m.put(0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 1); // 단위 행렬

            string path = Application.streamingAssetsPath + '/' + _cameraMatrixPath;
            if (!string.IsNullOrEmpty(_cameraMatrixPath))
            {
                if (!File.Exists(path))
                {
                    string msg = $"'{path}' 경로에 CameraMatrix 파일이 없습니다.";
                    Message.Send(new DetectionEvent(DetectionEvent.EventType.FileNotFound, path, msg));
                    Debug.LogError($"[{nameof(CVSettings)}] {msg}");
                    return m;
                }
                else
                {
                    double[] data = new double[9];
                    string[] lines = File.ReadAllLines(path);
                    if (lines.Length != 3)
                    {
                        string msg = "CameraMatrix 파일의 행의 수가 3이 아닙니다.";
                        Message.Send(new DetectionEvent(DetectionEvent.EventType.FileDataMismatch, path, msg));
                        Debug.LogError($"[{nameof(CVSettings)}] {msg}");
                        return m;
                    }

                    for (int i = 0; i < lines.Length; i++)
                    {
                        string line = lines[i];
                        string[] values = line.Split(new string[] { "  " }, StringSplitOptions.None);
                        if (values.Length != 3)
                        {
                            string msg = $"CameraMatrix 파일 {i}번째 행의 열의 수가 3이 아닙니다.";
                            Message.Send(new DetectionEvent(DetectionEvent.EventType.FileDataMismatch, path, msg));
                            Debug.LogError($"[{nameof(CVSettings)}] {msg}");
                            return m;
                        }

                        for (int j = 0; j < 3; j++)
                        {
                            bool ret = double.TryParse(values[j], out double result);
                            if (!ret)
                            {
                                string msg = $"CameraMatrix 파일 ({i}, {j}) 원소의 값을 double로 변환할 수 없습니다.";
                                Message.Send(new DetectionEvent(DetectionEvent.EventType.FileDataMismatch, path, msg));
                                Debug.LogError($"[{nameof(CVSettings)}] {msg}");
                                return m;
                            }
                            data[i * 3 + j] = result;
                        }
                    }

                    m.put(0, 0, data);
                    return m;
                }
            }

            return m;
        }

        private Mat GetDistortionMatrix()
        {
            Mat m = new Mat(4, 1, CvType.CV_64F);
            m.put(0, 0, 0, 0, 0, 0);

            string path = Application.streamingAssetsPath + '/' + _distortionPath;
            if (!string.IsNullOrEmpty(_distortionPath))
            {
                if (!File.Exists(path))
                {
                    string msg = $"'{path}' 경로에 Distortion 파일이 없습니다.";
                    Message.Send(new DetectionEvent(DetectionEvent.EventType.FileNotFound, path, msg));
                    Debug.LogError($"[{nameof(CVSettings)}] {msg}");
                    return m;
                }
                else
                {
                    double[] data = new double[4];
                    string[] lines = File.ReadAllLines(path);
                    if (lines.Length != 1)
                    {
                        string msg = "Distortion 파일의 행의 수가 1이 아닙니다.";
                        Message.Send(new DetectionEvent(DetectionEvent.EventType.FileDataMismatch, path, msg));
                        Debug.LogError($"[{nameof(CVSettings)}] {msg}");
                        return m;
                    }

                    string line = lines[0];
                    string[] values = line.Split(' ');
                    if (values.Length != 4)
                    {
                        string msg = "Distortion 파일의 열의 수가 4가 아닙니다.";
                        Message.Send(new DetectionEvent(DetectionEvent.EventType.FileDataMismatch, path, msg));
                        Debug.LogError($"[{nameof(CVSettings)}] {msg}");
                        return m;
                    }

                    for (int j = 0; j < 4; j++)
                    {
                        bool ret = double.TryParse(values[j], out double result);
                        if (!ret)
                        {
                            string msg = $"Distortion 파일 {j}번째 원소의 값을 double로 변환할 수 없습니다.";
                            Message.Send(new DetectionEvent(DetectionEvent.EventType.FileDataMismatch, path, msg));
                            Debug.LogError($"[{nameof(CVSettings)}] {msg}");
                            return m;
                        }
                        data[j] = result;
                    }
                    m.put(0, 0, data);
                    return m;
                }
            }

            return m;
        }

        private Mat GetKernel(int size)
        {
            return Imgproc.getStructuringElement(Imgproc.MORPH_ELLIPSE, new Size(size, size));
        }

        private Mat GetPerspectiveTransform(NormalizedRect rect, Vector2Int resolution)
        {
            var lt = new NormalizedPoint(rect.LeftTop.X * resolution.x, rect.LeftTop.Y * resolution.y);
            var rt = new NormalizedPoint(rect.RightTop.X * resolution.x, rect.RightTop.Y * resolution.y);
            var rb = new NormalizedPoint(rect.RightBottom.X * resolution.x, rect.RightBottom.Y * resolution.y);
            var lb = new NormalizedPoint(rect.LeftBottom.X * resolution.x, rect.LeftBottom.Y * resolution.y);

            Mat tfSrcMat = new Mat(4, 1, CvType.CV_32FC2);
            Mat tfDstMat = new Mat(4, 1, CvType.CV_32FC2);
            tfSrcMat.put(0, 0, lt.X, lt.Y, rt.X, rt.Y, rb.X, rb.Y, lb.X, lb.Y);
            tfDstMat.put(0, 0, 0.0f, 0.0f, OutputResolution.x, 0.0f, OutputResolution.x, OutputResolution.y, 0.0f, OutputResolution.y);
            return Imgproc.getPerspectiveTransform(tfSrcMat, tfDstMat);
        }

        private Mat LoadDiffTarget()
        {
            if (string.IsNullOrEmpty(DiffTargetPath))
                return null;

            string path = Application.streamingAssetsPath + '/' + DiffTargetPath;
            if (!File.Exists(path))
            {
                string msg = $"'{path}' 경로에 DiffTarget 이미지가 없습니다.";
                Message.Send(new DetectionEvent(DetectionEvent.EventType.FileNotFound, path, msg));
                Debug.LogError($"[{nameof(CVSettings)}] {msg}");
                return null;
            }

            byte[] bytes = File.ReadAllBytes(path);
            Texture2D tex = new Texture2D(OutputResolution.x, OutputResolution.y, TextureFormat.RGB24, false);
            tex.LoadImage(bytes);

            if (tex.width != OutputResolution.x || tex.height != OutputResolution.y)
            {
                string msg = "DiffTaget 파일과 비디오 장치의 해상도가 다릅니다.";
                Message.Send(new DetectionEvent(DetectionEvent.EventType.FileDataMismatch, path, msg));
                Debug.LogError($"[{nameof(CVSettings)}] {msg}");
                return null;
            }

            Mat diffMat = new Mat(OutputResolution.y, OutputResolution.x, CvType.CV_8UC3);
            Utils.texture2DToMat(tex, diffMat, true);
            return diffMat;
        }

        public void OnBeforeSerialize()
        {

        }

        public void OnAfterDeserialize()
        {
            if (!_setup)
                return;

            OuterPerspectiveTransform = GetPerspectiveTransform(OuterWarpingRect, OutputResolution);
            InnerPerspectiveTransform = GetPerspectiveTransform(InnerWarpingRect, OutputResolution);
            ErodeSize = _erodeSize;
            DilateSize = _dilateSize;
            FilterDilateSize = _filterDilateSize;
            LineErodeSize = _lineErodeSize;
            LineDilateSize = _lineDilateSize;
            CoveredLineErodeSize = _coveredLineErodeSize;
            CoveredLineDilateSize = _coveredLineDilateSize;
        }

        public Vector2Int InputResolution
        {
            get => _inputResolution;
            private set => _inputResolution = value;
        }

        public Vector2Int OutputResolution
        {
            get
            {
                return _outputResolution;
            }

            set
            {
                _outputResolution = value;
                LoadDiffTarget();
                OuterPerspectiveTransform = GetPerspectiveTransform(OuterWarpingRect, value);
                InnerPerspectiveTransform = GetPerspectiveTransform(InnerWarpingRect, value);
            }
        }

        public string CameraMatrixPath
        {
            get
            {
                return _cameraMatrixPath;
            }

            set
            {
                _cameraMatrixPath = value;
                CameraMatrix = GetCameraMatrix();
            }
        }

        public string DistortionPath
        {
            get
            {
                return _distortionPath;
            }

            set
            {
                _distortionPath = value;
                DistortionMatrix = GetDistortionMatrix();
            }
        }

        public Mat CameraMatrix
        {
            get => _cameraMatrix;
            private set => _cameraMatrix = value;
        }

        public Mat DistortionMatrix
        {
            get => _distortionMatrix;
            private set => _distortionMatrix = value;
        }

        public NormalizedRect OuterWarpingRect
        {
            get
            {
                return _outerWarpingRect;
            }

            set
            {
                _outerWarpingRect = value;
                OuterPerspectiveTransform = GetPerspectiveTransform(value, OutputResolution);
                Message.Send(new RefreshWarpingRect() { Outer = value });
            }
        }

        public NormalizedRect InnerWarpingRect
        {
            get
            {
                return _innerWarpingRect;
            }

            set
            {
                _innerWarpingRect = value;
                InnerPerspectiveTransform = GetPerspectiveTransform(value, OutputResolution);
                Message.Send(new RefreshWarpingRect() { Inner = value });
            }
        }

        public Mat OuterPerspectiveTransform
        {
            get => _outerPerspectiveTransform;
            private set => _outerPerspectiveTransform = value;
        }

        public Mat InnerPerspectiveTransform
        {
            get => _innerPerspectiveTransform;
            private set => _innerPerspectiveTransform = value;
        }

        public bool UseDiff
        {
            get => _useDiff;
            set => _useDiff = value;
        }

        public DiffOrder DiffOrder
        {
            get => _diffOrder;
            set => _diffOrder = value;
        }

        public DiffMethod DiffMethod
        {
            get => _diffMethod;
            set => _diffMethod = value;
        }

        public int DiffInterval
        {
            get => _diffInterval;
            set => _diffInterval = value;
        }

        public string DiffTargetPath
        {
            get
            {
                return _diffTargetPath;
            }

            set
            {
                _diffTargetPath = value;
                if (string.IsNullOrEmpty(value))
                {
                    _diffTargetWatcher.EnableRaisingEvents = false;
                }
                else
                {
                    string path = Application.streamingAssetsPath + '/' + value;
                    _diffTargetWatcher.Path = Application.streamingAssetsPath + '/' + value.Substring(0, value.LastIndexOf('/'));
                    _diffTargetWatcher.Filter = value.Substring(value.LastIndexOf('/') + 1);
                    _diffTargetWatcher.EnableRaisingEvents = true;
                }

                DiffTarget = LoadDiffTarget();
            }
        }

        public float DiffTargetAutoSaveDelay
        {
            get => _diffTargetAutoSaveDelay;
            set => _diffTargetAutoSaveDelay = value;
        }

        public bool DiffTargetAutoSave
        {
            get => _diffTargetAutoSave;
            set => _diffTargetAutoSave = value;
        }

        public Mat DiffTarget
        {
            get => _diffTarget;
            private set => _diffTarget = value;
        }

        public MorphologyMode MorphologyMode
        {
            get => _morphologyMode;
            set => _morphologyMode = value;
        }

        public int ErodeSize
        {
            get
            {
                return _erodeSize;
            }

            set
            {
                _erodeSize = Mathf.Max(1, value);
                ErodeKernel = GetKernel(_erodeSize);
            }
        }

        public int DilateSize
        {
            get
            {
                return _dilateSize;
            }

            set
            {
                _dilateSize = Mathf.Max(1, value);
                DilateKernel = GetKernel(_dilateSize);
            }
        }

        public Mat ErodeKernel
        {
            get => _erodeKernel;
            private set => _erodeKernel = value;
        }

        public Mat DilateKernel
        {
            get => _dilateKernel;
            private set => _dilateKernel = value;
        }

        public int ErodeIterations
        {
            get => _erodeIterations;
            set => _erodeIterations = value;
        }

        public int DilateIterations
        {
            get => _dilateIterations;
            set => _dilateIterations = value;
        }

        public int Threshold
        {
            get => _threshold;
            set => _threshold = value;
        }

        public int MaxVal
        {
            get => _maxVal;
            set => _maxVal = value;
        }

        public ContourRetrievalMode ContourRetrievalMode
        {
            get => _contourRetrievalMode;
            set => _contourRetrievalMode = value;
        }

        public ContourApproxMethod ContourApproxMethod
        {
            get => _contourApproxMode;
            set => _contourApproxMode = value;
        }

        public double MinContourSize
        {
            get => _minContourSize;
            set => _minContourSize = value;
        }

        public double MaxContourSize
        {
            get => _maxContourSize;
            set => _maxContourSize = value;
        }

        public float CircleSamplingRate
        {
            get => _circleSamplingRate;
            set => _circleSamplingRate = value;
        }

        public float CircleValidRate
        {
            get => _circleValidRate;
            set => _circleValidRate = value;
        }

        public float CircleErrorThreshold
        {
            get => _circleErrorThreshold;
            set => _circleErrorThreshold = value;
        }

        public double QuadEpsilonMultiplier
        {
            get => _quadEpsilonMultiplier;
            set => _quadEpsilonMultiplier = value;
        }

        public ushort MinDepth
        {
            get => _minDepth;
            set => _minDepth = value;
        }

        public ushort MaxDepth
        {
            get => _maxDepth;
            set => _maxDepth = value;
        }

        public int FilterDilateSize
        {
            get
            {
                return _filterDilateSize;
            }

            set
            {
                _filterDilateSize = Mathf.Max(1, value);
                FilterDilateKernel = GetKernel(_filterDilateSize);
            }
        }

        public int FilterDilateIterations
        {
            get => _filterDilateIterations;
            set => _filterDilateIterations = value;
        }

        public Mat FilterDilateKernel
        {
            get => _filterDilateKernel;
            private set => _filterDilateKernel = value;
        }

        public double DiffMultiplier
        {
            get => _diffMultiplier;
            set => _diffMultiplier = value;
        }

        public int DiffBlurSize
        {
            get => _diffBlurSize;
            set => _diffBlurSize = value;
        }

        public int DiffBlurIterations
        {
            get => _diffBlurItertaions;
            set => _diffBlurItertaions = value;
        }

        public int LineMedianBlurSize
        {
            get => _lineMedianBlurSize;
            set => _lineMedianBlurSize = value;
        }

        public int LineMedianBlurIterations
        {
            get => _lineMedianBlurIterations;
            set => _lineMedianBlurIterations = value;
        }

        public int LineThreshold
        {
            get => _lineThreshold;
            set => _lineThreshold = value;
        }

        public int LineErodeSize
        {
            get
            {
                return _lineErodeSize;
            }

            set
            {
                _lineErodeSize = Mathf.Max(1, value);
                LineErodeKernel = GetKernel(_lineErodeSize);
            }
        }

        public int LineDilateSize
        {
            get
            {
                return _lineDilateSize;
            }

            set
            {
                _lineDilateSize = Mathf.Max(1, value);
                LineDilateKernel = GetKernel(_lineDilateSize);
            }
        }

        public Mat LineErodeKernel
        {
            get => _lineErodeKernel;
            private set => _lineErodeKernel = value;
        }

        public Mat LineDilateKernel
        {
            get => _lineDilateKernel;
            private set => _lineDilateKernel = value;
        }

        public int LineMorphIterations
        {
            get => _lineMorphIterations;
            set => _lineMorphIterations = value;
        }

        public int SharpFactor
        {
            get => _sharpFactor;
            set => _sharpFactor = value;
        }

        public int BlobNoiseContourSize
        {
            get => _blobNoiseContourSize;
            set => _blobNoiseContourSize = value;
        }

        public int BlobNoiseAreaSize
        {
            get => _blobNoiseAreaSize;
            set => _blobNoiseAreaSize = value;
        }

        public int BlobNoiseDilateSize
        {
            get => _blobNoiseDilateSize;
            set => _blobNoiseDilateSize = value;
        }

        public int BlobNoiseDilateIterations
        {
            get => _blobNoiseDilateIterations;
            set => _blobNoiseDilateIterations = value;
        }

        public int CoveredLineGaussBlurSize
        {
            get => _coveredLineGaussBlurSize;
            set => _coveredLineGaussBlurSize = value;
        }

        public int CoveredLinePreErodeSize
        {
            get => _coveredLinePreErodeSize;
            set => _coveredLinePreErodeSize = value;
        }

        public int CoveredLineMedianBlurSize
        {
            get => _coveredLineMedianBlurSize;
            set => _coveredLineMedianBlurSize = value;
        }

        public int CoveredLineThreshold
        {
            get => _coveredLineThreshold;
            set => _coveredLineThreshold = value;
        }

        public int CoveredLineErodeSize
        {
            get
            {
                return _coveredLineErodeSize;
            }

            set
            {
                _coveredLineErodeSize = Mathf.Max(1, value);
                CoveredLineErodeKernel = GetKernel(_coveredLineErodeSize);
            }
        }

        public int CoveredLineDilateSize
        {
            get
            {
                return _coveredLineDilateSize;
            }

            set
            {
                _coveredLineDilateSize = Mathf.Max(1, value);
                CoveredLineDilateKernel = GetKernel(_coveredLineDilateSize);
            }
        }

        public Mat CoveredLineErodeKernel
        {
            get => _coveredLineErodeKernel;
            private set => _coveredLineErodeKernel = value;
        }

        public Mat CoveredLineDilateKernel
        {
            get => _coveredLineDilateKernel;
            private set => _coveredLineDilateKernel = value;
        }

        public int CoveredLineMorphIterations
        {
            get => _coveredLineMorphIterations;
            set => _coveredLineMorphIterations = value;
        }

        public int FinalErodeSize
        {
            get => _finalErodeSize;
            set => _finalErodeSize = value;
        }

        public int FinalErodeIterations
        {
            get => _finalErodeIterations;
            set => _finalErodeIterations = value;
        }
    }
}