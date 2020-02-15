using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WebGetTLEData
{

    private string baseURL = "https://www.celestrak.com/NORAD/elements/";

    public void FetchTLEData(List<string> satPackages)
    {

        string url = baseURL + "";

        GetFile(url);
    }

    IEnumerator GetFile(string file_name)
    {
        string url = "https://files.rcsb.org/download/" + file_name + ".pdb";
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                string savePath = string.Format("{0}/{1}.txt", Application.persistentDataPath, file_name);
                System.IO.File.WriteAllText(savePath, www.downloadHandler.text);
            }
        }
    }
}
