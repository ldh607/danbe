using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using OpenCVForUnity;
using CellBig.Models;

public class IRCamViewer : MonoBehaviour
{

    private Texture2D _texture;

    private Mat _calibMat;
    private Mat _warpMat;
    InputField FilePath;

    [SerializeField, Range(0, 100)]
    private int _saveQuality = 100;
    [SerializeField]
    private RawImage _image;
    public RawImage Image
    {
        get
        {
            return _image;
        }

        set
        {
            _image = value;
        }
    }

    void Start()
    {
        FilePath = transform.parent.Find("OriginOption").Find("FilePath").GetComponent<InputField>();

        transform.parent.Find("OriginOption").Find("Save").GetComponent<Button>().onClick.AddListener(CheckSave);

        CellBig.Module.Detection.DetectionInfoModel info = Model.First<CellBig.Module.Detection.DetectionInfoModel>();
        FilePath.text = info.CVSettings.DiffTargetPath;
        var Resolution = info.CVSettings.OutputResolution;
        _calibMat = new Mat(Resolution.y, Resolution.x, CvType.CV_8UC3);
        _warpMat = new Mat(Resolution.y, Resolution.x, CvType.CV_8UC3);

        _texture = new Texture2D(Resolution.x, Resolution.y, TextureFormat.RGB24, false);
        Image.texture = _texture;
    }

    private void OnEnable()
    {
        Message.AddListener<CellBig.Module.Detection.CV.Output.CalibMat>(ProcessCalibMat);
        Message.AddListener<CellBig.Module.Detection.CV.Output.WarpMat>(ProcessWarpMat);
    }

    private void OnDisable()
    {
        Message.RemoveListener<CellBig.Module.Detection.CV.Output.CalibMat>(ProcessCalibMat);
        Message.RemoveListener<CellBig.Module.Detection.CV.Output.WarpMat>(ProcessWarpMat);
    }

    private void ProcessCalibMat(CellBig.Module.Detection.CV.Output.CalibMat mat)
    {
        if (_calibMat != null)
        {
            mat.Value.copyTo(_calibMat);
            Utils.matToTexture2D(_calibMat, _texture);
        }
    }

    private void ProcessWarpMat(CellBig.Module.Detection.CV.Output.WarpMat mat)
    {
        if(_warpMat!= null)
            mat.Value.copyTo(_warpMat);
    }

    private void CheckSave()
    {
        if (string.IsNullOrEmpty(FilePath.text))
        {
            Debug.LogError($"파일 저장 경로가 입력되지 않았습니다.");
            return;
        }

        Texture2D tex = new Texture2D(_warpMat.width(), _warpMat.height(), TextureFormat.RGB24, false);
        Utils.matToTexture2D(_warpMat, tex, true);

        byte[] bytes = tex.EncodeToJPG(_saveQuality);
        File.WriteAllBytes(Application.streamingAssetsPath + '/' + FilePath.text, bytes);

        Model.First<CellBig.Module.Detection.DetectionInfoModel>().CVSettings.DiffTargetPath = FilePath.text;
    }
}
