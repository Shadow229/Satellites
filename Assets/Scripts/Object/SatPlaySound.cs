using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SatPlaySound : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("DEBUG: Trigger has been entered by: " + other.tag.ToString());
        if (other.tag.Equals("MainCamera"))
        {
            //Debug.Log("DEBUG: Hit camera - playing sound");
            GetComponentInParent<AudioSource>().Play();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Debug.Log("DEBUG: Trigger has been exited by: " + other.tag.ToString());
        if (other.tag.Equals("MainCamera"))
        {
            //Debug.Log("DEBUG: Hit ended - stop playing sound");
            GetComponentInParent<AudioSource>().Stop();
        }
    }
}
