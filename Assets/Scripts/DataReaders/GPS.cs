using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public struct UserLocationInfo
{
    public float Latitude;
    public float Longitude;
    public float Altitude;

    public bool Set;
}


public class GPS
{
    public IEnumerator GetGPSFromDevice(TextMeshProUGUI gpsText)
    {
        // First, check if user has location service enabled
        if (!Input.location.isEnabledByUser)
        {
            gpsText.text = "GPS Disabled on Device!";
            yield break;
        }


        Debug.Log("Starting location services");

        // Start service before querying location
        Input.location.Start();

        // Wait until service initializes
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // Service didn't initialize in 20 seconds
        if (maxWait < 1)
        {
            gpsText.text = "GPS Initialisation Failed";
            Debug.Log("Timed out");
            yield break;
        }

        // Connection has failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            gpsText.text = "GPS Connection Failed";
            Debug.Log("Unable to determine device location");
            yield break;
        }
        else
        {
            // Access granted and location value could be retrieved
            Debug.Log("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);

            float Latitude = Input.location.lastData.latitude;
            float Longitude = Input.location.lastData.longitude;
            float Altitude = Input.location.lastData.altitude;

            //store it
            GameObject.Find("SatelliteManager").GetComponent<SatManager>().SetUserLocationInfo(Latitude, Longitude, Altitude);

            //GUI
            gpsText.text = string.Format("Current: Lat({0:0.00}) : Long({1:0.00}) : Alt({2:0.00})", Latitude, Longitude, Altitude);
        }

        Debug.Log("Stopping location services");

        // Stop service if there is no need to query location updates continuously
        Input.location.Stop();

    }
}
