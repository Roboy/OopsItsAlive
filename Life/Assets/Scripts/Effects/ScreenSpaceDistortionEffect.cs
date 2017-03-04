using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenSpaceDistortionEffect : MonoBehaviour
{
    public RenderTexture DistortionRT;
    RenderTexture screenRT;
    Camera distortCam;
    Camera mainCam;
    Material effectMaterial;

    void Awake()
    {
        //screenRT = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.Default);
        //screenRT.wrapMode = TextureWrapMode.Repeat;

        DistortionRT = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.Default);
        DistortionRT.wrapMode = TextureWrapMode.Repeat;

        effectMaterial = new Material(Shader.Find("Custom/Composite"));

        mainCam = GetComponent<Camera>();
        //mainCam.SetTargetBuffers(screenRT.colorBuffer, screenRT.depthBuffer);

        distortCam = new GameObject("DistortionCam").AddComponent<Camera>();
        distortCam.enabled = false;
    }

    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        distortCam.CopyFrom(mainCam);
        distortCam.backgroundColor = Color.black;
        distortCam.cullingMask = 1 << LayerMask.NameToLayer("Distortion");
        //mainCam.cullingMask = mainCam.cullingMask ^ (1 << LayerMask.NameToLayer("Distortion"));
        distortCam.targetTexture = DistortionRT;
        distortCam.Render();

        effectMaterial.SetTexture("_DistortionTex", DistortionRT);
        Graphics.Blit(src, dst, effectMaterial);
    }
}
