Shader "Replace/Replacement"
{
	SubShader
	{
		Tags {"OverdrawTag" = "ol"}
		ZWrite Off
		ZTest LEqual

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				return fixed4(0, 0.00390625, 0, 1); //1/256
			}
			ENDCG
		}
	}

	SubShader
	{
		Tags {"OverdrawTag" = "1_u"}
		ZWrite[_ZWrite]
		ZTest[unity_GUIZTestMode]

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				return fixed4(0, 0.00390625, 0, 1); //1/256
			}
			ENDCG
		}
	}

	SubShader
	{
		Tags {"OverdrawTag" = "1_default"}
		Blend One One

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				return fixed4(0, 0.00390625, 0, 1); //1/256
			}
			ENDCG
		}
	}

	SubShader
	{
		Tags {"OverdrawTag" = "2_default"}
		Blend One One

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				return fixed4(0, 0.0078125, 0, 1); //1/128
			}
			ENDCG
		}
	}



	SubShader
	{
		Tags {"OverdrawTag" = "1_t"}
		ZWrite[_WriteDepth]
		ZTest [_ZTest]
		Blend One One

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				return fixed4(0, 0.00390625, 0, 1); //1/256
			}
			ENDCG
		}
	}



	SubShader
	{
		Tags {"OverdrawTag" = "1"}
		ZWrite[_ZWrite]
		ZTest [_ZTest]
		Blend One One

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				return fixed4(0, 0.00390625, 0, 1); //1/256
			}
			ENDCG
		}
	}

	SubShader 
	{
		Tags {"OverdrawTag" = "2"}

		Blend One One

		Pass
		{
			ZWrite[_ZWrite1]
			ZTest[_ZTest1]

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				return fixed4(0, 0.00390625, 0, 1); //1/256
			}
			ENDCG
		}

		Pass
		{
			ZWrite[_ZWrite2]
			ZTest[_ZTest2]

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				return fixed4(0, 0.00390625, 0, 1); //1/256
			}
			ENDCG
		}
	}
}
