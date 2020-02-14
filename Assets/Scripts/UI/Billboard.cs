using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
     private Camera cam;
    public GameObject satParent;

    private void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        //set position relative to sat
        if (satParent != null)
        {
            transform.position = satParent.transform.position;
        }

        //rotate around to look cool
        Quaternion rot = transform.rotation;
        transform.LookAt(cam.transform);
        Quaternion lookRot = transform.rotation;
        transform.rotation = rot;

        //rotate around
        Quaternion rotationAdd = Quaternion.AngleAxis(2.5f, transform.up) * lookRot;


        //apply rotation
        transform.rotation = rotationAdd;
    }

}
