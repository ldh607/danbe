using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using CellBig;
using CellBig.Models;
using CellBig.UI.Event;
using CellBig.Constants;

namespace CellBig.UI
{
    public class SelectGameDialog : IDialog
    {
        public Canvas _canvas;
        private SettingModel _sm;
        public GameObject MainUI;
        public GameObject SettingUI;
        Image FadeAllImg;
        Image FadeOBJ;
        [Space]
        [Header("GameThumnail Image")]
        public Image LeftGameImage;
        public Image MidGameImage;
        public Image RightGameImage;
        public Sprite DefaultGameImage;
        public int OnGameCount = 0;
        public int CurrentSelectedIndex = 0;
        public GameObject Bottom;
        public GameObject FadeAllOBJ;
        [Space]

        [SerializeField] List<GameData> gamedata = new List<GameData>();
        [SerializeField] public List<GameObject> ToggleOBJList = new List<GameObject>();

        public void SetContents()
        {
            var BG = transform.Find("BG");
            foreach (ContentType GameName in Enum.GetValues(typeof(ContentType)))
            {
                if (IsFindBundle(GameName))
                {
                    GameData gameData = new GameData();
                    gameData.GameName = (SceneName)GameName;
                    gameData.GameNameShow = GameName.ToString();
                    gameData.GameSelectToggle = true;
                    gameData.GameTime = 0;
                    gameData.GameThumnail = Resources.Load<Sprite>("UIs/Textures/KR/" + GameName.ToString());
                    gameData.BackGround = BG.Find(GameName.ToString() + "BG").gameObject;
                    gamedata.Add(gameData);
                }
            }
            for (int i = 0; i < BG.childCount; i++)
            {
                BG.GetChild(i).gameObject.SetActive(false);
            }
            if (gamedata.Count <= 0)
            {
                MidGameImage.sprite = DefaultGameImage;
                LeftGameImage.sprite = DefaultGameImage;
                RightGameImage.sprite = DefaultGameImage;
            }

        }

        public bool IsFindBundle(ContentType GameName)
        {
            var path = Application.streamingAssetsPath + "/AssetBundles/" + GameName.ToString() + "Bundle";
            Debug.Log(path);
            return File.Exists(path);
        }

        public void SetGameTime(int ToggleNum)
        {
            gamedata[ToggleNum].GameTime = int.Parse(ToggleOBJList[ToggleNum].transform.Find("InputField").gameObject.GetComponent<InputField>().text);
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            _sm = Model.First<SettingModel>();
        }

        protected override void OnEnter()
        {
            SoundManager.Instance.PlaySound(SoundType.lobby_bgm_main_0);
            _canvas = transform.parent.GetComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            FadeAllImg = transform.Find("FadeAll").GetComponent<Image>();
            FadeOBJ = GameObject.Find("UI").transform.Find("Fade").GetComponent<Image>();
            FadeAllImg.DOFade(0, 0.5f);
            SetContents();
            SetContentsToggle();
            OnGameCount = gamedata.Count;
            FadeOBJ.DOFade(0, 0.75f);
            SetThumnail();
            setPage(0);
            SettingUI.SetActive(false);
            FadeAllOBJ.SetActive(true);
        }

        protected override void OnExit()
        {
            SoundManager.Instance.StopSound(SoundType.lobby_bgm_main_0);
        }

        void FadeIn(Image FadeImg)
        {
            if (FadeImg.color.a < 1)
                FadeImg.color = new Color(0, 0, 0, 1);
        }
        void FadeOut(Image FadeImg)
        {
            if (FadeImg.color.a > 0f)
                FadeImg.DOFade(0, 0.5f).SetDelay(0.25f);
        }

        public int curPage = 1;
        public int maxPage;

        public void setPage(int arrow)
        {
            Text pageText = Bottom.transform.GetChild(0).GetComponent<Text>();
            maxPage = (gamedata.Count / 3) + 1;
            curPage += arrow;
            if (curPage == 0)
            {
                curPage++;
            }
            else if (curPage > maxPage)
            {
                curPage--;
            }
            pageText.text = curPage + "/" + maxPage + "Page";

            SetContentsToggle();
        }

        List<GameData> gameBGData;
        public void SetThumnail()
        {
            gameBGData = new List<GameData>();
            foreach (var item in gamedata)
            {
                if(item.GameSelectToggle)
                {
                    gameBGData.Add(item);
                }
            }
            foreach (var item in gamedata)
            {
                item.BackGround.SetActive(false);
            }
            if (OnGameCount > 1)
            {
                LeftGameImage.sprite = CurrentSelectedIndex == 0 ?
                    gameBGData[gameBGData.Count - 1].GameThumnail : gameBGData[CurrentSelectedIndex - 1].GameThumnail;

                MidGameImage.sprite = gameBGData[CurrentSelectedIndex].GameThumnail;

                RightGameImage.sprite = OnGameCount > 2 ?
                    (CurrentSelectedIndex >= OnGameCount - 1 ?
                    gameBGData[0].GameThumnail :
                gameBGData[CurrentSelectedIndex + 1].GameThumnail) : DefaultGameImage;

                gameBGData[CurrentSelectedIndex].BackGround.SetActive(true);
            }
            else
            {
                LeftGameImage.sprite = DefaultGameImage;
                RightGameImage.sprite = DefaultGameImage;

                GameData activeGame;
                for (int i = 0; i < gameBGData.Count; i++)
                {
                    if (gameBGData[i].GameSelectToggle)
                    {
                        activeGame = gameBGData[i];
                        MidGameImage.sprite = activeGame.GameThumnail;
                        CurrentSelectedIndex = i;
                        activeGame.BackGround.SetActive(true);
                        break;
                    }
                }
            }
        }

