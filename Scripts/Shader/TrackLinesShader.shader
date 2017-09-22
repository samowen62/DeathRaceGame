Shader "Custom/TrackLinesShader" {
    Properties{
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _NormalTex("Normap map", 2D) = "bump" {}
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(0,1)) = 0.0
        _ScrollXSpeed("X scroll speed", Range(-10, 10)) = 0
        _ScrollYSpeed("Y scroll speed", Range(-10, 10)) = -0.4 }
        SubShader{
            Tags { "RenderType" = "Opaque" }
            LOD 200

            CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _NormalTex;
        fixed _ScrollXSpeed;
        fixed _ScrollYSpeed;

        struct Input {
            float2 uv_MainTex;
            float2 uv_NormalTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutputStandard o) {
            fixed offsetX = _ScrollXSpeed * _Time;
            fixed offsetY = _ScrollYSpeed * _Time;
            fixed2 offsetUV = fixed2(offsetX, offsetY);

            fixed2 normalUV = IN.uv_NormalTex + offsetUV;
            fixed2 mainUV = IN.uv_MainTex + offsetUV;

            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D(_MainTex, mainUV) * _Color;
            o.Albedo = c.rgb;

            float4 normalPixel = tex2D(_NormalTex, normalUV);
            float3 n = UnpackNormal(normalPixel);
            o.Normal = n.xyz;

            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
        FallBack "Diffuse"
}