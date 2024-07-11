Shader "Custom/SpriteOutlineShader"
{
    Properties
    {
        _Color ("Tint", Color) = (1,1,1,1)
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0

		_OutlineColor("Outline Color", Color) = (1,1,1,1)

        // From sprite default shader
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
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

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        CGPROGRAM
        #pragma surface surf Lambert vertex:vert nofog nolightmap nodynlightmap keepalpha noinstancing addshadow
        #pragma multi_compile_local _ PIXELSNAP_ON
        #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
        #include "UnitySprites.cginc"

        fixed4 _OutlineColor;
        float4 _MainTex_TexelSize;

        struct Input
        {
            float4 vertex : SV_POSITION;
            fixed4 color : COLOR;
            float2 uv_MainTex : TEXCOORD0;
        };

        
        float GetWave(float2 coord){
            float wave = cos( (coord +  _Time.y * 3) * .75) * 0.5 + .75;
            wave *= 1-coord;

            return wave;
        }

        void vert(inout appdata_full v, out Input o)
        {
            v.vertex = UnityFlipSprite(v.vertex, _Flip);

            #if defined(PIXELSNAP_ON)
            v.vertex = UnityPixelSnap(v.vertex);
            #endif
            
            UNITY_INITIALIZE_OUTPUT(Input,o)
            o.color = v.color * _Color * _RendererColor;
        }

        void surf(Input IN, inout SurfaceOutput o)
        {
            fixed4 c = SampleSpriteTexture(IN.uv_MainTex) * IN.color;
            if (c.a == 0) 
            {
                float yTexel = _MainTex_TexelSize.y;
                float xTexel =_MainTex_TexelSize.x;
                fixed4 pixelUp = tex2D(_MainTex, IN.uv_MainTex + fixed2(0, yTexel));
                fixed4 pixelDown = tex2D(_MainTex, IN.uv_MainTex - fixed2(0, yTexel));
                fixed4 pixelRight = tex2D(_MainTex, IN.uv_MainTex + fixed2(xTexel, 0));
                fixed4 pixelLeft = tex2D(_MainTex, IN.uv_MainTex - fixed2(xTexel, 0));
                
                float direction = pixelDown.a + pixelLeft.a - (pixelRight.a + pixelUp.a);
                if (pixelUp.a > 0 || pixelRight.a > 0 || pixelLeft.a > 0 || pixelDown.a > 0) {
                    c.rgba = _OutlineColor;
                    c.a *= GetWave(IN.uv_MainTex.yx * 2 - 1);
                }
            }
            
            o.Albedo = c.rgb * c.a;
            o.Alpha = c.a;
        }
        ENDCG
    }

    FallBack "Diffuse"
}
