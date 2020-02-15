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
        WebGetTLEData WebGet = new WebGetTLEData();

        foreach(string TLEFile in TrackedTLEFiles)
        {
            StartCoroutine(WebGet.GetFile(TLEFile));
        }

    }
}
