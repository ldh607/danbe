using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Sprites;


[ExecuteInEditMode]
[RequireComponent(typeof(Image))]
[AddComponentMenu("UI/Effects/CircleEffect")]
public class ImageCircle : MonoBehaviour
{
	[Range(-1, 1)] public float _Offset;
	[Range(0, 1)] public float _InOut;
	[Range(0, 1)] public float _Alpha;
	[Range(0, 1)] public float _Fading = 1;

	public AnimationCurve CircleCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
	public Ease CircleEaseType = Ease.OutQuad;

	public AnimationCurve AlphaCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
	public Ease AlphaEaseType = Ease.OutQuad;

	public float delay;
	public float duration;
	public float endValue;

	public float AlphaDelay;
	public float AlphaDuration;
	public float AlphaEndValue;

	public LoopType loopType = LoopType.Restart;
	public int loops = 1;
	public bool isRelative;
	public bool isFrom;

	public LoopType AlphaloopType = LoopType.Restart;
	public int Alphaloops = 1;
	public bool AlphaisRelative;
	public bool AlphaisFrom;

	Tweener _tweenerCircle;
	Tweener _tweenerAlpha;
	Image _target;
	Material _material;
	Shader _shader;

	bool _enable;


	void OnEnable()
	{
		_target = GetComponent<Image>();

		EnableCircle();
		ChangeValue();
#region for Tween

		_tweenerCircle = _material.DOFloat(endValue, "_Offset", duration);

		if (isFrom)
			_tweenerCircle.From(isRelative);
		else
			_tweenerCircle.SetRelative(isRelative);

		_tweenerCircle.SetLoops(loops, loopType);
		_tweenerCircle.SetDelay(delay);

		if (CircleEaseType == Ease.INTERNAL_Custom)
			_tweenerCircle.SetEase(CircleCurve);
#endregion

#region for AlphaTween

		_tweenerAlpha = _material.DOFloat(AlphaEndValue, "_Fading", AlphaDuration);

		if (AlphaisFrom)
			_tweenerAlpha.From(AlphaisRelative);
		else
			_tweenerAlpha.SetRelative(AlphaisRelative);

		_tweenerAlpha.SetLoops(Alphaloops, AlphaloopType);
		_tweenerAlpha.SetDelay(AlphaDelay);

		if (AlphaEaseType == Ease.INTERNAL_Custom)
			_tweenerAlpha.SetEase(AlphaCurve);

#endregion
	}

	void EnableCircle()
	{
		if (!SetupShader())
			return;

		_enable = true;

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
			_shader = Shader.Find("KUF/Circle");

		if (_material == null || _material == _target.defaultMaterial)
			_target.material = _material = new Material(_shader);

		var uv = DataUtility.GetOuterUV(_target.overrideSprite);
		Vector2 ResultVec = new Vector2();
		ResultVec.Set(1 / (uv.z - uv.x), 1 / (uv.w - uv.y));

		_material.SetFloat("_UvX", uv.x / (uv.z - uv.x));
		_material.SetFloat("_UvY", uv.y / (uv.w - uv.y));

		_material.SetFloat("_ScaleX", ResultVec.x);
		_material.SetFloat("_ScaleY", ResultVec.y);

		return true;

	}

	public void ChangeValue()
	{
		if (!_enable)
			return;

		_material.SetFloat("_Offset", _Offset);
		_material.SetFloat("_Alpha", _Alpha);
		_material.SetFloat("_InOut", _InOut);
		_material.SetFloat("_Fading", _Fading);
	}

	void OnDisable()
	{
#region Tween Kill
		if (_tweenerCircle != null && _tweenerCircle.IsActive())
			_tweenerCircle.Kill();

		_tweenerCircle = null;

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
