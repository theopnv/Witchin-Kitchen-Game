Shader "Custom/MinimalInstancedShader" 
{
    Properties
    {
        _Color("Color", Color) = (1, 1, 1, 1)
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(0,1)) = 0.0

        _CurTime("Current simulation time", Float) = 0.0
        _WindDirection("Wind direction", Vector) = (1.0, 0.0, 0.0, 0.0)
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
        #pragma surface surf Standard fullforwardshadows
        #pragma multi_compile_instancing

        #include "UnityCG.cginc"

        sampler2D _MainTex;
        half _Glossiness;
        half _Metallic;

        float _CurTime;
        float4 _WindDirection;
        sampler2D _RollingWindTex;
        float4 _RollingWindOffset;
        float _MeshHeight;

		struct Input
		{
			float4 vertex : SV_POSITION;
            float2 uv_MainTex;
		};

		UNITY_INSTANCING_BUFFER_START(Props)
		UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
		UNITY_DEFINE_INSTANCED_PROP(float4, _InstancePosition)
		UNITY_INSTANCING_BUFFER_END(Props)

		void vert(inout appdata_full v)
		{
			UNITY_SETUP_INSTANCE_ID(v);

            float4 outVertex = v.vertex;

            float4 instancePosition = UNITY_ACCESS_INSTANCED_PROP(Props, _InstancePosition);
            float offset = dot(instancePosition.xyz, float3(5.0f, 5.0f, 5.0f));
            float windStrength = sin(_CurTime + offset) + cos(_CurTime + offset);
            windStrength *= 0.4f;

            //windStrength = 0.0f;
            float2 rollingWindSampleCoords = instancePosition.xz * 0.001f + _RollingWindOffset;
            fixed4 rollingWind = tex2Dlod(_RollingWindTex, float4(rollingWindSampleCoords.xy, 0, 0));
            windStrength += rollingWind.x;
            windStrength = min(1.0f, windStrength);

            float bendFactor = (v.vertex.y / _MeshHeight);
            bendFactor *= bendFactor;
            windStrength *= bendFactor;

            float4 localWindDirection = mul(unity_WorldToObject, _WindDirection);
            outVertex.xyz += localWindDirection.xyz * windStrength * 2.0f;

			v.vertex = outVertex;
		}

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            float4 color = UNITY_ACCESS_INSTANCED_PROP(Props, _Color);

            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * color;
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
		ENDCG
	}
}