using System.Collections;
using System.Collections.Generic;
using Output = CellBig.Module.Detection.CV.Output;
using UnityEngine;
using UnityEngine.UI;
using CellBig.Common;
using CellBig.Models;
using UnityEngine.SceneManagement;

namespace CellBig.Contents
{
    public class WaterPlayContent : IContent
    {
        //[System.Serializable]
        //public class DrawnLineData
        //{
        //    public List<Vector2> preList = new List<Vector2>();
        //    public GameObject LineObject;
        //    public int Tick;
        //}
        private Transform _ObjRoot;

        [System.Serializable]
        public class SensorDataParse
        {
            public List<Vector2> data = new List<Vector2>();
            public GameObject LineObject;
            public int ParseTick;
            public int ActiveTick;
        }


        [SerializeField]
        private float _zPosition = 10f;

        public Camera _camera;
        private ObjectPool _polygonPool;
        //private List<GameObject> _activePolygon = new List<GameObject>();

        public List<Obi.ObiEmitter> emitt = new List<Obi.ObiEmitter>();
        public GameObject Emitter;

        public GameObject Floor;
        public GameObject Middle;
        public GameObject Top;
        public GameObject Under;
        public GameObject Pipe;
        public Material Line;

        public Vector2 LeftEgnorePos = new Vector2();
        public Vector2 RightEgnorePos = new Vector2();

        GameObject _emitter;
        GameObject _Floor;
        GameObject _Middle;
        GameObject _Top;
        GameObject _Under;
        GameObject _Pipe;

        //public List<DrawnLineData> DrawnLines = new List<DrawnLineData>();
        List<SensorDataParse> parseData = new List<SensorDataParse>();

        Coroutine Checker;
        Coroutine StandardTimer;
        float FpsTime;
        bool ShowGui;
        public Text Fps;
        SettingModel sm;

        protected override void OnLoadStart()
        {
            _ObjRoot = new GameObject("ObjRoot").transform;
            _ObjRoot.SetParent(transform);
            _ObjRoot.ResetLocal();

            StartCoroutine(_cLoadProcess());
        }

