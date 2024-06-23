Shader "_BF Custom Unlit/Flag 2 - fixed by offset timer"
{
    Properties
    {
        //Texture
        _MainTex ("Texture", 2D) = "white" {}
        //TimeScaling
        [Header(Time Scale)][Space(12)]
        _TimeOffset("TimeOffset", Range(0,99999999999)) = 0
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
            half _TimeOffset;

            v2f vert (appdata v)
            {
                v2f o;
                half sinUse = _SinTime.w / 3;

                v.vertex.z = v.vertex.z + v.vertex.x * sin(280 * cos(30 * _TimeOffset) * v.vertex.x + 45 * _TimeOffset)/ 19 - (v.vertex.x * cos(210 * (v.vertex.y) + 30 * _TimeOffset)/ 7.5f);
                v.vertex.y = v.vertex.y + v.vertex.x * cos(30 * _TimeOffset)/40;
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
