// Taken from https://github.com/Shrimpey/UltimateOutline

// This version of the shader does support shadows, but it does not support transparent outlines

Shader "Custom/Outline"
{
    Properties
    {
        _Color("Main Color", Color) = (0.5,0.5,0.5,1)
        _MainTex("Texture", 2D) = "white" {}

        _OutlineColor("Outline color", Color) = (1,0,0,0.5)
        _OutlineWidth("Outlines width", Range(0.0, 2.0)) = 0.15

        _Angle("Switch shader on angle", Range(0.0, 180.0)) = 89
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        Cull Front
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 normal : NORMAL;
            };

            uniform float4 _OutlineColor;
            uniform float _OutlineWidth;

            uniform sampler2D _MainTex;
            uniform float4 _Color;
            uniform float _Angle;

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                appdata original = v;

                float3 scaleDir = normalize(v.vertex.xyz - float4(0,0,0,1));
                //This shader consists of 2 ways of generating outline that are dynamically switched based on demiliter angle
                //If vertex normal is pointed away from object origin then custom outline generation is used (based on scaling along the origin-vertex vector)
                //Otherwise the old-school normal vector scaling is used
                //This way prevents weird artifacts from being created when using either of the methods
                if (degrees(acos(dot(scaleDir.xyz, v.normal.xyz))) > _Angle)
                {
                    v.vertex.xyz += normalize(v.normal.xyz) * _OutlineWidth;
                }
                else
                {
                    v.vertex.xyz += scaleDir * _OutlineWidth;
                }

                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            half4 frag(v2f i) : COLOR
            {
                float4 color = _OutlineColor;
                color.a = 1;
                return color;
            }

            ENDCG
        }
    }
    Fallback "Diffuse"
}
