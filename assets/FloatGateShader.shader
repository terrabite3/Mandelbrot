Shader "Unlit/FloatGateShader"
{
	Properties
	{
		_ColorTex("Colors", 2D) = "black" {}
		_MainTex("Texture", 2D) = "white" {}
		_Cutoff("Cutoff", float) = 1
	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" }
			LOD 100

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

				struct appdata
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};

				struct v2f
				{
					float2 uv : TEXCOORD0;
					float4 vertex : SV_POSITION;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;
				sampler2D _ColorTex;
				float _Cutoff;

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					// Backup
					/*float value = tex2D(_MainTex, i.uv);
					if (value < 0)
						return fixed4(0.0, 0.0, 0.0, 1.0);
					if (value > _Cutoff)
						return fixed4(0.0, 0.0, 0.0, 1.0);
					float val = float2(fmod(value / 64.0, 1.0), 0.0);
					fixed4 col = tex2D(_ColorTex, val);
					return col;*/

					float value = tex2D(_MainTex, i.uv);
					float val = float2(fmod(value / 64.0, 1.0), 0.0);
					//float val = float2(fmod((_Cutoff - value) / 64.0, 1.0), 0.0);  // Makes colors change
					fixed4 col = tex2D(_ColorTex, val);

					float4 mask = float4(1.0, 1.0, 1.0, 1.0);
					mask.rgb *= sign(value);
					mask.rgb *= max(sign(_Cutoff - value), 0.0);

					return col * mask;
				}
				ENDCG
			}
		}
}
