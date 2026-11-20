Shader "CombatVFX/Unlit Flipbook"
{
    Properties
    {
        _MainTex ("Flipbook", 2D) = "white" {}
        _Tint ("Tint", Color) = (1, 1, 1, 1)
        [Toggle] _Recolor ("Recolor", Float) = 0
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        Cull Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            fixed4 _Tint;
            half _Recolor;

            struct Attributes
            {
                float4 position : POSITION;
                fixed4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 position : SV_POSITION;
                fixed4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            Varyings Vert(Attributes input)
            {
                Varyings output;
                output.position = UnityObjectToClipPos(input.position);
                output.color = input.color;
                output.uv = input.uv;
                return output;
            }

            fixed4 Frag(Varyings input) : SV_Target
            {
                fixed4 sampleColor = tex2D(_MainTex, input.uv);
                half brightness = max(sampleColor.r, max(sampleColor.g, sampleColor.b));
                half core = brightness * brightness * brightness * 0.45;
                fixed3 colored = brightness * lerp(_Tint.rgb, fixed3(1, 1, 1), core);
                fixed3 rgb = lerp(sampleColor.rgb, colored, _Recolor) * input.color.rgb;
                half edgeFade = smoothstep(0.0, 0.2, input.uv.y) * smoothstep(0.0, 0.07, 1.0 - input.uv.y);
                fixed alpha = sampleColor.a * input.color.a * _Tint.a * lerp(1.0, edgeFade, _Recolor);
                return fixed4(rgb * alpha, alpha);
            }
            ENDCG
        }
    }
}

