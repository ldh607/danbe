using UnityEngine;
using System.Collections;
using UnityEditor;
using DG.DOTweenEditor.Core;
using DG.Tweening;


[CustomEditor(typeof(ImageEdgeFade))]
public class ImageEdgeFadeInspector : Editor
{

	ImageEdgeFade _edgeFade;

	// Use this for initialization
	void Start()
	{
	}

	void OnEnable()
	{
		_edgeFade = target as ImageEdgeFade;
	}

	public override void OnInspectorGUI()
	{
		EditorGUIUtils.SetGUIStyles();

		GUILayout.BeginHorizontal();
		GUILayout.Label("IMAGE EDGE FADE", EditorGUIUtils.sideLogoIconBoldLabelStyle);

		GUILayout.FlexibleSpace();
		if (GUILayout.Button("▲", EditorGUIUtils.btIconStyle))
			UnityEditorInternal.ComponentUtility.MoveComponentUp(_edgeFade);

		if (GUILayout.Button("▼", EditorGUIUtils.btIconStyle))
			UnityEditorInternal.ComponentUtility.MoveComponentDown(_edgeFade);
		GUILayout.EndHorizontal();


		GUILayout.BeginVertical("Box");

		EditorGUI.BeginChangeCheck();
		_edgeFade._Offset = EditorGUILayout.Slider("Offset", _edgeFade._Offset, 0f, 1f);
		_edgeFade._Alpha = EditorGUILayout.Slider("Alpha", _edgeFade._Alpha, 0f, 1f);
		_edgeFade._ClipLeft = EditorGUILayout.Slider("Clip Left", _edgeFade._ClipLeft, 0f, 1f);
		_edgeFade._ClipRight = EditorGUILayout.Slider("Clip Right", _edgeFade._ClipRight, 0f, 1f);
		_edgeFade._ClipUp = EditorGUILayout.Slider("Clip Up", _edgeFade._ClipUp, 0f, 1f);
		_edgeFade._ClipDown = EditorGUILayout.Slider("Clip Down", _edgeFade._ClipDown, 0f, 1f);
		if (EditorGUI.EndChangeCheck())
		{
			_edgeFade.ChangeValue();
		}

		GUILayout.EndVertical();

		GUILayout.BeginVertical("Box");

		_edgeFade.cLeftEaseType = EditorGUIUtils.FilteredEasePopup(_edgeFade.cLeftEaseType);
		if (_edgeFade.cLeftEaseType == Ease.INTERNAL_Custom)
			_edgeFade.cLeftCurve = EditorGUILayout.CurveField(" Clip Left Curve", _edgeFade.cLeftCurve);

		_edgeFade.cLeftDuration = EditorGUILayout.FloatField("Clip Left Duration", _edgeFade.cLeftDuration);
		if (_edgeFade.cLeftDuration < 0)
			_edgeFade.cLeftDuration = 0;

		_edgeFade.cLeftDelay = EditorGUILayout.FloatField("Clip Left Delay", _edgeFade.cLeftDelay);
		if (_edgeFade.cLeftDelay < 0)
			_edgeFade.cLeftDelay = 0;

		_edgeFade.cLeftLoops = EditorGUILayout.IntField(new GUIContent("Loop", "Set to -1 for infinite loops"), _edgeFade.cLeftLoops);
		if (_edgeFade.cLeftLoops < -1)
			_edgeFade.cLeftLoops = -1;
		if (_edgeFade.cLeftLoops > 1 || _edgeFade.cLeftLoops == -1)
			_edgeFade.cLeftLoopType = (LoopType)EditorGUILayout.EnumPopup(" Clip Loop Type", _edgeFade.cLeftLoopType);

		_edgeFade.cLeftEndValue = EditorGUILayout.FloatField("Clip Left End Value", _edgeFade.cLeftEndValue);

		GUILayout.EndVertical();

		GUILayout.BeginVertical("Box");

		_edgeFade.cRightEaseType = EditorGUIUtils.FilteredEasePopup(_edgeFade.cRightEaseType);
		if (_edgeFade.cRightEaseType == Ease.INTERNAL_Custom)
			_edgeFade.cRightCurve = EditorGUILayout.CurveField(" Clip Right Curve", _edgeFade.cRightCurve);
		
		_edgeFade.cRightDuration = EditorGUILayout.FloatField("Clip Right Duration", _edgeFade.cRightDuration);
		if (_edgeFade.cRightDuration < 0)
			_edgeFade.cRightDuration = 0;

		_edgeFade.cRightDelay = EditorGUILayout.FloatField("Clip Right Delay", _edgeFade.cRightDelay);
		if (_edgeFade.cRightDelay < 0)
			_edgeFade.cRightDelay = 0;

		_edgeFade.cRightLoops = EditorGUILayout.IntField(new GUIContent("Loop", "Set to -1 for infinite loops"), _edgeFade.cRightLoops);
		if (_edgeFade.cRightLoops < -1)
			_edgeFade.cRightLoops = -1;
		if (_edgeFade.cRightLoops > 1 || _edgeFade.cRightLoops == -1)
			_edgeFade.cRightLoopType = (LoopType)EditorGUILayout.EnumPopup(" Clip Loop Type", _edgeFade.cRightLoopType);

		_edgeFade.cRightEndValue = EditorGUILayout.FloatField("Clip Right End Value", _edgeFade.cRightEndValue);

		GUILayout.EndVertical();

	}



}
