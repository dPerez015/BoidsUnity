using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;

//indicamos que la clase es serializable y creamos el item en el menu de volumenes
[Serializable, VolumeComponentMenu("Post-processing/Custom/BlackAndWhite")]
public sealed class CustomPostProcess : CustomPostProcessVolumeComponent, IPostProcessComponent
{
    //variable que podremos editar desde el editor de volumenes, usa una clase especial
    public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);
    public ColorParameter color = new ColorParameter(Color.red);

    Material mat;

    //lambda function asignada a isActive
    public bool IsActive() => mat != null && intensity.value > 0f;

    //indicamos en que punto de inyección ha de introducirse el postproceso
    public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

    public override void Setup()
    {
        if (Shader.Find("Hidden/Shader/FullScreenPass") != null)
            mat = new Material(Shader.Find("Hidden/Shader/FullScreenPass"));
    }

    public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
    {
        if (mat == null)
            return;
        mat.SetFloat("_Intensity", intensity.value);
        mat.SetTexture("_InputTexture", source);
        mat.SetColor("_ColorTint", color.value);
        HDUtils.DrawFullScreen(cmd, mat, destination);
    }

    public override void Cleanup() => CoreUtils.Destroy(mat);
}
