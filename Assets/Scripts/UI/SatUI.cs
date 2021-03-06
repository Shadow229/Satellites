﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class SatUI : MonoBehaviour
{
    public bool UIVisible = false;

    public GameObject SatName, ID, Altitude, Azimuth, Elevation, Range;
    public Camera SatCam;
    public float FOVDiv = 50000f;
    private float _satCamFOV;

    private TextMeshProUGUI TMP_SatName, TMP_ID, TMP_Altitude, TMP_Azimuth, TMP_Elevation, TMP_Range;

    private float UIScaleTime = 1f;
    private GameObject _infoPanel;
    private GameObject _satVis;
    private GameObject _earth;
    private GameObject _activeSatellite;

    public AudioClip menuIn, menuOut;

    private void Start()
    {
        //get objects
        _earth = GameObject.Find("Earth");
        //get panel components
        _infoPanel = transform.GetChild(0).gameObject;
        _satVis = transform.GetChild(1).gameObject;
        //get text meshes from UI elements
        TMP_SatName = SatName.GetComponent<TextMeshProUGUI>();
        TMP_ID = ID.GetComponent<TextMeshProUGUI>();
        TMP_Altitude = Altitude.GetComponent<TextMeshProUGUI>();
        TMP_Azimuth = Azimuth.GetComponent<TextMeshProUGUI>();
        TMP_Elevation = Elevation.GetComponent<TextMeshProUGUI>();
        TMP_Range = Range.GetComponent<TextMeshProUGUI>();
    }

    //toggle UI visibility
    public void UIVisibility(GameObject Satellite)
    {
        //update touch when changed to ios
        //// Handle screen touches.
        //if (Input.touchCount > 0)
        //{
        //    //the screen has been touched - show the
        //}

        //on click
        if (Input.GetMouseButtonDown(0))
        {
            //ignore if over UI
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }


            if (UIVisible)
            {
                HideUI();
            }
            else
            {
                ShowUI(Satellite);
            }       
        }
    }


    private void SwitchSat(GameObject Satellite)
    {
        if (UIVisible && Satellite != _activeSatellite)
        {
            //set our active satellite
            _activeSatellite = Satellite;
            //attach the camera to the satellite
            AttachViewCam();
        }
    }

    //call to update all UI values and visibility
    public void UpdateUI(GameObject Satellite)
    {
        //check UI visibility
        UIVisibility(Satellite);
        //check for a live switch
        SwitchSat(Satellite);

        //if UI is open, update the values
        if (UIVisible)
        {
            //set UI values
            SetUIValues();
            //update cam direction
            UpdateViewCam();
        }
        else
        {
            //reset UI values
            SetDefaultUIValues();
            //clear active sat
            _activeSatellite = null;
        }
    }


    //hid ethe UI from screen
    public void HideUI()
    {
        if (UIVisible)
        {
            SetDefaultUIValues();

            UIVisible = false;

            //play sound
            AudioSource audio = GetComponent<AudioSource>();
            if (!audio.isPlaying)
            {
                audio.clip = menuOut;
                audio.Play();
            }

            LeanTween.scaleY(_infoPanel, 0f, UIScaleTime).setEase(LeanTweenType.easeInOutElastic);
            LeanTween.scaleX(_satVis, 0f, UIScaleTime).setEase(LeanTweenType.easeInOutElastic);
            StartCoroutine(HideUITimed());
        }
       
    }

    private void ShowUI(GameObject Satellite)
    {
        UIVisible = true;

        //set our active satellite
        _activeSatellite = Satellite;
        //attach the camera to the satellite
        AttachViewCam();


        //activate UI components
        _infoPanel.SetActive(UIVisible);
        _satVis.SetActive(UIVisible);

        //play sound
        AudioSource audio = GetComponent<AudioSource>();
        if (!audio.isPlaying)
        {
            audio.clip = menuIn;
            audio.Play();
        }

        //animation on screen
        LeanTween.scaleY(_infoPanel, 3.1f, UIScaleTime).setEase(LeanTweenType.easeInOutElastic);
        LeanTween.scaleX(_satVis, 2.4f, UIScaleTime).setEase(LeanTweenType.easeInOutElastic);
    }

    IEnumerator HideUITimed()
    {
        yield return new WaitForSeconds(UIScaleTime);

        _infoPanel.SetActive(UIVisible);
        _satVis.SetActive(UIVisible);

        //detach view cam from satellite
        DetachViewCam();

    }

    //replace all values with default dash
    private void SetDefaultUIValues()
    {
        TMP_SatName.text = "-";
        TMP_ID.text = "-";
        TMP_Altitude.text = "-";
        TMP_Azimuth.text = "-";
        TMP_Elevation.text = "-";
        TMP_Range.text = "-";
    }

    private void SetUIValues()
    {
        //check camera parented correctly
        if (SatCam.transform.parent == null)
        {
            AttachViewCam();
        }

        //get script
        SatScript satScript = _activeSatellite.GetComponent<SatScript>();

        //sense check
        if (satScript)
        {
            //assign values
            TMP_SatName.text = satScript.TLE1.ToString();
            TMP_ID.text = satScript.TLE2.Split(' ')[1].Trim();
            TMP_Altitude.text = satScript.Altitude.ToString("n2") + "  km";
            TMP_Azimuth.text = satScript.Azmuth.ToString("n2");
            TMP_Elevation.text = satScript.Elevation.ToString("n2");
            TMP_Range.text = satScript.Range.ToString("n2") + " km";

            _satCamFOV = Mathf.Clamp(FOVDiv / satScript.Altitude, 1, 60);
        }
    }


    private void AttachViewCam()
    {
        SatCam.transform.parent = _activeSatellite.transform;
    }
    public void DetachViewCam()
    {
        SatCam.transform.parent = null;
    }

    private void UpdateViewCam()
    {
        if (_earth != null)
        {
            SatCam.transform.localPosition = Vector3.zero;
            SatCam.transform.LookAt(_earth.transform);
            SatCam.fieldOfView = _satCamFOV;
        }
    }
}
