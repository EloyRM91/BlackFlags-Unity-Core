Shader "_BF Custom Unlit/FogMove"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        Cull Off

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
            half4 _MainTex_TexelSize;

            v2f vert (appdata v)
            {
                v2f o;

                _MainTex_ST.z = 0.015 * _Time.w;

                //v.uv = v.uv.x + 0.05 * _Time.w;

                //v.vertex.x = v.vertex.x + 0.05 * _Time.w;
                o.vertex = UnityObjectToClipPos(v.vertex);
                //o.uv = TRANSFORM_TEX(float2(v.uv.x + 0.05 * _Time.w, v.uv.y), _MainTex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                col.rgb = 0.4 * col.rgb;
                return col;
            }
            ENDCG
        }
    }
}
