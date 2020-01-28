using UnityEngine;
using System.Collections;
using System;

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

    public void UpdateMonth()
    {

        if (Day > 29)
        {
            Month++;
            Day = 1;
            Hours = 0;
            Mins = 0;
            Seconds = 0;
        }
        else
        {
            Month--;

            Hours = 23;
            Mins = 59;
            Seconds = 59;
        }
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

	[HideInInspector]
	public bool clockwise = true;
	[HideInInspector]
	public float direction = 1f;
	[HideInInspector]
	public float directionChangeSpeed = 2f;

    Quaternion AxisTilt;

   // public Vector3 rotationVector;
  //  Quaternion originalRot;



    private float speed = 10f;


    private void Start()
    {
        AxisTilt = Quaternion.Euler(-0.29f, -37.65f, 0.4f);
    }

    private void FixedUpdate()
    {
        TimeNow = DateTime.Now;
    }

    // Update is called once per frame
    void Update() {

        if (SetGMTNow)
        {
            GMT.SetDateTime(TimeNow);
            SetGMTNow = false;
        }

        try
        {
		    dtGMT = new DateTime(GMT.Year, GMT.Month, GMT.Day, GMT.Hours, GMT.Mins, GMT.Seconds);
        }
        catch (ArgumentOutOfRangeException)
        {
            GMT.UpdateMonth();
            CheckGMT();
            throw;
        }


		//update remaining time of rotation
		if (!realTime)
		{
			remainingTime = rotationTime - TimeinSeconds(GMT.Hours, GMT.Mins, GMT.Seconds);
		}
		else
		{
			remainingTime = rotationTime - TimeinSeconds(TimeNow.Hour, TimeNow.Minute, TimeNow.Second);
		}	     

		//calculate the rotation amount
		speed = 360.0f / rotationTime;

		
		if (direction < 1f) {
			direction += Time.deltaTime / (directionChangeSpeed / 2);
		}

		if (realTime) 
        {
			transform.Rotate(transform.up, (speed * direction) * Time.deltaTime);
		}
		else
		{
            float distDeg = speed * direction * remainingTime;

          //  originalRot = Quaternion.Euler(rotationVector);

            //transform.rotation = originalRot * Quaternion.AngleAxis(distDeg, Vector3.up);

            //End code after changes
            transform.rotation = AxisTilt * Quaternion.AngleAxis(distDeg, Vector3.up);

        }

        if (Play)
        {
            //acrue delta time
            DeltaAcc += Time.deltaTime * SpeedMultiplier;

            if (Math.Abs((DeltaAcc)) >= 1)
            {
                //add complete seconds onto the GMT
                GMT.Seconds += (int)DeltaAcc;
                DeltaAcc -= (int)DeltaAcc;

                //check GMT for rollover
                CheckGMT();
            }
          
        }
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

            //calculate the rotation amount
            speed = 360.0f / rotationTime;

            float distDeg = speed * direction * remainingTime;

            transform.rotation = AxisTilt * Quaternion.AngleAxis(distDeg, Vector3.up);
        }
    }


    private void OnValidate()
    {
        CheckGMT();
        InitEarthPosRot();
    }

    void CheckGMT()
	{
		if (GMT.Seconds < 0)
		{
			GMT.Mins -= 1;
			GMT.Seconds = 59;
		}
		if (GMT.Seconds > 59)
		{
			GMT.Mins += 1;
			GMT.Seconds = 0;
		}

		if (GMT.Mins < 0)
		{
			GMT.Hours -= 1;
			GMT.Mins = 59;
		}
		if (GMT.Mins > 59)
		{
			GMT.Hours += 1;
			GMT.Mins = 0;
		}

		if (GMT.Hours < 0)
		{
			GMT.Day -= 1;
			GMT.Hours = 23;
		}
		if (GMT.Hours > 23)
		{
			GMT.Day += 1;
			GMT.Hours = 0;
		}
        if (GMT.Month < 1)
        {
            GMT.Day -= 1;
            GMT.Month = 12;
        }
        if (GMT.Month > 12)
        {
            GMT.Year += 1;
            GMT.Month = 1;
        }
    }



}
