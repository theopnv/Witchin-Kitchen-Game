Shader "Custom/PunchWave"
{
    Properties
    {
        _Alpha("Alpha", Float) = 1.0
        _Radius("Radius of the wave", Float) = 1.0
        _RampTex("Ramp texture (RGBA)", 2D) = "white" {}
        _RampTexSmall("Ramp texture (RGBA)", 2D) = "white" {}
        _RampTexInterp("Interp between small and big ramp texture", Range(0,1)) = 1.0
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // Use shader model 3.0 target, to get nicer looking lighting
            #pragma target 3.0

            struct appdata_t
            {
                float4 vertex   : POSITION;
            };

            struct v2f
            {
                float4 vertex  : SV_POSITION;
                float4 localPos : TEXCOORD1;
            };

            half _Alpha;
            half _Radius;
            sampler2D _RampTex;
            sampler2D _RampTexSmall;
            half _RampTexInterp;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.localPos = v.vertex;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = fixed4(0.0f, 0.0f, 0.0f, 0.0f);

                float polarDist = sqrt(i.localPos.x * i.localPos.x + i.localPos.z * i.localPos.z);

                if (polarDist > _Radius)
                    col.a = 0;
                else
                {
                    half2 rampUV = half2(1.0 - polarDist / _Radius, 0.5);
                    fixed4 small = tex2D(_RampTexSmall, rampUV);
                    fixed4 big = tex2D(_RampTex, rampUV);

                    col = lerp(small, big, _RampTexInterp);
                    col.a *= _Alpha;
                }

                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
