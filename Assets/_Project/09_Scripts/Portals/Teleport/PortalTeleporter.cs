﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(BoxCollider))]
public class PortalTeleporter : MonoBehaviour
{
    [SerializeField] private PortalTeleporter _linkedPortal = null;

    private List<PortalTraveller> _trackedTravellers;


    private void Awake()
    {
        _trackedTravellers = new List<PortalTraveller>();
    }

    private void FixedUpdate()
    {
        CheckEveryTravellerShouldTeleport();
    }

    /// <summary>
    /// When entering a portal collider, adds current traveller to '_trackedTravellers'.
    /// This way, the portal can check if it should be teleported(or not) each frame
    /// </summary>
    /// <param name="objectCollidedEntering"></param>
    private void OnTriggerEnter(Collider objectCollidedEntering)
    {
        AddObjectToTrackedTravellers(objectCollidedEntering.gameObject, true);
    }


    /// <summary>
    /// When exiting a portal collider, remove current traveller from '_trackedTravellers'
    /// </summary>
    /// <param name="objectCollidedLeaving"></param>
    private void OnTriggerExit(Collider objectCollidedLeaving)
    {
        PortalTraveller portalTraveller = objectCollidedLeaving.GetComponent<PortalTraveller>();

        if (portalTraveller != null)
            RemoveObjectFromTrackedTravellers(portalTraveller, true);
        else
            Debug.LogError("Error: Expected component '" + portalTraveller.GetType().ToString() + "' wasn't found on object '" + objectCollidedLeaving.name + "'.");
    }

    /// <summary>
    /// Removes the current traveller from '_trackedTravellers' list.
    /// Also remove 'PortalTraveller' component that will no longer be usefull
    /// </summary>
    /// <param name="portalTravellerToRemove"></param>
    /// <param name="shouldDestroyComponent"></param>
    private void RemoveObjectFromTrackedTravellers(PortalTraveller portalTravellerToRemove, bool shouldDestroyComponent)
    {
        if (_trackedTravellers.Contains(portalTravellerToRemove))
        {
            if (shouldDestroyComponent)
                Destroy(portalTravellerToRemove.GetComponent<PortalTraveller>());
            _trackedTravellers.Remove(portalTravellerToRemove);
        }
    }

    /// <summary>
    /// Adds the current traveller to '_trackedTravellers' list.
    /// If new traveller, adds the 'PortalTraveller' script to it
    /// </summary>
    /// <param name="newTraveller"></param>
    /// <param name="shouldAddComponent"></param>
    public void AddObjectToTrackedTravellers(GameObject newTraveller, bool shouldAddComponent)
    {
        if (!newTraveller.CompareTag("Portal") &&
            !CheckIsTravellerInTrackedList(newTraveller))
        {
            PortalTraveller newPortalTraveller = (shouldAddComponent) ? (newTraveller.AddComponent<PortalTraveller>()) : (newTraveller.GetComponent<PortalTraveller>());

            _trackedTravellers.Add(newPortalTraveller);
            newPortalTraveller.distanceFromLastPortal = newTraveller.transform.position - transform.position;
        }
    }

    private bool CheckIsTravellerInTrackedList(GameObject traveller)
    {
        PortalTraveller portalTraveller;

        if (traveller.TryGetComponent<PortalTraveller>(out portalTraveller))
        {
            if (_trackedTravellers.Contains(portalTraveller))
                return true;
        }
        return false;
    }


    /// <summary>
    /// Checks if travellers contained in '_trackedTravellers' list should be teleported. If yes, teleports them
    /// </summary>
    private void CheckEveryTravellerShouldTeleport()
    {
        // Parse list backward so elements can be deleted without a problem
        for (int travellerIndex = _trackedTravellers.Count - 1; travellerIndex >= 0; travellerIndex -= 1)
        {
            bool hasTeleported = false;
            PortalTraveller currentTraveller = _trackedTravellers[travellerIndex];

            if (CheckIfTravellerShouldBeTeleported(currentTraveller))
            {
                TeleportTraveller(currentTraveller);
                hasTeleported = true;
            }

            // Compute new distance from portal. Usefull to compute if traveller should be teleported or not
            if (!hasTeleported)
                currentTraveller.distanceFromLastPortal = currentTraveller.transform.position - transform.position;
        }
    }


    /// <summary>
    /// Compute if traveller has crossed the portal. If so, it should be teleported to the linked portal
    /// </summary>
    /// <param name="currentTraveller"></param>
    /// <returns></returns>
    private bool CheckIfTravellerShouldBeTeleported(PortalTraveller currentTraveller)
    {
        Vector3 distanceBetweenTravellerAndPortal = currentTraveller.transform.position - transform.position;
        int currentTravellerSideOfPortal = System.Math.Sign(Vector3.Dot(distanceBetweenTravellerAndPortal, transform.forward));
        int previousTravellerSideOfPortal = System.Math.Sign(Vector3.Dot(currentTraveller.distanceFromLastPortal, transform.forward));

        // Should teleport if sign from Dot product changed since last frame (meaning traveller has crossed the portal)
        if (currentTravellerSideOfPortal != previousTravellerSideOfPortal)
            return true;
        return false;
    }

    private void TeleportTraveller(PortalTraveller currentTraveller)
    {
        // Compute matrix to find linked portal's position where current traveller should be teleported.
        // Taking into account rotation, linked portal's position and current portal's position. In any case, the correct position will be found
        var m = _linkedPortal.transform.localToWorldMatrix * transform.worldToLocalMatrix * currentTraveller.transform.localToWorldMatrix;

        currentTraveller.transform.position = m.GetColumn(3);
        currentTraveller.transform.rotation = m.rotation;

        // Remove current traveller from '_trackedTravellers' in this portal
        RemoveObjectFromTrackedTravellers(currentTraveller, false);
        // Add current traveller to '_trackedTravellers' in linked portal
        _linkedPortal.AddObjectToTrackedTravellers(currentTraveller.gameObject, false);
    }
}
