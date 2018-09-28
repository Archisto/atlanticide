/* Source: http://wiki.unity3d.com/index.php/Dissolve_With_Texture */
/* Author: genericuser (radware.wordpress.com) */

// Simple "dissolving" shader by genericuser (radware.wordpress.com).
// Clips materials, using an image as guidance.
// Use clouds or random noise as the slice guide for best results.

// You'll most likely want to alter the amount of dissolving at runtime,
// which is easily done by using SetFloat() to alter "_SliceAmount", like here:
// renderer.material.SetFloat("_SliceAmount", 0.5 + Mathf.Sin(Time.time) * 0.5)

Shader "Custom/Dissolve" {
	Properties {
		_MainTex("Texture (RGB)", 2D) = "white" {}
		_SliceGuide("Slice Guide (RGB)", 2D) = "white" {}
		_SliceAmount("Slice Amount", Range(0.0, 1.0)) = 0.5
	}
		SubShader {
		Tags { "RenderType" = "Opaque" }
		Cull Off
		CGPROGRAM
		// If you're not planning on using shadows, remove "addshadow" for better performance
#pragma surface surf Lambert addshadow
		struct Input {
		float2 uv_MainTex;
		float2 uv_SliceGuide;
		float _SliceAmount;
	};
	sampler2D _MainTex;
	sampler2D _SliceGuide;
	float _SliceAmount;
	void surf(Input IN, inout SurfaceOutput o) {
		clip(tex2D(_SliceGuide, IN.uv_SliceGuide).rgb - _SliceAmount);
		o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;
	}
	ENDCG
	}
		Fallback "Diffuse"
}
