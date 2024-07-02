// Made with Amplify Shader Editor v1.9.1.8
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "FiberShaders/FSurface"
{
	Properties
	{
		_MainColor1("Main Color", Color) = (0.6,0.6,0.6,1)
		_MainTexture("Main Texture", 2D) = "white" {}
		[Toggle]_AmbientOcclusion("AmbientOcclusion", Float) = 0
		_AmbientOcclusionColor("Ambient Occlusion Color", Color) = (0.5,0.5,0.5,0)
		_AOIntensity("AO Intensity", Range( 0 , 1)) = 1
		_AmbientOcclusionTexture("Ambient Occlusion Texture", 2D) = "white" {}
		[Toggle]_Normal("Normal", Float) = 0
		[Normal]_NormalMap("Normal Map", 2D) = "bump" {}
		_NormalScale("Normal Scale", Range( -1 , 1)) = 0.3
		[Toggle]_RimLight("RimLight", Float) = 0
		_RimIntensity("Rim Intensity", Range( 0 , 1)) = 1
		[HDR]_RimColor("Rim Color", Color) = (0.4,0.4,0.4,0)
		_RimBias("Rim Bias", Range( 0 , 1)) = 0
		_RimScale("Rim Scale", Range( 0 , 5)) = 1
		_RimPower("Rim Power", Range( 0 , 5)) = 1.5
		_Metallic("Metallic", Range( 0 , 2)) = 0
		_Smoothness("Smoothness", Range( 0 , 2)) = 0
		_MetallicTexture("Metallic Texture", 2D) = "white" {}
		[Toggle]_Reflect("Reflect", Float) = 0
		_ReflectionStrength("Reflection Strength", Range( 0 , 1)) = 1
		[HDR]_ReflectColor("Reflect Color", Color) = (1,1,1,1)
		_ReflectMap1("Reflect Map", CUBE) = "white" {}
		_CubeMapRotate("Cube Map Rotate", Range( 0 , 360)) = 0
		_CubemapYPosition("Cubemap Y Position", Range( -5 , 5)) = 0
		[Toggle]_ReflectionFresnel("ReflectionFresnel", Float) = 0
		[Toggle]_RefFresnelInvert("RefFresnelInvert", Float) = 0
		_ReflectFresnelBias("Reflect Fresnel Bias", Range( 0 , 1)) = 0
		_ReflectFresnelScale("Reflect Fresnel Scale", Range( 0 , 5)) = 1
		_ReflectFresnelPower("Reflect Fresnel Power", Range( 0 , 5)) = 1.5
		[Toggle]_Emission("Emission", Float) = 0
		_EmissionIntensity("Emission Intensity", Range( 0 , 1)) = 0.5
		_EmissionTexture("Emission Texture", 2D) = "white" {}
		[Header(Translucency)]
		_Translucency("Strength", Range( 0 , 50)) = 1
		_TransNormalDistortion("Normal Distortion", Range( 0 , 1)) = 0.1
		_TransScattering("Scaterring Falloff", Range( 1 , 50)) = 2
		_TransDirect("Direct", Range( 0 , 1)) = 1
		_TransAmbient("Ambient", Range( 0 , 1)) = 0.2
		_TransShadow("Shadow", Range( 0 , 1)) = 0.9
		[Toggle]_TransmissionOnOff("TransmissionOnOff", Float) = 0
		[HDR]_EmissionColor("Emission Color", Color) = (0,0,0,0)
		[Toggle]_Properties("Properties", Float) = 0
		[Toggle]_TranslucencyOnOff("TranslucencyOnOff", Float) = 0
		[HDR]_TransmissionColor("Transmission Color", Color) = (0,0,0,0)
		[HDR]_TranslucencyColor("Translucency Color", Color) = (0,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityStandardUtils.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
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

		struct SurfaceOutputStandardCustom
		{
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			half Alpha;
			half3 Transmission;
			half3 Translucency;
		};

		uniform float _Normal;
		uniform sampler2D _NormalMap;
		uniform float4 _NormalMap_ST;
		uniform float _NormalScale;
		uniform float4 _MainColor1;
		uniform sampler2D _MainTexture;
		uniform float4 _MainTexture_ST;
		uniform sampler2D _MetallicTexture;
		uniform float4 _MetallicTexture_ST;
		uniform float _RimBias;
		uniform float _RimScale;
		uniform float _RimPower;
		uniform float4 _RimColor;
		uniform float _RimIntensity;
		uniform float _RimLight;
		uniform float _ReflectionFresnel;
		uniform float _RefFresnelInvert;
		uniform float _ReflectFresnelBias;
		uniform float _ReflectFresnelScale;
		uniform float _ReflectFresnelPower;
		uniform float _Reflect;
		uniform float _ReflectionStrength;
		uniform samplerCUBE _ReflectMap1;
		uniform float _CubeMapRotate;
		uniform float _CubemapYPosition;
		uniform float4 _ReflectColor;
		uniform float _Emission;
		uniform float _EmissionIntensity;
		uniform float4 _EmissionColor;
		uniform sampler2D _EmissionTexture;
		uniform float4 _EmissionTexture_ST;
		uniform float _Metallic;
		uniform float _Properties;
		uniform float _Smoothness;
		uniform float _AmbientOcclusion;
		uniform sampler2D _AmbientOcclusionTexture;
		uniform float4 _AmbientOcclusionTexture_ST;
		uniform float _AOIntensity;
		uniform float4 _AmbientOcclusionColor;
		uniform float _TransmissionOnOff;
		uniform float4 _TransmissionColor;
		uniform half _Translucency;
		uniform half _TransNormalDistortion;
		uniform half _TransScattering;
		uniform half _TransDirect;
		uniform half _TransAmbient;
		uniform half _TransShadow;
		uniform float _TranslucencyOnOff;
		uniform float4 _TranslucencyColor;

		inline half4 LightingStandardCustom(SurfaceOutputStandardCustom s, half3 viewDir, UnityGI gi )
		{
			#if !defined(DIRECTIONAL)
			float3 lightAtten = gi.light.color;
			#else
			float3 lightAtten = lerp( _LightColor0.rgb, gi.light.color, _TransShadow );
			#endif
			half3 lightDir = gi.light.dir + s.Normal * _TransNormalDistortion;
			half transVdotL = pow( saturate( dot( viewDir, -lightDir ) ), _TransScattering );
			half3 translucency = lightAtten * (transVdotL * _TransDirect + gi.indirect.diffuse * _TransAmbient) * s.Translucency;
			half4 c = half4( s.Albedo * translucency * _Translucency, 0 );

			half3 transmission = max(0 , -dot(s.Normal, gi.light.dir)) * gi.light.color * s.Transmission;
			half4 d = half4(s.Albedo * transmission , 0);

			SurfaceOutputStandard r;
			r.Albedo = s.Albedo;
			r.Normal = s.Normal;
			r.Emission = s.Emission;
			r.Metallic = s.Metallic;
			r.Smoothness = s.Smoothness;
			r.Occlusion = s.Occlusion;
			r.Alpha = s.Alpha;
			return LightingStandard (r, viewDir, gi) + c + d;
		}

		inline void LightingStandardCustom_GI(SurfaceOutputStandardCustom s, UnityGIInput data, inout UnityGI gi )
		{
			#if defined(UNITY_PASS_DEFERRED) && UNITY_ENABLE_REFLECTION_BUFFERS
				gi = UnityGlobalIllumination(data, s.Occlusion, s.Normal);
			#else
				UNITY_GLOSSY_ENV_FROM_SURFACE( g, s, data );
				gi = UnityGlobalIllumination( data, s.Occlusion, s.Normal, g );
			#endif
		}

		void surf( Input i , inout SurfaceOutputStandardCustom o )
		{
			float2 temp_output_1_0_g66 = float2( 0,0 );
			float dotResult4_g66 = dot( temp_output_1_0_g66 , temp_output_1_0_g66 );
			float3 appendResult10_g66 = (float3((temp_output_1_0_g66).x , (temp_output_1_0_g66).y , sqrt( ( 1.0 - saturate( dotResult4_g66 ) ) )));
			float3 normalizeResult12_g66 = normalize( appendResult10_g66 );
			float2 uv_NormalMap = i.uv_texcoord * _NormalMap_ST.xy + _NormalMap_ST.zw;
			float3 NewObjectNormal15 = (( _Normal )?( UnpackScaleNormal( tex2D( _NormalMap, uv_NormalMap ), _NormalScale ) ):( normalizeResult12_g66 ));
			o.Normal = NewObjectNormal15;
			float2 uv_MainTexture = i.uv_texcoord * _MainTexture_ST.xy + _MainTexture_ST.zw;
			float4 tex2DNode2 = tex2D( _MainTexture, uv_MainTexture );
			float4 MainDiffuse6 = ( _MainColor1 * tex2DNode2 );
			o.Albedo = MainDiffuse6.rgb;
			float2 uv_MetallicTexture = i.uv_texcoord * _MetallicTexture_ST.xy + _MetallicTexture_ST.zw;
			float4 tex2DNode77 = tex2D( _MetallicTexture, uv_MetallicTexture );
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = Unity_SafeNormalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 NewNormals17 = normalize( (WorldNormalVector( i , (( _Normal )?( UnpackScaleNormal( tex2D( _NormalMap, uv_NormalMap ), _NormalScale ) ):( normalizeResult12_g66 )) )) );
			float fresnelNdotV32 = dot( normalize( NewNormals17 ), ase_worldViewDir );
			float fresnelNode32 = ( _RimBias + _RimScale * pow( max( 1.0 - fresnelNdotV32 , 0.0001 ), _RimPower ) );
			float4 RimSetUp23 = ( ( fresnelNode32 * _RimColor * _RimIntensity ) * (( _RimLight )?( 1.0 ):( 0.0 )) );
			float fresnelNdotV36 = dot( normalize( NewNormals17 ), ase_worldViewDir );
			float fresnelNode36 = ( _ReflectFresnelBias + _ReflectFresnelScale * pow( max( 1.0 - fresnelNdotV36 , 0.0001 ), _ReflectFresnelPower ) );
			float ReflectFresnel44 = (( _ReflectionFresnel )?( (( _RefFresnelInvert )?( ( 1.0 - fresnelNode36 ) ):( fresnelNode36 )) ):( 1.0 ));
			half3 VertexPos40_g65 = normalize( WorldReflectionVector( i , NewObjectNormal15 ) );
			float3 appendResult74_g65 = (float3(0.0 , VertexPos40_g65.y , 0.0));
			float3 VertexPosRotationAxis50_g65 = appendResult74_g65;
			float3 break84_g65 = VertexPos40_g65;
			float3 appendResult81_g65 = (float3(break84_g65.x , 0.0 , break84_g65.z));
			float3 VertexPosOtherAxis82_g65 = appendResult81_g65;
			half Angle44_g65 = radians( _CubeMapRotate );
			float3 appendResult52 = (float3(0.0 , -_CubemapYPosition , 0.0));
			float4 Reflect59 = (( _Reflect )?( ( _ReflectionStrength * ( texCUBE( _ReflectMap1, ( ( VertexPosRotationAxis50_g65 + ( VertexPosOtherAxis82_g65 * cos( Angle44_g65 ) ) + ( cross( float3(0,1,0) , VertexPosOtherAxis82_g65 ) * sin( Angle44_g65 ) ) ) + appendResult52 ) ) * _ReflectColor ) * ReflectFresnel44 ) ):( float4( 0,0,0,0 ) ));
			float2 uv_EmissionTexture = i.uv_texcoord * _EmissionTexture_ST.xy + _EmissionTexture_ST.zw;
			o.Emission = ( ( tex2DNode77 * saturate( ( RimSetUp23 + ( ReflectFresnel44 * Reflect59 ) ) ) ) + (( _Emission )?( ( _EmissionIntensity * _EmissionColor * tex2D( _EmissionTexture, uv_EmissionTexture ) ) ):( float4( 0,0,0,0 ) )) ).rgb;
			o.Metallic = ( tex2DNode77 * _Metallic * (( _Properties )?( 1.0 ):( 0.0 )) ).r;
			o.Smoothness = ( _Smoothness * (( _Properties )?( 1.0 ):( 0.0 )) );
			float2 uv_AmbientOcclusionTexture = i.uv_texcoord * _AmbientOcclusionTexture_ST.xy + _AmbientOcclusionTexture_ST.zw;
			float4 clampResult89 = clamp( ( tex2D( _AmbientOcclusionTexture, uv_AmbientOcclusionTexture ) + ( 1.0 - _AOIntensity ) + _AmbientOcclusionColor ) , float4( 0,0,0,0 ) , float4( 1,1,1,0 ) );
			o.Occlusion = (( _AmbientOcclusion )?( clampResult89 ):( float4( 1,1,1,0 ) )).r;
			o.Transmission = (( _TransmissionOnOff )?( _TransmissionColor ):( float4( 0,0,0,0 ) )).rgb;
			o.Translucency = (( _TranslucencyOnOff )?( _TranslucencyColor ):( float4( 0,0,0,0 ) )).rgb;
			o.Alpha = 1;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf StandardCustom keepalpha fullforwardshadows exclude_path:deferred 

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
				SurfaceOutputStandardCustom o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandardCustom, o )
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
	CustomEditor "FSurfaceEditor"
}
/*ASEBEGIN
Version=19108
Node;AmplifyShaderEditor.CommentaryNode;1;-3046.852,-510.1035;Inherit;False;790.9033;522.4771;Main Texture;6;7;6;5;4;3;2;Main Texture;0.4566038,0.9526708,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;10;-4444.537,-547.7506;Inherit;False;1362.807;553.545;Comment;7;17;16;15;14;13;12;11;Normals;0.4402515,0.4635113,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;19;-3764.744,101.3995;Inherit;False;1512.866;597.899;Rim Light;14;33;32;31;30;29;28;27;26;25;24;23;22;21;20;Rim Light;0.4422169,1,0.3915094,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;20;-3088.471,186.4444;Inherit;True;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;-2791.587,182.0553;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;23;-2505.084,177.7657;Inherit;True;RimSetUp;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;24;-2785.911,438.1274;Inherit;True;RimSwitch;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;25;-3404.33,601.3464;Inherit;False;Property;_RimIntensity;Rim Intensity;10;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;28;-3720.22,227.1196;Inherit;True;World;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;29;-3726.837,435.7014;Inherit;False;Property;_RimBias;Rim Bias;12;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;30;-3726.837,515.7013;Inherit;False;Property;_RimScale;Rim Scale;13;0;Create;True;0;0;0;False;0;False;1;1;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;31;-3726.837,607.7013;Inherit;False;Property;_RimPower;Rim Power;14;0;Create;True;0;0;0;False;0;False;1.5;1.5;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.FresnelNode;32;-3426.743,184.6993;Inherit;True;Standard;WorldNormal;ViewDir;True;True;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;33;-3073.991,416.7463;Inherit;False;Property;_RimLight;RimLight;9;0;Create;True;0;0;0;False;0;False;0;True;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;22;-2505.907,406.0482;Inherit;True;Fresnel;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;27;-3721.211,145.7124;Inherit;False;17;NewNormals;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;35;-3831.282,797.4146;Inherit;False;1612.246;619.855;Reflect Fresnel;10;44;43;42;38;36;45;37;39;40;41;Reflect Fresnel;0,0.7859545,1,1;0;0
Node;AmplifyShaderEditor.ToggleSwitchNode;43;-2672.266,883.5916;Inherit;False;Property;_ReflectionFresnel;ReflectionFresnel;24;0;Create;True;0;0;0;False;0;False;0;True;2;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;44;-2427.844,875.6445;Inherit;True;ReflectFresnel;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;47;-4532.222,1479.028;Inherit;False;2322.336;690.999;Reflection Map;17;64;63;62;61;60;59;58;57;56;55;54;53;52;51;50;49;48;Reflection Map;0,0.4211543,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;48;-3121.383,1805.009;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;49;-4003.132,1721.595;Inherit;True;Compute Rotation Y;-1;;65;693b7d13a80c93a4e8b791a9cd5e5ab2;0;2;38;FLOAT3;0,0,0;False;43;FLOAT;0;False;1;FLOAT3;19
Node;AmplifyShaderEditor.RadiansOpNode;50;-4163.129,1769.595;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldReflectionVector;51;-4259.13,1545.595;Inherit;True;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DynamicAppendNode;52;-3854.875,2008.405;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.NegateNode;53;-4030.872,2024.404;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;54;-3700.334,1726.795;Inherit;True;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;55;-4490.544,1545.705;Inherit;True;15;NewObjectNormal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;56;-2853.988,1803.857;Inherit;True;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;57;-3185.383,1693.009;Inherit;False;Property;_ReflectionStrength;Reflection Strength;19;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;59;-2414.838,1799.52;Inherit;True;Reflect;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;60;-3377.383,1933.009;Inherit;False;Property;_ReflectColor;Reflect Color;20;1;[HDR];Create;True;0;0;0;False;0;False;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;63;-4451.131,1769.595;Inherit;False;Property;_CubeMapRotate;Cube Map Rotate;22;0;Create;True;0;0;0;False;0;False;0;0;0;360;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;64;-4318.871,2024.404;Inherit;False;Property;_CubemapYPosition;Cubemap Y Position;23;0;Create;True;0;0;0;False;0;False;0;0;-5;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;42;-2918.055,902.0345;Inherit;False;Property;_RefFresnelInvert;RefFresnelInvert;25;0;Create;True;0;0;0;False;0;False;0;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;61;-3473.383,1725.009;Inherit;True;Property;_ReflectMap1;Reflect Map;21;0;Create;True;1;Reflection Settings;0;0;False;0;False;-1;a56607be4cc72cb45a5062c7dca43a1b;a56607be4cc72cb45a5062c7dca43a1b;True;0;False;white;LockedToCube;False;Object;-1;Auto;Cube;8;0;SAMPLERCUBE;;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;45;-3722.335,834.532;Inherit;False;17;NewNormals;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;37;-3755.714,939.9985;Inherit;True;World;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;39;-3779.516,1148.58;Inherit;False;Property;_ReflectFresnelBias;Reflect Fresnel Bias;26;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;40;-3772.641,1236.262;Inherit;False;Property;_ReflectFresnelScale;Reflect Fresnel Scale;27;0;Create;True;0;0;0;False;0;False;1;4.34;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;41;-3772.641,1330.892;Inherit;False;Property;_ReflectFresnelPower;Reflect Fresnel Power;28;0;Create;True;0;0;0;False;0;False;1.5;1.49;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.FresnelNode;36;-3455.366,904.4532;Inherit;True;Standard;WorldNormal;ViewDir;True;True;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;38;-3130.085,1009.229;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;58;-3124.516,1491.189;Inherit;True;44;ReflectFresnel;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;-3022.852,-226.1695;Inherit;True;Property;_MainTexture;Main Texture;1;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;-2697.693,-452.0776;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;5;-2699.239,-229.4898;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;6;-2467.217,-453.4175;Inherit;True;MainDiffuse;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;7;-2467.827,-231.4356;Inherit;True;MainAlpha;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;11;-4089.526,-483.7505;Inherit;True;Property;_NormalMap;Normal Map;7;1;[Normal];Create;True;1;Normal Settings;0;0;False;0;False;-1;None;6d39320162db6324482a2e59f12ba2d1;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ToggleSwitchNode;12;-3776.117,-481.0376;Inherit;True;Property;_Normal;Normal;6;0;Create;True;0;0;0;False;0;False;0;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;13;-4393.537,-483.7505;Float;True;Property;_NormalScale;Normal Scale;8;0;Create;True;0;0;0;False;0;False;0.3;0.3;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;14;-4052.827,-246.1313;Inherit;True;Normal Reconstruct Z;-1;;66;63ba85b764ae0c84ab3d698b86364ae9;0;1;1;FLOAT2;0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;15;-3302.747,-229.9473;Inherit;True;NewObjectNormal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WorldNormalVector;16;-3537.799,-483.8196;Inherit;True;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RegisterLocalVarNode;17;-3285.921,-480.9427;Float;True;NewNormals;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;9;-301.5699,-580.6376;Inherit;True;6;MainDiffuse;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;18;-297.0965,-391.7827;Inherit;True;15;NewObjectNormal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ColorNode;76;-365.765,125.9685;Inherit;False;Property;_TransmissionColor;Transmission Color;43;1;[HDR];Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;74;-1005.861,-107.8948;Inherit;False;Property;_Metallic;Metallic;15;0;Create;True;0;0;0;False;0;False;0;0;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;86;-1195.997,58.76698;Inherit;True;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;89;-947.697,58.76686;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;87;-1403.959,174.3742;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;78;-647.9902,-200.6478;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;68;-607.8797,541.4069;Inherit;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;66;-912.1028,542.8412;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;46;-1439.485,622.1237;Inherit;True;44;ReflectFresnel;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;90;-364.2751,550.3974;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;94;-119.1158,666.3771;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;34;-1438.595,408.8903;Inherit;True;23;RimSetUp;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;93;-647.4437,1052.786;Inherit;True;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;3;-2934.196,-453.1035;Inherit;False;Property;_MainColor1;Main Color;0;0;Create;True;1;Diffuse Settings;0;0;False;0;False;0.6,0.6,0.6,1;0.7924528,0.7924528,0.7924528,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;79;-1704.523,-29.5139;Inherit;True;Property;_AmbientOcclusionTexture;Ambient Occlusion Texture;5;0;Create;True;0;0;0;False;0;False;-1;9a8dd88696a6e4787b38a3fe818ac0f6;9a8dd88696a6e4787b38a3fe818ac0f6;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ToggleSwitchNode;103;-648.1199,26.15275;Inherit;False;Property;_AmbientOcclusion;AmbientOcclusion;2;0;Create;True;0;0;0;False;0;False;0;True;2;0;COLOR;1,1,1,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;80;-1680.674,171.9044;Inherit;True;Property;_AOIntensity;AO Intensity;4;0;Create;True;0;0;0;False;0;False;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;104;-1653.202,-209.1116;Inherit;False;Property;_AmbientOcclusionColor;Ambient Occlusion Color;3;0;Create;True;0;0;0;False;0;False;0.5,0.5,0.5,0;0.5,0.5,0.5,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;26;-3339.51,419.1664;Inherit;False;Property;_RimColor;Rim Color;11;1;[HDR];Create;True;1;Rim Light Settings;0;0;False;0;False;0.4,0.4,0.4,0;0.4,0.4,0.4,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;77;-1028.706,-311.459;Inherit;True;Property;_MetallicTexture;Metallic Texture;17;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;75;-401.9393,3.366282;Inherit;False;Property;_Smoothness;Smoothness;16;0;Create;True;0;0;0;False;0;False;0;0;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;105;-948.4272,-470.2559;Inherit;False;Property;_Properties;Properties;41;0;Create;True;0;0;0;False;0;False;0;True;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;106;-95.62822,-161.456;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;69;-1124.746,737.082;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;65;-1440.848,827.631;Inherit;True;59;Reflect;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.ToggleSwitchNode;62;-2633.153,1803.921;Inherit;False;Property;_Reflect;Reflect;18;0;Create;True;0;0;0;False;0;False;0;True;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ToggleSwitchNode;99;-396.0794,1049.503;Inherit;False;Property;_Emission;Emission;29;0;Create;True;0;0;0;False;0;False;0;True;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;91;-977.4435,997.6666;Inherit;False;Property;_EmissionIntensity;Emission Intensity;30;0;Create;True;0;0;0;False;0;False;0.5;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;92;-996.5283,1292.161;Inherit;True;Property;_EmissionTexture;Emission Texture;31;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;107;-924.7052,1090.097;Inherit;False;Property;_EmissionColor;Emission Color;40;1;[HDR];Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;73;-364.3133,301.4012;Inherit;False;Property;_TranslucencyColor;Translucency Color;44;1;[HDR];Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ToggleSwitchNode;102;-86.84959,383.1999;Inherit;False;Property;_TranslucencyOnOff;TranslucencyOnOff;42;0;Create;True;0;0;0;False;0;False;0;True;2;0;COLOR;0,0,0,0;False;1;COLOR;1,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;441.618,-33.79337;Float;False;True;-1;2;FSurfaceEditor;0;0;Standard;FiberShaders/FSurface;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;ForwardOnly;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;32;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.ToggleSwitchNode;110;-86.15004,130.8313;Inherit;False;Property;_TransmissionOnOff;TransmissionOnOff;39;0;Create;True;0;0;0;False;0;False;0;True;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
WireConnection;20;0;32;0
WireConnection;20;1;26;0
WireConnection;20;2;25;0
WireConnection;21;0;20;0
WireConnection;21;1;33;0
WireConnection;23;0;21;0
WireConnection;24;0;33;0
WireConnection;32;0;27;0
WireConnection;32;4;28;0
WireConnection;32;1;29;0
WireConnection;32;2;30;0
WireConnection;32;3;31;0
WireConnection;22;0;32;0
WireConnection;43;1;42;0
WireConnection;44;0;43;0
WireConnection;48;0;61;0
WireConnection;48;1;60;0
WireConnection;49;38;51;0
WireConnection;49;43;50;0
WireConnection;50;0;63;0
WireConnection;51;0;55;0
WireConnection;52;1;53;0
WireConnection;53;0;64;0
WireConnection;54;0;49;19
WireConnection;54;1;52;0
WireConnection;56;0;57;0
WireConnection;56;1;48;0
WireConnection;56;2;58;0
WireConnection;59;0;62;0
WireConnection;42;0;36;0
WireConnection;42;1;38;0
WireConnection;61;1;54;0
WireConnection;36;0;45;0
WireConnection;36;4;37;0
WireConnection;36;1;39;0
WireConnection;36;2;40;0
WireConnection;36;3;41;0
WireConnection;38;0;36;0
WireConnection;4;0;3;0
WireConnection;4;1;2;0
WireConnection;5;0;3;4
WireConnection;5;1;2;4
WireConnection;6;0;4;0
WireConnection;7;0;5;0
WireConnection;11;5;13;0
WireConnection;12;0;14;0
WireConnection;12;1;11;0
WireConnection;15;0;12;0
WireConnection;16;0;12;0
WireConnection;17;0;16;0
WireConnection;86;0;79;0
WireConnection;86;1;87;0
WireConnection;86;2;104;0
WireConnection;89;0;86;0
WireConnection;87;0;80;0
WireConnection;78;0;77;0
WireConnection;78;1;74;0
WireConnection;78;2;105;0
WireConnection;68;0;66;0
WireConnection;66;0;34;0
WireConnection;66;1;69;0
WireConnection;90;0;77;0
WireConnection;90;1;68;0
WireConnection;94;0;90;0
WireConnection;94;1;99;0
WireConnection;93;0;91;0
WireConnection;93;1;107;0
WireConnection;93;2;92;0
WireConnection;103;1;89;0
WireConnection;106;0;75;0
WireConnection;106;1;105;0
WireConnection;69;0;46;0
WireConnection;69;1;65;0
WireConnection;62;1;56;0
WireConnection;99;1;93;0
WireConnection;102;1;73;0
WireConnection;0;0;9;0
WireConnection;0;1;18;0
WireConnection;0;2;94;0
WireConnection;0;3;78;0
WireConnection;0;4;106;0
WireConnection;0;5;103;0
WireConnection;0;6;110;0
WireConnection;0;7;102;0
WireConnection;110;1;76;0
ASEEND*/
//CHKSM=38EE309837D4E54D3008FB6CA47080DE55C92C0C