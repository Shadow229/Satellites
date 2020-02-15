using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

public class Options : MonoBehaviour
{
    public TLE_Reader tle_reader;
    public TLEFile tleFile;

    public TMP_Dropdown satLoadout;
    public TMP_Dropdown satIndividual;

    private readonly float menuspeed = 1f;
    private GameObject optionScreen;

    public TextMeshProUGUI LastFetched;

    private bool _TrailRenderEnabled = false;


    private void Start()
    {
        PopulateDropdowns();
        optionScreen = transform.GetChild(0).gameObject;

        getFileModDate();

        //update staellite selection dropdown to reflect the TLE file selected
        satIndividual.AddOptions(tle_reader.GenerateSatNames(tleFile.TrackedTLEFiles[0]));

    }

    public void getFileModDate()
    {
        string savePath = string.Format("{0}/{1}.txt", Application.dataPath + "/Scripts/TLE Files", "Active");
        System.DateTime mod = File.GetLastWriteTime(savePath);

        string lf = string.Format("{0:D2}/{1:D2}/{2:D4}", mod.Day, mod.Month, mod.Year);

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

        //clear individual sat selection
       // satIndividual.ClearOptions();
        //update staellite selection dropdown to reflect the TLE file selected
       //satIndividual.AddOptions(tle_reader.GenerateSatNames(tleFile.TrackedTLEFiles[tle_index]));

    }



    public void Loadout()
    {
        //update the satelite loadouts
        tle_reader.UpdateTLEFileSelection();

        //close the options menu
        CloseOptions();

        //turn trails back on if off
        SwitchTrailRenderers(true);
    }

    public void OpenOptions()
    {
        optionScreen.SetActive(true);
        LeanTween.scaleX(optionScreen, 3.9f, menuspeed).setEase(LeanTweenType.easeInOutElastic);
    }

    public void CloseOptions()
    {
        LeanTween.scaleX(optionScreen, 0f, menuspeed).setEase(LeanTweenType.easeInOutElastic);
        StartCoroutine(DisableMenu());
        //turn trails back on if off
        SwitchTrailRenderers(true);
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

        SwitchTrailRenderers(false);
    }



    private void SwitchTrailRenderers(bool val)
    {
        if (_TrailRenderEnabled != val)
        {
            _TrailRenderEnabled = val;

            GameObject satMan = GameObject.Find("SatelliteManager");

            foreach (Transform sat in satMan.transform)
            {
                sat.gameObject.GetComponent<TrailRenderer>().Clear();
                sat.gameObject.GetComponent<TrailRenderer>().enabled = val;
            }
        }
       
    }


    public void FetchNewTLE()
    {
        LastFetched.text = "Updating TLE Data...";

        tleFile.GetNewFiles();
    }


    public void UpdateGPS()
    {
        GPS gps = new GPS();

        StartCoroutine(gps.GetGPSFromDevice());
    }


}
