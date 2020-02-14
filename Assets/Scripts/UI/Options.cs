using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Options : MonoBehaviour
{
    public TLE_Reader tle_reader;

    public TMP_Dropdown satLoadout;

    private float menuspeed = 1f;
    private GameObject optionScreen;


    private void Start()
    {
        PopulateDropdowns();
        optionScreen = transform.GetChild(0).gameObject;
    }

    //change tlelistfile index based on selection




    void PopulateDropdowns()
    {
        satLoadout.AddOptions(tle_reader.tleFile.TLEFiles);
    }
    

    public void satLoadoutChange(int tle_index)
    {
        //register the index on value change
        tle_reader.tleFile.listIndex = tle_index;

    }



    public void Loadout()
    {
        //update the satelite loadouts
        tle_reader.UpdateTLEFileSelection();

        //close the options menu
        CloseOptions();
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
    }

    IEnumerator DisableMenu()
    {
        yield return new WaitForSeconds(menuspeed);

        optionScreen.SetActive(false);

    }


    public void UpdateScale(float a_scale)
    {
        GameObject.Find("Earth").GetComponent<Earth>().SetScale(a_scale);
    }

}
