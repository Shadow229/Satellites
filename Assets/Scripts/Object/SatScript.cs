using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zeptomoby.OrbitTools;




public class SatScript : MonoBehaviour
{
    //Satellite information
    public string TLE1, TLE2, TLE3;
    public float Lat, Long, Altitude, Azmuth, Elevation, Range;

    [SerializeField]
    private float _rotSpeed;

    GameObject earth;
    Earth earthScript;
    SatManager satManager;


    Satellite sat;


    // Start is called before the first frame update
    void Start()
    {
        earth = GameObject.Find("Earth");
        earthScript = earth.GetComponent<Earth>();
        satManager = GameObject.Find("SatelliteManager").GetComponent<SatManager>();

        // generate TLE object
        Tle tle = new Tle(TLE1, TLE2, TLE3);

        // Create an orbit object using the SDP4/SGP4 TLE object.
        sat = new Satellite(tle);

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
        _rotSpeed = UnityEngine.Random.Range(0.5f, 1.5f);

        //face earth
        Vector3 direction = (earth.transform.position - transform.position).normalized;
        transform.up = direction;

    }

    public void UpdateSatellite()
    {

        if (sat != null)
        {        
            //if there is an error with the orbit (ie, the sat is no longer in orbit and the TLE reflects no orbit trajectory)
            if (sat.Orbit.isError)
            {
                //debug log
                Debug.Log("Removed Satellite: " + TLE1.ToString());
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

            //scale position vector inline with earth scale
            position /= earthScript.ScaleAmount;

            //get scale
            float scale = earthScript.GetScale();

            //update position
            transform.position = position +  new Vector3(0f, scale * 3, 0f); ;

            //trim it for local scale (this is due to the models being scaled differently at creation)
            scale /= 200;

            //update scale
            transform.localScale = new Vector3(scale, scale, scale);

            //rotate around
            Quaternion rotationAdd = Quaternion.AngleAxis(_rotSpeed, transform.up);

            //face earth
            Vector3 direction = earth.transform.position - transform.position;
            Quaternion toRotation = Quaternion.FromToRotation(transform.up, direction) * transform.rotation;

            //combine rotations
            Quaternion FinalRot = toRotation * rotationAdd;

            //apply rotation
            transform.rotation = FinalRot;

            UpdateSatMetrics(eciSDP4);
        }
    }

    void UpdateSatMetrics(EciTime eciSDP4)
    {

        Site site;

        if (satManager.userLocInfo.Set == false)
        {
            //cheltenham default if no location is set via GPS
            site = new Site(51.9, -2.0834, 0);
        }
        else
        {
            site = new Site(satManager.userLocInfo.Latitude, satManager.userLocInfo.Longitude, satManager.userLocInfo.Altitude);
        }


        // Now get the "look angle" from the site to the satellite. 
        Topo topoLook = site.GetLookAngle(eciSDP4);

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
            Altitude = hit.distance * earthScript.ScaleAmount;
        }

            return alt;
    }



}

