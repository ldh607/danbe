using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using System;

namespace CellBig.Contents
{
    public abstract class IContent : MonoBehaviour
    {
        protected string _name;

        public IContentUILoader _uiLoader;
        public bool dontDestroy = false;
        public bool stackable = true;

        public delegate void OnComplete(GameObject obj);
        OnComplete _onLoadComplete;

        protected bool _contentLoadComplete = true;
        protected bool _uiLoadComplete = true;

        public bool isActive { get; private set; }

        [Header("[ Lighting Setting ]")]
        public LightingsSetting lightingSetting;

        void LightingInit()
        {
            RenderSettings.skybox = null;
            RenderSettings.sun = null;

            RenderSettings.ambientMode = AmbientMode.Skybox;
            RenderSettings.ambientIntensity = 1;
            RenderSettings.ambientSkyColor = new Color(0.212f, 0.227f, 0.259f);
            RenderSettings.defaultReflectionMode = DefaultReflectionMode.Skybox;

            RenderSettings.defaultReflectionResolution = 128;
            RenderSettings.reflectionIntensity = 1;
            RenderSettings.reflectionBounces = 1;
        }


        public virtual void LightingSetting()
        {
            if (!lightingSetting.isLightingSetting)
                return;
            RenderSettings.skybox = lightingSetting.skyBoxMaterial;
            RenderSettings.sun = lightingSetting.sunSource;
            RenderSettings.ambientMode = lightingSetting.ambientMode;

            if (lightingSetting.ambientMode == AmbientMode.Trilight) RenderSettings.ambientSkyColor = lightingSetting.TrilightMode_SkyColor;
            else if (lightingSetting.ambientMode == AmbientMode.Flat) RenderSettings.ambientSkyColor = lightingSetting.FlatMode_AmbientColor;

            RenderSettings.ambientIntensity = lightingSetting.SkyBoxMode_IntensityMultiplier;
            RenderSettings.ambientEquatorColor = lightingSetting.TrilightMode_EquatorColor;
            RenderSettings.ambientGroundColor = lightingSetting.TrilightMode_GroundColor;


            RenderSettings.defaultReflectionMode = lightingSetting.reflectionMode;

            if (RenderSettings.defaultReflectionMode == DefaultReflectionMode.Custom)
                RenderSettings.customReflection = lightingSetting.CubemapMode_Cubemap;

            RenderSettings.defaultReflectionResolution = lightingSetting.reflection_Resolution;

            RenderSettings.reflectionIntensity = lightingSetting.Reflections_IntensityMultiplier;
            RenderSettings.reflectionBounces = lightingSetting.Reflections_Bounces;

            if (lightingSetting.isLightMap)
            {
                lightingSetting.lightMapData = new LightmapData[lightingSetting.lightMapCount];

                for (int i = 0; i < lightingSetting.lightMapCount; i++)
                {
                    LightmapData data = new LightmapData();
                    data.lightmapColor = Resources.Load("LightMap/" + lightingSetting.contentName + "/Lightmap-" + i + "_comp_light") as Texture2D;
                    data.lightmapDir = Resources.Load("LightMap/" + lightingSetting.contentName + "/Lightmap-" + i + "_comp_dir") as Texture2D;

                    lightingSetting.lightMapData[i] = data;
                }

                LightmapSettings.lightmaps = lightingSetting.lightMapData;
            }

            RenderSettings.fog = lightingSetting.Fog;
            if (lightingSetting.Fog)
            {
                RenderSettings.fogMode = lightingSetting.Fog_Mode;
                RenderSettings.fogColor = lightingSetting.Fog_Color;
                RenderSettings.fogDensity = lightingSetting.Fog_Density;
                RenderSettings.fogEndDistance = lightingSetting.Fog_End;
                RenderSettings.fogStartDistance = lightingSetting.Fog_Start;
            }

            DynamicGI.UpdateEnvironment();
        }


#if UNITY_EDITOR
        public void SetEditorLightingsSetting()
        {
            if (lightingSetting.isLightingSetting)
            {
                lightingSetting.skyBoxMaterial = RenderSettings.skybox;
                lightingSetting.sunSource = RenderSettings.sun;
                lightingSetting.ambientMode = RenderSettings.ambientMode;

                lightingSetting.SkyBoxMode_IntensityMultiplier = RenderSettings.ambientIntensity;
                lightingSetting.TrilightMode_SkyColor = RenderSettings.ambientSkyColor;
                lightingSetting.TrilightMode_EquatorColor = RenderSettings.ambientEquatorColor;
                lightingSetting.TrilightMode_GroundColor = RenderSettings.ambientGroundColor;
                lightingSetting.FlatMode_AmbientColor = RenderSettings.ambientSkyColor;

                lightingSetting.reflectionMode = RenderSettings.defaultReflectionMode;

                if (RenderSettings.defaultReflectionMode == DefaultReflectionMode.Custom)
                    lightingSetting.CubemapMode_Cubemap = RenderSettings.customReflection;

                lightingSetting.reflection_Resolution = RenderSettings.defaultReflectionResolution;

                lightingSetting.Reflections_IntensityMultiplier = RenderSettings.reflectionIntensity;
                lightingSetting.Reflections_Bounces = RenderSettings.reflectionBounces;

                if (lightingSetting.isLightMap)
                {
                    lightingSetting.lightMapCount = LightmapSettings.lightmaps.Length;
                }

                lightingSetting.Fog = RenderSettings.fog;
                if (lightingSetting.Fog)
                {
                    lightingSetting.Fog_Mode = RenderSettings.fogMode;
                    lightingSetting.Fog_Color = RenderSettings.fogColor;
                    lightingSetting.Fog_Density = RenderSettings.fogDensity;
                    lightingSetting.Fog_End = RenderSettings.fogEndDistance;
                    lightingSetting.Fog_Start = RenderSettings.fogStartDistance;
                }
            }
        }
#endif
        public void Load(OnComplete complete)
        {
            _name = GetType().Name;
            _onLoadComplete = complete;

            Message.AddListener<Event.EnterContentMsg>(_name, Enter);
            Message.AddListener<Event.ExitContentMsg>(_name, Exit);

            StartCoroutine(LoadingProcess());
        }

