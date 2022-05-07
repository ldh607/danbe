using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Sprites;
using DG.Tweening;


[ExecuteInEditMode]
[RequireComponent(typeof(Image))]
[AddComponentMenu("UI/Effects/EdgeFade")]
public class ImageEdgeFade : MonoBehaviour
{
	[Range(0, 1)] public float _Alpha = 1f;
	[Range(0f, 1f)] public float _Offset;
	[Range(0f, 1f)] public float _ClipLeft;
	[Range(0f, 1f)] public float _ClipRight;
	[Range(0f, 1f)] public float _ClipUp;
	[Range(0f, 1f)] public float _ClipDown;

#region ClipLeft

	public AnimationCurve cLeftCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
	public Ease cLeftEaseType = Ease.OutQuad;

	public float cLeftDelay;
	public float cLeftDuration;
	public float cLeftEndValue;

	public LoopType cLeftLoopType = LoopType.Restart;
	public int cLeftLoops = 1;
	public bool cLeftIsRelative;
	public bool cLeftIsFrom;

	Tweener _cLeftTweener;

#endregion

#region ClipRight

	public AnimationCurve cRightCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
	public Ease cRightEaseType = Ease.OutQuad;

	public float cRightDelay;
	public float cRightDuration;
	public float cRightEndValue;

	public LoopType cRightLoopType = LoopType.Restart;
	public int cRightLoops = 1;
	public bool cRightIsRelative;
	public bool cRightIsFrom;

	Tweener _cRightTweener;

#endregion

	Image _target;
	Material _material;
	Shader _shader;

	bool _enable;


	void OnEnable()
	{
		_target = GetComponent<Image>();
		EnableEdgeFade();
		ChangeValue();

#region for Tween

		//Clip Left Tween
		_cLeftTweener = _material.DOFloat(1 - cLeftEndValue, "_ClipLeft", cLeftDuration);

		if (cLeftIsFrom)
			_cLeftTweener.From(cLeftIsRelative);
		else
			_cLeftTweener.SetRelative(cLeftIsRelative);

		_cLeftTweener.SetLoops(cLeftLoops, cLeftLoopType);
		_cLeftTweener.SetDelay(cLeftDelay);

		if (cLeftEaseType == Ease.INTERNAL_Custom)
			_cLeftTweener.SetEase(cLeftCurve);

		//Clip Right Tween
		_cRightTweener = _material.DOFloat(1 - cRightEndValue, "_ClipRight", cRightDuration);

		if (cRightIsFrom)
			_cRightTweener.From(cRightIsRelative);
		else
			_cRightTweener.SetRelative(cRightIsRelative);

		_cRightTweener.SetLoops(cRightLoops, cRightLoopType);
		_cRightTweener.SetDelay(cRightDelay);

		if (cRightEaseType == Ease.INTERNAL_Custom)
			_cRightTweener.SetEase(cRightCurve);

#endregion
	}

	void EnableEdgeFade()
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
			_shader = Shader.Find("KUF/EdgeFade");

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

	void OnDisable()
	{
#region Tween Kill

		if (_cLeftTweener != null && _cLeftTweener.IsActive())
			_cLeftTweener.Kill();

		if (_cRightTweener != null && _cRightTweener.IsActive())
			_cRightTweener.Kill();

		_cLeftTweener = null;
		_cRightTweener = null;

#endregion
		DisableEdgeFade();
	}

	public void DisableEdgeFade()
	{
		if (!_enable || _target == null)
			return;

		_target.material = _material = null;
		_enable = false;
	}

	public void ChangeValue()
	{
		if (!_enable)
			return;
		
		_material.SetFloat("_Alpha", 1 - _Alpha);
		_material.SetFloat("_Offset", _Offset);
		_material.SetFloat("_ClipLeft", 1 - _ClipLeft);
		_material.SetFloat("_ClipRight", 1 - _ClipRight);
		_material.SetFloat("_ClipUp", 1 - _ClipUp);
		_material.SetFloat("_ClipDown", 1 - _ClipDown);
	}

}
