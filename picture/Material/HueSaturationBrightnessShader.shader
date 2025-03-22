Shader "Custom/HueSaturationBrightnessShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _HueShift ("Hue Shift", Range(-180, 180)) = 0
        _Saturation ("Saturation", Range(0, 2)) = 1
        _Brightness ("Brightness", Range(0, 2)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        ZWrite Off // Prevents depth writing for transparency issues
        Blend SrcAlpha OneMinusSrcAlpha // Standard alpha blending
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            sampler2D _MainTex;
            float _HueShift;
            float _Saturation;
            float _Brightness;

            v2f vert (appdata_t v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float3 RGBtoHSV(float3 c) {
                float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
                float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
                float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));
                float d = q.x - min(q.w, q.y);
                float e = 1.0e-10;
                return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
            }

            float3 HSVtoRGB(float3 hsv) {
                float3 rgb = clamp(abs(fmod(hsv.x * 6.0 + float3(0.0, 4.0, 2.0), 6.0) - 3.0) - 1.0, 0.0, 1.0);
                return hsv.z * lerp(float3(1.0, 1.0, 1.0), rgb, hsv.y);
            }

            fixed4 frag(v2f i) : SV_Target {
                fixed4 col = tex2D(_MainTex, i.uv);
                float alpha = col.a;
                float3 hsv = RGBtoHSV(col.rgb);
                hsv.x = fmod(hsv.x + (_HueShift / 180), 1.0);
                hsv.y = saturate(hsv.y * _Saturation);
                hsv.z = saturate(hsv.z * _Brightness);
                col.rgb = HSVtoRGB(hsv);
                col.a = alpha; // Preserve original alpha
                
                return col;
            }
            ENDCG
        }
    }
}
