using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using UnityEngine.UI;

public class Options : MonoBehaviour
{
    public TLE_Reader tle_reader;
    public TLEFile tleFile;

    public TMP_Dropdown satLoadout;
    public TMP_Dropdown satIndividual;
    public SatSearch satSearch;
    public Earth earth;

    private readonly float menuspeed = 1f;
    private GameObject optionScreen;

    public TextMeshProUGUI LastFetched;
    public TextMeshProUGUI GPS;
    public SatUI satUI;

    private bool _TrailRenderEnabled = false;
    private bool _OptionsOpen = false;

    public bool IsOptionsOpen() { return _OptionsOpen; }

    private void Start()
    {
        PopulateDropdowns();
        optionScreen = transform.GetChild(0).gameObject;

        getFileModDate();
    }

    public void getFileModDate()
    {
        Debug.Log("Getting file mod date");

        string savePath = string.Format("{0}/{1}.txt", Application.persistentDataPath + "/TLE Files", "Active");
        System.DateTime mod = File.GetLastWriteTime(savePath);

        string lf = string.Format("{0:00}/{1:00}/{2:0000}", mod.Day, mod.Month, mod.Year);

        LastFetched.text = "Last Fetched " + lf;

    }


    void PopulateDropdowns()
    {
        satLoadout.AddOptions(tleFile.TrackedTLEFiles);
    }
    

    //called from changing the dropdown satellite loadout in options menu
    public void SatLoadoutChange(int tle_index)
    {
        //register the index on value change
        tleFile.listIndex = tle_index;

        //There are just too many satellites for unity's UI to cope with all sats in a dropdown. Put on hold for now
        //clear individual sat selection
        //satIndividual.ClearOptions();
        //update staellite selection dropdown to reflect the TLE file selected
        //satIndividual.AddOptions(tle_reader.GenerateSatNames(tleFile.TrackedTLEFiles[tle_index]));

        //run the update on the sat search incase the sat doesnt exist in the new TLE
        satSearch.GetAvailableSats(satSearch.InputField.text);

    }



    public void Loadout()
    {
        //update the satelite loadouts
        tle_reader.UpdateTLEFileSelection();

        //scale sats and trails
        UpdateScale(earth.GetScale());

        //close the options menu
        CloseOptions();
    }

    public void OpenOptions()
    {
        //dont run if menu is already open
        if (_OptionsOpen) {return;}
        _OptionsOpen = true;

        //hide crosshair
        GameObject.Find("Crosshair").GetComponent<Image>().enabled = false;

        //close satview UI
        if (satUI.UIVisible) {satUI.HideUI();}
        

        //play sound
        AudioSource audio = GetComponent<AudioSource>();
        if (!audio.isPlaying)
        {
            audio.clip = satUI.menuIn;
            audio.Play();
        }

        optionScreen.SetActive(true);
        LeanTween.scaleX(optionScreen, 3.9f, menuspeed).setEase(LeanTweenType.easeInOutElastic);

        //turn trails off
        EnableTrailRenders(false);

    }

    public void CloseOptions()
    {
        _OptionsOpen = false;

        //play sound
        AudioSource audio = GetComponent<AudioSource>();
        if (!audio.isPlaying)
        {
            audio.clip = satUI.menuOut;
            audio.Play();
        }


        LeanTween.scaleX(optionScreen, 0f, menuspeed).setEase(LeanTweenType.easeInOutElastic);
        StartCoroutine(DisableMenu());
        //turn trails back on if off with a delay to allow for sats to spawn fully
        StartCoroutine(DelayedEnableTrailRenders(true, 1.5f));

        //show crosshair
        GameObject.Find("Crosshair").GetComponent<Image>().enabled = true;
    }

    IEnumerator DisableMenu()
    {
        yield return new WaitForSeconds(menuspeed);

        optionScreen.SetActive(false);
    }


    public void UpdateScale(float a_scale)
    {
        //scale earth
        GameObject.Find("Earth").GetComponent<Earth>().UpdateScale(a_scale);
        //scale highlight ring
        Camera.main.GetComponent<SatChecker>().SetHightlightRingScale(a_scale * 0.5f);
        //scale the trail renderers
        ScaleTrailRenderers(a_scale);
    }



    public void EnableTrailRenders(bool val)
    {
        if (_TrailRenderEnabled != val)
        {
            _TrailRenderEnabled = val;

            Debug.Log("The Trail Renderers are " + (val ? "On" : "Off"));

            SatManager satMan = GameObject.Find("SatelliteManager").GetComponent<SatManager>();

            foreach (GameObject sat in satMan.Satellites)
            {
                TrailRenderer tr = sat.GetComponent<TrailRenderer>();
                tr.Clear();
                tr.enabled = val;
                tr.Clear();
            }
         }
    }

    private IEnumerator DelayedEnableTrailRenders(bool val, float delay)
    {
        Debug.Log("Delayed Trail Renderer Triggered for " + (val ? "On" : "Off"));

        yield return new WaitForSeconds(delay);

        EnableTrailRenders(val);
    }


    public void ScaleTrailRenderers(float a_scale)
    {
        SatManager satMan = GameObject.Find("SatelliteManager").GetComponent< SatManager>();

        foreach (GameObject sat in satMan.Satellites)
        {
            sat.GetComponent<TrailRenderer>().startWidth = a_scale / 200;
            //while here, scale the audio source distance too (still playing with these to get ideal falloff)
            sat.GetComponent<AudioSource>().minDistance = a_scale / 2.5f;
            sat.GetComponent<AudioSource>().maxDistance = a_scale / 1.8f;

            //Debug.Log("DEBUG: Audio scale is: " + a_scale.ToString());
        }

        Debug.Log("DEBUG: Trail Scale is: " + (a_scale / 200).ToString("n4"));
    }

    public void FetchNewTLE()
    {
        LastFetched.text = "Updating TLE Data...";

        tleFile.GetNewFiles();
    }


    public void UpdateGPS()
    {
        GPS gps = new GPS();

        GPS.text = "Triangulating...";
        StartCoroutine(gps.GetGPSFromDevice(GPS));
    }



    public void ShowDateTime(bool show)
    {
        GameObject.Find("Earth").GetComponent<Earth>().DateTimeToggle(show);
    }
}
