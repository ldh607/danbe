using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using OpenCVForUnity;
using CellBig.Models;
using CellBig.Module.VideoDevice;

namespace CellBig.Module.Detection
{
    public class ImageTool : MonoBehaviour
    {
        private enum RectMode
        {
            Outer,
            Inner
        }

        [SerializeField]
        private RawImage _image;
        [SerializeField, Range(0, 100)]
        private int _saveQuality = 100;
        [SerializeField]
        private string _savePath;
        [SerializeField]
        private KeyCode _saveKey = KeyCode.LeftShift;
        [SerializeField]
        private KeyCode _activeKey = KeyCode.Tab;

        private RectMode _mode = RectMode.Outer;
        private Mat _calibMat;
        private Mat _warpMat;
        private Texture2D _texture;
        private int _currentPointIdx;
        private NormalizedPoint[] _outerPoints = new NormalizedPoint[4];
        private NormalizedPoint[] _innerPoints = new NormalizedPoint[4];
        private bool _enabled = false;

        private IEnumerator Start()
        {
            _image.enabled = false;

            DetectionInfoModel info = Model.First<DetectionInfoModel>();
            Resolution = info.CVSettings.OutputResolution;
            _calibMat = new Mat(Resolution.y, Resolution.x, CvType.CV_8UC3);
            _warpMat = new Mat(Resolution.y, Resolution.x, CvType.CV_8UC1);
            _texture = new Texture2D(Resolution.x, Resolution.y, TextureFormat.RGB24, false);
            Image.texture = _texture;

            CV.CVSettings cv = info.CVSettings;
            OnWarpingRectChanged(new CV.RefreshWarpingRect() { Outer = cv.OuterWarpingRect, Inner = cv.InnerWarpingRect });
            Message.AddListener<CV.RefreshWarpingRect>(OnWarpingRectChanged);
            Message.AddListener<CV.Output.WarpMat>(ProcessWarpMat);

            if (cv.DiffTargetAutoSave)
            {
                yield return new WaitForSeconds(cv.DiffTargetAutoSaveDelay);
                Save();
            }
        }

        private void OnDestroy()
        {
            Message.RemoveListener<CV.RefreshWarpingRect>(OnWarpingRectChanged);
            Message.RemoveListener<CV.Output.WarpMat>(ProcessWarpMat);
        }

        private void Update()
        {
            CheckActive();
            CheckSave();

            if (Enabled)
            {
                CheckPointChange();
                CheckPointSelection();
                CheckPointTranslation();
            }
        }

        private void ProcessCalibMat(CV.Output.CalibMat mat)
        {
            mat.Value.copyTo(_calibMat);
            ProcessImage();
        }

        private void ProcessWarpMat(CV.Output.WarpMat mat)
        {
            mat.Value.copyTo(_warpMat);
        }

        private void OnWarpingRectChanged(CV.RefreshWarpingRect rect)
        {
            if (rect.Outer != null)
            {
                NormalizedRect outerRect = rect.Outer.Value;
                _outerPoints[0] = outerRect.LeftTop;
                _outerPoints[1] = outerRect.RightTop;
                _outerPoints[2] = outerRect.RightBottom;
                _outerPoints[3] = outerRect.LeftBottom;
            }

            if (rect.Inner != null)
            {
                NormalizedRect innerRect = rect.Inner.Value;
                _innerPoints[0] = innerRect.LeftTop;
                _innerPoints[1] = innerRect.RightTop;
                _innerPoints[2] = innerRect.RightBottom;
                _innerPoints[3] = innerRect.LeftBottom;
            }
        }

