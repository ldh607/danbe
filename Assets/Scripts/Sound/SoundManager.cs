#define LOAD_FROM_ASSETBUNDLE

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CellBig.Common;
using DG.Tweening;
using CellBig.Models;

namespace CellBig
{
    public enum SoundType
    {
        None = 0,
        BGM_WaterPlay_Waiting,
        BGM_WaterPlay_Play,
        AMB_WaterPlay_Water,
        SFX_WaterPlay_Duck,
        SFX_WaterPlay_Ball,
        SFX_WaterPlay_Common,
        SFX_WaterPlay_Wheel,
        SFX_WaterPlay_Flower,
        SFX_WaterPlay_Fountain,
        SFX_WaterPlay_Bowl,
        SFX_WaterPlay_Kettle,
        SFX_WaterPlay_Bang,
        Voice_WaterPlay_Wellcome,
        Voice_WaterPlay_GameStart_1,
        Voice_WaterPlay_GameStart_2,
        Voice_WaterPlay_Correct_1,
        Voice_WaterPlay_Correct_2,
        Voice_WaterPlay_Correct_3,
        Voice_WaterPlay_Correct_4,
        Voice_WaterPlay_Correct_5,
        Voice_WaterPlay_GameEnd,
        SFX_WaterPlay_Orange,
        SFX_Voice_Guide_1,
        SFX_Voice_Guide_2,
        lobby_sfx_button_0,
        SFBall_bgm_main_0,
        SFBall_sfx_cannon_0,
        SFBall_sfx_blackhall_0,
        SFBall_sfx_cube_0,
        SFBall_sfx_post_0,
        SFBall_voice_guide_0,
        SFBall_voice_guide_1,
        SFBall_voice_guide_2,
        SFBall_voice_guide_3,
        SFBall_voice_guide_4,
        SFBall_voice_guide_5,
        SFBall_voice_guide_6,
        SFBall_voice_guide_7,
        SFBall_voice_guide_8,
        lobby_bgm_main_0,
        SFBall_sfx_float_0,
        SFBall_sfx_neon_0,
        SFBall_sfx_shard_0,
    }
    public class SoundManager : MonoSingleton<SoundManager>
    {
        public float Volume = 1.0f;
        public float FadeDuration = 1.0f;

        SoundData _table;

        bool _loadComplete = false;
        public bool IsComplete { get { return _loadComplete; } }

        class ClipCache
        {
            public string resourceName;
            public SoundData.Param data;
            public AudioClip clip;
            public bool instant;
            public bool bgm;
        }

        readonly Dictionary<int, ClipCache> _caches = new Dictionary<int, ClipCache>();

        readonly ObjectPool<AudioSource> _audioSourcePool = new ObjectPool<AudioSource>();

        class PlayingAudio
        {
            public ClipCache clipCache;
            public AudioSource audioSource;
            public bool isFadeOut = false;
        }
        readonly LinkedList<PlayingAudio> _playingAudio = new LinkedList<PlayingAudio>();

        public IEnumerator cSetup()
        {
            _loadComplete = false;
            _table = Model.First<SoundTableSettingModel>().GetSoundData();

            for (int i = 0; i < _table.list.Count; i++)
                yield return StartCoroutine(cLoad(_table.list[i].Index));

            _loadComplete = true;
        }

        public IEnumerator cLoad(int id, bool instant = true, bool bgm = false)
        {
            if (id == 0)
                yield break;

            if (_caches.ContainsKey(id))
                yield break;

            var data = _table.list.Find(x => x.Index == id);
            if (data == null)
            {
                Debug.LogErrorFormat("Could not found 'BT_SoundRow' : {0} of {1}", id, gameObject.name);
                yield break;
            }

            string path = Model.First<SettingModel>().GetLocalizingPath();

            string fullpath = string.Empty;
            if (string.IsNullOrWhiteSpace(data.FilePath))
                fullpath = string.Format("Sound/{0}/{1}", path, data.FileName);
            else
                fullpath = string.Format("Sound/{0}/{1}/{2}", path, data.FilePath, data.FileName);


            string bundle_name = $"sound_{path}";
            if (AssetBundleLoader.isEditorLoad) bundle_name = "";

            yield return StartCoroutine(AssetBundleLoader.Instance.LoadAsync<AudioClip>(bundle_name, fullpath, o => _OnPostLoadProcess(o, fullpath, id, data, instant, bgm), ".ogg"));
        }

        void _OnPostLoadProcess(Object o, string name, int id, SoundData.Param data, bool instant, bool bgm)
        {
            if (!_caches.ContainsKey(id))
            {
                if (o == null)
                {
                    Debug.LogError("Sound is NULL : " + name);
                    return;
                }

                var sound = bgm ? o as AudioClip : Instantiate(o) as AudioClip;
                _caches.Add(id, new ClipCache { resourceName = name, data = data, clip = sound, instant = instant, bgm = bgm });
            }
        }

