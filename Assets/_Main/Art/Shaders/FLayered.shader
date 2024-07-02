// Made with Amplify Shader Editor v1.9.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "FiberShaders/FLayered"
{
	Properties
	{
		[HDR][Gamma]_LayerAColor("LayerAColor", Color) = (1,1,1,1)
		[Gamma]_LayerATexture("LayerATexture", 2D) = "white" {}
		[Toggle]_LayerBSwitch("LayerBSwitch", Float) = 0
		[Toggle]_LayerASwitch("LayerASwitch", Float) = 0
		[HDR][Gamma]_LayerBColor("LayerBColor", Color) = (1,1,1,1)
		[Gamma]_LayerBTexture("LayerBTexture", 2D) = "white" {}
		[Toggle]_LayerCSwitch("LayerCSwitch", Float) = 0
		[HDR][Gamma]_LayerCColor("LayerCColor", Color) = (1,1,1,1)
		[Gamma]_LayerCTexture("LayerCTexture", 2D) = "white" {}
		[Toggle]_LayerDSwitch("LayerDSwitch", Float) = 0
		[HDR]_LayerDColor("LayerDColor", Color) = (1,1,1,1)
		_LayerDTexture("LayerDTexture", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Unlit alpha:fade keepalpha 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform half4 _LayerAColor;
		uniform sampler2D _LayerATexture;
		uniform half4 _LayerATexture_ST;
		uniform half _LayerASwitch;
		uniform half4 _LayerBColor;
		uniform sampler2D _LayerBTexture;
		uniform half4 _LayerBTexture_ST;
		uniform half _LayerBSwitch;
		uniform half4 _LayerCColor;
		uniform sampler2D _LayerCTexture;
		uniform half4 _LayerCTexture_ST;
		uniform half _LayerCSwitch;
		uniform half4 _LayerDColor;
		uniform sampler2D _LayerDTexture;
		uniform half4 _LayerDTexture_ST;
		uniform half _LayerDSwitch;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_LayerATexture = i.uv_texcoord * _LayerATexture_ST.xy + _LayerATexture_ST.zw;
			half4 tex2DNode5 = tex2D( _LayerATexture, uv_LayerATexture );
			half LayerA89 = (( _LayerASwitch )?( 1.0 ):( 0.0 ));
			float2 uv_LayerBTexture = i.uv_texcoord * _LayerBTexture_ST.xy + _LayerBTexture_ST.zw;
			half4 tex2DNode12 = tex2D( _LayerBTexture, uv_LayerBTexture );
			half LayerB30 = (( _LayerBSwitch )?( 1.0 ):( 0.0 ));
			half temp_output_44_0 = ( _LayerBColor.a * tex2DNode12.a * LayerB30 );
			half4 lerpResult53 = lerp( ( ( _LayerAColor * tex2DNode5 ) * LayerA89 ) , ( ( _LayerBColor * tex2DNode12 ) * LayerB30 ) , temp_output_44_0);
			float2 uv_LayerCTexture = i.uv_texcoord * _LayerCTexture_ST.xy + _LayerCTexture_ST.zw;
			half4 tex2DNode15 = tex2D( _LayerCTexture, uv_LayerCTexture );
			half LayerC34 = (( _LayerCSwitch )?( 1.0 ):( 0.0 ));
			half temp_output_48_0 = ( _LayerCColor.a * tex2DNode15.a * LayerC34 );
			half4 lerpResult56 = lerp( lerpResult53 , ( ( _LayerCColor * tex2DNode15 ) * LayerC34 ) , temp_output_48_0);
			float2 uv_LayerDTexture = i.uv_texcoord * _LayerDTexture_ST.xy + _LayerDTexture_ST.zw;
			half4 tex2DNode19 = tex2D( _LayerDTexture, uv_LayerDTexture );
			half LayerD36 = (( _LayerDSwitch )?( 1.0 ):( 0.0 ));
			half temp_output_50_0 = ( _LayerDColor.a * tex2DNode19.a * LayerD36 );
			half4 lerpResult59 = lerp( lerpResult56 , ( ( _LayerDColor * tex2DNode19 ) * LayerD36 ) , temp_output_50_0);
			o.Emission = lerpResult59.rgb;
			o.Alpha = ( ( _LayerAColor.a * tex2DNode5.a * LayerA89 ) + temp_output_44_0 + temp_output_48_0 + temp_output_50_0 );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "FLayeredEditor"
}
/*ASEBEGIN
Version=19200
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;10;-1506.648,-601.8071;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;13;-1496.075,-28.03471;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;48;-1499.81,200.3171;Inherit;True;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;17;-1487.525,488.329;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;50;-1492.804,739.9146;Inherit;True;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;55;-2206.527,92.32823;Inherit;True;34;LayerC;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;58;-2196.24,600.1453;Inherit;True;36;LayerD;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;57;-192.6122,497.513;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;53;-629.7472,-612.3806;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;59;41.34544,490.1461;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;51;-1010.117,-584.5797;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;54;-911.8943,-5.540424;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;56;-317.4823,0.6081802;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;47;-1167.84,-228.795;Inherit;True;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;34;-679.6448,-1423.239;Inherit;False;LayerC;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;36;-680.1205,-1304.362;Inherit;False;LayerD;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;11;-1839.934,-597.5129;Inherit;False;Property;_LayerBColor;LayerBColor;4;2;[HDR];[Gamma];Create;True;0;0;0;False;0;False;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;12;-1895.555,-343.5472;Inherit;True;Property;_LayerBTexture;LayerBTexture;5;1;[Gamma];Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;14;-1828.428,-32.51289;Inherit;False;Property;_LayerCColor;LayerCColor;7;2;[HDR];[Gamma];Create;True;0;0;0;False;0;False;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;15;-1891.497,196.3693;Inherit;True;Property;_LayerCTexture;LayerCTexture;8;1;[Gamma];Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;18;-1832.753,495.5918;Inherit;False;Property;_LayerDColor;LayerDColor;10;1;[HDR];Create;True;0;0;0;False;0;False;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;19;-1898.305,734.6599;Inherit;True;Property;_LayerDTexture;LayerDTexture;11;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ToggleSwitchNode;32;-939.2479,-1423.273;Inherit;False;Property;_LayerCSwitch;LayerCSwitch;6;0;Create;True;0;0;0;False;0;False;0;True;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;35;-939.2715,-1302.557;Inherit;False;Property;_LayerDSwitch;LayerDSwitch;9;0;Create;True;0;0;0;False;0;False;0;True;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;88;405.0911,448.9197;Half;False;True;-1;2;FLayeredEditor;0;0;Unlit;FiberShaders/FLayered;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;1;False;;0;False;;False;0;False;;0;False;;False;1;Transparent;0;True;False;0;False;Transparent;;Transparent;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;2;5;False;;10;False;;0;5;False;;10;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;30;-680.8666,-1540.102;Inherit;False;LayerB;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;29;-934.3934,-1539.794;Inherit;False;Property;_LayerBSwitch;LayerBSwitch;2;0;Create;True;0;0;0;False;0;False;0;True;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;45;-1535.103,-1194.911;Inherit;True;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;-1533.824,-1426.61;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;6;-1868.271,-1422.162;Inherit;False;Property;_LayerAColor;LayerAColor;0;2;[HDR];[Gamma];Create;True;0;0;0;False;0;False;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;5;-1941.271,-1201.986;Inherit;True;Property;_LayerATexture;LayerATexture;1;1;[Gamma];Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;52;-2186.094,-420.757;Inherit;True;30;LayerB;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;90;-937.8348,-1659.445;Inherit;False;Property;_LayerASwitch;LayerASwitch;3;0;Create;True;0;0;0;False;0;False;0;True;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;89;-684.308,-1659.754;Inherit;False;LayerA;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;91;-2042.295,-974.4362;Inherit;True;89;LayerA;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;44;-1508.878,-347.1857;Inherit;True;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;92;-1019.488,-1093.057;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
WireConnection;10;0;11;0
WireConnection;10;1;12;0
WireConnection;13;0;14;0
WireConnection;13;1;15;0
WireConnection;48;0;14;4
WireConnection;48;1;15;4
WireConnection;48;2;55;0
WireConnection;17;0;18;0
WireConnection;17;1;19;0
WireConnection;50;0;18;4
WireConnection;50;1;19;4
WireConnection;50;2;58;0
WireConnection;57;0;17;0
WireConnection;57;1;58;0
WireConnection;53;0;92;0
WireConnection;53;1;51;0
WireConnection;53;2;44;0
WireConnection;59;0;56;0
WireConnection;59;1;57;0
WireConnection;59;2;50;0
WireConnection;51;0;10;0
WireConnection;51;1;52;0
WireConnection;54;0;13;0
WireConnection;54;1;55;0
WireConnection;56;0;53;0
WireConnection;56;1;54;0
WireConnection;56;2;48;0
WireConnection;47;0;45;0
WireConnection;47;1;44;0
WireConnection;47;2;48;0
WireConnection;47;3;50;0
WireConnection;34;0;32;0
WireConnection;36;0;35;0
WireConnection;88;2;59;0
WireConnection;88;9;47;0
WireConnection;30;0;29;0
WireConnection;45;0;6;4
WireConnection;45;1;5;4
WireConnection;45;2;91;0
WireConnection;7;0;6;0
WireConnection;7;1;5;0
WireConnection;89;0;90;0
WireConnection;44;0;11;4
WireConnection;44;1;12;4
WireConnection;44;2;52;0
WireConnection;92;0;7;0
WireConnection;92;1;91;0
ASEEND*/
//CHKSM=FF04719890CD41901AC0BD0547B52D80184CCDBE