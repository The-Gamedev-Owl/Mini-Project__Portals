using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePortalCamera : MonoBehaviour
{
    [SerializeField] private Transform _playerCameraTransform;
    [SerializeField] private Transform _currentPortalTransform;
    [SerializeField] private Transform _linkedPortalTransform;


    private void LateUpdate()
    {
        ComputeNewCameraPositionAndRotation();
    }

    private void ComputeNewCameraPositionAndRotation()
    {
        Matrix4x4 localToWorldMatrix = _playerCameraTransform.localToWorldMatrix;

        /* 
         * This opertation allows to get our camera position depending on both portal positions AND rotations compared to the world
         * This means even if portals are rotated, the camera will correctly be placed
         */
        localToWorldMatrix = _currentPortalTransform.localToWorldMatrix * _linkedPortalTransform.worldToLocalMatrix * localToWorldMatrix;
        // Column 3 is the one which contains position
        transform.SetPositionAndRotation(localToWorldMatrix.GetColumn(3), localToWorldMatrix.rotation);
    }
}
