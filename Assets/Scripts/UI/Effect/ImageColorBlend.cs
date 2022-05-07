using UnityEngine;
using UnityEngine.UI;


[ExecuteInEditMode]
[AddComponentMenu("UI/Effects/ColorBlend")]
public class ImageColorBlend : MonoBehaviour
{
	public bool ownComponent;
	public bool isChildShaderApply;

	bool _enable;

	MaskableGraphic _target;
	Material _material;
	static Shader _shader;


	public enum BlendMode
	{
		GrayScale,
		Luminosity,
		Saturation,
		Color,
		Additive,
		Subtract,
		Multiply,
		Divide,
		Difference,
		HardMix,
		Burn,
		Dodge,
		Overlay,
		Screen,
		Darken,
		Darker,
		Lighten,
		Lighter,
		HardLight,
		SoftLight,
		PinLight,
		VividLight
	}

	[SerializeField]
	public BlendMode blendMode;

	[SerializeField]
	public float blendStrength;


	void Awake()
	{
		var other = GetComponents<ImageColorBlend>();
		if (other.Length > 1)
		{
			if (Application.isPlaying)
				Destroy(this);
			else
				DestroyImmediate(this);

			return;
		}
		ownComponent = (gameObject.GetComponent<MaskableGraphic>() != null);
	}
	
	void OnEnable()
	{
		if (ownComponent)
			EnableBlend();
		else
			ChildBlend();	
	}

	public void EnableBlend()
	{
		if (!SetupShader())
			return;

		_enable = true;

		ChangeBlendMode();
		ChangeBlendStrength();

		if (isChildShaderApply)
			ChilderenShaderSetup();
	}

	public void ChildBlend()
	{
		if (_shader == null)
			_shader = Shader.Find("UI/Color Blend");

		_material = new Material(_shader);
		_enable = true;

		ChangeBlendMode();
		ChangeBlendStrength();

		if (isChildShaderApply)
			ChilderenShaderSetup();
	}

	bool SetupShader()
	{
		if (_target == null)
		{
			_target = GetComponent<MaskableGraphic>();
			if (_target == null)
			{
				ShowError();
				return false;
			}
		}

		if (_shader == null)
			_shader = Shader.Find("UI/Color Blend");
			
		if (_material == null || _material == _target.defaultMaterial)
			_target.material = _material = new Material(_shader);

		return true;
	}

	public void ChilderenShaderSetup()
	{
		var childGo = transform.GetComponentsInChildren<MaskableGraphic>();

		for (int i = 0; i < childGo.Length; ++i)
		{

			childGo[i].material = new Material(_shader);

			ChangeBlendModeChild(childGo[i].material);
			ChangeBlendStrengthChild(childGo[i].material);
		}
	}

	public void ChangeBlendMode()
	{
		if (!_enable)
			return;
		
		for (int i = (int)BlendMode.GrayScale; i <= (int)BlendMode.VividLight; ++i)
		{
			var mode = (BlendMode)i;
			var modeName = string.Format("_{0}", mode.ToString().ToUpper());

			if (mode == blendMode)
				_material.EnableKeyword(modeName);
			else
				_material.DisableKeyword(modeName);
		}

		if (isChildShaderApply)
		{
			var childGo = transform.GetComponentsInChildren<MaskableGraphic>();

			for (int i = 0; i < childGo.Length; ++i)
				ChangeBlendModeChild(childGo[i].material);
		}
	}

	public void ChangeBlendStrength()
	{
		if (!_enable)
			return;
		
		_material.SetFloat("_BlendStrength", blendStrength);

		if (isChildShaderApply)
		{
			var childGo = transform.GetComponentsInChildren<MaskableGraphic>();

			for (int i = 0; i < childGo.Length; ++i)
				ChangeBlendStrengthChild(childGo[i].material);
		}
	}


#region ChildShaderSetup

	public void ChangeBlendModeChild(Material m)
	{
		for (int i = (int)BlendMode.GrayScale; i <= (int)BlendMode.VividLight; ++i)
		{
			var mode = (BlendMode)i;
			var modeName = string.Format("_{0}", mode.ToString().ToUpper());

			if (mode == blendMode)
				m.EnableKeyword(modeName);
			else
				m.DisableKeyword(modeName);
		}
	}

	public void ChangeBlendStrengthChild(Material m)
	{
		m.SetFloat("_BlendStrength", blendStrength);
	}
#endregion

	void OnDisable()
	{
		DisableBlend();
	}

	public void DisableBlend()
	{
		if ((!_enable || _target == null) && !isChildShaderApply)
			return;

		if (_target != null)
			_target.material = _material = null;
		
		_enable = false;

		if (isChildShaderApply)
		{
			var childGo = transform.GetComponentsInChildren<MaskableGraphic>();

			for (int i = 0; i < childGo.Length; ++i)
			{
				childGo[i].material = null;
			}
		}
	}

	public void ShowError()
	{
		Debug.LogWarning("Require [Image] or [RawImage] component.");
		enabled = false;
	}
}
