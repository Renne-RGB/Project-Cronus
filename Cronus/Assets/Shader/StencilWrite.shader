Shader "Custom/StencilWriteVisible" {
    Properties {
        _Color ("Vision Color", Color) = (1, 1, 1, 0.2) // 白い半透明
    }
    SubShader {
        Tags { "RenderType"="Transparent" "Queue"="Geometry-1" }
        
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off 

        Pass {
            Stencil {
                Ref 1
                Comp Always
                Pass Replace
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 _Color;

            struct appdata { float4 vertex : POSITION; };
            struct v2f { float4 pos : SV_POSITION; };

            v2f vert (appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                return _Color;
            }
            ENDCG
        }
    }
}