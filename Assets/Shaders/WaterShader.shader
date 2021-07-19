// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/WaterShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0

        // Height of gerstner wave vertices (Not much effect on looks, but better to have at 1. Using for debugging.)
        _RealHeight ("Gerstner wave height", Range(0,1)) = 1

    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows vertex:vert addshadow

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        // Gerstner wave variables
        int _waveCount = 0;
        float4 _waves[20];

        // Water surface variables
        float _steepnessModifier = 1;
        float _wavelengthModifier = 1;
        float _RealHeight;

        // Returns the change in position for the vertex.
        // Adjusts the tangent and binormal for later normal calculation.
        float3 GerstnerWave ( float4 wave, float3 p, inout float3 tangent, inout float3 binormal ) 
        {
            
            // Calculations for sine curve definitions.
		    float steepness = wave.z * _steepnessModifier;
		    float wavelength = wave.w * _wavelengthModifier;
		    float k = 2 * UNITY_PI / wavelength;
			float c = sqrt(9.8 / k);
			float2 d = normalize(wave.xy);
			float f = k * (dot(d, p.xz) - c * _Time.y);
			float a = steepness / k;

            // Calculations for finding normal.
			tangent += float3(
				-d.x * d.x * (steepness * sin(f)),
				d.x * (steepness * cos(f)),
				-d.x * d.y * (steepness * sin(f))
			);
			binormal += float3(
				-d.x * d.y * (steepness * sin(f)),
				d.y * (steepness * cos(f)),
				-d.y * d.y * (steepness * sin(f))
			);

            // Change in position
			return float3(
				d.x * (a * cos(f)),
				a * sin(f) * (20.0 / _waveCount),
				d.y * (a * cos(f))
			);
		}

        // Calculate vertex position and normal vector.
        void vert(inout appdata_full vertexData) {
			float3 tangent = float3(1, 0, 0);
			float3 binormal = float3(0, 0, 1);
			float3 p = vertexData.vertex.xyz;

            // Use global postion to calculate deltas, rather than local.
            float3 gridPoint = mul(unity_ObjectToWorld, vertexData.vertex);

            // Adjust position and normal for each wave.
            float3 j = float3(p.x,0,p.z);
            for (int i=0;i<_waveCount;i++)
            {
                j += GerstnerWave(_waves[i], gridPoint, tangent, binormal);
            }
            float3 normal = normalize(cross(binormal, tangent));

            // To handle scenarios for rough water that could get in way of camera.
            // Scales down vertex height towards zero, keeps appearance of water surface largely the same.
            j.y *= _RealHeight;

			vertexData.vertex.xyz = j;
			vertexData.normal = normal;
		}


        // Default color handling.
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
