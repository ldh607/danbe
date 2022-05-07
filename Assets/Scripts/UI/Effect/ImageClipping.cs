using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Sprites;
using DG.Tweening;


[ExecuteInEditMode]
[RequireComponent(typeof(Image))]
[AddComponentMenu("UI/Effects/Clipping")]
public class ImageClipping : MonoBehaviour
{
	[Range(0f, 1f)] public float _ClipLeft = 0f;
	[Range(0f, 1f)] public float _ClipRight = 0f;
	[Range(0f, 1f)] public float _ClipUp = 0f;
	[Range(0f, 1f)] public float _ClipDown = 0f;


#region Right
	public AnimationCurve ClipRightCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
	public Ease ClipRightEaseType = Ease.OutQuad;

	public float ClipRightDelay;
	public float ClipRightDuration;
	public float ClipRightEndValue;

	public LoopType ClipRightLoopType = LoopType.Restart;
	public int ClipRightLoop = 1;
	public bool isClipRightRelative;
	public bool isClipRightFrom;
#endregion

#region Left
	public AnimationCurve ClipLeftCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
	public Ease ClipLeftEaseType = Ease.OutQuad;

	public float ClipLeftDelay;
	public float ClipLeftDuration;
	public float ClipLeftEndValue;

	public LoopType ClipLeftLoopType = LoopType.Restart;
	public int ClipLeftLoop = 1;
	public bool isClipLeftRelative;
	public bool isClipLeftFrom;
#endregion

#region Down
    public AnimationCurve ClipDownCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
    public Ease ClipDownEaseType = Ease.OutQuad;

    public float ClipDownDelay;
    public float ClipDownDuration;
    public float ClipDownEndValue;

    public LoopType ClipDownLoopType = LoopType.Restart;
    public int ClipDownLoop = 1;
    public bool isClipDownRelative;
    public bool isClipDownFrom;
#endregion

    Tweener _clipRightTweener;
	Tweener _clipLeftTweener;
    Tweener _clipDownTweener;
	Image _target;
	Material _material;
	Shader _shader;
	bool _enable;


	void OnEnable()
	{
		_target = GetComponent<Image>();

		EnableClipping();

#region Setting Tweener
		_clipRightTweener = _material.DOFloat(1 - ClipRightEndValue, "_ClipRight", ClipRightDuration);

		if (isClipRightFrom)
			_clipRightTweener.From(isClipRightRelative);
		else
			_clipRightTweener.SetRelative(isClipRightRelative);

		_clipRightTweener.SetLoops(ClipRightLoop, ClipRightLoopType);
		_clipRightTweener.SetDelay(ClipRightDelay);

		if (ClipRightEaseType == Ease.INTERNAL_Custom)
			_clipRightTweener.SetEase(ClipRightCurve);

		_clipLeftTweener = _material.DOFloat(1 - ClipLeftEndValue, "_ClipLeft", ClipLeftDuration);

		if (isClipLeftFrom)
			_clipLeftTweener.From(isClipLeftRelative);
		else
			_clipLeftTweener.SetRelative(isClipLeftRelative);

		_clipLeftTweener.SetLoops(ClipLeftLoop, ClipLeftLoopType);
		_clipLeftTweener.SetDelay(ClipLeftDelay);

		if (ClipLeftEaseType == Ease.INTERNAL_Custom)
			_clipLeftTweener.SetEase(ClipLeftCurve);

        _clipDownTweener = _material.DOFloat(1 - ClipDownEndValue, "_ClipLeft", ClipDownDuration);

        if (isClipDownFrom)
            _clipDownTweener.From(isClipDownRelative);
        else
            _clipDownTweener.SetRelative(isClipDownRelative);

        _clipDownTweener.SetLoops(ClipDownLoop, ClipDownLoopType);
        _clipDownTweener.SetDelay(ClipDownDelay);

        if (ClipDownEaseType == Ease.INTERNAL_Custom)
            _clipDownTweener.SetEase(ClipDownCurve);
#endregion
    }

    void EnableClipping()
	{
		if (!SetupShader())
			return;

		_enable = true;

		_material.SetFloat("_ClipLeft", 1 - _ClipLeft);
		_material.SetFloat("_ClipRight", 1 - _ClipRight);
		_material.SetFloat("_ClipUp", 1 - _ClipUp);
		_material.SetFloat("_ClipDown", 1 - _ClipDown);

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
			_shader = Shader.Find("KUF/ImageClipping");

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

		_material.SetFloat("_ClipLeft", 1 - _ClipLeft);
		_material.SetFloat("_ClipRight", 1 - _ClipRight);
		_material.SetFloat("_ClipUp", 1 - _ClipUp);
		_material.SetFloat("_ClipDown", 1 - _ClipDown);
	}

	public void TweenReset()
	{
		OnEnable();
	}

	public void TweenPause()
	{
		_clipRightTweener.Pause();
		_clipLeftTweener.Pause();
        _clipDownTweener.Pause();
	}

	public void TweenPlay()
	{
		_clipRightTweener.Restart(true);
		_clipLeftTweener.Restart(true);
        _clipDownTweener.Restart(true);
	}

	void OnDisable()
	{
#region Tweener Kill
		if (_clipRightTweener != null && _clipRightTweener.IsActive())
			_clipRightTweener.Kill();

		if (_clipLeftTweener != null && _clipLeftTweener.IsActive())
			_clipLeftTweener.Kill();

        if (_clipDownTweener != null && _clipDownTweener.IsActive())
            _clipDownTweener.Kill();

        _clipRightTweener = null;
		_clipLeftTweener = null;
        _clipDownTweener = null;
#endregion
		DisableClipping();
	}

	void DisableClipping()
	{
		if (!_enable || _target == null)
			return;

		_target.material = _material = null;
		_enable = false;
	}
}
