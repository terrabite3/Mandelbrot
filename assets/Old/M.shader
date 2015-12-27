Shader "Unlit/M"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 5.0

            #include "UnityCG.cginc"

            struct vertexInput {
                float4 vertex : POSITION;
                float4 texcoord0 : TEXCOORD0;
            };

            struct fragmentInput{
                float4 position : SV_POSITION;
                float4 texcoord0 : TEXCOORD0;
            };

            fragmentInput vert(vertexInput i){
                fragmentInput o;
                o.position = mul (UNITY_MATRIX_MVP, i.vertex);
                o.texcoord0 = i.texcoord0;
                return o;
            }
            fixed4 frag(fragmentInput i) : SV_Target {
				float x0 = i.texcoord0.x;
				float y0 = i.texcoord0.y;

				float x = 0.0;
				float y = 0.0;
				
				float xSqr;
				float ySqr;
				float xTemp; 
				
				int iteration = 0;
				int maxIt = 100;
				
				while(true) {
					xSqr = x * x;
					ySqr = y * y;
					if (xSqr + ySqr > 4.0) break;
					if (iteration >= maxIt) break;
					
					xTemp = xSqr - ySqr + x0;
					y = 2.0 * x * y + y0;
					x = xTemp;

					iteration++;
				}
				
				if (iteration == maxIt) {
					return fixed4(0.0, 0.0, 0.0, 0.0);
				} else {
					return float4(float(iteration) / maxIt, 0.0, 0.0, 1.0);
				}


//                return fixed4(i.texcoord0.xy,0.0,1.0);
            }
            ENDCG
			//#include "UnityCG.cginc"

//			struct appdata
//			{
//				float4 vertex : POSITION;
//				float2 uv : TEXCOORD0;
//			};

//			struct v2f
//			{
//				float2 uv : TEXCOORD0;
//				UNITY_FOG_COORDS(1)
//				float4 vertex : SV_POSITION;
//			};

//			sampler2D _MainTex;
//			float4 _MainTex_ST;
//			
//			v2f vert (appdata v)
//			{
//				v2f o;
///				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
//				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
//				UNITY_TRANSFER_FOG(o,o.vertex);
//				return o;
//			}
//			
//			fixed4 frag (v2f i) : SV_Target
//			{
//				// sample the texture
//				fixed4 col = tex2D(_MainTex, i.uv);
//				// apply fog
//				UNITY_APPLY_FOG(i.fogCoord, col);				
//				return col;
//			}
		}
	}
}
