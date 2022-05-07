using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(SoftAlphaMask))]
public class SoftAlphaMaskInspector : Editor
{

	//	SoftAlphaMask _alphaMask;
	
	//	void OnEnable()
	//	{
	//		_alphaMask = target as SoftAlphaMask;
	//	}

	public override void OnInspectorGUI()
	{
		EditorGUILayout.BeginVertical("Box");

		DrawDefaultInspector();

		EditorGUILayout.EndVertical();
	}

}
