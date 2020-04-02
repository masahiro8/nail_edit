Shader "Custom/NoiseSpec"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        float random (fixed2 p) {
            //0~1未満の乱数を返す→0.5以上の乱数を返す
            float rangeAll = 0.6;//ラメの明るさの分布幅 0〜1未満の値を指定する
            float shift = 0.5; //ラメの集団全体の明るさをどれぐらい持ち上げるか 0〜1未満の値を指定する
            float rand = frac(sin(dot(p, fixed2(12.9898, 78.233))) * 43758.5453123) * rangeAll + shift;
            float minBrightness = 0.5;
            float rangeMin = rangeAll * minBrightness + shift; // 黒い点をなくす
            if (rand <= rangeMin){ return rand = rangeMin; }
            return rand;
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float b = random(floor(IN.uv_MainTex * 300));
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb * b;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
