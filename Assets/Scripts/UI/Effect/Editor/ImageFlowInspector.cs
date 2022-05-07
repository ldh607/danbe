using UnityEngine;
using UnityEditor;
using DG.DOTweenEditor.Core;


[CustomEditor(typeof(ImageFlow))]
public class ImageFlowInspector : Editor
{
	ImageFlow _flow;

	void OnEnable()
	{
		_flow = target as ImageFlow;
	}

	public override void OnInspectorGUI()
	{
		EditorGUIUtils.SetGUIStyles();

		GUILayout.BeginHorizontal();
		EditorGUIUtils.InspectorLogo();
		GUILayout.Label("IMAGE FLOW", EditorGUIUtils.sideLogoIconBoldLabelStyle);

		GUILayout.FlexibleSpace();
		if (GUILayout.Button("▲", EditorGUIUtils.btIconStyle))
			UnityEditorInternal.ComponentUtility.MoveComponentUp(_flow);

		if (GUILayout.Button("▼", EditorGUIUtils.btIconStyle))
			UnityEditorInternal.ComponentUtility.MoveComponentDown(_flow);

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
		_flow._FlowTex = (Texture2D)EditorGUILayout.ObjectField(_flow._FlowTex, typeof(Texture2D), false,
			GUILayout.Width(180), GUILayout.Height(180));
		if (EditorGUI.EndChangeCheck())
		{
			_flow.ChangeTexture();
		}

		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();

		GUILayout.BeginVertical("Box");

		EditorGUI.BeginChangeCheck();
		_flow._AddColorR = EditorGUILayout.FloatField("+ Color R", _flow._AddColorR);
		_flow._AddColorG = EditorGUILayout.FloatField("+ Color G", _flow._AddColorG);
		_flow._AddColorB = EditorGUILayout.FloatField("+ Color B", _flow._AddColorB);

		_flow._ScrollX = EditorGUILayout.FloatField(" Layer1 ", _flow._ScrollX);
		_flow._ScrollY = EditorGUILayout.FloatField(" Layer2 ", _flow._ScrollY);

		if (EditorGUI.EndChangeCheck())
		{
			_flow.ChangeValue();
		}

		GUILayout.EndVertical();

	}


}
