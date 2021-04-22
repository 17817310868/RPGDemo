// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Role" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_GlowTex ("_GlowTex", 2D) = "white" {}
		//_Ambient ("_Ambient (color)", COLOR) = (1.0,1.0,1.0,1.0)
		//_CullOff("_CullOff",float) = 0.75
		_LColor("Color",COLOR)=(1.0,1.0,1.0,1.0)
		_light("light",float) = 5.0
		_LightFaceScale("LightFaceScale",float) = 0.76
		_Moonlight("Moonlight",COLOR)=(1.0,1.0,1.0,1.0)
		
	}
//	SubShader {
//		LOD 400
//		Tags { "QUEUE"="Geometry+500" "IGNOREPROJECTOR"="true" }
//		pass{
//			Tags {"LIGHTMODE"="ForwardBase" "RenderType"="TransparentCutout" }
//			CGPROGRAM
//			#pragma vertex vert
//			#pragma fragment frag
//			#include "UnityCG.cginc"
//			struct vert_Input{
//				fixed4 vertex:POSITION;
//				fixed2 texcoord:TEXCOORD0;
//				fixed3 normal:NORMAL;
//			};
//			
//			struct vert_Output{
//				fixed4 pos:SV_POSITION;
//				fixed4 uv:TEXCOORD0;
//				fixed4 uv1:TEXCOORD1;
//			};
//			
//			uniform sampler2D _MainTex;
//			uniform sampler2D _GlowTex;
//			uniform sampler2D _LightFace;
//			uniform float4x4 _texViewProj;
//			fixed4 _MainTex_ST;
//			fixed4 _LColor;
//			fixed4 _Moonlight;
//			fixed4 _LightColor0;
//			float _light;
//			float _LightFaceScale;
//			float _Cutoff;
//			fixed4 _Ambient= fixed4(0.706,0.706,0.706,1.0);
//			vert_Output vert(vert_Input i){
//				vert_Output o;
//				float3 normal = normalize(i.normal);
//				o.pos = mul(UNITY_MATRIX_MVP,fixed4(i.vertex.xyz,1.0));
//				o.uv.xy =  TRANSFORM_TEX(i.texcoord, _MainTex);
//				half2 capCoord_2;
//				capCoord_2.x = dot(UNITY_MATRIX_IT_MV[0].xyz,normal);
//				capCoord_2.y = dot(UNITY_MATRIX_IT_MV[1].xyz,normal);
//				o.uv.zw = capCoord_2*0.5+0.5;
//				float4 tmpvar_13,tmpvar_10;
//				tmpvar_10.xyz = normal;
//				tmpvar_10.w = 0.0;
//				o.uv1.x = max (0.2, dot (normalize(mul(_Object2World ,tmpvar_10)).xyz, normalize(_WorldSpaceLightPos0.xyz)));
//				tmpvar_13.w = 1.0;
//				tmpvar_13.xyz = mul(_Object2World,i.vertex).xyz;
//  				float4 tmpvar_14 = mul(_texViewProj,tmpvar_13) ;
//  				o.uv1.zw = ((tmpvar_14 / tmpvar_14.w).xy + fixed2(1.0, 1.0))*0.5;
//  				return o;
//			}
//			
//			fixed4 frag(vert_Output o):COLOR{
//				fixed4 tmpvar_2 = tex2D(_MainTex, o.uv.xy);
//				fixed4 tmpvar_3 = tex2D(_GlowTex, o.uv.zw);
//				fixed tmpvar_4 = tmpvar_2.w;//floor(tmpvar_2.w*1.5);
//				if(tmpvar_4>0.75)discard;
//				float3 tmpvar_5 = tmpvar_2.xyz *(fixed3(0.706,0.706,0.706)+ _LightColor0.xyz* o.uv1.x) + tmpvar_3.xyz * tmpvar_4*0.5;
//				fixed4 tmpvar_10 = tex2D(_LightFace, o.uv1.zw);
//				fixed4 baseColor_6,lightColor_9;
//				lightColor_9.w = tmpvar_10.w;
//				lightColor_9.xyz = tmpvar_10.xyz*(tmpvar_10.x + tmpvar_10.y + tmpvar_10.z) * 0.6666;
//				baseColor_6.xyz = (tmpvar_5.xyz * ((lightColor_9.xyz* _Moonlight.xyz) * _LightFaceScale));//lightColor_9.xyz *
//				baseColor_6.w = tmpvar_2.w;
//				fixed4 tex_1;
//			 	tex_1.xyz = (((baseColor_6.xyz * _light) ) * _LColor.xyz);
//  				tex_1.w = (tex_1.x + tex_1.y + tex_1.z) * 0.35;
//  				return tex_1;
//			}
//			ENDCG
//		}
//	} 
	
	SubShader {
		LOD 200
		Tags { "QUEUE"="Transparent-10" "IGNOREPROJECTOR"="true" }
		pass{
			Tags {"LIGHTMODE"="ForwardBase"  "IGNOREPROJECTOR"="true" "RenderType"="TransparentCutout" }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			struct vert_Input{
				fixed4 vertex:POSITION;
				fixed2 texcoord:TEXCOORD0;
				fixed3 normal:NORMAL;
			};
			
			struct vert_Output{
				fixed4 pos:SV_POSITION;
				fixed4 uv:TEXCOORD0;
				fixed2 uv1:TEXCOORD1;
			};
			
			uniform sampler2D _MainTex;
			uniform sampler2D _GlowTex;
			uniform sampler2D _Specular;
			fixed4 _MainTex_ST;
			fixed4 _LColor;
			fixed4 _LightColor0;
			float _light;
			float _Cutoff;
			fixed4 _Ambient= fixed4(0.7,0.7,0.7,1.0);
			vert_Output vert(vert_Input i){
				vert_Output o;
				float3 normal = normalize(i.normal);
				o.pos = UnityObjectToClipPos(fixed4(i.vertex.xyz,1.0));
				o.uv.xy =  TRANSFORM_TEX(i.texcoord, _MainTex);
				half2 capCoord_2;
				capCoord_2.x = dot(UNITY_MATRIX_IT_MV[0].xyz,normal);
				capCoord_2.y = dot(UNITY_MATRIX_IT_MV[1].xyz,normal);
				o.uv.zw = capCoord_2*0.5+0.5;
				float4 tmpvar_13,tmpvar_10;
				tmpvar_10.xyz = normal;
				tmpvar_10.w = 0.0;
				o.uv1.x = max (0.2, dot (normalize(mul(unity_ObjectToWorld ,tmpvar_10)).xyz, normalize(_WorldSpaceLightPos0.xyz)));
  				return o;
			}
			
			fixed4 frag(vert_Output o):COLOR{
				fixed4 tmpvar_2 = tex2D(_MainTex, o.uv.xy);
				fixed4 tmpvar_3 = tex2D(_GlowTex, o.uv.zw);
				fixed tmpvar_4 = tmpvar_2.w;
				if(tmpvar_4>0.75) discard;
				float3 tmpvar_5 = tmpvar_2.xyz *(fixed3(0.706,0.706,0.706)+ _LightColor0.xyz* o.uv1.x )+ tmpvar_3.xyz * tmpvar_4*0.5;
				fixed4 tex_1;
				tex_1.xyz = tmpvar_5;
				tex_1.xyz = tex_1.xyz * _light * _LColor.xyz;
				tex_1.w =(tex_1.x + tex_1.y + tex_1.z) * 0.35;
  				return tex_1;
			}
			ENDCG
		}
	} 
	FallBack "Mobile/Diffuse"
}
