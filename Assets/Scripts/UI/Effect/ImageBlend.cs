using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.UI;
using DG.Tweening;


[ExecuteInEditMode]
[RequireComponent(typeof(Image))]
[AddComponentMenu("UI/Effects/ImageBlend")]
public class ImageBlend : MonoBehaviour
{

	public enum BlendMode
	{
		AlphaMask,
		NoAlphaMask
	}

	public enum MaskBlendMode
	{
		Default,
		Alpha
	}

	public Texture2D Mask;
	public float ScrollX;
	public float ScrollY;
	public float ScrollAlpha;
	public float _ScaleX;
	public float _ScaleY;
	public float _AddColorR;
	public float _AddColorG;
	public float _AddColorB;

	public AnimationCurve ScrollXCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
	public AnimationCurve ScrollYCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
	public AnimationCurve ScrollAlphaCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));

	public Ease XeaseType = Ease.OutQuad;
	public Ease YeaseType = Ease.OutQuad;
	public Ease AlphaEaseType = Ease.OutQuad;

	public float delay;
	public float duration;
	public float endXValue;
	public float endYValue;
	public float endAlphaValue;

	public LoopType loopTypeX = LoopType.Restart;
	public int loopsX = 1;
	public bool isRelative;
	public bool isFrom;

	public LoopType loopTypeY = LoopType.Restart;
	public int loopsY = 1;

	public LoopType loopTypeAlpha = LoopType.Restart;
	public int loopsAlpha = 1;

	bool _enable;

	Image _target;
	Material _material;
	Shader _shader;

	Tweener _tweenerX;
	Tweener _tweenerY;
	Tweener _tweenerAlpha;

	[SerializeField]
	public BlendMode blendMode;

	[SerializeField]
	public MaskBlendMode maskBlendMode;

	void OnEnable()
	{
		_target = GetComponent<Image>();

		EnableBlend();
		ChangeScroll();

		#region for Editor

		//Duration 수정 예정
		_tweenerX = _material.DOFloat(endXValue, "_ScrollX", duration);
		_tweenerY = _material.DOFloat(endYValue, "_ScrollY", duration);
		_tweenerAlpha = _material.DOFloat(endAlphaValue, "_ScrollAlpha", duration);

		if (isFrom)
		{
			_tweenerX.From(isRelative);
			_tweenerY.From(isRelative);
			_tweenerAlpha.From(isRelative);
		}
		else
		{
			_tweenerX.SetRelative(isRelative);
			_tweenerY.SetRelative(isRelative);
			_tweenerAlpha.SetRelative(isRelative);
		}

		//loop 수정 예정
		_tweenerX.SetLoops(loopsX, loopTypeX);
		_tweenerY.SetLoops(loopsY, loopTypeY);
		_tweenerAlpha.SetLoops(loopsAlpha, loopTypeAlpha);

		_tweenerX.SetDelay(delay);
		_tweenerY.SetDelay(delay);
		_tweenerAlpha.SetDelay(delay);

		if (XeaseType == Ease.INTERNAL_Custom)
			_tweenerX.SetEase(ScrollXCurve);
		else
			_tweenerX.SetEase(ScrollXCurve);
		
		if (YeaseType == Ease.INTERNAL_Custom)
			_tweenerY.SetEase(ScrollYCurve);
		else
			_tweenerY.SetEase(ScrollYCurve);

		if (AlphaEaseType == Ease.INTERNAL_Custom)
			_tweenerAlpha.SetEase(ScrollAlphaCurve);
		else
			_tweenerAlpha.SetEase(ScrollAlphaCurve);
		
		#endregion

//		_tweenerX.Pause();
//		_tweenerY.Pause();
//		_tweenerAlpha.Pause();

	}

	public void EnableBlend()
	{
		if (!SetupShader())
			return;

		_enable = true;

		ChangeBlendMode();
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
			_shader = Shader.Find("KUF/ImageBlend");

		if (_target.material.shader == _shader)
			_material = _target.material;

		if (_material == null || _material == _target.defaultMaterial)
			_target.material = _material = new Material(_shader);
		
		_material.SetTexture("_MaskTex", Mask);

		var uv = DataUtility.GetOuterUV(_target.overrideSprite);
		Vector2 ResultVec = new Vector2();
		ResultVec.Set(1 / (uv.z - uv.x), 1 / (uv.w - uv.y));

		_material.SetFloat("_MainUVnx", uv.x / (uv.z - uv.x));
		_material.SetFloat("_MainUVny", uv.y / (uv.w - uv.y));

		_material.SetFloat("_MaskUVx", ResultVec.x);
		_material.SetFloat("_MaskUVy", ResultVec.y);

		return true;
	}

	public void ChangeBlendMode()
	{
		if (!_enable)
			return;

		for (int i = (int)BlendMode.AlphaMask; i <= (int)BlendMode.NoAlphaMask; ++i)
		{
			var mode = (BlendMode)i;
			var modeName = string.Format("_{0}", mode.ToString().ToUpper());

			if (mode == blendMode)
			{
				_material.EnableKeyword(modeName);
			}
			else
			{
				_material.DisableKeyword(modeName);
			}
		}

	}


	public void ChangeScroll()
	{
		if (!_enable)
			return;

		_material.SetFloat("_ScrollX", ScrollX);
		_material.SetFloat("_ScrollY", ScrollY);
		_material.SetFloat("_ScrollAlpha", ScrollAlpha);
		_material.SetFloat("_ScaleX", _ScaleX);
		_material.SetFloat("_ScaleY", _ScaleY);
		_material.SetFloat("_AddColorR", _AddColorR);
		_material.SetFloat("_AddColorG", _AddColorG);
		_material.SetFloat("_AddColorB", _AddColorB);
	}

	public void ChangeTexture()
	{
		_material.SetTexture("_MaskTex", Mask);
	}

	void OnDisable()
	{

		#region for Editor

		if (_tweenerX != null && _tweenerX.IsActive())
			_tweenerX.Kill();

		if (_tweenerY != null && _tweenerY.IsActive())
			_tweenerY.Kill();

		if (_tweenerAlpha != null && _tweenerAlpha.IsActive())
			_tweenerAlpha.Kill();
		

		_tweenerX = null;
		_tweenerY = null;
		_tweenerAlpha = null;

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

	public void TweenPause()
	{
		_tweenerX.Pause();
		_tweenerY.Pause();
		_tweenerAlpha.Pause();
	}

	public void TweenPlay()
	{
		_tweenerX.Play();
		_tweenerY.Play();
		_tweenerAlpha.Play();
	}

}
