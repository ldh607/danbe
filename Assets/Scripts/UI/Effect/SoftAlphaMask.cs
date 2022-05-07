using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Sprites;


[ExecuteInEditMode]
[AddComponentMenu("UI/Effects/SoftAlphaMask")]
public class SoftAlphaMask : MonoBehaviour
{
	Image _target;
	Material _material;
	Shader _shader;

	public RectTransform MaskArea;
	RectTransform myRect;

	public RectTransform maskScalingRect;

	public Texture AlphaMask;

	[Range(0, 1)]
	public float CutOff = 0;

	public bool HardBlend = false;

	public bool FlipAlphaMask = false;

	Vector2 AlphaUV;

	Vector2 min;
	Vector2 max = Vector2.one;
	Vector2 p;
	Vector2 siz;

	Rect maskRect;
	Rect contentRect;

	Vector2 centre;

	bool _enable;

	void OnEnable()
	{
		_target = GetComponent<Image>();

		myRect = GetComponent<RectTransform>();
		if (!MaskArea)
			MaskArea = myRect;

		EnableMask();
		SetMask();
	}


	void EnableMask()
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
			_shader = Shader.Find("KUF/SoftAlphaMask");

		if (_material == null || _material == _target.defaultMaterial)
			_target.material = _material = new Material(_shader);

		return true;

	}

#if UNITY_EDITOR
	void Update()
	{
		if (!Application.isPlaying)
			SetMask();
	}
#endif

	public void SetMask()
	{
		if (!_enable)
			return;

		maskRect = MaskArea.rect;
		contentRect = myRect.rect;


		if (maskScalingRect != null)
			maskRect = maskScalingRect.rect;

		centre = myRect.transform.InverseTransformPoint(MaskArea.transform.position);

		if (maskScalingRect != null)
			centre = myRect.transform.InverseTransformPoint(maskScalingRect.transform.position);

		AlphaUV = new Vector2(maskRect.width / contentRect.width, maskRect.height / contentRect.height);

		min = centre;
		max = min;

		siz = new Vector2(maskRect.width, maskRect.height) * .5f;

		min -= siz;
		max += siz;

		min = new Vector2(min.x / contentRect.width, min.y / contentRect.height) + new Vector2(.5f, .5f);
		max = new Vector2(max.x / contentRect.width, max.y / contentRect.height) + new Vector2(.5f, .5f);

		_material.SetFloat("_HardBlend", HardBlend ? 1 : 0);

		_material.SetVector("_Min", min);
		_material.SetVector("_Max", max);

		_material.SetTexture("_AlphaMask", AlphaMask);
		_material.SetInt("_FlipAlphaMask", FlipAlphaMask ? 1 : 0);

		_material.SetVector("_AlphaUV", AlphaUV);

		_material.SetFloat("_CutOff", CutOff);

		var uv = DataUtility.GetOuterUV(_target.overrideSprite);

		float cal_1 = uv.z - uv.x;
		float cal_2 = uv.w - uv.y;

		_material.SetFloat("_UVx", uv.x / cal_1);
		_material.SetFloat("_UVy", uv.y / cal_2);

		_material.SetFloat("_ScaleX", 1 / cal_1);
		_material.SetFloat("_ScaleY", 1 / cal_2);

	}

	void OnDisable()
	{
		DisableMask();
	}

	public void DisableMask()
	{
		if (!_enable || _target == null)
			return;

		_target.material = _material = null;
		_enable = false;
	}
}
