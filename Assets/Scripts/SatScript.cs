using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zeptomoby.OrbitTools;

public class SatScript : MonoBehaviour
{
    // Test SGP4
    public string str1 = "ISS(ZARYA)";
    public string str2 = "1 25544U 98067A   20024.22416442 -.00025624  00000-0 -45634-3 0  9999";
    public string str3 = "2 25544  51.6460 345.7248 0006034 189.3781 304.5467 15.49109327209547";

    public float Lat, Long, Altitude, Azmuth, Elevation, Range;

    GameObject earth;
    SpinFree earthScript;


    // Start is called before the first frame update
    void Start()
    {
        earth = GameObject.Find("Earth");
        earthScript = earth.GetComponent<SpinFree>();
    }

    // Update is called once per frame
    void Update()
    {
        Tle tle1 = new Tle(str1, str2, str3);

        PrintPosVel(tle1);

        // Create an orbit object using the SDP4 TLE object.
        Satellite satSDP4 = new Satellite(tle1);

        DateTime dt = earthScript.realTime ? System.DateTime.Now : earthScript.dtGMT;
       // DateTime now = System.DateTime.Now;
        //get the position at time after epoch
        EciTime eciSDP4 = satSDP4.PositionEci(dt);

        Vector3 position = new Vector3((float)eciSDP4.Position.X, (float)eciSDP4.Position.Y, (float)eciSDP4.Position.Z);

        transform.position = position;

        Site cheltenham = new Site(51.9, -2.0834, 0);

        // Now get the "look angle" from the site to the satellite. 
        Topo topoLook = cheltenham.GetLookAngle(eciSDP4);

        Azmuth = (float)topoLook.AzimuthDeg;
        Elevation = (float)topoLook.ElevationDeg;
        Range = (float)topoLook.Range;

        GetAltitude();

    }

    float GetAltitude()
    {
        float alt = 0.0f;
        RaycastHit hit;
        Vector3 dir = earth.transform.position - transform.position;

        Ray downRay = new Ray(transform.position, dir);
        Debug.DrawRay(transform.position, dir, Color.green);

        if (Physics.Raycast(downRay, out hit))
        {
            Altitude = hit.distance;
        }

            return alt;
    }



    static void PrintPosVel(Tle tle)
    {
        const int Step = 360;

        Satellite sat = new Satellite(tle);
        List<Eci> coords = new List<Eci>();

        // Calculate position, velocity
        // mpe = "minutes past epoch"
        for (int mpe = 0; mpe <= (Step * 4); mpe += Step)
        {
            // Get the position of the satellite at time "mpe".
            // The coordinates are placed into the variable "eci".
            Eci eci = sat.PositionEci(mpe);

            // Add the coordinate object to the list
            coords.Add(eci);
        }

        // Print TLE data
        Console.Write("{0}\n", tle.Name);
        Console.Write("{0}\n", tle.Line1);
        Console.Write("{0}\n", tle.Line2);

        // Header
        Console.Write("\n  TSINCE            X                Y                Z\n\n");

        // Iterate over each of the ECI position objects pushed onto the
        // coordinate list, above, printing the ECI position information
        // as we go.
        for (int i = 0; i < coords.Count; i++)
        {
            Eci e = coords[i] as Eci;

            Console.Write("{0,8}.00 {1,16:f8} {2,16:f8} {3,16:f8}\n",
                          i * Step,
                          e.Position.X,
                          e.Position.Y,
                          e.Position.Z);
        }

        Console.Write("\n                  XDOT             YDOT             ZDOT\n\n");

        // Iterate over each of the ECI position objects in the coordinate
        // list again, but this time print the velocity information.
        for (int i = 0; i < coords.Count; i++)
        {
            Eci e = coords[i] as Eci;

            Console.Write("{0,24:f8} {1,16:f8} {2,16:f8}\n",
                          e.Velocity.X,
                          e.Velocity.Y,
                          e.Velocity.Z);
        }
    }
}
