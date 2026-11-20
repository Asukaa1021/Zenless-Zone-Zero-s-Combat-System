Shader "Naiwa/Laugh Vertex Color"
{
    Properties
    {
        _Smoothness ("Smoothness", Range(0, 1)) = 0.35
        _SpecularStrength ("Specular strength", Range(0, 1)) = 0.02
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Cull Off
        CGPROGRAM
        #pragma surface surf StandardSpecular fullforwardshadows addshadow
        #pragma target 3.0

        struct Input
        {
            float4 color : COLOR;
        };

        half _Smoothness;
        half _SpecularStrength;

        void surf (Input input, inout SurfaceOutputStandardSpecular output)
        {
            float3 albedo = input.color.rgb;
            #if defined(UNITY_COLORSPACE_GAMMA)
                albedo = LinearToGammaSpace(albedo);
            #endif
            output.Albedo = albedo;
            output.Specular = _SpecularStrength.xxx;
            output.Smoothness = _Smoothness;
            output.Alpha = 1;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
