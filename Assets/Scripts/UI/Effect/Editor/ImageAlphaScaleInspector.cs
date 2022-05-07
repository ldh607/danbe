using UnityEditor;
using DG.DOTweenEditor.Core;
using UnityEngine;
using DG.Tweening;


[CustomEditor(typeof(ImageAlphaScale))]
public class ImageAlphaScaleInspector : Editor
{

	ImageAlphaScale _alphascale;

	void OnEnable()
	{
		_alphascale = target as ImageAlphaScale;
	}

	public override void OnInspectorGUI()
	{
		EditorGUIUtils.SetGUIStyles();

		GUILayout.BeginHorizontal();
		EditorGUIUtils.InspectorLogo();
		GUILayout.Label("IMAGE ALPHA SCALE", EditorGUIUtils.sideLogoIconBoldLabelStyle);

		GUILayout.FlexibleSpace();
		if (GUILayout.Button("▲", EditorGUIUtils.btIconStyle))
			UnityEditorInternal.ComponentUtility.MoveComponentUp(_alphascale);

		if (GUILayout.Button("▼", EditorGUIUtils.btIconStyle))
			UnityEditorInternal.ComponentUtility.MoveComponentDown(_alphascale);

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
		_alphascale._Inside = EditorGUILayout.Slider("Start Inside", _alphascale._Inside, 1, 0);
		_alphascale._Alpha = EditorGUILayout.Slider("Start Alpha", _alphascale._Alpha, 0, 1);
		if (EditorGUI.EndChangeCheck())
			_alphascale.ChangeValue();

		GUILayout.EndVertical();

		GUILayout.BeginVertical("Box");

		_alphascale.ASeaseType = EditorGUIUtils.FilteredEasePopup(_alphascale.ASeaseType);
		if (_alphascale.ASeaseType == Ease.INTERNAL_Custom)
			_alphascale.AScurve = EditorGUILayout.CurveField("   AlphaScale Curve", _alphascale.AScurve);

		_alphascale.AlphaEaseType = EditorGUIUtils.FilteredEasePopup(_alphascale.AlphaEaseType);
		if (_alphascale.AlphaEaseType == Ease.INTERNAL_Custom)
			_alphascale.AlphaCurve = EditorGUILayout.CurveField("   Alpha Curve", _alphascale.AlphaCurve);

		_alphascale.duration = EditorGUILayout.FloatField("Duration", _alphascale.duration);
		if (_alphascale.duration < 0)
			_alphascale.duration = 0;

		_alphascale.delay = EditorGUILayout.FloatField("Delay", _alphascale.delay);
		if (_alphascale.delay < 0)
			_alphascale.delay = 0;

		_alphascale.loops = EditorGUILayout.IntField(new GUIContent("Loop", "Set to -1 for infinite loops"), _alphascale.loops);
		if (_alphascale.loops < -1)
			_alphascale.loops = -1;
		if (_alphascale.loops > 1 || _alphascale.loops == -1)
			_alphascale.loopType = (LoopType)EditorGUILayout.EnumPopup("   Loop Type", _alphascale.loopType);

		GUIEndValue();

		GUILayout.EndVertical();

	}

	void GUIEndValue()
	{
		EditorGUILayout.BeginVertical("Box");

		EditorGUI.BeginChangeCheck();

		_alphascale.endASvalue = EditorGUILayout.FloatField("AlphaScale Value", _alphascale.endASvalue);
		_alphascale.endAlphaValue = EditorGUILayout.FloatField("Alpha Value", _alphascale.endAlphaValue);

		if (EditorGUI.EndChangeCheck())
			_alphascale.ChangeValue();

		EditorGUILayout.EndVertical();
	}

}
