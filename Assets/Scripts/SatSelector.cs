using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SatSelector : MonoBehaviour
{
    bool ShowInfo = false;
    public GameObject Satellite;
    private void Update()
    {
#if UNITY_IOS

        //update touch
        // Handle screen touches.
        if (Input.touchCount > 0)
        {
            //the screen has been touched - show the
        }

#else
        //update mouse


#endif
    }
}
