// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/UnderwaterShader"
{
    Properties
    {
        // Color property for material inspector, default to white
        _Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Texture", 2D) = "" {}
		_Ratio("Mix Color Ratio", Range(0,1)) = 0.0
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader {
        Tags { "RenderType"="Transparent" }
        LOD 200
       
        Blend SrcAlpha OneMinusSrcAlpha
       
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
		#pragma target 3.0
   
        sampler2D _MainTex;
 
        struct Input {
            float2 uv_MainTex;
        };
 
		half _Glossiness;
        half _Metallic;
        float _Ratio;
        fixed4 _Color;
 
        void surf (Input IN, inout SurfaceOutputStandard o) {
            // Albedo comes from a texture tinted by color
            fixed4 c = (1 - _Ratio) * tex2D (_MainTex, IN.uv_MainTex) + (_Ratio * _Color);
            o.Albedo = c.rgb;
			o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
			o.Emission = o.Albedo * 0.1;
            o.Alpha = c.a;
        }
       
        ENDCG
    }
    FallBack "Diffuse"
}