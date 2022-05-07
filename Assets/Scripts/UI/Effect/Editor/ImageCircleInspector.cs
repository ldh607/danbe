using UnityEngine;
using System.Collections;
using UnityEditor;
using DG.DOTweenEditor.Core;
using DG.Tweening;


[CustomEditor(typeof(ImageCircle))]
public class ImageCircleInspector : Editor
{

	ImageCircle _circle;

	void Start()
	{
		_circle._Fading = 1;
		_circle.AlphaEndValue = 1;
	}

	void OnEnable()
	{
		_circle = target as ImageCircle;
	
	}

	public override void OnInspectorGUI()
	{
		EditorGUIUtils.SetGUIStyles();


		GUILayout.BeginHorizontal();
		EditorGUIUtils.InspectorLogo();
		GUILayout.Label("IMAGE Circle", EditorGUIUtils.sideLogoIconBoldLabelStyle);

		GUILayout.FlexibleSpace();
		if (GUILayout.Button("▲", EditorGUIUtils.btIconStyle))
			UnityEditorInternal.ComponentUtility.MoveComponentUp(_circle);

		if (GUILayout.Button("▼", EditorGUIUtils.btIconStyle))
			UnityEditorInternal.ComponentUtility.MoveComponentDown(_circle);
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
		_circle._Offset = EditorGUILayout.Slider("Start Circle Value", _circle._Offset, -1, 1);
		_circle._Fading = EditorGUILayout.Slider("Fade", _circle._Fading, 0, 1);
		if (EditorGUI.EndChangeCheck())
		{
			_circle.ChangeValue();
		}

		GUILayout.EndVertical();

		GUILayout.BeginVertical("Box");
		_circle.CircleEaseType = EditorGUIUtils.FilteredEasePopup(_circle.CircleEaseType);
		if (_circle.CircleEaseType == Ease.INTERNAL_Custom)
			_circle.CircleCurve = EditorGUILayout.CurveField("   Circle Value Curve", _circle.CircleCurve);

		_circle.duration = EditorGUILayout.FloatField("Duration", _circle.duration);
		if (_circle.duration < 0)
			_circle.duration = 0;

		_circle.delay = EditorGUILayout.FloatField("Delay", _circle.delay);
		if (_circle.delay < 0)
			_circle.delay = 0;

		_circle.loops = EditorGUILayout.IntField(new GUIContent("Loop", "Set to -1 for infinite loops"), _circle.loops);
		if (_circle.loops < -1)
			_circle.loops = -1;
		if (_circle.loops > 1 || _circle.loops == -1)
			_circle.loopType = (LoopType)EditorGUILayout.EnumPopup("   Loop Type", _circle.loopType);

		GUIEndValue();

		GUILayout.EndVertical();

#region for Alpha

		GUILayout.BeginVertical("Box");
		_circle.AlphaEaseType = EditorGUIUtils.FilteredEasePopup(_circle.AlphaEaseType);
		if (_circle.AlphaEaseType == Ease.INTERNAL_Custom)
			_circle.AlphaCurve = EditorGUILayout.CurveField("   Alpha Curve", _circle.AlphaCurve);

		_circle.AlphaDuration = EditorGUILayout.FloatField("Alpha Duration", _circle.AlphaDuration);
		if (_circle.AlphaDuration < 0)
			_circle.AlphaDuration = 0;

		_circle.AlphaDelay = EditorGUILayout.FloatField("Alpha Delay", _circle.AlphaDelay);
		if (_circle.AlphaDelay < 0)
			_circle.AlphaDelay = 0;

		_circle.Alphaloops = EditorGUILayout.IntField(new GUIContent("Loop", "Set to -1 for infinite loops"), _circle.Alphaloops);
		if (_circle.Alphaloops < -1)
			_circle.Alphaloops = -1;
		if (_circle.Alphaloops > 1 || _circle.Alphaloops == -1)
			_circle.AlphaloopType = (LoopType)EditorGUILayout.EnumPopup("   Loop Type", _circle.AlphaloopType);

		GUIAlphaEndValue();

		GUILayout.EndVertical();

#endregion

	}

	void GUIEndValue()
	{
		EditorGUILayout.BeginVertical("Box");
		_circle.endValue = EditorGUILayout.FloatField("End Circle Value", _circle.endValue);

		EditorGUILayout.EndVertical();
	}

	void GUIAlphaEndValue()
	{
		EditorGUILayout.BeginVertical("Box");
		_circle.AlphaEndValue = EditorGUILayout.FloatField("End Alpha Value", _circle.AlphaEndValue);

		EditorGUILayout.EndVertical();
	}

}
