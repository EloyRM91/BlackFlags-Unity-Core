Shader "_BF Sails/Vela Latina 1"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _WavingAmplitude("Amplitud", Range(0,15)) = 0.5
    }
    SubShader
    {
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
            //Texturas
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;
            //Parámetros
            half _WavingAmplitude;

            v2f vert (appdata v)
            {
                v2f o;
                v.vertex.x = 0.0025 -2.5 * pow(v.vertex.y - 0.035, 2) ;

                v.vertex.x = v.vertex.x + _WavingAmplitude * sin(_Time.w) * (v.vertex.y - 1.15 * v.vertex.z) * (v.vertex.z);


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
