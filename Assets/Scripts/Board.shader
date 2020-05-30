﻿Shader "Unlit/Board"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_RenderTex("RenderTexture", 2D) = "white" {}
		_PreviousTexture("RenderTexture", 2D) = "black" {}
	}
		SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 100
		Cull off
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			sampler2D _RenderTex;
			sampler2D _PreviousTexture;
			float4 _MainTex_ST;
			float boxPositionX;
			float boxPositionY;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 renderTexCol = tex2D(_RenderTex, i.uv);
				fixed4 prevTexCol = tex2D(_PreviousTexture, i.uv);

				
				col = renderTexCol;

				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}

		////Second pass
		//Pass
		//{
		//	CGPROGRAM
		//	#pragma vertex vert
		//	#pragma fragment frag
		//	// make fog work
		//	#pragma multi_compile_fog

		//	#include "UnityCG.cginc"

		//	struct appdata
		//	{
		//		float4 vertex : POSITION;
		//		float2 uv : TEXCOORD0;
		//	};

		//	struct v2f
		//	{
		//		float2 uv : TEXCOORD0;
		//		UNITY_FOG_COORDS(1)
		//		float4 vertex : SV_POSITION;
		//	};

		//	sampler2D _MainTex;
		//	sampler2D _RenderTex;
		//	float4 _MainTex_ST;

		//	v2f vert(appdata v)
		//	{
		//		v2f o;
		//		o.vertex = UnityObjectToClipPos(v.vertex);
		//		o.uv = TRANSFORM_TEX(v.uv, _MainTex);
		//		UNITY_TRANSFER_FOG(o,o.vertex);
		//		return o;
		//	}

		//	fixed4 frag(v2f i) : SV_Target
		//	{
		//		// sample the texture
		//		fixed4 col = tex2D(_MainTex, i.uv);
		//		fixed4 renderTexCol = tex2D(_RenderTex, i.uv);

		//		//if (i.uv.x > 0.1 && i.uv.y > 0) {
		//		//	renderTexCol.r = 0;
		//		//	renderTexCol.g = 1;
		//		//	renderTexCol.b = 0;
		//		//	renderTexCol.a = 1;
		//		//}
		//		col = col * renderTexCol;
		//		// apply fog
		//		UNITY_APPLY_FOG(i.fogCoord, col);
		//		return col;
		//	}
		//	ENDCG
		//}


	}
}