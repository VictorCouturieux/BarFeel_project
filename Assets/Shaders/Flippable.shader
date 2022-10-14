Shader "Unlit/Flippable"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _AlbedoColor ("Albedo Color", Color) = (1., 1., 1., 1.)
        _AlphaMultiplier ("Alpha Multiplier", Range(0, 1)) = 1.
        _XUVMultiplier ("X UV Multiplier", Float) = 1.
        _YUVMultiplier ("Y UV Multiplier", Float) = 1.
    }

    SubShader
    {
        Tags { 
            "RenderType"="Transparent"
            "Queue"="Transparent"
        }
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _AlbedoColor;
            float _AlphaMultiplier;
            float _XUVMultiplier;
            float _YUVMultiplier;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv *= float2(_XUVMultiplier, _YUVMultiplier);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv) * _AlbedoColor;
                col.a *= _AlphaMultiplier;
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}