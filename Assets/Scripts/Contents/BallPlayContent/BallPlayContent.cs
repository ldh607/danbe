using System.Collections;
using System.Collections.Generic;
using Output = CellBig.Module.Detection.CV.Output;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using CellBig.Common;
using CellBig.Models;
using CellBig.Constants;
using DigitalRuby.ThunderAndLightning;

namespace CellBig.Contents
{
    public class BallPlayContent : IContent
    {
        private Transform _ObjRoot;
        private VideoPlayer _Video;
        private float _VideoTransTime = 3.0f;
        private List<BGData> BGDataList = new List<BGData>();
        public class BGData
        {
            public BallBGState BGName;
            public float BGTime;
            public VideoClip VideoClip;
        }

        [System.Serializable]
        public class SensorDataParse
        {
            public List<Vector2> data = new List<Vector2>();
            public GameObject LineObject;
            public int ParseTick;
            public int ActiveTick;
        }

        [SerializeField]
        private float _zPosition = 20f;

        public Camera _camera;
        private ObjectPool _polygonPool;
        private ObjectPool _fxPool;
        public Material Line;
        List<SensorDataParse> parseData = new List<SensorDataParse>();
        Coroutine Checker;
        Coroutine StandardTimer;

        float FpsTime;
        bool ShowGui;
        public Text Fps;
        public GameObject FX_Line;
        SettingModel _sm;
        public int Lightning_Divider = 20;

        protected override void OnLoadStart()
        {
            _ObjRoot = new GameObject("ObjRoot").transform;
            _ObjRoot.SetParent(transform);
            _ObjRoot.ResetLocal();

            StartCoroutine(_cLoadProcess());
        }

        private IEnumerator _cLoadProcess()
        {
            yield return StartCoroutine(AssetBundleLoader.Instance.LoadAsync<GameObject>("BallPlayBundle", "BallPlayGame", (o) =>
            {
                var bgs = _OnLoadBGObject(o);
            }));
            _ObjRoot.gameObject.SetActive(false);
            SetLoadComplete();
        }

        private GameObject _OnLoadBGObject(GameObject obj)
        {
            return Instantiate(obj, _ObjRoot);
        }

        protected override void OnEnter()
        {
            _ObjRoot.gameObject.SetActive(true);

            var Level = _ObjRoot.GetChild(0).Find("Level");

            for (int i = 0; i < Level.transform.childCount; i++)
            {
                Level.transform.GetChild(i).gameObject.SetActive(false);
            }
            int RandomMap = UnityEngine.Random.Range(0, Level.transform.childCount);
            Level.transform.GetChild(RandomMap).gameObject.SetActive(true);

            Message.Send<UI.Event.FadeOutMsg>(new UI.Event.FadeOutMsg(true, true, 0.5f));
            SoundManager.Instance.PlaySound(SoundType.SFBall_bgm_main_0);

            _camera = _ObjRoot.GetChild(0).Find("BallCam").GetComponent<Camera>();

            if (_polygonPool == null)
                _polygonPool = Util.Instance.CreateObjectPool(this.gameObject, new GameObject("Polygon", typeof(MeshCollider)), 100);

            if (_fxPool == null)
                _fxPool = Util.Instance.CreateObjectPool(this.gameObject, FX_Line, 100);

            _sm = Model.First<SettingModel>();
            UI.IDialog.RequestDialogEnter<BallPlayDialog>();

            Message.AddListener<Output.ViewportContours>(Process);
            _Video = _ObjRoot.GetChild(0).GetChild(0).FindChildRecursive("Video").GetComponent<VideoPlayer>();
            _VideoTransTime = _sm.VideoTransTime;
            SetVideoClip();

            Checker = StartCoroutine(LineObjectCheck());

            if (StandardTimer != null)
                StopCoroutine(StandardTimer);

            StartCoroutine(_cChangeVideo());
        }

        void SetVideoClip()
        {
            BGData bgdata = new BGData();
            bgdata.BGName = BallBGState.Change;
            bgdata.BGTime = _sm.VideoTransTime;
            bgdata.VideoClip = Resources.Load<VideoClip>("Video/" + bgdata.BGName.ToString() + "Video");
            BGDataList.Add(bgdata);

            for (int i = 0; i < _sm.BG_Order.Length; i++)
            {
                BGData bgData = new BGData();
                bgData.BGName = (BallBGState)(_sm.BG_Order[i] + 1);
                bgData.BGTime = _sm.BG_Time[i];
                bgData.VideoClip = Resources.Load<VideoClip>("Video/" + bgData.BGName.ToString() + "Video");
                BGDataList.Add(bgData);
            }
        }

