Shader "_BF Sails/Vela Gavia 1"
{
    Properties
    {
        //Texture
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _NormalMap("NormalMap", 2D) = "bump" {}

        //Parameters
        [Header(Parameters)][Space(12)]
        _Seniode_Fr("Frecuencia senoidal", Range(-10,10)) = 2.5

        //Time scale
        [Header(Time Scale)][Space(12)]
        _TimeOffset("TimeOffset", Range(0,99999999999)) = 0
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" "PerformanceChecks" = "False" }
        //Cull Off
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows vertex:vert addshadow
        #pragma target 3.0

            //Texture
        sampler2D _MainTex;
        sampler2D _NormalMap;

            //Parametros
        half _Seniode_Fr;
        
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
            data.vertex.x = -0.0015 + 8.5 * pow(data.vertex.y - 0.014, 2);
            data.vertex.x = data.vertex.x + (4.6 * sin(_Seniode_Fr * _TimeOffset) - 16) * (0.02 - data.vertex.z) * (data.vertex.z);
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
