using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


[ExecuteInEditMode]
[AddComponentMenu("UI/Effects/UGUIGradient")]
public class UGUIGradientEffect : BaseMeshEffect
{
	public Color32 topColor = Color.white;
	public Color32 bottomColor = Color.black;

	[Range(0, 10)] public float Offset;

	public override void ModifyMesh(VertexHelper helper)
	{
		if (!IsActive() || helper.currentVertCount == 0)
			return;

		List<UIVertex> vertices = new List<UIVertex>();
		helper.GetUIVertexStream(vertices);

#region Vertical

		float bottomY = vertices[0].position.y;
		float topY = vertices[0].position.y;

		for (int i = 1; i < vertices.Count; i++)
		{
			float y = vertices[i].position.y;
			if (y > topY)
			{
				topY = y;
			}
			else if (y < bottomY)
			{
				bottomY = y;
			}
		}

		float uiElementheight = topY - bottomY;


		UIVertex v = new UIVertex();

		for (int i = 0; i < helper.currentVertCount; i++)
		{
			helper.PopulateUIVertex(ref v, i);
			v.color = Color32.Lerp(bottomColor, topColor, (v.position.y - bottomY) / uiElementheight);
			helper.SetUIVertex(v, i);
		}

#endregion



//		#region Horizon
//
//		float leftX = vertices[0].position.x;
//		float rightX = vertices[0].position.x;
//
//		for (int i = 1; i < vertices.Count; i++)
//		{
//			float x = vertices[i].position.x;
//			if (x > leftX)
//			{
//				leftX = x;
//			}
//			else if (x < rightX)
//			{
//				rightX = x;
//			}
//		}
//
//		float uiElementWidth = leftX - rightX;
//
//		UIVertex v2 = new UIVertex();
//
//		for (int i = 0; i < helper.currentVertCount; i++)
//		{
//			helper.PopulateUIVertex(ref v2, i);
//			v2.color = Color32.Lerp(rightColor, leftColor, (v2.position.x - rightX) / uiElementWidth);
//			helper.SetUIVertex(v2, i);
//		}
//
//		#endregion	

	}

}
