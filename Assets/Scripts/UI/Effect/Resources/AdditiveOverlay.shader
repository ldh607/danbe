// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "AdditiveOverlay" 
{
	Properties 
	{
		[PerRendererData]_MainTex("Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)

		_StencilComp("Stencil Comparison", Float) = 8
		_Stencil("Stencil ID", Float) = 0
		_StencilOp("Stencil Operation", Float) = 0
		_StencilWriteMask("Stencil Write Mask", Float) = 255
		_StencilReadMask("Stencil ReIad Mask", Float) = 255

		_ColorMask("Color Mask", Float) = 15

		_TargetTex("Target Texture", 2D) = "white"{}

		_MainTexUVMoveX("Main Tex UV Move X", Float) = 0
		_MainTexUVMoveY("Main Tex UV Move Y", Float) = 0

		_MainTexScalingX("Main Tex Scaling X", Float) = 0
		_MainTexScalingY("Main Tex Scaling Y", Float) = 0

		_InnerUVx("Inner UV x", Float) = 0
		_InnerUVy("Inner UV y", Float) = 0
		_InnerSizeX("Inner Size X", Float) = 0
		_InnerSizeY("Inner Size Y", Float) = 0

		_OuterUV("OuterUV", Vector) = (0,0,0,0)
		_OuterSizeX("Outer UV Size X", Float) = 0
		_OuterSizeY("Outer uv Size Y", Float) = 0

		_SizeRatioX ("Size Ratio X", Float) = 0
		_SizeRatioY ("Size Ratio Y", Float) = 0

		_MoveX("Move X", float) = 0
		_MoveY("Move Y", float) = 0

		_PowerStrength("_PowerStrength Strength", Float) = 1
	}
	SubShader 
	{
		Tags 
		{ 
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

		Stencil
		{
			Ref[_Stencil]
			Comp[_StencilComp]
			Pass[_StencilOp]
			ReadMask[_StencilReadMask]
			WriteMask[_StencilWriteMask]
		}

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest[unity_GUIZTestMode]
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask[_ColorMask]

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#include "UnityUI.cginc"

			struct appdata_t
			{
				float4			vertex		:	POSITION;
				float4			color		:	COLOR;
				float2			texcoord	:	TEXCOORD0;
			};

			struct v2f
			{
				float4			vertex			:	SV_POSITION;
				fixed4			color			:	COLOR;
				half2			texcoord		:	TEXCOORD0;
				float4			worldPosition	:	TEXCOORD1;
			};

			sampler2D _MainTex;
			sampler2D _TargetTex;
			float4 _MainTex_ST;
			float4 _TargetTex_ST;
			fixed4 _Color;
			float4 _ClipRect;


			float _MainTexUVMoveX;
			float _MainTexUVMoveY;
	
			float _MainTexScalingX;
			float _MainTexScalingY;

			float _InnerUVx;
			float _InnerUVy;

			bool _UseClipRect;

			float _InnerSizeX;
			float _InnerSizeY;

			float4 _OuterUV;
			float _OuterSizeX;
			float _OuterSizeY;

			float _SizeRatioX;
			float _SizeRatioY;
			fixed4 _TextureSampleAdd;

			float _MoveX;
			float _MoveY;

			fixed _PowerStrength;

			v2f vert(appdata_t i)
			{
				v2f o;

				o.worldPosition = i.vertex;

				o.vertex = UnityObjectToClipPos(o.worldPosition);

				o.texcoord = TRANSFORM_TEX(i.texcoord, _MainTex);

				#ifdef UNITY_HALF_TEXEL_OFFSET
				o.vertex.xy += (_ScreenParams.zw - 1.0) * float2(-1,1);
				#endif

				o.color = i.color * _Color;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				half2 mainTexcoord = i.texcoord;

				half4 color = (tex2D(_MainTex, i.texcoord)) * i.color;
				color.w *= UnityGet2DClipping(i.worldPosition.xy, _ClipRect);


				half2 targetUV;

				mainTexcoord.x *= _MainTexScalingX;
				mainTexcoord.y *= _MainTexScalingY;

				mainTexcoord.x -= _MainTexUVMoveX;
				mainTexcoord.y -= _MainTexUVMoveY;

				targetUV.x = (_InnerSizeX * mainTexcoord.x ) + _InnerUVx - _MoveX;
				targetUV.y = (_InnerSizeY * mainTexcoord.y ) + _InnerUVy - _MoveY;

				half4 targetColor = (tex2D(_TargetTex, targetUV )) * i.color;
				if(targetUV.x < _OuterUV.x || targetUV.x > _OuterUV.z
				|| targetUV.y < _OuterUV.y || targetUV.y > _OuterUV.w)
				{
					targetColor.a = 0;
				}


				half4 resultColor;
				half4 multiColor = color * targetColor;
				resultColor = multiColor * _PowerStrength;

				return resultColor;
			}
			ENDCG
		}
	}
	FallBack "UI/Default"
}
