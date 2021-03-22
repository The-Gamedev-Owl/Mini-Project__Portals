using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalTravellerVisualClone : MonoBehaviour
{
    // Update list depending on Wanted Types
    private List<System.Type> _wantedTypes = new List<System.Type>
    {
        typeof(PortalTravellerVisualClone),
        typeof(Transform),
        typeof(MeshFilter),
        typeof(MeshRenderer)
    };

    public void CleanTravellerUnwantedComponents()
    {
        LoopThroughChildren(transform);
    }

    private void LoopThroughChildren(Transform parent)
    {
        RemoveUnwantedComponents(parent);
        foreach (Transform child in parent)
        {
            if (child.childCount > 0)
                LoopThroughChildren(child);
            RemoveUnwantedComponents(child);
            //yield return null;
        }
    }

    private void RemoveUnwantedComponents(Transform transform)
    {
        var components = transform.GetComponents(typeof(Component));

        foreach (Component currentComponent in components)
        {
            if (!IsCurrentComponentWanted(currentComponent.GetType()))
                Destroy(currentComponent);
        }
    }

    private bool IsCurrentComponentWanted(System.Type currentComponentType)
    {
        foreach (System.Type currentWantedType in _wantedTypes)
        {
            if (currentComponentType == currentWantedType)
                return true;
        }
        return false;
    }
}
