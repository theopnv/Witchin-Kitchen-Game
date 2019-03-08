// Adapted from https://halisavakis.com/my-take-on-shaders-dissolve-shader

Shader "Custom/Dissolve"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _EmissiveColor("Emissive Color (RGB)", Color) = (0,0,0,0)
        _EmissiveColorRamp("Emissive Color Ramp (RGB)", 2D) = "white" {}
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _SliceGuide("Slice Guide (RGB)", 2D) = "white" {}
        _SliceAmount("Slice Amount", Range(0.0, 1.0)) = 0
        _SpinSpeed("Spin Speed", Range(0.0, 10.0)) = 0
 
        _BurnSize("Burn Size", Range(0.0, 1.0)) = 0.15
        _BurnRamp("Burn Ramp (RGB)", 2D) = "white" {}
        _BurnColor("Burn Color", Color) = (1,1,1,1)
 
        _EmissionAmount("Emission amount", float) = 2.0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
        Cull Off
        CGPROGRAM
        #pragma surface surf Lambert addshadow
        #pragma target 3.0
 
        fixed4 _Color;
        fixed4 _EmissiveColor;
        sampler2D _MainTex;
        sampler2D _EmissiveColorRamp;
        sampler2D _SliceGuide;
        sampler2D _BumpMap;
        sampler2D _BurnRamp;
        fixed4 _BurnColor;
        float _BurnSize;
        float _SliceAmount;
        float _EmissionAmount;
        float _SpinSpeed;
 
        struct Input
        {
            float2 uv_MainTex;
        };
 
 
        void surf (Input IN, inout SurfaceOutput o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            half test = tex2D(_SliceGuide, IN.uv_MainTex + float2(_SliceAmount, _SliceAmount) * _SpinSpeed).rgb - _SliceAmount;
            clip(test);
 
            o.Albedo = c.rgb;
            o.Alpha = c.a;

            o.Emission = _EmissiveColor.rgb * tex2D(_EmissiveColorRamp, float2(_SliceAmount, 0)) * _EmissionAmount;

            o.Emission = tex2D(_BurnRamp, float2(1.0 - test * (1 / _BurnSize), 0)) * _BurnColor * _EmissionAmount;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
