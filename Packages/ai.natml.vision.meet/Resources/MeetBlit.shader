Shader "Hidden/Meet/Blit" {
    Properties {
        _MainTex ("Main texture", 2D) = "" {}
    }
    SubShader {
        Pass {
            ZTest Always Cull Off ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            UNITY_DECLARE_SCREENSPACE_TEXTURE(_MainTex);
            uniform float4 _MainTex_ST;

            struct appdata_t {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f vert (appdata_t v) {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord.xy, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
                #if UNITY_UV_STARTS_AT_TOP
                float2 uv = float2(i.texcoord.x, 1.0 - i.texcoord.y);
                #else
                float2 uv = i.texcoord;
                #endif
                fixed4 color = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, uv);
                return fixed4(color.r, color.r, color.r, 1.0);
            }
            ENDCG
        }
    }
    Fallback Off
}