        private NormalizedRect SortNormalizedRect(NormalizedPoint[] points)
        {
            NormalizedRect rect = new NormalizedRect();
            NormalizedPoint[] sortedPoints = points.Clone() as NormalizedPoint[];
            Array.Sort(sortedPoints, (p0, p1) => p0.Y < p1.Y ? -1 : 1); // Y값 오름차순 정렬
            if (sortedPoints[0].X < sortedPoints[1].X)
            {
                rect.LeftTop = sortedPoints[0];
                rect.RightTop = sortedPoints[1];
            }
            else
            {
                rect.LeftTop = sortedPoints[1];
                rect.RightTop = sortedPoints[0];
            }
            if (sortedPoints[2].X < sortedPoints[3].X)
            {
                rect.RightBottom = sortedPoints[3];
                rect.LeftBottom = sortedPoints[2];
            }
            else
            {
                rect.RightBottom = sortedPoints[2];
                rect.LeftBottom = sortedPoints[3];
            }

            return rect;
        }

        private void CheckActive()
        {
            if (Input.GetKeyDown(_activeKey))
            {
                if (!Enabled)
                {
                    Message.Send(new SetActiveKinectRGB(true));
                    Enabled = true;
                }
                else
                {
                    if (_mode == RectMode.Outer)
                        _mode = RectMode.Inner;
                    else if (_mode == RectMode.Inner)
                    {
                        Message.Send(new SetActiveKinectRGB(false));
                        Enabled = false;
                    }
                }
            }
        }

