using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SatManager : MonoBehaviour
{
    public List<GameObject> Satellites;

    [SerializeField]
    private int ExecutionChunkSize = 50;
    private int CurrentChunk;
    private int MaxChunks;


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
        //loop through satellites and update in chunks
        for (int i = 0; i < ExecutionChunkSize; i++)
        {
            int ittrVal = i + (ExecutionChunkSize * (CurrentChunk - 1));
            //exit early if we're out of sats
            if (ittrVal >= Satellites.Count || Satellites.Count == 0)
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
}
