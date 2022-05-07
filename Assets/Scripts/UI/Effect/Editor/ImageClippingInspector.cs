using UnityEngine;
using UnityEditor;
using DG.DOTweenEditor.Core;
using DG.Tweening;


[CustomEditor(typeof(ImageClipping))]
public class ImageClippingInspector : Editor
{
	ImageClipping _clipping;

	void OnEnable()
	{
		_clipping = target as ImageClipping;
	}

	public override void OnInspectorGUI()
	{
		EditorGUIUtils.SetGUIStyles();

		GUILayout.BeginHorizontal();
		EditorGUIUtils.InspectorLogo();
		GUILayout.Label("IMAGE CLIPPING", EditorGUIUtils.sideLogoIconBoldLabelStyle);

		GUILayout.FlexibleSpace();
		if (GUILayout.Button("▲", EditorGUIUtils.btIconStyle))
			UnityEditorInternal.ComponentUtility.MoveComponentUp(_clipping);

		if (GUILayout.Button("▼", EditorGUIUtils.btIconStyle))
			UnityEditorInternal.ComponentUtility.MoveComponentDown(_clipping);

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

		GUILayout.BeginVertical("Box");

		EditorGUI.BeginChangeCheck();
		_clipping._ClipLeft = EditorGUILayout.Slider("Start Clip Left", _clipping._ClipLeft, 0, 1);
		_clipping._ClipRight = EditorGUILayout.Slider("Start Clip Right", _clipping._ClipRight, 0, 1);
		_clipping._ClipUp = EditorGUILayout.Slider("Start Clip Up", _clipping._ClipUp, 0, 1);
		_clipping._ClipDown = EditorGUILayout.Slider("Start Clip Down", _clipping._ClipDown, 0, 1);

		if (EditorGUI.EndChangeCheck())
			_clipping.ChangeValue();

		GUILayout.EndVertical();

		GUILayout.BeginVertical("Box");

		_clipping.ClipRightEaseType = EditorGUIUtils.FilteredEasePopup(_clipping.ClipRightEaseType);
		if (_clipping.ClipRightEaseType == Ease.INTERNAL_Custom)
			_clipping.ClipRightCurve = EditorGUILayout.CurveField(" Right Clipping Curve", _clipping.ClipRightCurve);

		_clipping.ClipRightDuration = EditorGUILayout.FloatField(" Right Clipping Duration", _clipping.ClipRightDuration);
		if (_clipping.ClipRightDuration < 0)
			_clipping.ClipRightDuration = 0;

		_clipping.ClipRightDelay = EditorGUILayout.FloatField(" Right Clipping Delay", _clipping.ClipRightDelay);
		if (_clipping.ClipRightDelay < 0)
			_clipping.ClipRightDelay = 0;

		_clipping.ClipRightLoop = EditorGUILayout.IntField(new GUIContent("Loop", "Set to -1 for infinite loops"), _clipping.ClipRightLoop);
		if (_clipping.ClipRightLoop < -1)
			_clipping.ClipRightLoop = -1;
		if (_clipping.ClipRightLoop > 1 || _clipping.ClipRightLoop == -1)
			_clipping.ClipRightLoopType = (LoopType)EditorGUILayout.EnumPopup(" Loop Type", _clipping.ClipRightLoopType);

		_clipping.ClipLeftEaseType = EditorGUIUtils.FilteredEasePopup(_clipping.ClipLeftEaseType);
		if (_clipping.ClipLeftEaseType == Ease.INTERNAL_Custom)
			_clipping.ClipLeftCurve = EditorGUILayout.CurveField(" Left Clipping Curve", _clipping.ClipLeftCurve);

		_clipping.ClipLeftDuration = EditorGUILayout.FloatField(" Left Clipping Duration", _clipping.ClipLeftDuration);
		if (_clipping.ClipLeftDuration < 0)
			_clipping.ClipLeftDuration = 0;

		_clipping.ClipLeftDelay = EditorGUILayout.FloatField(" Left Clipping Delay", _clipping.ClipLeftDelay);
		if (_clipping.ClipLeftDelay < 0)
			_clipping.ClipLeftDelay = 0;

		_clipping.ClipLeftLoop = EditorGUILayout.IntField(new GUIContent("Loop", "Set to -1 for infinite loops"), _clipping.ClipLeftLoop);
		if (_clipping.ClipLeftLoop < -1)
			_clipping.ClipLeftLoop = -1;
		if (_clipping.ClipLeftLoop > 1 || _clipping.ClipLeftLoop == -1)
			_clipping.ClipLeftLoopType = (LoopType)EditorGUILayout.EnumPopup(" Loop Type", _clipping.ClipLeftLoopType);
		
		EditorGUILayout.BeginVertical("Box");

		GUIEndValue();

		EditorGUILayout.EndVertical();

		GUILayout.EndVertical();
	}

	void GUIEndValue()
	{
		EditorGUILayout.BeginVertical("Box");

		_clipping.ClipRightEndValue = EditorGUILayout.FloatField("Right Clipping End Value", _clipping.ClipRightEndValue);
		_clipping.ClipLeftEndValue = EditorGUILayout.FloatField("Left Clipping End Value", _clipping.ClipLeftEndValue);

		EditorGUILayout.EndVertical();

	}
}
