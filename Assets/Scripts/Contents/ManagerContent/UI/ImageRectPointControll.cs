using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using OpenCVForUnity;
using CellBig.Models;
using CellBig.Module.VideoDevice;

namespace CellBig.Module.Detection
{
    public class ImageRectPointControll : MonoBehaviour
    {
        Toggle isInner;

        Toggle index_1;
        Toggle index_2;
        Toggle index_3;
        Toggle index_4;

        GameObject Point1;
        GameObject Point2;
        GameObject Point3;
        GameObject Point4;

        GameObject TargetPos;
        RectTransform thisTransform;

        NormalizedRect InnerRect;
        NormalizedRect outerRect;
        Coroutine inputRect;
        CV.CVSettings setting;

        bool isSet = false;

        private void Awake()
        {
            setting = Model.First<DetectionInfoModel>().CVSettings;

            if (!isSet)
                UiSet();
        }

        void UiSet()
        {
            thisTransform = GetComponent<RectTransform>();

            Point1 = transform.Find("1").gameObject;
            Point2 = transform.Find("2").gameObject;
            Point3 = transform.Find("3").gameObject;
            Point4 = transform.Find("4").gameObject;

            isInner = transform.parent.Find("OriginOption").Find("Inner").GetComponent<Toggle>();
            isInner.onValueChanged.AddListener(delegate {
                Message.Send(!isInner.isOn ? new CV.RefreshWarpingRect() { Outer = setting.OuterWarpingRect } : new CV.RefreshWarpingRect() { Inner = setting.InnerWarpingRect });
            });
            var num = transform.parent.Find("OriginOption").Find("Num");

            index_1 = num.Find("1").GetComponent<Toggle>();
            index_1.onValueChanged.AddListener(delegate { IndexChange(index_1.isOn, Point1); });
            index_2 = num.Find("2").GetComponent<Toggle>();
            index_2.onValueChanged.AddListener(delegate { IndexChange(index_2.isOn, Point2); });
            index_3 = num.Find("3").GetComponent<Toggle>();
            index_3.onValueChanged.AddListener(delegate { IndexChange(index_3.isOn, Point3); });
            index_4 = num.Find("4").GetComponent<Toggle>();
            index_4.onValueChanged.AddListener(delegate { IndexChange(index_4.isOn, Point4); });
            index_1.isOn = true;
            index_2.isOn = false;
            index_3.isOn = false;
            index_4.isOn = false;
            TargetPos = Point1;
            isSet = true;
        }

        private void OnEnable()
        {
            outerRect = setting.OuterWarpingRect;
            InnerRect = setting.InnerWarpingRect;

            OnWarpingRectChanged(new CV.RefreshWarpingRect() { Outer = outerRect, Inner = InnerRect });
            Message.AddListener<CV.RefreshWarpingRect>(OnWarpingRectChanged);
            inputRect = StartCoroutine(CheckPointTranslation());
        }

        private void OnDisable()
        {
            Message.RemoveListener<CV.RefreshWarpingRect>(OnWarpingRectChanged);
            if (inputRect != null)
                StopCoroutine(inputRect);

        }

        private void OnWarpingRectChanged(CV.RefreshWarpingRect rect)
        {
            if (!isInner.isOn)
            {
                if (rect.Outer != null)
                {
                    NormalizedRect outerRect = rect.Outer.Value;
                    SetRectPosToLocalPos(Point1, new Vector2((float)outerRect.LeftTop.X, (float)outerRect.LeftTop.Y));
                    SetRectPosToLocalPos(Point2, new Vector2((float)outerRect.RightTop.X, (float)outerRect.RightTop.Y));
                    SetRectPosToLocalPos(Point3, new Vector2((float)outerRect.RightBottom.X, (float)outerRect.RightBottom.Y));
                    SetRectPosToLocalPos(Point4, new Vector2((float)outerRect.LeftBottom.X, (float)outerRect.LeftBottom.Y));
                }
            }
            else
            {
                if (rect.Inner != null)
                {
                    NormalizedRect innerRect = rect.Inner.Value;
                    SetRectPosToLocalPos(Point1, new Vector2((float)innerRect.LeftTop.X, (float)innerRect.LeftTop.Y));
                    SetRectPosToLocalPos(Point2, new Vector2((float)innerRect.RightTop.X, (float)innerRect.RightTop.Y));
                    SetRectPosToLocalPos(Point3, new Vector2((float)innerRect.RightBottom.X, (float)innerRect.RightBottom.Y));
                    SetRectPosToLocalPos(Point4, new Vector2((float)innerRect.LeftBottom.X, (float)innerRect.LeftBottom.Y));
                }
            }
        }

