using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using OpenCVForUnity;
using CellBig.Models;
using CellBig.Module.VideoDevice;

namespace CellBig.Module.Detection
{
    public class SyncTool : MonoBehaviour
    {
        private enum RectMode
        {
            Outer,
            Inner
        }

        [SerializeField]
        private RawImage _image;
        [SerializeField]
        private GameObject _syncHelper;
        
        private RectMode _mode = RectMode.Outer;
        private NormalizedPoint[] _outerPoints = new NormalizedPoint[4];
        private NormalizedPoint[] _innerPoints = new NormalizedPoint[4];
        private int _currentPointIdx = 0;
        private Mat _cameraMat;
        private Mat _syncMat;
        private Texture2D _texture;
        private CV.CVSettings _cvSettings;
        
        private bool _enabled = false;

        private void Start()
        {
            _image.enabled = false;
            _syncHelper.SetActive(false);

            DetectionInfoModel info = Model.First<DetectionInfoModel>();
            _cvSettings = info.CVSettings;
            OnWarpingRectChanged(new CV.RefreshWarpingRect() { Outer = _cvSettings.OuterWarpingRect, Inner = _cvSettings.InnerWarpingRect });

            Vector2Int resolution = _cvSettings.OutputResolution;
            _cameraMat = new Mat(resolution.y, resolution.x, CvType.CV_8UC3);
            _syncMat = new Mat(resolution.y, resolution.x, CvType.CV_8UC3);
            _texture = new Texture2D(resolution.x, resolution.y, TextureFormat.RGB24, false);
            _image.texture = _texture;

            Message.AddListener<CV.RefreshWarpingRect>(OnWarpingRectChanged);
        }

        private void OnDestroy()
        {
            Message.RemoveListener<CV.RefreshWarpingRect>(OnWarpingRectChanged);
        }

        private void Update()
        {
            CheckActive();
            CheckSyncHelperActive();

            if (Enabled)
            {
                CheckPointChange();
                CheckPointSelection();
                CheckPointTranslation();
            }
        }

        private void CheckActive()
        {
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Tab))
            {
                if (!Enabled)
                {
                    Enabled = true;
                    Message.AddListener<CV.Output.CalibMat>(ProcessCameraMat);
                }
                else
                {
                    if (_mode == RectMode.Outer)
                    {
                        Message.RemoveListener<CV.Output.CalibMat>(ProcessCameraMat);
                        Message.AddListener<CV.Output.WaterplayMat>(ProcessSyncMat);
                        _mode = RectMode.Inner;
                    }
                    else if (_mode == RectMode.Inner)
                    {
                        Message.RemoveListener<CV.Output.WaterplayMat>(ProcessSyncMat);
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
            if (Input.GetMouseButton(0))
            {
                UnityEngine.Rect rect = GetUIRect(_image.rectTransform);
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

        private void CheckSyncHelperActive()
        {
            if (Input.GetKeyDown(KeyCode.Insert))
            {
                _syncHelper.SetActive(true);
            }
            else if (Input.GetKeyDown(KeyCode.Delete))
            {
                _syncHelper.SetActive(false);
            }
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

        private UnityEngine.Rect GetUIRect(RectTransform transform)
        {
            Vector2 uiPos = transform.position;
            uiPos -= transform.rect.size * transform.pivot * transform.lossyScale;
            UnityEngine.Rect uiRect = new UnityEngine.Rect(uiPos, transform.rect.size);
            return uiRect;
        }

        private void ProcessCameraMat(CV.Output.CalibMat mat)
        {
            mat.Value.copyTo(_cameraMat);

            Point p = new Point();
            Scalar outerColor = new Scalar(255, 0, 0);
            float ratio = _cameraMat.width() / _cameraMat.height();

            p.x = Input.mousePosition.x / Screen.width * _cameraMat.width();
            p.y = (1f - Input.mousePosition.y / Screen.height) * _cameraMat.height();
            Imgproc.circle(_cameraMat, p, (int)(6 * ratio), outerColor);

            int number = 1;
            foreach (var point in _outerPoints)
            {
                p.x = point.X * _cameraMat.width();
                p.y = point.Y * _cameraMat.height();
                Imgproc.circle(_cameraMat, p, (int)(6 * ratio), outerColor, 2);
                p.y -= 20;
                Imgproc.putText(_cameraMat, number.ToString(), p, 2, ratio, outerColor);
                number++;
            }

            Utils.matToTexture2D(_cameraMat, _texture);
        }

        private void ProcessSyncMat(CV.Output.WaterplayMat mat)
        {
            Imgproc.cvtColor(mat.Value, _syncMat, Imgproc.COLOR_GRAY2RGB);

            Mat transform = _cvSettings.InnerPerspectiveTransform * _cvSettings.OuterPerspectiveTransform.inv();
            Imgproc.warpPerspective(_syncMat, _syncMat, transform, _syncMat.size());

            Point p = new Point();
            Scalar innerColor = new Scalar(0, 0, 255);
            float ratio = _syncMat.width() / _syncMat.height();

            p.x = Input.mousePosition.x / Screen.width * _syncMat.width();
            p.y = (1f - Input.mousePosition.y / Screen.height) * _syncMat.height();
            Imgproc.circle(_syncMat, p, (int)(6 * ratio), innerColor);
            
            int number = 1;
            foreach (var point in _innerPoints)
            {
                p.x = point.X * _syncMat.width();
                p.y = point.Y * _syncMat.height();
                Imgproc.circle(_syncMat, p, (int)(6 * ratio), innerColor, 2);
                p.y -= 20;
                Imgproc.putText(_syncMat, number.ToString(), p, 2, ratio, innerColor);
                number++;
            }

            Utils.matToTexture2D(_syncMat, _texture);
        }

        public bool Enabled
        {
            get
            {
                return _enabled;
            }

            set
            {
                _currentPointIdx = 0;
                _mode = RectMode.Outer;
                _image.enabled = value;
                _enabled = value;
            }
        }
    }
}