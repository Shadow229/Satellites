using UnityEngine;
using System.Collections;
using System;
using TMPro;
using UnityEngine.UI;

[System.Serializable]


/// <summary>
/// Spin the object at a specified speed
/// </summary>
public class Earth : MonoBehaviour {

    //constants

    //earth is 12742km
    private const float _diameter = 12742;
    //86400 seconds in a day
	private const float rotationTime = 86400;

    [HideInInspector]
    public DateTime TimeNow;
    [HideInInspector]
    public float ScaleAmount;
    [SerializeField]
    private float _scale = 0.3f;
    public float YOrigin;
    public float HeightMultiplier = 0.3f;
    public float DefaultHeight = 1;

	[HideInInspector]
	public DateTime dtGMT;

	[SerializeField]
	private float remainingTime;
    public bool UpdateDateTime = false;

    Quaternion AxisTilt;

    private float _spinSpeed;
    private bool _showDateTime = true;

    private GameObject CurrentDtTime;
    private bool _initialised = false;

    //helper functions
    public void SetScale(float s) { _scale = s; }
    public float GetScale() { return _scale; }


    public void Init()
    {
        //Debug.Log("DEBUG: Earth Init: " + _initialised.ToString());
        //can potentially be ran multiple times from changing placement of AR position
        if (!_initialised)
        {
            //Debug.Log("DEBUG: Initialising Earth!");

            _initialised = true;
            AxisTilt = Quaternion.Euler(-0.29f, -37.65f, 0.4f);
            InitEarthPosRot();
            //calculate the rotation amount
            _spinSpeed = 360.0f / rotationTime;
            //set the scale
            UpdateScale(_scale);

            TimeNow = DateTime.Now;

            CurrentDtTime = GameObject.Find("CurrentDtTime");

            //initialise sat checker on camera
            Camera.main.GetComponent<SatChecker>().Init();
        }
        
    }

    private void FixedUpdate()
    { 
        UpdateDateTimeUI();
    }

    // Update is called once per frame
    void Update()
    { 
        UpdateAutoRotation();
	}


    private void UpdateAutoRotation()
    {
        //set remaining time
        remainingTime = rotationTime - TimeinSeconds(TimeNow.Hour, TimeNow.Minute, TimeNow.Second);

        //rotate earth
        transform.Rotate(transform.up, _spinSpeed * Time.deltaTime);
    }


    void InitEarthPosRot()
    {
        //if its switched to realtime - reinitialise the correct rotation of the earth
        remainingTime = rotationTime - TimeinSeconds(TimeNow.Hour, TimeNow.Minute, TimeNow.Second);

        float distDeg = _spinSpeed * remainingTime;

        transform.rotation = AxisTilt * Quaternion.AngleAxis(distDeg, Vector3.up);

    }


    public void UpdateScale(float a_scale)
    {
        //set scale
        _scale = a_scale;

        //set uniform scale
        transform.localScale = new Vector3(_scale, _scale, _scale);

        //update global scale amount (used for satelite positioning)
        ScaleAmount = _diameter / _scale;

        //raise position to accomodate scaled objects
        transform.position = new Vector3(transform.position.x, YOrigin + DefaultHeight + (_scale * HeightMultiplier), transform.position.z);
    }


    public void DateTimeToggle(bool val)
    {
        _showDateTime = val;

        //if reactivating - reset time
        if (_showDateTime)
        {
            CurrentDtTime.SetActive(true);
            TimeNow = System.DateTime.Now;
        }
        else
        {
            CurrentDtTime.SetActive(false);
        }
    }

    private void UpdateDateTimeUI()
    {
        if (_showDateTime && _initialised)
        {
                int day, month, year, hour, min, sec;

                //update of delta time to save querying to CPU time each frame
                TimeNow = TimeNow.AddSeconds(Time.deltaTime);

                day = TimeNow.Day;
                month = TimeNow.Month;
                year = TimeNow.Year;
                hour = TimeNow.Hour;
                min = TimeNow.Minute;
                sec = TimeNow.Second;

                //format and print
                CurrentDtTime.GetComponent<TextMeshProUGUI>().text = string.Format("{0:00}/{1:00}/{2:0000} {3:00}:{4:00}:{5:00}", day, month, year, hour, min, sec);
        }
      
    }



    int TimeinSeconds(int h, int m, int s)
    {
        int t;

        t = (h * 60 * 60) + (m * 60) + s;

        return t;
    }

}
