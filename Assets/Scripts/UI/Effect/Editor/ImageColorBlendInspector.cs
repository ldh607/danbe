using UnityEngine;
using UnityEditor;


namespace Pocatcom.UI
{
	[CustomEditor(typeof(ImageColorBlend))]
	public class ImageColorBlendInspector : Editor
	{
		ImageColorBlend _blender;
		bool _showRequireComponent;


		void OnEnable()
		{
			_blender = target as ImageColorBlend;
			_showRequireComponent = (_blender.gameObject.GetComponent<UnityEngine.UI.MaskableGraphic>() == null);
		}

		public override void OnInspectorGUI()
		{
			if (_showRequireComponent)
			{
				EditorGUILayout.HelpBox("Color Blend does not apply to this GameObject", MessageType.Info);
				EditorGUI.BeginChangeCheck();
				_blender.blendMode = (ImageColorBlend.BlendMode)EditorGUILayout.EnumPopup("Blend Mode", _blender.blendMode);
				if (EditorGUI.EndChangeCheck())
				{
					_blender.ChangeBlendMode();
				}

				EditorGUI.BeginChangeCheck();
				_blender.blendStrength = EditorGUILayout.Slider("Blend Strength", _blender.blendStrength, 0.0f, 1.0f);
				if (EditorGUI.EndChangeCheck())
				{
					_blender.ChangeBlendStrength();
				}
			}
			else
			{
			
				EditorGUI.BeginChangeCheck();
				_blender.blendMode = (ImageColorBlend.BlendMode)EditorGUILayout.EnumPopup("Blend Mode", _blender.blendMode);
				if (EditorGUI.EndChangeCheck())
				{
					_blender.ChangeBlendMode();
				}

				EditorGUI.BeginChangeCheck();
				_blender.blendStrength = EditorGUILayout.Slider("Blend Strength", _blender.blendStrength, 0.0f, 1.0f);
				if (EditorGUI.EndChangeCheck())
				{
					_blender.ChangeBlendStrength();
				}
			}

			EditorGUI.BeginChangeCheck();
			_blender.isChildShaderApply = EditorGUILayout.Toggle("Childern Apply?", _blender.isChildShaderApply);
			if (EditorGUI.EndChangeCheck())
			{
				_blender.ChilderenShaderSetup();
			}

		}
	}
}