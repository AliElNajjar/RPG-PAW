Shader "SOTN Custom/ScreenMelt" 
{
	Properties 
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}		
		_MeltSpeed ("Melt Speed", range (0,0.2)) = 0			//used in helper C# script to set _Timer
		[HideInInspector] _Timer ("Timer", float) = 0
	}

	SubShader 
	{
		Cull off
		Zwrite off

		CGPROGRAM
		#pragma surface surf Standard
		#pragma target 3.0		

		fixed4 _Color;
		sampler2D _MainTex;		
		float _MeltSpeed;
		float _Timer;			

		struct Input 
		{
			float2 uv_MainTex;
		};

		float2 _Offset[256];			//set by helper C# script; value is varied slightly so that each bar moves down a different amount

		void surf (Input IN, inout SurfaceOutputStandard o)
		{
			IN.uv_MainTex.y += _Timer *_Offset[round(IN.uv_MainTex.x*256.0f)].x;    //moves pixel down by an amount setup in the _Offset array based on current
																					 //column (x value) and the amount of time in _timer that has gone by
			if (IN.uv_MainTex.y > 1) discard;                     //(ONLY NEW LINE) gets rid of bug where pixel at top of clamp texture stretches
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;			
			o.Emission = c.rgb;
			o.Albedo = 0;
			o.Alpha = c.a;
			if (o.Alpha < 0.01) discard;
		}

		ENDCG
	}
}
