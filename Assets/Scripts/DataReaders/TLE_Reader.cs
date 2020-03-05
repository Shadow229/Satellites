using System.Collections;
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

    private TLEFile tleFile;

    public GameObject SatellitePrefab;

    GameObject SatelliteManager = null;

    private string CurrentSat = "";
    private string SelectedSat = "";


    private void Awake()
    {
        SatelliteManager = GameObject.Find("SatelliteManager");

        if (SatelliteManager == null)
        {
            throw new System.ArgumentException("Parameter cannot be null", "Missing Satellite Manager");
        }

        tleFile = GetComponent<TLEFile>();
    }


    public void UpdateTLEFileSelection()
    {
        //get the current selected TLE file
        int Index = tleFile.listIndex;
        string TLEFileSelect = tleFile.TrackedTLEFiles[Index];

        if (TLEFileSelect.CompareTo(CurrentTLEFile) != 0 || SelectedSat.CompareTo(CurrentSat) != 0)
        {
            //update the set TLE file
            CurrentTLEFile = TLEFileSelect;
            //update the set selected sat
            CurrentSat = SelectedSat;

            //generate sats
            GenerateSatellites();
        }
    }

    //generates satellites from new TLE file
    private void GenerateSatellites()
    {
        //clear current satellites
        SatelliteManager.GetComponent<SatManager>().ClearAllSatellites();

        //read TLE data in
        string Filepath = Application.persistentDataPath + "/TLE Files/" + CurrentTLEFile.ToString() + ".txt";

        SourceFile = new FileInfo(Filepath);
        reader = SourceFile.OpenText();

        //loop through txt file - limit the loads here
        int ittr = int.MaxValue;

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
                if (SelectedSat == "" || SelectedSat.CompareTo(tle1) == 0)
                {
                    //Debug.Log("DEBUG: Instantiating new sat: " + tle1.ToString());

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
        }

        reader.Close();

        //initialise the satellite manager
        SatelliteManager.GetComponent<SatManager>().Init();

    }

    public List<string> GenerateSatNames(string TLEFile)
    {
        List<string> SatNames = new List<string>();

        //read TLE data in
        string Filepath = Application.persistentDataPath + "/TLE Files/" + TLEFile + ".txt";

        SourceFile = new FileInfo(Filepath);
        reader = SourceFile.OpenText();

        //add all other sats from the files
        while (!reader.EndOfStream)
        {
            //read TLE lines
            string satname = reader.ReadLine().Trim();

            //skip down next 2 lines
            reader.ReadLine();
            reader.ReadLine();

            //sense check on data reads
            if (satname != null)
            {
                SatNames.Add(satname);
            }
        }

        reader.Close();

        return SatNames;
    }


    public void SelectSatellite(string sat)
    {
        SelectedSat = sat;
    }

}
