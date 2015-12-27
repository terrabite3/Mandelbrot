/*
Notes:
Float performance is acceptable. Double performance is very poor.
This could be fixed by rendering the fractal to an image, probably 
with a compute shader. Then the main shader would only perform the
gating and color mapping.

The textures could be managed through Unity as planes. If you 
implement zooming in orthgraphic mode, you can layer the planes in
the z-axis. Then you just need a manager for the textures which
schedules the rendering of sections as needed.

The obvious way to store textures is as a double representing the
smoothed escape value. Another way would be to save doubles for x
and y with an int for iteration number. This would allow the 
rendering kernels to pick up where they left off when the user 
increases the maximum iterations. This would be a tradeoff for 
higher memory requirements to save computation time.

Crazy idea: The GPU kernels would produce a texture of floats
(I think float is plenty of precision for colors), but would also
return the (x,y) doubles to the CPU. The CPU would scan through
the data to extract the points which did not escape. These could
be further iterated on the CPU to produce patches for the textures,
freeing up RAM in the process. This would relieve the GPU from 
storing the full data at 128 bpp while not squandering the nearly-
complete computations.

When an area needs to be rerendered at a higher zoom, you can reuse
those points. 
*/

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
				double x0 = i.texcoord0.x;
				double y0 = i.texcoord0.y;

				double x = 0.0;
				double y = 0.0;
				
				double xSqr;
				double ySqr;
				double xTemp; 
				
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

            }
            ENDCG
		}
	}
}
