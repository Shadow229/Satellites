using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

//[ExecuteInEditMode]
public class TLEFile : MonoBehaviour
{
    //TLE Files
    [HideInInspector]
    public int listIndex = 0;
 
    public List<string> TrackedTLEFiles;


    public void GetNewFiles()
    {
        //checking directory
        Debug.Log("Checking Persistent Data Directory");
        if (!Directory.Exists(Application.persistentDataPath + "/TLE Files"))
        {
            Debug.Log("Creating Directory...");
            Directory.CreateDirectory(Application.persistentDataPath + "/TLE Files");
        }
        else
        {
            Debug.Log("Directory Exists!");
        }

        Debug.Log("Getting new TLE Files");

        WebGetTLEData WebGet = new WebGetTLEData();

        foreach(string TLEFile in TrackedTLEFiles)
        {
            StartCoroutine(WebGet.GetFile(TLEFile));
        }
    }
}
