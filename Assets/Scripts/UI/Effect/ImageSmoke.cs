using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Sprites;
using DG.Tweening;


[ExecuteInEditMode]
[RequireComponent(typeof(Image))]
[AddComponentMenu("UI/Effects/SmokeEffect")]
public class ImageSmoke : MonoBehaviour
{
	public Texture2D SmokeTex;

	[Range(0, 1)] public float _Alpha = 1f;

	[Range(64, 256)] public float _Value1 = 64;
	[Range(0, 1)] public float _Value2 = 1;
	[Range(0, 1)] public float _Value3 = 1;

	public float _Value4;
	public Color _SmokeColor1 = new Color(1f, 0f, 1f, 1f);
	public Color _SmokeColor2 = new Color(1f, 1f, 1f, 1f);

	public AnimationCurve SmokeSpdCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
	public Ease SmokeEaseType = Ease.OutQuad;

	public float delay;
	public float duration;
	public float endSpeedValue;

	public LoopType loopType = LoopType.Restart;
	public int loops = 1;
	public bool isRelative;
	public bool isFrom;

	Tweener _tweenerSmoke;
	Image _target;
	Material _material;
	Shader _shader;

	bool _enable;

	void Update()
	{
//		if (_enable)
//		{
//			_material.SetFloat("_Alpha", 1 - _Alpha);
//			_material.SetFloat("_Value1", _Value1);
//
//			if (_Value2 == 1)
//				_Value2 = 0.995f;
//
//			_material.SetFloat("_Value2", _Value2);
//			_material.SetFloat("_Value3", _Value3);
//			_material.SetFloat("_Value4", _Value4);
//			_material.SetColor("_SmokeColor1", _SmokeColor1);
//			_material.SetColor("_SmokeColor2", _SmokeColor2);
//
//			if (SmokeTex)
//			{
//				SmokeTex.wrapMode = TextureWrapMode.Repeat;
//				_material.SetTexture("_SmokeTex", SmokeTex);
//			}
//
//		}
	}

	void OnEnable()
	{
		_target = GetComponent<Image>();

		EnableSmoke();

#region for Editor
		_tweenerSmoke = _material.DOFloat(endSpeedValue, "_Value2", duration);

		if (isFrom)
			_tweenerSmoke.From(isRelative);
		else
			_tweenerSmoke.SetRelative(isRelative);

		_tweenerSmoke.SetLoops(loops, loopType);
		_tweenerSmoke.SetDelay(delay);

		if (SmokeEaseType == Ease.INTERNAL_Custom)
			_tweenerSmoke.SetEase(SmokeSpdCurve);
		#endregion

//		_tweenerSmoke.Pause();

	}

	void EnableSmoke()
	{
		if (!SetupShader())
			return;
		

		_enable = true;
		ChangeScroll();
	}

	bool SetupShader()
	{
		if (_target == null)
		{
			_target = GetComponent<Image>();
			if (_target == null)
			{
				return false;
			}
		}

		if (_shader == null)
			_shader = Shader.Find("KUF/Smoke");

		if (_material == null || _material == _target.defaultMaterial)
			_target.material = _material = new Material(_shader);


		_material.SetTexture("_SmokeTex", SmokeTex);

		if (SmokeTex)
			SmokeTex.wrapMode = TextureWrapMode.Repeat;

		var uv = DataUtility.GetOuterUV(_target.overrideSprite);
		Vector2 ResultVec = new Vector2();
		ResultVec.Set(1 / (uv.z - uv.x), 1 / (uv.w - uv.y));

		_material.SetFloat("_SmokeInitUvX", uv.x / (uv.z - uv.x));
		_material.SetFloat("_SmokeInitUvY", uv.y / (uv.w - uv.y));

		_material.SetFloat("_MainAtlasUvX", ResultVec.x);
		_material.SetFloat("_MainAtlasUvY", ResultVec.y);

//		Debug.LogFormat("UV : {0}", uv);
//		Debug.LogFormat("_MainAtlasUv :  {0}", ResultVec);
//		Debug.LogFormat("_SmokeInitUvX : {0} _SmokeInitUvY : {1}",
//			uv.x / (uv.z - uv.x), uv.y / (uv.w - uv.y));

		return true;

	}
	public void ChangeScroll()
	{
		if (!_enable)
			return;
		
		_material.SetFloat("_Alpha", 1 - _Alpha);
		_material.SetFloat("_Value1", _Value1);

		if (_Value2 == 1)
			_Value2 = 0.995f;

		_material.SetFloat("_Value2", _Value2);
		_material.SetFloat("_Value3", _Value3);
		_material.SetFloat("_Value4", _Value4);
		_material.SetColor("_SmokeColor1", _SmokeColor1);
		_material.SetColor("_SmokeColor2", _SmokeColor2);
	}

	public void ChangeTexture()
	{
		_material.SetTexture("_SmokeTex", SmokeTex);	
	}

	void OnDisable()
	{
#region Tweener Kill

		if (_tweenerSmoke != null && _tweenerSmoke.IsActive())
			_tweenerSmoke.Kill();

		_tweenerSmoke = null;

#endregion

		DisableBlend();
	}

	public void DisableBlend()
	{
		if (!_enable || _target == null)
			return;

		_target.material = _material = null;
		_enable = false;
	}

}
