using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
//using UnityEngine.Experimental.XR;
using UnityEngine.XR.ARSubsystems;
using System;
using UnityEngine.EventSystems;

public class ARPlacement : MonoBehaviour
{
    public GameObject PlacementIndicator;
    public GameObject Earth;

    private Pose placementPose;
    private bool placementPoseIsValid = false;
    private ARRaycastManager arRaycastManager;
    private bool EarthSet = false;

    private void Start()
    {
        arRaycastManager = FindObjectOfType<ARSessionOrigin>().GetComponent<ARRaycastManager>();

        if (arRaycastManager)
        {
            Debug.Log("RayCastManager is not null");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!EarthSet)
        {
            UpdatePlacementPose();
            UpdatePlacementIndicator();

            //when the user touches the screen place earth
            if (placementPoseIsValid && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && !IsPointerOverUIObject())
            {
                Debug.Log("placement pose is valid");

                PlaceObject();

                //Hide indicator
                PlacementIndicator.SetActive(false);
                //hide cloud points
                ARPointCloudManager pointCloudManager = FindObjectOfType<ARSessionOrigin>().GetComponent<ARPointCloudManager>();
                pointCloudManager.enabled = false;
                foreach (var pointCloud in pointCloudManager.trackables)
                {
                    pointCloud.gameObject.SetActive(false);
                }
                //stop update
                EarthSet = true;

            }
        }
        else
        {
            //tap with 2 finger to reset the position of earth
            if (Input.touchCount == 2 && Input.GetTouch(0).phase == TouchPhase.Began && !IsPointerOverUIObject())
            {
                EarthSet = false;
                PlacementIndicator.SetActive(true);
                ARPointCloudManager pointCloudManager = FindObjectOfType<ARSessionOrigin>().GetComponent<ARPointCloudManager>();
                pointCloudManager.enabled = true;
                foreach (var pointCloud in pointCloudManager.trackables)
                {
                    pointCloud.gameObject.SetActive(true);
                }
            }
        }
        
    }

    private void PlaceObject()
    {
        //place
        Earth.transform.position = new Vector3(placementPose.position.x, Earth.transform.position.y, placementPose.position.z);
        //show mesh
        Earth.transform.GetChild(0).gameObject.SetActive(true);
    }

    private void UpdatePlacementPose()
    {
        Vector3 screenCentre = Camera.current.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        arRaycastManager.Raycast(screenCentre, hits, TrackableType.Planes);

        placementPoseIsValid = hits.Count > 0;

        if (placementPoseIsValid)
        {
            placementPose = hits[0].pose;

            Vector3 cameraForward = Camera.current.transform.forward;
            Vector3 cameraBearing = new Vector3(cameraForward.x, 0f, cameraForward.z).normalized;

            placementPose.rotation = Quaternion.LookRotation(cameraBearing);
        }
    }

    private void UpdatePlacementIndicator()
    {
        if (placementPoseIsValid)
        {
            PlacementIndicator.SetActive(true);
            PlacementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        else
        {
            PlacementIndicator.SetActive(false);
        }
    }



    private bool IsPointerOverUIObject()
    {
        if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
        {
            return true;
        }
        return false;

    }
}
