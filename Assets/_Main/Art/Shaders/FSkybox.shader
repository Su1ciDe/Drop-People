// Made with Amplify Shader Editor v1.9.1.8
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "FiberShaders/FSkybox"
{
	Properties
	{
		[Toggle]_EnableFog("Enable Fog", Float) = 0
		[Toggle]_Gradient("Gradient", Float) = 0
		[Toggle]_EnableHaze("Enable Haze", Float) = 0
		[HDR][Gamma][NoScaleOffset]_Tex("Cubemap", CUBE) = "black" {}
		[HDR][Gamma][NoScaleOffset]_Tex1("Cubemap Layer", CUBE) = "black" {}
		[Space(10)]_CubemapTransition("Cubemap Blend", Range( 0 , 1)) = 0
		_Exposure("Cubemap Exposure", Range( 0 , 8)) = 1
		[HDR][Gamma]_TopColor("Top Color", Color) = (0.467,0.735135,1,1)
		[HDR][Gamma]_TintColor("Cubemap Tint Color", Color) = (0.5,0.5,0.5,1)
		[HDR][Gamma]_BottomColor("Bottom Color", Color) = (0.348,0.2635828,0.1479,1)
		[HDR][Gamma]_Power("Power", Range( 0 , 8)) = 1
		_PositionY("PositionY", Float) = 0
		[HDR][Gamma]_Falloff("Falloff", Range( 0 , 10)) = 0.5
		[HDR][Gamma]_HorizonHeight("Horizon Height", Range( -1 , 1)) = 0
		_HazePosition("Haze Position", Float) = 0
		_FogPosition("Fog Position", Float) = 0
		_Rotation("Rotation", Range( 0 , 360)) = 0
		_RotationSpeed("Rotation Speed", Float) = 1
		[HideInInspector]_Tex_HDR1("DecodeInstructions", Vector) = (0,0,0,0)
		[HideInInspector]_Tex_HDR("DecodeInstructions", Vector) = (0,0,0,0)
		_PositionX("PositionX", Float) = 0
		_HazeHeight("Haze Height", Range( 0 , 1)) = 0.3
		_FogHeight("Fog Height", Range( 0 , 1)) = 0.45
		_PositionZ("PositionZ", Float) = 0
		[Toggle]_EnableRotation("Enable Rotation", Float) = 0
		_FogIntensity("Fog Intensity", Range( 0 , 1)) = 1
		_HazeIntensity("Haze Intensity", Range( 0 , 1)) = 1
		_FogFill("Fog Fill", Range( 0 , 1)) = 0.3
		_HazeFill("Haze Fill", Range( 0 , 1)) = 0
		_FogSmoothness("Fog Smoothness", Range( -2 , 1)) = 0.2
		_HazeSmoothness("Haze Smoothness", Range( -2 , 1)) = 0.175
		[HDR][Gamma]_HazeColor("Haze Color", Color) = (1,0.5969347,0,1)
		[Toggle]_CustomFogColor("Custom Fog Color", Float) = 1
		[HDR][Gamma]_FogColor("Fog Color", Color) = (0.4875,0.455,0.65,1)
		[Toggle]_BlendSky("Blend Sky", Float) = 0
		[Toggle]_CubemapSky("Cubemap Sky", Float) = 0
		[Toggle]_AdditiveGradient("Additive Gradient", Float) = 0
		[HDR][Gamma]_BaseColor("Base Color", Color) = (1,1,1,1)
		[ASEEnd][Toggle]_EnablePosition("EnablePosition", Float) = 0

	}
	
	SubShader
	{
		
		
		Tags { "RenderType"="Opaque" }
	LOD 100

		CGINCLUDE
		#pragma target 3.0
		ENDCG
		Blend Off
		AlphaToMask Off
		Cull Back
		ColorMask RGBA
		ZWrite On
		ZTest LEqual
		Offset 0 , 0
		
		
		
		Pass
		{
			Name "Unlit"

			CGPROGRAM

			

			#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
			//only defining to not throw compilation error over Unity 5.5
			#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
			#endif
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"
			#include "UnityShaderVariables.cginc"
			#define ASE_NEEDS_VERT_POSITION
			#define ASE_NEEDS_FRAG_POSITION


			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 worldPos : TEXCOORD0;
				#endif
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			uniform half4 _Tex_HDR;
			uniform half4 _Tex_HDR1;
			uniform half _EnableFog;
			uniform half _EnableHaze;
			uniform half _AdditiveGradient;
			uniform half _Gradient;
			uniform half4 _BaseColor;
			uniform half4 _TopColor;
			uniform half4 _BottomColor;
			uniform half _HorizonHeight;
			uniform half _Falloff;
			uniform half _Power;
			uniform half _CubemapSky;
			uniform half _BlendSky;
			uniform samplerCUBE _Tex;
			uniform half _EnableRotation;
			uniform half _EnablePosition;
			uniform half _PositionX;
			uniform half _PositionY;
			uniform half _PositionZ;
			uniform float _Rotation;
			uniform float _RotationSpeed;
			uniform samplerCUBE _Tex1;
			uniform half _CubemapTransition;
			uniform half4 _TintColor;
			uniform half _Exposure;
			uniform half4 _HazeColor;
			uniform half _HazePosition;
			uniform half _HazeHeight;
			uniform half _HazeSmoothness;
			uniform half _HazeFill;
			uniform half _HazeIntensity;
			uniform half _CustomFogColor;
			uniform half4 _FogColor;
			uniform half _FogPosition;
			uniform half _FogHeight;
			uniform half _FogSmoothness;
			uniform half _FogFill;
			uniform half _FogIntensity;

			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				half lerpResult70 = lerp( 1.0 , ( unity_OrthoParams.y / unity_OrthoParams.x ) , unity_OrthoParams.w);
				half CameraMode67 = lerpResult70;
				half3 appendResult53 = (half3(v.vertex.xyz.x , ( v.vertex.xyz.y * CameraMode67 ) , v.vertex.xyz.z));
				half3 appendResult194 = (half3(0.0 , 0.0 , 0.0));
				half3 appendResult43 = (half3(_PositionX , -_PositionY , _PositionZ));
				half3 temp_output_33_0 = ( appendResult53 + (( _EnablePosition )?( appendResult43 ):( appendResult194 )) );
				half3 VertexPos40_g51 = appendResult53;
				half3 appendResult74_g51 = (half3(0.0 , VertexPos40_g51.y , 0.0));
				float3 VertexPosRotationAxis50_g51 = appendResult74_g51;
				half3 break84_g51 = VertexPos40_g51;
				half3 appendResult81_g51 = (half3(break84_g51.x , 0.0 , break84_g51.z));
				float3 VertexPosOtherAxis82_g51 = appendResult81_g51;
				half Angle44_g51 = ( radians( _Rotation ) + ( _Time.y * _RotationSpeed ) );
				half3 temp_output_44_0 = ( ( VertexPosRotationAxis50_g51 + ( VertexPosOtherAxis82_g51 * cos( Angle44_g51 ) ) + ( cross( half3(0,1,0) , VertexPosOtherAxis82_g51 ) * sin( Angle44_g51 ) ) ) + appendResult43 + (( _EnablePosition )?( appendResult43 ):( appendResult194 )) );
				half3 vertexToFrag46 = (( _EnableRotation )?( temp_output_44_0 ):( temp_output_33_0 ));
				o.ase_texcoord2.xyz = vertexToFrag46;
				
				o.ase_texcoord1 = v.vertex;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord2.w = 0;
				float3 vertexValue = float3(0, 0, 0);
				#if ASE_ABSOLUTE_VERTEX_POS
				vertexValue = v.vertex.xyz;
				#endif
				vertexValue = vertexValue;
				#if ASE_ABSOLUTE_VERTEX_POS
				v.vertex.xyz = vertexValue;
				#else
				v.vertex.xyz += vertexValue;
				#endif
				o.vertex = UnityObjectToClipPos(v.vertex);

				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				#endif
				return o;
			}
			
			fixed4 frag (v2f i ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
				fixed4 finalColor;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 WorldPosition = i.worldPos;
				#endif
				half4 BaseColor183 = _BaseColor;
				half3 temp_cast_0 = (i.ase_texcoord1.xyz.y).xxx;
				half3 temp_cast_1 = ((0.0 + (_HorizonHeight - -1.0) * (2.0 - 0.0) / (1.0 - -1.0))).xxx;
				half3 temp_cast_2 = (( 1.0 - (-3.0 + (0.5 - 0.0) * (1.0 - -3.0) / (1.0 - 0.0)) )).xxx;
				half HorizonPos18 = ( distance( max( ( abs( ( temp_cast_0 - temp_cast_1 ) ) - ( temp_cast_2 * float3( 0.5,0.5,0.5 ) ) ) , float3( 0,0,0 ) ) , float3( 0,0,0 ) ) / (0.01 + (_Falloff - 0.0) * (1.0 - 0.01) / (1.0 - 0.0)) );
				half saferPower20 = abs( HorizonPos18 );
				half4 lerpResult22 = lerp( _TopColor , _BottomColor , pow( saferPower20 , (0.001 + (_Power - 0.0) * (5.0 - 0.001) / (20.0 - 0.0)) ));
				half4 Gradient23 = lerpResult22;
				half3 vertexToFrag46 = i.ase_texcoord2.xyz;
				half4 texCUBENode57 = texCUBE( _Tex, vertexToFrag46 );
				half4 lerpResult162 = lerp( texCUBENode57 , texCUBE( _Tex1, vertexToFrag46 ) , _CubemapTransition);
				half4 CubeMap59 = ( (( _BlendSky )?( lerpResult162 ):( texCUBENode57 )) * _TintColor * _Exposure * unity_ColorSpaceDouble );
				half4 FinalComp186 = (( _AdditiveGradient )?( ( (( _Gradient )?( Gradient23 ):( BaseColor183 )) + (( _CubemapSky )?( CubeMap59 ):( BaseColor183 )) ) ):( ( (( _Gradient )?( Gradient23 ):( BaseColor183 )) * (( _CubemapSky )?( CubeMap59 ):( BaseColor183 )) ) ));
				half4 HazeColor147 = _HazeColor;
				half Zero173 = 0.0;
				half One174 = 1.0;
				half lerpResult128 = lerp( saturate( pow( (Zero173 + (abs( ( i.ase_texcoord1.xyz.y + -_HazePosition ) ) - Zero173) * (One174 - Zero173) / (_HazeHeight - Zero173)) , ( 1.0 - _HazeSmoothness ) ) ) , 0.0 , _HazeFill);
				half lerpResult129 = lerp( 1.0 , lerpResult128 , _HazeIntensity);
				half Haze130 = lerpResult129;
				half4 lerpResult146 = lerp( HazeColor147 , FinalComp186 , Haze130);
				half4 FogColor153 = (( _CustomFogColor )?( _FogColor ):( unity_FogColor ));
				half lerpResult103 = lerp( saturate( pow( (Zero173 + (abs( ( i.ase_texcoord1.xyz.y + -_FogPosition ) ) - Zero173) * (One174 - Zero173) / (_FogHeight - Zero173)) , ( 1.0 - _FogSmoothness ) ) ) , 0.0 , _FogFill);
				half lerpResult104 = lerp( 1.0 , lerpResult103 , _FogIntensity);
				half Fog105 = lerpResult104;
				half4 lerpResult156 = lerp( FogColor153 , (( _EnableHaze )?( lerpResult146 ):( FinalComp186 )) , Fog105);
				
				
				finalColor = (( _EnableFog )?( lerpResult156 ):( (( _EnableHaze )?( lerpResult146 ):( FinalComp186 )) ));
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "FSkyboxEditor"
	
	Fallback Off
}
/*ASEBEGIN
Version=19108
Node;AmplifyShaderEditor.CommentaryNode;190;1984.307,425.1045;Inherit;False;1361.8;465.4001;Final;10;156;2;3;155;154;1;148;146;106;187;Final;0.4188679,1,0.6931632,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;188;495.0063,421.4883;Inherit;False;1419.438;544.2423;Composite;9;170;169;167;24;27;165;64;184;186;Composite;1,0.5249614,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;31;-4220.208,-1455.647;Inherit;False;486.3721;591.3653;Vars;8;172;174;173;171;28;29;182;183;Vars;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;4;-4207.236,-679.017;Inherit;False;2217.7;857.2597;Gradient;19;192;16;191;8;21;19;26;5;6;10;7;13;18;20;23;22;9;14;12;Gradient;1,0,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;32;-4206.237,236.5394;Inherit;False;2227.392;753.6918;Cubemap Coordinates;23;36;52;41;46;65;35;51;40;48;50;49;42;39;38;37;53;47;45;44;43;33;194;195;Cubemap Coordinates;0,0.4980392,0,1;0;0
Node;AmplifyShaderEditor.SimpleAddOpNode;33;-2737.401,628.0874;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;34;-1923.746,237.7586;Inherit;False;1714.901;941.7689;Base;12;164;162;161;159;158;57;60;62;61;55;56;59;Base;0,0.4980392,1,1;0;0
Node;AmplifyShaderEditor.SimpleAddOpNode;44;-2737.401,372.0874;Inherit;False;3;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;45;-3889.401,500.0874;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;53;-3693.401,372.0874;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;38;-3917.401,743.0872;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;51;-3275.4,305.0874;Inherit;True;Compute Rotation Y;-1;;51;693b7d13a80c93a4e8b791a9cd5e5ab2;0;2;38;FLOAT3;0,0,0;False;43;FLOAT;0;False;1;FLOAT3;19
Node;AmplifyShaderEditor.StaticSwitch;35;-2545.4,343.3685;Float;False;Property;_EnableRotation;Enable Rotation;14;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT3;0,0,0;False;0;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT3;0,0,0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.PosVertexDataNode;41;-4164.401,347.0874;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;66;-4202.72,1028.308;Inherit;False;1146.89;300.717;Perspective|Orthographic;5;71;70;69;68;67;Perspective|Orthographic;1,0,1,1;0;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;68;-3912.786,1115.592;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OrthoParams;69;-4174.785,1140.592;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;70;-3528.785,1094.592;Inherit;False;3;0;FLOAT;1;False;1;FLOAT;0.5;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;71;-3719.786,1086.592;Half;False;Constant;_Float7;Float 7;47;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;67;-3272.786,1092.592;Half;True;CameraMode;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;52;-4160.4,523.0873;Inherit;False;67;CameraMode;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;29;-4007.713,-1405.647;Inherit;False;Gray50;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;78;-4195.638,1425.058;Inherit;False;1961.222;685.6313;Fog;22;151;152;107;105;104;102;103;101;100;99;97;89;81;82;150;98;88;94;92;153;175;176;Fog;0.3899074,0.5224131,0.6132076,1;0;0
Node;AmplifyShaderEditor.SimpleAddOpNode;92;-3842.608,1495.562;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;94;-3714.608,1495.562;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;88;-4086.699,1487.623;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;108;-4179.748,2167.68;Inherit;False;1797.025;545.9419;Haze;20;147;143;130;129;127;128;126;125;122;132;149;111;131;124;123;120;119;135;177;178;Haze;0.5283019,0.1564771,0,1;0;0
Node;AmplifyShaderEditor.PosVertexDataNode;135;-4074.008,2230.245;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;119;-3829.917,2238.184;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;120;-3667.164,2238.184;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;123;-3509.917,2238.184;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;124;-3301.917,2238.184;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;131;-3961.217,2378.986;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;122;-3486.617,2585.923;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;125;-3140.727,2241.58;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;128;-2981.703,2244.974;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;129;-2801.586,2239.882;Inherit;False;3;0;FLOAT;1;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;130;-2639.792,2240.532;Half;False;Haze;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;98;-3556.592,1495.562;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;82;-4003.875,1637.495;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;81;-4179.874,1637.495;Inherit;False;Property;_FogPosition;Fog Position;16;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;97;-3533.205,1834.307;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;99;-3354.502,1492.607;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;100;-3174.829,1489.652;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;103;-2994.396,1492.607;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;104;-2808.307,1491.13;Inherit;False;3;0;FLOAT;1;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;105;-2630.148,1487.774;Half;False;Fog;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FogAndAmbientColorsNode;107;-3062.077,1823.288;Inherit;False;unity_FogColor;0;1;COLOR;0
Node;AmplifyShaderEditor.ToggleSwitchNode;151;-2765.89,1819.313;Inherit;False;Property;_CustomFogColor;Custom Fog Color;33;0;Create;True;0;0;0;False;0;False;1;True;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;153;-2537.156,1815.793;Inherit;False;FogColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;147;-2638.734,2523.675;Inherit;False;HazeColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;28;-4170.208,-1403.969;Inherit;False;Constant;_GrayFifty;[GrayFifty];8;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;171;-4164.609,-1299.688;Inherit;False;Constant;_Zero;[Zero];38;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;173;-4003.781,-1300.059;Inherit;False;Zero;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;174;-4005.781,-1197.059;Inherit;False;One;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;172;-4163.148,-1196.167;Inherit;False;Constant;_One;[One];38;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;175;-3773.735,1586.274;Inherit;False;173;Zero;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;176;-3764.735,1752.274;Inherit;False;174;One;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;177;-3741.52,2332.945;Inherit;False;173;Zero;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;178;-3737.52,2500.945;Inherit;False;174;One;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;183;-3962.238,-1045.837;Inherit;False;BaseColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;64;732.8294,794.2147;Inherit;False;59;CubeMap;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;184;545.0063,622.793;Inherit;False;183;BaseColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;169;1248.294,695.4883;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;167;1232.294,471.4883;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ToggleSwitchNode;27;923.0867,475.2941;Inherit;True;Property;_Gradient;Gradient;1;0;Create;True;0;0;0;False;0;False;0;True;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;186;1689.645,579.8023;Inherit;False;FinalComp;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;156;2688.508,695.1047;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;155;2464.508,679.1047;Inherit;False;153;FogColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;154;2448.508,775.1049;Inherit;False;105;Fog;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;148;2071.508,475.1045;Inherit;False;147;HazeColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;146;2271.506,620.1045;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;106;2079.508,708.1046;Inherit;False;130;Haze;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;187;2034.307,579.3248;Inherit;False;186;FinalComp;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;12;-3389.948,-129.8053;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;14;-3357.948,-257.8052;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;22;-2508.828,-417.8419;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;23;-2252.829,-418.8419;Inherit;True;Gradient;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCRemapNode;13;-3767.948,-244.8052;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;-1;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;7;-3768.948,-52.80531;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;9;-3568.515,-401.8055;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ToggleSwitchNode;170;1440.294,567.4883;Inherit;True;Property;_AdditiveGradient;Additive Gradient;37;0;Create;True;0;0;0;False;0;False;0;True;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;61;-1030.315,877.1179;Half;False;Property;_Exposure;Cubemap Exposure;6;0;Create;False;0;0;0;False;0;False;1;1;0;8;0;1;FLOAT;0
Node;AmplifyShaderEditor.RadiansOpNode;42;-3883.665,634.944;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;39;-3714.544,723.9596;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;37;-4173.401,743.0872;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;40;-4186.401,629.0873;Float;False;Property;_Rotation;Rotation;17;0;Create;True;0;0;0;False;0;False;0;0;0;360;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;36;-4178.401,831.0865;Float;False;Property;_RotationSpeed;Rotation Speed;18;0;Create;True;0;0;0;False;0;False;1;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;47;-3343.401,819.0872;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;43;-3180.402,791.4872;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;50;-3352.978,910.2958;Inherit;False;Property;_PositionZ;PositionZ;24;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;49;-3355.978,727.2968;Inherit;False;Property;_PositionX;PositionX;21;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;194;-3183.365,590.7394;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ToggleSwitchNode;195;-3018.365,673.7394;Inherit;False;Property;_EnablePosition;EnablePosition;39;0;Create;True;0;0;0;False;0;False;0;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ToggleSwitchNode;65;-2518.708,510.9272;Inherit;False;Property;_EnableRotation;Enable Rotation;25;0;Create;True;0;0;0;False;0;False;0;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;127;-3095.614,2423.142;Half;False;Property;_HazeIntensity;Haze Intensity;27;0;Create;True;0;0;0;False;1;;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;126;-3272.844,2347.512;Half;False;Property;_HazeFill;Haze Fill;29;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;111;-4137.217,2378.986;Inherit;False;Property;_HazePosition;Haze Position;15;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;149;-3842.196,2420.732;Half;False;Property;_HazeHeight;Haze Height;22;0;Create;True;0;0;0;False;0;False;0.3;0.3;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;102;-3109.563,1719.564;Half;False;Property;_FogIntensity;Fog Intensity;26;0;Create;True;0;0;0;False;1;;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;101;-3307.01,1614.698;Half;False;Property;_FogFill;Fog Fill;28;0;Create;True;0;0;0;False;0;False;0.3;0.3;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;3;2432.508,535.1045;Inherit;False;Property;_EnableHaze;Enable Haze;2;0;Create;True;0;0;0;False;0;False;0;True;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ToggleSwitchNode;2;2880.508,615.1045;Inherit;False;Property;_EnableFog;Enable Fog;0;0;Create;True;0;0;0;False;0;False;0;True;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ToggleSwitchNode;165;925.8259,707.3306;Inherit;True;Property;_CubemapSky;Cubemap Sky;36;0;Create;True;0;0;0;False;0;False;0;True;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;1;3109.134,605.9289;Half;False;True;-1;2;FSkyboxEditor;100;5;FiberShaders/FSkybox;0770190933193b94aaa3065e307002fa;True;Unlit;0;0;Unlit;2;False;True;0;1;False;;0;False;;0;1;False;;0;False;;True;0;False;;0;False;;False;False;False;False;False;False;False;False;False;True;0;False;;False;True;0;False;;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;True;1;False;;True;3;False;;True;True;0;False;;0;False;;True;1;RenderType=Opaque=RenderType;True;2;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;0;;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;0;1;True;False;;False;0
Node;AmplifyShaderEditor.RangedFloatNode;150;-3868.695,1667.78;Half;False;Property;_FogHeight;Fog Height;23;0;Create;True;0;0;0;False;0;False;0.45;0.45;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;89;-3863.675,1835.321;Half;False;Property;_FogSmoothness;Fog Smoothness;30;0;Create;True;0;0;0;False;0;False;0.2;0.2;-2;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;132;-3838.497,2586.539;Half;False;Property;_HazeSmoothness;Haze Smoothness;31;0;Create;True;0;0;0;False;0;False;0.175;0.175;-2;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;162;-1244.081,468.5776;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;56;-652.4651,369.5067;Inherit;False;4;4;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;161;-1843.955,811.7075;Half;False;Property;_CubemapTransition;Cubemap Blend;5;0;Create;False;0;0;0;False;1;Space(10);False;0;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;24;735.0994,535.1194;Inherit;False;23;Gradient;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ToggleSwitchNode;164;-967.6295,366.8032;Inherit;False;Property;_BlendSky;Blend Sky;35;0;Create;True;0;0;0;False;0;False;0;True;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;59;-452.7594,372.0068;Half;True;CubeMap;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.VertexToFragmentNode;46;-2233.2,512.4874;Inherit;True;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;158;-1866.913,599.9799;Inherit;True;Property;_Tex1;Cubemap Layer;4;3;[HDR];[Gamma];[NoScaleOffset];Create;False;0;0;0;False;0;False;-1;None;None;True;0;False;black;LockedToCube;False;Object;-1;Auto;Cube;8;0;SAMPLERCUBE;;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;62;-970.3154,701.1183;Half;False;Property;_TintColor;Cubemap Tint Color;8;2;[HDR];[Gamma];Create;False;0;0;0;False;0;False;0.5,0.5,0.5,1;0.5,0.5,0.5,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorSpaceDouble;55;-970.3154,525.1185;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector4Node;60;-1881.638,942.3046;Half;False;Property;_Tex_HDR;DecodeInstructions;20;1;[HideInInspector];Create;False;0;0;0;True;0;False;0,0,0,0;1,1,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector4Node;159;-1662.192,942.2513;Half;False;Property;_Tex_HDR1;DecodeInstructions;19;1;[HideInInspector];Create;False;0;0;0;True;0;False;0,0,0,0;1,1,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;57;-1878.091,361.7673;Inherit;True;Property;_Tex;Cubemap;3;3;[HDR];[Gamma];[NoScaleOffset];Create;False;0;0;0;False;0;False;-1;None;None;True;0;False;black;LockedToCube;False;Object;-1;Auto;Cube;8;0;SAMPLERCUBE;;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;21;-3309.947,-625.8055;Inherit;False;Property;_TopColor;Top Color;7;2;[HDR];[Gamma];Create;True;0;0;0;False;0;False;0.467,0.735135,1,1;0,0.5,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;19;-3309.947,-433.8055;Inherit;False;Property;_BottomColor;Bottom Color;9;2;[HDR];[Gamma];Create;True;0;0;0;False;0;False;0.348,0.2635828,0.1479,1;0.6415094,0.1476068,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;182;-4184.237,-1083.837;Inherit;False;Property;_BaseColor;Base Color;38;2;[HDR];[Gamma];Create;True;0;0;0;False;0;False;1,1,1,1;0.5,0.5,0.5,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;143;-2885.886,2521.553;Inherit;False;Property;_HazeColor;Haze Color;32;2;[HDR];[Gamma];Create;True;0;0;0;False;0;False;1,0.5969347,0,1;1,0.5969347,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;152;-3036.806,1918.524;Inherit;False;Property;_FogColor;Fog Color;34;2;[HDR];[Gamma];Create;True;0;0;0;False;0;False;0.4875,0.455,0.65,1;0.4875,0.455,0.65,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;18;-3019.921,-231.4662;Inherit;False;HorizonPos;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;10;-4075.095,-250.0652;Inherit;False;Property;_HorizonHeight;Horizon Height;13;2;[HDR];[Gamma];Create;True;0;0;0;False;0;False;0;0;-1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;26;-3309.947,-241.8052;Inherit;False;BoxMask;-1;;52;9dce4093ad5a42b4aa255f0153c4f209;0;4;1;FLOAT3;0,0,0;False;4;FLOAT;0;False;10;FLOAT3;0,0,0;False;17;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;5;-4151.948,-58.06531;Inherit;False;Constant;_HorizonSize;Horizon Size;3;2;[HDR];[Gamma];Create;True;0;0;0;False;0;False;0.5;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;6;-3959.948,-52.80531;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-3;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;191;-3463.814,-21.06133;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0.01;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;8;-3762.948,47.19466;Inherit;False;Property;_Falloff;Falloff;12;2;[HDR];[Gamma];Create;True;0;0;0;False;0;False;0.5;1;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;16;-3254.1,53.95721;Inherit;False;Property;_Power;Power;10;2;[HDR];[Gamma];Create;True;0;0;0;False;0;False;1;1;0;8;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;192;-2962.653,-12.19513;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;20;False;3;FLOAT;0.001;False;4;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;20;-2778.948,-150.8052;Inherit;True;True;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;48;-3513.4,815.0872;Inherit;False;Property;_PositionY;PositionY;11;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
WireConnection;33;0;53;0
WireConnection;33;1;195;0
WireConnection;44;0;51;19
WireConnection;44;1;43;0
WireConnection;44;2;195;0
WireConnection;45;0;41;2
WireConnection;45;1;52;0
WireConnection;53;0;41;1
WireConnection;53;1;45;0
WireConnection;53;2;41;3
WireConnection;38;0;37;0
WireConnection;38;1;36;0
WireConnection;51;38;53;0
WireConnection;51;43;39;0
WireConnection;35;1;33;0
WireConnection;35;0;44;0
WireConnection;68;0;69;2
WireConnection;68;1;69;1
WireConnection;70;0;71;0
WireConnection;70;1;68;0
WireConnection;70;2;69;4
WireConnection;67;0;70;0
WireConnection;29;0;28;0
WireConnection;92;0;88;2
WireConnection;92;1;82;0
WireConnection;94;0;92;0
WireConnection;119;0;135;2
WireConnection;119;1;131;0
WireConnection;120;0;119;0
WireConnection;123;0;120;0
WireConnection;123;1;177;0
WireConnection;123;2;149;0
WireConnection;123;3;177;0
WireConnection;123;4;178;0
WireConnection;124;0;123;0
WireConnection;124;1;122;0
WireConnection;131;0;111;0
WireConnection;122;0;132;0
WireConnection;125;0;124;0
WireConnection;128;0;125;0
WireConnection;128;2;126;0
WireConnection;129;1;128;0
WireConnection;129;2;127;0
WireConnection;130;0;129;0
WireConnection;98;0;94;0
WireConnection;98;1;175;0
WireConnection;98;2;150;0
WireConnection;98;3;175;0
WireConnection;98;4;176;0
WireConnection;82;0;81;0
WireConnection;97;0;89;0
WireConnection;99;0;98;0
WireConnection;99;1;97;0
WireConnection;100;0;99;0
WireConnection;103;0;100;0
WireConnection;103;2;101;0
WireConnection;104;1;103;0
WireConnection;104;2;102;0
WireConnection;105;0;104;0
WireConnection;151;0;107;0
WireConnection;151;1;152;0
WireConnection;153;0;151;0
WireConnection;147;0;143;0
WireConnection;173;0;171;0
WireConnection;174;0;172;0
WireConnection;183;0;182;0
WireConnection;169;0;27;0
WireConnection;169;1;165;0
WireConnection;167;0;27;0
WireConnection;167;1;165;0
WireConnection;27;0;184;0
WireConnection;27;1;24;0
WireConnection;186;0;170;0
WireConnection;156;0;155;0
WireConnection;156;1;3;0
WireConnection;156;2;154;0
WireConnection;146;0;148;0
WireConnection;146;1;187;0
WireConnection;146;2;106;0
WireConnection;12;0;7;0
WireConnection;14;0;9;2
WireConnection;22;0;21;0
WireConnection;22;1;19;0
WireConnection;22;2;20;0
WireConnection;23;0;22;0
WireConnection;13;0;10;0
WireConnection;7;0;6;0
WireConnection;170;0;169;0
WireConnection;170;1;167;0
WireConnection;42;0;40;0
WireConnection;39;0;42;0
WireConnection;39;1;38;0
WireConnection;47;0;48;0
WireConnection;43;0;49;0
WireConnection;43;1;47;0
WireConnection;43;2;50;0
WireConnection;195;0;194;0
WireConnection;195;1;43;0
WireConnection;65;0;33;0
WireConnection;65;1;44;0
WireConnection;3;0;187;0
WireConnection;3;1;146;0
WireConnection;2;0;3;0
WireConnection;2;1;156;0
WireConnection;165;0;184;0
WireConnection;165;1;64;0
WireConnection;1;0;2;0
WireConnection;162;0;57;0
WireConnection;162;1;158;0
WireConnection;162;2;161;0
WireConnection;56;0;164;0
WireConnection;56;1;62;0
WireConnection;56;2;61;0
WireConnection;56;3;55;0
WireConnection;164;0;57;0
WireConnection;164;1;162;0
WireConnection;59;0;56;0
WireConnection;46;0;65;0
WireConnection;158;1;46;0
WireConnection;57;1;46;0
WireConnection;18;0;26;0
WireConnection;26;1;14;0
WireConnection;26;4;13;0
WireConnection;26;10;12;0
WireConnection;26;17;191;0
WireConnection;6;0;5;0
WireConnection;191;0;8;0
WireConnection;192;0;16;0
WireConnection;20;0;18;0
WireConnection;20;1;192;0
ASEEND*/
//CHKSM=1251E51D0766EFB8B63B5918D3A0EB221B869CB6