using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CellBig.Module.Detection;
using CellBig.Module.Detection.CV;

namespace CellBig.UI
{
    public class WaterPlaySensorSettingDialog : IDialog
    {
        bool isSet = false;
        CVSettings setting;

        [SerializeField]
        private double _diffMultiplier = 1;
        [SerializeField]
        private int _diffBlurSize;
        [SerializeField]
        private int _diffBlurItertaions;
        [SerializeField]
        private int _lineMedianBlurSize;
        [SerializeField]
        private int _lineMedianBlurIterations;
        [SerializeField]
        private int _lineThreshold;
        [SerializeField]
        private int _lineErodeSize;
        [SerializeField]
        private int _lineDilateSize;
        [SerializeField]
        private int _lineMorphIterations;
        [SerializeField]
        private int _sharpFactor = 2;
        [SerializeField]
        private int _blobNoiseContourSize;
        [SerializeField]
        private int _blobNoiseAreaSize;
        [SerializeField]
        private int _blobNoiseDilateSize;
        [SerializeField]
        private int _blobNoiseDilateIterations;
        [SerializeField]
        private int _coveredLineGaussBlurSize;
        [SerializeField]
        private int _coveredLinePreErodeSize;
        [SerializeField]
        private int _coveredLineMedianBlurSize;
        [SerializeField]
        private int _coveredLineThreshold;
        [SerializeField]
        private int _coveredLineErodeSize;
        [SerializeField]
        private int _coveredLineDilateSize;
        [SerializeField]
        private int _coveredLineMorphIterations;
        [SerializeField]
        private int _finalErodeSize = 1;
        [SerializeField]
        private int _finalErodeIterations;


        ValueControll[] controller;

        protected override void OnEnter()
        {
            //if (!isSet)
            //    StartCoroutine(UiSet());
            base.OnEnter();
            dialogView.SetActive(false);
        }


        private void Update()
        {
            if (Input.GetKey(KeyCode.LeftShift))
                if (Input.GetKeyDown(KeyCode.H))
                {
                    dialogView.SetActive(!dialogView.activeSelf);
                }
        }

        IEnumerator UiSet()
        {
            setting = Model.First<DetectionInfoModel>().CVSettings;
            yield return null;
            InitData();
            dialogView.transform.Find("Detection").Find("Save").GetComponent<Button>().onClick.AddListener(SaveSensor);
            dialogView.transform.Find("Detection").Find("Reset").GetComponent<Button>().onClick.AddListener(ResetSensor);

            controller = GetComponentsInChildren<ValueControll>(true);

            foreach (var item in controller)
            {
                while (!item.isSet)
                {
                    yield return null;
                }
                item.ValueChangeAction = ValueChange;
                SetData(item);
            }

            isSet = true;
            yield return null;
        }

        public void InitData()
        {
            _diffMultiplier = setting.DiffMultiplier;
            _diffBlurSize = setting.DiffBlurSize;
            _diffBlurItertaions = setting.DiffBlurIterations;
            _lineMedianBlurSize = setting.LineMedianBlurSize;
            _lineMedianBlurIterations = setting.LineMedianBlurIterations;
            _lineThreshold = setting.LineThreshold;
            _lineErodeSize = setting.LineErodeSize;
            _lineDilateSize = setting.LineDilateSize;
            _lineMorphIterations = setting.LineMorphIterations;
            _sharpFactor = setting.SharpFactor;
            _blobNoiseContourSize = setting.BlobNoiseContourSize;
            _blobNoiseAreaSize = setting.BlobNoiseAreaSize;
            _blobNoiseDilateSize = setting.BlobNoiseDilateSize;
            _blobNoiseDilateIterations = setting.BlobNoiseDilateIterations;
            _coveredLineGaussBlurSize = setting.CoveredLineGaussBlurSize;
            _coveredLinePreErodeSize = setting.CoveredLinePreErodeSize;
            _coveredLineMedianBlurSize = setting.CoveredLineMedianBlurSize;
            _coveredLineThreshold = setting.CoveredLineThreshold;
            _coveredLineErodeSize = setting.CoveredLineErodeSize;
            _coveredLineDilateSize = setting.CoveredLineDilateSize;
            _coveredLineMorphIterations = setting.CoveredLineMorphIterations;
            _finalErodeSize = setting.FinalErodeSize;
            _finalErodeIterations = setting.FinalErodeIterations;
        }

