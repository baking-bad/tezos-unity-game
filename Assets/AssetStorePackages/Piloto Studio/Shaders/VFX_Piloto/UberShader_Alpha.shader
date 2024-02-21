// Made with Amplify Shader Editor v1.9.1.5
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Piloto Studio/Alpha Uber Shader"
{
	Properties
	{
		[HideInInspector] _EmissionColor("Emission Color", Color) = (1,1,1,1)
		[HideInInspector] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
		[ASEBegin]_MainTex("Main Texture", 2D) = "white" {}
		_Desaturate("Desaturate?", Range( 0 , 1)) = 0
		_MainTextureChannel("Main Texture Channel", Vector) = (1,1,1,0)
		_MainAlphaChannel("Main Alpha Channel", Vector) = (0,0,0,1)
		_MainTexturePanning("Main Texture Panning", Vector) = (0,0,0,0)
		_MainAlphaPanning("Main Alpha Panning", Vector) = (0,0,0,0)
		_AlphaOverride("Alpha Override", 2D) = "white" {}
		_AlphaOverridePanning("Alpha Override Panning", Vector) = (0,0,0,0)
		_AlphaOverrideChannel("Alpha Override Channel", Vector) = (1,0,0,0)
		_FlipbooksColumsRows("Flipbooks Colums & Rows", Vector) = (1,1,0,0)
		_DetailNoise("Detail Noise", 2D) = "white" {}
		_DetailNoisePanning("Detail Noise Panning", Vector) = (0,0,0,0)
		_DetailDistortionChannel("Detail Distortion Channel", Vector) = (1,0,0,0)
		_DistortionIntensity("Distortion Intensity", Float) = 0
		_DetailMultiplyChannel("Detail Multiply Channel", Vector) = (0,0,0,0)
		_DetailAdditiveChannel("Detail Additive Channel", Vector) = (0,0,0,0)
		[Toggle(_USESOFTALPHA_ON)] _UseSoftAlpha("UseSoftAlpha", Float) = 0
		[ASEEnd]_SoftFadeFactor("SoftFadeFactor", Range( 0.1 , 1)) = 0


		//_TessPhongStrength( "Tess Phong Strength", Range( 0, 1 ) ) = 0.5
		//_TessValue( "Tess Max Tessellation", Range( 1, 32 ) ) = 16
		//_TessMin( "Tess Min Distance", Float ) = 10
		//_TessMax( "Tess Max Distance", Float ) = 25
		//_TessEdgeLength ( "Tess Edge length", Range( 2, 50 ) ) = 16
		//_TessMaxDisp( "Tess Max Displacement", Float ) = 25

		[HideInInspector] _QueueOffset("_QueueOffset", Float) = 0
        [HideInInspector] _QueueControl("_QueueControl", Float) = -1

        [HideInInspector][NoScaleOffset] unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset] unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset] unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
	}

	SubShader
	{
		LOD 0

		

		Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Transparent" "Queue"="Transparent" "UniversalMaterialType"="Unlit" }

		Cull Back
		AlphaToMask Off

		

		HLSLINCLUDE
		#pragma target 5.0
		#pragma prefer_hlslcc gles
		// ensure rendering platforms toggle list is visible

		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Filtering.hlsl"

		#ifndef ASE_TESS_FUNCS
		#define ASE_TESS_FUNCS
		float4 FixedTess( float tessValue )
		{
			return tessValue;
		}

		float CalcDistanceTessFactor (float4 vertex, float minDist, float maxDist, float tess, float4x4 o2w, float3 cameraPos )
		{
			float3 wpos = mul(o2w,vertex).xyz;
			float dist = distance (wpos, cameraPos);
			float f = clamp(1.0 - (dist - minDist) / (maxDist - minDist), 0.01, 1.0) * tess;
			return f;
		}

		float4 CalcTriEdgeTessFactors (float3 triVertexFactors)
		{
			float4 tess;
			tess.x = 0.5 * (triVertexFactors.y + triVertexFactors.z);
			tess.y = 0.5 * (triVertexFactors.x + triVertexFactors.z);
			tess.z = 0.5 * (triVertexFactors.x + triVertexFactors.y);
			tess.w = (triVertexFactors.x + triVertexFactors.y + triVertexFactors.z) / 3.0f;
			return tess;
		}

		float CalcEdgeTessFactor (float3 wpos0, float3 wpos1, float edgeLen, float3 cameraPos, float4 scParams )
		{
			float dist = distance (0.5 * (wpos0+wpos1), cameraPos);
			float len = distance(wpos0, wpos1);
			float f = max(len * scParams.y / (edgeLen * dist), 1.0);
			return f;
		}

		float DistanceFromPlane (float3 pos, float4 plane)
		{
			float d = dot (float4(pos,1.0f), plane);
			return d;
		}

		bool WorldViewFrustumCull (float3 wpos0, float3 wpos1, float3 wpos2, float cullEps, float4 planes[6] )
		{
			float4 planeTest;
			planeTest.x = (( DistanceFromPlane(wpos0, planes[0]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[0]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[0]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.y = (( DistanceFromPlane(wpos0, planes[1]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[1]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[1]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.z = (( DistanceFromPlane(wpos0, planes[2]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[2]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[2]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.w = (( DistanceFromPlane(wpos0, planes[3]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos1, planes[3]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlane(wpos2, planes[3]) > -cullEps) ? 1.0f : 0.0f );
			return !all (planeTest);
		}

		float4 DistanceBasedTess( float4 v0, float4 v1, float4 v2, float tess, float minDist, float maxDist, float4x4 o2w, float3 cameraPos )
		{
			float3 f;
			f.x = CalcDistanceTessFactor (v0,minDist,maxDist,tess,o2w,cameraPos);
			f.y = CalcDistanceTessFactor (v1,minDist,maxDist,tess,o2w,cameraPos);
			f.z = CalcDistanceTessFactor (v2,minDist,maxDist,tess,o2w,cameraPos);

			return CalcTriEdgeTessFactors (f);
		}

		float4 EdgeLengthBasedTess( float4 v0, float4 v1, float4 v2, float edgeLength, float4x4 o2w, float3 cameraPos, float4 scParams )
		{
			float3 pos0 = mul(o2w,v0).xyz;
			float3 pos1 = mul(o2w,v1).xyz;
			float3 pos2 = mul(o2w,v2).xyz;
			float4 tess;
			tess.x = CalcEdgeTessFactor (pos1, pos2, edgeLength, cameraPos, scParams);
			tess.y = CalcEdgeTessFactor (pos2, pos0, edgeLength, cameraPos, scParams);
			tess.z = CalcEdgeTessFactor (pos0, pos1, edgeLength, cameraPos, scParams);
			tess.w = (tess.x + tess.y + tess.z) / 3.0f;
			return tess;
		}

		float4 EdgeLengthBasedTessCull( float4 v0, float4 v1, float4 v2, float edgeLength, float maxDisplacement, float4x4 o2w, float3 cameraPos, float4 scParams, float4 planes[6] )
		{
			float3 pos0 = mul(o2w,v0).xyz;
			float3 pos1 = mul(o2w,v1).xyz;
			float3 pos2 = mul(o2w,v2).xyz;
			float4 tess;

			if (WorldViewFrustumCull(pos0, pos1, pos2, maxDisplacement, planes))
			{
				tess = 0.0f;
			}
			else
			{
				tess.x = CalcEdgeTessFactor (pos1, pos2, edgeLength, cameraPos, scParams);
				tess.y = CalcEdgeTessFactor (pos2, pos0, edgeLength, cameraPos, scParams);
				tess.z = CalcEdgeTessFactor (pos0, pos1, edgeLength, cameraPos, scParams);
				tess.w = (tess.x + tess.y + tess.z) / 3.0f;
			}
			return tess;
		}
		#endif //ASE_TESS_FUNCS
		ENDHLSL

		
		Pass
		{
			
			Name "Forward"
			Tags { "LightMode"="UniversalForwardOnly" }

			Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
			ZWrite Off
			ZTest LEqual
			Offset 0 , 0
			ColorMask RGBA

			

			HLSLPROGRAM

			#pragma multi_compile_instancing
			#define _SURFACE_TYPE_TRANSPARENT 1
			#define ASE_SRP_VERSION 120111
			#define REQUIRE_DEPTH_TEXTURE 1


			#pragma multi_compile _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3

			#pragma multi_compile _ LIGHTMAP_ON
			#pragma multi_compile _ DIRLIGHTMAP_COMBINED
			#pragma shader_feature _ _SAMPLE_GI
			#pragma multi_compile _ DEBUG_DISPLAY

			#pragma vertex vert
			#pragma fragment frag

			#define SHADERPASS SHADERPASS_UNLIT

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Debug/Debugging3D.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceData.hlsl"

			#define ASE_NEEDS_FRAG_COLOR
			#pragma shader_feature_local _USESOFTALPHA_ON


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					float4 shadowCoord : TEXCOORD1;
				#endif
				#ifdef ASE_FOG
					float fogFactor : TEXCOORD2;
				#endif
				float4 ase_color : COLOR;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				float4 ase_texcoord5 : TEXCOORD5;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _DetailNoise_ST;
			float4 _DetailDistortionChannel;
			float4 _MainTex_ST;
			float4 _MainTextureChannel;
			float4 _MainAlphaChannel;
			float4 _DetailAdditiveChannel;
			float4 _DetailMultiplyChannel;
			float4 _AlphaOverride_ST;
			float4 _AlphaOverrideChannel;
			float2 _MainTexturePanning;
			float2 _DetailNoisePanning;
			float2 _MainAlphaPanning;
			float2 _FlipbooksColumsRows;
			float2 _AlphaOverridePanning;
			float _Desaturate;
			float _DistortionIntensity;
			float _SoftFadeFactor;
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _MainTex;
			sampler2D _DetailNoise;
			sampler2D _AlphaOverride;
			uniform float4 _CameraDepthTexture_TexelSize;


			
			VertexOutput VertexFunction ( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float4 ase_clipPos = TransformObjectToHClip((v.vertex).xyz);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord5 = screenPos;
				
				o.ase_color = v.ase_color;
				o.ase_texcoord3 = v.ase_texcoord;
				o.ase_texcoord4 = v.ase_texcoord1;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = defaultVertexValue;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float4 positionCS = TransformWorldToHClip( positionWS );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					o.worldPos = positionWS;
				#endif

				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = positionCS;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif

				#ifdef ASE_FOG
					o.fogFactor = ComputeFogFactor( positionCS.z );
				#endif

				o.clipPos = positionCS;

				return o;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_color = v.ase_color;
				o.ase_texcoord = v.ase_texcoord;
				o.ase_texcoord1 = v.ase_texcoord1;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				o.ase_texcoord1 = patch[0].ase_texcoord1 * bary.x + patch[1].ase_texcoord1 * bary.y + patch[2].ase_texcoord1 * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag ( VertexOutput IN  ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 WorldPosition = IN.worldPos;
				#endif

				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float2 uv_DetailNoise = IN.ase_texcoord3.xy * _DetailNoise_ST.xy + _DetailNoise_ST.zw;
				float2 panner80 = ( 1.0 * _Time.y * _DetailNoisePanning + uv_DetailNoise);
				float4 tex2DNode79 = tex2D( _DetailNoise, panner80 );
				float4 break17_g75 = tex2DNode79;
				float4 appendResult18_g75 = (float4(break17_g75.x , break17_g75.y , break17_g75.z , break17_g75.w));
				float4 clampResult19_g75 = clamp( ( appendResult18_g75 * _DetailDistortionChannel ) , float4( 0,0,0,0 ) , float4( 1,1,1,1 ) );
				float4 break2_g75 = clampResult19_g75;
				float clampResult20_g75 = clamp( ( break2_g75.x + break2_g75.y + break2_g75.z + break2_g75.w ) , 0.0 , 1.0 );
				float3 temp_cast_1 = (clampResult20_g75).xxx;
				float3 desaturateInitialColor190 = temp_cast_1;
				float desaturateDot190 = dot( desaturateInitialColor190, float3( 0.299, 0.587, 0.114 ));
				float3 desaturateVar190 = lerp( desaturateInitialColor190, desaturateDot190.xxx, 1.0 );
				float3 DistortionNoise90 = desaturateVar190;
				float4 texCoord168 = IN.ase_texcoord4;
				texCoord168.xy = IN.ase_texcoord4.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult176 = (float2(texCoord168.z , 0.0));
				float2 appendResult182 = (float2(0.0 , 0.0));
				float2 LocalUVOffset184 = ( appendResult176 + appendResult182 );
				float2 uv_MainTex = IN.ase_texcoord3.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				float3 UVFlipbookInput194 = ( ( DistortionNoise90 * _DistortionIntensity ) + float3( ( LocalUVOffset184 + uv_MainTex ) ,  0.0 ) );
				float temp_output_4_0_g119 = _FlipbooksColumsRows.x;
				float temp_output_5_0_g119 = _FlipbooksColumsRows.y;
				float2 appendResult7_g119 = (float2(temp_output_4_0_g119 , temp_output_5_0_g119));
				float totalFrames39_g119 = ( temp_output_4_0_g119 * temp_output_5_0_g119 );
				float2 appendResult8_g119 = (float2(totalFrames39_g119 , temp_output_5_0_g119));
				float4 texCoord75 = IN.ase_texcoord4;
				texCoord75.xy = IN.ase_texcoord4.xy * float2( 1,1 ) + float2( 0,0 );
				float clampResult42_g119 = clamp( texCoord75.x , 0.0001 , ( totalFrames39_g119 - 1.0 ) );
				float temp_output_35_0_g119 = frac( ( ( (float)0 + clampResult42_g119 ) / totalFrames39_g119 ) );
				float2 appendResult29_g119 = (float2(temp_output_35_0_g119 , ( 1.0 - temp_output_35_0_g119 )));
				float2 temp_output_15_0_g119 = ( ( UVFlipbookInput194.xy / appendResult7_g119 ) + ( floor( ( appendResult8_g119 * appendResult29_g119 ) ) / appendResult7_g119 ) );
				float2 temp_output_73_0 = temp_output_15_0_g119;
				float2 panner22 = ( 1.0 * _Time.y * _MainTexturePanning + temp_output_73_0);
				float4 break17_g143 = tex2D( _MainTex, panner22 );
				float4 appendResult18_g143 = (float4(break17_g143.x , break17_g143.y , break17_g143.z , break17_g143.w));
				float4 clampResult19_g143 = clamp( ( appendResult18_g143 * _MainTextureChannel ) , float4( 0,0,0,0 ) , float4( 1,1,1,1 ) );
				float4 break2_g143 = clampResult19_g143;
				float clampResult20_g143 = clamp( ( break2_g143.x + break2_g143.y + break2_g143.z + break2_g143.w ) , 0.0 , 1.0 );
				float MainTexInfo25 = clampResult20_g143;
				float3 temp_cast_6 = (MainTexInfo25).xxx;
				float3 desaturateInitialColor166 = temp_cast_6;
				float desaturateDot166 = dot( desaturateInitialColor166, float3( 0.299, 0.587, 0.114 ));
				float3 desaturateVar166 = lerp( desaturateInitialColor166, desaturateDot166.xxx, _Desaturate );
				float4 break156 = ( _DetailAdditiveChannel * tex2DNode79 );
				float4 appendResult155 = (float4(break156.x , break156.y , break156.z , break156.w));
				float3 desaturateInitialColor191 = appendResult155.xyz;
				float desaturateDot191 = dot( desaturateInitialColor191, float3( 0.299, 0.587, 0.114 ));
				float3 desaturateVar191 = lerp( desaturateInitialColor191, desaturateDot191.xxx, 1.0 );
				float3 AdditiveNoise91 = desaturateVar191;
				float4 texCoord71 = IN.ase_texcoord3;
				texCoord71.xy = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float4 break17_g142 = tex2DNode79;
				float4 appendResult18_g142 = (float4(break17_g142.x , break17_g142.y , break17_g142.z , break17_g142.w));
				float4 clampResult19_g142 = clamp( ( appendResult18_g142 * _DetailMultiplyChannel ) , float4( 0,0,0,0 ) , float4( 1,1,1,1 ) );
				float4 break2_g142 = clampResult19_g142;
				float clampResult20_g142 = clamp( ( break2_g142.x + break2_g142.y + break2_g142.z + break2_g142.w ) , 0.0 , 1.0 );
				float ifLocalVar106 = 0;
				if( ( _DetailMultiplyChannel.x + _DetailMultiplyChannel.y + _DetailMultiplyChannel.z + _DetailMultiplyChannel.w ) <= 0.0 )
				ifLocalVar106 = 1.0;
				else
				ifLocalVar106 = clampResult20_g142;
				float3 temp_cast_11 = (ifLocalVar106).xxx;
				float3 desaturateInitialColor189 = temp_cast_11;
				float desaturateDot189 = dot( desaturateInitialColor189, float3( 0.299, 0.587, 0.114 ));
				float3 desaturateVar189 = lerp( desaturateInitialColor189, desaturateDot189.xxx, 1.0 );
				float3 MultiplyNoise92 = desaturateVar189;
				
				float2 uv_AlphaOverride = IN.ase_texcoord3.xy * _AlphaOverride_ST.xy + _AlphaOverride_ST.zw;
				float temp_output_4_0_g118 = _FlipbooksColumsRows.x;
				float temp_output_5_0_g118 = _FlipbooksColumsRows.y;
				float2 appendResult7_g118 = (float2(temp_output_4_0_g118 , temp_output_5_0_g118));
				float totalFrames39_g118 = ( temp_output_4_0_g118 * temp_output_5_0_g118 );
				float2 appendResult8_g118 = (float2(totalFrames39_g118 , temp_output_5_0_g118));
				float clampResult42_g118 = clamp( texCoord75.x , 0.0001 , ( totalFrames39_g118 - 1.0 ) );
				float temp_output_35_0_g118 = frac( ( ( (float)0 + clampResult42_g118 ) / totalFrames39_g118 ) );
				float2 appendResult29_g118 = (float2(temp_output_35_0_g118 , ( 1.0 - temp_output_35_0_g118 )));
				float2 temp_output_15_0_g118 = ( ( ( LocalUVOffset184 + uv_AlphaOverride ) / appendResult7_g118 ) + ( floor( ( appendResult8_g118 * appendResult29_g118 ) ) / appendResult7_g118 ) );
				float2 panner44 = ( 1.0 * _Time.y * _AlphaOverridePanning + temp_output_15_0_g118);
				float4 break2_g121 = ( tex2D( _AlphaOverride, panner44 ) * _AlphaOverrideChannel );
				float AlphaOverride49 = saturate( ( break2_g121.x + break2_g121.y + break2_g121.z + break2_g121.w ) );
				float2 panner33 = ( 1.0 * _Time.y * _MainAlphaPanning + temp_output_73_0);
				float4 break2_g120 = ( tex2D( _MainTex, panner33 ) * _MainAlphaChannel );
				float MainAlpha30 = saturate( ( break2_g120.x + break2_g120.y + break2_g120.z + break2_g120.w ) );
				float temp_output_55_0 = ( AlphaOverride49 * MainAlpha30 );
				float4 texCoord60 = IN.ase_texcoord3;
				texCoord60.xy = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g122 = ( texCoord60.w - ( 1.0 - temp_output_55_0 ) );
				float temp_output_40_0 = ( IN.ase_color.a * temp_output_55_0 * saturate( saturate( ( temp_output_3_0_g122 / fwidth( temp_output_3_0_g122 ) ) ) ) );
				float4 screenPos = IN.ase_texcoord5;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float screenDepth199 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float distanceDepth199 = abs( ( screenDepth199 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( _SoftFadeFactor ) );
				#ifdef _USESOFTALPHA_ON
				float staticSwitch198 = ( temp_output_40_0 * saturate( distanceDepth199 ) );
				#else
				float staticSwitch198 = temp_output_40_0;
				#endif
				
				float3 BakedAlbedo = 0;
				float3 BakedEmission = 0;
				float3 Color = ( IN.ase_color * float4( ( desaturateVar166 + AdditiveNoise91 ) , 0.0 ) * ( texCoord71.z + 1.0 ) * float4( MultiplyNoise92 , 0.0 ) ).rgb;
				float Alpha = staticSwitch198;
				float AlphaClipThreshold = 0.5;
				float AlphaClipThresholdShadow = 0.5;

				#ifdef _ALPHATEST_ON
					clip( Alpha - AlphaClipThreshold );
				#endif

				#if defined(_DBUFFER)
					ApplyDecalToBaseColor(IN.clipPos, Color);
				#endif

				#if defined(_ALPHAPREMULTIPLY_ON)
				Color *= Alpha;
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif

				#ifdef ASE_FOG
					Color = MixFog( Color, IN.fogFactor );
				#endif

				return half4( Color, Alpha );
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "DepthOnly"
			Tags { "LightMode"="DepthOnly" }

			ZWrite On
			ColorMask 0
			AlphaToMask Off

			HLSLPROGRAM

			#pragma multi_compile_instancing
			#define _SURFACE_TYPE_TRANSPARENT 1
			#define ASE_SRP_VERSION 120111
			#define REQUIRE_DEPTH_TEXTURE 1


			#pragma vertex vert
			#pragma fragment frag

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

			#define ASE_NEEDS_FRAG_COLOR
			#pragma shader_feature_local _USESOFTALPHA_ON


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_color : COLOR;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
				float3 worldPos : TEXCOORD0;
				#endif
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
				float4 shadowCoord : TEXCOORD1;
				#endif
				float4 ase_color : COLOR;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _DetailNoise_ST;
			float4 _DetailDistortionChannel;
			float4 _MainTex_ST;
			float4 _MainTextureChannel;
			float4 _MainAlphaChannel;
			float4 _DetailAdditiveChannel;
			float4 _DetailMultiplyChannel;
			float4 _AlphaOverride_ST;
			float4 _AlphaOverrideChannel;
			float2 _MainTexturePanning;
			float2 _DetailNoisePanning;
			float2 _MainAlphaPanning;
			float2 _FlipbooksColumsRows;
			float2 _AlphaOverridePanning;
			float _Desaturate;
			float _DistortionIntensity;
			float _SoftFadeFactor;
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _AlphaOverride;
			sampler2D _MainTex;
			sampler2D _DetailNoise;
			uniform float4 _CameraDepthTexture_TexelSize;


			
			VertexOutput VertexFunction( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float4 ase_clipPos = TransformObjectToHClip((v.vertex).xyz);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord4 = screenPos;
				
				o.ase_color = v.ase_color;
				o.ase_texcoord2 = v.ase_texcoord1;
				o.ase_texcoord3 = v.ase_texcoord;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = defaultVertexValue;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					o.worldPos = positionWS;
				#endif

				o.clipPos = TransformWorldToHClip( positionWS );
				#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR) && defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					VertexPositionInputs vertexInput = (VertexPositionInputs)0;
					vertexInput.positionWS = positionWS;
					vertexInput.positionCS = o.clipPos;
					o.shadowCoord = GetShadowCoord( vertexInput );
				#endif

				return o;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_color : COLOR;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_color = v.ase_color;
				o.ase_texcoord1 = v.ase_texcoord1;
				o.ase_texcoord = v.ase_texcoord;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				o.ase_texcoord1 = patch[0].ase_texcoord1 * bary.x + patch[1].ase_texcoord1 * bary.y + patch[2].ase_texcoord1 * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag(VertexOutput IN  ) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				#if defined(ASE_NEEDS_FRAG_WORLD_POSITION)
					float3 WorldPosition = IN.worldPos;
				#endif

				float4 ShadowCoords = float4( 0, 0, 0, 0 );

				#if defined(ASE_NEEDS_FRAG_SHADOWCOORDS)
					#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
						ShadowCoords = IN.shadowCoord;
					#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
						ShadowCoords = TransformWorldToShadowCoord( WorldPosition );
					#endif
				#endif

				float4 texCoord168 = IN.ase_texcoord2;
				texCoord168.xy = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult176 = (float2(texCoord168.z , 0.0));
				float2 appendResult182 = (float2(0.0 , 0.0));
				float2 LocalUVOffset184 = ( appendResult176 + appendResult182 );
				float2 uv_AlphaOverride = IN.ase_texcoord3.xy * _AlphaOverride_ST.xy + _AlphaOverride_ST.zw;
				float temp_output_4_0_g118 = _FlipbooksColumsRows.x;
				float temp_output_5_0_g118 = _FlipbooksColumsRows.y;
				float2 appendResult7_g118 = (float2(temp_output_4_0_g118 , temp_output_5_0_g118));
				float totalFrames39_g118 = ( temp_output_4_0_g118 * temp_output_5_0_g118 );
				float2 appendResult8_g118 = (float2(totalFrames39_g118 , temp_output_5_0_g118));
				float4 texCoord75 = IN.ase_texcoord2;
				texCoord75.xy = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float clampResult42_g118 = clamp( texCoord75.x , 0.0001 , ( totalFrames39_g118 - 1.0 ) );
				float temp_output_35_0_g118 = frac( ( ( (float)0 + clampResult42_g118 ) / totalFrames39_g118 ) );
				float2 appendResult29_g118 = (float2(temp_output_35_0_g118 , ( 1.0 - temp_output_35_0_g118 )));
				float2 temp_output_15_0_g118 = ( ( ( LocalUVOffset184 + uv_AlphaOverride ) / appendResult7_g118 ) + ( floor( ( appendResult8_g118 * appendResult29_g118 ) ) / appendResult7_g118 ) );
				float2 panner44 = ( 1.0 * _Time.y * _AlphaOverridePanning + temp_output_15_0_g118);
				float4 break2_g121 = ( tex2D( _AlphaOverride, panner44 ) * _AlphaOverrideChannel );
				float AlphaOverride49 = saturate( ( break2_g121.x + break2_g121.y + break2_g121.z + break2_g121.w ) );
				float2 uv_DetailNoise = IN.ase_texcoord3.xy * _DetailNoise_ST.xy + _DetailNoise_ST.zw;
				float2 panner80 = ( 1.0 * _Time.y * _DetailNoisePanning + uv_DetailNoise);
				float4 tex2DNode79 = tex2D( _DetailNoise, panner80 );
				float4 break17_g75 = tex2DNode79;
				float4 appendResult18_g75 = (float4(break17_g75.x , break17_g75.y , break17_g75.z , break17_g75.w));
				float4 clampResult19_g75 = clamp( ( appendResult18_g75 * _DetailDistortionChannel ) , float4( 0,0,0,0 ) , float4( 1,1,1,1 ) );
				float4 break2_g75 = clampResult19_g75;
				float clampResult20_g75 = clamp( ( break2_g75.x + break2_g75.y + break2_g75.z + break2_g75.w ) , 0.0 , 1.0 );
				float3 temp_cast_3 = (clampResult20_g75).xxx;
				float3 desaturateInitialColor190 = temp_cast_3;
				float desaturateDot190 = dot( desaturateInitialColor190, float3( 0.299, 0.587, 0.114 ));
				float3 desaturateVar190 = lerp( desaturateInitialColor190, desaturateDot190.xxx, 1.0 );
				float3 DistortionNoise90 = desaturateVar190;
				float2 uv_MainTex = IN.ase_texcoord3.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				float3 UVFlipbookInput194 = ( ( DistortionNoise90 * _DistortionIntensity ) + float3( ( LocalUVOffset184 + uv_MainTex ) ,  0.0 ) );
				float temp_output_4_0_g119 = _FlipbooksColumsRows.x;
				float temp_output_5_0_g119 = _FlipbooksColumsRows.y;
				float2 appendResult7_g119 = (float2(temp_output_4_0_g119 , temp_output_5_0_g119));
				float totalFrames39_g119 = ( temp_output_4_0_g119 * temp_output_5_0_g119 );
				float2 appendResult8_g119 = (float2(totalFrames39_g119 , temp_output_5_0_g119));
				float clampResult42_g119 = clamp( texCoord75.x , 0.0001 , ( totalFrames39_g119 - 1.0 ) );
				float temp_output_35_0_g119 = frac( ( ( (float)0 + clampResult42_g119 ) / totalFrames39_g119 ) );
				float2 appendResult29_g119 = (float2(temp_output_35_0_g119 , ( 1.0 - temp_output_35_0_g119 )));
				float2 temp_output_15_0_g119 = ( ( UVFlipbookInput194.xy / appendResult7_g119 ) + ( floor( ( appendResult8_g119 * appendResult29_g119 ) ) / appendResult7_g119 ) );
				float2 temp_output_73_0 = temp_output_15_0_g119;
				float2 panner33 = ( 1.0 * _Time.y * _MainAlphaPanning + temp_output_73_0);
				float4 break2_g120 = ( tex2D( _MainTex, panner33 ) * _MainAlphaChannel );
				float MainAlpha30 = saturate( ( break2_g120.x + break2_g120.y + break2_g120.z + break2_g120.w ) );
				float temp_output_55_0 = ( AlphaOverride49 * MainAlpha30 );
				float4 texCoord60 = IN.ase_texcoord3;
				texCoord60.xy = IN.ase_texcoord3.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g122 = ( texCoord60.w - ( 1.0 - temp_output_55_0 ) );
				float temp_output_40_0 = ( IN.ase_color.a * temp_output_55_0 * saturate( saturate( ( temp_output_3_0_g122 / fwidth( temp_output_3_0_g122 ) ) ) ) );
				float4 screenPos = IN.ase_texcoord4;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float screenDepth199 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float distanceDepth199 = abs( ( screenDepth199 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( _SoftFadeFactor ) );
				#ifdef _USESOFTALPHA_ON
				float staticSwitch198 = ( temp_output_40_0 * saturate( distanceDepth199 ) );
				#else
				float staticSwitch198 = temp_output_40_0;
				#endif
				

				float Alpha = staticSwitch198;
				float AlphaClipThreshold = 0.5;

				#ifdef _ALPHATEST_ON
					clip(Alpha - AlphaClipThreshold);
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif
				return 0;
			}
			ENDHLSL
		}

		
		Pass
		{
			
            Name "SceneSelectionPass"
            Tags { "LightMode"="SceneSelectionPass" }

			Cull Off

			HLSLPROGRAM

			#pragma multi_compile_instancing
			#define _SURFACE_TYPE_TRANSPARENT 1
			#define ASE_SRP_VERSION 120111
			#define REQUIRE_DEPTH_TEXTURE 1


			#pragma vertex vert
			#pragma fragment frag

			#define ATTRIBUTES_NEED_NORMAL
			#define ATTRIBUTES_NEED_TANGENT
			#define SHADERPASS SHADERPASS_DEPTHONLY

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#define ASE_NEEDS_FRAG_COLOR
			#pragma shader_feature_local _USESOFTALPHA_ON


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_color : COLOR;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _DetailNoise_ST;
			float4 _DetailDistortionChannel;
			float4 _MainTex_ST;
			float4 _MainTextureChannel;
			float4 _MainAlphaChannel;
			float4 _DetailAdditiveChannel;
			float4 _DetailMultiplyChannel;
			float4 _AlphaOverride_ST;
			float4 _AlphaOverrideChannel;
			float2 _MainTexturePanning;
			float2 _DetailNoisePanning;
			float2 _MainAlphaPanning;
			float2 _FlipbooksColumsRows;
			float2 _AlphaOverridePanning;
			float _Desaturate;
			float _DistortionIntensity;
			float _SoftFadeFactor;
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _AlphaOverride;
			sampler2D _MainTex;
			sampler2D _DetailNoise;
			uniform float4 _CameraDepthTexture_TexelSize;


			
			int _ObjectId;
			int _PassValue;

			struct SurfaceDescription
			{
				float Alpha;
				float AlphaClipThreshold;
			};

			VertexOutput VertexFunction(VertexInput v  )
			{
				VertexOutput o;
				ZERO_INITIALIZE(VertexOutput, o);

				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float4 ase_clipPos = TransformObjectToHClip((v.vertex).xyz);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord2 = screenPos;
				
				o.ase_color = v.ase_color;
				o.ase_texcoord = v.ase_texcoord1;
				o.ase_texcoord1 = v.ase_texcoord;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = defaultVertexValue;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				o.clipPos = TransformWorldToHClip(positionWS);

				return o;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_color : COLOR;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_color = v.ase_color;
				o.ase_texcoord1 = v.ase_texcoord1;
				o.ase_texcoord = v.ase_texcoord;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				o.ase_texcoord1 = patch[0].ase_texcoord1 * bary.x + patch[1].ase_texcoord1 * bary.y + patch[2].ase_texcoord1 * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag(VertexOutput IN ) : SV_TARGET
			{
				SurfaceDescription surfaceDescription = (SurfaceDescription)0;

				float4 texCoord168 = IN.ase_texcoord;
				texCoord168.xy = IN.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult176 = (float2(texCoord168.z , 0.0));
				float2 appendResult182 = (float2(0.0 , 0.0));
				float2 LocalUVOffset184 = ( appendResult176 + appendResult182 );
				float2 uv_AlphaOverride = IN.ase_texcoord1.xy * _AlphaOverride_ST.xy + _AlphaOverride_ST.zw;
				float temp_output_4_0_g118 = _FlipbooksColumsRows.x;
				float temp_output_5_0_g118 = _FlipbooksColumsRows.y;
				float2 appendResult7_g118 = (float2(temp_output_4_0_g118 , temp_output_5_0_g118));
				float totalFrames39_g118 = ( temp_output_4_0_g118 * temp_output_5_0_g118 );
				float2 appendResult8_g118 = (float2(totalFrames39_g118 , temp_output_5_0_g118));
				float4 texCoord75 = IN.ase_texcoord;
				texCoord75.xy = IN.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float clampResult42_g118 = clamp( texCoord75.x , 0.0001 , ( totalFrames39_g118 - 1.0 ) );
				float temp_output_35_0_g118 = frac( ( ( (float)0 + clampResult42_g118 ) / totalFrames39_g118 ) );
				float2 appendResult29_g118 = (float2(temp_output_35_0_g118 , ( 1.0 - temp_output_35_0_g118 )));
				float2 temp_output_15_0_g118 = ( ( ( LocalUVOffset184 + uv_AlphaOverride ) / appendResult7_g118 ) + ( floor( ( appendResult8_g118 * appendResult29_g118 ) ) / appendResult7_g118 ) );
				float2 panner44 = ( 1.0 * _Time.y * _AlphaOverridePanning + temp_output_15_0_g118);
				float4 break2_g121 = ( tex2D( _AlphaOverride, panner44 ) * _AlphaOverrideChannel );
				float AlphaOverride49 = saturate( ( break2_g121.x + break2_g121.y + break2_g121.z + break2_g121.w ) );
				float2 uv_DetailNoise = IN.ase_texcoord1.xy * _DetailNoise_ST.xy + _DetailNoise_ST.zw;
				float2 panner80 = ( 1.0 * _Time.y * _DetailNoisePanning + uv_DetailNoise);
				float4 tex2DNode79 = tex2D( _DetailNoise, panner80 );
				float4 break17_g75 = tex2DNode79;
				float4 appendResult18_g75 = (float4(break17_g75.x , break17_g75.y , break17_g75.z , break17_g75.w));
				float4 clampResult19_g75 = clamp( ( appendResult18_g75 * _DetailDistortionChannel ) , float4( 0,0,0,0 ) , float4( 1,1,1,1 ) );
				float4 break2_g75 = clampResult19_g75;
				float clampResult20_g75 = clamp( ( break2_g75.x + break2_g75.y + break2_g75.z + break2_g75.w ) , 0.0 , 1.0 );
				float3 temp_cast_3 = (clampResult20_g75).xxx;
				float3 desaturateInitialColor190 = temp_cast_3;
				float desaturateDot190 = dot( desaturateInitialColor190, float3( 0.299, 0.587, 0.114 ));
				float3 desaturateVar190 = lerp( desaturateInitialColor190, desaturateDot190.xxx, 1.0 );
				float3 DistortionNoise90 = desaturateVar190;
				float2 uv_MainTex = IN.ase_texcoord1.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				float3 UVFlipbookInput194 = ( ( DistortionNoise90 * _DistortionIntensity ) + float3( ( LocalUVOffset184 + uv_MainTex ) ,  0.0 ) );
				float temp_output_4_0_g119 = _FlipbooksColumsRows.x;
				float temp_output_5_0_g119 = _FlipbooksColumsRows.y;
				float2 appendResult7_g119 = (float2(temp_output_4_0_g119 , temp_output_5_0_g119));
				float totalFrames39_g119 = ( temp_output_4_0_g119 * temp_output_5_0_g119 );
				float2 appendResult8_g119 = (float2(totalFrames39_g119 , temp_output_5_0_g119));
				float clampResult42_g119 = clamp( texCoord75.x , 0.0001 , ( totalFrames39_g119 - 1.0 ) );
				float temp_output_35_0_g119 = frac( ( ( (float)0 + clampResult42_g119 ) / totalFrames39_g119 ) );
				float2 appendResult29_g119 = (float2(temp_output_35_0_g119 , ( 1.0 - temp_output_35_0_g119 )));
				float2 temp_output_15_0_g119 = ( ( UVFlipbookInput194.xy / appendResult7_g119 ) + ( floor( ( appendResult8_g119 * appendResult29_g119 ) ) / appendResult7_g119 ) );
				float2 temp_output_73_0 = temp_output_15_0_g119;
				float2 panner33 = ( 1.0 * _Time.y * _MainAlphaPanning + temp_output_73_0);
				float4 break2_g120 = ( tex2D( _MainTex, panner33 ) * _MainAlphaChannel );
				float MainAlpha30 = saturate( ( break2_g120.x + break2_g120.y + break2_g120.z + break2_g120.w ) );
				float temp_output_55_0 = ( AlphaOverride49 * MainAlpha30 );
				float4 texCoord60 = IN.ase_texcoord1;
				texCoord60.xy = IN.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g122 = ( texCoord60.w - ( 1.0 - temp_output_55_0 ) );
				float temp_output_40_0 = ( IN.ase_color.a * temp_output_55_0 * saturate( saturate( ( temp_output_3_0_g122 / fwidth( temp_output_3_0_g122 ) ) ) ) );
				float4 screenPos = IN.ase_texcoord2;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float screenDepth199 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float distanceDepth199 = abs( ( screenDepth199 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( _SoftFadeFactor ) );
				#ifdef _USESOFTALPHA_ON
				float staticSwitch198 = ( temp_output_40_0 * saturate( distanceDepth199 ) );
				#else
				float staticSwitch198 = temp_output_40_0;
				#endif
				

				surfaceDescription.Alpha = staticSwitch198;
				surfaceDescription.AlphaClipThreshold = 0.5;

				#if _ALPHATEST_ON
					float alphaClipThreshold = 0.01f;
					#if ALPHA_CLIP_THRESHOLD
						alphaClipThreshold = surfaceDescription.AlphaClipThreshold;
					#endif
					clip(surfaceDescription.Alpha - alphaClipThreshold);
				#endif

				half4 outColor = half4(_ObjectId, _PassValue, 1.0, 1.0);
				return outColor;
			}
			ENDHLSL
		}

		
		Pass
		{
			
            Name "ScenePickingPass"
            Tags { "LightMode"="Picking" }

			HLSLPROGRAM

			#pragma multi_compile_instancing
			#define _SURFACE_TYPE_TRANSPARENT 1
			#define ASE_SRP_VERSION 120111
			#define REQUIRE_DEPTH_TEXTURE 1


			#pragma vertex vert
			#pragma fragment frag

			#define ATTRIBUTES_NEED_NORMAL
			#define ATTRIBUTES_NEED_TANGENT
			#define SHADERPASS SHADERPASS_DEPTHONLY

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#define ASE_NEEDS_FRAG_COLOR
			#pragma shader_feature_local _USESOFTALPHA_ON


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_color : COLOR;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				float4 ase_color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _DetailNoise_ST;
			float4 _DetailDistortionChannel;
			float4 _MainTex_ST;
			float4 _MainTextureChannel;
			float4 _MainAlphaChannel;
			float4 _DetailAdditiveChannel;
			float4 _DetailMultiplyChannel;
			float4 _AlphaOverride_ST;
			float4 _AlphaOverrideChannel;
			float2 _MainTexturePanning;
			float2 _DetailNoisePanning;
			float2 _MainAlphaPanning;
			float2 _FlipbooksColumsRows;
			float2 _AlphaOverridePanning;
			float _Desaturate;
			float _DistortionIntensity;
			float _SoftFadeFactor;
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _AlphaOverride;
			sampler2D _MainTex;
			sampler2D _DetailNoise;
			uniform float4 _CameraDepthTexture_TexelSize;


			
			float4 _SelectionID;


			struct SurfaceDescription
			{
				float Alpha;
				float AlphaClipThreshold;
			};

			VertexOutput VertexFunction(VertexInput v  )
			{
				VertexOutput o;
				ZERO_INITIALIZE(VertexOutput, o);

				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float4 ase_clipPos = TransformObjectToHClip((v.vertex).xyz);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord2 = screenPos;
				
				o.ase_color = v.ase_color;
				o.ase_texcoord = v.ase_texcoord1;
				o.ase_texcoord1 = v.ase_texcoord;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif
				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				o.clipPos = TransformWorldToHClip(positionWS);
				return o;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_color : COLOR;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_color = v.ase_color;
				o.ase_texcoord1 = v.ase_texcoord1;
				o.ase_texcoord = v.ase_texcoord;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				o.ase_texcoord1 = patch[0].ase_texcoord1 * bary.x + patch[1].ase_texcoord1 * bary.y + patch[2].ase_texcoord1 * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag(VertexOutput IN ) : SV_TARGET
			{
				SurfaceDescription surfaceDescription = (SurfaceDescription)0;

				float4 texCoord168 = IN.ase_texcoord;
				texCoord168.xy = IN.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult176 = (float2(texCoord168.z , 0.0));
				float2 appendResult182 = (float2(0.0 , 0.0));
				float2 LocalUVOffset184 = ( appendResult176 + appendResult182 );
				float2 uv_AlphaOverride = IN.ase_texcoord1.xy * _AlphaOverride_ST.xy + _AlphaOverride_ST.zw;
				float temp_output_4_0_g118 = _FlipbooksColumsRows.x;
				float temp_output_5_0_g118 = _FlipbooksColumsRows.y;
				float2 appendResult7_g118 = (float2(temp_output_4_0_g118 , temp_output_5_0_g118));
				float totalFrames39_g118 = ( temp_output_4_0_g118 * temp_output_5_0_g118 );
				float2 appendResult8_g118 = (float2(totalFrames39_g118 , temp_output_5_0_g118));
				float4 texCoord75 = IN.ase_texcoord;
				texCoord75.xy = IN.ase_texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float clampResult42_g118 = clamp( texCoord75.x , 0.0001 , ( totalFrames39_g118 - 1.0 ) );
				float temp_output_35_0_g118 = frac( ( ( (float)0 + clampResult42_g118 ) / totalFrames39_g118 ) );
				float2 appendResult29_g118 = (float2(temp_output_35_0_g118 , ( 1.0 - temp_output_35_0_g118 )));
				float2 temp_output_15_0_g118 = ( ( ( LocalUVOffset184 + uv_AlphaOverride ) / appendResult7_g118 ) + ( floor( ( appendResult8_g118 * appendResult29_g118 ) ) / appendResult7_g118 ) );
				float2 panner44 = ( 1.0 * _Time.y * _AlphaOverridePanning + temp_output_15_0_g118);
				float4 break2_g121 = ( tex2D( _AlphaOverride, panner44 ) * _AlphaOverrideChannel );
				float AlphaOverride49 = saturate( ( break2_g121.x + break2_g121.y + break2_g121.z + break2_g121.w ) );
				float2 uv_DetailNoise = IN.ase_texcoord1.xy * _DetailNoise_ST.xy + _DetailNoise_ST.zw;
				float2 panner80 = ( 1.0 * _Time.y * _DetailNoisePanning + uv_DetailNoise);
				float4 tex2DNode79 = tex2D( _DetailNoise, panner80 );
				float4 break17_g75 = tex2DNode79;
				float4 appendResult18_g75 = (float4(break17_g75.x , break17_g75.y , break17_g75.z , break17_g75.w));
				float4 clampResult19_g75 = clamp( ( appendResult18_g75 * _DetailDistortionChannel ) , float4( 0,0,0,0 ) , float4( 1,1,1,1 ) );
				float4 break2_g75 = clampResult19_g75;
				float clampResult20_g75 = clamp( ( break2_g75.x + break2_g75.y + break2_g75.z + break2_g75.w ) , 0.0 , 1.0 );
				float3 temp_cast_3 = (clampResult20_g75).xxx;
				float3 desaturateInitialColor190 = temp_cast_3;
				float desaturateDot190 = dot( desaturateInitialColor190, float3( 0.299, 0.587, 0.114 ));
				float3 desaturateVar190 = lerp( desaturateInitialColor190, desaturateDot190.xxx, 1.0 );
				float3 DistortionNoise90 = desaturateVar190;
				float2 uv_MainTex = IN.ase_texcoord1.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				float3 UVFlipbookInput194 = ( ( DistortionNoise90 * _DistortionIntensity ) + float3( ( LocalUVOffset184 + uv_MainTex ) ,  0.0 ) );
				float temp_output_4_0_g119 = _FlipbooksColumsRows.x;
				float temp_output_5_0_g119 = _FlipbooksColumsRows.y;
				float2 appendResult7_g119 = (float2(temp_output_4_0_g119 , temp_output_5_0_g119));
				float totalFrames39_g119 = ( temp_output_4_0_g119 * temp_output_5_0_g119 );
				float2 appendResult8_g119 = (float2(totalFrames39_g119 , temp_output_5_0_g119));
				float clampResult42_g119 = clamp( texCoord75.x , 0.0001 , ( totalFrames39_g119 - 1.0 ) );
				float temp_output_35_0_g119 = frac( ( ( (float)0 + clampResult42_g119 ) / totalFrames39_g119 ) );
				float2 appendResult29_g119 = (float2(temp_output_35_0_g119 , ( 1.0 - temp_output_35_0_g119 )));
				float2 temp_output_15_0_g119 = ( ( UVFlipbookInput194.xy / appendResult7_g119 ) + ( floor( ( appendResult8_g119 * appendResult29_g119 ) ) / appendResult7_g119 ) );
				float2 temp_output_73_0 = temp_output_15_0_g119;
				float2 panner33 = ( 1.0 * _Time.y * _MainAlphaPanning + temp_output_73_0);
				float4 break2_g120 = ( tex2D( _MainTex, panner33 ) * _MainAlphaChannel );
				float MainAlpha30 = saturate( ( break2_g120.x + break2_g120.y + break2_g120.z + break2_g120.w ) );
				float temp_output_55_0 = ( AlphaOverride49 * MainAlpha30 );
				float4 texCoord60 = IN.ase_texcoord1;
				texCoord60.xy = IN.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g122 = ( texCoord60.w - ( 1.0 - temp_output_55_0 ) );
				float temp_output_40_0 = ( IN.ase_color.a * temp_output_55_0 * saturate( saturate( ( temp_output_3_0_g122 / fwidth( temp_output_3_0_g122 ) ) ) ) );
				float4 screenPos = IN.ase_texcoord2;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float screenDepth199 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float distanceDepth199 = abs( ( screenDepth199 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( _SoftFadeFactor ) );
				#ifdef _USESOFTALPHA_ON
				float staticSwitch198 = ( temp_output_40_0 * saturate( distanceDepth199 ) );
				#else
				float staticSwitch198 = temp_output_40_0;
				#endif
				

				surfaceDescription.Alpha = staticSwitch198;
				surfaceDescription.AlphaClipThreshold = 0.5;

				#if _ALPHATEST_ON
					float alphaClipThreshold = 0.01f;
					#if ALPHA_CLIP_THRESHOLD
						alphaClipThreshold = surfaceDescription.AlphaClipThreshold;
					#endif
					clip(surfaceDescription.Alpha - alphaClipThreshold);
				#endif

				half4 outColor = 0;
				outColor = _SelectionID;

				return outColor;
			}

			ENDHLSL
		}

		
		Pass
		{
			
            Name "DepthNormals"
            Tags { "LightMode"="DepthNormalsOnly" }

			ZTest LEqual
			ZWrite On


			HLSLPROGRAM

			#pragma multi_compile_instancing
			#define _SURFACE_TYPE_TRANSPARENT 1
			#define ASE_SRP_VERSION 120111
			#define REQUIRE_DEPTH_TEXTURE 1


			#pragma vertex vert
			#pragma fragment frag

			#define ATTRIBUTES_NEED_NORMAL
			#define ATTRIBUTES_NEED_TANGENT
			#define VARYINGS_NEED_NORMAL_WS

			#define SHADERPASS SHADERPASS_DEPTHNORMALSONLY

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#define ASE_NEEDS_FRAG_COLOR
			#pragma shader_feature_local _USESOFTALPHA_ON


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_color : COLOR;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				float3 normalWS : TEXCOORD0;
				float4 ase_color : COLOR;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _DetailNoise_ST;
			float4 _DetailDistortionChannel;
			float4 _MainTex_ST;
			float4 _MainTextureChannel;
			float4 _MainAlphaChannel;
			float4 _DetailAdditiveChannel;
			float4 _DetailMultiplyChannel;
			float4 _AlphaOverride_ST;
			float4 _AlphaOverrideChannel;
			float2 _MainTexturePanning;
			float2 _DetailNoisePanning;
			float2 _MainAlphaPanning;
			float2 _FlipbooksColumsRows;
			float2 _AlphaOverridePanning;
			float _Desaturate;
			float _DistortionIntensity;
			float _SoftFadeFactor;
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _AlphaOverride;
			sampler2D _MainTex;
			sampler2D _DetailNoise;
			uniform float4 _CameraDepthTexture_TexelSize;


			
			struct SurfaceDescription
			{
				float Alpha;
				float AlphaClipThreshold;
			};

			VertexOutput VertexFunction(VertexInput v  )
			{
				VertexOutput o;
				ZERO_INITIALIZE(VertexOutput, o);

				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float4 ase_clipPos = TransformObjectToHClip((v.vertex).xyz);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord3 = screenPos;
				
				o.ase_color = v.ase_color;
				o.ase_texcoord1 = v.ase_texcoord1;
				o.ase_texcoord2 = v.ase_texcoord;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = defaultVertexValue;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float3 normalWS = TransformObjectToWorldNormal(v.ase_normal);

				o.clipPos = TransformWorldToHClip(positionWS);
				o.normalWS.xyz =  normalWS;

				return o;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_color : COLOR;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_color = v.ase_color;
				o.ase_texcoord1 = v.ase_texcoord1;
				o.ase_texcoord = v.ase_texcoord;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				o.ase_texcoord1 = patch[0].ase_texcoord1 * bary.x + patch[1].ase_texcoord1 * bary.y + patch[2].ase_texcoord1 * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag(VertexOutput IN ) : SV_TARGET
			{
				SurfaceDescription surfaceDescription = (SurfaceDescription)0;

				float4 texCoord168 = IN.ase_texcoord1;
				texCoord168.xy = IN.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult176 = (float2(texCoord168.z , 0.0));
				float2 appendResult182 = (float2(0.0 , 0.0));
				float2 LocalUVOffset184 = ( appendResult176 + appendResult182 );
				float2 uv_AlphaOverride = IN.ase_texcoord2.xy * _AlphaOverride_ST.xy + _AlphaOverride_ST.zw;
				float temp_output_4_0_g118 = _FlipbooksColumsRows.x;
				float temp_output_5_0_g118 = _FlipbooksColumsRows.y;
				float2 appendResult7_g118 = (float2(temp_output_4_0_g118 , temp_output_5_0_g118));
				float totalFrames39_g118 = ( temp_output_4_0_g118 * temp_output_5_0_g118 );
				float2 appendResult8_g118 = (float2(totalFrames39_g118 , temp_output_5_0_g118));
				float4 texCoord75 = IN.ase_texcoord1;
				texCoord75.xy = IN.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float clampResult42_g118 = clamp( texCoord75.x , 0.0001 , ( totalFrames39_g118 - 1.0 ) );
				float temp_output_35_0_g118 = frac( ( ( (float)0 + clampResult42_g118 ) / totalFrames39_g118 ) );
				float2 appendResult29_g118 = (float2(temp_output_35_0_g118 , ( 1.0 - temp_output_35_0_g118 )));
				float2 temp_output_15_0_g118 = ( ( ( LocalUVOffset184 + uv_AlphaOverride ) / appendResult7_g118 ) + ( floor( ( appendResult8_g118 * appendResult29_g118 ) ) / appendResult7_g118 ) );
				float2 panner44 = ( 1.0 * _Time.y * _AlphaOverridePanning + temp_output_15_0_g118);
				float4 break2_g121 = ( tex2D( _AlphaOverride, panner44 ) * _AlphaOverrideChannel );
				float AlphaOverride49 = saturate( ( break2_g121.x + break2_g121.y + break2_g121.z + break2_g121.w ) );
				float2 uv_DetailNoise = IN.ase_texcoord2.xy * _DetailNoise_ST.xy + _DetailNoise_ST.zw;
				float2 panner80 = ( 1.0 * _Time.y * _DetailNoisePanning + uv_DetailNoise);
				float4 tex2DNode79 = tex2D( _DetailNoise, panner80 );
				float4 break17_g75 = tex2DNode79;
				float4 appendResult18_g75 = (float4(break17_g75.x , break17_g75.y , break17_g75.z , break17_g75.w));
				float4 clampResult19_g75 = clamp( ( appendResult18_g75 * _DetailDistortionChannel ) , float4( 0,0,0,0 ) , float4( 1,1,1,1 ) );
				float4 break2_g75 = clampResult19_g75;
				float clampResult20_g75 = clamp( ( break2_g75.x + break2_g75.y + break2_g75.z + break2_g75.w ) , 0.0 , 1.0 );
				float3 temp_cast_3 = (clampResult20_g75).xxx;
				float3 desaturateInitialColor190 = temp_cast_3;
				float desaturateDot190 = dot( desaturateInitialColor190, float3( 0.299, 0.587, 0.114 ));
				float3 desaturateVar190 = lerp( desaturateInitialColor190, desaturateDot190.xxx, 1.0 );
				float3 DistortionNoise90 = desaturateVar190;
				float2 uv_MainTex = IN.ase_texcoord2.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				float3 UVFlipbookInput194 = ( ( DistortionNoise90 * _DistortionIntensity ) + float3( ( LocalUVOffset184 + uv_MainTex ) ,  0.0 ) );
				float temp_output_4_0_g119 = _FlipbooksColumsRows.x;
				float temp_output_5_0_g119 = _FlipbooksColumsRows.y;
				float2 appendResult7_g119 = (float2(temp_output_4_0_g119 , temp_output_5_0_g119));
				float totalFrames39_g119 = ( temp_output_4_0_g119 * temp_output_5_0_g119 );
				float2 appendResult8_g119 = (float2(totalFrames39_g119 , temp_output_5_0_g119));
				float clampResult42_g119 = clamp( texCoord75.x , 0.0001 , ( totalFrames39_g119 - 1.0 ) );
				float temp_output_35_0_g119 = frac( ( ( (float)0 + clampResult42_g119 ) / totalFrames39_g119 ) );
				float2 appendResult29_g119 = (float2(temp_output_35_0_g119 , ( 1.0 - temp_output_35_0_g119 )));
				float2 temp_output_15_0_g119 = ( ( UVFlipbookInput194.xy / appendResult7_g119 ) + ( floor( ( appendResult8_g119 * appendResult29_g119 ) ) / appendResult7_g119 ) );
				float2 temp_output_73_0 = temp_output_15_0_g119;
				float2 panner33 = ( 1.0 * _Time.y * _MainAlphaPanning + temp_output_73_0);
				float4 break2_g120 = ( tex2D( _MainTex, panner33 ) * _MainAlphaChannel );
				float MainAlpha30 = saturate( ( break2_g120.x + break2_g120.y + break2_g120.z + break2_g120.w ) );
				float temp_output_55_0 = ( AlphaOverride49 * MainAlpha30 );
				float4 texCoord60 = IN.ase_texcoord2;
				texCoord60.xy = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g122 = ( texCoord60.w - ( 1.0 - temp_output_55_0 ) );
				float temp_output_40_0 = ( IN.ase_color.a * temp_output_55_0 * saturate( saturate( ( temp_output_3_0_g122 / fwidth( temp_output_3_0_g122 ) ) ) ) );
				float4 screenPos = IN.ase_texcoord3;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float screenDepth199 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float distanceDepth199 = abs( ( screenDepth199 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( _SoftFadeFactor ) );
				#ifdef _USESOFTALPHA_ON
				float staticSwitch198 = ( temp_output_40_0 * saturate( distanceDepth199 ) );
				#else
				float staticSwitch198 = temp_output_40_0;
				#endif
				

				surfaceDescription.Alpha = staticSwitch198;
				surfaceDescription.AlphaClipThreshold = 0.5;

				#if _ALPHATEST_ON
					clip(surfaceDescription.Alpha - surfaceDescription.AlphaClipThreshold);
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif

				float3 normalWS = IN.normalWS;

				return half4(NormalizeNormalPerPixel(normalWS), 0.0);
			}

			ENDHLSL
		}

		
		Pass
		{
			
            Name "DepthNormalsOnly"
            Tags { "LightMode"="DepthNormalsOnly" }

			ZTest LEqual
			ZWrite On

			HLSLPROGRAM

			#pragma multi_compile_instancing
			#define _SURFACE_TYPE_TRANSPARENT 1
			#define ASE_SRP_VERSION 120111
			#define REQUIRE_DEPTH_TEXTURE 1


			#pragma exclude_renderers glcore gles gles3 
			#pragma vertex vert
			#pragma fragment frag

			#define ATTRIBUTES_NEED_NORMAL
			#define ATTRIBUTES_NEED_TANGENT
			#define ATTRIBUTES_NEED_TEXCOORD1
			#define VARYINGS_NEED_NORMAL_WS
			#define VARYINGS_NEED_TANGENT_WS

			#define SHADERPASS SHADERPASS_DEPTHNORMALSONLY

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#define ASE_NEEDS_FRAG_COLOR
			#pragma shader_feature_local _USESOFTALPHA_ON


			struct VertexInput
			{
				float4 vertex : POSITION;
				float3 ase_normal : NORMAL;
				float4 ase_color : COLOR;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 clipPos : SV_POSITION;
				float3 normalWS : TEXCOORD0;
				float4 ase_color : COLOR;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			CBUFFER_START(UnityPerMaterial)
			float4 _DetailNoise_ST;
			float4 _DetailDistortionChannel;
			float4 _MainTex_ST;
			float4 _MainTextureChannel;
			float4 _MainAlphaChannel;
			float4 _DetailAdditiveChannel;
			float4 _DetailMultiplyChannel;
			float4 _AlphaOverride_ST;
			float4 _AlphaOverrideChannel;
			float2 _MainTexturePanning;
			float2 _DetailNoisePanning;
			float2 _MainAlphaPanning;
			float2 _FlipbooksColumsRows;
			float2 _AlphaOverridePanning;
			float _Desaturate;
			float _DistortionIntensity;
			float _SoftFadeFactor;
			#ifdef ASE_TESSELLATION
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END
			sampler2D _AlphaOverride;
			sampler2D _MainTex;
			sampler2D _DetailNoise;
			uniform float4 _CameraDepthTexture_TexelSize;


			
			struct SurfaceDescription
			{
				float Alpha;
				float AlphaClipThreshold;
			};

			VertexOutput VertexFunction(VertexInput v  )
			{
				VertexOutput o;
				ZERO_INITIALIZE(VertexOutput, o);

				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				float4 ase_clipPos = TransformObjectToHClip((v.vertex).xyz);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord3 = screenPos;
				
				o.ase_color = v.ase_color;
				o.ase_texcoord1 = v.ase_texcoord1;
				o.ase_texcoord2 = v.ase_texcoord;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.vertex.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif

				float3 vertexValue = defaultVertexValue;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.vertex.xyz = vertexValue;
				#else
					v.vertex.xyz += vertexValue;
				#endif

				v.ase_normal = v.ase_normal;

				float3 positionWS = TransformObjectToWorld( v.vertex.xyz );
				float3 normalWS = TransformObjectToWorldNormal(v.ase_normal);

				o.clipPos = TransformWorldToHClip(positionWS);
				o.normalWS.xyz =  normalWS;

				return o;
			}

			#if defined(ASE_TESSELLATION)
			struct VertexControl
			{
				float4 vertex : INTERNALTESSPOS;
				float3 ase_normal : NORMAL;
				float4 ase_color : COLOR;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.vertex = v.vertex;
				o.ase_normal = v.ase_normal;
				o.ase_color = v.ase_color;
				o.ase_texcoord1 = v.ase_texcoord1;
				o.ase_texcoord = v.ase_texcoord;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), _WorldSpaceCameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(v[0].vertex, v[1].vertex, v[2].vertex, edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), _WorldSpaceCameraPos, _ScreenParams, unity_CameraWorldClipPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
				return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.vertex = patch[0].vertex * bary.x + patch[1].vertex * bary.y + patch[2].vertex * bary.z;
				o.ase_normal = patch[0].ase_normal * bary.x + patch[1].ase_normal * bary.y + patch[2].ase_normal * bary.z;
				o.ase_color = patch[0].ase_color * bary.x + patch[1].ase_color * bary.y + patch[2].ase_color * bary.z;
				o.ase_texcoord1 = patch[0].ase_texcoord1 * bary.x + patch[1].ase_texcoord1 * bary.y + patch[2].ase_texcoord1 * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.vertex.xyz - patch[i].ase_normal * (dot(o.vertex.xyz, patch[i].ase_normal) - dot(patch[i].vertex.xyz, patch[i].ase_normal));
				float phongStrength = _TessPhongStrength;
				o.vertex.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.vertex.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			half4 frag(VertexOutput IN ) : SV_TARGET
			{
				SurfaceDescription surfaceDescription = (SurfaceDescription)0;

				float4 texCoord168 = IN.ase_texcoord1;
				texCoord168.xy = IN.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult176 = (float2(texCoord168.z , 0.0));
				float2 appendResult182 = (float2(0.0 , 0.0));
				float2 LocalUVOffset184 = ( appendResult176 + appendResult182 );
				float2 uv_AlphaOverride = IN.ase_texcoord2.xy * _AlphaOverride_ST.xy + _AlphaOverride_ST.zw;
				float temp_output_4_0_g118 = _FlipbooksColumsRows.x;
				float temp_output_5_0_g118 = _FlipbooksColumsRows.y;
				float2 appendResult7_g118 = (float2(temp_output_4_0_g118 , temp_output_5_0_g118));
				float totalFrames39_g118 = ( temp_output_4_0_g118 * temp_output_5_0_g118 );
				float2 appendResult8_g118 = (float2(totalFrames39_g118 , temp_output_5_0_g118));
				float4 texCoord75 = IN.ase_texcoord1;
				texCoord75.xy = IN.ase_texcoord1.xy * float2( 1,1 ) + float2( 0,0 );
				float clampResult42_g118 = clamp( texCoord75.x , 0.0001 , ( totalFrames39_g118 - 1.0 ) );
				float temp_output_35_0_g118 = frac( ( ( (float)0 + clampResult42_g118 ) / totalFrames39_g118 ) );
				float2 appendResult29_g118 = (float2(temp_output_35_0_g118 , ( 1.0 - temp_output_35_0_g118 )));
				float2 temp_output_15_0_g118 = ( ( ( LocalUVOffset184 + uv_AlphaOverride ) / appendResult7_g118 ) + ( floor( ( appendResult8_g118 * appendResult29_g118 ) ) / appendResult7_g118 ) );
				float2 panner44 = ( 1.0 * _Time.y * _AlphaOverridePanning + temp_output_15_0_g118);
				float4 break2_g121 = ( tex2D( _AlphaOverride, panner44 ) * _AlphaOverrideChannel );
				float AlphaOverride49 = saturate( ( break2_g121.x + break2_g121.y + break2_g121.z + break2_g121.w ) );
				float2 uv_DetailNoise = IN.ase_texcoord2.xy * _DetailNoise_ST.xy + _DetailNoise_ST.zw;
				float2 panner80 = ( 1.0 * _Time.y * _DetailNoisePanning + uv_DetailNoise);
				float4 tex2DNode79 = tex2D( _DetailNoise, panner80 );
				float4 break17_g75 = tex2DNode79;
				float4 appendResult18_g75 = (float4(break17_g75.x , break17_g75.y , break17_g75.z , break17_g75.w));
				float4 clampResult19_g75 = clamp( ( appendResult18_g75 * _DetailDistortionChannel ) , float4( 0,0,0,0 ) , float4( 1,1,1,1 ) );
				float4 break2_g75 = clampResult19_g75;
				float clampResult20_g75 = clamp( ( break2_g75.x + break2_g75.y + break2_g75.z + break2_g75.w ) , 0.0 , 1.0 );
				float3 temp_cast_3 = (clampResult20_g75).xxx;
				float3 desaturateInitialColor190 = temp_cast_3;
				float desaturateDot190 = dot( desaturateInitialColor190, float3( 0.299, 0.587, 0.114 ));
				float3 desaturateVar190 = lerp( desaturateInitialColor190, desaturateDot190.xxx, 1.0 );
				float3 DistortionNoise90 = desaturateVar190;
				float2 uv_MainTex = IN.ase_texcoord2.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				float3 UVFlipbookInput194 = ( ( DistortionNoise90 * _DistortionIntensity ) + float3( ( LocalUVOffset184 + uv_MainTex ) ,  0.0 ) );
				float temp_output_4_0_g119 = _FlipbooksColumsRows.x;
				float temp_output_5_0_g119 = _FlipbooksColumsRows.y;
				float2 appendResult7_g119 = (float2(temp_output_4_0_g119 , temp_output_5_0_g119));
				float totalFrames39_g119 = ( temp_output_4_0_g119 * temp_output_5_0_g119 );
				float2 appendResult8_g119 = (float2(totalFrames39_g119 , temp_output_5_0_g119));
				float clampResult42_g119 = clamp( texCoord75.x , 0.0001 , ( totalFrames39_g119 - 1.0 ) );
				float temp_output_35_0_g119 = frac( ( ( (float)0 + clampResult42_g119 ) / totalFrames39_g119 ) );
				float2 appendResult29_g119 = (float2(temp_output_35_0_g119 , ( 1.0 - temp_output_35_0_g119 )));
				float2 temp_output_15_0_g119 = ( ( UVFlipbookInput194.xy / appendResult7_g119 ) + ( floor( ( appendResult8_g119 * appendResult29_g119 ) ) / appendResult7_g119 ) );
				float2 temp_output_73_0 = temp_output_15_0_g119;
				float2 panner33 = ( 1.0 * _Time.y * _MainAlphaPanning + temp_output_73_0);
				float4 break2_g120 = ( tex2D( _MainTex, panner33 ) * _MainAlphaChannel );
				float MainAlpha30 = saturate( ( break2_g120.x + break2_g120.y + break2_g120.z + break2_g120.w ) );
				float temp_output_55_0 = ( AlphaOverride49 * MainAlpha30 );
				float4 texCoord60 = IN.ase_texcoord2;
				texCoord60.xy = IN.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float temp_output_3_0_g122 = ( texCoord60.w - ( 1.0 - temp_output_55_0 ) );
				float temp_output_40_0 = ( IN.ase_color.a * temp_output_55_0 * saturate( saturate( ( temp_output_3_0_g122 / fwidth( temp_output_3_0_g122 ) ) ) ) );
				float4 screenPos = IN.ase_texcoord3;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float screenDepth199 = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH( ase_screenPosNorm.xy ),_ZBufferParams);
				float distanceDepth199 = abs( ( screenDepth199 - LinearEyeDepth( ase_screenPosNorm.z,_ZBufferParams ) ) / ( _SoftFadeFactor ) );
				#ifdef _USESOFTALPHA_ON
				float staticSwitch198 = ( temp_output_40_0 * saturate( distanceDepth199 ) );
				#else
				float staticSwitch198 = temp_output_40_0;
				#endif
				

				surfaceDescription.Alpha = staticSwitch198;
				surfaceDescription.AlphaClipThreshold = 0.5;

				#if _ALPHATEST_ON
					clip(surfaceDescription.Alpha - surfaceDescription.AlphaClipThreshold);
				#endif

				#ifdef LOD_FADE_CROSSFADE
					LODDitheringTransition( IN.clipPos.xyz, unity_LODFade.x );
				#endif

				float3 normalWS = IN.normalWS;

				return half4(NormalizeNormalPerPixel(normalWS), 0.0);
			}

			ENDHLSL
		}
		
	}
	
	CustomEditor "UnityEditor.ShaderGraphUnlitGUI"
	FallBack "Hidden/Shader Graph/FallbackError"
	
	Fallback Off
}
/*ASEBEGIN
Version=19105
Node;AmplifyShaderEditor.CommentaryNode;103;-853.4689,-2259.997;Inherit;False;1884.647;1001.187;Extra Noise Setup;22;91;191;155;156;87;157;85;90;190;158;79;83;80;84;81;92;189;86;105;162;108;106;;1,1,1,1;0;0
Node;AmplifyShaderEditor.TexturePropertyNode;83;-814.2831,-2017.479;Inherit;True;Property;_DetailNoise;Detail Noise;10;0;Create;True;0;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.TextureCoordinatesNode;81;-564.8421,-1844.278;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;84;-560.261,-1728.191;Inherit;False;Property;_DetailNoisePanning;Detail Noise Panning;11;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.PannerNode;80;-326.8439,-1769.278;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;173;1088,-1728;Inherit;False;869.2021;446.9999;UV Offset Controlled by custom vertex stream;6;184;179;180;176;182;168;;0.3699214,0.2971698,1,1;0;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;168;1120,-1664;Inherit;False;1;-1;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;79;-140.0461,-2013.979;Inherit;True;Property;_TextureSample3;Texture Sample 3;4;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector4Node;85;-116.0612,-1805.789;Inherit;False;Property;_DetailDistortionChannel;Detail Distortion Channel;12;0;Create;True;0;0;0;False;0;False;1,0,0,0;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;180;1136,-1424;Inherit;False;Constant;_InitialOffset;Initial Offset;16;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;182;1408,-1424;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FunctionNode;158;163.2251,-2003.815;Inherit;False;Channel Picker;-1;;75;dc5f4cb24a8bdf448b40a1ec5866280e;0;2;5;FLOAT4;1,0,0,0;False;7;FLOAT4;0,0,0,1;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;176;1328,-1600;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;179;1600,-1456;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;1,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DesaturateOpNode;190;190.6096,-1917.282;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;184;1760,-1456;Inherit;False;LocalUVOffset;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;90;172.7251,-1829.615;Inherit;False;DistortionNoise;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;197;1095.931,-2387.688;Inherit;False;853.4072;636.7309;Set UV Modifiers For Main Tex;8;93;96;186;7;174;95;94;194;;1,0.8279877,0,1;0;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;7;1152.569,-1914.957;Inherit;False;0;27;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;186;1185.564,-2043.374;Inherit;False;184;LocalUVOffset;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;93;1149.931,-2337.688;Inherit;False;90;DistortionNoise;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;95;1145.931,-2175.688;Inherit;False;Property;_DistortionIntensity;Distortion Intensity;13;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;94;1371.933,-2262.688;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;50;-2911.51,-2551.852;Inherit;False;1894.068;530.1917;Alpha Override;12;49;165;48;44;45;177;193;51;188;187;43;47;;0,0.5461459,1,1;0;0
Node;AmplifyShaderEditor.SimpleAddOpNode;174;1411.421,-2066.139;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;47;-2880.768,-2493.259;Inherit;True;Property;_AlphaOverride;Alpha Override;6;0;Create;True;0;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SimpleAddOpNode;96;1548.933,-2129.688;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT2;0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;187;-2592.767,-2413.259;Inherit;False;184;LocalUVOffset;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;43;-2619.169,-2337.565;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;34;-2548.24,-1516.688;Inherit;False;1576.333;998.0396;Main Texture Set Vars;16;195;25;163;10;6;22;23;30;164;28;12;33;27;32;73;78;;0,0.5461459,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;194;1715.339,-2074.068;Inherit;False;UVFlipbookInput;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.IntNode;193;-2276.262,-2251.418;Inherit;False;Constant;_Int1;Int 1;9;0;Create;True;0;0;0;False;0;False;0;0;False;0;1;INT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;188;-2400.768,-2413.259;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.IntNode;78;-2286.642,-1281.145;Inherit;False;Constant;_Int0;Int 0;9;0;Create;True;0;0;0;False;0;False;0;0;False;0;1;INT;0
Node;AmplifyShaderEditor.GetLocalVarNode;195;-2524.121,-1448.583;Inherit;False;194;UVFlipbookInput;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;75;-2512.07,-1982.654;Inherit;False;1;-1;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;74;-2538.623,-1820.153;Inherit;False;Property;_FlipbooksColumsRows;Flipbooks Colums & Rows;9;0;Create;True;0;0;0;False;0;False;1,1;1,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.FunctionNode;177;-2274.88,-2413.233;Inherit;False;Flipbook;-1;;118;53c2488c220f6564ca6c90721ee16673;2,71,0,68,0;8;51;SAMPLER2D;0.0;False;13;FLOAT2;0,0;False;4;FLOAT;3;False;5;FLOAT;3;False;24;FLOAT;0;False;2;FLOAT;0;False;55;FLOAT;0;False;70;FLOAT;0;False;5;COLOR;53;FLOAT2;0;FLOAT;47;FLOAT;48;FLOAT;62
Node;AmplifyShaderEditor.Vector2Node;51;-2289.648,-2177.604;Inherit;False;Property;_AlphaOverridePanning;Alpha Override Panning;7;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.FunctionNode;73;-2289.329,-1441.295;Inherit;False;Flipbook;-1;;119;53c2488c220f6564ca6c90721ee16673;2,71,0,68,0;8;51;SAMPLER2D;0.0;False;13;FLOAT2;0,0;False;4;FLOAT;3;False;5;FLOAT;3;False;24;FLOAT;0;False;2;FLOAT;0;False;55;FLOAT;0;False;70;FLOAT;0;False;5;COLOR;53;FLOAT2;0;FLOAT;47;FLOAT;48;FLOAT;62
Node;AmplifyShaderEditor.Vector2Node;32;-1965.995,-1302.504;Inherit;False;Property;_MainAlphaPanning;Main Alpha Panning;5;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.PannerNode;44;-2000.767,-2381.259;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;33;-1965.995,-1414.504;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;27;-1997.995,-1110.504;Inherit;True;Property;_MainTex;Main Texture;0;0;Create;False;0;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SamplerNode;28;-1741.995,-1430.504;Inherit;True;Property;_TextureSample1;Texture Sample 1;4;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector4Node;12;-1717.069,-1206.742;Inherit;False;Property;_MainAlphaChannel;Main Alpha Channel;3;0;Create;True;0;0;0;False;0;False;0,0,0,1;0,0,0,1;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector4Node;48;-1750.768,-2290.259;Inherit;False;Property;_AlphaOverrideChannel;Alpha Override Channel;8;0;Create;True;0;0;0;False;0;False;1,0,0,0;0,1,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;45;-1808.768,-2477.259;Inherit;True;Property;_TextureSample2;Texture Sample 2;4;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;164;-1421.995,-1430.504;Inherit;False;Channel Picker Alpha;-1;;120;e49841402b321534583d1dc019041b68;0;2;5;FLOAT4;1,0,0,0;False;7;FLOAT4;0,0,0,1;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;165;-1504.768,-2477.259;Inherit;False;Channel Picker Alpha;-1;;121;e49841402b321534583d1dc019041b68;0;2;5;FLOAT4;1,0,0,0;False;7;FLOAT4;0,0,0,1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;49;-1296.768,-2477.259;Inherit;True;AlphaOverride;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;30;-1197.995,-1430.504;Inherit;True;MainAlpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;52;-208.8856,-525.6209;Inherit;False;49;AlphaOverride;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;53;-186.5113,-450.0185;Inherit;False;30;MainAlpha;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;55;5.488959,-546.0185;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;64;188.3074,-430.0071;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;60;127.5556,-355.1935;Inherit;False;0;-1;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;202;153.5714,-84.502;Inherit;False;Property;_SoftFadeFactor;SoftFadeFactor;17;0;Create;True;0;0;0;False;0;False;0;0;0.1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;70;365.1349,-362.5852;Inherit;False;Step Antialiasing;-1;;122;2a825e80dfb3290468194f83380797bd;0;2;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;199;436.5714,-155.502;Inherit;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;61;708.7333,-566.5916;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;68;545.5906,-363.4737;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;203;692.5714,-160.502;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;40;730.5135,-407.2835;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;201;914.9233,-360.6032;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DesaturateOpNode;191;804.6099,-2039.383;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FunctionNode;162;183.4254,-1498.115;Inherit;False;Channel Picker;-1;;142;dc5f4cb24a8bdf448b40a1ec5866280e;0;2;5;FLOAT4;1,0,0,0;False;7;FLOAT4;0,0,0,1;False;1;FLOAT;0
Node;AmplifyShaderEditor.DesaturateOpNode;189;572.5096,-1546.582;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;98;768.8834,-899.3236;Inherit;False;92;MultiplyNoise;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.Vector2Node;23;-1981.995,-726.5037;Inherit;False;Property;_MainTexturePanning;Main Texture Panning;4;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.BreakToComponentsNode;156;549.593,-2040.846;Inherit;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.FunctionNode;163;-1405.995,-774.5037;Inherit;False;Channel Picker;-1;;143;dc5f4cb24a8bdf448b40a1ec5866280e;0;2;5;FLOAT4;1,0,0,0;False;7;FLOAT4;0,0,0,1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;101;269.4243,-985.1658;Inherit;False;91;AdditiveNoise;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;157;426.8263,-2036.38;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleAddOpNode;105;55.67661,-1545.463;Inherit;False;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;155;676.3929,-2043.947;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;108;253.3054,-1362.1;Inherit;False;Constant;_Float0;Float 0;15;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;25;-1197.995,-774.5037;Inherit;True;MainTexInfo;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;102;612.7458,-1010.09;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.Vector4Node;87;151.4388,-2185.69;Inherit;False;Property;_DetailAdditiveChannel;Detail Additive Channel;15;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector4Node;10;-1749.062,-746.3322;Inherit;False;Property;_MainTextureChannel;Main Texture Channel;2;0;Create;True;0;0;0;False;0;False;1,1,1,0;1,1,1,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StaticSwitch;198;978.9233,-552.6033;Inherit;False;Property;_UseSoftAlpha;UseSoftAlpha;16;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;36;68.63216,-1082.473;Inherit;False;25;MainTexInfo;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;6;-1757.995,-966.5038;Inherit;True;Property;_TextureSample0;Texture Sample 0;0;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;91;801.5255,-1951.116;Inherit;False;AdditiveNoise;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;39;794.9197,-1040.442;Inherit;False;4;4;0;COLOR;1,0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;3;FLOAT3;0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;92;729.9253,-1547.715;Inherit;False;MultiplyNoise;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.Vector4Node;86;-193.6613,-1574.091;Inherit;False;Property;_DetailMultiplyChannel;Detail Multiply Channel;14;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;37;572.5109,-1181.602;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;136;565.3867,-724.3369;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ConditionalIfNode;106;414.8053,-1547.1;Inherit;False;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;22;-1965.995,-838.5037;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DesaturateOpNode;166;73.56007,-1012.246;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;71;521.1351,-885.5583;Inherit;False;0;-1;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;167;14.5006,-924.7495;Inherit;False;Property;_Desaturate;Desaturate?;1;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;205;1346.249,-1045.851;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;ExtraPrePass;0;0;ExtraPrePass;5;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;False;True;1;1;False;;0;False;;0;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;0;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;206;1346.249,-1045.851;Float;False;True;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;13;Piloto Studio/Alpha Uber Shader;2992e84f91cbeb14eab234972e07ea9d;True;Forward;0;1;Forward;8;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;UniversalMaterialType=Unlit;True;7;True;12;all;0;False;True;1;5;False;;10;False;;1;1;False;;10;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;2;False;;True;3;False;;True;True;0;False;;0;False;;True;1;LightMode=UniversalForwardOnly;False;False;0;;0;0;Standard;23;Surface;1;638227594256591002;  Blend;0;0;Two Sided;1;0;Forward Only;0;0;Cast Shadows;0;638227594294084865;  Use Shadow Threshold;0;0;Receive Shadows;1;0;GPU Instancing;1;0;LOD CrossFade;0;0;Built-in Fog;0;0;DOTS Instancing;0;0;Meta Pass;0;0;Extra Pre Pass;0;0;Tessellation;0;0;  Phong;0;0;  Strength;0.5,False,;0;  Type;0;0;  Tess;16,False,;0;  Min;10,False,;0;  Max;25,False,;0;  Edge Length;16,False,;0;  Max Displacement;25,False,;0;Vertex Position,InvertActionOnDeselection;1;0;0;10;False;True;False;True;False;False;True;True;True;True;False;;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;207;1346.249,-1045.851;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;ShadowCaster;0;2;ShadowCaster;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;False;False;True;False;False;False;False;0;False;;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;False;True;1;LightMode=ShadowCaster;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;208;1346.249,-1045.851;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;DepthOnly;0;3;DepthOnly;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;False;False;True;False;False;False;False;0;False;;False;False;False;False;False;False;False;False;False;True;1;False;;False;False;True;1;LightMode=DepthOnly;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;209;1346.249,-1045.851;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;Meta;0;4;Meta;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Meta;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;210;1346.249,-1045.851;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;Universal2D;0;5;Universal2D;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;False;True;1;1;False;;0;False;;0;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;1;LightMode=Universal2D;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;211;1346.249,-1045.851;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;SceneSelectionPass;0;6;SceneSelectionPass;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=SceneSelectionPass;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;212;1346.249,-1045.851;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;ScenePickingPass;0;7;ScenePickingPass;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Picking;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;213;1346.249,-1045.851;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;DepthNormals;0;8;DepthNormals;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;False;True;1;LightMode=DepthNormalsOnly;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;214;1346.249,-1045.851;Float;False;False;-1;2;UnityEditor.ShaderGraphUnlitGUI;0;1;New Amplify Shader;2992e84f91cbeb14eab234972e07ea9d;True;DepthNormalsOnly;0;9;DepthNormalsOnly;0;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;False;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;False;False;False;True;4;RenderPipeline=UniversalPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;UniversalMaterialType=Unlit;True;3;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;False;;True;3;False;;False;True;1;LightMode=DepthNormalsOnly;False;True;9;d3d11;metal;vulkan;xboxone;xboxseries;playstation;ps4;ps5;switch;0;;0;0;Standard;0;False;0
WireConnection;81;2;83;0
WireConnection;80;0;81;0
WireConnection;80;2;84;0
WireConnection;79;0;83;0
WireConnection;79;1;80;0
WireConnection;182;0;180;0
WireConnection;158;5;79;0
WireConnection;158;7;85;0
WireConnection;176;0;168;3
WireConnection;179;0;176;0
WireConnection;179;1;182;0
WireConnection;190;0;158;0
WireConnection;184;0;179;0
WireConnection;90;0;190;0
WireConnection;94;0;93;0
WireConnection;94;1;95;0
WireConnection;174;0;186;0
WireConnection;174;1;7;0
WireConnection;96;0;94;0
WireConnection;96;1;174;0
WireConnection;43;2;47;0
WireConnection;194;0;96;0
WireConnection;188;0;187;0
WireConnection;188;1;43;0
WireConnection;177;13;188;0
WireConnection;177;4;74;1
WireConnection;177;5;74;2
WireConnection;177;24;75;1
WireConnection;177;2;193;0
WireConnection;73;13;195;0
WireConnection;73;4;74;1
WireConnection;73;5;74;2
WireConnection;73;24;75;1
WireConnection;73;2;78;0
WireConnection;44;0;177;0
WireConnection;44;2;51;0
WireConnection;33;0;73;0
WireConnection;33;2;32;0
WireConnection;28;0;27;0
WireConnection;28;1;33;0
WireConnection;45;0;47;0
WireConnection;45;1;44;0
WireConnection;164;5;28;0
WireConnection;164;7;12;0
WireConnection;165;5;45;0
WireConnection;165;7;48;0
WireConnection;49;0;165;0
WireConnection;30;0;164;0
WireConnection;55;0;52;0
WireConnection;55;1;53;0
WireConnection;64;0;55;0
WireConnection;70;1;64;0
WireConnection;70;2;60;4
WireConnection;199;0;202;0
WireConnection;68;0;70;0
WireConnection;203;0;199;0
WireConnection;40;0;61;4
WireConnection;40;1;55;0
WireConnection;40;2;68;0
WireConnection;201;0;40;0
WireConnection;201;1;203;0
WireConnection;191;0;155;0
WireConnection;162;5;79;0
WireConnection;162;7;86;0
WireConnection;189;0;106;0
WireConnection;156;0;157;0
WireConnection;163;5;6;0
WireConnection;163;7;10;0
WireConnection;157;0;87;0
WireConnection;157;1;79;0
WireConnection;105;0;86;1
WireConnection;105;1;86;2
WireConnection;105;2;86;3
WireConnection;105;3;86;4
WireConnection;155;0;156;0
WireConnection;155;1;156;1
WireConnection;155;2;156;2
WireConnection;155;3;156;3
WireConnection;25;0;163;0
WireConnection;102;0;166;0
WireConnection;102;1;101;0
WireConnection;198;1;40;0
WireConnection;198;0;201;0
WireConnection;6;0;27;0
WireConnection;6;1;22;0
WireConnection;91;0;191;0
WireConnection;39;0;37;0
WireConnection;39;1;102;0
WireConnection;39;2;136;0
WireConnection;39;3;98;0
WireConnection;92;0;189;0
WireConnection;136;0;71;3
WireConnection;106;0;105;0
WireConnection;106;2;162;0
WireConnection;106;3;108;0
WireConnection;106;4;108;0
WireConnection;22;0;73;0
WireConnection;22;2;23;0
WireConnection;166;0;36;0
WireConnection;166;1;167;0
WireConnection;206;2;39;0
WireConnection;206;3;198;0
ASEEND*/
//CHKSM=4C9751F3246B30D73909D84260D4CB120E6EC83C