        public void OnGameStart()
        {
            if (gamedata.Count > 0)
            {
                Message.Send<SelectContentMsg>(new SelectContentMsg((Constants.ContentType)((int)gamedata[CurrentSelectedIndex].GameName), gamedata));
                SoundManager.Instance.PlaySound(SoundType.lobby_sfx_button_0);
            }
        }

        public void SetContentsToggle()
        {
            for (int i = 0, j = (curPage * 3) - 3; i < 3; i++)
            {
                if (gamedata.Count > (j + i))
                {
                    ToggleOBJList[i].SetActive(true);

                    Toggle toggle = ToggleOBJList[i].transform.GetComponentInChildren<Toggle>(true);
                    toggle.onValueChanged.RemoveAllListeners();

                    ToggleOBJList[i].transform.GetComponentInChildren<Text>(true).text = gamedata[j + i].GameNameShow;

                    toggle.isOn = gamedata[j + i].GameSelectToggle;
                    toggle.interactable = true;
                    int index = j + i;
                    toggle.onValueChanged.AddListener(
                    delegate
                    {
                        OnContentListToggle(index);
                    });
                    //ToggleOBJList[i].transform.Find("InputField").gameObject.SetActive(toggle.interactable);
                    ToggleOBJList[i].transform.Find("InputField").gameObject.SetActive(gamedata[j + i].GameSelectToggle);
                }
                else
                {
                    ToggleOBJList[i].SetActive(false);
                    ToggleOBJList[i].transform.GetComponentInChildren<Text>(true).text = "Comming Soon";
                    ToggleOBJList[i].transform.GetComponentInChildren<Toggle>(true).interactable = false;
                    ToggleOBJList[i].transform.Find("InputField").gameObject.SetActive(false);
                }
            }
        }

        public void OnContentListToggle(int Index)
        {
            if (gamedata.Count <= Index)
                return;

            bool isZeroToggle = true;
            bool isActive = ToggleOBJList[Index % 3].transform.GetComponentInChildren<Toggle>(true).isOn;
            gamedata[Index].GameSelectToggle = isActive;

            foreach (var item in gamedata)
            {
                if (item.GameSelectToggle)
                {
                    isZeroToggle = false;
                }
            }

            if (isZeroToggle)
            {
                ToggleOBJList[Index % 3].transform.GetComponentInChildren<Toggle>(true).isOn = true;
                isActive = ToggleOBJList[Index % 3].transform.GetComponentInChildren<Toggle>(true).isOn;
                gamedata[Index].GameSelectToggle = isActive;
            }

            OnGameCount += isActive ? 1 : -1;

            if (SettingUI.activeSelf)
                SoundManager.Instance.PlaySound(SoundType.lobby_sfx_button_0);

            ToggleOBJList[Index % 3].transform.Find("InputField").gameObject.SetActive(isActive);
        }


        public void OnGameSettingBtn()
        {

            MainUI.SetActive(SettingUI.activeSelf);
            SettingUI.SetActive(!SettingUI.activeSelf);

            SetThumnail();
            SoundManager.Instance.PlaySound(SoundType.lobby_sfx_button_0);

        }

        float BtnDelay = 1.3f;
        float curdelay = 0;
        bool isPossibleBtn = true;

        IEnumerator _cTimeDelayCalcul()
        {
            if (isPossibleBtn)
            {
                while (curdelay < BtnDelay)
                {
                    isPossibleBtn = false;
                    curdelay += Time.deltaTime;
                    yield return null;
                }
                curdelay = 0;
                isPossibleBtn = true;

            }
            else yield break;
        }

        public void OnGameSelectArrowBtn(int arrow)
        {
            if (OnGameCount <= 1 || isPossibleBtn == false) return;
            StartCoroutine(_cTimeDelayCalcul());
            FadeIn(FadeOBJ);

            CurrentSelectedIndex += arrow;

            if (CurrentSelectedIndex < 0)
                CurrentSelectedIndex = gameBGData.Count - 1;

            else if (CurrentSelectedIndex >= gameBGData.Count)
                CurrentSelectedIndex = 0;
            SetThumnail();
            FadeOut(FadeOBJ);
            SoundManager.Instance.PlaySound(SoundType.lobby_sfx_button_0);

        }
    }
}

