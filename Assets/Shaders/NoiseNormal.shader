Shader "Custom/NoiseNormal"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0

        _BumpMap    ("Normal Map"  , 2D         ) = "bump" {}
		_BumpScale  ("Normal Scale", Range(0, 1)) = 1.0

        _BumpMap2    ("Normal Map"  , 2D         ) = "bump" {}
		_BumpScale2  ("Normal Scale", Range(0, 1)) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        fixed4 _Color;
        sampler2D _MainTex;
        half _Glossiness;
        half _Metallic;

        sampler2D _BumpMap;
		half _BumpScale;

        sampler2D _BumpMap2;
		half _BumpScale2;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_BumpMap2;
        };


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

            fixed3 n  = UnpackScaleNormal(tex2D(_BumpMap, IN.uv_MainTex), _BumpScale);
            fixed3 n2 = UnpackScaleNormal(tex2D(_BumpMap2, IN.uv_BumpMap2), _BumpScale2);
            o.Normal = BlendNormals(n, n2);
            // o.Normal = n;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
