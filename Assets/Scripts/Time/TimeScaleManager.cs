using UnityEngine;
using System.Collections.Generic;


namespace CellBig.Common
{
	public class TimeScaleManager : MonoSingleton<TimeScaleManager>
	{
		List<KeyValuePair<GameObject, float>> timeScaleList = new List<KeyValuePair<GameObject, float>>();
		List<int> removeIndexList = new List<int>();

		// [youngspapa] 일시정지용 변수
		float _playSpeed = 1f;
		float _speed = singleSpeed;

		// [youngspapa] 수치가 정해지면 상수로 변경해야 함.
		const float singleSpeed = 1.2f;
		const float doubleSpeed = 1.8f;
		bool _doubleSpeedOn = false;

#if UNITY_EDITOR
		static bool _isDuckOn = false;
		public bool isDuckOn = _isDuckOn;
		static float _duckBattleSpeed = singleSpeed;
		public float duckBattleSpeed = _duckBattleSpeed;
#endif

		protected float speed
		{
			get
			{
				return _speed;
			}
			set
			{
				_speed = value;
#if UNITY_EDITOR
				if (_isDuckOn)
					_speed = _duckBattleSpeed;
				_duckBattleSpeed = _speed;
				duckBattleSpeed = _speed;
#endif
				ApplyScale();
			}
		}

		public float currentTimeScale
		{
			get
			{
				if (timeScaleList.Count > 0)
					return _playSpeed * _speed * timeScaleList[timeScaleList.Count - 1].Value;
				return _playSpeed * _speed;
			}
		}

		protected override void Init()
		{
			speed = singleSpeed;
		}

		protected override void Release()
		{
			timeScaleList.Clear();
		}

		void ApplyScale()
		{
			Time.timeScale = currentTimeScale;
		}

		public void SetDoubleSpeed(bool isOn)
		{
			_doubleSpeedOn = isOn;
			SetSpeed();
		}

		void SetSpeed()
		{
			if (_doubleSpeedOn)
				speed = doubleSpeed;
			else
				speed = singleSpeed;
		}

		public void SetScale(GameObject go, float timeScale)
		{
			if (timeScale.AlmostEqual(1f, 0.001f))
				ResetScale(go);
			else
			{
				timeScaleList.Add(new KeyValuePair<GameObject, float>(go, timeScale));
				ApplyScale();
			}
		}

		public void ResetScale(GameObject go)
		{
			for (int i = timeScaleList.Count - 1; i >= 0; i--)
			{
				if (timeScaleList[i].Key == go)
					removeIndexList.Add(i);
			}
			for (int i = 0; i < removeIndexList.Count; i++)
				timeScaleList.RemoveAt(removeIndexList[i]);

			removeIndexList.Clear();

			ApplyScale();
		}

		public void ResetAllScale()
		{
			timeScaleList.Clear();
			SetSpeed();
		}

		public void QuitScaleManager()
		{
			timeScaleList.Clear();
			speed = 1.0f;
		}

		public void Pause()
		{
			_playSpeed = 0f;
			ApplyScale();
		}

		public void Resume()
		{
			_playSpeed = 1f;
			ApplyScale();
		}

		// [youngspapa] 중간에 타임스케일 값이 변경되었을 때 매니저에 기록되어있는 값으로 다시 복구하는 함수
		public void Restore()
		{
			ApplyScale();
		}

		void OnDestroy()
		{
			Time.timeScale = 1f;
		}

#if UNITY_EDITOR
		/// <summary>
		/// For ONLY Battle test scene.
		/// </summary>
		public void ForceSetScaleOne()
		{
			speed = 1.0f;
		}

		/// <summary>
		/// For ONLY Battle test scene.
		/// </summary>
		public void ForceSetScale(float scale)
		{
			speed = scale;
		}

		void Update()
		{
			if (isDuckOn)
			{
				if (!_duckBattleSpeed.AlmostEqual(duckBattleSpeed))
				{
					if (duckBattleSpeed < 0f)
						duckBattleSpeed = 0f;
					_duckBattleSpeed = duckBattleSpeed;
					speed = duckBattleSpeed;
				}
			}

			if (_isDuckOn != isDuckOn)
			{
				_isDuckOn = isDuckOn;
				if (!_isDuckOn)
					SetSpeed();
			}
		}
#endif
	}
}
