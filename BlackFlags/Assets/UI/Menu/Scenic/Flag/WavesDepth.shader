Shader "Custom/WavesDepth"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_TrailTex("Texture", 2D) = "white" {}
		_FoamCollisionTex("Texture", 2D) = "white" {}
		_Darkness("Wave Color Offset - Darkness",Range(0.0,1.0)) = 0.2
	}
		SubShader
		{

			Tags { "RenderType" = "Opaque" }
				Cull Off
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
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			sampler2D _TrailTex;
			sampler2D _FoamCollisionTex;
			float4 _MainTex_ST;
			float4 _TrailTex_ST;
			half _Darkness;


			sampler2D _CameraDepthTexture;

			v2f vert(appdata v)
			{
				v2f o;
				half sinUse = _SinTime.w / 2;
				v.vertex.y = (v.vertex.y + sin(v.vertex.x * 5 + _Time.y) ) / 20;
				v.vertex.y = v.vertex.y + (sin((v.vertex.x * 1.5) + _Time.w) ) / 10;
				v.vertex.x = v.vertex.x + 0.2 * sinUse;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				half sT = sin(_Time.y / 7) / 8;
				fixed4 c1 = tex2D(_MainTex,i.uv);
				fixed4 c2 = tex2D(_TrailTex, float2(i.uv.x + sT, i.uv.y));
				c1 = fixed4(c1.x - _Darkness, c1.y - _Darkness, c1.z - _Darkness, 1);
				c1.x = c1.x + 0.3*(c2.x * c2.w);
				c1.y = c1.y + 0.3*(c2.y * c2.w);
				c1.z = c1.z + 0.3*(c2.z * c2.w);

				//c1 = fixed4(c1.x - _Darkness, c1.y - _Darkness, c1.z - _Darkness, 1);
				//UNITY_APPLY_FOG(i.fogCoord, col);



				//float4 depthSample = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, i.screenPos);
				//float depth = LinearEyeDepth(depthSample).r;
				// Because the camera depth texture returns a value between 0-1,
				// we can use that value to create a grayscale color
				// to test the value output.

				//float deadLine = 1 - saturate(3 * (depth - i.screenPos.w));
				//c1 = c1 - foamLine;
				//c1 = c1 - float4(0.8*deadLine, 0.5*deadLine, 0.4*deadLine, 1);

				//float foamLine = 1 - saturate(15 * (depth - i.screenPos.w));
				//c1 = c1 + 0.7*foamLine;

				//fixed4 c3 = tex2D(_FoamCollisionTex, float2((i.uv.x - _Time.y/8)/0.2, i.uv.y/0.1));
				//float foamBreaker = 1 - saturate(5 * (depth - i.screenPos.w));
				//c1 = (1 - foamBreaker) * c1 + c3 * foamBreaker;
				return c1;
			}
			ENDCG
		}
		}
}
