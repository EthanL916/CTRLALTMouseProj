// Made with Amplify Shader Editor v1.9.9.12
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Vefects/SH_Vefects_URP_VFX_Bubbles_Particles_Transparent_BIRP"
{
	Properties
	{
		[Space(13)][Header(General)][Space(13)] _EmissionMultiply( "Emission Multiply", Float ) = 0
		_SpeCular( "SpeCular", Float ) = 0.01
		_Smoothness( "Smoothness", Float ) = 0.99
		_OverallTilesMultiply( "Overall Tiles Multiply", Float ) = 1
		[Toggle( _USEWORLDSPACE_ON )] _UseWorldSpace( "Use World Space", Float ) = 0
		[Space(33)][Header(LUT)][Space(13)] _LUT( "LUT", 2D ) = "white" {}
		_LUTOffset( "LUT Offset", Float ) = 0
		_LUTAmplitude( "LUT Amplitude", Float ) = 1
		_LUTPanSpeed( "LUT Pan Speed", Float ) = 0
		_LUTDesaturate( "LUT Desaturate", Float ) = 0
		_LUTTint( "LUT Tint", Color ) = ( 1, 1, 1, 0 )
		_HueShift( "Hue Shift", Float ) = 0
		_HueShiftSpeed( "Hue Shift Speed", Float ) = 0
		[Space(33)][Header(Center Color)][Space(13)] _CenterColor( "Center Color", Color ) = ( 1, 1, 1, 0 )
		_CenterActive( "Center Active", Range( 0, 1 ) ) = 0
		_FrCenterScale( "Fr Center Scale", Float ) = 1
		_FrCenterPower( "Fr Center Power", Float ) = 1
		_FrCenterBias( "Fr Center Bias", Float ) = 0
		[Space(33)][Header(Fresnel)][Space(13)] _FrSolidColor( "Fr Solid Color", Color ) = ( 1, 1, 1, 0 )
		_FrSolidColorActive( "Fr Solid Color Active", Range( 0, 1 ) ) = 0
		_FrScale( "Fr Scale", Float ) = 1
		_FrPower( "Fr Power", Float ) = 1
		_FrBias( "Fr Bias", Float ) = 0
		_FrEmissionMultiply( "Fr Emission Multiply", Float ) = 0
		[Space(33)][Header(WPO Triplanar)][Space(13)] _WPOTriplanarTexture( "WPO Triplanar Texture", 2D ) = "white" {}
		_WPOIntensity( "WPO Intensity", Float ) = 0.1
		_WPOTriplanarContrast( "WPO Triplanar Contrast", Float ) = 1
		_WPOTile( "WPO Tile", Vector ) = ( 1, 1, 1, 0 )
		_WPOTileSpeed( "WPO Tile Speed", Vector ) = ( 0, 1, 1, 0 )
		_WPOTileOffsetCustom( "WPO Tile Offset Custom", Vector ) = ( 0, 0, 0, 0 )
		[Space(33)][Header(Opacity)][Space(13)] _OpacityOverall( "Opacity Overall", Float ) = 1
		_OpacityMin( "Opacity Min", Float ) = 0
		_OpacityMax( "Opacity Max", Float ) = 1
		_OpacityFrScale( "Opacity Fr Scale", Float ) = 1
		_OpacityFrPower( "Opacity Fr Power", Float ) = 1
		_OpacityFrBias( "Opacity Fr Bias", Float ) = 0
		[Space(33)][Header(AR)][Space(13)] _Cull( "Cull", Float ) = 2
		_Src( "Src", Float ) = 5
		_Dst( "Dst", Float ) = 10
		_ZWrite( "ZWrite", Float ) = 0
		_ZTest( "ZTest", Float ) = 2
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IsEmissive" = "true"  }
		Cull [_Cull]
		ZWrite [_ZWrite]
		ZTest [_ZTest]
		Blend [_Src] [_Dst]
		
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.5
		#pragma shader_feature_local _USEWORLDSPACE_ON
		#define ASE_VERSION 19912
		#pragma surface surf StandardSpecular keepalpha noshadow vertex:vertexDataFunc 
		#undef TRANSFORM_TEX
		#define TRANSFORM_TEX(tex,name) float4(tex.xy * name##_ST.xy + name##_ST.zw, tex.z, tex.w)
		struct Input
		{
			float3 worldPos;
			float3 worldNormal;
			float4 uv_texcoord;
			float4 vertexColor : COLOR;
		};

		uniform float _Src;
		uniform float _Dst;
		uniform float _ZWrite;
		uniform float _ZTest;
		uniform float _Cull;
		uniform sampler2D _WPOTriplanarTexture;
		uniform float3 _WPOTile;
		uniform float _OverallTilesMultiply;
		uniform float3 _WPOTileOffsetCustom;
		uniform float3 _WPOTileSpeed;
		uniform float _WPOTriplanarContrast;
		uniform float _WPOIntensity;
		uniform sampler2D _LUT;
		uniform float _LUTPanSpeed;
		uniform float _FrBias;
		uniform float _FrScale;
		uniform float _FrPower;
		uniform float _LUTOffset;
		uniform float _LUTAmplitude;
		uniform float _LUTDesaturate;
		uniform float4 _LUTTint;
		uniform float4 _FrSolidColor;
		uniform float _FrSolidColorActive;
		uniform float _HueShift;
		uniform float _HueShiftSpeed;
		uniform float4 _CenterColor;
		uniform float _FrCenterBias;
		uniform float _FrCenterScale;
		uniform float _FrCenterPower;
		uniform float _CenterActive;
		uniform float _EmissionMultiply;
		uniform float _FrEmissionMultiply;
		uniform float _SpeCular;
		uniform float _Smoothness;
		uniform float _OpacityMin;
		uniform float _OpacityMax;
		uniform float _OpacityFrBias;
		uniform float _OpacityFrScale;
		uniform float _OpacityFrPower;
		uniform float _OpacityOverall;


		float3 HSVToRGB( float3 c )
		{
			float4 K = float4( 1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0 );
			float3 p = abs( frac( c.xxx + K.xyz ) * 6.0 - K.www );
			return c.z * lerp( K.xxx, saturate( p - K.xxx ), c.y );
		}


		float3 RGBToHSV(float3 c)
		{
			float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
			float4 p = lerp( float4( c.bg, K.wz ), float4( c.gb, K.xy ), step( c.b, c.g ) );
			float4 q = lerp( float4( p.xyw, c.r ), float4( c.r, p.yzx ), step( p.x, c.r ) );
			float d = q.x - min( q.w, q.y );
			float e = 1.0e-10;
			return float3( abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
		}

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_normalOS = v.normal.xyz;
			float3 ase_positionWS = mul( unity_ObjectToWorld, v.vertex );
			float3 objToWorld112 = mul( unity_ObjectToWorld, float4( float3( 0,0,0 ), 1 ) ).xyz;
			#ifdef _USEWORLDSPACE_ON
				float3 staticSwitch192 = ase_positionWS;
			#else
				float3 staticSwitch192 = ( ase_positionWS - objToWorld112 );
			#endif
			float Tile_Rand219 = v.texcoord1.x;
			float overallTilesMult159 = ( _OverallTilesMultiply * Tile_Rand219 );
			float3 temp_output_118_0 = ( ( ( staticSwitch192 * ( _WPOTile * overallTilesMult159 ) ) + _WPOTileOffsetCustom ) + ( _Time.y * _WPOTileSpeed ) );
			float3 ase_normalWS = UnityObjectToWorldNormal( v.normal );
			float3 temp_cast_2 = (_WPOTriplanarContrast).xxx;
			float3 temp_output_125_0 = pow( abs( ase_normalWS ) , temp_cast_2 );
			float3 break129 = temp_output_125_0;
			float3 break127 = ( temp_output_125_0 / ( break129.x + break129.y + break129.z ) );
			float WPO_Rand212 = v.texcoord.z;
			float4 lerpResult100 = lerp( float4( float3( 0, 0, 0 ) , 0.0 ) , ( float4( ase_normalOS , 0.0 ) * saturate( ( ( tex2Dlod( _WPOTriplanarTexture, float4( (temp_output_118_0).xy, 0, 0.0) ) * break127.z ) + ( tex2Dlod( _WPOTriplanarTexture, float4( (temp_output_118_0).yz, 0, 0.0) ) * break127.x ) + ( tex2Dlod( _WPOTriplanarTexture, float4( (temp_output_118_0).xz, 0, 0.0) ) * break127.y ) ) ) ) , ( _WPOIntensity * WPO_Rand212 ));
			float4 VO193 = lerpResult100;
			v.vertex.xyz += VO193.rgb;
			v.vertex.w = 1;
		}

		void surf( Input i , inout SurfaceOutputStandardSpecular o )
		{
			float2 temp_cast_0 = (_LUTPanSpeed).xx;
			float3 ase_positionWS = i.worldPos;
			float3 ase_viewVectorWS = ( ( unity_OrthoParams.w == 0 ) ? _WorldSpaceCameraPos - ase_positionWS : UNITY_MATRIX_V[ 2 ].xyz );
			float3 ase_viewDirWS = normalize( ase_viewVectorWS );
			float3 ase_normalWS = i.worldNormal;
			float fresnelNdotV202 = dot( ase_normalWS, ase_viewDirWS );
			float fresnelNode202 = ( _FrBias + _FrScale * pow( max( 1.0 - fresnelNdotV202 , 0.0001 ), _FrPower ) );
			float CFr227 = saturate( fresnelNode202 );
			float LUT_Rand214 = i.uv_texcoord.w;
			float2 temp_cast_1 = (( ( CFr227 + ( _LUTOffset + LUT_Rand214 ) ) * _LUTAmplitude )).xx;
			float2 panner208 = ( 1.0 * _Time.y * temp_cast_0 + temp_cast_1);
			float3 desaturateInitialColor187 = tex2D( _LUT, panner208 ).rgb;
			float desaturateDot187 = dot( desaturateInitialColor187, float3( 0.299, 0.587, 0.114 ));
			float3 desaturateVar187 = lerp( desaturateInitialColor187, desaturateDot187.xxx, _LUTDesaturate );
			float4 lerpResult235 = lerp( ( float4( desaturateVar187 , 0.0 ) * _LUTTint ) , _FrSolidColor , _FrSolidColorActive);
			float3 hsvTorgb89 = RGBToHSV( lerpResult235.rgb );
			float HueShift249 = ( _HueShift + ( _Time.y * _HueShiftSpeed ) );
			float3 hsvTorgb88 = HSVToRGB( float3(( hsvTorgb89.x + HueShift249 ),hsvTorgb89.y,hsvTorgb89.z) );
			float fresnelNdotV242 = dot( ase_normalWS, ase_viewDirWS );
			float fresnelNode242 = ( _FrCenterBias + _FrCenterScale * pow( max( 1.0 - fresnelNdotV242 , 0.0001 ), _FrCenterPower ) );
			float CenFr247 = saturate( fresnelNode242 );
			float3 lerpResult237 = lerp( ( (i.vertexColor).rgb * _CenterColor.rgb ) , hsvTorgb88 , CenFr247);
			float3 lerpResult239 = lerp( hsvTorgb88 , lerpResult237 , _CenterActive);
			float3 BC197 = lerpResult239;
			o.Albedo = BC197;
			float4 lerpResult233 = lerp( float4( ( lerpResult239 * _EmissionMultiply ) , 0.0 ) , ( lerpResult235 * _FrEmissionMultiply ) , CFr227);
			float4 EM195 = lerpResult233;
			o.Emission = EM195.rgb;
			float3 temp_cast_6 = (_SpeCular).xxx;
			o.Specular = temp_cast_6;
			o.Smoothness = _Smoothness;
			float fresnelNdotV170 = dot( ase_normalWS, ase_viewDirWS );
			float fresnelNode170 = ( _OpacityFrBias + _OpacityFrScale * pow( max( 1.0 - fresnelNdotV170 , 0.0001 ), _OpacityFrPower ) );
			float lerpResult171 = lerp( _OpacityMin , _OpacityMax , fresnelNode170);
			float op179 = saturate( ( saturate( lerpResult171 ) * _OpacityOverall ) );
			o.Alpha = op179;
		}

		ENDCG
	}
	Fallback Off
	CustomEditor "AmplifyShaderEditor.MaterialInspector"
}
/*ASEBEGIN
Version=19912
{"type":"AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor","id":215,"pos":[-784,-2576],"params":["Inherit","False","566.2041","564.012","Rand","5","218","166","214","212","219","Rand","0,0,0,1","0","0"]}
{"type":"AmplifyShaderEditor.TexCoordVertexDataNode, AmplifyShaderEditor","id":218,"pos":[-736,-2272],"params":["Inherit","False","1","4","0","5","FLOAT4","0","FLOAT","1","FLOAT","2","FLOAT","3","FLOAT","4"]}
{"type":"AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor","id":160,"pos":[-1760,-2528],"params":["Inherit","False","676","162.95","Overall","2","159","158","Overall","0,0,0,1","0","0"]}
{"type":"AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor","id":219,"pos":[-480,-2272],"params":["Inherit","False","Tile Rand","-1","True","1","0","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":77,"pos":[-5632,-256],"params":["Inherit","False","Property","_FrPower","Fr Power","22","0","Create","True","0","0","0","False","0","False","Object","-1","","1","1","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":76,"pos":[-5632,-384],"params":["Inherit","False","Property","_FrScale","Fr Scale","21","0","Create","True","0","0","0","False","0","False","Object","-1","","1","1","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":78,"pos":[-5632,-512],"params":["Inherit","False","Property","_FrBias","Fr Bias","23","0","Create","True","0","0","0","False","0","False","Object","-1","","0","0","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":158,"pos":[-1712,-2480],"params":["Inherit","False","Property","_OverallTilesMultiply","Overall Tiles Multiply","4","0","Create","True","0","0","0","False","0","False","Object","-1","","1","1","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor","id":222,"pos":[-1760,-2272],"params":["Inherit","False","219","Tile Rand","1","0","OBJECT","","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.TexCoordVertexDataNode, AmplifyShaderEditor","id":166,"pos":[-736,-2528],"params":["Inherit","False","0","4","0","5","FLOAT4","0","FLOAT","1","FLOAT","2","FLOAT","3","FLOAT","4"]}
{"type":"AmplifyShaderEditor.FresnelNode, AmplifyShaderEditor","id":202,"pos":[-5376,-512],"params":["Inherit","False","Standard","WorldNormal","ViewDir","True","True","5","0","FLOAT3","0,0,1","False","4","FLOAT3","0,0,0","False","1","FLOAT","0","False","2","FLOAT","1","False","3","FLOAT","5","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor","id":221,"pos":[-1504,-2272],"params":["Inherit","False","2","2","0","FLOAT","0","False","1","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.SaturateNode, AmplifyShaderEditor","id":206,"pos":[-5120,-512],"params":["Inherit","False","1","0","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor","id":214,"pos":[-480,-2400],"params":["Inherit","False","LUT Rand","-1","True","1","0","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.WorldPosInputsNode, AmplifyShaderEditor","id":111,"pos":[-8064,1152],"params":["Inherit","False","0","4","FLOAT3","0","FLOAT","1","FLOAT","2","FLOAT","3"]}
{"type":"AmplifyShaderEditor.TransformPositionNode, AmplifyShaderEditor","id":112,"pos":[-8064,1408],"params":["Inherit","False","Object","World","False","Fast","True","1","0","FLOAT3","0,0,0","False","5","FLOAT3","0","FLOAT","1","FLOAT","2","FLOAT","3","FLOAT","4"]}
{"type":"AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor","id":159,"pos":[-1328,-2480],"params":["Inherit","False","overallTilesMult","-1","True","1","0","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":205,"pos":[-5632,-768],"params":["Inherit","False","Property","_LUTOffset","LUT Offset","7","0","Create","True","0","0","0","False","0","False","Object","-1","","0","0","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor","id":217,"pos":[-5632,-640],"params":["Inherit","False","214","LUT Rand","1","0","OBJECT","","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor","id":227,"pos":[-4864,-512],"params":["Inherit","False","CFr","-1","True","1","0","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.Vector3Node, AmplifyShaderEditor","id":122,"pos":[-8064,1664],"params":["Inherit","False","Property","_WPOTile","WPO Tile","28","0","Create","True","0","0","0","False","0","False","Object","-1","","1,1,1","1,1,1","0","4","FLOAT3","0","FLOAT","1","FLOAT","2","FLOAT","3"]}
{"type":"AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor","id":164,"pos":[-7808,1792],"params":["Inherit","False","159","overallTilesMult","1","0","OBJECT","","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.SimpleSubtractOpNode, AmplifyShaderEditor","id":113,"pos":[-7808,1152],"params":["Inherit","False","2","0","FLOAT3","0,0,0","False","1","FLOAT3","0,0,0","False","1","FLOAT3","0"]}
{"type":"AmplifyShaderEditor.SimpleAddOpNode, AmplifyShaderEditor","id":216,"pos":[-5376,-768],"params":["Inherit","False","2","2","0","FLOAT","0","False","1","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor","id":228,"pos":[-5120,-1024],"params":["Inherit","False","227","CFr","1","0","OBJECT","","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor","id":163,"pos":[-7808,1664],"params":["Inherit","False","2","2","0","FLOAT3","0,0,0","False","1","FLOAT","0","False","1","FLOAT3","0"]}
{"type":"AmplifyShaderEditor.StaticSwitch, AmplifyShaderEditor","id":192,"pos":[-7808,1024],"params":["Inherit","False","Property","_UseWorldSpace","Use World Space","5","0","Create","True","0","0","0","False","0","False","","0","0","0","True","","Toggle","2","Key0","Key1","Create","True","True","All","9","1","FLOAT3","0,0,0","False","0","FLOAT3","0,0,0","False","2","FLOAT3","0,0,0","False","3","FLOAT3","0,0,0","False","4","FLOAT3","0,0,0","False","5","FLOAT3","0,0,0","False","6","FLOAT3","0,0,0","False","7","FLOAT3","0,0,0","False","8","FLOAT3","0,0,0","False","1","FLOAT3","0"]}
{"type":"AmplifyShaderEditor.SimpleAddOpNode, AmplifyShaderEditor","id":203,"pos":[-5120,-896],"params":["Inherit","False","2","2","0","FLOAT","0","False","1","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":207,"pos":[-4864,-768],"params":["Inherit","False","Property","_LUTAmplitude","LUT Amplitude","8","0","Create","True","0","0","0","False","0","False","Object","-1","","1","1","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.WorldNormalVector, AmplifyShaderEditor","id":130,"pos":[-5760,896],"params":["Inherit","False","False","1","0","FLOAT3","0,0,1","False","4","FLOAT3","0","FLOAT","1","FLOAT","2","FLOAT","3"]}
{"type":"AmplifyShaderEditor.SimpleTimeNode, AmplifyShaderEditor","id":119,"pos":[-7040,1648],"params":["Inherit","False","1","0","FLOAT","1","False","5","FLOAT","0","FLOAT","1","FLOAT","2","FLOAT","3","FLOAT","4"]}
{"type":"AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor","id":117,"pos":[-7552,1152],"params":["Inherit","False","2","2","0","FLOAT3","0,0,0","False","1","FLOAT3","0,0,0","False","1","FLOAT3","0"]}
{"type":"AmplifyShaderEditor.Vector3Node, AmplifyShaderEditor","id":121,"pos":[-7040,1408],"params":["Inherit","False","Property","_WPOTileSpeed","WPO Tile Speed","29","0","Create","True","0","0","0","False","0","False","Object","-1","","0,1,1","1,1,1","0","4","FLOAT3","0","FLOAT","1","FLOAT","2","FLOAT","3"]}
{"type":"AmplifyShaderEditor.Vector3Node, AmplifyShaderEditor","id":138,"pos":[-7552,1664],"params":["Inherit","False","Property","_WPOTileOffsetCustom","WPO Tile Offset Custom","30","0","Create","True","0","0","0","False","0","False","Object","-1","","0,0,0","0,0,0","0","4","FLOAT3","0","FLOAT","1","FLOAT","2","FLOAT","3"]}
{"type":"AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor","id":204,"pos":[-4864,-896],"params":["Inherit","False","2","2","0","FLOAT","0","False","1","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":209,"pos":[-4608,-768],"params":["Inherit","False","Property","_LUTPanSpeed","LUT Pan Speed","9","0","Create","True","0","0","0","False","0","False","Object","-1","","0","0","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor","id":251,"pos":[-1842,-3122],"params":["Inherit","False","932","418.95","Hue Shift","6","97","98","99","94","95","249","Hue Shift","0,0,0,1","0","0"]}
{"type":"AmplifyShaderEditor.AbsOpNode, AmplifyShaderEditor","id":123,"pos":[-5600,896],"params":["Inherit","False","1","0","FLOAT3","0,0,0","False","1","FLOAT3","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":124,"pos":[-5504,1024],"params":["Inherit","False","Property","_WPOTriplanarContrast","WPO Triplanar Contrast","27","0","Create","True","0","0","0","False","0","False","Object","-1","","1","1","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor","id":120,"pos":[-6784,1648],"params":["Inherit","False","2","2","0","FLOAT","0","False","1","FLOAT3","0,0,0","False","1","FLOAT3","0"]}
{"type":"AmplifyShaderEditor.BreakToComponentsNode, AmplifyShaderEditor","id":129,"pos":[-5248,1024],"params":["Inherit","False","FLOAT3","1","0","FLOAT3","0,0,0","False","16","FLOAT","0","FLOAT","1","FLOAT","2","FLOAT","3","FLOAT","4","FLOAT","5","FLOAT","6","FLOAT","7","FLOAT","8","FLOAT","9","FLOAT","10","FLOAT","11","FLOAT","12","FLOAT","13","FLOAT","14","FLOAT","15"]}
{"type":"AmplifyShaderEditor.SimpleAddOpNode, AmplifyShaderEditor","id":136,"pos":[-7296,1152],"params":["Inherit","False","2","2","0","FLOAT3","0,0,0","False","1","FLOAT3","0,0,0","False","1","FLOAT3","0"]}
{"type":"AmplifyShaderEditor.PannerNode, AmplifyShaderEditor","id":208,"pos":[-4608,-896],"params":["Inherit","False","3","0","FLOAT2","0,0","False","2","FLOAT2","0,0","False","1","FLOAT","1","False","1","FLOAT2","0"]}
{"type":"AmplifyShaderEditor.PowerNode, AmplifyShaderEditor","id":125,"pos":[-5504,896],"params":["Inherit","False","False","2","0","FLOAT3","0,0,0","False","1","FLOAT","1","False","1","FLOAT3","0"]}
{"type":"AmplifyShaderEditor.SimpleAddOpNode, AmplifyShaderEditor","id":118,"pos":[-6784,1152],"params":["Inherit","False","2","2","0","FLOAT3","0,0,0","False","1","FLOAT3","0,0,0","False","1","FLOAT3","0"]}
{"type":"AmplifyShaderEditor.SimpleAddOpNode, AmplifyShaderEditor","id":128,"pos":[-5120,1024],"params":["Inherit","False","3","3","0","FLOAT","0","False","1","FLOAT","0","False","2","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":188,"pos":[-3840,-768],"params":["Inherit","False","Property","_LUTDesaturate","LUT Desaturate","10","0","Create","True","0","0","0","False","0","False","Object","-1","","0","0","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.SamplerNode, AmplifyShaderEditor","id":199,"pos":[-4224,-896],"params":["Inherit","True","Property","_LUT","LUT","6","0","Create","True","0","0","0","False","3","Space(33)","Header(LUT)","Space(13)","False","","-1","8db0b8e5b6c595f45b120a625e4d68e2","8db0b8e5b6c595f45b120a625e4d68e2","True","0","False","white","Auto","False","Object","-1","Auto","Texture2D","False","8","0","SAMPLER2D","","False","1","FLOAT2","0,0","False","2","FLOAT","0","False","3","FLOAT2","0,0","False","4","FLOAT2","0,0","False","5","FLOAT","1","False","6","FLOAT","0","False","7","SAMPLERSTATE","","False","6","COLOR","0","FLOAT","1","FLOAT","2","FLOAT","3","FLOAT","4","FLOAT3","5"]}
{"type":"AmplifyShaderEditor.SimpleTimeNode, AmplifyShaderEditor","id":97,"pos":[-1792,-2944],"params":["Inherit","False","1","0","FLOAT","1","False","5","FLOAT","0","FLOAT","1","FLOAT","2","FLOAT","3","FLOAT","4"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":99,"pos":[-1536,-2816],"params":["Inherit","False","Property","_HueShiftSpeed","Hue Shift Speed","13","0","Create","True","0","0","0","False","0","False","Object","-1","","0","0","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.ComponentMaskNode, AmplifyShaderEditor","id":114,"pos":[-6528,1152],"params":["Inherit","False","True","True","False","False","1","0","FLOAT3","0,0,0","False","1","FLOAT2","0"]}
{"type":"AmplifyShaderEditor.ComponentMaskNode, AmplifyShaderEditor","id":115,"pos":[-6528,1280],"params":["Inherit","False","False","True","True","False","1","0","FLOAT3","0,0,0","False","1","FLOAT2","0"]}
{"type":"AmplifyShaderEditor.ComponentMaskNode, AmplifyShaderEditor","id":116,"pos":[-6528,1408],"params":["Inherit","False","True","False","True","False","1","0","FLOAT3","0,0,0","False","1","FLOAT2","0"]}
{"type":"AmplifyShaderEditor.SimpleDivideOpNode, AmplifyShaderEditor","id":126,"pos":[-4992,896],"params":["Inherit","False","2","0","FLOAT3","0,0,0","False","1","FLOAT","0","False","1","FLOAT3","0"]}
{"type":"AmplifyShaderEditor.TexturePropertyNode, AmplifyShaderEditor","id":110,"pos":[-6528,896],"params":["Inherit","True","Property","_WPOTriplanarTexture","WPO Triplanar Texture","25","0","Create","True","0","0","0","False","3","Space(33)","Header(WPO Triplanar)","Space(13)","False","","edba5d18294de3a4daddbf6d71ac0849","edba5d18294de3a4daddbf6d71ac0849","False","white","Auto","Texture2D","False","-1","0","2","SAMPLER2D","0","SAMPLERSTATE","1"]}
{"type":"AmplifyShaderEditor.DesaturateOpNode, AmplifyShaderEditor","id":187,"pos":[-3840,-896],"params":["Inherit","False","2","0","FLOAT3","0,0,0","False","1","FLOAT","0","False","1","FLOAT3","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":245,"pos":[-5632,0],"params":["Inherit","False","Property","_FrCenterScale","Fr Center Scale","16","0","Create","True","0","0","0","False","0","False","Object","-1","","1","1","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":244,"pos":[-5632,128],"params":["Inherit","False","Property","_FrCenterPower","Fr Center Power","17","0","Create","True","0","0","0","False","0","False","Object","-1","","1","1","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":246,"pos":[-5632,-128],"params":["Inherit","False","Property","_FrCenterBias","Fr Center Bias","18","0","Create","True","0","0","0","False","0","False","Object","-1","","0","0","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor","id":98,"pos":[-1536,-2944],"params":["Inherit","False","2","2","0","FLOAT","0","False","1","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":94,"pos":[-1536,-3072],"params":["Inherit","False","Property","_HueShift","Hue Shift","12","0","Create","True","0","0","0","False","0","False","Object","-1","","0","0","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.ColorNode, AmplifyShaderEditor","id":190,"pos":[-3584,-768],"params":["Inherit","False","Property","_LUTTint","LUT Tint","11","0","Create","True","0","0","0","False","0","False","Object","-1","","1,1,1,0","1,1,1,0","True","True","0","6","COLOR","0","FLOAT","1","FLOAT","2","FLOAT","3","FLOAT","4","FLOAT3","5"]}
{"type":"AmplifyShaderEditor.SamplerNode, AmplifyShaderEditor","id":107,"pos":[-6144,1152],"params":["Inherit","True","Property","_TextureSample3","Texture Sample 0","7","0","Create","True","0","0","0","False","0","False","","-1","None","None","True","0","False","white","Auto","False","Instance","-1","Auto","Texture2D","False","8","0","SAMPLER2D","","False","1","FLOAT2","0,0","False","2","FLOAT","0","False","3","FLOAT2","0,0","False","4","FLOAT2","0,0","False","5","FLOAT","1","False","6","FLOAT","0","False","7","SAMPLERSTATE","","False","6","COLOR","0","FLOAT","1","FLOAT","2","FLOAT","3","FLOAT","4","FLOAT3","5"]}
{"type":"AmplifyShaderEditor.SamplerNode, AmplifyShaderEditor","id":108,"pos":[-6144,1408],"params":["Inherit","True","Property","_TextureSample4","Texture Sample 0","7","0","Create","True","0","0","0","False","0","False","","-1","None","None","True","0","False","white","Auto","False","Instance","-1","Auto","Texture2D","False","8","0","SAMPLER2D","","False","1","FLOAT2","0,0","False","2","FLOAT","0","False","3","FLOAT2","0,0","False","4","FLOAT2","0,0","False","5","FLOAT","1","False","6","FLOAT","0","False","7","SAMPLERSTATE","","False","6","COLOR","0","FLOAT","1","FLOAT","2","FLOAT","3","FLOAT","4","FLOAT3","5"]}
{"type":"AmplifyShaderEditor.SamplerNode, AmplifyShaderEditor","id":109,"pos":[-6144,1664],"params":["Inherit","True","Property","_TextureSample5","Texture Sample 0","7","0","Create","True","0","0","0","False","0","False","","-1","None","None","True","0","False","white","Auto","False","Instance","-1","Auto","Texture2D","False","8","0","SAMPLER2D","","False","1","FLOAT2","0,0","False","2","FLOAT","0","False","3","FLOAT2","0,0","False","4","FLOAT2","0,0","False","5","FLOAT","1","False","6","FLOAT","0","False","7","SAMPLERSTATE","","False","6","COLOR","0","FLOAT","1","FLOAT","2","FLOAT","3","FLOAT","4","FLOAT3","5"]}
{"type":"AmplifyShaderEditor.BreakToComponentsNode, AmplifyShaderEditor","id":127,"pos":[-4736,896],"params":["Inherit","False","FLOAT3","1","0","FLOAT3","0,0,0","False","16","FLOAT","0","FLOAT","1","FLOAT","2","FLOAT","3","FLOAT","4","FLOAT","5","FLOAT","6","FLOAT","7","FLOAT","8","FLOAT","9","FLOAT","10","FLOAT","11","FLOAT","12","FLOAT","13","FLOAT","14","FLOAT","15"]}
{"type":"AmplifyShaderEditor.FresnelNode, AmplifyShaderEditor","id":242,"pos":[-5376,-128],"params":["Inherit","False","Standard","WorldNormal","ViewDir","True","True","5","0","FLOAT3","0,0,1","False","4","FLOAT3","0,0,0","False","1","FLOAT","0","False","2","FLOAT","1","False","3","FLOAT","5","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.SimpleAddOpNode, AmplifyShaderEditor","id":95,"pos":[-1280,-3072],"params":["Inherit","False","2","2","0","FLOAT","0","False","1","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor","id":189,"pos":[-3584,-896],"params":["Inherit","False","2","2","0","FLOAT3","0,0,0","False","1","COLOR","0,0,0,0","False","1","COLOR","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":236,"pos":[-3840,-1408],"params":["Inherit","False","Property","_FrSolidColorActive","Fr Solid Color Active","20","0","Create","True","0","0","0","False","0","False","Object","-1","","0","0","0","1","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.ColorNode, AmplifyShaderEditor","id":85,"pos":[-3840,-1280],"params":["Inherit","False","Property","_FrSolidColor","Fr Solid Color","19","0","Create","True","0","0","0","False","3","Space(33)","Header(Fresnel)","Space(13)","False","Object","-1","","1,1,1,0","1,1,1,0","True","True","0","6","COLOR","0","FLOAT","1","FLOAT","2","FLOAT","3","FLOAT","4","FLOAT3","5"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":169,"pos":[-2560,2432],"params":["Inherit","False","Property","_OpacityFrBias","Opacity Fr Bias","36","0","Create","True","0","0","0","False","0","False","Object","-1","","0","0","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":167,"pos":[-2560,2688],"params":["Inherit","False","Property","_OpacityFrPower","Opacity Fr Power","35","0","Create","True","0","0","0","False","0","False","Object","-1","","1","1","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":168,"pos":[-2560,2560],"params":["Inherit","False","Property","_OpacityFrScale","Opacity Fr Scale","34","0","Create","True","0","0","0","False","0","False","Object","-1","","1","1","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor","id":103,"pos":[-4736,1408],"params":["Inherit","False","2","2","0","COLOR","0,0,0,0","False","1","FLOAT","0","False","1","COLOR","0"]}
{"type":"AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor","id":104,"pos":[-4736,1152],"params":["Inherit","False","2","2","0","COLOR","0,0,0,0","False","1","FLOAT","0","False","1","COLOR","0"]}
{"type":"AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor","id":105,"pos":[-4736,1664],"params":["Inherit","False","2","2","0","COLOR","0,0,0,0","False","1","FLOAT","0","False","1","COLOR","0"]}
{"type":"AmplifyShaderEditor.SaturateNode, AmplifyShaderEditor","id":243,"pos":[-5120,-128],"params":["Inherit","False","1","0","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.VertexColorNode, AmplifyShaderEditor","id":223,"pos":[-3840,-2304],"params":["Inherit","False","0","5","COLOR","0","FLOAT","1","FLOAT","2","FLOAT","3","FLOAT","4"]}
{"type":"AmplifyShaderEditor.LerpOp, AmplifyShaderEditor","id":235,"pos":[-3456,-1280],"params":["Inherit","True","3","0","COLOR","0,0,0,0","False","1","COLOR","0,0,0,0","False","2","FLOAT","0","False","1","COLOR","0"]}
{"type":"AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor","id":249,"pos":[-1152,-3072],"params":["Inherit","False","HueShift","-1","True","1","0","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.FresnelNode, AmplifyShaderEditor","id":170,"pos":[-2176,2432],"params":["Inherit","False","Standard","WorldNormal","ViewDir","True","True","5","0","FLOAT3","0,0,1","False","4","FLOAT3","0,0,0","False","1","FLOAT","0","False","2","FLOAT","1","False","3","FLOAT","5","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":172,"pos":[-2176,2176],"params":["Inherit","False","Property","_OpacityMin","Opacity Min","32","0","Create","True","0","0","0","False","0","False","Object","-1","","0","0","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":173,"pos":[-2176,2304],"params":["Inherit","False","Property","_OpacityMax","Opacity Max","33","0","Create","True","0","0","0","False","0","False","Object","-1","","1","1","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.SimpleAddOpNode, AmplifyShaderEditor","id":106,"pos":[-4480,1152],"params":["Inherit","False","3","3","0","COLOR","0,0,0,0","False","1","COLOR","0,0,0,0","False","2","COLOR","0,0,0,0","False","1","COLOR","0"]}
{"type":"AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor","id":212,"pos":[-480,-2528],"params":["Inherit","False","WPO Rand","-1","True","1","0","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor","id":247,"pos":[-4864,-128],"params":["Inherit","False","CenFr","-1","True","1","0","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.ComponentMaskNode, AmplifyShaderEditor","id":225,"pos":[-3584,-2304],"params":["Inherit","False","True","True","True","False","1","0","COLOR","0,0,0,0","False","1","FLOAT3","0"]}
{"type":"AmplifyShaderEditor.RGBToHSVNode, AmplifyShaderEditor","id":89,"pos":[-3200,-1280],"params":["Inherit","False","1","0","FLOAT3","0,0,0","False","4","FLOAT3","0","FLOAT","1","FLOAT","2","FLOAT","3"]}
{"type":"AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor","id":250,"pos":[-2944,-1408],"params":["Inherit","False","249","HueShift","1","0","OBJECT","","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.ColorNode, AmplifyShaderEditor","id":238,"pos":[-3840,-1920],"params":["Inherit","False","Property","_CenterColor","Center Color","14","0","Create","True","0","0","0","False","3","Space(33)","Header(Center Color)","Space(13)","False","Object","-1","","1,1,1,0","1,1,1,0","True","True","0","6","COLOR","0","FLOAT","1","FLOAT","2","FLOAT","3","FLOAT","4","FLOAT3","5"]}
{"type":"AmplifyShaderEditor.LerpOp, AmplifyShaderEditor","id":171,"pos":[-1792,2176],"params":["Inherit","False","3","0","FLOAT","0","False","1","FLOAT","0","False","2","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.SaturateNode, AmplifyShaderEditor","id":131,"pos":[-4224,1152],"params":["Inherit","True","1","0","COLOR","0,0,0,0","False","1","COLOR","0"]}
{"type":"AmplifyShaderEditor.NormalVertexDataNode, AmplifyShaderEditor","id":156,"pos":[-2320,1280],"params":["Inherit","False","0","5","FLOAT3","0","FLOAT","1","FLOAT","2","FLOAT","3","FLOAT","4"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":102,"pos":[-1792,1536],"params":["Inherit","False","Property","_WPOIntensity","WPO Intensity","26","0","Create","True","0","0","0","False","0","False","Object","-1","","0.1","0","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor","id":213,"pos":[-1792,1664],"params":["Inherit","False","212","WPO Rand","1","0","OBJECT","","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":80,"pos":[-2304,384],"params":["Inherit","False","Property","_EmissionMultiply","Emission Multiply","1","0","Create","True","0","0","0","False","3","Space(13)","Header(General)","Space(13)","False","Object","-1","","0","0","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":232,"pos":[-3072,256],"params":["Inherit","False","Property","_FrEmissionMultiply","Fr Emission Multiply","24","0","Create","True","0","0","0","False","0","False","Object","-1","","0","0","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor","id":241,"pos":[-3840,-1664],"params":["Inherit","False","247","CenFr","1","0","OBJECT","","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.SimpleAddOpNode, AmplifyShaderEditor","id":90,"pos":[-2944,-1280],"params":["Inherit","False","2","2","0","FLOAT","0","False","1","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor","id":252,"pos":[-3328,-2304],"params":["Inherit","False","2","2","0","FLOAT3","0,0,0","False","1","FLOAT3","0,0,0","False","1","FLOAT3","0"]}
{"type":"AmplifyShaderEditor.SaturateNode, AmplifyShaderEditor","id":175,"pos":[-1536,2176],"params":["Inherit","False","1","0","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":177,"pos":[-1280,2304],"params":["Inherit","False","Property","_OpacityOverall","Opacity Overall","31","0","Create","True","0","0","0","False","3","Space(33)","Header(Opacity)","Space(13)","False","Object","-1","","1","1","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor","id":133,"pos":[-2048,1280],"params":["Inherit","False","2","2","0","FLOAT3","0,0,0","False","1","COLOR","0,0,0,0","False","1","COLOR","0"]}
{"type":"AmplifyShaderEditor.Vector3Node, AmplifyShaderEditor","id":101,"pos":[-1792,1280],"params":["Inherit","False","Constant","_Vector0","Vector 0","15","0","Create","True","0","0","0","False","0","False","Object","-1","","0,0,0","0,0,0","0","4","FLOAT3","0","FLOAT","1","FLOAT","2","FLOAT","3"]}
{"type":"AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor","id":165,"pos":[-1536,1536],"params":["Inherit","False","2","2","0","FLOAT","0","False","1","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor","id":79,"pos":[-2304,256],"params":["Inherit","False","2","2","0","FLOAT3","0,0,0","False","1","FLOAT","0","False","1","FLOAT3","0"]}
{"type":"AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor","id":234,"pos":[-1920,384],"params":["Inherit","False","227","CFr","1","0","OBJECT","","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor","id":231,"pos":[-3072,128],"params":["Inherit","False","2","2","0","COLOR","0,0,0,0","False","1","FLOAT","0","False","1","COLOR","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":240,"pos":[-3840,-2048],"params":["Inherit","False","Property","_CenterActive","Center Active","15","0","Create","True","0","0","0","False","0","False","Object","-1","","0","1","0","1","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.HSVToRGBNode, AmplifyShaderEditor","id":88,"pos":[-2816,-1280],"params":["Inherit","False","3","0","FLOAT","0","False","1","FLOAT","0","False","2","FLOAT","0","False","4","FLOAT3","0","FLOAT","1","FLOAT","2","FLOAT","3"]}
{"type":"AmplifyShaderEditor.LerpOp, AmplifyShaderEditor","id":237,"pos":[-3200,-1920],"params":["Inherit","False","3","0","FLOAT3","0,0,0","False","1","FLOAT3","0,0,0","False","2","FLOAT","0","False","1","FLOAT3","0"]}
{"type":"AmplifyShaderEditor.SimpleMultiplyOpNode, AmplifyShaderEditor","id":178,"pos":[-1152,2176],"params":["Inherit","False","2","2","0","FLOAT","0","False","1","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.LerpOp, AmplifyShaderEditor","id":100,"pos":[-1536,1280],"params":["Inherit","False","3","0","COLOR","0,0,0,0","False","1","COLOR","0,0,0,0","False","2","FLOAT","0","False","1","COLOR","0"]}
{"type":"AmplifyShaderEditor.LerpOp, AmplifyShaderEditor","id":233,"pos":[-1920,256],"params":["Inherit","False","3","0","COLOR","0,0,0,0","False","1","COLOR","0,0,0,0","False","2","FLOAT","0","False","1","COLOR","0"]}
{"type":"AmplifyShaderEditor.LerpOp, AmplifyShaderEditor","id":239,"pos":[-2816,-2048],"params":["Inherit","True","3","0","FLOAT3","0,0,0","False","1","FLOAT3","0,0,0","False","2","FLOAT","0","False","1","FLOAT3","0"]}
{"type":"AmplifyShaderEditor.SaturateNode, AmplifyShaderEditor","id":176,"pos":[-896,2176],"params":["Inherit","False","1","0","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.CommentaryNode, AmplifyShaderEditor","id":186,"pos":[432,-16],"params":["Inherit","False","1252","162.95","Ge Lush was here! <3","5","182","183","184","185","181","Ge Lush was here! <3","0.3782746,0.2798741,1,1","0","0"]}
{"type":"AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor","id":193,"pos":[-1280,1280],"params":["Inherit","False","VO","-1","True","1","0","COLOR","0,0,0,0","False","1","COLOR","0"]}
{"type":"AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor","id":197,"pos":[-1536,-896],"params":["Inherit","False","BC","-1","True","1","0","FLOAT3","0,0,0","False","1","FLOAT3","0"]}
{"type":"AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor","id":195,"pos":[-1536,256],"params":["Inherit","False","EM","-1","True","1","0","COLOR","0,0,0,0","False","1","COLOR","0"]}
{"type":"AmplifyShaderEditor.RegisterLocalVarNode, AmplifyShaderEditor","id":179,"pos":[-640,2176],"params":["Inherit","False","op","-1","True","1","0","FLOAT","0","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":182,"pos":[736,32],"params":["Inherit","False","Property","_Src","Src","38","0","Create","True","0","0","0","True","0","False","Object","-1","","5","5","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":183,"pos":[992,32],"params":["Inherit","False","Property","_Dst","Dst","39","0","Create","True","0","0","0","True","0","False","Object","-1","","10","10","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":184,"pos":[1248,32],"params":["Inherit","False","Property","_ZWrite","ZWrite","40","0","Create","True","0","0","0","True","0","False","Object","-1","","0","0","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":185,"pos":[1504,32],"params":["Inherit","False","Property","_ZTest","ZTest","41","0","Create","True","0","0","0","True","0","False","Object","-1","","2","2","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":181,"pos":[480,32],"params":["Inherit","False","Property","_Cull","Cull","37","0","Create","True","0","0","0","True","3","Space(33)","Header(AR)","Space(13)","False","Object","-1","","2","2","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor","id":180,"pos":[-640,384],"params":["Inherit","False","179","op","1","0","OBJECT","","False","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor","id":194,"pos":[-640,640],"params":["Inherit","False","193","VO","1","0","OBJECT","","False","1","COLOR","0"]}
{"type":"AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor","id":196,"pos":[-640,512],"params":["Inherit","False","195","EM","1","0","OBJECT","","False","1","COLOR","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":12,"pos":[-640,256],"params":["Inherit","False","Property","_Smoothness","Smoothness","3","0","Create","True","0","0","0","False","0","False","Object","-1","","0.99","0.99","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.RangedFloatNode, AmplifyShaderEditor","id":11,"pos":[-640,128],"params":["Inherit","False","Property","_SpeCular","SpeCular","2","0","Create","True","0","0","0","False","0","False","Object","-1","","0.01","0.01","0","0","0","1","FLOAT","0"]}
{"type":"AmplifyShaderEditor.GetLocalVarNode, AmplifyShaderEditor","id":198,"pos":[-640,0],"params":["Inherit","False","197","BC","1","0","OBJECT","","False","1","FLOAT3","0"]}
{"type":"AmplifyShaderEditor.StandardSurfaceOutputNode, AmplifyShaderEditor","id":253,"pos":[0,0],"params":["Float","False","True","-1","3","AmplifyShaderEditor.MaterialInspector","0","0","StandardSpecular","Vefects/SH_Vefects_URP_VFX_Bubbles_Particles_Transparent_BIRP","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","False","Back","0","True","_ZWrite","0","True","_ZTest","False","0","False","","0","False","","False","0","0","False","","0","Custom","0.5","True","False","0","True","Transparent","","Transparent","All","12","all","True","True","True","True","0","False","","False","0","False","","255","False","","255","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","0","False","","False","2","15","10","25","False","0.5","False","1","5","True","_Src","10","True","_Dst","0","0","False","","0","False","","0","False","","0","False","","0","False","0","0,0,0,0","VertexOffset","True","False","Cylindrical","False","True","Relative","0","","0","-1","-1","-1","0","False","0","0","True","_Cull","-1","0","False","","0","0","0","False","0.1","False","","0","False","","False","17","0","FLOAT3","0,0,0","False","1","FLOAT3","0,0,0","False","2","FLOAT3","0,0,0","False","3","FLOAT3","0,0,0","False","4","FLOAT","0","False","5","FLOAT","0","False","6","FLOAT3","0,0,0","False","7","FLOAT3","0,0,0","False","8","FLOAT","0","False","9","FLOAT","0","False","10","FLOAT","0","False","13","FLOAT3","0,0,0","False","11","FLOAT3","0,0,0","False","12","FLOAT3","0,0,0","False","16","FLOAT4","0,0,0,0","False","14","FLOAT4","0,0,0,0","False","15","FLOAT3","0,0,0","False","0"]}
{"wire":[219,0,218,1]}
{"wire":[202,1,78,0]}
{"wire":[202,2,76,0]}
{"wire":[202,3,77,0]}
{"wire":[221,0,158,0]}
{"wire":[221,1,222,0]}
{"wire":[206,0,202,0]}
{"wire":[214,0,166,4]}
{"wire":[159,0,221,0]}
{"wire":[227,0,206,0]}
{"wire":[113,0,111,0]}
{"wire":[113,1,112,0]}
{"wire":[216,0,205,0]}
{"wire":[216,1,217,0]}
{"wire":[163,0,122,0]}
{"wire":[163,1,164,0]}
{"wire":[192,1,113,0]}
{"wire":[192,0,111,0]}
{"wire":[203,0,228,0]}
{"wire":[203,1,216,0]}
{"wire":[117,0,192,0]}
{"wire":[117,1,163,0]}
{"wire":[204,0,203,0]}
{"wire":[204,1,207,0]}
{"wire":[123,0,130,0]}
{"wire":[120,0,119,0]}
{"wire":[120,1,121,0]}
{"wire":[129,0,125,0]}
{"wire":[136,0,117,0]}
{"wire":[136,1,138,0]}
{"wire":[208,0,204,0]}
{"wire":[208,2,209,0]}
{"wire":[125,0,123,0]}
{"wire":[125,1,124,0]}
{"wire":[118,0,136,0]}
{"wire":[118,1,120,0]}
{"wire":[128,0,129,0]}
{"wire":[128,1,129,1]}
{"wire":[128,2,129,2]}
{"wire":[199,1,208,0]}
{"wire":[114,0,118,0]}
{"wire":[115,0,118,0]}
{"wire":[116,0,118,0]}
{"wire":[126,0,125,0]}
{"wire":[126,1,128,0]}
{"wire":[187,0,199,5]}
{"wire":[187,1,188,0]}
{"wire":[98,0,97,0]}
{"wire":[98,1,99,0]}
{"wire":[107,0,110,0]}
{"wire":[107,1,114,0]}
{"wire":[108,0,110,0]}
{"wire":[108,1,115,0]}
{"wire":[109,0,110,0]}
{"wire":[109,1,116,0]}
{"wire":[127,0,126,0]}
{"wire":[242,1,246,0]}
{"wire":[242,2,245,0]}
{"wire":[242,3,244,0]}
{"wire":[95,0,94,0]}
{"wire":[95,1,98,0]}
{"wire":[189,0,187,0]}
{"wire":[189,1,190,0]}
{"wire":[103,0,108,0]}
{"wire":[103,1,127,0]}
{"wire":[104,0,107,0]}
{"wire":[104,1,127,2]}
{"wire":[105,0,109,0]}
{"wire":[105,1,127,1]}
{"wire":[243,0,242,0]}
{"wire":[235,0,189,0]}
{"wire":[235,1,85,0]}
{"wire":[235,2,236,0]}
{"wire":[249,0,95,0]}
{"wire":[170,1,169,0]}
{"wire":[170,2,168,0]}
{"wire":[170,3,167,0]}
{"wire":[106,0,104,0]}
{"wire":[106,1,103,0]}
{"wire":[106,2,105,0]}
{"wire":[212,0,166,3]}
{"wire":[247,0,243,0]}
{"wire":[225,0,223,0]}
{"wire":[89,0,235,0]}
{"wire":[171,0,172,0]}
{"wire":[171,1,173,0]}
{"wire":[171,2,170,0]}
{"wire":[131,0,106,0]}
{"wire":[90,0,89,1]}
{"wire":[90,1,250,0]}
{"wire":[252,0,225,0]}
{"wire":[252,1,238,5]}
{"wire":[175,0,171,0]}
{"wire":[133,0,156,0]}
{"wire":[133,1,131,0]}
{"wire":[165,0,102,0]}
{"wire":[165,1,213,0]}
{"wire":[79,0,239,0]}
{"wire":[79,1,80,0]}
{"wire":[231,0,235,0]}
{"wire":[231,1,232,0]}
{"wire":[88,0,90,0]}
{"wire":[88,1,89,2]}
{"wire":[88,2,89,3]}
{"wire":[237,0,252,0]}
{"wire":[237,1,88,0]}
{"wire":[237,2,241,0]}
{"wire":[178,0,175,0]}
{"wire":[178,1,177,0]}
{"wire":[100,0,101,0]}
{"wire":[100,1,133,0]}
{"wire":[100,2,165,0]}
{"wire":[233,0,79,0]}
{"wire":[233,1,231,0]}
{"wire":[233,2,234,0]}
{"wire":[239,0,88,0]}
{"wire":[239,1,237,0]}
{"wire":[239,2,240,0]}
{"wire":[176,0,178,0]}
{"wire":[193,0,100,0]}
{"wire":[197,0,239,0]}
{"wire":[195,0,233,0]}
{"wire":[179,0,176,0]}
{"wire":[253,0,198,0]}
{"wire":[253,2,196,0]}
{"wire":[253,3,11,0]}
{"wire":[253,4,12,0]}
{"wire":[253,9,180,0]}
{"wire":[253,11,194,0]}
ASEEND*/
//CHKSM=DA4ABCA5A01884A6606F1D8EC917FF917FC28160