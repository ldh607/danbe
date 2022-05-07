using UnityEngine;
using UnityEditor;
using DG.DOTweenEditor.Core;
using DG.Tweening;


[CustomEditor(typeof(ImageSmoke))]
public class ImageSmokeInspector : Editor
{
	ImageSmoke _smoke;

	void OnEnable()
	{
		_smoke = target as ImageSmoke;
	}

	public override void OnInspectorGUI()
	{
		EditorGUIUtils.SetGUIStyles();

		GUILayout.BeginHorizontal();
		EditorGUIUtils.InspectorLogo();
		GUILayout.Label("IMAGE SMOKE", EditorGUIUtils.sideLogoIconBoldLabelStyle);

		GUILayout.FlexibleSpace();
		if (GUILayout.Button("▲", EditorGUIUtils.btIconStyle))
			UnityEditorInternal.ComponentUtility.MoveComponentUp(_smoke);

		if (GUILayout.Button("▼", EditorGUIUtils.btIconStyle))
			UnityEditorInternal.ComponentUtility.MoveComponentDown(_smoke);
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

		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();

		EditorGUI.BeginChangeCheck();

		_smoke.SmokeTex = (Texture2D)EditorGUILayout.ObjectField(_smoke.SmokeTex, typeof(Texture2D), false,
			GUILayout.Width(180), GUILayout.Height(180));
		if (EditorGUI.EndChangeCheck())
		{
			_smoke.ChangeTexture();
		}

		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();

		GUILayout.BeginVertical("Box");

		EditorGUI.BeginChangeCheck();
		_smoke._Value2 = EditorGUILayout.Slider("Start Smoke Value", _smoke._Value2, 0, 1);
		_smoke._SmokeColor1 = EditorGUILayout.ColorField("Smoke Color", _smoke._SmokeColor1);
		_smoke._SmokeColor2 = EditorGUILayout.ColorField("Smoke Color2", _smoke._SmokeColor2);

		if (EditorGUI.EndChangeCheck())
		{
			_smoke.ChangeScroll();
		}

		GUILayout.EndVertical();


		GUILayout.BeginVertical("Box");
		_smoke.SmokeEaseType = EditorGUIUtils.FilteredEasePopup(_smoke.SmokeEaseType);
		if (_smoke.SmokeEaseType == Ease.INTERNAL_Custom)
			_smoke.SmokeSpdCurve = EditorGUILayout.CurveField("   Smoke Value Curve", _smoke.SmokeSpdCurve);

		_smoke.duration = EditorGUILayout.FloatField("Duration", _smoke.duration);
		if (_smoke.duration < 0)
			_smoke.duration = 0;

		_smoke.delay = EditorGUILayout.FloatField("Delay", _smoke.delay);
		if (_smoke.delay < 0)
			_smoke.delay = 0;

		_smoke.loops = EditorGUILayout.IntField(new GUIContent("Loop", "Set to -1 for infinite loops"), _smoke.loops);
		if (_smoke.loops < -1)
			_smoke.loops = -1;
		if (_smoke.loops > 1 || _smoke.loops == -1)
			_smoke.loopType = (LoopType)EditorGUILayout.EnumPopup("   Loop Type", _smoke.loopType);

		GUIEndValue();

		GUILayout.EndVertical();


	}

	void GUIEndValue()
	{
		EditorGUILayout.BeginVertical("Box");
		_smoke.endSpeedValue = EditorGUILayout.FloatField("End Smoke Value", _smoke.endSpeedValue);

		EditorGUILayout.EndVertical();
	}


}
