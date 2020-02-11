using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SatChecker : MonoBehaviour
{

    public GameObject HighlightRingPrefab;
    public GameObject VisbileSat;
    public GameObject HighlightRing;
    public LayerMask satLayer;

    // Update is called once per frame
    void Update()
    {
        CheckForSat();
    }


    private void CheckForSat()
    {
        Ray checkRay = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        if (Physics.Raycast(checkRay, out hit, satLayer))
        {
            Debug.DrawLine(transform.position, hit.point, Color.red);

            if (VisbileSat != hit.transform.gameObject || VisbileSat == null)
            {
                float distance = hit.distance;
                VisbileSat = hit.transform.gameObject;
                //Debug.Log("Hit: " + hit.transform.gameObject.GetComponent<SatScript>().TLE1.ToString());
                //Debug.Log("Hit Distanct: " + distance.ToString());
                Debug.DrawLine(transform.position, hit.point, Color.cyan);
                Highlight();
            }
        }
        else
        {
            Debug.DrawRay(transform.position, checkRay.direction, Color.blue);

            if (HighlightRing != null)
            {
                GameObject.Destroy(HighlightRing);
                VisbileSat = null;
            }

        }
    }


    private void Highlight()
    {
        if (HighlightRing == null)
        {
            //instantiate highlight
            HighlightRing = Instantiate(HighlightRingPrefab, VisbileSat.transform.position, Quaternion.identity);
            HighlightRing.transform.LookAt(Camera.main.transform);
            HighlightRing.GetComponent<Billboard>().satParent = VisbileSat;
        }
        else
        {
            HighlightRing.transform.position = VisbileSat.transform.position;
        }

    }
}
