Shader "Unlit/Outline2"
{
    Properties
    {
        _OutlineColor("Outline color", Color) = (1, 1, 1, 1)
        _OutlineEmissiveFactor("Outline emissive factor", Float) = 1.0
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        Cull Front
        CGPROGRAM
        #pragma surface surf Lambert vertex:vert

        struct Input
        {
            float2 uv_MainTex;
        };

        float4 _OutlineColor;
        float _OutlineEmissiveFactor;

        void vert (inout appdata_full v)
        {
            v.normal.xyz = v.normal * -1;
        }

        void surf (Input IN, inout SurfaceOutput o)
        {
            o.Albedo = float3(0.0, 0.0, 0.0);
            o.Emission = _OutlineColor.rgb * _OutlineEmissiveFactor;
            o.Alpha = 1.0;
        }
        ENDCG
    } 
    Fallback "Diffuse"
}
