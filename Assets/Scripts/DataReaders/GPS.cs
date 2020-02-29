using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct UserLocationInfo
{
    public float Latitude;
    public float Longitude;
    public float Altitude;

    public bool Set;
}


public class GPS
{
    public IEnumerator GetGPSFromDevice()
    {
        // First, check if user has location service enabled
        if (!Input.location.isEnabledByUser)
            yield break;

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
            Debug.Log("Timed out");
            yield break;
        }

        // Connection has failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
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

            GameObject.Find("SatelliteManager").GetComponent<SatManager>().SetUserLocationInfo(Latitude, Longitude, Altitude);
        }

        Debug.Log("Stopping location services");

        // Stop service if there is no need to query location updates continuously
        Input.location.Stop();

    }
}
