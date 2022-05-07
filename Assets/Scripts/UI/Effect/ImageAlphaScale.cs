using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.UI;
using DG.Tweening;


[ExecuteInEditMode]
[RequireComponent(typeof(Image))]
[AddComponentMenu("UI/Effects/AlphaScale")]
public class ImageAlphaScale : MonoBehaviour
{
	[Range(-1, 1)] public float _Inside = 0f;
	[Range(0, 1)] public float _Alpha = 1f;
	[Range(-10, 10)] public float _Tv1 = 1f;
	[Range(-10, 10)] public float _Tv2 = 1f;


	public AnimationCurve AScurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
	public Ease ASeaseType = Ease.OutQuad;

	public AnimationCurve AlphaCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
	public Ease AlphaEaseType = Ease.OutQuad;

	public float delay;
	public float duration;
	public float endASvalue;
	public float endAlphaValue;

	public LoopType loopType = LoopType.Restart;
	public int loops = 1;
	public bool isRelative;
	public bool isFrom;

	Tweener _tweenerAS;
	Tweener _tweenerAlpha;
	Image _target;
	Material _material;
	Shader _shader;

	bool _enable;

	void Update()
	{
//		if (_enable)
//		{
//			_material.SetFloat("_Inside", _Inside);
//			_material.SetFloat("_Alpha", _Alpha);
//			_material.SetFloat("_Tv1", _Tv1);
//			_material.SetFloat("_Tv2", _Tv2);
//		}
	}

	void OnEnable()
	{
		_target = GetComponent<Image>();

		EnableAS();

#region for Editor

		_tweenerAS = _material.DOFloat(endASvalue, "_Inside", duration);
		_tweenerAlpha = _material.DOFloat(endAlphaValue, "_Alpha", duration);
			
		if (isFrom)
		{
			_tweenerAS.From(isRelative);
			_tweenerAlpha.From(isRelative);
		}
		else
		{
			_tweenerAS.SetRelative(isRelative);
			_tweenerAlpha.SetRelative(isRelative);
		}

		_tweenerAS.SetLoops(loops, loopType);
		_tweenerAlpha.SetLoops(loops, loopType);

		_tweenerAS.SetDelay(delay);
		_tweenerAlpha.SetDelay(delay);

		if (ASeaseType == Ease.INTERNAL_Custom)
			_tweenerAS.SetEase(AScurve);

		if (AlphaEaseType == Ease.INTERNAL_Custom)
			_tweenerAlpha.SetEase(AlphaCurve);
#endregion

		_tweenerAS.Pause();
		_tweenerAlpha.Pause();
	}

	void EnableAS()
	{
		if (!SetupShader())
			return;

		_enable = true;
		ChangeValue();
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
			_shader = Shader.Find("KUF/AlphaScale");

		if (_material == null || _material == _target.defaultMaterial)
			_target.material = _material = new Material(_shader);

		var uv = DataUtility.GetOuterUV(_target.overrideSprite);
		Vector2 ResultVec = new Vector2();
		ResultVec.Set(1 / (uv.z - uv.x), 1 / (uv.w - uv.y));

		_material.SetFloat("_AtlasInitUvX", ((uv.z - uv.x) / 2) + uv.x);
		_material.SetFloat("_AtlasInitUvY", ((uv.w - uv.y) / 2) + uv.y);

		_material.SetFloat("_MainAtlasUvX", ResultVec.x);
		_material.SetFloat("_MainAtlasUvY", ResultVec.y);

		return true;
	}

	public void ChangeValue()
	{
		if (_enable)
		{
			_material.SetFloat("_Inside", _Inside);
			_material.SetFloat("_Alpha", _Alpha);
			_material.SetFloat("_Tv1", _Tv1);
			_material.SetFloat("_Tv2", _Tv2);
		}
	}

	void OnDisable()
	{
#region Tweener Kill

		if (_tweenerAS != null && _tweenerAS.IsActive())
			_tweenerAS.Kill();

		if (_tweenerAlpha != null && _tweenerAlpha.IsActive())
			_tweenerAlpha.Kill();

		_tweenerAS = null;
		_tweenerAlpha = null;

#endregion

		DisableAS();
	}

	void DisableAS()
	{
		if (!_enable || _target == null)
			return;

		_target.material = _material = null;
		_enable = false;
	}

}
