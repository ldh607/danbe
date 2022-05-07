using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CellBig.Constants;

namespace CellBig.Models
{
    public class GameModel : Model
    {
        //여기에는 테이블 같은 정적 데이터(모드별 Default 값)
        public ModelRef<SettingModel> setting = new ModelRef<SettingModel>(); // 게임세팅
        public ModelRef<SoundTableSettingModel> soundTableSetting = new ModelRef<SoundTableSettingModel>(); // 사운드
        public ModelRef<LocalizingModel> localizing = new ModelRef<LocalizingModel>(); // 로컬라이징
        public ModelRef<PlayContentModel> playContent = new ModelRef<PlayContentModel>(); // 게임순서 / 정보

        // 키넥트 관련 사항 추가

        public bool Sound = true;       // 사운드 On / Off
        public bool PlayStop = false;   // 게임 플레이 On / Off

        public void Setup()
        {
            setting.Model = new SettingModel();
            setting.Model.Setup(this);

            soundTableSetting.Model = new SoundTableSettingModel();
            soundTableSetting.Model.Setup(this);

            //localizing.Model = new LocalizingModel();
            //localizing.Model.Setup(this);

            playContent.Model = new PlayContentModel();
            playContent.Model.Setup(this);

        }
    }
}

