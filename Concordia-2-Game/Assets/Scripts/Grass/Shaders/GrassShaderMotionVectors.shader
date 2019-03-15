Shader "Unlit/GrassShaderMotionVectors"
{
    Properties
    {

    }

    SubShader
    {
        Pass
        {
            Name "Motion Vectors"
            Tags { "LightMode" = "MotionVectors" }

            ZTest LEqual Cull Back ZWrite On

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment FragMotionVectors
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"

            float4x4 _NonJitteredVP;
            float4x4 _PreviousVP;

            // Constant
            float _MeshHeight;
            sampler2D _RollingWindTex;
            float _RollingWindPositionScale;
            float4 _Bounds;

            // Variable
            sampler2D _Displacement;
            float _CurTime;
            float _DisplacementStrength;
            float _Flexibility;
            float _WindStrength;
            float4 _WindDirection;
            float4 _RollingWindOffset;
            float4 _Scale;

            sampler2D _PrevDisplacement;
            float _PrevCurTime;
            float _PrevDisplacementStrength;
            float _PrevFlexibility;
            float _PrevWindStrength;
            float4 _PrevWindDirection;
            float4 _PrevRollingWindOffset;
            float4 _PrevScale;

            struct appdata
            {
                float4 vertex : POSITION;
                uint instanceID : SV_InstanceID;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 v0 : TEXCOORD0;
                float4 v1 : TEXCOORD1;
            };

            UNITY_INSTANCING_BUFFER_START(Props)
            UNITY_DEFINE_INSTANCED_PROP(float4, _InstancePosition)
            UNITY_INSTANCING_BUFFER_END(Props)

            float4 animate
            (
                float4 vert,
                float4 instancePosition,
                float displacementStrength,
                float flexibility,
                float windStr,
                float4 windDirection,
                float4 rollingWindOffset,
                float4 scale,
                sampler2D displacementTex
            )
            {
                float4 outVertex = vert;
                float3 addVertex = float3(0.0f, 0.0f, 0.0f);
                float vertexLength = length(outVertex.xyz);

                float2 rollingWindSampleCoords = instancePosition.xz * _RollingWindPositionScale + rollingWindOffset;
                fixed4 rollingWind = tex2Dlod(_RollingWindTex, float4(rollingWindSampleCoords.xy, 0, 0));
                float windStrength = min(1.0f, rollingWind.x);

                float bendFactor = min(1.0f, max(0.0f, vert.y) / _MeshHeight);
                bendFactor *= bendFactor;
                windStrength *= bendFactor;

                float4 localWindDirection = mul(unity_WorldToObject, windDirection);
                addVertex.xyz += localWindDirection.xyz * windStrength * windStr;


                float2 displacementUV = float2(0.0f, 0.0f);
                displacementUV.x = (instancePosition.x - _Bounds.x) / _Bounds.z;
                displacementUV.y = (instancePosition.z - _Bounds.y) / _Bounds.w;

                fixed4 displacement = tex2Dlod(displacementTex, float4(displacementUV.xy, 0, 0));
                fixed2 packedDisplacementDirection = displacement.xy;
                fixed2 displacementDirection = (packedDisplacementDirection * 2.0f) - fixed2(1.0f, 1.0f);
                addVertex.xz += displacementDirection.xy * displacement.z * bendFactor * displacementStrength;


                outVertex.xyz = normalize(outVertex.xyz + addVertex * flexibility) * vertexLength * scale.xyz;
                return outVertex;
            }

            v2f vert(appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);

                float4 instancePosition = UNITY_ACCESS_INSTANCED_PROP(Props, _InstancePosition);

                float4 v0 = animate(v.vertex, instancePosition, _PrevDisplacementStrength, _PrevFlexibility, _PrevWindStrength, _PrevWindDirection, _PrevRollingWindOffset, _PrevScale, _PrevDisplacement);
                float4 v1 = animate(v.vertex, instancePosition, _DisplacementStrength, _Flexibility, _WindStrength, _WindDirection, _RollingWindOffset, _Scale, _Displacement);

                o.vertex = UnityObjectToClipPos(v1);
                o.v0 = mul(_PreviousVP, mul(unity_ObjectToWorld, v0));;
                o.v1 = mul(_NonJitteredVP, mul(unity_ObjectToWorld, v1));
                return o;
            }

            float4 FragMotionVectors(v2f i) : SV_Target
            {
                // return half4(1.0, 1.0, 1.0, 1.0);

                float3 hp0 = i.v0.xyz / i.v0.w;
                float3 hp1 = i.v1.xyz / i.v1.w;

                float2 vp0 = (hp0.xy + 1) / 2;
                float2 vp1 = (hp1.xy + 1) / 2;

                #if UNITY_UV_STARTS_AT_TOP
                vp0.y = 1 - vp0.y;
                vp1.y = 1 - vp1.y;
                #endif

                return half4(vp1 - vp0, 0, 1);
            }

            ENDCG
        }
    }
}
