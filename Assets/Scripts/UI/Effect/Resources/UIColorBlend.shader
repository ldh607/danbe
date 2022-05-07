// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "UI/Color Blend"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
	
		_BlendStrength("Blend Strength", Float) = 1

		_StencilComp("Stencil Comparison", Float) = 8
		_Stencil("Stencil ID", Float) = 0
		_StencilOp("Stencil Operation", Float) = 0
		_StencilWriteMask("Stencil Write Mask", Float) = 255
		_StencilReadMask("Stencil Read Mask", Float) = 255

		_ColorMask("Color Mask", Float) = 15

	}

	SubShader
	{
		Tags
		{
			"Queue"="Transparent"
			"IgnoreProjector"="True"
			"RenderType"="Transparent"
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Stencil
		{
			Ref [_Stencil]
			Comp [_StencilComp]
			Pass [_StencilOp]
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
		}

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest [unity_GUIZTestMode]
		Fog { Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask [_ColorMask]

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "UnityUI.cginc"
			#include "UIColorBlendFunction.cginc"

			#pragma multi_compile _GRAYSCALE _LUMINOSITY _SATURATION _COLOR _ADDITIVE _SUBTRACT _MULTIPLY _DIVIDE _DIFFERENCE _HARDMIX _BURN _DODGE _OVERLAY _SCREEN _DARKEN _DARKER _LIGHTEN _LIGHTER _HARDLIGHT _SOFTLIGHT _PINLIGHT _VIVIDLIGHT

			sampler2D _MainTex;
			fixed4 _TextureSampleAdd;
			float4 _ClipRect;
			fixed _BlendStrength;

			struct VertexInput
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct VertexOutput
			{
				float4 pos : SV_POSITION;
				fixed4 color : COLOR;
				half2 texcoord : TEXCOORD0;
				float4 worldPosition : TEXCOORD1;
			};

			VertexOutput vert(VertexInput v)
			{
				VertexOutput o;
				o.worldPosition = v.vertex;
				o.pos = UnityObjectToClipPos(o.worldPosition);

				o.texcoord = v.texcoord;

				o.color = v.color;

				return o;
			}

			fixed4 frag(VertexOutput i) : SV_Target
			{
				half4 c = tex2D(_MainTex, i.texcoord);

				c.w *= UnityGet2DClipping(i.worldPosition.xy, _ClipRect);

				fixed3 blendColor = c.rgb;

				#if defined(_GRAYSCALE)
					blendColor = Luminance601(c.rgb * i.color.rgb).xxx;

				#elif defined(_LUMINOSITY)
					blendColor = Luminosity(c.rgb, i.color.rgb);

				#elif defined(_SATURATION)
					blendColor = Saturation(c.rgb, i.color.rgb);

				#elif defined(_COLOR)
					blendColor = Color(c.rgb, i.color.rgb);

				#elif defined(_ADDITIVE)
					blendColor = Additive(c.rgb, i.color.rgb);

				#elif defined(_SUBTRACT)
					blendColor = Subtract(c.rgb, i.color.rgb);

				#elif defined(_MULTIPLY)
					blendColor = Multiply(c.rgb, i.color.rgb);

				#elif defined(_DIVIDE)
					blendColor = Divide(c.rgb, i.color.rgb);

				#elif defined(_DIFFERENCE)
					blendColor = Difference(c.rgb, i.color.rgb);

				#elif defined(_HARDMIX)
					blendColor = HardMix(c.rgb, i.color.rgb);

				#elif defined(_BURN)
					blendColor = Burn(c.rgb, i.color.rgb);

				#elif defined(_DODGE)
					blendColor = Dodge(c.rgb, i.color.rgb);

				#elif defined(_OVERLAY)
					blendColor = Overlay(c.rgb, i.color.rgb);

				#elif defined(_SCREEN)
					blendColor = Screen(c.rgb, i.color.rgb);

				#elif defined(_DARKEN)
					blendColor = Darken(c.rgb, i.color.rgb);

				#elif defined(_DARKER)
					blendColor = Darker(c.rgb, i.color.rgb);

				#elif defined(_LIGHTEN)
					blendColor = Lighten(c.rgb, i.color.rgb);

				#elif defined(_LIGHTER)
					blendColor = Lighter(c.rgb, i.color.rgb);

				#elif defined(_HARDLIGHT)
					blendColor = HardLight(c.rgb, i.color.rgb);

				#elif defined(_SOFTLIGHT)
					blendColor = SoftLight(c.rgb, i.color.rgb);

				#elif defined(_PINLIGHT)
					blendColor = PinLight(c.rgb, i.color.rgb);

				#elif defined(_VIVIDLIGHT)
					blendColor = VividLight(c.rgb, i.color.rgb);

				#endif

				c.rgb = lerp(c.rgb, blendColor, _BlendStrength);
				c.a = c.a * i.color.a;
				return c;
			}
			ENDCG
		}
	}
	FallBack "UI/Default"
}
