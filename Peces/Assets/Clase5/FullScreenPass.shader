Shader "Hidden/Shader/FullScreenPass"
{
    HLSLINCLUDE

    #pragma target 4.5
    #pragma only_renderers d3d11 ps4 xboxone vulkan metal switch

    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/PostProcessing/Shaders/FXAA.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/PostProcessing/Shaders/RTUpscale.hlsl"

    struct Attributes
    {
        uint vertexID : SV_VertexID;
        UNITY_VERTEX_INPUT_INSTANCE_ID
    };

    struct Varyings
    {
        float4 positionCS : SV_POSITION;
        float2 texcoord   : TEXCOORD0;
        UNITY_VERTEX_OUTPUT_STEREO
    };

    Varyings Vert(Attributes input)
    {
        Varyings output;
        UNITY_SETUP_INSTANCE_ID(input);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
        output.positionCS = GetFullScreenTriangleVertexPosition(input.vertexID);
        output.texcoord = GetFullScreenTriangleTexCoord(input.vertexID);
        return output;
    }

    // List of properties to control your post process effect
    float _Intensity;
	float4 _ColorTint;
    TEXTURE2D_X(_InputTexture);

	//Acceso al depth, no funciona
	//depth va de 1 a 0, siendo 1 el plano y 0 en el plano far
	//float depth = LoadCameraDepth(input.positionCS.xy);
	float linearDepth(float z) {
#if UNITY_REVERSED_Z != 1
		float c1 = _ZBufferParams.y;
		float c0 = 1 - c1;
		return 1.0 / (c0*z + c1);
#else
		float n = _ProjectionParams.y;
		float f = _ProjectionParams.z;
		return  (2 * n) / (f + n - z * (f - n));
#endif
	}

    float4 CustomPostProcess(Varyings input) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
		//obtenemos la posicion UV del fragmento que vamos a renderizar
        uint2 positionSS = input.texcoord * _ScreenSize.xy;
		//obtenemos el color de la textura de input
        float3 outColor = LOAD_TEXTURE2D_X(_InputTexture, positionSS).xyz;
	    
		return float4(-outColor.xyz, 1.0);

    }

    ENDHLSL

    SubShader
    {
        Pass
        {
            Name "FullScreenPass"

            ZWrite Off
            ZTest Always
            Blend Off
            Cull Off

            HLSLPROGRAM
                #pragma fragment CustomPostProcess
                #pragma vertex Vert
            ENDHLSL
        }
    }
    Fallback Off
}
