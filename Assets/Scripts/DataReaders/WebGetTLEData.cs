using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WebGetTLEData
{

    //NORAD WEBSITE
    private readonly string baseURL = "https://www.celestrak.com/NORAD/elements/";


    //pick up tracked TLE files from the NORAD website
    public IEnumerator GetFile(string file_name)
    {
        //Debug.Log("Attempting file get: " + file_name);

        string url = baseURL + file_name + ".txt";
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                string savePath = string.Format("{0}/{1}.txt", Application.persistentDataPath + "/TLE Files", file_name);
                System.IO.File.WriteAllText(savePath, www.downloadHandler.text);
            }
        }

        //update last read on active list
        if (file_name.Contains("Active"))
        {
            GameObject.Find("OptionsManager").GetComponent<Options>().getFileModDate();
        }
    }
}