        private void CheckPointChange()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                _currentPointIdx = 0;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                _currentPointIdx = 1;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                _currentPointIdx = 2;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                _currentPointIdx = 3;
            }
        }

        private void CheckPointSelection()
        {
            if (Input.GetMouseButtonDown(0))
            {
                UnityEngine.Rect rect = GetUIRect(Image.rectTransform);
                Vector2 mousePos = Input.mousePosition;
                mousePos *= rect.size / new Vector2(Screen.width, Screen.height);
                mousePos -= rect.position;
                mousePos.y = rect.height - mousePos.y;
                mousePos /= rect.size;

                NormalizedPoint warpPoint = new NormalizedPoint(mousePos.x, mousePos.y);
                NormalizedRect warpingRect = new NormalizedRect();
                if (_mode == RectMode.Outer)
                {
                    _outerPoints[_currentPointIdx] = warpPoint;
                    warpingRect = SortNormalizedRect(_outerPoints);
                }
                else if (_mode == RectMode.Inner)
                {
                    _innerPoints[_currentPointIdx] = warpPoint;
                    warpingRect = SortNormalizedRect(_innerPoints);
                }

                Message.RemoveListener<CV.RefreshWarpingRect>(OnWarpingRectChanged);
                Message.Send(_mode == RectMode.Outer ? new CV.RefreshWarpingRect() { Outer = warpingRect } : new CV.RefreshWarpingRect() { Inner = warpingRect });
                Message.AddListener<CV.RefreshWarpingRect>(OnWarpingRectChanged);
            }
        }

        private void CheckPointTranslation()
        {
            int xDir = 0;
            int yDir = 0;
            if (Input.GetKey(KeyCode.A)) xDir--;
            if (Input.GetKey(KeyCode.D)) xDir++;
            if (Input.GetKey(KeyCode.S)) yDir++;
            if (Input.GetKey(KeyCode.W)) yDir--;
            float multiflier = 1f;
            if (Input.GetKey(KeyCode.G)) { multiflier = 20f; }

            if (xDir != 0 || yDir != 0)
            {
                NormalizedPoint[] points = _mode == RectMode.Outer ? _outerPoints : _innerPoints;
                NormalizedPoint p = points[_currentPointIdx];
                p.X += xDir * Time.deltaTime * 0.001f * multiflier;
                //p.X = p.X < 0 ? 0 : (p.X > 1 ? 1 : p.X);
                p.Y += yDir * Time.deltaTime * 0.001f * multiflier;
                //p.Y = p.Y < 0 ? 0 : (p.Y > 1 ? 1 : p.Y);

                points[_currentPointIdx] = p;

                NormalizedRect warpingRect = new NormalizedRect();
                if (_mode == RectMode.Outer)
                    warpingRect = SortNormalizedRect(points);
                else if (_mode == RectMode.Inner)
                    warpingRect = SortNormalizedRect(points);

                Message.RemoveListener<CV.RefreshWarpingRect>(OnWarpingRectChanged);
                Message.Send(_mode == RectMode.Outer ? new CV.RefreshWarpingRect() { Outer = warpingRect } : new CV.RefreshWarpingRect() { Inner = warpingRect });
                Message.AddListener<CV.RefreshWarpingRect>(OnWarpingRectChanged);
            }
        }

        private void CheckSave()
        {
            if (Input.GetKey(KeyCode.LeftControl) && (Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.Keypad0)))
            {
                Save();
            }
        }

        private void Save()
        {
            if (string.IsNullOrEmpty(_savePath))
            {
                Debug.LogError($"[{nameof(ImageTool)}] 파일 저장 경로가 입력되지 않았습니다.");
                return;
            }
            Texture2D tex = new Texture2D(_warpMat.width(), _warpMat.height(), TextureFormat.RGB24, false);
            Utils.matToTexture2D(_warpMat, tex, true);

            byte[] bytes = tex.EncodeToPNG();
            File.WriteAllBytes(Application.streamingAssetsPath + '/' + _savePath, bytes);

            Model.First<DetectionInfoModel>().CVSettings.DiffTargetPath = _savePath;
        }

        private void ProcessImage()
        {
            Point p = new Point();
            Scalar outerColor = new Scalar(255, 0, 0);
            Scalar innerColor = new Scalar(0, 0, 255);
            float ratio = Resolution.x / Resolution.y;

            p.x = Input.mousePosition.x / Screen.width * Resolution.x;
            p.y = (1f - Input.mousePosition.y / Screen.height) * Resolution.y;
            Imgproc.circle(_calibMat, p, _mode == RectMode.Outer ? (int)(6 * ratio) : (int)(3 * ratio), _mode == RectMode.Outer ? outerColor : innerColor);

            int number = 1;
            foreach (var point in _outerPoints)
            {
                p.x = point.X * Resolution.x;
                p.y = point.Y * Resolution.y;
                Imgproc.circle(_calibMat, p, (int)(6 * ratio), outerColor, 2);
                p.y -= 20;
                Imgproc.putText(_calibMat, number.ToString(), p, 2, ratio, outerColor);
                number++;
            }

            number = 1;
            foreach (var point in _innerPoints)
            {
                p.x = point.X * Resolution.x;
                p.y = point.Y * Resolution.y;
                Imgproc.circle(_calibMat, p, (int)(3 * ratio), innerColor, 2);
                p.y -= 20;
                Imgproc.putText(_calibMat, number.ToString(), p, 2, ratio, innerColor);
                number++;
            }

            Utils.matToTexture2D(_calibMat, _texture);
        }

        private UnityEngine.Rect GetUIRect(RectTransform transform)
        {
            Vector2 uiPos = transform.position;
            uiPos -= transform.rect.size * transform.pivot * transform.lossyScale;
            UnityEngine.Rect uiRect = new UnityEngine.Rect(uiPos, transform.rect.size);
            return uiRect;
        }

        public bool Enabled
        {
            get
            {
                return _enabled;
            }

            set
            {
                if (_enabled != value)
                {
                    if (value)
                    {
                        Message.AddListener<CV.Output.CalibMat>(ProcessCalibMat);
                    }
                    else
                    {
                        Message.RemoveListener<CV.Output.CalibMat>(ProcessCalibMat);
                    }
                }

                _currentPointIdx = 0;
                _mode = RectMode.Outer;
                _image.enabled = value;
                _enabled = value;
            }
        }

        public RawImage Image
        {
            get
            {
                return _image;
            }

            set
            {
                _image = value;
                _image.texture = _texture;
            }
        }

        public Vector2Int Resolution { get; private set; }
    }
}