        void OnVideoPrepared(VideoPlayer source_)
        {
            _Video.Play();
        }

        IEnumerator _cChangeVideo()
        {
            int videoIndex = 1;
            while (true)
            {
                _Video.prepareCompleted += OnVideoPrepared;
                _Video.clip = BGDataList[videoIndex].VideoClip;
                _Video.Prepare();

                _sm.BGState = BGDataList[videoIndex].BGName;
                switch (_sm.BGState)
                {
                    case BallBGState.Earth:
                        Physics.gravity = new Vector3(0, -9.81f, 0);
                        break;
                    case BallBGState.Moon:
                        Physics.gravity = new Vector3(0, -1.6f, 0);
                        break;
                    case BallBGState.Space:
                        Physics.gravity = new Vector3(0, 0f, 0);
                        break;
                    default:
                        Physics.gravity = new Vector3(0, -9.81f, 0);
                        break;
                }


                if (BGDataList.Count > 2)
                {
                    yield return new WaitForSeconds(BGDataList[videoIndex].BGTime);
                    videoIndex += 1;
                    if (videoIndex >= BGDataList.Count) videoIndex = 1;

                    _Video.prepareCompleted += OnVideoPrepared;
                    _Video.clip = BGDataList[0].VideoClip;
                    _Video.Prepare();
                    yield return new WaitForSeconds(BGDataList[0].BGTime);
                }
                else
                {
                    yield return new WaitForSeconds(300f);
                }
            }
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Backspace))
            {
                foreach (var item in parseData)
                {
                    if (item.LineObject != null)
                        _polygonPool.PoolObject(item.LineObject);
                }
                parseData.Clear();
                _polygonPool.ClearAll();
            }
            else if (Input.GetKey(KeyCode.UpArrow))
            {
                Physics.gravity += new Vector3(0, 0.1f, 0);
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                Physics.gravity -= new Vector3(0, 0.1f, 0);
            }


        }
        protected override void OnExit()
        {
            Physics.gravity = new Vector3(0, -9.81f, 0);
            SoundManager.Instance.StopAllSound();
            foreach (var item in parseData)
            {
                if (item.LineObject != null)
                    _polygonPool.PoolObject(item.LineObject);
            }
            parseData.Clear();

            if (Checker != null)
            {
                StopCoroutine(Checker);
                Checker = null;
            }
            Message.RemoveListener<Output.ViewportContours>(Process);
            UI.IDialog.RequestDialogExit<BallPlayDialog>();
        }

        void Process(Output.ViewportContours output)
        {
            Debug.Log("-------------- count " + output.Value.Count);
            foreach (var item in parseData)
            {
                item.ParseTick++;
            }

            foreach (var contour in output.Value)
            {
                if (contour.Count <= _sm.MinContourSize)
                    continue;
                bool isDrawn = false;
                var contourData = ConvertList(contour);
                if (contourData.Count <= _sm.MinContourSize)
                    continue;

                foreach (var item in parseData)
                {
                    if (SensorChackList(contourData, item.data))
                    {
                        if (item.LineObject != null)
                        {
                            float mesh = ((float)item.LineObject.GetComponent<MeshFilter>().mesh.vertices.Length) / ((float)contourData.Count);
                            if (mesh < 0.9f)    
                                break;
                        }
                        item.data.Clear();
                        item.data.AddRange(contourData);
                        item.ParseTick = 0;
                        item.ActiveTick++;

                        isDrawn = true;
                        break;
                    }
                }
                if (!isDrawn)
                {
                    SensorDataParse newData = new SensorDataParse();

                    newData.data = contourData;
                    parseData.Add(newData);
                }
            }
        }

        public bool SensorChackList(List<Vector2> preList, List<Vector2> positions)
        {
            float percent = 0;
            int Range = 1;

            int errorCount = 0;
            for (int i = 0; i < positions.Count; i += Range)
            {
                bool isError = true;
                for (int index = 0; index < preList.Count; index++)
                {
                    var dis = (positions[i] - preList[index]).sqrMagnitude;
                    if (dis > 0.01f)
                    {
                        index += 5;
                    }
                    else if (dis < 0.0001f)
                    {
                        percent++;
                        isError = false;
                        break;
                    }
                }
                if (isError)
                {
                    if (++errorCount * Range > positions.Count * 0.1f)
                    {
                        return false;
                    }
                }
            }

            float isValue = (percent * Range) / positions.Count;
            if (isValue > 0.8f)
            {
                return true;
            }
            return false;
        }

        public bool ChackList(List<Vector2> preList, List<Vector2> positions)
        {
            if (preList.Count == positions.Count && preList.Count > 0 && positions.Count > 0)
            {
                if (preList[0] == positions[0] && preList[preList.Count - 1] == positions[positions.Count - 1])
                    return true;
            }
            return false;
        }

        IEnumerator LineObjectCheck()
        {
            while (true)
            {
                ContentReset();
                SensorDataActiveCheck();
                SensorDataRemoveCheck();
                yield return null;
            }
        }

        void ContentReset()
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    StartCoroutine(ContentExit());
                }
                if (Input.GetKeyDown(KeyCode.LeftAlt))
                {
                    StartCoroutine(ContentExit(true));
                }
            }

        }

        private void FpsShow()
        {
            FpsTime += (Time.unscaledDeltaTime - FpsTime) * 0.1f;
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                ShowGui = !ShowGui;
                try
                {
                    Fps.gameObject.SetActive(ShowGui);
                }
                catch (System.Exception e)
                {
                    Debug.LogError(e);
                    throw;
                }
            }


            if (ShowGui)
            {
                float msec = FpsTime * 1000.0f;
                float fps = 1.0f / FpsTime;
                string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
                Fps.text = text;
            }
        }

        void SensorDataRemoveCheck()
        {
            for (int i = 0; i < parseData.Count;)
            {
                if (parseData[i].ParseTick > _sm.MaxInputTick)
                {
                    if (parseData[i].LineObject != null)
                    {
                        for (int k = 0; k < parseData[i].LineObject.transform.childCount; k++)
                        {
                            if (parseData[i].LineObject.transform.childCount > 0)
                            {
                                for (int m = 0; m < parseData[i].LineObject.transform.GetChild(k).childCount; m++)
                                {
                                    Destroy(parseData[i].LineObject.transform.GetChild(k).GetChild(m).gameObject);
                                }
                                _fxPool.PoolObject(parseData[i].LineObject.transform.GetChild(k).gameObject);
                            }
                        }

                        _polygonPool.PoolObject(parseData[i].LineObject);
                    }
                    parseData.Remove(parseData[i]);
                    continue;
                }
                i++;
            }
        }

        void SensorDataActiveCheck()
        {
            for (int i = 0; i < parseData.Count; i++)
            {
                if (parseData[i].ActiveTick > 30)
                {
                    for (int k = 0; k < parseData[i].LineObject.transform.childCount; k++)
                    {
                        if (parseData[i].LineObject.transform.childCount > 0)
                        {
                            for (int m = 0; m < parseData[i].LineObject.transform.GetChild(k).childCount; m++)
                            {
                                Destroy(parseData[i].LineObject.transform.GetChild(k).GetChild(m).gameObject);
                            }
                            _fxPool.PoolObject(parseData[i].LineObject.transform.GetChild(k).gameObject);
                        }
                    }
                    _polygonPool.PoolObject(parseData[i].LineObject);
                    parseData[i].LineObject = null;
                    parseData[i].ActiveTick = _sm.ActiveTick;
                }
                if (parseData[i].ActiveTick >= _sm.ActiveTick && parseData[i].LineObject == null)
                {
                    var outputData = parseData[i].data;
                    GameObject inst = null;
                    inst = _polygonPool.GetObject();
                    inst.transform.SetParent(_polygonPool.transform);
                    inst.name = "Line";
                    inst.layer = 11;
                    Triangulator tr = new Triangulator(outputData.ToArray());
                    int[] indices = tr.Triangulate();
                    GameObject Fx_OBJ = _fxPool.GetObject(inst.transform);
                    LightningBoltPathScriptBase lightning = Fx_OBJ.GetComponent<LightningBoltPathScriptBase>();
                    lightning.LightningPath.Clear();

                    Vector3[] vertices = new Vector3[outputData.Count];
                    for (int j = 0; j < vertices.Length; j++)
                    {
                        Vector2 point = outputData[j];
                        //카메라 Z값 확인해서 수정하기
                        vertices[j] = _camera.ViewportToWorldPoint(new Vector3(point.x, point.y, _zPosition - _camera.transform.position.z));

                        if (_sm.BallPlayLineShow)
                        {
                            if (j % Lightning_Divider == 0)
                            {
                                GameObject light_OBJ = new GameObject();
                                light_OBJ.transform.SetParent(Fx_OBJ.transform);
                                light_OBJ.transform.position = _camera.ViewportToWorldPoint(new Vector3(point.x, point.y, _zPosition - _camera.transform.position.z));
                                lightning.LightningPath.Add(light_OBJ);
                            }
                        }
                        //Debug.Log("좌표 : " + point.x + "///" + point.y);
                    }

                    // Create the mesh
                    Mesh msh = new Mesh();
                    msh.vertices = vertices;
                    msh.triangles = indices;

                    var mf = inst.GetComponent<MeshFilter>();
                    if (mf == null)
                        mf = inst.AddComponent<MeshFilter>();
                    mf.mesh = msh;

                    var edgeCollider = inst.GetComponent<MeshCollider>();
                    if (edgeCollider != null)
                    {
                        edgeCollider.sharedMesh = msh;
                    }
                    if (Model.First<SettingModel>().BallPlayLineShow)
                    {
                        var render = inst.GetComponent<MeshRenderer>();
                        if (render == null)
                            render = inst.AddComponent<MeshRenderer>();
                        Line.SetVector("_FresnelColor", new Vector4(1.4980f, 1.3803f, 1.2078f, 1.0f));

                        render.material = Line;
                    }
                    var repos = inst.GetComponent<PositionReset>();
                    if (repos == null)
                        inst.AddComponent<PositionReset>();
                    parseData[i].LineObject = inst;
                }
            }
        }


        IEnumerator ContentTimer()
        {

            yield return new WaitForSeconds(_sm.ContentTime);
            if (parseData.Count > _sm.ContentEndCount)
                StartCoroutine(ContentExit(true));
            else
                StartCoroutine(ContentExit());
        }

        IEnumerator ContentStopTimer()
        {
            float Timer = 0;
            while (true)
            {
                if (parseData.Count <= 0)
                {
                    Timer += Time.deltaTime;
                }
                else
                {
                    Timer = 0;
                }
                if (Timer > _sm.NoActiveTime)
                {
                    break;
                }
                yield break;
            }
            StartCoroutine(ContentExit());
        }

        IEnumerator ContentExit(bool isReset = false)
        {
            Message.Send<UI.Event.FadeInMsg>(new UI.Event.FadeInMsg(true, true, 0.5f));
            yield return new WaitForSeconds(0.5f);
            SoundManager.Instance.StopAllSound();
            OnExit();

            yield return null;
            SoundManager.Instance.PlaySound(SoundType.Voice_WaterPlay_GameEnd);
            if (isReset)
            {
                IContent.RequestContentExit<BallPlayContent>();
                IContent.RequestContentEnter<BallPlayContent>();
            }
            else
            {
                CellBig.Scene.SceneManager.Instance.Load(Constants.SceneName.BallPlayTutorialScene, true);
            }

        }

        List<Vector2> ConvertList(List<Vector2> source)
        {
            List<Vector2> result = new List<Vector2>();
            for (int i = 0; i < source.Count; i += 5)
            {
                var item = source[i];
                var tmp = ConvertVector2(item);
                tmp = ReconvertVector2(tmp);
                if (!result.Contains(tmp))
                    result.Add(tmp);
            }
            return result;
        }

        Vector2 ConvertVector2(Vector2 source)
        {
            int x = (int)(source.x * 1000);
            int y = (int)(source.y * 1000);
            Vector2 result = new Vector2(x, y);
            return result;
        }

        Vector2 ReconvertVector2(Vector2 source)
        {
            float x = source.x * 0.001f;
            float y = source.y * 0.001f;
            Vector2 result = new Vector2(x, y);
            return result;
        }

    }
}