Shader "Demo/ParticleAdd"
{
	Properties
	{
		[Enum(CullOff,0,CullFront,1,CullBack,2)] _CullMode("CullMode", Float) = 0
		_MainTex("MainTex", 2D) = "white" {}
		[HDR]_MainColor("MainColor", Color) = (0.6886792,0.6886792,0.6886792,1)
		_Main_U_Speed("Main_U_Speed", Range(-5 , 5)) = 0
		_Main_V_Speed("Main_V_Speed", Range(-5 , 5)) = 0
		[Toggle(_DISTORTED_ON)] _Distorted("Distorted", Float) = 0
		_DistortedTex("DistortedTex", 2D) = "white" {}
		_DistortedStrength("DistortedStrength", Range(0 , 2)) = 0
		_Distorted_U_Speed("Distorted_U_Speed", Range(-3 , 3)) = 0
		_Distorted_V_Speed("Distorted_V_Speed", Range(-3 , 3)) = 0
		[Toggle(_SELFALPHAMASK_ON)] _SelfAlphaMask("SelfAlphaMask", Float) = 0
		_MaskTex("MaskTex", 2D) = "white" {}
		_Mask_U_Speed("Mask_U_Speed", Range(-3 , 3)) = 0
		_Mask_V_Speed("Mask_V_Speed", Range(-3 , 3)) = 0
		[IntRange][Enum(DepthOn,1,DepthOff,0)]_ZWrite("WriteDepth", Float) = 0
        [HideInInspector]_ZTest("_", Float) = 4.0
		[Toggle(_DEPTHFADE_ON)] _DepthFade("DepthFade", Float) = 0
		_Softness("Softness", Range(0 , 2)) = 0
		_MaskOnGround("MaskOnGround", Range(0 , 1)) = 1
	}

		SubShader
		{

			Tags { "Queue" = "Transparent" "RenderType" = "Transparent" "OverdrawTag" = "1_t"}
			LOD 100

			Pass
			{
				CGINCLUDE
				#pragma target 3.0
				ENDCG
				Blend One One
				Cull[_CullMode]
				ColorMask RGBA
				ZWrite[_ZWrite]
				ZTest [_ZTest]
				Offset 0 , 0

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"
				#include "UnityShaderVariables.cginc"
				#pragma shader_feature _DISTORTED_ON
				#pragma shader_feature _DEPTHFADE_ON
				#pragma shader_feature _SELFALPHAMASK_ON
				#pragma shader_feature _HIGH_RENDERQUEUE_ON
				#pragma multi_compile_fog
				//#pragma multi_compile_instancing

				struct appdata
				{
					float4 vertex : POSITION;
					UNITY_VERTEX_INPUT_INSTANCE_ID
					float2 ase_texcoord : TEXCOORD0;
					float4 ase_color : COLOR;
				};

				struct v2f
				{
					float4 vertex : SV_POSITION;
					UNITY_VERTEX_OUTPUT_STEREO
					float4 uv : TEXCOORD0;
					float4 ase_color : COLOR;
					float4 screenPos : TEXCOORD1;
					float2 uv_dis : TEXCOORD3;
					float3 worldPos : TEXCOORD4;
					UNITY_FOG_COORDS(2)
				};

				half _MaskOnGround;

				uniform sampler2D _MainTex;
				uniform float _Main_U_Speed;
				uniform float _Main_V_Speed;
				uniform float4 _MainTex_ST;

				#ifdef _DISTORTED_ON
				uniform sampler2D _DistortedTex;
				uniform float _Distorted_U_Speed;
				uniform float _Distorted_V_Speed;
				uniform float4 _DistortedTex_ST;
				uniform float _DistortedStrength;
				#endif
				uniform float4 _MainColor;
				#ifdef _SELFALPHAMASK_ON
				#else
				uniform sampler2D _MaskTex;
				uniform float _Mask_U_Speed;
				uniform float _Mask_V_Speed;
				uniform float4 _MaskTex_ST;
				#endif
				#ifdef _DEPTHFADE_ON
				uniform sampler2D _CameraDepthTexture;
				uniform float _Softness;
				#endif
				uniform half _CullMode;
				v2f vert(appdata v)
				{
					v2f o;
					UNITY_SETUP_INSTANCE_ID(v);
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
					fixed4 ase_clipPos = UnityObjectToClipPos(v.vertex);

					#ifdef _DISTORTED_ON
						fixed2 appendResult3_g69 = (fixed2(_Distorted_U_Speed , _Distorted_V_Speed));
						fixed2 uv_DistortedTex = v.ase_texcoord.xy * _DistortedTex_ST.xy + _DistortedTex_ST.zw;
						fixed2 uv_dis = (appendResult3_g69 * _Time.y) + uv_DistortedTex;
						o.uv_dis = uv_dis;
					#else
						o.uv_dis = (0.0).xx;
					#endif

					fixed2 appendResult3_g71 = (fixed2(_Main_U_Speed , _Main_V_Speed));
					fixed2 uv_MainTex = v.ase_texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
					o.uv.xy = (appendResult3_g71 * _Time.y) + uv_MainTex;

					//mask---------------
					#ifdef _SELFALPHAMASK_ON
						// o.uv.zw = (0.0).xx;
					#else
						fixed2 appendResult3_g70 = (fixed2(_Mask_U_Speed , _Mask_V_Speed));
						fixed2 uv_MaskTex = v.ase_texcoord.xy * _MaskTex_ST.xy + _MaskTex_ST.zw;
						o.uv.zw = (appendResult3_g70 * _Time.y) + uv_MaskTex;
					#endif
					#ifdef _DEPTHFADE_ON
						fixed4 screenPos = ComputeScreenPos(ase_clipPos);
						o.screenPos = screenPos;
					#else
						o.screenPos = (0.0).xxxx;
					#endif

					o.ase_color = v.ase_color * _MainColor;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
					UNITY_TRANSFER_FOG(o,o.vertex);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					fixed4 finalColor;
					#ifdef _DISTORTED_ON
					fixed2 staticSwitch200 = tex2D(_DistortedTex, i.uv_dis).rg * _DistortedStrength;
					fixed2 uv_main = i.uv.xy + staticSwitch200;
					#else
					fixed2 uv_main = i.uv.xy;
					#endif
					fixed4 tex2DNode204 = tex2D(_MainTex,uv_main);

					#ifdef _SELFALPHAMASK_ON
						fixed staticSwitch215 = tex2DNode204.a;
					#else
						fixed3 desaturateInitialColor201 = tex2D(_MaskTex,  i.uv.zw).rgb;
						fixed desaturateDot201 = dot(desaturateInitialColor201, fixed3(0.299, 0.587, 0.114));
						fixed3 desaturateVar201 = lerp(desaturateInitialColor201, desaturateDot201.xxx, 1.0);
						fixed staticSwitch215 = (tex2DNode204.a * (desaturateVar201).x);
					#endif
					fixed temp_output_217_0 = (i.ase_color.a * staticSwitch215);

					#ifdef _DEPTHFADE_ON
						fixed4 screenPos = i.screenPos;
						fixed4 ase_screenPosNorm = screenPos / screenPos.w;
						ase_screenPosNorm.z = (UNITY_NEAR_CLIP_VALUE >= 0) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
						fixed screenDepth225 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD(screenPos))));
						fixed distanceDepth225 = saturate(abs((screenDepth225 - LinearEyeDepth(ase_screenPosNorm.z)) / (_Softness)));
						fixed staticSwitch228 = (temp_output_217_0 * distanceDepth225);
					#else
						fixed staticSwitch228 = temp_output_217_0;
					#endif
						//�͹��ذ�
						staticSwitch228 = _MaskOnGround ? (i.worldPos.y < -0.1 ? 0 : staticSwitch228) : staticSwitch228;
					fixed4 appendResult220 = fixed4(((tex2DNode204).rgb * (i.ase_color).rgb * (staticSwitch228)) , 0);


					finalColor = appendResult220;
					UNITY_APPLY_FOG(i.fogCoord, finalColor);
					return finalColor;
				}
				ENDCG
			}
		}
		CustomEditor "ShaderIDBlend_Shader"
}
