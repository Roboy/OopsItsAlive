Shader "Custom/Composite"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			struct vIN
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct vOUT
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			sampler2D _MainTex;
			sampler2D _DistortionTex;

			vOUT vert(vIN v)
			{
				vOUT o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.texcoord;
				return o;
			}

			fixed4 frag(vOUT i) : COLOR
			{
				fixed4 distort = tex2D(_DistortionTex, i.uv);
				fixed2 uvdistort;
				if (distort.g > 0 || distort.b > 0 || distort.r > 0) 
				{
					uvdistort = fixed2(sin(i.uv.y * 250)*0.0025, sin(i.uv.x * 250)*0.0025);
				}
				else
				{
					uvdistort = fixed2(0, 0);
				}
				fixed4 tex = tex2D(_MainTex, fixed2(i.uv.xy + uvdistort));
				return tex;
			}
			ENDCG
		}
	}
}
