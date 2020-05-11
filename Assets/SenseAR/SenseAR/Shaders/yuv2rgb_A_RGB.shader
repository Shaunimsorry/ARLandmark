Shader "SenseAR/yuv_nv21_2_rgb_A_RGB"
{
	Properties
	{
		_MainTex ("Y Texture", 2D) = "gray"
		_uvTex("UV Texture", 2D) = "bump"
		[HideInInspector]_gamma("gamma", float) = 1.0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
	        ZWrite Off // don't write to depth buffer 
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			float4x4 _DisplayTransform;
            float2   _MainTextST;

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
			sampler2D _uvTex;

			float _gamma;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);

				float texX = v.uv.x;
				float texY = v.uv.y * _MainTextST.x + _MainTextST.y;
				
				o.uv.x = (_DisplayTransform[0].x * texX + _DisplayTransform[1].x * (texY) + _DisplayTransform[2].x);
 			 	o.uv.y = (_DisplayTransform[0].y * texX + _DisplayTransform[1].y * (texY) + (_DisplayTransform[2].y));

				return o;
			}
            
            fixed3 yuv2rgb(float2 texCoordinate)
            {
            	fixed y = tex2D(_MainTex, texCoordinate).a;
				y = pow(y, _gamma);

			    fixed v = tex2D(_uvTex, texCoordinate).r - 0.5;
			    fixed u = tex2D(_uvTex, texCoordinate).g - 0.5;
			    
			    fixed3 rgb;
			    rgb.r = y + 1.13983*v;
			    rgb.g = y - 0.39465*u - 0.58060*v;
			    rgb.b = y + 2.03211*u;
			    
			    return rgb;
             }

			
			fixed4 frag (v2f i) : SV_Target
			{
				float2 uv_reverseY = float2(i.uv.x, 1.0-i.uv.y);
				fixed3 col = yuv2rgb(uv_reverseY);
				
				return fixed4(col, 1);
			}
			ENDCG
		}
	}
}
