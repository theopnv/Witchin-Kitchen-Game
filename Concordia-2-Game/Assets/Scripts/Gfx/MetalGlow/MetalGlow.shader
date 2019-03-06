Shader "Custom/MetalGlow"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0

        _GlowHeight("Height of the cauldron", Float) = 1.0
        _GlowColor("Glow color (RGBA)", 2D) = "white" {}
        _GlowEmissive("Glow emissive (R)", 2D) = "black" {}
        _GlowColorSmall("Glow color small (RGBA)", 2D) = "white" {}
        _GlowEmissiveSmall("Glow emissive small (R)", 2D) = "black" {}
        _GlowIntensity("_GlowIntensity", Range(0,1)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows
        #pragma vertex vert

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
            float3 localPos;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        sampler2D _GlowColor;
        sampler2D _GlowEmissive;
        sampler2D _GlowColorSmall;
        sampler2D _GlowEmissiveSmall;
        half _GlowHeight;
        half _GlowIntensity;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void vert(inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.localPos = v.vertex.xyz;
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;

            // Glow albedo and emission
            half heightRatio = (IN.localPos.y + _GlowHeight / 2.0f) / _GlowHeight;
            half2 glowUV = half2(heightRatio, 0.5);
            fixed4 glowColor = tex2D(_GlowColor, glowUV);
            fixed glowEmission = tex2D(_GlowEmissive, glowUV).r;

            // Glow albedo and emission for small
            fixed4 glowColorSmall = tex2D(_GlowColorSmall, glowUV);
            fixed glowEmissionSmall = tex2D(_GlowEmissive, glowUV).r;

            // Lerp between big and small
            fixed4 interpedGlowColor = lerp(glowColorSmall, glowColor, _GlowIntensity);
            fixed interpedGlowEmission = lerp(glowEmissionSmall, glowEmission, _GlowIntensity);

            o.Albedo = lerp(c.rgb, interpedGlowColor.rgb, interpedGlowColor.a);
            o.Emission = o.Albedo * interpedGlowEmission;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
