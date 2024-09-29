Shader "Unlit/BackgroundMenuEffect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        _Threshold("Umbral Alfa", Range(0,360)) = 0
        _Margin("Margen Transición", Range(0,360)) = 0
        _Opacity("Opacidad", Range(0,255)) = 0
    }
    SubShader
    {
        Blend SrcAlpha OneMinusSrcAlpha
        Tags { "RenderType"="Opaque" }
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
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float2 _Opacity;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                //float a = cos(_Time.x/50000) * 128 + 128;

                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                col.a = _Opacity;
  
                return col;
            }
            ENDCG
        }
    }
}
