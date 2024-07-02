// Made with Amplify Shader Editor v1.9.2.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "FiberShaders/FToonTransparent"
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
		_SpecularGlossy("SpecularGlossy", Range( 0 , 100)) = 10
		_ToonizeSpecular("Toonize Specular", Range( 1 , 250)) = 1
		_SpecularCelIn("Specular Cel In", Range( 0.1 , 2)) = 1
		_SpecularCelOut("Specular Cel Out", Range( 0.1 , 2)) = 1
		[HDR]_RimColor("Rim Color", Color) = (0.4,0.4,0.4,0)
		_RimBias("Rim Bias", Range( 0 , 1)) = 0
		_ReflectFresnelBias("Reflect Fresnel Bias", Range( 0 , 1)) = 0
		_TransparentFresnelBias("Transparent Fresnel Bias", Range( 0 , 1)) = 0
		_RimScale("Rim Scale", Range( 0 , 5)) = 1
		_ReflectFresnelScale("Reflect Fresnel Scale", Range( 0 , 5)) = 1
		_TransparentFresnelScale("Transparent Fresnel Scale", Range( 0 , 5)) = 1
		[Toggle]_MultiplyRim("Multiply Rim", Float) = 0
		_ReflectFresnelPower("Reflect Fresnel Power", Range( 0 , 5)) = 1.5
		_TransparentFresnelPower("Transparent Fresnel Power", Range( 0 , 5)) = 1.5
		_RimPower("Rim Power", Range( 0 , 5)) = 1.5
		_ReflectMap1("Reflect Map", CUBE) = "white" {}
		[Toggle]_Toonize("Toonize", Float) = 0
		[Toggle]_ShadeFineTune("Shade Fine Tune", Float) = 0
		[Toggle]_SpecularToonize("SpecularToonize", Float) = 0
		[Toggle]_SpecularCelFinetune("Specular Cel Finetune", Float) = 0
		[Toggle]_RimLight("RimLight", Float) = 0
		_RimIntensity("Rim Intensity", Range( 0 , 1)) = 1
		_ReflectionStrength("Reflection Strength", Range( 0 , 1)) = 1
		[Toggle]_ShadeFold("ShadeFold", Float) = 0
		[HDR]_ReflectColor("Reflect Color", Color) = (1,1,1,1)
		[Toggle]_Reflect("Reflect", Float) = 0
		[Toggle]_ReflectionFresnel("ReflectionFresnel", Float) = 0
		[ToggleUI]_TransparentFresnelTog("TransparentFresnelTog", Float) = 0
		_CubeMapRotate("Cube Map Rotate", Range( 0 , 360)) = 0
		_CubemapYPosition("Cubemap Y Position", Range( -5 , 5)) = 0
		[Toggle]_RefFresnelInvert("RefFresnelInvert", Float) = 0
		[ToggleUI]_TransFresnelInvert("TransFresnelInvert", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Custom"  "Queue" = "Overlay+0" "IgnoreProjector" = "True" }
		Cull Back
		Blend SrcAlpha OneMinusSrcAlpha
		
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
			float2 uv_texcoord;
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
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

		uniform float4 _MainColor1;
		uniform sampler2D _MainTexture;
		uniform float4 _MainTexture_ST;
		uniform float _TransparentFresnelTog;
		uniform float _TransFresnelInvert;
		uniform float _Normal;
		uniform sampler2D _NormalMap;
		uniform float4 _NormalMap_ST;
		uniform float _NormalScale;
		uniform float _TransparentFresnelBias;
		uniform float _TransparentFresnelScale;
		uniform float _TransparentFresnelPower;
		uniform float _Specular;
		uniform float _SpecularToonize;
		uniform float _SpecularCelFinetune;
		uniform float _SpecularGlossy;
		uniform float _SpecularCelOut;
		uniform float _SpecularCelIn;
		uniform float _ToonizeSpecular;
		uniform float _SpecularAmbient;
		uniform float4 _SpecColor1;
		uniform float _SpecularIntensity;
		uniform float _MultiplyRim;
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
			float2 uv_MainTexture = i.uv_texcoord * _MainTexture_ST.xy + _MainTexture_ST.zw;
			float4 tex2DNode149 = tex2D( _MainTexture, uv_MainTexture );
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = Unity_SafeNormalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float2 temp_output_1_0_g64 = float2( 0,0 );
			float dotResult4_g64 = dot( temp_output_1_0_g64 , temp_output_1_0_g64 );
			float3 appendResult10_g64 = (float3((temp_output_1_0_g64).x , (temp_output_1_0_g64).y , sqrt( ( 1.0 - saturate( dotResult4_g64 ) ) )));
			float3 normalizeResult12_g64 = normalize( appendResult10_g64 );
			float2 uv_NormalMap = i.uv_texcoord * _NormalMap_ST.xy + _NormalMap_ST.zw;
			float3 NewNormals95 = normalize( (WorldNormalVector( i , (( _Normal )?( UnpackScaleNormal( tex2D( _NormalMap, uv_NormalMap ), _NormalScale ) ):( normalizeResult12_g64 )) )) );
			float fresnelNdotV247 = dot( normalize( NewNormals95 ), ase_worldViewDir );
			float fresnelNode247 = ( _TransparentFresnelBias + _TransparentFresnelScale * pow( max( 1.0 - fresnelNdotV247 , 0.0001 ), _TransparentFresnelPower ) );
			float temp_output_265_0 = saturate( fresnelNode247 );
			float TransparentFresnel250 = (( _TransparentFresnelTog )?( (( _TransFresnelInvert )?( ( 1.0 - temp_output_265_0 ) ):( temp_output_265_0 )) ):( 1.0 ));
			float4 temp_cast_0 = (0.0).xxxx;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = Unity_SafeNormalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float dotResult52 = dot( NewNormals95 , ase_worldlightDir );
			float NdotL56 = dotResult52;
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
			float4 temp_cast_9 = (1.0).xxxx;
			float4 clampResult120 = clamp( ( 1.0 - (( _SpecularToonize )?( posterize115 ):( (( _SpecularCelFinetune )?( (float4( 0,0,0,0 ) + (temp_output_109_0 - float4( 0,0,0,0 )) * (temp_cast_4 - float4( 0,0,0,0 )) / (temp_cast_3 - float4( 0,0,0,0 ))) ):( temp_output_109_0 )) )) ) , temp_cast_8 , temp_cast_9 );
			float4 Specular121 = ( clampResult120 * ( _SpecColor1 * _SpecularIntensity ) );
			float4 SpecularONOFF288 = (( _Specular )?( Specular121 ):( temp_cast_0 ));
			float4 temp_output_151_0 = ( _MainColor1.a * tex2DNode149.a * TransparentFresnel250 * ( 1.0 - ( SpecularONOFF288 * saturate( Specular121 ) ) ) );
			float4 MainAlpha152 = temp_output_151_0;
			float4 temp_cast_11 = (0.0).xxxx;
			float3 temp_output_975_0_g66 = float3( 0,0,1 );
			float3 newWorldNormal1072_g66 = normalize( (WorldNormalVector( i , temp_output_975_0_g66 )) );
			float3 normalizeResult1044_g66 = ASESafeNormalize( ( ase_worldViewDir + ase_worldlightDir ) );
			float dotResult1046_g66 = dot( newWorldNormal1072_g66 , normalizeResult1044_g66 );
			float Dot_NdotH1098_g66 = dotResult1046_g66;
			float dotResult1055_g66 = dot( newWorldNormal1072_g66 , ase_worldlightDir );
			float Dot_NdotL1100_g66 = dotResult1055_g66;
			float lerpResult1130_g66 = lerp( ( _ShadowStrength * _ShadowStrength * 0.7978846 ) , ( _ShadowStrength * _ShadowStrength * sqrt( ( 2.0 / ( 2.0 * UNITY_PI ) ) ) ) , _ShadowApproxmation);
			float RoughnessApproxmation1138_g66 = lerpResult1130_g66;
			float temp_output_1312_0_g66 = ( ( max( Dot_NdotL1100_g66 , 0.0 ) * RoughnessApproxmation1138_g66 ) + ( 1.0 - RoughnessApproxmation1138_g66 ) );
			float temp_output_1308_0_g66 = ( max( Dot_NdotH1098_g66 , 0.0 ) * temp_output_1312_0_g66 * temp_output_1312_0_g66 );
			float temp_output_1108_0_g66 = ( 1.0 - ( ( 1.0 - ase_lightAtten ) * _WorldSpaceLightPos0.w ) );
			float lerpResult1109_g66 = lerp( ( saturate( ( ( temp_output_1308_0_g66 + _ShadowOffset ) / _ShadowSharpness ) ) * ase_lightAtten ) , temp_output_1108_0_g66 , _ShadowStrength);
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			float4 ase_lightColor = 0;
			#else //aselc
			float4 ase_lightColor = _LightColor0;
			#endif //aselc
			float3 LightColor1348_g66 = ase_lightColor.rgb;
			float3 temp_output_1125_0_g66 = ( lerpResult1109_g66 * LightColor1348_g66 * ( ( 1.0 - _ShadowIntensity ) * 10.0 ) );
			float temp_output_60_0 = ( NdotL56 + _BaseCellOffset );
			float4 MainDiffuse61 = ( _MainColor1 * tex2DNode149 * temp_output_151_0 );
			float div101=256.0/float((int)(( _Toonize )?( _ToonizeVar ):( 1.0 )));
			float4 posterize101 = ( floor( CalculateContrast(_ShadingContrast,saturate( ( ( saturate( (( _ShadeFineTune )?( ( temp_output_60_0 / _BaseCellSharpness ) ):( NdotL56 )) ) * MainDiffuse61 ) + ( saturate( ( 1.0 - temp_output_60_0 ) ) * _ShadeColor * _ShadingContribution * MainDiffuse61 ) ) )) * div101 ) / div101 );
			float3 temp_output_898_0_g66 = ( float4( ase_lightColor.rgb , 0.0 ) * posterize101 ).rgb;
			float3 temp_output_1210_0_g66 = saturate( ( temp_output_1125_0_g66 * temp_output_898_0_g66 ) );
			float3 temp_output_280_0 = (temp_output_1210_0_g66*2.0 + 0.0);
			float3 BaseColor103 = (( _ShadeFold )?( temp_output_280_0 ):( temp_output_280_0 ));
			float fresnelNdotV76 = dot( normalize( NewNormals95 ), ase_worldViewDir );
			float fresnelNode76 = ( _RimBias + _RimScale * pow( max( 1.0 - fresnelNdotV76 , 0.0001 ), _RimPower ) );
			float4 RimSetUp81 = ( ( fresnelNode76 * _RimColor * _RimIntensity ) * (( _RimLight )?( 1.0 ):( 0.0 )) );
			float3 NewObjectNormal144 = (( _Normal )?( UnpackScaleNormal( tex2D( _NormalMap, uv_NormalMap ), _NormalScale ) ):( normalizeResult12_g64 ));
			half3 VertexPos40_g65 = normalize( WorldReflectionVector( i , NewObjectNormal144 ) );
			float3 appendResult74_g65 = (float3(0.0 , VertexPos40_g65.y , 0.0));
			float3 VertexPosRotationAxis50_g65 = appendResult74_g65;
			float3 break84_g65 = VertexPos40_g65;
			float3 appendResult81_g65 = (float3(break84_g65.x , 0.0 , break84_g65.z));
			float3 VertexPosOtherAxis82_g65 = appendResult81_g65;
			half Angle44_g65 = radians( _CubeMapRotate );
			float3 appendResult127 = (float3(0.0 , -_CubemapYPosition , 0.0));
			float fresnelNdotV234 = dot( normalize( NewNormals95 ), ase_worldViewDir );
			float fresnelNode234 = ( _ReflectFresnelBias + _ReflectFresnelScale * pow( max( 1.0 - fresnelNdotV234 , 0.0001 ), _ReflectFresnelPower ) );
			float ReflectFresnel242 = (( _ReflectionFresnel )?( (( _RefFresnelInvert )?( ( 1.0 - fresnelNode234 ) ):( fresnelNode234 )) ):( 1.0 ));
			float4 Reflect137 = (( _Reflect )?( ( _ReflectionStrength * ( texCUBE( _ReflectMap1, ( ( VertexPosRotationAxis50_g65 + ( VertexPosOtherAxis82_g65 * cos( Angle44_g65 ) ) + ( cross( float3(0,1,0) , VertexPosOtherAxis82_g65 ) * sin( Angle44_g65 ) ) ) + appendResult127 ) ) * _ReflectColor ) * ReflectFresnel242 ) ):( float4( 0,0,0,0 ) ));
			float4 temp_cast_16 = (0.0).xxxx;
			float RimSwitch196 = (( _RimLight )?( 1.0 ):( 0.0 ));
			float Fresnel80 = fresnelNode76;
			float4 lerpResult160 = lerp( ( (( _Specular )?( Specular121 ):( temp_cast_16 )) + float4( BaseColor103 , 0.0 ) + Reflect137 ) , RimSetUp81 , ( RimSwitch196 * Fresnel80 ));
			c.rgb = (( _MultiplyRim )?( lerpResult160 ):( ( (( _Specular )?( Specular121 ):( temp_cast_11 )) + float4( BaseColor103 , 0.0 ) + RimSetUp81 + Reflect137 ) )).rgb;
			c.a = MainAlpha152.r;
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
		#pragma surface surf StandardCustomLighting keepalpha fullforwardshadows 

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
			sampler3D _DitherMaskLOD;
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
				UnityGI gi;
				UNITY_INITIALIZE_OUTPUT( UnityGI, gi );
				o.Alpha = LightingStandardCustomLighting( o, worldViewDir, gi ).a;
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "FToonTransparentEditor"
}
/*ASEBEGIN
Version=19202
Node;AmplifyShaderEditor.CommentaryNode;256;-8139.933,-5206.637;Inherit;False;1913.089;629.3394;Transparent Fresnel;11;248;246;255;254;251;252;250;253;249;247;265;Transparent Fresnel;0.5283019,0.3036852,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;241;-7771.92,-3664.319;Inherit;False;1564.325;619.855;Reflect Fresnel;10;242;235;240;237;231;232;233;230;234;229;Reflect Fresnel;0,0.7859545,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;43;-9722.324,-2820.604;Inherit;False;1121.2;207.2;Camera Mode;5;124;123;122;94;93;Camera Mode;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;44;-9219.381,-3615.612;Inherit;False;1362.807;553.545;Comment;7;145;144;95;48;47;46;45;Normals;0.4402515,0.4635113,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;49;-9199.37,-4245.357;Inherit;False;801.7261;511.2055;Normal dot Light;4;56;52;51;50;Normal dot Light;0.7171531,0.5226415,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;53;-6999.074,-4566.985;Inherit;False;1063.376;883.5909;Main Texture;12;285;284;151;61;150;152;149;258;147;286;289;290;Main Texture;0.4566038,0.9526708,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;54;-9474.565,-87.62061;Inherit;False;3228.367;540.4689;Specular Setup;22;114;220;106;105;110;112;111;109;108;107;113;115;118;116;223;143;117;221;120;121;119;224;Specular Setup;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;55;-9788.542,-1428.949;Inherit;False;4297.195;1242.748;Base Color;28;103;259;102;261;199;57;203;62;59;58;60;68;96;148;101;98;146;65;200;99;66;63;64;67;100;97;69;280;Base Color;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleAddOpNode;69;-7993.773,-948.9503;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;70;-7727.406,-2136.981;Inherit;False;1512.866;597.899;Rim Light;14;142;81;80;77;76;75;74;73;72;71;191;196;225;228;Rim Light;0.4422169,1,0.3915094,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;82;-9799.635,-2200.639;Inherit;False;1409.389;612.5826;Indirect Diffuse;6;92;90;89;88;87;84;Indirect Diffuse;0.9610584,0.681,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;83;-7869.173,-4298.576;Inherit;False;814.6841;469.7439;Light Affect;4;104;91;85;260;Light Affect;0.9947262,1,0.6176471,1;0;0
Node;AmplifyShaderEditor.LerpOp;87;-8950.423,-1974.601;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;90;-8613.914,-1975.217;Inherit;True;IndirectDiffuse;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;93;-9224.324,-2770.605;Half;False;Constant;_Float7;Float 7;47;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;97;-7721.774,-932.9506;Inherit;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;122;-9032.324,-2770.605;Inherit;False;3;0;FLOAT;1;False;1;FLOAT;0.5;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;123;-9410.324,-2773.605;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OrthoParams;124;-9672.324,-2770.605;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;125;-8518.868,-2954.874;Inherit;False;2314.286;717.8335;Reflection Map;17;133;139;245;141;134;137;243;140;136;138;128;132;127;130;131;129;135;Reflection Map;0,0.4211543,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;84;-9326.468,-1863.76;Float;True;Constant;_IndirectDiffuseContribution;Indirect Diffuse Contribution;20;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;88;-9769.222,-1954.756;Inherit;True;95;NewNormals;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FunctionNode;89;-9547.471,-1956.975;Inherit;True;Dielectric Specular;14;;48;cf90616a2350c5c4cba1069366c98fa1;1,1,0;2;2;FLOAT;0;False;9;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;92;-9334.408,-2152.42;Float;True;Constant;_Float8;Float 8;20;0;Create;True;0;0;0;False;0;False;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LightColorNode;100;-7161.776,-1108.95;Inherit;True;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;77;-7051.134,-2051.936;Inherit;True;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;191;-6754.251,-2056.325;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;80;-6468.572,-1832.333;Inherit;True;Fresnel;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;67;-8457.784,-1364.95;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;64;-8489.784,-1140.951;Inherit;True;61;MainDiffuse;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;63;-8665.785,-932.9506;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;66;-8457.784,-932.9506;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;65;-8553.784,-692.9512;Float;True;Property;_ShadingContribution;Shading Contribution;22;0;Create;True;0;0;0;False;0;False;0.7;1;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;146;-8505.784,-452.9512;Inherit;False;Property;_ShadeColor;Shade Color;17;0;Create;True;1;Shade Settings;0;0;False;0;False;0.35,0.35,0.35,1;0.35,0.35,0.35,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleContrastOpNode;98;-7417.775,-932.9506;Inherit;True;2;1;COLOR;0,0,0,0;False;0;FLOAT;-0.66;False;1;COLOR;0
Node;AmplifyShaderEditor.PosterizeNode;101;-7145.776,-868.9512;Inherit;True;1;2;1;COLOR;0,0,0,0;False;0;INT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;96;-8233.772,-756.9514;Inherit;True;4;4;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;68;-8217.772,-1220.951;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;60;-9249.374,-1280.846;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;58;-9641.748,-1136.184;Float;True;Property;_BaseCellOffset;Base Cell Offset;21;0;Create;True;0;0;0;False;0;False;0;0;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;59;-9314.414,-1049.057;Float;True;Property;_BaseCellSharpness;Base Cell Sharpness;20;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;62;-8964.834,-1281.339;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0.01;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;203;-8702.312,-1368.327;Inherit;True;Property;_ShadeFineTune;Shade Fine Tune;51;0;Create;True;0;0;0;False;0;False;0;True;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;57;-9579.676,-1371.12;Inherit;True;56;NdotL;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;52;-8880.309,-4091.037;Inherit;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;56;-8644.757,-4091.935;Float;True;NdotL;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;50;-9140.12,-4184.526;Inherit;True;95;NewNormals;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;51;-9147.421,-3961.271;Inherit;True;True;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;119;-6679.624,61.52948;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;121;-6447.596,58.99841;Inherit;True;Specular;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;120;-7127.276,-45.11265;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0.347,0.347,0.347,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;221;-6863.45,172.6972;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;117;-7168.196,353.264;Inherit;False;Property;_SpecularIntensity;Specular Intensity;27;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;143;-7108.694,178.9386;Inherit;False;Property;_SpecColor1;Specular Color;26;1;[HDR];Create;False;1;Specular Settings;0;0;False;0;False;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;116;-7355.684,-45.32066;Inherit;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;118;-7449.934,252.7598;Inherit;False;Property;_SpecularAmbient;Specular Ambient;29;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;220;-7452.354,171.3971;Inherit;False;Constant;_Float0;Float 0;34;0;Create;True;0;0;0;False;0;False;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;114;-8167.294,329.3868;Inherit;False;Property;_ToonizeSpecular;Toonize Specular;31;0;Create;True;0;0;0;False;0;False;1;1;1;250;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;107;-9196.306,-26.62955;Inherit;False;Blinn-Phong Light;45;;62;cf814dba44d007a4e958d2ddd5813da6;0;3;42;COLOR;0,0,0,0;False;52;FLOAT3;0,0,0;False;43;COLOR;0,0,0,0;False;2;COLOR;0;FLOAT;57
Node;AmplifyShaderEditor.OneMinusNode;108;-8973.806,-29.19755;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleContrastOpNode;109;-8788.806,-35.73246;Inherit;True;2;1;COLOR;0,0,0,0;False;0;FLOAT;2.86;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;110;-9118.625,110.3474;Inherit;False;Property;_SpecularGlossy;SpecularGlossy;30;0;Create;True;0;0;0;False;0;False;10;3.9;0;100;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;105;-9446.135,-27.00825;Inherit;True;56;NdotL;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;106;-9443.535,177.3529;Inherit;True;95;NewNormals;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TFHCRemapNode;113;-8474.596,43.15189;Inherit;True;5;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0.092,0.092,0.092,1;False;3;COLOR;0,0,0,0;False;4;COLOR;0.795,0.795,0.795,1;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;111;-8794.146,225.0366;Inherit;False;Property;_SpecularCelOut;Specular Cel Out;33;0;Create;True;0;0;0;False;0;False;1;0.9829412;0.1;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;112;-8790.726,327.2356;Inherit;False;Property;_SpecularCelIn;Specular Cel In;32;0;Create;True;0;0;0;False;0;False;1;1.332;0.1;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.PosterizeNode;115;-7860.833,149.8801;Inherit;True;52;2;1;COLOR;0,0,0,0;False;0;INT;52;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;81;-6467.748,-2060.615;Inherit;True;RimSetUp;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;196;-6748.575,-1800.254;Inherit;True;RimSwitch;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;228;-7366.993,-1637.035;Inherit;False;Property;_RimIntensity;Rim Intensity;55;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;142;-7302.173,-1819.215;Inherit;False;Property;_RimColor;Rim Color;34;1;[HDR];Create;True;1;Rim Light Settings;0;0;False;0;False;0.4,0.4,0.4,0;0.4,0.4,0.4,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;145;-8864.371,-3551.612;Inherit;True;Property;_NormalMap;Normal Map;23;1;[Normal];Create;True;1;Normal Settings;0;0;False;0;False;-1;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;45;-9168.381,-3551.612;Float;True;Property;_NormalScale;Normal Scale;25;0;Create;True;0;0;0;False;0;False;0.3;0.3;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;46;-8827.671,-3313.993;Inherit;True;Normal Reconstruct Z;-1;;64;63ba85b764ae0c84ab3d698b86364ae9;0;1;1;FLOAT2;0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;144;-8077.583,-3297.809;Inherit;True;NewObjectNormal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WorldNormalVector;48;-8312.643,-3551.681;Inherit;True;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RegisterLocalVarNode;95;-8060.756,-3548.804;Float;True;NewNormals;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;75;-7683.873,-2092.669;Inherit;False;95;NewNormals;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;71;-7682.882,-2011.261;Inherit;True;World;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;72;-7689.499,-1802.68;Inherit;False;Property;_RimBias;Rim Bias;35;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;73;-7689.499,-1722.68;Inherit;False;Property;_RimScale;Rim Scale;38;0;Create;True;0;0;0;False;0;False;1;1;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;74;-7689.499,-1630.68;Inherit;False;Property;_RimPower;Rim Power;44;0;Create;True;0;0;0;False;0;False;1.5;1.5;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.FresnelNode;76;-7389.406,-2053.681;Inherit;True;Standard;WorldNormal;ViewDir;True;True;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;135;-7124.128,-2577.909;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;129;-8005.878,-2661.322;Inherit;True;Compute Rotation Y;-1;;65;693b7d13a80c93a4e8b791a9cd5e5ab2;0;2;38;FLOAT3;0,0,0;False;43;FLOAT;0;False;1;FLOAT3;19
Node;AmplifyShaderEditor.RadiansOpNode;131;-8165.877,-2613.322;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldReflectionVector;130;-8261.878,-2837.322;Inherit;True;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DynamicAppendNode;127;-7857.623,-2374.512;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.NegateNode;132;-8033.619,-2358.512;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;128;-7703.079,-2656.122;Inherit;True;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;138;-8493.292,-2837.213;Inherit;True;144;NewObjectNormal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;136;-6856.733,-2579.06;Inherit;True;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;140;-7188.128,-2689.909;Inherit;False;Property;_ReflectionStrength;Reflection Strength;56;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;243;-7127.261,-2902.038;Inherit;True;242;ReflectFresnel;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;137;-6417.583,-2583.397;Inherit;True;Reflect;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;134;-7380.128,-2449.909;Inherit;False;Property;_ReflectColor;Reflect Color;58;1;[HDR];Create;True;0;0;0;False;0;False;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;141;-7476.128,-2657.909;Inherit;True;Property;_ReflectMap1;Reflect Map;49;0;Create;True;1;Reflection Settings;0;0;False;0;False;-1;None;None;True;0;False;white;LockedToCube;False;Object;-1;Auto;Cube;8;0;SAMPLERCUBE;;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;139;-8453.879,-2613.322;Inherit;False;Property;_CubeMapRotate;Cube Map Rotate;62;0;Create;True;0;0;0;False;0;False;0;0;0;360;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;133;-8321.618,-2358.512;Inherit;False;Property;_CubemapYPosition;Cubemap Y Position;63;0;Create;True;0;0;0;False;0;False;0;0;-5;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;229;-7716.295,-3614.319;Inherit;False;95;NewNormals;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FresnelNode;234;-7421.831,-3575.331;Inherit;True;Standard;WorldNormal;ViewDir;True;True;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;230;-7715.303,-3532.911;Inherit;True;World;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.OneMinusNode;237;-7096.549,-3498.05;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;242;-6450.633,-3575.82;Inherit;True;ReflectFresnel;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;231;-7721.921,-3324.33;Inherit;False;Property;_ReflectFresnelBias;Reflect Fresnel Bias;36;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;232;-7721.921,-3240.085;Inherit;False;Property;_ReflectFresnelScale;Reflect Fresnel Scale;39;0;Create;True;0;0;0;False;0;False;1;1;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;233;-7721.921,-3152.33;Inherit;False;Property;_ReflectFresnelPower;Reflect Fresnel Power;42;0;Create;True;0;0;0;False;0;False;1.5;1.5;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;223;-7597.514,-35.89375;Inherit;False;Property;_SpecularToonize;SpecularToonize;52;0;Create;True;0;0;0;False;0;False;0;True;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ToggleSwitchNode;224;-8178.694,-37.20554;Inherit;False;Property;_SpecularCelFinetune;Specular Cel Finetune;53;0;Create;True;0;0;0;False;0;False;0;True;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;249;-7218.862,-5035.298;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;250;-6506.439,-5111.248;Inherit;True;TransparentFresnel;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;154;-5118.381,-4200.841;Inherit;True;121;Specular;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;156;-5118.381,-4008.841;Inherit;True;103;BaseColor;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;158;-5118.381,-3800.841;Inherit;True;81;RimSetUp;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;197;-4878.381,-3576.841;Inherit;True;196;RimSwitch;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;195;-4638.381,-3576.841;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;159;-4638.381,-3800.841;Inherit;True;3;3;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;160;-4382.38,-3800.841;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;161;-4350.38,-4200.841;Inherit;True;4;4;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;157;-4878.381,-3368.841;Inherit;True;80;Fresnel;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;162;-5128.193,-4487.347;Inherit;True;137;Reflect;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;219;-5087.173,-4287.398;Inherit;False;Constant;_NonSpecular;NonSpecular;34;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;251;-7028.683,-5114.998;Inherit;True;Property;_TransFresnelInvert;TransFresnelInvert;65;0;Create;True;0;0;0;False;0;False;0;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;235;-6681.364,-3578.142;Inherit;False;Property;_ReflectionFresnel;ReflectionFresnel;60;0;Create;True;0;0;0;False;0;False;0;True;2;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;240;-6911.874,-3577.75;Inherit;False;Property;_RefFresnelInvert;RefFresnelInvert;64;0;Create;True;0;0;0;False;0;False;0;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;47;-8550.961,-3552.124;Inherit;True;Property;_Normal;Normal;24;0;Create;True;0;0;0;False;0;False;0;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ToggleSwitchNode;245;-6635.898,-2578.996;Inherit;False;Property;_Reflect;Reflect;59;0;Create;True;0;0;0;False;0;False;0;True;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ToggleSwitchNode;225;-7036.654,-1821.635;Inherit;False;Property;_RimLight;RimLight;54;0;Create;True;0;0;0;False;0;False;0;True;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;199;-7373.248,-596.1821;Inherit;False;Property;_Toonize;Toonize;50;0;Create;True;0;0;0;False;0;False;0;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;153;-4078.38,-3976.841;Inherit;True;Property;_MultiplyRim;Multiply Rim;41;0;Create;True;0;0;0;False;0;False;0;True;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LightAttenuation;260;-7808.611,-4248.358;Inherit;True;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;91;-7773.173,-4042.577;Inherit;True;56;NdotL;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;85;-7533.173,-4122.576;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;104;-7304.175,-4125.576;Float;True;LightColor_NdotL;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;94;-8833.935,-2765.805;Float;False;CAMERA_MODE;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;253;-8096.476,-4859.544;Inherit;False;Property;_TransparentFresnelBias;Transparent Fresnel Bias;37;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;254;-8096.476,-4775.299;Inherit;False;Property;_TransparentFresnelScale;Transparent Fresnel Scale;40;0;Create;True;0;0;0;False;0;False;1;1;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;255;-8096.476,-4687.544;Inherit;False;Property;_TransparentFresnelPower;Transparent Fresnel Power;43;0;Create;True;0;0;0;False;0;False;1.5;1.5;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;246;-8090.85,-5149.533;Inherit;False;95;NewNormals;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;248;-8089.86,-5068.125;Inherit;True;World;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.FresnelNode;247;-7769.944,-5104.442;Inherit;True;Standard;WorldNormal;ViewDir;True;True;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;265;-7440.882,-5094.874;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-3781.314,-3876.666;Float;False;True;-1;2;FToonTransparentEditor;0;0;CustomLighting;FiberShaders/FToonTransparent;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;_TransparentFresnelPower;0;False;;False;0;False;;0;False;;False;0;Custom;0.5;True;True;0;True;Custom;;Overlay;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;2;5;False;_TransparentFresnelPower;10;False;;0;5;False;;10;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;11;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;_TransparentFresnelScale;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.ColorNode;147;-6910.4,-4505.586;Float;False;Property;_MainColor1;Main Color;12;0;Create;True;1;Diffuse Settings;0;0;False;0;False;0.6,0.6,0.6,1;0.7830189,0.7830189,0.7830189,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;149;-6973.373,-4311.962;Inherit;True;Property;_MainTexture;Main Texture;13;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;152;-6157.257,-4269.561;Inherit;True;MainAlpha;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;150;-6384.938,-4515.276;Inherit;True;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;61;-6153.295,-4512.644;Inherit;True;MainDiffuse;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;261;-6646.178,-988.3357;Inherit;False;Material Lighting;0;;66;b5db59747a6f5584db32cde93c4695e4;8,1290,0,1289,0,1288,0,1074,0,1323,0,1322,0,1128,0,1280,0;8;898;FLOAT3;0,0,0;False;975;FLOAT3;0,0,1;False;1081;FLOAT;0;False;1079;FLOAT;0;False;1077;FLOAT;0;False;1075;FLOAT;0;False;1246;FLOAT2;0,0;False;1250;FLOAT2;0,0;False;1;FLOAT3;894
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;102;-6873.776,-980.9503;Inherit;True;2;2;0;FLOAT3;0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ToggleSwitchNode;259;-6141.4,-984.8851;Inherit;False;Property;_ShadeFold;ShadeFold;57;0;Create;True;0;0;0;False;0;False;0;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;103;-5920.261,-984.5764;Float;True;BaseColor;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;151;-6618.208,-4277.919;Inherit;True;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;200;-7552.248,-632.1823;Inherit;False;Constant;_NotToonize;NotToonize;33;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;99;-7649.775,-553.9507;Inherit;False;Property;_ToonizeVar;ToonizeVar;18;0;Create;True;0;0;0;False;0;False;1;1;1;200;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;257;-4198.524,-3357.911;Inherit;True;152;MainAlpha;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;148;-7833.773,-708.9511;Inherit;False;Property;_ShadingContrast;Shading Contrast;19;0;Create;True;0;0;0;False;0;False;1.5;1;0.1;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleAndOffsetNode;280;-6383.237,-986.7791;Inherit;True;3;0;FLOAT3;0,0,0;False;1;FLOAT;2;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;287;-4064.38,-3607.152;Inherit;False;103;BaseColor;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ToggleSwitchNode;155;-4894.381,-4203.295;Inherit;False;Property;_Specular;Specular;28;0;Create;True;0;0;0;False;0;False;0;True;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;288;-4627.488,-4334.982;Inherit;False;SpecularONOFF;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;258;-6970.467,-4102.174;Inherit;True;250;TransparentFresnel;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;284;-6952.857,-3875.496;Inherit;True;121;Specular;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;285;-6747.203,-3871.768;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;289;-6719.05,-4009.642;Inherit;False;288;SpecularONOFF;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;286;-6436.695,-3933.506;Inherit;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;290;-6588.514,-3887.41;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ToggleSwitchNode;252;-6784.407,-5115.39;Inherit;True;Property;_TransparentFresnelTog;TransparentFresnelTog;61;0;Create;True;0;0;0;False;0;False;0;False;2;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
WireConnection;69;0;68;0
WireConnection;69;1;96;0
WireConnection;87;0;92;0
WireConnection;87;1;89;0
WireConnection;87;2;84;0
WireConnection;90;0;87;0
WireConnection;97;0;69;0
WireConnection;122;0;93;0
WireConnection;122;1;123;0
WireConnection;122;2;124;4
WireConnection;123;0;124;2
WireConnection;123;1;124;1
WireConnection;89;2;88;0
WireConnection;77;0;76;0
WireConnection;77;1;142;0
WireConnection;77;2;228;0
WireConnection;191;0;77;0
WireConnection;191;1;225;0
WireConnection;80;0;76;0
WireConnection;67;0;203;0
WireConnection;63;0;60;0
WireConnection;66;0;63;0
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
WireConnection;60;0;57;0
WireConnection;60;1;58;0
WireConnection;62;0;60;0
WireConnection;62;1;59;0
WireConnection;203;0;57;0
WireConnection;203;1;62;0
WireConnection;52;0;50;0
WireConnection;52;1;51;0
WireConnection;56;0;52;0
WireConnection;119;0;120;0
WireConnection;119;1;221;0
WireConnection;121;0;119;0
WireConnection;120;0;116;0
WireConnection;120;1;118;0
WireConnection;120;2;220;0
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
WireConnection;144;0;47;0
WireConnection;48;0;47;0
WireConnection;95;0;48;0
WireConnection;76;0;75;0
WireConnection;76;4;71;0
WireConnection;76;1;72;0
WireConnection;76;2;73;0
WireConnection;76;3;74;0
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
WireConnection;136;0;140;0
WireConnection;136;1;135;0
WireConnection;136;2;243;0
WireConnection;137;0;245;0
WireConnection;141;1;128;0
WireConnection;234;0;229;0
WireConnection;234;4;230;0
WireConnection;234;1;231;0
WireConnection;234;2;232;0
WireConnection;234;3;233;0
WireConnection;237;0;234;0
WireConnection;242;0;235;0
WireConnection;223;0;224;0
WireConnection;223;1;115;0
WireConnection;224;0;109;0
WireConnection;224;1;113;0
WireConnection;249;0;265;0
WireConnection;250;0;252;0
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
WireConnection;251;0;265;0
WireConnection;251;1;249;0
WireConnection;235;1;240;0
WireConnection;240;0;234;0
WireConnection;240;1;237;0
WireConnection;47;0;46;0
WireConnection;47;1;145;0
WireConnection;245;1;136;0
WireConnection;199;0;200;0
WireConnection;199;1;99;0
WireConnection;153;0;161;0
WireConnection;153;1;160;0
WireConnection;85;0;260;0
WireConnection;85;1;91;0
WireConnection;104;0;85;0
WireConnection;94;0;122;0
WireConnection;247;0;246;0
WireConnection;247;4;248;0
WireConnection;247;1;253;0
WireConnection;247;2;254;0
WireConnection;247;3;255;0
WireConnection;265;0;247;0
WireConnection;0;9;257;0
WireConnection;0;13;153;0
WireConnection;152;0;151;0
WireConnection;150;0;147;0
WireConnection;150;1;149;0
WireConnection;150;2;151;0
WireConnection;61;0;150;0
WireConnection;261;898;102;0
WireConnection;102;0;100;1
WireConnection;102;1;101;0
WireConnection;259;0;280;0
WireConnection;259;1;280;0
WireConnection;103;0;259;0
WireConnection;151;0;147;4
WireConnection;151;1;149;4
WireConnection;151;2;258;0
WireConnection;151;3;286;0
WireConnection;280;0;261;894
WireConnection;155;0;219;0
WireConnection;155;1;154;0
WireConnection;288;0;155;0
WireConnection;285;0;284;0
WireConnection;286;0;290;0
WireConnection;290;0;289;0
WireConnection;290;1;285;0
WireConnection;252;1;251;0
ASEEND*/
//CHKSM=E01C114824CE1C30F17935F2B04BBFEE9DA2D2AB