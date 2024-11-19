Shader "MyShader/SToon Outline2" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		[Header(Colorize)] [Space(5)] _Color ("Color", Vector) = (1,1,1,1)
		[HideInInspector] _ColIntense ("Intensity", Range(0, 3)) = 1
		[HideInInspector] _ColBright ("Brightness", Range(-1, 1)) = 0
		_AmbientCol ("Ambient", Range(0, 1)) = 0
		[Header(Detail)] [Space(5)] [Toggle] _Segmented ("Segmented", Float) = 1
		_Steps ("Steps", Range(1, 25)) = 3
		_StpSmooth ("Smoothness", Range(0, 1)) = 0
		_Offset ("Lit Offset", Range(-1, 1.1)) = 0
		[Header(Light)] [Space(5)] [Toggle] _Clipped ("Clipped", Float) = 0
		_MinLight ("Min Light", Range(0, 1)) = 0
		_MaxLight ("Max Light", Range(0, 1)) = 1
		_Lumin ("Luminocity", Range(0, 2)) = 0
		[Header(Outline)] [Space(5)] _OtlColor ("Color", Vector) = (0,0,0,1)
		_OtlWidth ("Width", Range(0, 5)) = 1
		[Header(Shine)] [Space(5)] [HDR] _ShnColor ("Color", Vector) = (1,1,0,1)
		[Toggle] _ShnOverlap ("Overlap", Float) = 0
		_ShnIntense ("Intensity", Range(0, 1)) = 0
		_ShnRange ("Range", Range(0, 1)) = 0.15
		_ShnSmooth ("Smoothness", Range(0, 1)) = 0
		_RimColor ("Rim Color", Vector) = (1,0,0,1)
		_RimPower ("Rim Power", Range(0.0001, 3)) = 1
		_RimIntensity ("Rim Intensity", Range(0, 100)) = 1
		_ColorGlitter ("ColorGlitter", Vector) = (0,0,0,1)
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		sampler2D _MainTex;
		fixed4 _Color;
		struct Input
		{
			float2 uv_MainTex;
		};
		
		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
}