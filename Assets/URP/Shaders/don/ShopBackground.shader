Shader "Don/ShopBackground"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _LinesTex ("Lines Texture", 2D) = "white" {}
        _SpinSpeed ("Spin Speed", Float) = 50
        _SpinAmount ("SpinAmount", Float) = 0
        _Contrast ("Contrast", Float) = 0
        _PixelSizeFactor ("Pixel Size Factor", Float) = 700
        _SpinEase ("Spin Easing", Float) = 0.5
        
        _Color1 ("Color 1", Color) = (0, 0, 0)
        _Color2 ("Color 2", Color) = (0, 0, 0)
        _Color3 ("Color 3", Color) = (0, 0, 0)
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 scr_pos : TEXCOORD1;
            };

            sampler2D _MainTex;
            sampler2D _LinesTex;
            float _SpinTime;
            float _SpinSpeed;
            float _Curvature;
            float _SpinAmount;
            float _Contrast;
            float _PixelSizeFactor;
            float _SpinEase;

            half4 _Color1;
            half4 _Color2;
            half4 _Color3;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.scr_pos = ComputeScreenPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                _SpinTime = _Time * _SpinSpeed;
                const float pixel_size = length(i.uv) / _PixelSizeFactor;

                float2 uv = i.uv - 0.5;
                uv = floor(uv * (1 / pixel_size)) * pixel_size;

                float uv_len = length(uv);

                float speed = (_SpinTime * _SpinEase * 0.2) + 902.2;
                float new_pixel_angle = (atan2(uv.x, uv.y)) + speed - _SpinEase * 20. * (1. * _SpinAmount * uv_len + (1. - 1. * _SpinAmount));
                float2 mid = (i.uv / length(i.uv)) / 2.;
                uv = (float2((uv_len * cos(new_pixel_angle) + mid.x), (uv_len * sin(new_pixel_angle) + mid.y)) - mid);

                uv *= 30.;
                speed = _Time * (2);

                float2 uv2 = float2(uv.x + uv.y, 0);
                
                for(int j=0; j < 8; j++) {
		            uv2 += sin(max(uv.x, uv.y)) + uv;
		            uv  += 0.5 * float2(cos(3.1123314 + 0.353 * uv2.y + speed * 0.931121), sin(uv2.x - 0.113 * speed));
        		    uv  -= 0.9 * cos(uv.x + uv.y) - 0.4 * sin(uv.x * 0.711 - uv.y);
                }
                
                float contrast_mod = (0.25 * _Color1 + 0.5 * _SpinAmount + 1.2);
                float paint_res =min(2., max(0.,length(uv)*(0.035)*contrast_mod));
                float c1p = max(0.,1. - contrast_mod*abs(1.-paint_res));
                float c2p = max(0.,2. - contrast_mod*abs(paint_res));
                float c3p = (1. - min(1., c1p + c2p)) ;

                half4 ret_color = (0.2/_Contrast)*_Color1 + (1. - 0.3/_Contrast)*(_Color1*c1p + _Color2*c2p + half4(c3p*_Color3.rgb * 20, c3p*_Color1.a));;
                
                return ret_color;
            }
            ENDCG
        }
    }
}
