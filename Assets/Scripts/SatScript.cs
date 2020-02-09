using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zeptomoby.OrbitTools;

public class SatScript : MonoBehaviour
{
    // Test SGP4
    public string TLE1 = "ISS(ZARYA)";
    public string TLE2 = "1 25544U 98067A   20024.22416442 -.00025624  00000-0 -45634-3 0  9999";
    public string TLE3 = "2 25544  51.6460 345.7248 0006034 189.3781 304.5467 15.49109327209547";

    public float Lat, Long, Altitude, Azmuth, Elevation, Range;

    [SerializeField]
    private float _rotSpeed, _rotAmt;

    GameObject earth;
    Earth earthScript;

    Satellite sat;


    // Start is called before the first frame update
    void Start()
    {
        earth = GameObject.Find("Earth");
        earthScript = earth.GetComponent<Earth>();

        // generate TLE object
        Tle tle1 = new Tle(TLE1, TLE2, TLE3);

        // Create an orbit object using the SDP4/SGP4 TLE object.
        sat = new Satellite(tle1);

        //initialise sat
        Init();

    }

    private void Init()
    {
        //update our satellites position on instantiation
        UpdateSatellite();
        //enable to trail renderer from there
        GetComponent<TrailRenderer>().enabled = true;
        //assign an arbitrary rotation around local up
        _rotSpeed = UnityEngine.Random.Range(5f, 35f);
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSatellite();
    }


    void UpdateSatellite()
    {

        if (sat != null)
        {        
            //if there is an error with the orbit (ie, the sat is no longer in orbit and the TLE reflects no orbit trajectory)
            if (sat.Orbit.isError)
            {
                //delete the prefab
                GameObject.Destroy(this.gameObject);
                return;
            }

            //get date time value
            DateTime dt = earthScript.realTime ? earthScript.TimeNow : earthScript.dtGMT;

            //get the position at time after epoch
            EciTime eciSDP4 = sat.PositionEci(dt);

            //flip position values to orientate satelite to earth object
            Vector3 position = new Vector3((float)eciSDP4.Position.X, (float)eciSDP4.Position.Z, (float)eciSDP4.Position.Y);

            //scale position vecotor inline with earth scale
            position /= earthScript.ScaleAmount;

            //update position
            transform.position = position;

            //face earth
            Vector3 targetDirection = (earth.transform.position - transform.position).normalized;
            //face earth

            //this just wobbles.. no idea why it wont rotate around its local Y axis..
            transform.up = targetDirection;
            //rotate around x
            _rotAmt = (_rotAmt + (_rotSpeed * Time.deltaTime)) % 360;
            transform.eulerAngles += transform.up * _rotAmt;


            UpdateSatMetrics(eciSDP4);
        }
    }

    void UpdateSatMetrics(EciTime eciSDP4)
    {
        Site cheltenham = new Site(51.9, -2.0834, 0);

        // Now get the "look angle" from the site to the satellite. 
        Topo topoLook = cheltenham.GetLookAngle(eciSDP4);

        //update some metrics for the satellite for info
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

}

