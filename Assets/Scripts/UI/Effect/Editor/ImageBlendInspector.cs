using UnityEngine;
using UnityEditor;
using DG.DOTweenEditor.Core;
using DG.Tweening;


[CustomEditor(typeof(ImageBlend))]
public class ImageBlendInspector : Editor
{
	ImageBlend _blender;

	void OnEnable()
	{
		_blender = target as ImageBlend;
	}

	public override void OnInspectorGUI()
	{
		EditorGUIUtils.SetGUIStyles();

		GUILayout.BeginHorizontal();
		EditorGUIUtils.InspectorLogo();
		GUILayout.Label("IMAGE BLEND", EditorGUIUtils.sideLogoIconBoldLabelStyle);

		// Up-down buttons
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("▲", EditorGUIUtils.btIconStyle))
			UnityEditorInternal.ComponentUtility.MoveComponentUp(_blender);
		
		if (GUILayout.Button("▼", EditorGUIUtils.btIconStyle))
			UnityEditorInternal.ComponentUtility.MoveComponentDown(_blender);
		GUILayout.EndHorizontal();

		if (Application.isPlaying)
		{
			GUILayout.Space(8);
			GUILayout.Label("Animation Editor disabled while in play mode", EditorGUIUtils.wordWrapLabelStyle);
			GUILayout.Space(4);
			GUILayout.Label("NOTE: when using DOPlayNext, the sequence is determined by the DOTweenAnimation Components order in the target GameObject's Inspector", EditorGUIUtils.wordWrapLabelStyle);
			GUILayout.Space(10);
			return;
		}

		EditorGUILayout.BeginVertical("Box");

		EditorGUI.BeginChangeCheck();
		_blender.blendMode = (ImageBlend.BlendMode)EditorGUILayout.EnumPopup("    Blend Mode", _blender.blendMode);
		if (EditorGUI.EndChangeCheck())
		{
			_blender.ChangeBlendMode();
		}

		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();

		EditorGUI.BeginChangeCheck();

		_blender.Mask = (Texture2D)EditorGUILayout.ObjectField(_blender.Mask, typeof(Texture2D), false,
			GUILayout.Width(180), GUILayout.Height(180));
		if (EditorGUI.EndChangeCheck())
		{
			_blender.ChangeTexture();
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();

		EditorGUILayout.EndVertical();

		EditorGUILayout.BeginVertical("Box");
		EditorGUI.BeginChangeCheck();
		_blender.ScrollX = EditorGUILayout.FloatField("   Start Scroll X", _blender.ScrollX);
		_blender.ScrollY = EditorGUILayout.FloatField("   Start Scroll Y", _blender.ScrollY);
		_blender.ScrollAlpha = EditorGUILayout.FloatField("   Start Scroll Alpha", _blender.ScrollAlpha);
		_blender._ScaleX = EditorGUILayout.FloatField("   Scale X", _blender._ScaleX);
		_blender._ScaleY = EditorGUILayout.FloatField("   Sclae Y", _blender._ScaleY);
		_blender._AddColorR = EditorGUILayout.FloatField("   +Color R", _blender._AddColorR);
		_blender._AddColorG = EditorGUILayout.FloatField("   +Color G", _blender._AddColorG);
		_blender._AddColorB = EditorGUILayout.FloatField("   +Color B", _blender._AddColorB);
		if (EditorGUI.EndChangeCheck())
		{
			_blender.ChangeScroll();
		}
		EditorGUILayout.EndVertical();

		EditorGUILayout.BeginVertical("Box");

		_blender.XeaseType = EditorGUIUtils.FilteredEasePopup(_blender.XeaseType);
		if (_blender.XeaseType == Ease.INTERNAL_Custom)
			_blender.ScrollXCurve = EditorGUILayout.CurveField("   Scroll X Curve", _blender.ScrollXCurve);

		_blender.YeaseType = EditorGUIUtils.FilteredEasePopup(_blender.YeaseType);
		if (_blender.YeaseType == Ease.INTERNAL_Custom)
			_blender.ScrollYCurve = EditorGUILayout.CurveField("   Scroll Y Curve", _blender.ScrollYCurve);

		_blender.duration = EditorGUILayout.FloatField("Duration", _blender.duration);
		if (_blender.duration < 0)
			_blender.duration = 0;
		
		_blender.delay = EditorGUILayout.FloatField("Delay", _blender.delay);
		if (_blender.delay < 0)
			_blender.delay = 0;
		
		_blender.loopsX = EditorGUILayout.IntField(new GUIContent("Loops X", "Set to -1 for infinite loops"), _blender.loopsX);
		if (_blender.loopsX < -1)
			_blender.loopsX = -1;
		if (_blender.loopsX > 1 || _blender.loopsX == -1)
			_blender.loopTypeX = (LoopType)EditorGUILayout.EnumPopup("   Loop Type X", _blender.loopTypeX);

		_blender.loopsY = EditorGUILayout.IntField(new GUIContent("Loops Y", "Set to -1 for infinite loops"), _blender.loopsY);
		if (_blender.loopsY < -1)
			_blender.loopsY = -1;
		if (_blender.loopsY > 1 || _blender.loopsY == -1)
			_blender.loopTypeY = (LoopType)EditorGUILayout.EnumPopup("   Loop Type Y", _blender.loopTypeY);


		

		GUIEndValue();

		EditorGUILayout.EndVertical();

#region Mask Coclor Blend

		EditorGUILayout.BeginVertical("Box");

		EditorGUI.BeginChangeCheck();
		_blender.maskBlendMode = (ImageBlend.MaskBlendMode)EditorGUILayout.EnumPopup("Mask Blend Mode", _blender.maskBlendMode);
		if (EditorGUI.EndChangeCheck())
		{
			_blender.ChangeBlendMode();
		}
			
		if (_blender.maskBlendMode == ImageBlend.MaskBlendMode.Alpha)
		{
			_blender.AlphaEaseType = EditorGUIUtils.FilteredEasePopup(_blender.AlphaEaseType);
			if (_blender.AlphaEaseType == Ease.INTERNAL_Custom)
				_blender.ScrollAlphaCurve = EditorGUILayout.CurveField("   Scroll Alpha Curve", _blender.ScrollAlphaCurve);

			GUILayout.BeginHorizontal();
			_blender.endAlphaValue = EditorGUILayout.FloatField("    End Scroll Alpha Value", _blender.endAlphaValue);
			GUILayout.EndHorizontal();

		}

		_blender.loopsAlpha = EditorGUILayout.IntField(new GUIContent("Loops Alpha Type", "Set to -1 for infinite loops"), _blender.loopsAlpha);
		if (_blender.loopsAlpha < -1)
			_blender.loopsAlpha = -1;
		if (_blender.loopsAlpha > 1 || _blender.loopsAlpha == -1)
			_blender.loopTypeAlpha = (LoopType)EditorGUILayout.EnumPopup("   Loop Type Alpha", _blender.loopTypeAlpha);

		EditorGUILayout.EndVertical();

#endregion
	}

	void GUIEndValue()
	{

		EditorGUILayout.BeginVertical("Box");
		GUILayout.BeginHorizontal();
		GUIToFromButton();
		_blender.endXValue = EditorGUILayout.FloatField("End Scroll X Value", _blender.endXValue);
		_blender.endYValue = EditorGUILayout.FloatField("End Scroll Y Value", _blender.endYValue);
		GUILayout.EndHorizontal();
		EditorGUILayout.EndVertical();
	}

	void GUIToFromButton()
	{
		
	}

}
