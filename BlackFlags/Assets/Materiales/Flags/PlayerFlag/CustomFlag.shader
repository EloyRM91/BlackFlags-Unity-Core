Shader "Unlit/Customized Flag"
{
    Properties
    {
        //Texture
        _MainTex ("Texture", 2D) = "white" {}
        _AvatarTex("Texture", 2D) = "white" {}
        //Parameters
        _WavingAmplitude("Amplitud", Range(0,15)) = 0.5
        _WavingOffset("Offset de vaivén", Range(-3,3)) = 0

    }
    SubShader
    {
        Tags{ "RenderType" = "Transparent" "Queue" = "Transparent"}
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite off

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
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _AvatarTex;
            float4 _AvatarTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                half sinUse = _SinTime.w / 3;

                v.vertex.z = v.vertex.z + v.vertex.x * sin(280 * cos(_Time.z) * v.vertex.x + _Time.w) / 19 - (v.vertex.x * cos(210 * (v.vertex.y) + _Time.z) / 7.5f);
                v.vertex.y = v.vertex.y + v.vertex.x * cos(2 * _Time.y) / 40;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {

                fixed4 col = tex2D(_MainTex, i.uv);
                return col;
            }
            ENDCG
        }
    }
}
