Shader "Custom/GrassShader" 
{
    Properties
    {
        _FreezeColor("Freeze color", Color) = (1, 1, 1, 1)
        _FreezeFactor("Freeze factor", Float) = 0.0

        _Color("Color", Color) = (1, 1, 1, 1)
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(0,1)) = 0.0

        _CurTime("Current simulation time", Float) = 0.0
        _DisplacementStrength("DisplacementStrength", Float) = 1.0
        _Flexibility("Flexibility", Float) = 1.0
        _WindStrength("Wind strength", Float) = 2.0
        _WindDirection("Wind direction", Vector) = (1.0, 0.0, 0.0, 0.0)
        _RollingWindPositionScale("Rolling wind position scale", Float) = 0.001
        _RollingWindTex("Rolling wind texture", 2D) = "black" {}
        _RollingWindOffset("Rolling wind texture", Vector) = (0.0, 0.0, 0.0, 0.0)
        _MeshHeight("Height of the mesh", Float) = 1.0
        _Scale("Scale", Vector) = (1.0, 1.0, 1.0, 1.0)
        _Displacement("Displacement R,G = direction, B = intensity, A is unused", 2D) = "black"
        _Bounds("X,Y = back left, Z,W = width,depth", Vector) = (0.0, 0.0, 0.0, 0.0)

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

        float4 _FreezeColor;
        float _FreezeFactor;

        half _Glossiness;
        half _Metallic;

        float _CurTime;
        float _DisplacementStrength;
        float _Flexibility;
        float _WindStrength;
        float4 _WindDirection;
        float _RollingWindPositionScale;
        sampler2D _RollingWindTex;
        float4 _RollingWindOffset;
        float _MeshHeight;
        sampler2D _Displacement;
        float4 _Scale;
        float4 _Bounds;

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
            float3 addVertex = float3(0.0f, 0.0f, 0.0f);
            float vertexLength = length(outVertex.xyz);

            float4 instancePosition = UNITY_ACCESS_INSTANCED_PROP(Props, _InstancePosition);
            float2 rollingWindSampleCoords = instancePosition.xz * _RollingWindPositionScale + _RollingWindOffset;
            fixed4 rollingWind = tex2Dlod(_RollingWindTex, float4(rollingWindSampleCoords.xy, 0, 0));
            float windStrength = min(1.0f, rollingWind.x);

            float bendFactor = min(1.0f, max(0.0f, v.vertex.y) / _MeshHeight);
            bendFactor *= bendFactor;
            windStrength *= bendFactor;

            float4 localWindDirection = mul(unity_WorldToObject, _WindDirection);
            addVertex.xyz += localWindDirection.xyz * windStrength * _WindStrength;

         
            float2 displacementUV = float2(0.0f, 0.0f);
            displacementUV.x = (instancePosition.x - _Bounds.x) / _Bounds.z;
            displacementUV.y = (instancePosition.z - _Bounds.y) / _Bounds.w;

            fixed4 displacement = tex2Dlod(_Displacement, float4(displacementUV.xy, 0, 0));
            fixed2 packedDisplacementDirection = displacement.xy;
            fixed2 displacementDirection = (packedDisplacementDirection * 2.0f) - fixed2(1.0f, 1.0f);
            addVertex.xz += displacementDirection.xy * displacement.z * bendFactor * _DisplacementStrength;


            outVertex.xyz = normalize(outVertex.xyz + addVertex * _Flexibility) * vertexLength * _Scale.xyz;
            o.vertex = outVertex;
            v.vertex = outVertex;
        }

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            float4 color = UNITY_ACCESS_INSTANCED_PROP(Props, _Color);
            color = lerp(color, _FreezeColor, _FreezeFactor);

            fixed4 c = color;
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
}