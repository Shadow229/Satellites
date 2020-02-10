﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

//read TLE from .txt files
public class TLE_Reader : MonoBehaviour
{
    //file & reader
    protected FileInfo SourceFile = null;
    protected StreamReader reader = null;

    //TLE Data strings
    protected string Str1, Str2, Str3 = "";

    private string CurrentTLEFile = "";

    TLEFile tleFile;

    [Tooltip("Set to -1 to load entire file")]
    public int SatLoadoutAmount = 5;

    public GameObject SatellitePrefab;

    GameObject SatelliteManager = null;

    bool forceUpdate = false;

    private void Start()
    {
        SatelliteManager = GameObject.Find("SatelliteManager");

        if (SatelliteManager == null)
        {
            throw new System.ArgumentException("Parameter cannot be null", "Missing Satellite Manager");
        }

        tleFile = GetComponent<TLEFile>();
    }

    private void Update()
    {
        //get the current selected TLE file
        int Index = tleFile.listIndex;
        string TLEFileSelect = tleFile.TLEFiles[Index];

        if (TLEFileSelect.CompareTo(CurrentTLEFile) != 0 || forceUpdate)
        {
            forceUpdate = false;

            //update the set TLE file
            CurrentTLEFile = TLEFileSelect;

            GenerateSatellites();
        }
    }

    private void OnValidate()
    {
        forceUpdate = true;
    }


    //generates satellites from new TLE file
    private void GenerateSatellites()
    {
        //clear current satellites
        foreach (Transform child in SatelliteManager.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        //read TLE data in
        string Filepath = Application.dataPath + "/Scripts/TLE Files/" + CurrentTLEFile.ToString() + ".txt";

        SourceFile = new FileInfo(Filepath);
        reader = SourceFile.OpenText();

        //loop through txt file - limit the loads here
        int ittr = SatLoadoutAmount == -1 ? int.MaxValue : SatLoadoutAmount;

        for (int i = 0; i < ittr; i++)
        {
            //early out if the file is empty
            if (reader.EndOfStream) { break; }

            //read TLE lines
            string tle1 = reader.ReadLine().Trim();
            string tle2 = reader.ReadLine().Trim();
            string tle3 = reader.ReadLine().Trim();

            //sense check on data reads
            if (tle1 != null && tle2 != null && tle3 != null)
            {
                //instantiate new satellite prefab
                GameObject sat = Instantiate(SatellitePrefab, new Vector3(0, 0, 0), Quaternion.identity, SatelliteManager.transform);
                //name it
                sat.name = tle1;

                //update TLE information
                SatScript ss = sat.GetComponent<SatScript>();

                ss.TLE1 = tle1;
                ss.TLE2 = tle2;
                ss.TLE3 = tle3;

            }
        }

        reader.Close();

        //initialise the satellite manager
        SatelliteManager.GetComponent<SatManager>().Init();

    }



}
