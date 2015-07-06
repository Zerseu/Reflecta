Shader "Reflecta/Client Post Processing" 
{
	Properties 
	{
		_MainTex ("Base (RGBA)", 2D) = "white" {}
	}

	SubShader 
	{
		Pass 
		{
			Cull Off ZWrite Off ZTest Always
			Fog { Mode Off }
				
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"

			uniform sampler2D _MainTex;

			fixed4 frag (v2f_img i) : SV_Target
			{
				//Flip image vertically...
				fixed4 pixel = tex2D(_MainTex, half2(i.uv.x, 1.0 - i.uv.y));

				//Swap R and B channels...
				fixed aux;
				aux = pixel.r;
				pixel.r = pixel.b;
				pixel.b = aux;

				//Clamp alpha value...
				pixel.a = ceil(pixel.a);

				return pixel;
			}
			ENDCG
		}
	}

	Fallback off
}
