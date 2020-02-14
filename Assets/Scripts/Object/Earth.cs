using UnityEngine;
using System.Collections;
using System;
using TMPro;

[System.Serializable]

//change some of this for datetime functions - it'll handle the speed multipliers better and save on manual calcs and the try catch with out of date month params!
public class DtTime
{
	public int	Year,
				Month,
				Day,
				Hours,
				Mins,	
				Seconds;

    public DtTime() {}
    public DtTime(DateTime DtTime)
    {
        SetDateTime(DtTime);
    }

    public void SetDateTime(DateTime DtTime)
    {
        Year = DtTime.Year;
        Month = DtTime.Month;
        Day = DtTime.Day;
        Hours = DtTime.Hour;
        Mins = DtTime.Minute;
        Seconds = DtTime.Second;
    }
}


/// <summary>
/// Spin the object at a specified speed
/// </summary>
public class Earth : MonoBehaviour {

    [HideInInspector]
    public DateTime TimeNow;

    //[Tooltip("Time in seconds for earth to rotate once")]
    [HideInInspector]
	public float rotationTime = 86400;

    //earth is 12742km - we need it about 0.4 unity units for AR.
    private readonly float _diameter = 12742;
    [HideInInspector]
    public float ScaleAmount;
    [SerializeField]
    private float Scale = 0.4f;

	public bool realTime = true;
	public DtTime GMT;
    public bool SetGMTNow = true;
    public bool Play = false;
    [Range(-2000,2000)]
    public int SpeedMultiplier = 1;

    private float DeltaAcc = 0.0f;

	[HideInInspector]
	public DateTime dtGMT;

	[SerializeField]
	private float remainingTime;
    private bool showDateTime;

	[HideInInspector]
	public bool clockwise = true;
	[HideInInspector]
	public float directionChangeSpeed = 2f;

    Quaternion AxisTilt;

    private float spinSpeed = 10f;

    public GameObject CurrentDtTime;

    public float SpawnElevation = 0.5f;

    public void SetScale(float s) { Scale = s; }

    private void Start()
    {
        AxisTilt = Quaternion.Euler(-0.29f, -37.65f, 0.4f);
        InitEarthPosRot();
        //calculate the rotation amount
        spinSpeed = 360.0f / rotationTime;

        TimeNow = DateTime.Now;
    }

    private void FixedUpdate()
    { 
        UpdateDatTimeUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (realTime)
        {
            UpdateAutoRotation();
        }
        else
        {
            UpdateManualRotation();
        }

        transform.position = new Vector3(0f, Scale * 3, 0f);
	}


    private void UpdateManualRotation()
    {
        //set manual time
        try
        {
            dtGMT = new DateTime(GMT.Year, GMT.Month, GMT.Day, GMT.Hours, GMT.Mins, GMT.Seconds);
        }
        catch (ArgumentOutOfRangeException)
        {
            throw;
        }
        
        //update remaining time
        remainingTime = rotationTime - TimeinSeconds(GMT.Hours, GMT.Mins, GMT.Seconds);

        //get rotation amount
        float distDeg = spinSpeed * remainingTime;

        //rotate earth
        transform.rotation = AxisTilt * Quaternion.AngleAxis(distDeg, Vector3.up);

        //if play is selected, resume from date set
        if (Play)
        {
            //acrue delta time
            DeltaAcc += Time.deltaTime * SpeedMultiplier;

            if (Math.Abs((DeltaAcc)) >= 1)
            {
                //add complete seconds onto the GMT
                GMT.Seconds += (int)DeltaAcc;
                DeltaAcc -= (int)DeltaAcc;
            }
        }

    }

    private void UpdateAutoRotation()
    {
        //set remaining time
        remainingTime = rotationTime - TimeinSeconds(TimeNow.Hour, TimeNow.Minute, TimeNow.Second);

        //rotate earth
        transform.Rotate(transform.up, spinSpeed * Time.deltaTime);
    }


	int TimeinSeconds(int h, int m, int s)
	{
		int t;

		t = (h * 60 * 60) + (m * 60) + s;

		return t;
	}

    void InitEarthPosRot()
    {
        //if its switched to realtime - reinitialise the correct rotation of the earth
        if (realTime)
        {
            remainingTime = rotationTime - TimeinSeconds(TimeNow.Hour, TimeNow.Minute, TimeNow.Second);

            float distDeg = spinSpeed * remainingTime;

            transform.rotation = AxisTilt * Quaternion.AngleAxis(distDeg, Vector3.up);
        }
    }


    private void OnValidate()
    {
        InitEarthPosRot();
        UpdateScale();

        if (SetGMTNow)
        {
            GMT.SetDateTime(TimeNow);
            SetGMTNow = false;
        }
    }


    private void UpdateScale()
    {
        //set uniform scale
        transform.localScale = new Vector3(Scale, Scale, Scale);

        //update global scale amount (used for satelite positioning)
        ScaleAmount = _diameter * transform.localScale.x;
    }


    public void DateTimeToggle(bool val)
    {
        showDateTime = val;

        //if reactivating - reset time
        if (showDateTime)
        {
            CurrentDtTime.SetActive(true);
            TimeNow = System.DateTime.Now;
        }
        else
        {
            CurrentDtTime.SetActive(false);
        }
    }

    private void UpdateDatTimeUI()
    {

        if (showDateTime)
        {
                int day, month, year, hour, min, sec;

                TimeNow = TimeNow.AddSeconds(Time.deltaTime);

                day = TimeNow.Day;
                month = TimeNow.Month;
                year = TimeNow.Year;
                hour = TimeNow.Hour;
                min = TimeNow.Minute;
                sec = TimeNow.Second;

                CurrentDtTime.GetComponent<TextMeshProUGUI>().text = string.Format("{0:D2}/{1:D2}/{2:D4} {3:D2}:{4:D2}:{5:D2}", day, month, year, hour, min, sec);
        }
      
    }

}
