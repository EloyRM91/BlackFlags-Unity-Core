Shader "_BF Sails/Vela Cangreja 1 Surf"
{
    Properties
    {
        //Textura
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _NormalMap("NormalMap", 2D) = "bump" {}
        //Parámetros
        _Arqueo_Amplitud("Amplitud del arqueo base", Range(-0.15, 0.15)) = 0.00025
        _Arqueo_Factor("Factor lineal del arqueo", Range(-10, 10)) = 0.5
        _Arqueo_OffsetH("Offset horizontal del pico de arqueo", Range(-0.15, 0.15)) = 0.0035
        _Senoide_Fr("Frecuencia senoidal", Range(-100, 100)) = 25
        _Senoide_Offset("Offset de la senoide",Range(-10, 10)) = 3
        _Restriccion_OffsetY("Offset de la restricción vertical de percha",Range(-0.15, 0.15)) = 0.0186
        _Restriccion_m("Pendiente angular de percha",Range(-10, 10)) = 0.395
        //_WavingOffset("Offset de vaivén", Range(0,3)) = 0.3

        //TimeScaling
        [Header(Time Scale)][Space(12)]
        _TimeOffset("TimeOffset", Range(0,99999999999)) = 0
    }
        SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows vertex:vert addshadow
        #pragma target 3.0

        //Textura
        sampler2D _MainTex;
        sampler2D _NormalMap;
        //Parámetros
        half _Arqueo_Amplitud;
        half _Arqueo_Factor;
        half _Arqueo_OffsetH;
        half _Senoide_Fr;
        half _Senoide_Offset;
        half _Restriccion_OffsetY;
        half _Restriccion_m;
        //TimeScale
        half _TimeOffset;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_NormalMap;
        };

        fixed4 _Color;

        void vert(inout appdata_full data)
        {
            data.vertex.z = data.vertex.z + _Arqueo_Amplitud - _Arqueo_Factor * pow(data.vertex.y - _Arqueo_OffsetH, 2);
            data.vertex.z = data.vertex.z + (2.5 * sin(_Senoide_Fr * _TimeOffset) + _Senoide_Offset) * (data.vertex.x - (_Restriccion_OffsetY + _Restriccion_m * data.vertex.y)) * (data.vertex.x);
        }

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
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