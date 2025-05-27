Shader "Custom/RevealCircle"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _RevealPos ("Reveal Pos", Vector) = (0, 0, 0, 0)
        _Radius ("Radius", Float) = 0.1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        ZWrite Off Cull Off ZTest Always
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _RevealPos; // x,y: pos in UV space (0~1)
            float _Radius;

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float dist = distance(i.uv, _RevealPos.xy);
                if (dist < _Radius)
                {
                    return float4(0, 0, 0, 0); // Transparent
                }
                return tex2D(_MainTex, i.uv); // Keep original
            }
            ENDCG
        }
    }
}
