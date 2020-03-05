using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SatManager : MonoBehaviour
{
    public List<GameObject> Satellites;

    [SerializeField]
    private int ExecutionChunkSize = 50;
    private int CurrentChunk;
    private int MaxChunks;

    public UserLocationInfo userLocInfo;


    public void Init()
    {
        //recalc max chunks
        MaxChunks = Mathf.CeilToInt((float)transform.childCount / ExecutionChunkSize);
        //reset chunk
        CurrentChunk = 1;
        //clear the old sat list
        Satellites.Clear();
        //store all sats in list
        foreach (Transform child in transform)
        {
            Satellites.Add(child.gameObject);
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //no update with no satellites
        if (Satellites.Count == 0)
        {
            return;
        }

        //loop through satellites and update in chunks
        for (int i = 0; i < ExecutionChunkSize; i++)
        {
            int ittrVal = i + (ExecutionChunkSize * (CurrentChunk - 1));
            //exit early if we're out of sats
            if (ittrVal >= Satellites.Count)
            {
                break;
            }


            //get the object
            GameObject sat = Satellites[ittrVal];

            //check if the object has decayed out
            if (sat == null)
            {
                //sat visual removed due to orbit decay
                Satellites.RemoveAt(ittrVal);
                //reinitialise manager
                Init();
                //go to next
                continue;
            }

            //get the objects script
            SatScript satScript = sat.GetComponent<SatScript>();

            //run the update
            satScript.UpdateSatellite();
        }

        //increment current chunk for next frame
        CurrentChunk = (CurrentChunk % MaxChunks) + 1;    
    }


    public void SetUserLocationInfo(float lat, float lng, float alt)
    {
        userLocInfo.Latitude = lat;
        userLocInfo.Longitude = lng;
        userLocInfo.Altitude = alt;
        userLocInfo.Set = true;
    }


    public void ClearAllSatellites()
    {
        foreach (Transform child in transform)
        {
            //delete the sat
            GameObject.Destroy(child.gameObject);
        }
        //clear the list
        Satellites.Clear();
    }
}