        void SetData(ValueControll value)
        {
            if (string.Equals(value.gameObject.name, "_diffMultiplier")) value.SetValue(_diffMultiplier.ToString());
            else if (string.Equals(value.gameObject.name, "_diffBlurSize")) value.SetValue(_diffBlurSize.ToString());
            else if (string.Equals(value.gameObject.name, "_diffBlurItertaions")) value.SetValue(_diffBlurItertaions.ToString());
            else if (string.Equals(value.gameObject.name, "_lineMedianBlurSize")) value.SetValue(_lineMedianBlurSize.ToString());
            else if (string.Equals(value.gameObject.name, "_lineMedianBlurIterations")) value.SetValue(_lineMedianBlurIterations.ToString());
            else if (string.Equals(value.gameObject.name, "_lineThreshold")) value.SetValue(_lineThreshold.ToString());
            else if (string.Equals(value.gameObject.name, "_lineErodeSize")) value.SetValue(_lineErodeSize.ToString());
            else if (string.Equals(value.gameObject.name, "_lineDilateSize")) value.SetValue(_lineDilateSize.ToString());
            else if (string.Equals(value.gameObject.name, "_lineMorphIterations")) value.SetValue(_lineMorphIterations.ToString());
            else if (string.Equals(value.gameObject.name, "_sharpFactor")) value.SetValue(_sharpFactor.ToString());
            else if (string.Equals(value.gameObject.name, "_blobNoiseContourSize")) value.SetValue(_blobNoiseContourSize.ToString());
            else if (string.Equals(value.gameObject.name, "_blobNoiseAreaSize")) value.SetValue(_blobNoiseAreaSize.ToString());
            else if (string.Equals(value.gameObject.name, "_blobNoiseDilateSize")) value.SetValue(_blobNoiseDilateSize.ToString());
            else if (string.Equals(value.gameObject.name, "_blobNoiseDilateIterations")) value.SetValue(_blobNoiseDilateIterations.ToString());
            else if (string.Equals(value.gameObject.name, "_coveredLineGaussBlurSize")) value.SetValue(_coveredLineGaussBlurSize.ToString());
            else if (string.Equals(value.gameObject.name, "_coveredLinePreErodeSize")) value.SetValue(_coveredLinePreErodeSize.ToString());
            else if (string.Equals(value.gameObject.name, "_coveredLineMedianBlurSize")) value.SetValue(_coveredLineMedianBlurSize.ToString());
            else if (string.Equals(value.gameObject.name, "_coveredLineThreshold")) value.SetValue(_coveredLineThreshold.ToString());
            else if (string.Equals(value.gameObject.name, "_coveredLineErodeSize")) value.SetValue(_coveredLineErodeSize.ToString());
            else if (string.Equals(value.gameObject.name, "_coveredLineDilateSize")) value.SetValue(_coveredLineDilateSize.ToString());
            else if (string.Equals(value.gameObject.name, "_coveredLineMorphIterations")) value.SetValue(_coveredLineMorphIterations.ToString());
            else if (string.Equals(value.gameObject.name, "_finalErodeSize")) value.SetValue(_finalErodeSize.ToString());
            else if (string.Equals(value.gameObject.name, "_finalErodeIterations")) value.SetValue(_finalErodeIterations.ToString());

        }

