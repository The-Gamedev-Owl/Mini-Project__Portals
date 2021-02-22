using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class RenderPortalCamera : MonoBehaviour
{
    [SerializeField] private MeshRenderer _linkedPortalScreenMeshRenderer = null;
    [SerializeField] private MeshRenderer _portalScreenMeshRenderer = null;

    private Camera _portalCamera;

    void Start()
    {
        _portalCamera = GetComponent<Camera>();
        if (_portalCamera == null)
            Debug.LogError("Error: No Camera component found in gameObject '" + name + "'.");
    }

    private void OnPreRender()
    {
        // Hide portal screen so camera don't render it
        _portalScreenMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
    }

    private void OnPreCull()
    {
        RenderCameraViewThroughPortal();
    }

    private void OnPostRender()
    {
        // Show portal screen so player still see it. Illusion with PreRender is flawless
        _portalScreenMeshRenderer.material.SetInt("displayMask", 1);
        _portalScreenMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
    }

    private void RenderCameraViewThroughPortal()
    {
        CreateRenderTexture();

        _portalCamera.Render();
    }

    private void CreateRenderTexture()
    {
        RenderTexture newRenderTexture;

        if (_portalCamera.targetTexture == null || _portalCamera.targetTexture.width != Screen.width || _portalCamera.targetTexture.height != Screen.height)
        {
            // Remove current RenderTexture if one exist
            RemoveCurrentRenderTexture();

            // Create new RenderTexture
            newRenderTexture = new RenderTexture(Screen.width, Screen.height, 24);

            // Assign newly created TargetTexture to camera and '_MainTex' in linked portal screen's material
            _portalCamera.targetTexture = newRenderTexture;
            _linkedPortalScreenMeshRenderer.material.SetTexture("_MainTex", _portalCamera.targetTexture);
        }
    }

    private void RemoveCurrentRenderTexture()
    {
        if (_portalCamera.targetTexture != null)
            _portalCamera.targetTexture.Release();
    }
}
