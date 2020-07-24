Shader "Unlit/Gradient"
{
    Properties
    {
        _MainColor ("Main Color", Color) = (1,1,1,1)
        _GradientColor ("Gradient Color", Color) = (1,1,1,1)
        _Offset ("Gradient Offset", Range(-1,1)) = 0
        _Pattern ("Pattern", 2D) = "white" {}
        _ScrollSpeed("Scroll Speed", Range(-1,1)) = 0
        _PatternSaturation("Pattern Saturation", Range(0,1)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 pos : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 texcoord : TEXCOORD0;
            };

            float4 _MainColor;
            float4 _GradientColor;
            float _Offset;
            sampler2D _Pattern; 
            float4 _Pattern_ST;
            float _ScrollSpeed;
            float _PatternSaturation;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.pos);
                o.texcoord = v.texcoord;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float uv = i.texcoord;

                fixed4 col = {0,0,0,0};
                if(tex2D(_Pattern, frac(i.texcoord * _Pattern_ST.xy + _Time * _ScrollSpeed)).a > 0)
                    col = lerp(_MainColor, _GradientColor, i.texcoord.y + _Offset)* _PatternSaturation * 0.5;
                else
                    col = lerp(_GradientColor, _MainColor, i.texcoord.y + _Offset);

                return col;
            }
            ENDCG
        }
    }
}
