Shader "UI/Gradient"
{
    Properties
    {
        _Color1 ("Left Color", Color) = (0.67, 0.83, 0.87, 1) // Bleu (#ADD3DE)
        _Color2 ("Right Color", Color) = (0, 0, 0, 1) // Noir (#000000)
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Pass
        {
            ZWrite On // Active le ZWrite pour gérer la profondeur correctement
            Blend SrcAlpha OneMinusSrcAlpha
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            fixed4 _Color1;
            fixed4 _Color2;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return lerp(_Color1, _Color2, i.uv.x); // Dégradé horizontal (gauche → droite)
            }
            ENDCG
        }
    }
}



/*Shader "UI/Gradient"
{
    Properties
    {
        _Color1 ("Top Color", Color) = (0.67, 0.83, 0.87, 1) // Bleu (#ADD3DE)
        _Color2 ("Bottom Color", Color) = (0, 0, 0, 1) // Blanc (#FFFFFF)
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Overlay" }
        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            fixed4 _Color1;
            fixed4 _Color2;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return lerp(_Color1, _Color2, i.uv.x);//return lerp(_Color1, _Color2, i.uv.y); //

            }
            ENDCG
        }
    }
}*/
