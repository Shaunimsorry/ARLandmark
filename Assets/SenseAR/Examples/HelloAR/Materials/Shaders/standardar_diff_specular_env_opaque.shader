Shader "SenseAR/diff_specular_env_opaque" {
	Properties {
		_Cutoff("Cutout", float) = 0.3
		[Toggle] _IsDoubleFace("Double Face", Float) = 0

		_MainTex("Diffuse Map", 2D) = "white" {}
		_EmissiveMap("Emissive Map", 2D) = "black" {}
		_ReflectionMap("Reflection Map", 2D) = "black" {}
		_ReflectFactor("Reflection Factor",  Range(0.0, 1.0)) = 1.0
		_SkyboxMap("CubeMap", CUBE) = ""{}
	}
	SubShader {
		Pass { 
			Tags { "RenderType" = "Opaque" "Queue" = "Geometry" "LightMode"="ForwardBase" }
		
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			
			#include "Lighting.cginc"

			sampler2D _MainTex;
			sampler2D _EmissiveMap;
			sampler2D _ReflectionMap;
			samplerCUBE _SkyboxMap;
			
			float _ReflectFactor;
			float _Cutoff;
			float _IsDoubleFace;
			
			struct a2v {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 texcoord : TEXCOORD0;
			};
			
			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 worldRefl: TEXCOORD1;
			};

			v2f vert(a2v v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv.xy = v.texcoord.xy;

				fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);
				fixed3 worldViewDir = WorldSpaceViewDir(v.vertex);
				o.worldRefl = reflect(-worldViewDir, worldNormal);

				return o;
			}
			
			fixed4 frag(v2f i) : SV_Target
			{				
				fixed4 diffTexColor = tex2D(_MainTex, i.uv);
				fixed3 emisTexColor = tex2D(_EmissiveMap, i.uv);
				fixed3 cubeTexColor = texCUBE(_SkyboxMap, i.worldRefl);

				clip(diffTexColor.a - _Cutoff);

				//fixed3 viewDir = normalize(UNITY_MATRIX_IT_MV[2].xyz);
				fixed3 viewDir = normalize(UNITY_MATRIX_V[2].xyz);
				fixed3 temref = normalize(i.worldRefl);
				temref = temref + viewDir;
				float len2 = 2.0*sqrt(temref.x*temref.x+temref.y*temref.y+temref.z*temref.z); 
				fixed2 equireCoord = fixed2(0.5 - temref.x / len2, 0.5 + temref.y / len2);

				fixed3 planeRefColor = tex2D(_ReflectionMap, equireCoord) * _ReflectFactor;
				fixed3 cubeRefColor = _ReflectFactor * cubeTexColor.rgb;

				return fixed4(emisTexColor.rgb + diffTexColor.rgb + planeRefColor + cubeRefColor, 1.0);
			}
			
			ENDCG
		}
	} 
	FallBack "Specular"
}
