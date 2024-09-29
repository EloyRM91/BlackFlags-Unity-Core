Shader "Custom/NewSurfaceShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _MaskTex ("Mascara", 2D) = "white" {}
        _SecondTex ("Segunda Textura", 2D) = "white" {}
        _Threshold("Umbral Alfa", Range(0,255)) = 80
        //_Margin("Margen Transicion", Range(0,360)) = 0
        _Opacity("Opacidad", Range(0,255)) = 255
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows //alpha:fade
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _MaskTex;
        sampler2D _SecondTex;
        float _Threshold;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_MaskTex;
            float2 uv_SecondTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        float2 _Opacity;

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            fixed4 m = tex2D (_MainTex, IN.uv_MaskTex) * _Color;
            fixed4 c2 = tex2D (_MainTex, IN.uv_SecondTex) * _Color;

            //if(m.rgb.r < _Threshold) {
            //    o.Albedo = c2.rgb;
            //}
            //else {
            //    o.Albedo = c.rgb;
            //}
            o.Albedo = c2.rgb;
            o.Alpha = c2.a;
            //o.Alpha = _Opacity;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
