Shader "Don/ShadowDon"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color (RGBA)", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Cull Off
        Lighting Off
        ZWrite Off
        Offset -1, -1
        Fog
        {
            Mode Off
        }
        ColorMask RGB
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
            float4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv); // multiply by _Color
                col = float4(col.r, col.g, col.b, 0.1);
                return col;
            }
            ENDCG
        }
    }
}