        void ValueChange(ValueControll value)
        {
            bool isMiss = false;
            if (string.Equals(value.gameObject.name, "_diffMultiplier"))
            {
                string var = value.GetValue();
                if (var != "")
                {
                    var size = double.Parse(var);
                    setting.DiffMultiplier = size;
                }
                else { isMiss = true; }
            }
            else if (string.Equals(value.gameObject.name, "_diffBlurSize"))
            {
                string var = value.GetValue();
                if (var != "")
                {
                    var size = int.Parse(var);
                    setting.DiffBlurSize = size;
                }
                else { isMiss = true; }
            }
            else if (string.Equals(value.gameObject.name, "_diffBlurItertaions"))
            {
                string var = value.GetValue();
                if (var != "")
                {
                    var size = int.Parse(var);
                    setting.DiffBlurIterations = size;
                }
                else { isMiss = true; }
            }
            else if (string.Equals(value.gameObject.name, "_lineMedianBlurSize"))
            {
                string var = value.GetValue();
                if (var != "")
                {
                    var size = int.Parse(var);
                    setting.LineMedianBlurSize = size;
                }
                else { isMiss = true; }
            }
            else if (string.Equals(value.gameObject.name, "_lineMedianBlurIterations"))
            {
                string var = value.GetValue();
                if (var != "")
                {
                    var size = int.Parse(var);
                    setting.LineMedianBlurIterations = size;
                }
                else { isMiss = true; }
            }
            else if (string.Equals(value.gameObject.name, "_lineThreshold"))
            {
                string var = value.GetValue();
                if (var != "")
                {
                    var size = int.Parse(var);
                    setting.LineThreshold = size;
                }
                else { isMiss = true; }
            }
            else if (string.Equals(value.gameObject.name, "_lineErodeSize"))
            {
                string var = value.GetValue();
                if (var != "")
                {
                    var size = int.Parse(var);
                    setting.LineErodeSize = size;
                }
                else { isMiss = true; }
            }
            else if (string.Equals(value.gameObject.name, "_lineDilateSize"))
            {
                string var = value.GetValue();
                if (var != "")
                {
                    var size = int.Parse(var);
                    setting.LineDilateSize = size;
                }
                else { isMiss = true; }
            }
            else if (string.Equals(value.gameObject.name, "_lineMorphIterations"))
            {
                string var = value.GetValue();
                if (var != "")
                {
                    var size = int.Parse(var);
                    setting.LineMorphIterations = size;
                }
                else { isMiss = true; }
            }
            else if (string.Equals(value.gameObject.name, "_sharpFactor"))
            {
                string var = value.GetValue();
                if (var != "")
                {
                    var size = int.Parse(var);
                    setting.SharpFactor = size;
                }
                else { isMiss = true; }
            }
            else if (string.Equals(value.gameObject.name, "_blobNoiseContourSize"))
            {
                string var = value.GetValue();
                if (var != "")
                {
                    var size = int.Parse(var);
                    setting.BlobNoiseContourSize = size;
                }
                else { isMiss = true; }
            }
            else if (string.Equals(value.gameObject.name, "_blobNoiseAreaSize"))
            {
                string var = value.GetValue();
                if (var != "")
                {
                    var size = int.Parse(var);
                    setting.BlobNoiseAreaSize = size;
                }
                else { isMiss = true; }
            }
            else if (string.Equals(value.gameObject.name, "_blobNoiseDilateSize"))
            {
                string var = value.GetValue();
                if (var != "")
                {
                    var size = int.Parse(var);
                    setting.BlobNoiseDilateSize = size;
                }
                else { isMiss = true; }
            }
            else if (string.Equals(value.gameObject.name, "_blobNoiseDilateIterations"))
            {
                string var = value.GetValue();
                if (var != "")
                {
                    var size = int.Parse(var);
                    setting.BlobNoiseDilateIterations = size;
                }
                else { isMiss = true; }
            }
            else if (string.Equals(value.gameObject.name, "_coveredLineGaussBlurSize"))
            {
                string var = value.GetValue();
                if (var != "")
                {
                    var size = int.Parse(var);
                    setting.CoveredLineGaussBlurSize = size;
                }
                else { isMiss = true; }
            }
            else if (string.Equals(value.gameObject.name, "_coveredLinePreErodeSize"))
            {
                string var = value.GetValue();
                if (var != "")
                {
                    var size = int.Parse(var);
                    setting.CoveredLinePreErodeSize = size;
                }
                else { isMiss = true; }
            }
            else if (string.Equals(value.gameObject.name, "_coveredLineMedianBlurSize"))
            {
                string var = value.GetValue();
                if (var != "")
                {
                    var size = int.Parse(var);
                    setting.CoveredLineMedianBlurSize = size;
                }
                else { isMiss = true; }
            }
            else if (string.Equals(value.gameObject.name, "_coveredLineThreshold"))
            {
                string var = value.GetValue();
                if (var != "")
                {
                    var size = int.Parse(var);
                    setting.CoveredLineThreshold = size;
                }
                else { isMiss = true; }
            }
            else if (string.Equals(value.gameObject.name, "_coveredLineErodeSize"))
            {
                string var = value.GetValue();
                if (var != "")
                {
                    var size = int.Parse(var);
                    setting.CoveredLineErodeSize = size;
                }
                else { isMiss = true; }
            }
            else if (string.Equals(value.gameObject.name, "_coveredLineDilateSize"))
            {
                string var = value.GetValue();
                if (var != "")
                {
                    var size = int.Parse(var);
                    setting.CoveredLineDilateSize = size;
                }
                else { isMiss = true; }
            }
            else if (string.Equals(value.gameObject.name, "_coveredLineMorphIterations"))
            {
                string var = value.GetValue();
                if (var != "")
                {
                    var size = int.Parse(var);
                    setting.CoveredLineMorphIterations = size;
                }
                else { isMiss = true; }
            }
            else if (string.Equals(value.gameObject.name, "_finalErodeSize"))
            {
                string var = value.GetValue();
                if (var != "")
                {
                    var size = int.Parse(var);
                    setting.FinalErodeSize = size;
                }
                else { isMiss = true; }
            }
            else if (string.Equals(value.gameObject.name, "_finalErodeIterations"))
            {
                string var = value.GetValue();
                if (var != "")
                {
                    var size = int.Parse(var);
                    setting.FinalErodeIterations = size;
                }
                else { isMiss = true; }
            }
            if (isMiss == true)
            {
                SetData(value);
            }
        }

