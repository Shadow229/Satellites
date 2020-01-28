using UnityEngine;
using System.Collections;
using System;

[System.Serializable]
public struct DtTimeStruct
{
	public int	Year,
				Month,
				Day,
				Hours,
				Mins,	
				Seconds;
}


/// <summary>
/// Spin the object at a specified speed
/// </summary>
public class Earth : MonoBehaviour {
	[HideInInspector]
	public bool spin;

	[Tooltip("Time in seconds for earth to rotate once")]
	public float rotationTime = 86400;

	public bool realTime = true;
	public DtTimeStruct GMT;

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

    [HideInInspector]
    Quaternion originalRot;


	private float speed = 10f;


    private void Start()
    {
        originalRot = transform.rotation;
    }

    // Update is called once per frame
    void Update() {

		CheckGMT();

		if (!realTime)
		{
			spin = false;
		}
		else
		{
			spin = true;
		}
		dtGMT = new DateTime(GMT.Year, GMT.Month, GMT.Day, GMT.Hours, GMT.Mins, GMT.Seconds);

		//update remaining time of rotation
		if (!realTime)
		{
			remainingTime = rotationTime - TimeinSeconds(GMT.Hours, GMT.Mins, GMT.Seconds);
		}
		else
		{
			if (remainingTime <= 0)
			{
				remainingTime = rotationTime;
			}
			else
			{
				remainingTime -= Time.deltaTime;
			}
		}

		//calculate the rotation amount
		speed = 360.0f / rotationTime;

		
		if (direction < 1f) {
			direction += Time.deltaTime / (directionChangeSpeed / 2);
		}

		if (spin) {
			if (clockwise) {
					transform.Rotate(transform.up, (speed * direction) * Time.deltaTime);
			} else {
					transform.Rotate(-transform.up, (speed * direction) * Time.deltaTime);
			}
		}
		else
		{
            float distDeg = speed * direction * remainingTime;

			//Vector3 dir = (transform.up * (speed * direction) * remainingTime);
           // transform.eulerAngles = new Vector3(dir.x, dir.y, dir.z);
            transform.rotation = originalRot * Quaternion.AngleAxis(distDeg, Vector3.up);

        }
	}


	int TimeinSeconds(int h, int m, int s)
	{
		int t;

		t = (h * 60 * 60) + (m * 60) + s;

		return t;
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
	}
}
