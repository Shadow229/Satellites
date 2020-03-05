using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
//using UnityEngine.Experimental.XR;
using UnityEngine.XR.ARSubsystems;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ARPlacement : MonoBehaviour
{
    public GameObject PlacementIndicator;
    public GameObject Earth;

    private Pose placementPose;
    private bool placementPoseIsValid = false;
    private ARRaycastManager arRaycastManager;
    private bool EarthSet = false;

    private Earth earthScript;
    private Options options;
    private Image crosshair;

    private bool _moving = false;

    private void Start()
    {
        arRaycastManager = FindObjectOfType<ARSessionOrigin>().GetComponent<ARRaycastManager>();

        //if (arRaycastManager)
        //{
        //    Debug.Log("DEBUG: RayCastManager is not null");
        //}

        //get components
        earthScript = Earth.GetComponent<Earth>();
        options = GameObject.Find("OptionsManager").GetComponent<Options>();
        crosshair = GameObject.Find("Crosshair").GetComponent<Image>();


        //make sure the crosshair is hidden
        crosshair.GetComponent<Image>().enabled = false;

        //get GPS Location
        options.UpdateGPS();
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
                //Debug.Log("DEBUG: placement pose is valid");

                //show earth
                PlaceObject();

                //run an update on scale first (primarily for the trails but ensures the earth & sats are scaled correctly too)
                options.UpdateScale(earthScript.GetScale());

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

                //make sure the crosshair is not hidden
                crosshair.enabled = true;

                //if moving the position of an already positioned earth
                if (_moving)
                {
                    //turn on trail renders after the satellites have had time to reposition
                    StartCoroutine(TurnOnTrailRenders());
                }

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
                //make sure the crosshair is hidden
                crosshair.enabled = false;

                //turn off the trail renderes
                options.EnableTrailRenders(false);

                //set moving bool
                _moving = true;
            }
        }
                                                           
    }

    private void PlaceObject()
    {
        //set the earth origin
        earthScript.YOrigin = placementPose.position.y;
        //initialise
        earthScript.Init();

        //Debug.Log("DEBUG: YOrigin is: " + earthScript.YOrigin.ToString());
        //Debug.Log("DEBUG: The scale is: " + earthScript.GetScale().ToString());
        //Debug.Log("DEBUG: The multiplier is: " + earthScript.HeightMultiplier.ToString());
        //Debug.Log("DEBUG: Setting earth " + (earthScript.GetScale() * earthScript.HeightMultiplier).ToString() + " high.");

        //place
        Earth.transform.position = new Vector3(placementPose.position.x,
                                                placementPose.position.y + earthScript.DefaultHeight + (earthScript.GetScale() * earthScript.HeightMultiplier),
                                                placementPose.position.z);
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
        float currentScale = Earth.GetComponent<Earth>().GetScale();
        PlacementIndicator.transform.localScale = new Vector3(currentScale, currentScale, currentScale);

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

    private IEnumerator TurnOnTrailRenders()
    {
        yield return new WaitForSeconds(2);

        options.EnableTrailRenders(true);
    }
}
