Shader "_BF Sails/Vela latina 1 Surf 2"
{
    Properties
    {
            //Texture
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _NormalMap("NormalMap", 2D) = "bump" {}
            //Parameters
        _WavingAmplitude("Amplitud", Range(0,15)) = 0.5
        _WavingOffset("Offset de vaivén", Range(-3,3)) = 0
            //Time scale
        [Header(Time Scale)][Space(12)]
        _TimeOffset("TimeOffset", Range(0,99999999999)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows vertex:vert addshadow
        #pragma target 3.0

            //Teturas
        sampler2D _MainTex;
        sampler2D _NormalMap;

            //Parámetros
        half _WavingAmplitude;
        half _WavingOffset;

            //TimeScaler
        half _TimeOffset;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_NormalMap;
        };

        fixed4 _Color;

        void vert(inout appdata_full data) 
        {
            data.vertex.x =  - 1.4 * pow(data.vertex.y - 0.035, 2);
            data.vertex.x = data.vertex.x + (_WavingAmplitude * (sin(30 * _TimeOffset) - _WavingOffset) * (data.vertex.y - 1.15 * data.vertex.z) * (data.vertex.z) );
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Normal = UnpackNormal(tex2D(_NormalMap, IN.uv_NormalMap));
            o.Metallic = 0;
            o.Smoothness = 0;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
