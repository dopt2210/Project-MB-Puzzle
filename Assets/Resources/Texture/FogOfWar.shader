Shader "Unlit/FogOfWar"
{
    Properties
    {
        _MainTex ("Base Map", 2D) = "white" {}
        _FogTex ("Fog Mask", 2D) = "black" {}
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;
            sampler2D _FogTex;

            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f { float2 uv : TEXCOORD0; float4 vertex : SV_POSITION; };

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                fixed4 mapColor = tex2D(_MainTex, i.uv);
                fixed4 fog = tex2D(_FogTex, i.uv);
                return mapColor * (1.0 - fog.a);
            }
            ENDCG
        }
    }
}
