Shader "Testing/Distortion-Test"
{
    Properties
    {
        //Texturas
        [Header(Textures)] [Space(12)]
        _Color ("Color", Color) = (1,1,1,1)
        _NoiseTex1("Noise Tex 1", 2D) = "white" {}
        _DistorsionTex ("Flow Distortion Tex", 2D) = "white" {}
        _FoamTex("Espuma", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _NormalMap("NormalMap", 2D) = "bump" {}

        //Parámetros
        [Header(Settings)] [Space(12)]
        _CurrentAngle("Direccion", Range(0,360)) = 0
        _CurrentSpeed("Velocidad", Range(0,10)) = 1.5
        _DistortionSpeed("Distorsion", Range(0,2)) = 0.1

        //Time scale
        [Header(Time Scale)][Space(12)]
        _TimeOffset("TimeOffset", Range(0,99999999999)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

            //Tex
        sampler2D _DistorsionTex;
        sampler2D _NoiseTex1;
        sampler2D _NormalMap;
        sampler2D _FoamTex;
            //Parameters
        half _CurrentAngle;
        half _CurrentSpeed;
        half _DistortionSpeed;
            //Timing
        half _TimeOffset;
        struct Input
        {
            float2 uv_DistorsionTex;
            float2 uv_NoiseTex1;
            float2 uv_NormalMap;
            float2 uv_FoamTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed2 dir = fixed2(cos(_CurrentAngle), sin(_CurrentAngle));
            fixed4 cFlow = tex2D (_DistorsionTex, float2(IN.uv_DistorsionTex.x + dir.x * (_CurrentSpeed * _TimeOffset), IN.uv_DistorsionTex.y + dir.y * (_CurrentSpeed * _TimeOffset)));
            fixed4 dFoam = tex2D(_FoamTex, float2(IN.uv_FoamTex.x + dir.x * (_CurrentSpeed / 3 * _TimeOffset) + _DistortionSpeed * cFlow.g, IN.uv_FoamTex.y + dir.y * (_CurrentSpeed / 3 * _TimeOffset) + _DistortionSpeed * cFlow.r));

            fixed4 c = tex2D(_NoiseTex1, float2(IN.uv_NoiseTex1.x + _DistortionSpeed * cFlow.r, IN.uv_NoiseTex1.y + _DistortionSpeed * cFlow.g)) * _Color;
            
            fixed4 normal = tex2D(_NormalMap, float2(IN.uv_NormalMap.x + dir.x * (_CurrentSpeed/3 * _TimeOffset) + _DistortionSpeed * cFlow.g, IN.uv_NormalMap.y + dir.y * (_CurrentSpeed/3 * _TimeOffset)  +_DistortionSpeed * cFlow.r));

           // c = c + 0.5 * dFoam * clamp(dFoam.g - dFoam.r, 0, 1) * clamp( cFlow.r - 2* cFlow.g, 0, 1);
           //c = c +  dFoam * (dFoam.r) *  clamp(1.8 * cFlow.r - 2 * cFlow.g, 0, 1);

            o.Normal = UnpackNormal(normal);
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }

        //void surf (Input IN, inout SurfaceOutputStandard o)
        //{
        //    fixed2 dir = fixed2(cos(_CurrentAngle), sin(_CurrentAngle));
        //    fixed4 cFlow = tex2D (_DistorsionTex, float2(IN.uv_DistorsionTex.x + dir.x * (_CurrentSpeed * _Time.x / _TimeScale), IN.uv_DistorsionTex.y + dir.y * (_CurrentSpeed * _Time.x / _TimeScale)));
        //    fixed4 dFoam = tex2D(_FoamTex, float2(IN.uv_FoamTex.x + dir.x * (_CurrentSpeed / 3 * _Time.x / _TimeScale) + _DistortionSpeed * cFlow.g, IN.uv_FoamTex.y + dir.y * (_CurrentSpeed / 3 * _Time.x / _TimeScale) + _DistortionSpeed * cFlow.r));

        //    fixed4 c = tex2D(_NoiseTex1, float2(IN.uv_NoiseTex1.x + _DistortionSpeed * cFlow.r, IN.uv_NoiseTex1.y + _DistortionSpeed * cFlow.g)) * _Color;
        //    
        //    fixed4 normal = tex2D(_NormalMap, float2(IN.uv_NormalMap.x + dir.x * (_CurrentSpeed/3 * _Time.x / _TimeScale) + _DistortionSpeed * cFlow.g, IN.uv_NormalMap.y + dir.y * (_CurrentSpeed/3 * _Time.x / _TimeScale)  +_DistortionSpeed * cFlow.r));

        //   // c = c + 0.5 * dFoam * clamp(dFoam.g - dFoam.r, 0, 1) * clamp( cFlow.r - 2* cFlow.g, 0, 1);
        //   //c = c +  dFoam * (dFoam.r) *  clamp(1.8 * cFlow.r - 2 * cFlow.g, 0, 1);

        //    o.Normal = UnpackNormal(normal);
        //    o.Albedo = c.rgb;
        //    o.Metallic = _Metallic;
        //    o.Smoothness = _Glossiness;
        //    o.Alpha = c.a;
        //}
        ENDCG
    }
    FallBack "Diffuse"
}
