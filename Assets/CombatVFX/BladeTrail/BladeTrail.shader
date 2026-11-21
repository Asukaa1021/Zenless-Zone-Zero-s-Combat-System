Shader "CombatVFX/Blade Trail"
{
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

            struct Attributes
            {
                float4 vertex : POSITION;
                fixed4 color : COLOR;
                half2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 position : SV_POSITION;
                fixed4 color : COLOR;
                half width : TEXCOORD0;
            };

            Varyings Vert(Attributes input)
            {
                Varyings output;
                output.position = UnityObjectToClipPos(input.vertex);
                output.color = input.color;
                output.width = input.uv.y;
                return output;
            }

            fixed4 Frag(Varyings input) : SV_Target
            {
                half edge = 1.0 - smoothstep(0.9, 1.0, input.width);
                half body = smoothstep(0.06, 0.76, input.width) * edge;
                half highlight = smoothstep(0.71, 0.9, input.width);
                half alpha = body * input.color.a * 0.93;
                fixed3 color = lerp(input.color.rgb, fixed3(1, 1, 1), highlight * 0.66);
                return fixed4(color * alpha, alpha);
            }
            ENDCG
        }
    }
}