        public int PlaySound(SoundType type, bool fade = false)
        {
            return _PlaySound((int)type, fade);
        }

        public void PlaySoundDelay(SoundType type, float delay, bool fade = false)
        {
            StartCoroutine(_cPlaySoundDelay(type, delay, fade));
        }

        private IEnumerator _cPlaySoundDelay(SoundType type, float delay, bool fade)
        {
            yield return new WaitForSeconds(delay);
            _PlaySound((int)type, fade);
        }

        private int _PlaySound(int id, bool fade = false)
        {
            ClipCache cache;
            if (_caches.TryGetValue(id, out cache))
            {
                var source = _audioSourcePool.GetObject() ?? gameObject.AddComponent<AudioSource>();
                _playingAudio.AddLast(new PlayingAudio { clipCache = cache, audioSource = source });

                source.clip = cache.clip;
                source.loop = cache.data.Loop;
                source.volume = fade ? 0.0f : cache.data.Volum;

                source.Play();

                if (fade)
                    source.DOFade(cache.data.Volum, FadeDuration);
            }

            int count = 0;
            var node = _playingAudio.First;
            while (node != null)
            {
                var audio = node.Value;
                if (audio.clipCache.data.Index == id)
                    count++;

                node = node.Next;
            }

            return count;
        }

        public void StopSound(SoundType type, int indexCount = 1, bool fade = true)
        {
            _StopSound((int)type, indexCount, fade);
        }

        private void _StopSound(int id, int indexCount = 1, bool fade = false)
        {
            int count = indexCount;
            var node = _playingAudio.First;

            while (node != null)
            {
                var audio = node.Value;
                if (audio.clipCache.data.Index == id
                    && (!fade || !audio.isFadeOut))
                {
                    count--;
                    if (0 == count)
                    {
                        if (fade)
                        {
                            audio.audioSource.DOFade(0.0f, FadeDuration).Play();
                        }
                        else
                        {
                            audio.audioSource.Stop();
                            audio.audioSource.clip = null;

                            _audioSourcePool.PoolObject(audio.audioSource);
                            _playingAudio.Remove(node);
                        }

                        break;
                    }
                }

                node = node.Next;
            }
        }

        public bool IsPlaySound(SoundType sound)
        {
            return _IsPlaySound((int)sound);
        }

        private bool _IsPlaySound(int id)
        {
            var node = _playingAudio.First;
            while (node != null)
            {
                var audio = node.Value;
                if (audio.clipCache.data.Index == id)
                {
                    if (audio.audioSource.isPlaying)
                        return true;
                }
                node = node.Next;
            }
            return false;
        }

        public void StopAllSound()
        {
            var node = _playingAudio.First;
            while (node != null)
            {
                var audio = node.Value;
                audio.audioSource.Stop();
                audio.audioSource.clip = null;

                _audioSourcePool.PoolObject(audio.audioSource);

                node = node.Next;
            }
            if (_loadComplete)
                StopAllCoroutines();

            _playingAudio.Clear();
        }

        void LateUpdate()
        {
            var node = _playingAudio.First;
            while (node != null)
            {
                var audio = node.Value;
                if (!audio.audioSource.isPlaying || audio.audioSource.volume == 0f)
                {
                    audio.audioSource.Stop();
                    audio.audioSource.clip = null;

                    _audioSourcePool.PoolObject(audio.audioSource);
                    _playingAudio.Remove(node);
                }

                node = node.Next;
            }
        }

        protected override void Release()
        {
            StopAllSound();
            UnloadAllLoadCaches();
        }

        public void UnloadAllInstantCaches()
        {
            var unloadList = new List<int>();

            foreach (var cache in _caches)
            {
                if (cache.Value.instant)
                {
                    Debug.LogFormat("UnloadAllInstantCaches - {0} - {1} - OK", cache.Value.data.Index, cache.Value.clip.name);

                    Destroy(cache.Value.clip);
                    cache.Value.clip = null;

                    unloadList.Add(cache.Value.data.Index);
                }
                else
                    Debug.LogFormat("UnloadAllInstantCaches - {0} - {1} - NO", cache.Value.data.Index, cache.Value.clip.name);
            }

            for (int i = 0; i < unloadList.Count; ++i)
            {
                _caches.Remove(unloadList[i]);
            }
        }

        public void UnloadAllLoadCaches()
        {
            foreach (var cache in _caches)
            {
                ResourceLoader.Instance.Unload(cache.Value.resourceName);

                if (!cache.Value.bgm)
                    Destroy(cache.Value.clip);

                cache.Value.clip = null;
            }

            _caches.Clear();
        }

        public void PlaySoundOnlyOne(SoundType sType)
        {
            _PlaySoundOnlyOne((int)sType);
        }

        private void _PlaySoundOnlyOne(int id)
        {
            if (!_IsPlaySound(id))
            {
                _PlaySound(id);
            }
        }
    }
}