        void ResetSensor()
        {
            setting.DiffMultiplier = _diffMultiplier;
            setting.DiffBlurSize = _diffBlurSize;
            setting.DiffBlurIterations = _diffBlurItertaions;
            setting.LineMedianBlurSize = _lineMedianBlurSize;
            setting.LineMedianBlurIterations = _lineMedianBlurIterations;
            setting.LineThreshold = _lineThreshold;
            setting.LineErodeSize = _lineErodeSize;
            setting.LineDilateSize = _lineDilateSize;
            setting.LineMorphIterations = _lineMorphIterations;
            setting.SharpFactor = _sharpFactor;
            setting.BlobNoiseContourSize = _blobNoiseContourSize;
            setting.BlobNoiseAreaSize = _blobNoiseAreaSize;
            setting.BlobNoiseDilateSize = _blobNoiseDilateSize;
            setting.BlobNoiseDilateIterations = _blobNoiseDilateIterations;
            setting.CoveredLineGaussBlurSize = _coveredLineGaussBlurSize;
            setting.CoveredLinePreErodeSize = _coveredLinePreErodeSize;
            setting.CoveredLineMedianBlurSize = _coveredLineMedianBlurSize;
            setting.CoveredLineThreshold = _coveredLineThreshold;
            setting.CoveredLineErodeSize = _coveredLineErodeSize;
            setting.CoveredLineDilateSize = _coveredLineDilateSize;
            setting.CoveredLineMorphIterations = _coveredLineMorphIterations;
            setting.FinalErodeSize = _finalErodeSize;
            setting.FinalErodeIterations = _finalErodeIterations;

            foreach (var item in controller)
            {
                SetData(item);
            }
        }

        void SaveSensor()
        {
            Message.Send<CellBig.Module.Detection.SaveSettings>(new CellBig.Module.Detection.SaveSettings());
            InitData();
        }
    }
}