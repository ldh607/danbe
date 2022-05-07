using UnityEngine;
using UnityEngine.UI;


[ExecuteInEditMode]
[AddComponentMenu("UI/Effects/ImageAdditiveOverlay")]
public class ImageAdditiveOverlay : MonoBehaviour
{
	
	public Image targetTexture;

	Image _overlayImg;
	Material _material;
	Shader _shader;
	RectTransform targetRect;
	RectTransform MaskArea;
	RectTransform myRect;

	Vector2 center;
	Vector2 rectRatio;
	Vector2 min;
	Vector2 max = Vector2.one;
	Vector2 size;

	Rect maskRect;
	Rect contentRect;

	bool _enable;

	[SerializeField]
	[Range(0f, 5.0f)]
	public float powerStrength;

	void OnEnable()
	{
		_overlayImg = GetComponent<Image>();

		myRect = GetComponent<RectTransform>();

        if (targetRect != null)
            targetRect = targetTexture.gameObject.GetComponent<RectTransform>();

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
		if (_overlayImg == null)
		{
			_overlayImg = GetComponent<Image>();
			if (_overlayImg == null)
				return false;
		}

		if (targetRect == null && targetTexture != null)
		{
            targetRect = targetTexture.gameObject.GetComponent<RectTransform>();
        }
		else if (targetTexture == null)
		{
			return false;
		}

		if (_shader == null)
			_shader = Shader.Find("AdditiveOverlay");

		if (_material == null || _material == _overlayImg.defaultMaterial)
			_overlayImg.material = _material = new Material(_shader);
		
		return true;
	}

	void Update()
	{
		SetMask();
	}

	public void SetMask()
	{
		if (!_enable)
			return;

		maskRect = MaskArea.rect;
		contentRect = myRect.rect;

		if (targetRect != null)
			maskRect = targetRect.rect;

		center = myRect.transform.InverseTransformPoint(MaskArea.transform.position);

		if (targetRect != null)
			center = myRect.transform.InverseTransformPoint(targetRect.transform.position);

		rectRatio = new Vector2(maskRect.width / contentRect.width, maskRect.height / contentRect.height);

		min = center;
		max = min;

		size = new Vector2(maskRect.width, maskRect.height) * 0.5f;

		min -= size;
		max += size;

		var RectratioWidth = (maskRect.width / contentRect.width) * 0.5f;
		var RectratioHeight = (maskRect.height / contentRect.height) * 0.5f;
		min = new Vector2(min.x / contentRect.width, min.y / contentRect.height) + new Vector2(RectratioWidth, RectratioHeight);
		max = new Vector2(max.x / contentRect.width, max.y / contentRect.height) + new Vector2(RectratioWidth, RectratioHeight);

#region MainTex
		var overlayUV = UnityEngine.Sprites.DataUtility.GetInnerUV(_overlayImg.overrideSprite);
		Vector2 overlaySize = new Vector2();
		overlaySize.Set((overlayUV.z - overlayUV.x), (overlayUV.w - overlayUV.y));

		_material.SetFloat("_MainTexScalingX", 1 / overlaySize.x);
		_material.SetFloat("_MainTexScalingY", 1 / overlaySize.y);

		_material.SetFloat("_MainTexUVMoveX", overlayUV.x / overlaySize.x);
		_material.SetFloat("_MainTexUVMoveY", overlayUV.y / overlaySize.y);

#endregion

#region TargetTex
		_material.SetTexture("_TargetTex", targetTexture.overrideSprite.texture);

		var inneruv = UnityEngine.Sprites.DataUtility.GetInnerUV(targetTexture.overrideSprite);
		var outeruv = UnityEngine.Sprites.DataUtility.GetOuterUV(targetTexture.overrideSprite);

		float innerSizeX = inneruv.z - inneruv.x;
		float innerSizeY = inneruv.w - inneruv.y;

		float reSizeX = innerSizeX / rectRatio.x;
		float reSizeY = innerSizeY / rectRatio.y;

		inneruv.x -= (reSizeX - innerSizeX) / 2.0f;
		inneruv.z += (reSizeX - innerSizeX) / 2.0f;

		inneruv.y -= (reSizeY - innerSizeY) / 2.0f;
		inneruv.w += (reSizeY - innerSizeY) / 2.0f;

		_material.SetFloat("_InnerUVx", inneruv.x);
		_material.SetFloat("_InnerUVy", inneruv.y);

		_material.SetFloat("_InnerSizeX", reSizeX);
		_material.SetFloat("_InnerSizeY", reSizeY);

		_material.SetVector("_OuterUV", outeruv);

		var outerSizeX = outeruv.z - outeruv.x;
		var outerSizeY = outeruv.w - outeruv.y;

		_material.SetFloat("_OuterSizeX", outerSizeX);
		_material.SetFloat("_OuterSizeY", outerSizeY);

		float moveX = min.x * reSizeX;
		float moveY = min.y * reSizeY;

		_material.SetFloat("_MoveX", moveX);
		_material.SetFloat("_MoveY", moveY);

		_material.SetFloat("_PowerStrength", powerStrength);
#endregion
	}

	void OnDisable()
	{
		DisableMask();
	}

	public void DisableMask()
	{
		if (!_enable || _overlayImg == null)
			return;

		_overlayImg.material = _material = null;
		_enable = false;
	}
}
