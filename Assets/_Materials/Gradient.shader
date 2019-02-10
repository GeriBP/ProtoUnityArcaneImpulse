Shader "Custom/Gradient" {
Properties {
	_MainTex ("Texture", 2D) = "white" {}
    _Color ("Bottom Color", Color) = (1,1,1,1)
    _Color2 ("Top Color", Color) = (1,1,1,1)
}
 
SubShader {
    Tags {"Queue"="Background"  "IgnoreProjector"="True"}
 
    Pass {
        CGPROGRAM
        #pragma vertex vert  
        #pragma fragment frag
        #include "UnityCG.cginc"
		
		 struct appdata
		{
			float4 vertex : POSITION;
			float2 uv : TEXCOORD0;
		};

        struct v2f {
            float4 pos : SV_POSITION;
            fixed4 col : COLOR;
        };

		fixed4 _Color;
        fixed4 _Color2;

        v2f vert (appdata_full v)
        {
            v2f o;
            o.pos = UnityObjectToClipPos (v.vertex);
            o.col = lerp(_Color,_Color2, v.texcoord.y);
            return o;
        }
       
        float4 frag (v2f i) : COLOR {
            return i.col;
        }
            ENDCG
        }
    }
}