        void LoadContentsUI()
        {
            if (_uiLoader != null)
            {
                _uiLoader.Load(
                    () =>
                    {
                        _uiLoadComplete = true;
                        OnUILoadComplete();
                    });
            }
            else
            {
                _uiLoadComplete = true;
            }
        }

        IEnumerator LoadingProcess()
        {
            _uiLoadComplete = false;
            _contentLoadComplete = false;

            OnLoadStart();
            LoadContentsUI();

            do
            {
                yield return null;
            }
            while (!_uiLoadComplete || !_contentLoadComplete);

            OnLoadComplete();

            if (_onLoadComplete != null)
                _onLoadComplete(gameObject);
        }

        protected void SetLoadComplete()
        {
            _contentLoadComplete = true;
        }

        /// <summary>
        /// 생성과 동시에 메시지 및 모델을 생성해야 할 경우 재정의 한 후 구현한다.
        /// 이 콜백을 재정의 하게 되면 적절한 타이밍에 SetLoadComplete() 를 호출해주어야 한다.
        /// </summary>
        protected virtual void OnLoadStart()
        {
            SetLoadComplete();
        }

        protected virtual void OnLoading(float progress)
        {
            /* BLANK */
        }

        protected virtual void OnLoadComplete()
        {
            /* BLANK */
        }

        protected virtual void OnUILoadComplete()
        {
            /* BLANK */
        }

        public void Unload()
        {
            Message.RemoveListener<Event.EnterContentMsg>(_name, Enter);
            Message.RemoveListener<Event.ExitContentMsg>(_name, Exit);

            OnExit();

            if (_uiLoader != null)
                _uiLoader.Unload();

            OnUnload();
        }

        /// <summary>
        /// OnLoad()에서 생성된 메세지나 모델을 이곳에서 해제 한다.
        /// </summary>
        protected virtual void OnUnload()
        {
            /* BLANK */
        }

        void Enter(Event.EnterContentMsg msg)
        {
            if (isActive)
            {
                Debug.LogWarningFormat("{0} are entered.", _name);
                return;
            }

            isActive = true;

            OnEnter();
        }

        void Exit(Event.ExitContentMsg msg)
        {
            OnExit();

            isActive = false;
        }

        protected abstract void OnEnter();
        protected abstract void OnExit();

        public static void RequestContentEnter<T>() where T : IContent
        {
            Message.Send<Event.EnterContentMsg>(typeof(T).Name, new Event.EnterContentMsg());
        }

        public static void RequestContentExit<T>() where T : IContent
        {
            Message.Send<Event.ExitContentMsg>(typeof(T).Name, new Event.ExitContentMsg());
        }

        public static string GetMsgName<T>()
        {
            return typeof(T).Name;
        }
    }


    [Serializable]
    public class LightingsSetting
    {
        public bool isLightingSetting;

        [Header("[ Environment ]")]
        public Material skyBoxMaterial;
        public Light sunSource;

        [Header("[ Environment_Environment Lighting ]")]
        public AmbientMode ambientMode;

        public float SkyBoxMode_IntensityMultiplier;

        [ColorUsage(false, true)]
        public Color TrilightMode_SkyColor;
        [ColorUsage(false, true)]
        public Color TrilightMode_EquatorColor;
        [ColorUsage(false, true)]
        public Color TrilightMode_GroundColor;

        [ColorUsage(false, true)]
        public Color FlatMode_AmbientColor;

        [Header("[ Environment_Environment Reflections ]")]
        public DefaultReflectionMode reflectionMode;
        public Cubemap CubemapMode_Cubemap;
        public int reflection_Resolution;
        public ReflectionCubemapCompression reflection_Compression;

        [Range(0f, 1f)]
        public float Reflections_IntensityMultiplier;
        [Range(1, 5)]
        public int Reflections_Bounces;

        public bool isLightMap;
        public int lightMapCount;
        public string contentName;

        public LightmapData[] lightMapData;

        [Header("[ Environment Others ]")]
        public bool Fog = false;
        public Color Fog_Color;
        public FogMode Fog_Mode;
        public float Fog_Density;
        public float Fog_End;
        public float Fog_Start;

    }
}
