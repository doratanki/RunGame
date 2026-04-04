Shader "Skybox/Gradient"
{
    Properties
    {
        _TopColor    ("Top Color",    Color) = (0.05, 0.05, 0.15, 1)
        _MiddleColor ("Middle Color", Color) = (0.85, 0.35, 0.05, 1)
        _BottomColor ("Bottom Color", Color) = (0.95, 0.65, 0.15, 1)
        _MiddlePos   ("Horizon Position", Range(0, 1)) = 0.4
        _Exponent    ("Gradient Sharpness", Range(0.1, 10)) = 2.0
    }

    SubShader
    {
        Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
        Cull Off ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 _TopColor;
            fixed4 _MiddleColor;
            fixed4 _BottomColor;
            float  _MiddlePos;
            float  _Exponent;

            struct appdata { float4 vertex : POSITION; };
            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 dir : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.dir = v.vertex.xyz;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float t = normalize(i.dir).y * 0.5 + 0.5; // 0=下, 1=上
                fixed4 col;
                if (t > _MiddlePos)
                    col = lerp(_MiddleColor, _TopColor, pow((t - _MiddlePos) / (1.0 - _MiddlePos), _Exponent));
                else
                    col = lerp(_BottomColor, _MiddleColor, pow(t / _MiddlePos, _Exponent));
                return col;
            }
            ENDCG
        }
    }
}