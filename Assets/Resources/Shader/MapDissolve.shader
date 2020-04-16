Shader "Custom/MapDissolve" 
{
	Properties
	{
		//_Color ("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Map("Map (RGB)", 2D) = "white" {}

		_Distance("Dissolve Distance", Float) = 1
		_DissTexture("Dissolve Texture", 2D) = "white" {}

		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0

		_Rim("Rim amount", Float) = 100

		[HideInInspector]
		_Cutoff("Base Alpha cutoff", Range(0,.9)) = .5

		[HDR]_DissolveColor("Dissolve Color", Color) = (0,0,1,1)

		_pivotX("PivotX", Float) = 189
	}

	SubShader
		{
			Tags{ "Queue" = "Geometry" }

			LOD 200

			CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
				#pragma surface surf Standard fullforwardshadows vertex:vert//alphatest:_Cutoff 

				sampler2D _MainTex,_Map,_DissTexture;

				struct Input 
				{
					float2 uv_MainTex;
					float2 uv_DissTexture;
					float3 worldPos;
					float3 localPos;
				};

				half _Glossiness;
				half _Metallic;
				half _Distance, _Interpolation;
				fixed4 _Color; 
				float4 _Center;

				float _pivotX;


				fixed _Rim;
				float4 _DissolveColor;

				void vert(inout appdata_full v, out Input o)
				{
					UNITY_INITIALIZE_OUTPUT(Input, o);
					o.localPos.x -= 5.0f;
					o.localPos = v.vertex.xyz ;
				}

				void surf(Input IN, inout SurfaceOutputStandard o)
				{
					fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
					fixed4 c2;

					IN.localPos.x += _pivotX;
					float d = length(IN.localPos);
					//
					////_Distance = _SinTime.w + 1.0;
					//
					d = _Distance - d + (tex2D(_DissTexture, IN.uv_DissTexture).a * c *saturate(_Distance)) + 0.5;
					//
					float sat = saturate(d);
					//
					c2 = (tex2D(_Map, IN.uv_MainTex) * (sat));
					c = (tex2D(_MainTex, IN.uv_MainTex) * (1 - sat));
					//
					o.Albedo = lerp(0, c2.rgb, saturate(d*d*_Rim)) + lerp(0, c.rgb, saturate(d*d*_Rim));
				
					// Metallic and smoothness come from slider variables
					o.Metallic = _Metallic;
					o.Smoothness = _Glossiness;
					o.Alpha = c.a;


					//Ray
					o.Emission = lerp(0, _DissolveColor.rgb, 1 - saturate(d*d*_Rim));

				}
				ENDCG
		}
	//FallBack "Transparent/Cutout/Diffuse"
	FallBack "Diffuse"
}
