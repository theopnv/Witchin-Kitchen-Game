Shader "Custom/GrassShader" 
{
    Properties
    {
        _Color("Color", Color) = (1, 1, 1, 1)
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(0,1)) = 0.0

        _CurTime("Current simulation time", Float) = 0.0
        _WindStrength("Wind strength", Float) = 2.0
        _WindDirection("Wind direction", Vector) = (1.0, 0.0, 0.0, 0.0)
        _RollingWindPositionScale("Rolling wind position scale", Float) = 0.001
        _RollingWindTex("Rolling wind texture", 2D) = "black" {}
        _RollingWindOffset("Rolling wind texture", Vector) = (0.0, 0.0, 0.0, 0.0)
        _MeshHeight("Height of the mesh", Float) = 1.0

        _InstancePosition("Instance position", Vector) = (1.0, 0.0, 0.0, 0.0)
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 200

        CGPROGRAM

        #pragma target 3.0
        #pragma vertex vert
        #pragma surface surf Standard addshadow fullforwardshadows
        #pragma multi_compile_instancing

        #include "UnityCG.cginc"

        half _Glossiness;
        half _Metallic;

        float _CurTime;
        float _WindStrength;
        float4 _WindDirection;
        float _RollingWindPositionScale;
        sampler2D _RollingWindTex;
        float4 _RollingWindOffset;
        float _MeshHeight;

        struct Input
        {
            float4 vertex : SV_POSITION;
            UNITY_VERTEX_INPUT_INSTANCE_ID
        };

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
        UNITY_DEFINE_INSTANCED_PROP(float4, _InstancePosition)
        UNITY_INSTANCING_BUFFER_END(Props)

        void vert(inout appdata_full v, out Input o)
        {
            UNITY_SETUP_INSTANCE_ID(v);
            UNITY_TRANSFER_INSTANCE_ID(v, o);

            float4 outVertex = v.vertex;
            float vertexLength = length(v.vertex);

            float4 instancePosition = UNITY_ACCESS_INSTANCED_PROP(Props, _InstancePosition);
            float2 rollingWindSampleCoords = instancePosition.xz * _RollingWindPositionScale + _RollingWindOffset;
            fixed4 rollingWind = tex2Dlod(_RollingWindTex, float4(rollingWindSampleCoords.xy, 0, 0));
            float windStrength = min(1.0f, rollingWind.x);

            float bendFactor = min(1.0f, max(0.0f, v.vertex.y) / _MeshHeight);
            bendFactor *= bendFactor;
            windStrength *= bendFactor;

            float4 localWindDirection = mul(unity_WorldToObject, _WindDirection);
            outVertex.xyz += localWindDirection.xyz * windStrength * _WindStrength;
            outVertex = normalize(outVertex) * vertexLength;

            o.vertex = outVertex;
            v.vertex = outVertex;
        }

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            float4 color = UNITY_ACCESS_INSTANCED_PROP(Props, _Color);

            fixed4 c = color;
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
}