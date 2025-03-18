Shader "Custom/OutlineSelection"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry+1" }
        Pass
        {
            ColorMask 0 // Don't write to any color channels
            ZWrite On // Write to depth buffer
            Cull Off // Render both front and back faces
        }
    }
}