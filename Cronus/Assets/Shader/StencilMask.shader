Shader "Custom/StencilMask"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Transparent-1" }
        Pass
        {
            ColorMask 0
            ZWrite Off

            Stencil
            {
                Ref 1
                Comp Always
                Pass Replace
            }
        }
    }
}