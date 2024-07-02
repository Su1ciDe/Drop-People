// Made with Amplify Shader Editor v1.9.2.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "FiberShaders/FToonOutline"
{
	Properties
	{
		[Header(SHADOWS)]_ShadowStrength("Shadow Strength", Range( 0 , 1)) = 0.65
		_ShadowIntensity("Shadow Intensity", Range( 0 , 1)) = 0.95
		[Enum(Near,0,Exact,1)]_ShadowApproxmation("Shadow Approxmation", Float) = 1
		_ShadowSharpness("Shadow Sharpness", Range( 0.01 , 1)) = 0.7
		_ShadowOffset("Shadow Offset", Range( 0 , 1)) = 0.5
		_MainColor1("Main Color", Color) = (0.6,0.6,0.6,1)
		_MainTexture("Main Texture", 2D) = "white" {}
		_ShadeColor("Shade Color", Color) = (0.35,0.35,0.35,1)
		_ToonizeVar("ToonizeVar", Range( 1 , 200)) = 1
		_ShadingContrast("Shading Contrast", Range( 0.1 , 4)) = 1.5
		_BaseCellSharpness("Base Cell Sharpness", Range( 0 , 1)) = 1
		_BaseCellOffset("Base Cell Offset", Range( -1 , 1)) = 0
		_ShadingContribution("Shading Contribution", Range( 0 , 2)) = 0.7
		[Normal]_NormalMap("Normal Map", 2D) = "bump" {}
		[Toggle]_Normal("Normal", Float) = 0
		_NormalScale("Normal Scale", Range( -1 , 1)) = 0.3
		[HDR]_SpecColor1("Specular Color", Color) = (1,1,1,1)
		_SpecularIntensity("Specular Intensity", Range( 0 , 1)) = 1
		[Toggle]_Specular("Specular", Float) = 0
		_SpecularAmbient("Specular Ambient", Range( 0 , 1)) = 0
		_SpecularGlossy("SpecularGlossy", Range( 0 , 100)) = 5
		_ToonizeSpecular("Toonize Specular", Range( 1 , 250)) = 1
		_SpecularCelIn("Specular Cel In", Range( 0.1 , 2)) = 1
		_SpecularCelOut("Specular Cel Out", Range( 0.1 , 2)) = 1
		[HDR]_RimColor("Rim Color", Color) = (0.4,0.4,0.4,0)
		_RimBias("Rim Bias", Range( 0 , 1)) = 0
		_ReflectFresnelBias("Reflect Fresnel Bias", Range( 0 , 1)) = 0
		_RimScale("Rim Scale", Range( 0 , 5)) = 1
		_ReflectFresnelScale("Reflect Fresnel Scale", Range( 0 , 5)) = 1
		[Toggle]_MultiplyRim("Multiply Rim", Float) = 0
		_ReflectFresnelPower("Reflect Fresnel Power", Range( 0 , 5)) = 1.5
		_RimPower("Rim Power", Range( 0 , 5)) = 1.5
		_OutlineSize("Outline Size", Range( 0 , 1)) = 0.05
		_OutlineColor("Outline Color", Color) = (0,0,0,0)
		_ReflectMap1("Reflect Map", CUBE) = "white" {}
		[Toggle]_Toonize("Toonize", Float) = 0
		[Toggle]_ShadeFineTune("Shade Fine Tune", Float) = 0
		[Toggle]_SpecularToonize("SpecularToonize", Float) = 0
		[Toggle]_SpecularCelFinetune("Specular Cel Finetune", Float) = 0
		[Toggle]_RimLight("RimLight", Float) = 0
		_RimIntensity("Rim Intensity", Range( 0 , 1)) = 1
		_ReflectionStrength("Reflection Strength", Range( 0 , 1)) = 1
		[HDR]_ReflectColor("Reflect Color", Color) = (1,1,1,1)
		[Toggle]_Reflect("Reflect", Float) = 0
		[Toggle]_ShadeFold("ShadeFold", Float) = 1
		[Toggle]_Outline("Outline", Float) = 0
		[Toggle]_ReflectionFresnel("ReflectionFresnel", Float) = 0
		_CubeMapRotate("Cube Map Rotate", Range( 0 , 360)) = 0
		_CubemapYPosition("Cubemap Y Position", Range( -5 , 5)) = 0
		[Toggle]_RefFresnelInvert("RefFresnelInvert", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ }
		ZWrite On
		Cull Front
		CGPROGRAM
		#pragma target 3.0
		#pragma surface outlineSurf Outline  keepalpha noshadow noambient novertexlights nolightmap nodynlightmap nodirlightmap nometa noforwardadd vertex:outlineVertexDataFunc 
		#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
		#pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
		#pragma multi_compile _ _FORWARD_PLUS
		
		void outlineVertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float outlineVar = (( _Outline )?( _OutlineSize ):( 0.0 ));
			v.vertex.xyz += ( v.normal * outlineVar );
		}
		inline half4 LightingOutline( SurfaceOutput s, half3 lightDir, half atten ) { return half4 ( 0,0,0, s.Alpha); }
		void outlineSurf( Input i, inout SurfaceOutput o )
		{
			o.Emission = _OutlineColor.rgb;
		}
		ENDCG
		

		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" }
		Cull Back
		ZWrite On
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "UnityStandardUtils.cginc"
		#include "UnityCG.cginc"
		#include "UnityShaderVariables.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
		#pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
		#pragma multi_compile _ _FORWARD_PLUS
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float3 worldNormal;
			INTERNAL_DATA
			float2 uv_texcoord;
			float3 worldPos;
			float3 worldRefl;
		};

		struct SurfaceOutputCustomLightingCustom
		{
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			half Alpha;
			Input SurfInput;
			UnityGIInput GIData;
		};

		uniform float _MultiplyRim;
		uniform float _Specular;
		uniform float _SpecularToonize;
		uniform float _SpecularCelFinetune;
		uniform float _SpecularGlossy;
		uniform float _Normal;
		uniform sampler2D _NormalMap;
		uniform float4 _NormalMap_ST;
		uniform float _NormalScale;
		uniform float _SpecularCelOut;
		uniform float _SpecularCelIn;
		uniform float _ToonizeSpecular;
		uniform float _SpecularAmbient;
		uniform float4 _SpecColor1;
		uniform float _SpecularIntensity;
		uniform float _ShadeFold;
		uniform half _ShadowStrength;
		uniform half _ShadowApproxmation;
		uniform half _ShadowOffset;
		uniform half _ShadowSharpness;
		uniform float _ShadowIntensity;
		uniform float _Toonize;
		uniform float _ToonizeVar;
		uniform float _ShadingContrast;
		uniform float _ShadeFineTune;
		uniform float _BaseCellOffset;
		uniform float _BaseCellSharpness;
		uniform float4 _MainColor1;
		uniform sampler2D _MainTexture;
		uniform float4 _MainTexture_ST;
		uniform float4 _ShadeColor;
		uniform float _ShadingContribution;
		uniform float _RimBias;
		uniform float _RimScale;
		uniform float _RimPower;
		uniform float4 _RimColor;
		uniform float _RimIntensity;
		uniform float _RimLight;
		uniform float _Reflect;
		uniform float _ReflectionStrength;
		uniform samplerCUBE _ReflectMap1;
		uniform float _CubeMapRotate;
		uniform float _CubemapYPosition;
		uniform float4 _ReflectColor;
		uniform float _ReflectionFresnel;
		uniform float _RefFresnelInvert;
		uniform float _ReflectFresnelBias;
		uniform float _ReflectFresnelScale;
		uniform float _ReflectFresnelPower;
		uniform float4 _OutlineColor;
		uniform float _Outline;
		uniform float _OutlineSize;


		float4 CalculateContrast( float contrastValue, float4 colorTarget )
		{
			float t = 0.5 * ( 1.0 - contrastValue );
			return mul( float4x4( contrastValue,0,0,t, 0,contrastValue,0,t, 0,0,contrastValue,t, 0,0,0,1 ), colorTarget );
		}

		float3 ASESafeNormalize(float3 inVec)
		{
			float dp3 = max(1.175494351e-38, dot(inVec, inVec));
			return inVec* rsqrt(dp3);
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 Outline249 = 0;
			v.vertex.xyz += Outline249;
			v.vertex.w = 1;
		}

		inline half4 LightingStandardCustomLighting( inout SurfaceOutputCustomLightingCustom s, half3 viewDir, UnityGI gi )
		{
			UnityGIInput data = s.GIData;
			Input i = s.SurfInput;
			half4 c = 0;
			#ifdef UNITY_PASS_FORWARDBASE
			float ase_lightAtten = data.atten;
			if( _LightColor0.a == 0)
			ase_lightAtten = 0;
			#else
			float3 ase_lightAttenRGB = gi.light.color / ( ( _LightColor0.rgb ) + 0.000001 );
			float ase_lightAtten = max( max( ase_lightAttenRGB.r, ase_lightAttenRGB.g ), ase_lightAttenRGB.b );
			#endif
			#if defined(HANDLE_SHADOWS_BLENDING_IN_GI)
			half bakedAtten = UnitySampleBakedOcclusion(data.lightmapUV.xy, data.worldPos);
			float zDist = dot(_WorldSpaceCameraPos - data.worldPos, UNITY_MATRIX_V[2].xyz);
			float fadeDist = UnityComputeShadowFadeDistance(data.worldPos, zDist);
			ase_lightAtten = UnityMixRealtimeAndBakedShadows(data.atten, bakedAtten, UnityComputeShadowFade(fadeDist));
			#endif
			float4 temp_cast_0 = (0.0).xxxx;
			float2 temp_output_1_0_g64 = float2( 0,0 );
			float dotResult4_g64 = dot( temp_output_1_0_g64 , temp_output_1_0_g64 );
			float3 appendResult10_g64 = (float3((temp_output_1_0_g64).x , (temp_output_1_0_g64).y , sqrt( ( 1.0 - saturate( dotResult4_g64 ) ) )));
			float3 normalizeResult12_g64 = normalize( appendResult10_g64 );
			float2 uv_NormalMap = i.uv_texcoord * _NormalMap_ST.xy + _NormalMap_ST.zw;
			float3 NewNormals95 = normalize( (WorldNormalVector( i , (( _Normal )?( UnpackScaleNormal( tex2D( _NormalMap, uv_NormalMap ), _NormalScale ) ):( normalizeResult12_g64 )) )) );
			float3 ase_worldPos = i.worldPos;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = Unity_SafeNormalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float dotResult52 = dot( NewNormals95 , ase_worldlightDir );
			float NdotL56 = max( dotResult52 , 0.0 );
			float4 temp_cast_1 = (NdotL56).xxxx;
			float4 temp_output_42_0_g62 = temp_cast_1;
			float4 temp_cast_2 = (( 1.0 - (temp_output_42_0_g62).a )).xxxx;
			float4 temp_output_109_0 = CalculateContrast(_SpecularGlossy,temp_cast_2);
			float4 temp_cast_3 = (_SpecularCelOut).xxxx;
			float4 temp_cast_4 = (_SpecularCelIn).xxxx;
			float4 temp_cast_6 = (_SpecularCelOut).xxxx;
			float4 temp_cast_7 = (_SpecularCelIn).xxxx;
			float div115=256.0/float((int)_ToonizeSpecular);
			float4 posterize115 = ( floor( (( _SpecularCelFinetune )?( (float4( 0,0,0,0 ) + (temp_output_109_0 - float4( 0,0,0,0 )) * (temp_cast_4 - float4( 0,0,0,0 )) / (temp_cast_3 - float4( 0,0,0,0 ))) ):( temp_output_109_0 )) * div115 ) / div115 );
			float4 temp_cast_8 = (_SpecularAmbient).xxxx;
			float4 clampResult120 = clamp( ( 1.0 - (( _SpecularToonize )?( posterize115 ):( (( _SpecularCelFinetune )?( (float4( 0,0,0,0 ) + (temp_output_109_0 - float4( 0,0,0,0 )) * (temp_cast_4 - float4( 0,0,0,0 )) / (temp_cast_3 - float4( 0,0,0,0 ))) ):( temp_output_109_0 )) )) ) , temp_cast_8 , float4( 1,1,1,0 ) );
			float4 Specular121 = ( clampResult120 * ( _SpecColor1 * _SpecularIntensity ) );
			float3 temp_output_975_0_g65 = float3( 0,0,1 );
			float3 newWorldNormal1072_g65 = normalize( (WorldNormalVector( i , temp_output_975_0_g65 )) );
			float3 ase_worldViewDir = Unity_SafeNormalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 normalizeResult1044_g65 = ASESafeNormalize( ( ase_worldViewDir + ase_worldlightDir ) );
			float dotResult1046_g65 = dot( newWorldNormal1072_g65 , normalizeResult1044_g65 );
			float Dot_NdotH1098_g65 = dotResult1046_g65;
			float dotResult1055_g65 = dot( newWorldNormal1072_g65 , ase_worldlightDir );
			float Dot_NdotL1100_g65 = dotResult1055_g65;
			float lerpResult1130_g65 = lerp( ( _ShadowStrength * _ShadowStrength * 0.7978846 ) , ( _ShadowStrength * _ShadowStrength * sqrt( ( 2.0 / ( 2.0 * UNITY_PI ) ) ) ) , _ShadowApproxmation);
			float RoughnessApproxmation1138_g65 = lerpResult1130_g65;
			float temp_output_1312_0_g65 = ( ( max( Dot_NdotL1100_g65 , 0.0 ) * RoughnessApproxmation1138_g65 ) + ( 1.0 - RoughnessApproxmation1138_g65 ) );
			float temp_output_1308_0_g65 = ( max( Dot_NdotH1098_g65 , 0.0 ) * temp_output_1312_0_g65 * temp_output_1312_0_g65 );
			float temp_output_1108_0_g65 = ( 1.0 - ( ( 1.0 - ase_lightAtten ) * _WorldSpaceLightPos0.w ) );
			float lerpResult1109_g65 = lerp( ( saturate( ( ( temp_output_1308_0_g65 + _ShadowOffset ) / _ShadowSharpness ) ) * ase_lightAtten ) , temp_output_1108_0_g65 , _ShadowStrength);
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			float4 ase_lightColor = 0;
			#else //aselc
			float4 ase_lightColor = _LightColor0;
			#endif //aselc
			float3 LightColor1348_g65 = ase_lightColor.rgb;
			float3 temp_output_1125_0_g65 = ( lerpResult1109_g65 * LightColor1348_g65 * ( ( 1.0 - _ShadowIntensity ) * 10.0 ) );
			float LightColor_NdotL104 = ( ase_lightAtten * NdotL56 );
			float temp_output_60_0 = ( LightColor_NdotL104 + _BaseCellOffset );
			float2 uv_MainTexture = i.uv_texcoord * _MainTexture_ST.xy + _MainTexture_ST.zw;
			float4 tex2DNode149 = tex2D( _MainTexture, uv_MainTexture );
			float4 MainDiffuse61 = ( _MainColor1 * tex2DNode149 );
			float div101=256.0/float((int)(( _Toonize )?( _ToonizeVar ):( 1.0 )));
			float4 posterize101 = ( floor( CalculateContrast(_ShadingContrast,saturate( ( ( saturate( (( _ShadeFineTune )?( ( temp_output_60_0 / _BaseCellSharpness ) ):( LightColor_NdotL104 )) ) * MainDiffuse61 ) + ( saturate( ( 1.0 - temp_output_60_0 ) ) * _ShadeColor * _ShadingContribution * MainDiffuse61 ) ) )) * div101 ) / div101 );
			float3 temp_output_898_0_g65 = ( float4( ase_lightColor.rgb , 0.0 ) * posterize101 ).rgb;
			float3 temp_output_1210_0_g65 = saturate( ( temp_output_1125_0_g65 * temp_output_898_0_g65 ) );
			float3 temp_output_260_0 = (temp_output_1210_0_g65*2.0 + 0.0);
			float3 BaseColor103 = (( _ShadeFold )?( temp_output_260_0 ):( temp_output_260_0 ));
			float fresnelNdotV76 = dot( normalize( NewNormals95 ), ase_worldViewDir );
			float fresnelNode76 = ( _RimBias + _RimScale * pow( max( 1.0 - fresnelNdotV76 , 0.0001 ), _RimPower ) );
			float4 RimSetUp81 = ( ( fresnelNode76 * _RimColor * _RimIntensity ) * (( _RimLight )?( 1.0 ):( 0.0 )) );
			float3 NewObjectNormal144 = (( _Normal )?( UnpackScaleNormal( tex2D( _NormalMap, uv_NormalMap ), _NormalScale ) ):( normalizeResult12_g64 ));
			half3 VertexPos40_g60 = normalize( WorldReflectionVector( i , NewObjectNormal144 ) );
			float3 appendResult74_g60 = (float3(0.0 , VertexPos40_g60.y , 0.0));
			float3 VertexPosRotationAxis50_g60 = appendResult74_g60;
			float3 break84_g60 = VertexPos40_g60;
			float3 appendResult81_g60 = (float3(break84_g60.x , 0.0 , break84_g60.z));
			float3 VertexPosOtherAxis82_g60 = appendResult81_g60;
			half Angle44_g60 = radians( _CubeMapRotate );
			float3 appendResult127 = (float3(0.0 , -_CubemapYPosition , 0.0));
			float fresnelNdotV234 = dot( normalize( NewNormals95 ), ase_worldViewDir );
			float fresnelNode234 = ( _ReflectFresnelBias + _ReflectFresnelScale * pow( max( 1.0 - fresnelNdotV234 , 0.0001 ), _ReflectFresnelPower ) );
			float ReflectFresnel242 = (( _ReflectionFresnel )?( (( _RefFresnelInvert )?( ( 1.0 - fresnelNode234 ) ):( fresnelNode234 )) ):( 1.0 ));
			float4 Reflect137 = (( _Reflect )?( ( _ReflectionStrength * ( texCUBE( _ReflectMap1, ( ( VertexPosRotationAxis50_g60 + ( VertexPosOtherAxis82_g60 * cos( Angle44_g60 ) ) + ( cross( float3(0,1,0) , VertexPosOtherAxis82_g60 ) * sin( Angle44_g60 ) ) ) + appendResult127 ) ) * _ReflectColor ) * ReflectFresnel242 ) ):( float4( 0,0,0,0 ) ));
			float4 temp_cast_13 = (0.0).xxxx;
			float RimSwitch196 = (( _RimLight )?( 1.0 ):( 0.0 ));
			float Fresnel80 = fresnelNode76;
			float4 lerpResult160 = lerp( ( (( _Specular )?( Specular121 ):( temp_cast_13 )) + float4( BaseColor103 , 0.0 ) + Reflect137 ) , RimSetUp81 , ( RimSwitch196 * Fresnel80 ));
			c.rgb = (( _MultiplyRim )?( lerpResult160 ):( ( (( _Specular )?( Specular121 ):( temp_cast_0 )) + float4( BaseColor103 , 0.0 ) + RimSetUp81 + Reflect137 ) )).rgb;
			c.a = 1;
			return c;
		}

		inline void LightingStandardCustomLighting_GI( inout SurfaceOutputCustomLightingCustom s, UnityGIInput data, inout UnityGI gi )
		{
			s.GIData = data;
		}

		void surf( Input i , inout SurfaceOutputCustomLightingCustom o )
		{
			o.SurfInput = i;
			o.Normal = float3(0,0,1);
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf StandardCustomLighting alpha:fade keepalpha fullforwardshadows vertex:vertexDataFunc 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float4 tSpace0 : TEXCOORD2;
				float4 tSpace1 : TEXCOORD3;
				float4 tSpace2 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				vertexDataFunc( v, customInputData );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.worldRefl = -worldViewDir;
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutputCustomLightingCustom o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputCustomLightingCustom, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "FToonOutlineEditor"
}
/*ASEBEGIN
Version=19202
Node;AmplifyShaderEditor.CommentaryNode;241;-8228.663,-3635.04;Inherit;False;1612.246;619.855;Reflect Fresnel;10;233;232;231;235;240;242;237;230;234;229;Reflect Fresnel;0,0.7859545,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;43;-10141.79,-2912.354;Inherit;False;1121.2;207.2;Camera Mode;5;124;123;122;94;93;Camera Mode;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;44;-9654.813,-3581.43;Inherit;False;1362.807;553.545;Comment;7;145;144;95;48;47;46;45;Normals;0.4402515,0.4635113,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;49;-8214.272,-4273.571;Inherit;False;801.7261;511.2055;Normal dot Light;5;56;52;51;50;259;Normal dot Light;0.7171531,0.5226415,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;53;-7385.863,-4910.708;Inherit;False;790.9033;522.4771;Main Texture;6;149;151;152;61;147;150;Main Texture;0.4566038,0.9526708,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;54;-10189.78,-715.2166;Inherit;False;3228.367;540.4689;Specular Setup;21;114;106;105;110;112;111;109;108;107;113;115;118;116;223;143;117;221;120;121;119;224;Specular Setup;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;55;-10220.85,-2077.979;Inherit;False;4116.102;1171.419;Base Color;28;257;57;252;103;203;62;59;58;60;68;96;148;101;98;146;65;199;200;99;66;63;64;67;100;102;97;69;260;Base Color;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleAddOpNode;69;-8426.078,-1597.981;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;70;-9788.907,-4315.146;Inherit;False;1512.866;597.899;Rim Light;14;142;81;80;77;76;75;74;73;72;71;191;196;225;228;Rim Light;0.4422169,1,0.3915094,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;82;-9778.104,-5008.682;Inherit;False;1409.389;612.5826;Indirect Diffuse;6;92;90;89;88;87;84;Indirect Diffuse;0.9610584,0.681,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;83;-8228.428,-4857.242;Inherit;False;814.6841;469.7439;Light Affect;4;104;91;85;258;Light Affect;0.9947262,1,0.6176471,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;85;-7892.427,-4681.242;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;87;-8928.894,-4782.643;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;90;-8592.384,-4783.259;Inherit;True;IndirectDiffuse;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;93;-9643.794,-2862.355;Half;False;Constant;_Float7;Float 7;47;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;97;-8154.08,-1581.981;Inherit;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;102;-7306.082,-1629.981;Inherit;True;2;2;0;FLOAT3;0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;104;-7663.428,-4684.242;Float;True;LightColor_NdotL;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;122;-9451.794,-2862.355;Inherit;False;3;0;FLOAT;1;False;1;FLOAT;0.5;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;123;-9829.794,-2865.355;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OrthoParams;124;-10091.79,-2862.355;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;125;-8950.609,-2919.294;Inherit;False;2322.336;690.999;Reflection Map;17;243;140;136;137;138;128;133;132;127;130;139;131;129;141;135;134;245;Reflection Map;0,0.4211543,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;84;-9304.938,-4671.802;Float;True;Constant;_IndirectDiffuseContribution;Indirect Diffuse Contribution;20;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;88;-9747.691,-4762.797;Inherit;True;95;NewNormals;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FunctionNode;89;-9525.94,-4765.017;Inherit;True;Dielectric Specular;13;;48;cf90616a2350c5c4cba1069366c98fa1;1,1,0;2;2;FLOAT;0;False;9;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;92;-9312.878,-4960.463;Float;True;Constant;_Float8;Float 8;20;0;Create;True;0;0;0;False;0;False;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LightColorNode;100;-7594.082,-1757.98;Inherit;True;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;151;-7038.25,-4630.094;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;61;-6806.23,-4854.022;Inherit;True;MainDiffuse;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;152;-6806.84,-4632.04;Inherit;True;MainAlpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;135;-7539.772,-2593.313;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;129;-8421.519,-2676.727;Inherit;True;Compute Rotation Y;-1;;60;693b7d13a80c93a4e8b791a9cd5e5ab2;0;2;38;FLOAT3;0,0,0;False;43;FLOAT;0;False;1;FLOAT3;19
Node;AmplifyShaderEditor.RadiansOpNode;131;-8581.517,-2628.727;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldReflectionVector;130;-8677.518,-2852.727;Inherit;True;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DynamicAppendNode;127;-8273.263,-2389.917;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.NegateNode;132;-8449.259,-2373.917;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;128;-8118.723,-2671.527;Inherit;True;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;77;-9112.634,-4230.101;Inherit;True;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;191;-8815.75,-4234.49;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;80;-8530.07,-4010.497;Inherit;True;Fresnel;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;67;-8890.09,-2013.979;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;64;-8922.09,-1789.98;Inherit;True;61;MainDiffuse;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;63;-9098.091,-1581.981;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;66;-8890.09,-1581.981;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;199;-7805.554,-1245.213;Inherit;False;Property;_Toonize;Toonize;48;0;Create;True;0;0;0;False;0;False;0;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;65;-8986.09,-1341.982;Float;True;Property;_ShadingContribution;Shading Contribution;21;0;Create;True;0;0;0;False;0;False;0.7;1.35;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;146;-8938.09,-1101.982;Inherit;False;Property;_ShadeColor;Shade Color;16;0;Create;True;1;Shade Settings;0;0;False;0;False;0.35,0.35,0.35,1;0.490566,0,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;147;-7273.208,-4853.708;Inherit;False;Property;_MainColor1;Main Color;11;0;Create;True;1;Diffuse Settings;0;0;False;0;False;0.6,0.6,0.6,1;0.7924528,0.7924528,0.7924528,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;149;-7361.863,-4626.774;Inherit;True;Property;_MainTexture;Main Texture;12;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleContrastOpNode;98;-7850.081,-1581.981;Inherit;True;2;1;COLOR;0,0,0,0;False;0;FLOAT;-0.66;False;1;COLOR;0
Node;AmplifyShaderEditor.PosterizeNode;101;-7578.082,-1517.982;Inherit;True;1;2;1;COLOR;0,0,0,0;False;0;INT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;96;-8666.079,-1405.982;Inherit;True;4;4;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;68;-8650.078,-1869.98;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;150;-7036.704,-4852.682;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;138;-8908.932,-2852.617;Inherit;True;144;NewObjectNormal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;60;-9681.68,-1929.875;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;58;-10074.05,-1785.214;Float;True;Property;_BaseCellOffset;Base Cell Offset;20;0;Create;True;0;0;0;False;0;False;0;0;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;59;-9746.72,-1698.088;Float;True;Property;_BaseCellSharpness;Base Cell Sharpness;19;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;62;-9397.14,-1930.368;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0.01;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;119;-7394.835,-566.0664;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;121;-7162.807,-568.5975;Inherit;True;Specular;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;221;-7578.66,-454.8982;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;116;-8070.896,-672.9167;Inherit;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;107;-9911.519,-654.2256;Inherit;False;Blinn-Phong Light;41;;62;cf814dba44d007a4e958d2ddd5813da6;0;3;42;COLOR;0,0,0,0;False;52;FLOAT3;0,0,0;False;43;COLOR;0,0,0,0;False;2;COLOR;0;FLOAT;57
Node;AmplifyShaderEditor.OneMinusNode;108;-9689.019,-656.7936;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleContrastOpNode;109;-9504.019,-663.3285;Inherit;True;2;1;COLOR;0,0,0,0;False;0;FLOAT;2.86;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;105;-10161.35,-654.6043;Inherit;True;56;NdotL;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;106;-10158.75,-450.2426;Inherit;True;95;NewNormals;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TFHCRemapNode;113;-9189.809,-584.444;Inherit;True;5;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0.092,0.092,0.092,1;False;3;COLOR;0,0,0,0;False;4;COLOR;0.795,0.795,0.795,1;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;111;-9509.358,-402.5589;Inherit;False;Property;_SpecularCelOut;Specular Cel Out;32;0;Create;True;0;0;0;False;0;False;1;1;0.1;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;112;-9505.938,-300.3598;Inherit;False;Property;_SpecularCelIn;Specular Cel In;31;0;Create;True;0;0;0;False;0;False;1;1;0.1;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.PosterizeNode;115;-8576.048,-477.7154;Inherit;True;52;2;1;COLOR;0,0,0,0;False;0;INT;52;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;81;-8529.247,-4238.78;Inherit;True;RimSetUp;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;196;-8810.074,-3978.418;Inherit;True;RimSwitch;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;228;-9428.493,-3815.199;Inherit;False;Property;_RimIntensity;Rim Intensity;53;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;142;-9363.673,-3997.379;Inherit;False;Property;_RimColor;Rim Color;33;1;[HDR];Create;True;1;Rim Light Settings;0;0;False;0;False;0.4,0.4,0.4,0;0.4,0.4,0.4,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;145;-9299.803,-3517.43;Inherit;True;Property;_NormalMap;Normal Map;22;1;[Normal];Create;True;1;Normal Settings;0;0;False;0;False;-1;None;6d39320162db6324482a2e59f12ba2d1;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ToggleSwitchNode;47;-8986.393,-3514.717;Inherit;True;Property;_Normal;Normal;23;0;Create;True;0;0;0;False;0;False;0;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;45;-9603.813,-3517.43;Float;True;Property;_NormalScale;Normal Scale;24;0;Create;True;0;0;0;False;0;False;0.3;0.3;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;46;-9263.103,-3279.811;Inherit;True;Normal Reconstruct Z;-1;;64;63ba85b764ae0c84ab3d698b86364ae9;0;1;1;FLOAT2;0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;144;-8513.022,-3263.627;Inherit;True;NewObjectNormal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WorldNormalVector;48;-8748.074,-3517.499;Inherit;True;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RegisterLocalVarNode;95;-8496.197,-3514.622;Float;True;NewNormals;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;75;-9745.374,-4270.833;Inherit;False;95;NewNormals;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;71;-9744.383,-4189.426;Inherit;True;World;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;72;-9751,-3980.844;Inherit;False;Property;_RimBias;Rim Bias;34;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;73;-9751,-3900.844;Inherit;False;Property;_RimScale;Rim Scale;36;0;Create;True;0;0;0;False;0;False;1;1;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;74;-9751,-3808.844;Inherit;False;Property;_RimPower;Rim Power;40;0;Create;True;0;0;0;False;0;False;1.5;1.5;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.FresnelNode;76;-9450.906,-4231.846;Inherit;True;Standard;WorldNormal;ViewDir;True;True;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;229;-8173.036,-3585.04;Inherit;False;95;NewNormals;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FresnelNode;234;-7878.572,-3546.052;Inherit;True;Standard;WorldNormal;ViewDir;True;True;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;230;-8172.044,-3503.632;Inherit;True;World;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.OneMinusNode;237;-7553.29,-3468.771;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;136;-7272.377,-2594.465;Inherit;True;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;140;-7603.772,-2705.313;Inherit;False;Property;_ReflectionStrength;Reflection Strength;54;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;243;-7542.905,-2917.443;Inherit;True;242;ReflectFresnel;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;137;-6833.228,-2598.802;Inherit;True;Reflect;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;134;-7795.772,-2465.313;Inherit;False;Property;_ReflectColor;Reflect Color;55;1;[HDR];Create;True;0;0;0;False;0;False;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;141;-7891.772,-2673.313;Inherit;True;Property;_ReflectMap1;Reflect Map;47;0;Create;True;1;Reflection Settings;0;0;False;0;False;-1;None;None;True;0;False;white;LockedToCube;False;Object;-1;Auto;Cube;8;0;SAMPLERCUBE;;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ToggleSwitchNode;245;-7051.542,-2594.401;Inherit;False;Property;_Reflect;Reflect;56;0;Create;True;0;0;0;False;0;False;0;True;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;139;-8869.519,-2628.727;Inherit;False;Property;_CubeMapRotate;Cube Map Rotate;60;0;Create;True;0;0;0;False;0;False;0;0;0;360;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;133;-8737.258,-2373.917;Inherit;False;Property;_CubemapYPosition;Cubemap Y Position;61;0;Create;True;0;0;0;False;0;False;0;0;-5;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;231;-8178.662,-3295.051;Inherit;False;Property;_ReflectFresnelBias;Reflect Fresnel Bias;35;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;232;-8178.662,-3210.807;Inherit;False;Property;_ReflectFresnelScale;Reflect Fresnel Scale;37;0;Create;True;0;0;0;False;0;False;1;4.34;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;233;-8178.662,-3123.051;Inherit;False;Property;_ReflectFresnelPower;Reflect Fresnel Power;39;0;Create;True;0;0;0;False;0;False;1.5;1.49;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;246;-7371.689,-4286.177;Inherit;False;974.947;537.7223;Simple Outline;6;253;247;248;249;250;256;Simple Outline;1,1,1,1;0;0
Node;AmplifyShaderEditor.ToggleSwitchNode;203;-9134.617,-2017.356;Inherit;True;Property;_ShadeFineTune;Shade Fine Tune;49;0;Create;True;0;0;0;False;0;False;0;True;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;143;-7823.905,-448.6569;Inherit;False;Property;_SpecColor1;Specular Color;25;1;[HDR];Create;False;1;Specular Settings;0;0;False;0;False;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;120;-7842.487,-672.7087;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;117;-7883.407,-274.3314;Inherit;False;Property;_SpecularIntensity;Specular Intensity;26;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;118;-8165.146,-450.8357;Inherit;False;Property;_SpecularAmbient;Specular Ambient;28;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;114;-8882.508,-298.2086;Inherit;False;Property;_ToonizeSpecular;Toonize Specular;30;0;Create;True;0;0;0;False;0;False;1;1;1;250;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;110;-9833.838,-517.2482;Inherit;False;Property;_SpecularGlossy;SpecularGlossy;29;0;Create;True;0;0;0;False;0;False;5;5;0;100;0;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;240;-7334.387,-3548.471;Inherit;False;Property;_RefFresnelInvert;RefFresnelInvert;62;0;Create;True;0;0;0;False;0;False;0;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;235;-7069.647,-3548.863;Inherit;False;Property;_ReflectionFresnel;ReflectionFresnel;59;0;Create;True;0;0;0;False;0;False;0;True;2;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;242;-6825.225,-3556.81;Inherit;True;ReflectFresnel;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;154;-5321.825,-4542.894;Inherit;True;121;Specular;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;156;-5321.825,-4350.894;Inherit;True;103;BaseColor;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;158;-5321.825,-4142.892;Inherit;True;81;RimSetUp;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;197;-5081.825,-3918.893;Inherit;True;196;RimSwitch;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;195;-4841.824,-3918.893;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;159;-4841.824,-4142.892;Inherit;True;3;3;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;160;-4585.824,-4142.892;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;161;-4553.824,-4542.894;Inherit;True;4;4;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;157;-5081.825,-3710.892;Inherit;True;80;Fresnel;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;162;-5331.637,-4829.4;Inherit;True;137;Reflect;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;219;-5290.617,-4629.451;Inherit;False;Constant;_NonSpecular;NonSpecular;34;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;153;-4281.824,-4318.894;Inherit;True;Property;_MultiplyRim;Multiply Rim;38;0;Create;True;0;0;0;False;0;False;0;True;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ToggleSwitchNode;225;-9098.154,-3999.799;Inherit;False;Property;_RimLight;RimLight;52;0;Create;True;0;0;0;False;0;False;0;True;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;223;-8312.728,-663.4899;Inherit;False;Property;_SpecularToonize;SpecularToonize;50;0;Create;True;0;0;0;False;0;False;0;True;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ToggleSwitchNode;224;-8893.908,-664.8016;Inherit;False;Property;_SpecularCelFinetune;Specular Cel Finetune;51;0;Create;True;0;0;0;False;0;False;0;True;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ToggleSwitchNode;155;-5097.825,-4545.348;Inherit;False;Property;_Specular;Specular;27;0;Create;True;0;0;0;False;0;False;0;True;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;251;-4272.67,-4029.215;Inherit;True;249;Outline;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;256;-7204.596,-3836.635;Inherit;False;Constant;_Float0;Float 0;47;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OutlineNode;250;-6837.863,-4149.51;Inherit;False;0;False;None;1;0;Front;True;True;True;True;0;False;;3;0;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;249;-6626.709,-4150.412;Inherit;True;Outline;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ToggleSwitchNode;253;-7044.287,-4059.584;Inherit;False;Property;_Outline;Outline;58;0;Create;True;0;0;0;False;0;False;0;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;247;-7339.54,-4058.913;Inherit;True;Property;_OutlineSize;Outline Size;45;0;Create;True;1;Outline Controls;0;0;False;0;False;0.05;0.044;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;248;-7278.541,-4233.176;Inherit;False;Property;_OutlineColor;Outline Color;46;0;Create;True;0;0;0;False;0;False;0,0,0,0;0.6320754,0.2264479,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-3948.821,-4318.764;Float;False;True;-1;2;FToonOutlineEditor;0;0;CustomLighting;FiberShaders/FToonOutline;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Transparent;0.5;True;True;0;False;Transparent;;Transparent;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;2;5;False;;10;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.LightAttenuation;258;-8160.694,-4813.278;Inherit;True;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;91;-8132.428,-4601.242;Inherit;True;56;NdotL;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;56;-7570.112,-4117.162;Float;True;NdotL;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;50;-8193.822,-4209.754;Inherit;True;95;NewNormals;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;51;-8201.123,-3986.497;Inherit;True;True;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DotProductOpNode;52;-7957.879,-4113.281;Inherit;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;259;-7727.683,-4116.396;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;57;-10011.98,-2020.149;Inherit;True;104;LightColor_NdotL;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;94;-9253.403,-2857.555;Float;False;CAMERA_MODE;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;103;-6348.083,-1630.662;Float;True;BaseColor;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;260;-6830.838,-1629.203;Inherit;True;3;0;FLOAT3;0,0,0;False;1;FLOAT;2;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FunctionNode;257;-7100.374,-1632.641;Inherit;False;Material Lighting;0;;65;b5db59747a6f5584db32cde93c4695e4;8,1290,0,1289,0,1288,0,1074,0,1323,0,1322,0,1128,0,1280,0;8;898;FLOAT3;0,0,0;False;975;FLOAT3;0,0,1;False;1081;FLOAT;0;False;1079;FLOAT;0;False;1077;FLOAT;0;False;1075;FLOAT;0;False;1246;FLOAT2;0,0;False;1250;FLOAT2;0,0;False;1;FLOAT3;894
Node;AmplifyShaderEditor.RangedFloatNode;200;-8002.773,-1275.904;Inherit;False;Constant;_NotToonize;NotToonize;33;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;99;-8097.19,-1198.982;Inherit;False;Property;_ToonizeVar;ToonizeVar;17;0;Create;True;0;0;0;False;0;False;1;1;1;200;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;148;-8266.078,-1357.982;Inherit;False;Property;_ShadingContrast;Shading Contrast;18;0;Create;True;0;0;0;False;0;False;1.5;1.5;0.1;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;252;-6578.186,-1632.327;Inherit;False;Property;_ShadeFold;ShadeFold;57;0;Create;True;0;0;0;False;0;False;1;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
WireConnection;69;0;68;0
WireConnection;69;1;96;0
WireConnection;85;0;258;0
WireConnection;85;1;91;0
WireConnection;87;0;92;0
WireConnection;87;1;89;0
WireConnection;87;2;84;0
WireConnection;90;0;87;0
WireConnection;97;0;69;0
WireConnection;102;0;100;1
WireConnection;102;1;101;0
WireConnection;104;0;85;0
WireConnection;122;0;93;0
WireConnection;122;1;123;0
WireConnection;122;2;124;4
WireConnection;123;0;124;2
WireConnection;123;1;124;1
WireConnection;89;2;88;0
WireConnection;151;0;147;4
WireConnection;151;1;149;4
WireConnection;61;0;150;0
WireConnection;152;0;151;0
WireConnection;135;0;141;0
WireConnection;135;1;134;0
WireConnection;129;38;130;0
WireConnection;129;43;131;0
WireConnection;131;0;139;0
WireConnection;130;0;138;0
WireConnection;127;1;132;0
WireConnection;132;0;133;0
WireConnection;128;0;129;19
WireConnection;128;1;127;0
WireConnection;77;0;76;0
WireConnection;77;1;142;0
WireConnection;77;2;228;0
WireConnection;191;0;77;0
WireConnection;191;1;225;0
WireConnection;80;0;76;0
WireConnection;67;0;203;0
WireConnection;63;0;60;0
WireConnection;66;0;63;0
WireConnection;199;0;200;0
WireConnection;199;1;99;0
WireConnection;98;1;97;0
WireConnection;98;0;148;0
WireConnection;101;1;98;0
WireConnection;101;0;199;0
WireConnection;96;0;66;0
WireConnection;96;1;146;0
WireConnection;96;2;65;0
WireConnection;96;3;64;0
WireConnection;68;0;67;0
WireConnection;68;1;64;0
WireConnection;150;0;147;0
WireConnection;150;1;149;0
WireConnection;60;0;57;0
WireConnection;60;1;58;0
WireConnection;62;0;60;0
WireConnection;62;1;59;0
WireConnection;119;0;120;0
WireConnection;119;1;221;0
WireConnection;121;0;119;0
WireConnection;221;0;143;0
WireConnection;221;1;117;0
WireConnection;116;0;223;0
WireConnection;107;42;105;0
WireConnection;107;52;106;0
WireConnection;108;0;107;57
WireConnection;109;1;108;0
WireConnection;109;0;110;0
WireConnection;113;0;109;0
WireConnection;113;2;111;0
WireConnection;113;4;112;0
WireConnection;115;1;224;0
WireConnection;115;0;114;0
WireConnection;81;0;191;0
WireConnection;196;0;225;0
WireConnection;145;5;45;0
WireConnection;47;0;46;0
WireConnection;47;1;145;0
WireConnection;144;0;47;0
WireConnection;48;0;47;0
WireConnection;95;0;48;0
WireConnection;76;0;75;0
WireConnection;76;4;71;0
WireConnection;76;1;72;0
WireConnection;76;2;73;0
WireConnection;76;3;74;0
WireConnection;234;0;229;0
WireConnection;234;4;230;0
WireConnection;234;1;231;0
WireConnection;234;2;232;0
WireConnection;234;3;233;0
WireConnection;237;0;234;0
WireConnection;136;0;140;0
WireConnection;136;1;135;0
WireConnection;136;2;243;0
WireConnection;137;0;245;0
WireConnection;141;1;128;0
WireConnection;245;1;136;0
WireConnection;203;0;57;0
WireConnection;203;1;62;0
WireConnection;120;0;116;0
WireConnection;120;1;118;0
WireConnection;240;0;234;0
WireConnection;240;1;237;0
WireConnection;235;1;240;0
WireConnection;242;0;235;0
WireConnection;195;0;197;0
WireConnection;195;1;157;0
WireConnection;159;0;155;0
WireConnection;159;1;156;0
WireConnection;159;2;162;0
WireConnection;160;0;159;0
WireConnection;160;1;158;0
WireConnection;160;2;195;0
WireConnection;161;0;155;0
WireConnection;161;1;156;0
WireConnection;161;2;158;0
WireConnection;161;3;162;0
WireConnection;153;0;161;0
WireConnection;153;1;160;0
WireConnection;223;0;224;0
WireConnection;223;1;115;0
WireConnection;224;0;109;0
WireConnection;224;1;113;0
WireConnection;155;0;219;0
WireConnection;155;1;154;0
WireConnection;250;0;248;0
WireConnection;250;1;253;0
WireConnection;249;0;250;0
WireConnection;253;0;256;0
WireConnection;253;1;247;0
WireConnection;0;13;153;0
WireConnection;0;11;251;0
WireConnection;56;0;259;0
WireConnection;52;0;50;0
WireConnection;52;1;51;0
WireConnection;259;0;52;0
WireConnection;94;0;122;0
WireConnection;103;0;252;0
WireConnection;260;0;257;894
WireConnection;257;898;102;0
WireConnection;252;0;260;0
WireConnection;252;1;260;0
ASEEND*/
//CHKSM=43D0C50D909EA1543D8CCB610FA1727F8E5B358B