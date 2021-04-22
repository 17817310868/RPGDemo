#warning Upgrade NOTE: unity_Scale shader variable was removed; replaced 'unity_Scale.w' with '1.0'
// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/RoleUI" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
        _RimColor ("Rim Color", Color) = (0.502,0.741,0.976,1.0)
    	_RimPower ("Rim Power", float) = 2.26
    	_RimScale ("Rim Scale", float) = 2.76
    	_Cutoff ("_Cutoff", float) = 0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		fog {
		  Mode Off
		}
		Pass {
			Tags { "LightMode" = "ForwardBase" }

			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			
			#pragma multi_compile_fwdbase
			 
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"
			
			sampler2D _MainTex;
			float4 _RimColor;
			float _RimPower;
			float _RimScale;
			float4 _MainTex_ST;
			
			
			float3 tmpvar_3;
			float3 tmpvar_4;
			float _Cutoff;
			struct a2v {
				float4 vertex : POSITION;
                fixed3 normal : NORMAL;
                fixed4 texcoord : TEXCOORD0;
			};
			
			struct v2f {
				float4 pos : POSITION;
				fixed2 uv : TEXCOORD0;
				fixed3 viewDir : TEXCOORD1;
				fixed3 texcoord3 : TEXCOORD2;
				fixed3 texcoord4 : TEXCOORD3;
				fixed3 shlight_3:TEXCOORD4;
			};
			
			v2f vert(a2v v) {
				v2f o;			
				o.pos =UnityObjectToClipPos(v.vertex);
				
				o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
				float3x3 tmpvar_8={unity_ObjectToWorld[0].x,unity_ObjectToWorld[0].y,unity_ObjectToWorld[0].z,
								  unity_ObjectToWorld[1].x,unity_ObjectToWorld[1].y,unity_ObjectToWorld[1].z,
								  unity_ObjectToWorld[2].x,unity_ObjectToWorld[2].y,unity_ObjectToWorld[2].z
								  };
				 float3 tmpvar_1 = normalize(v.normal);
				 o.texcoord3 = mul(tmpvar_8,tmpvar_1);
				 o.texcoord4 = mul(tmpvar_8,tmpvar_1* 1.0);
				o.shlight_3 = ShadeSH9(half4(normalize(o.texcoord4),1.0));
				o.viewDir = _WorldSpaceCameraPos - mul(unity_ObjectToWorld,v.vertex).xyz;
				return o;
			}
			
			fixed4 frag(v2f i) : COLOR {
				fixed4 texColor = tex2D(_MainTex, i.uv);
				fixed tmpvar = clamp(dot (normalize(i.viewDir), i.texcoord3), 0.0, 1.0);
				tmpvar = 1.0-tmpvar;
				fixed3  temp4 = (((_RimScale * pow (tmpvar, _RimPower)) * _RimColor.xyz) * texColor.xyz);
				if(texColor.w>0.75){discard;}

				fixed4 c_14,c_1;
				c_14.xyz = ((texColor.xyz * _LightColor0.xyz) * (max (0.0, dot (i.texcoord4, _WorldSpaceLightPos0.xyz)) * 2.0));
				c_14.w = texColor.w;
				c_1.w = c_14.w;
				c_1.xyz = (c_14.xyz + (texColor * i.shlight_3));
				c_1.xyz = (c_1.xyz + temp4);
				return c_1;
			}
			ENDCG
		}
	} 
		FallBack "Mobile/Diffuse"
}
