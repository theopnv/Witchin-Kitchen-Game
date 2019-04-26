// From https://lindenreid.wordpress.com/2017/12/15/simple-water-shader-in-unity/

Shader "Custom/LiquidShader"
{
    Properties
    {
        _Color("Color", Color) = (1, 1, 1, 1)
        _EdgeColor("Edge Color", Color) = (1, 1, 1, 1)
        _DepthFactor("Depth Factor", float) = 1.0
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 200

        Pass
        {
            CGPROGRAM
            #include "UnityCG.cginc"

            #pragma vertex vert
            #pragma fragment frag

            // Properties
            float4 _Color;
            float4 _EdgeColor;
            float  _DepthFactor;
            sampler2D _CameraDepthTexture;

            struct vertexInput
            {
                float4 vertex : POSITION;
                float4 texCoord : TEXCOORD1;
            };

            struct vertexOutput
            {
                float4 pos : SV_POSITION;
                float4 texCoord : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
            };

            vertexOutput vert(vertexInput input)
            {
                vertexOutput output;

                // convert to world space
                output.pos = UnityObjectToClipPos(input.vertex);

                // compute depth
                output.screenPos = ComputeScreenPos(output.pos);

                // texture coordinates 
                output.texCoord = input.texCoord;

                return output;
            }

            float4 frag(vertexOutput input) : COLOR
            {
                // apply depth texture
                float4 depthSample = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, input.screenPos);
                float depth = LinearEyeDepth(depthSample).r;

                // create foamline
                float foamLine = 1 - saturate(_DepthFactor * (depth - input.screenPos.w));

                //float4 col = _Color * foamRamp;
                float4 col = lerp(_Color, _EdgeColor, foamLine);
                return col;
            }

            ENDCG
        }
    }
}
