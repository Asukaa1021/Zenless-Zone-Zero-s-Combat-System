Shader "CombatVFX/Drakkar Ultimate Trail"
{
    Properties
    {
        _Texture ("Trail Mask", 2D) = "white" {}
        _Gold ("Gold", Color) = (1, 0.38, 0.035, 0.45)
        _GoldCore ("Gold Highlight", Color) = (1, 0.84, 0.36, 1)
        _Blue ("Blue", Color) = (0.08, 0.36, 1, 0.45)
        _BlueCore ("Blue Highlight", Color) = (0.31, 0.78, 1, 1)
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #include "UnityCG.cginc"

            sampler2D _Texture;
            half4 _Gold;
            half4 _GoldCore;
            half4 _Blue;
            half4 _BlueCore;

            struct Varyings
            {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings Vert(appdata_base input)
            {
                Varyings output;
                output.position = UnityObjectToClipPos(input.vertex);
                output.uv = input.texcoord.xy;
                return output;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                half mask = tex2D(_Texture, input.uv).r;
                half detail = mask * mask * mask;

                // The blue edge dips into the gold ribbon along the swing.
                half boundary = 0.57h + 0.13h * sin(input.uv.x * 22.0);
                half blueEdge = smoothstep(boundary - 0.035h, boundary + 0.035h, input.uv.y);
                half4 gold = lerp(_Gold, _GoldCore, detail);
                half4 blue = lerp(_Blue, _BlueCore, detail);
                half4 color = lerp(gold, blue, blueEdge);
                color.a *= mask;
                return color;
            }
            ENDCG
        }
    }
}