        private IEnumerator _cLoadProcess()
        {
            yield return StartCoroutine(AssetBundleLoader.Instance.LoadAsync<GameObject>("WaterPlayBundle", "WaterPlayGame", (o) =>
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
            sm = Model.First<SettingModel>();
            _ObjRoot.gameObject.SetActive(true);
            WaterSetting();
            this.GetComponent<VoiceObjSound>().enabled = true;
            CellBig.UI.IDialog.RequestDialogEnter<WaterPlayDialog>();
            Message.Send<UI.Event.FadeOutMsg>(new UI.Event.FadeOutMsg(true, true, 0.5f));

            SoundManager.Instance.PlaySound(SoundType.Voice_WaterPlay_GameStart_1 + (int)Random.Range(0, 2));
            Message.AddListener<Output.ViewportContours>(Process);
            Checker = StartCoroutine(LineObjectCheck());
            if (StandardTimer != null)
                StopCoroutine(StandardTimer);
        }

        void WaterSetting()
        {
            Message.Send<UI.Event.FadeOutMsg>(new UI.Event.FadeOutMsg(true, true, 0.5f));
            //  StandardTimer = StartCoroutine(ContentTimer());
            //  NoActiveTimer = StartCoroutine(ContentStopTimer());
            SoundManager.Instance.PlaySound(SoundType.BGM_WaterPlay_Play);
            SoundManager.Instance.PlaySound(SoundType.AMB_WaterPlay_Water);
            _emitter = Instantiate(Emitter, _ObjRoot);
            emitt.Clear();
            emitt.AddRange(_emitter.GetComponentsInChildren<Obi.ObiEmitter>());
            _camera = _ObjRoot.GetChild(0).Find("Main Camera").GetComponent<Camera>();
            _camera.GetComponent<Obi.ObiFluidRenderer>().particleRenderers = _emitter.GetComponentsInChildren<Obi.ObiParticleRenderer>();

            foreach (var item in emitt)
            {
                item.LimitLine = -10;
            }

            _Floor = Instantiate(Floor, _ObjRoot);
            _Middle = Instantiate(Middle, _ObjRoot);
            _Top = Instantiate(Top, _ObjRoot);
            _Under = Instantiate(Under, _ObjRoot);
            _Pipe = Instantiate(Pipe, _ObjRoot);
            _Floor.SetActive(false);
            _Middle.GetComponent<MiddleObjectsActive>().floor = _Floor.GetComponent<FloorObjectsActive>();
            _Middle.SetActive(false);
            Vector3 pos = _Middle.transform.position;
            pos.y = sm.middleYpos;
            _Middle.transform.position = pos;
            _Top.SetActive(false);
            _Under.SetActive(false);
            _Pipe.SetActive(false);

            _Pipe.SetActive(true);
            _Floor.SetActive(true);
            _Middle.SetActive(true);
            _Top.SetActive(true);
            _Under.SetActive(true);

            if (_polygonPool == null)
                _polygonPool = Util.Instance.CreateObjectPool(_ObjRoot.GetChild(0).gameObject, new GameObject("Polygon", typeof(MeshCollider), typeof(Obi.ObiCollider)), 100);

        }

        protected override void OnExit()
        {
            SoundManager.Instance.StopSound(SoundType.BGM_WaterPlay_Play);
            SoundManager.Instance.StopSound(SoundType.AMB_WaterPlay_Water);
            SoundManager.Instance.StopAllSound();
            this.GetComponent<VoiceObjSound>().enabled = false;

            foreach (var item in parseData)
            {
                if (item.LineObject != null)
                    _polygonPool.PoolObject(item.LineObject);
            }
            parseData.Clear();

            if (_Floor != null)
            {
                Destroy(_Floor);
            }
            if (_Middle != null)
            {
                Destroy(_Middle);
            }
            if (_Top != null)
            {
                Destroy(_Top);
            }
            if (_Under != null)
            {
                Destroy(_Under);
            }
            if (_Pipe != null)
            {
                Destroy(_Pipe);
            }
            if (_emitter != null)
            {
                Destroy(_emitter);
            }
            emitt.Clear();

            if (Checker != null)
                StopCoroutine(Checker);

            Message.RemoveListener<Output.ViewportContours>(Process);

            UI.IDialog.RequestDialogExit<WaterPlayDialog>();
        }

        void Process(Output.ViewportContours output)
        {
            Debug.Log("-------------- count " + output.Value.Count);
            foreach (var item in parseData)
            {
                item.ParseTick++;

            }


            //for (int i = 0; i < parseData.Count;i++)
            //{
            //    //if (parseData[i].ParseTick > sm.MaxInputTick)
            //    {
            //        //if (parseData[i].LineObject != null)

            //        _polygonPool.PoolObject(parseData[i].LineObject);

            //    }
            //}
            //parseData.Clear();

            foreach (var contour in output.Value)
            {
                if (contour.Count <= sm.MinContourSize)
                    continue;
                bool isDrawn = false;
                //var contourData = contour;
                var contourData = ConvertList(contour);
                //var contourData = contour;
                if (contourData.Count <= sm.MinContourSize)
                    continue;

                foreach (var item in parseData)
                {
                    if (SensorChackList(contourData, item.data))
                    {
                        if (item.LineObject != null)
                        {
                            float mesh = ((float)item.LineObject.GetComponent<MeshFilter>().mesh.vertices.Length) / ((float)contourData.Count);
                            if (mesh < 0.9)
                                break;
                        }
                        item.data.Clear();
                        item.data.AddRange(contourData);
                        item.ParseTick = 0;
                        item.ActiveTick++;
                        //if (item.ActiveTick > sm.ActiveTick * 2)
                        //    item.ActiveTick = sm.ActiveTick * 2;
                        isDrawn = true;
                        break;
                    }
                }
                if (!isDrawn)
                {

                    //var outputData = contourData;
                    //GameObject inst = null;
                    //inst = _polygonPool.GetObject();
                    //inst.name = "Line";

                    //Triangulator tr = new Triangulator(outputData.ToArray());
                    //int[] indices = tr.Triangulate();

                    //Vector3[] vertices = new Vector3[outputData.Count];
                    //for (int j = 0; j < vertices.Length; j++)
                    //{
                    //    Vector2 point = outputData[j];
                    //    vertices[j] = _camera.ViewportToWorldPoint(new Vector3(point.x, point.y, _zPosition - _camera.transform.position.z));
                    //}

                    //// Create the mesh
                    //Mesh msh = new Mesh();
                    //msh.vertices = vertices;
                    //msh.triangles = indices;

                    //var mf = inst.GetComponent<MeshFilter>();
                    //if (mf == null)
                    //    mf = inst.AddComponent<MeshFilter>();
                    //mf.mesh = msh;

                    //var edgeCollider = inst.GetComponent<MeshCollider>();
                    //if (edgeCollider != null)
                    //{
                    //    edgeCollider.sharedMesh = msh;
                    //    var obiCollider = inst.GetComponent<Obi.ObiCollider>();
                    //    if (obiCollider == null)
                    //        obiCollider = inst.AddComponent<Obi.ObiCollider>();
                    //    obiCollider.SourceCollider = edgeCollider;
                    //}
                    //if (Model.First<SettingModel>().LineShow)
                    //{
                    //    var render = inst.GetComponent<MeshRenderer>();
                    //    if (render == null)
                    //        render = inst.AddComponent<MeshRenderer>();
                    //    render.material = Line;
                    //}
                    //var repos = inst.GetComponent<PositionReset>();
                    //if (repos == null)
                    //    inst.AddComponent<PositionReset>();

                    SensorDataParse newData = new SensorDataParse();

                    newData.data = contourData;
                    //newData.LineObject = inst;
                    parseData.Add(newData);
                }
            }
        }

        public bool SensorChackList(List<Vector2> preList, List<Vector2> positions)
        {
            float percent = 0;

            //int Range = positions.Count / 10;
            //if (Range <= 1)
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
                FpsShow();
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
            //if (Input.GetKeyDown(KeyCode.LeftControl))
            //{
            //    ShowGui = !ShowGui;
            //    Fps.gameObject.SetActive(ShowGui);
            //}
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
                if (parseData[i].ParseTick > sm.MaxInputTick)
                {
                    if (parseData[i].LineObject != null)
                        _polygonPool.PoolObject(parseData[i].LineObject);
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
                    _polygonPool.PoolObject(parseData[i].LineObject);
                    parseData[i].LineObject = null;
                    parseData[i].ActiveTick = sm.ActiveTick;
                }
                if (parseData[i].ActiveTick >= sm.ActiveTick && parseData[i].LineObject == null)
                {
                    var outputData = parseData[i].data;
                    GameObject inst = null;
                    inst = _polygonPool.GetObject();
                    inst.name = "Line";

                    Triangulator tr = new Triangulator(outputData.ToArray());
                    int[] indices = tr.Triangulate();

                    Vector3[] vertices = new Vector3[outputData.Count];
                    for (int j = 0; j < vertices.Length; j++)
                    {
                        Vector2 point = outputData[j];
                        vertices[j] = _camera.ViewportToWorldPoint(new Vector3(point.x, point.y, _zPosition - _camera.transform.position.z));
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
                        var obiCollider = inst.GetComponent<Obi.ObiCollider>();
                        if (obiCollider == null)
                            obiCollider = inst.AddComponent<Obi.ObiCollider>();
                        obiCollider.SourceCollider = edgeCollider;
                    }
                    if (Model.First<SettingModel>().LineShow)
                    {
                        var render = inst.GetComponent<MeshRenderer>();
                        if (render == null)
                            render = inst.AddComponent<MeshRenderer>();
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
            yield return new WaitForSeconds(sm.ContentTime);
            if (parseData.Count > sm.ContentEndCount)
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
                if (Timer > sm.NoActiveTime)
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

            yield return null;
            SoundManager.Instance.PlaySound(SoundType.Voice_WaterPlay_GameEnd);
            if (isReset)
            {
                IContent.RequestContentExit<WaterPlayContent>();
                IContent.RequestContentEnter<WaterPlayContent>();
            }
            else
            {
                CellBig.Scene.SceneManager.Instance.Load(Constants.SceneName.WaterPlayTutorialScene, true);
            }
        }

        List<Vector2> ConvertList(List<Vector2> source)
        {
            List<Vector2> result = new List<Vector2>();
            for (int i = 0; i < source.Count; i += 5)
            {
                //if (source[i].x > LeftEgnorePos.x && source[i].x < LeftEgnorePos.y ||
                //    source[i].x > RightEgnorePos.x && source[i].x < RightEgnorePos.y)
                //    continue;
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