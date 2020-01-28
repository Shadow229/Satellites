using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[ExecuteInEditMode]
public class TLEFile : MonoBehaviour
{
    //TLE Files
    [HideInInspector]
    public int listIndex = 0;
    [HideInInspector]
    public List<string> TLEFiles;

    public bool FetchNewFiles = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (FetchNewFiles)
        {
            GetFileNames();
            FetchNewFiles = false;
        }
    }


    void GetFileNames()
    {
        TLEFiles.Clear();

        string path = Application.dataPath + "/Scripts/TLE Files";

        foreach (string file in System.IO.Directory.GetFiles(path))
        {
            string fileName = Path.GetFileNameWithoutExtension(file);

            fileName = fileName.Replace(".txt", "");

            if (TLEFiles.IndexOf(fileName) < 0)
            {
                TLEFiles.Add(fileName);
            }
        }
    }
}
