Shader "Unlit/Glass Liquid"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _AlbedoColor ("Albedo Color", Color) = (1., 1., 1., 1.)
        _FillRatio ("Fill Ratio", Range(0, 1.001)) = 0
        _MinFillY("Min Fill Y", Float) = 0
        _MaxFillY("Max Fill Y", Float) = 1
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off

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
                float vertexY : TEXCOORD2;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _AlbedoColor;
            float _FillRatio;
            float _MinFillY;
            float _MaxFillY;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.vertexY = v.vertex.y;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float yRatio = saturate((i.vertexY - _MinFillY) / (_MaxFillY - _MinFillY)) + 0.001;
                clip(_FillRatio - yRatio);

                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv) * _AlbedoColor;
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
