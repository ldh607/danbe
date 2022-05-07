using UnityEngine;
using UnityEngine.UI;


[ExecuteInEditMode]
[RequireComponent(typeof(Image))]
[AddComponentMenu("UI/Effects/Flow")]
public class ImageFlow : MonoBehaviour
{
	public Texture2D _FlowTex;

	public float _AddColorR;
	public float _AddColorG;
	public float _AddColorB;

	public float _ScrollX;
	public float _ScrollY;

	Image _target;
	Material _material;
	Shader _shader;

	bool _enable;

	// Update is called once per frame
	void Update()
	{
		if (_enable)
		{
			_material.SetTexture("_FlowTex", _FlowTex);

			_material.SetFloat("_AddColorR", _AddColorR);
			_material.SetFloat("_AddColorG", _AddColorG);
			_material.SetFloat("_AddColorB", _AddColorB);

			_material.SetFloat("_ScrollX", _ScrollX);
			_material.SetFloat("_ScrollY", _ScrollY);
		}
	}

	void OnEnable()
	{
		_target = GetComponent<Image>();

		EnableFlow();
	}

	void EnableFlow()
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
				return false;
		}

		if (_shader == null)
			_shader = Shader.Find("KUF/Flow");
		
		if (_material == null || _material == _target.defaultMaterial)
			_target.material = _material = new Material(_shader);

		_material.SetTexture("_FlowTex", _FlowTex);

		return true;

	}

	public void ChangeValue()
	{
		if (!_enable)
			return;

		_material.SetFloat("_AddColorR", _AddColorR);
		_material.SetFloat("_AddColorG", _AddColorG);
		_material.SetFloat("_AddColorB", _AddColorB);

		_material.SetFloat("_ScrollX", _ScrollX);
		_material.SetFloat("_ScrollY", _ScrollY);

	}

	public void ChangeTexture()
	{
		_material.SetTexture("_FlowTex", _FlowTex);
	}

	void OnDisable()
	{
		DisableFlow();
	}

	void DisableFlow()
	{
		if (!_enable || _target == null)
			return;

		_target.material = _material = null;
		_enable = false;
	}

}
