Shader "_BF Sails/Vela Mística 1"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _NormalMap("NormalMap", 2D) = "bump" {}

        //Parámetros
        _Seniode_Fr("Frecuencia senoidal", Range(-20, 20)) = 2

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

        sampler2D _MainTex;
        sampler2D _NormalMap;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_NormalMap;
        };
        //Texture
        fixed4 _Color;

        //Parameters
        half _Seniode_Fr;

        //Timescaled
        half _TimeOffset;

        void vert(inout appdata_full data)
        {
            data.vertex.z = 0.0025 - 2.5 * pow(data.vertex.y - 0.035, 2);
            data.vertex.z = data.vertex.z + 2.5 * sin(_Seniode_Fr * _TimeOffset) * (data.vertex.y - 2 * (data.vertex.z)) * (data.vertex.z);
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