        private IEnumerator CheckPointTranslation()
        {
            while (true)
            {
                bool isChange = false;
                int xDir = 0;
                int yDir = 0;
#if UNITY_EDITOR
                if (Input.GetKey(KeyCode.LeftShift))
#else
                if (Input.GetKey(KeyCode.LeftAlt))
#endif
                {
                    if (Input.GetKeyDown(KeyCode.Alpha1))
                    {
                        index_1.isOn = true;
                    }
                    else if (Input.GetKeyDown(KeyCode.Alpha2))
                    {
                        index_2.isOn = true;
                    }
                    else if (Input.GetKeyDown(KeyCode.Alpha3))
                    {
                        index_3.isOn = true;
                    }
                    else if (Input.GetKeyDown(KeyCode.Alpha4))
                    {
                        index_4.isOn = true;
                    }

                    if (Input.GetMouseButtonDown(0))
                    {
                        Vector2 mousePos = Input.mousePosition;
                        TargetPos.transform.position = mousePos;
                        isChange = true;
                    }
                }


                if (Input.GetKey(KeyCode.A)) xDir--;
                if (Input.GetKey(KeyCode.D)) xDir++;
                if (Input.GetKey(KeyCode.S)) yDir--;
                if (Input.GetKey(KeyCode.W)) yDir++;

                if (xDir != 0 || yDir != 0)
                {
                    if (Input.GetKey(KeyCode.LeftShift))
                        TargetPos.transform.localPosition += new Vector3(xDir * 5, yDir * 5);
                    else
                        TargetPos.transform.localPosition += new Vector3(xDir, yDir);
                    isChange = true;
                }


                if( isChange )
                { 
                    var LT = GetlocalPosToRectPos(Point1);
                    var RT = GetlocalPosToRectPos(Point2);
                    var RB = GetlocalPosToRectPos(Point3);
                    var LB = GetlocalPosToRectPos(Point4);

                    NormalizedRect warpingRect = new NormalizedRect();
                    warpingRect.LeftTop = new NormalizedPoint(LT.x, LT.y);
                    warpingRect.RightTop = new NormalizedPoint(RT.x, RT.y);
                    warpingRect.RightBottom = new NormalizedPoint(RB.x, RB.y);
                    warpingRect.LeftBottom = new NormalizedPoint(LB.x, LB.y);
                    //Debug.Log("////////////////rect " + warpingRect);
                    if (!isInner.isOn)
                        setting.OuterWarpingRect = warpingRect;
                    else
                        setting.InnerWarpingRect = warpingRect;

                    Message.Send(!isInner.isOn ? new CV.RefreshWarpingRect() { Outer = warpingRect } : new CV.RefreshWarpingRect() { Inner = warpingRect });
                }
                yield return null;
            }
        }

        private UnityEngine.Rect GetUIRect(RectTransform transform)
        {
            Vector2 uiPos = transform.position;
            uiPos -= transform.rect.size * transform.pivot * transform.lossyScale;
            UnityEngine.Rect uiRect = new UnityEngine.Rect(uiPos, transform.rect.size);
            return uiRect;
        }

        void IndexChange(bool Value , GameObject Point )
        {
            if (!Value)
                return;
            TargetPos = Point;
        }

        Vector2 GetlocalPosToRectPos(GameObject _target)
        {
            Vector3 position = _target.transform.localPosition;
            return new Vector2((position.x + thisTransform.rect.width / 2) / thisTransform.rect.width, 1 - (position.y + thisTransform.rect.height / 2) / thisTransform.rect.height);
        }

        void SetRectPosToLocalPos(GameObject _target, Vector2 pos)
        {
            _target.transform.localPosition = new Vector2(pos.x * thisTransform.rect.width - thisTransform.rect.width / 2, (1 - pos.y) * thisTransform.rect.height - thisTransform.rect.height / 2);
        }

        void ResetOption()
        {
            setting.InnerWarpingRect = InnerRect;
            setting.OuterWarpingRect = outerRect;
        }
    }
}