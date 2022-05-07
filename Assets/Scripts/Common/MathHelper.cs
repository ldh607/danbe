using UnityEngine;
using System.Linq;


namespace CellBig.Common
{
	public static class MathHelper
	{
		public static readonly float baseFrame = 30.0f;
		public static readonly float baseFPS = 1.0f / baseFrame;

		static System.Random _random;

		
		public static float GetFixedPointPart(float fractionalNum)
		{
			int integer = (int)fractionalNum;
			return fractionalNum - integer;
		}

		public static float FrameCountToNormalizeTime(int frame, float length)
		{
			float time = frame * baseFPS;
			float normalize = time / length;
			return Mathf.Clamp(normalize, 0.0f, 1.0f);
		}

		public static float FrameCountToNormalizeTime(float frame, float length)
		{
			float time = Mathf.Round(frame) * baseFPS;
			float normalize = time / length;
			return Mathf.Clamp(normalize, 0.0f, 1.0f);
		}

		public static int NormalizeTimeToFrameCount(float time, float length)
		{
			return Mathf.FloorToInt(time * length * baseFrame);
		}

		public static void RandomSeed(int seed)
		{
			_random = new System.Random(seed);
		}

		public static int Random()
		{
			if (_random == null)
				RandomSeed((int)System.DateTime.Now.Ticks);

			return _random.Next();
		}

		/// <summary>
		/// min <= r <= max
		/// </summary>
		public static int Random(int min, int max)
		{
			if (_random == null)
				RandomSeed((int)System.DateTime.Now.Ticks);

			return _random.Next(min, max + 1);
		}

		/// <summary>
		/// min <= r <= max
		/// </summary>
		public static float Random(float min, float max)
		{
			if (_random == null)
				RandomSeed((int)System.DateTime.Now.Ticks);

			var picked = _random.NextDouble();
			var diff = max - min;
			var random = (float)(picked * diff) + min;

			return Mathf.Clamp(random, min, max);
		}

		public static int[] GetRandomArray(int count, int start, bool useRandomSeed = false)
		{
			if (useRandomSeed)
			{
				if (_random == null)
					RandomSeed((int)System.DateTime.Now.Ticks);

				return Enumerable.Range(start, count).OrderBy(x => _random.Next()).Take(count).ToArray();
			}
			else
			{
				var random = new System.Random();
				return Enumerable.Range(start, count).OrderBy(x => random.Next()).Take(count).ToArray();
			}
		}

		public static float Punch(float amplitude, float period, float damp, float t)
		{
			if (Mathf.Abs(amplitude) < float.Epsilon)
				return 0.0f;

			if (t <= 0.0f)
				return 0.0f;
			else if (t >= 1.0f)
				return 0.0f;

			const float TwoPI = 2.0f * Mathf.PI;
			return (amplitude * Mathf.Pow(2.0f, -damp * t) * Mathf.Sin(t * TwoPI / period));
		}
	}

	
	[System.Serializable]
	public class MinMax
	{
		[SerializeField]
		float _min;

		public float min { get { return _min; } set { _min = value; } }

		[SerializeField]
		float _max;

		public float max { get { return _max; } set { _max = value; } }


		public MinMax()
		{
			_min = 0.0f;
			_max = 0.0f;
		}

		public MinMax(float min, float max)
		{
			_min = min;
			_max = max;
		}

		// min [inclusive] and max[exclusive]
		public bool IsInside(float value)
		{
			return (_max > value && _min <= value);
		}

		// Returns a random float number between and min [inclusive] and max [exclusive].
		public float GetRandom()
		{
			return Random.Range(_min, _max);
		}

		public float GetStep(int current, int total)
		{
			if (current >= total)
				return _max;

			if (current <= 0)
				current = 1;

			float step = (float)(current - 1) / (float)(total - 1);
			return ((_max - _min) * step) + _min;
		}

		public override string ToString()
		{
			return string.Format("Min: {0}, Max: {1}", _min, _max);
		}
	}

	public static class SwapHelper
	{
		public static void Swap<T>(ref T x, ref T y)
		{
			T t = y;
			y = x;
			x = t;
		}
	}
}
