Shader "Custom/Lambert" 
{
	Properties 
	{
		_MainTex("Albedo (RGB)", 2D)="white" {}
		_BumpMap("NormalMap", 2D)="bump" {} 
	}
	SubShader 
	{
		Tags { "RenderType"="Opaque" }

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		// very very important 
		#pragma surface surf Test noambient

		sampler2D _MainTex;
		sampler2D _BumpMap;

		struct Input 
		{
			float2 uv_MainTex;
			float2 uv_BumpMap;
		};

		void surf (Input IN, inout SurfaceOutput o) 
		{
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Normal = UnpackNormal(tex2D(_BumpMap,IN.uv_BumpMap));
			o.Alpha = c.a;
		}

		float4 LightingTest (SurfaceOutput s, float3 lightDir, float atten)
		{
			float ndot1 = dot(s.Normal, lightDir)*0.3f + 0.3f;
			return ndot1;
			
		 }

		 
		ENDCG
	}
	FallBack "Diffuse"
}
