// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Custom/Metaball"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _PlantTex ("Plant Texture", 2D) = "white" {}
        _DistortionTex ("Distortion Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
		_borderHardness ("Border Hardness", Float) = 10.0
		_borderHardness2 ("Border Hardness 2", Float) = 10.0
		_border2Offset ("Border 2 Offset", Float) = 1.0
		_distortionFactor ("Distortion Factor", Range(0, 1)) = 1.0
		_texDistortion ("Tex Distortion Multiplier", Range(0, 10)) = 1.0
	
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "Default"
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile __ UNITY_UI_CLIP_RECT
            #pragma multi_compile __ UNITY_UI_ALPHACLIP

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                float2 texcoord2 : TEXCOORD1;
                float2 texcoord3 : TEXCOORD2;
				uint id : SV_VertexID;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
                float2 texcoord2  : TEXCOORD1;
                float2 texcoord3  : TEXCOORD2;
                //float4 worldPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            sampler2D _PlantTex;
			sampler2D _DistortionTex;
            fixed4 _Color;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float4 _MainTex_ST;
            float4 _PlantTex_ST;
            float4 _DistortionTex_ST;
			float _borderHardness;
			float _borderHardness2;
			float _border2Offset;
			float _distortionFactor;
			float _texDistortion;

            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                //OUT.worldPosition = v.vertex;

                OUT.vertex = UnityObjectToClipPos(v.vertex);

                OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                OUT.texcoord2 = TRANSFORM_TEX(v.texcoord2, _PlantTex);
                OUT.texcoord3 = TRANSFORM_TEX(v.texcoord3, _DistortionTex);

                OUT.color = v.color * _Color;

                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
				half2 distortion = _distortionFactor * tex2D(_DistortionTex, IN.texcoord3);

                half4 color = (tex2D(_PlantTex, IN.texcoord2 + _texDistortion * distortion) + _TextureSampleAdd) * IN.color;

				color.a = clamp(_borderHardness * tex2D(_MainTex, IN.texcoord + distortion).r - 0.5 * _borderHardness + 0.5, 0.0, 1.0);
				float lightness = clamp(_borderHardness2 * tex2D(_MainTex, IN.texcoord + distortion).r - 0.5 * _borderHardness2 + 0.5 + _border2Offset, 0.0, 1.0);
				color.r *= lightness;
				color.g *= lightness;
				color.b *= lightness;

                #ifdef UNITY_UI_ALPHACLIP
                clip (color.a - 0.001);
                #endif

                return color;
            }
        ENDCG
        }
    }
}
