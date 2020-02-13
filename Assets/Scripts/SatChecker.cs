using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SatChecker : MonoBehaviour
{

    public GameObject HighlightRingPrefab;
    private GameObject VisibleSat;
    private GameObject HighlightRing;
    private LayerMask satLayer;
    public SatUI satUI;


    public GameObject GetVisibleSat() { return VisibleSat; }

    private void Start()
    {
        //satellite layer is layer 8
        satLayer = 1 << 8;
    }

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

            if (VisibleSat != hit.transform.gameObject || VisibleSat == null)
            {
                float distance = hit.distance;
                VisibleSat = hit.transform.gameObject;
                //Debug.Log("Hit: " + hit.transform.gameObject.GetComponent<SatScript>().TLE1.ToString());
                //Debug.Log("Hit Distanct: " + distance.ToString());
                Debug.DrawLine(transform.position, hit.point, Color.cyan);
                Highlight();
            }
            else if (VisibleSat == hit.transform.gameObject)
            {
                //update the UI
                satUI.UpdateUI(VisibleSat);
            }
        }
        else
        {
            Debug.DrawRay(transform.position, checkRay.direction, Color.blue);

            if (HighlightRing != null)
            {
                //hide the UI
                satUI.HideUI();
                GameObject.Destroy(HighlightRing);
                VisibleSat = null;
            }

        }
    }


    private void Highlight()
    {
        if (HighlightRing == null)
        {
            //instantiate highlight
            HighlightRing = Instantiate(HighlightRingPrefab, VisibleSat.transform.position, Quaternion.identity);
            HighlightRing.transform.LookAt(Camera.main.transform);
            HighlightRing.GetComponent<Billboard>().satParent = VisibleSat;
        }
        else
        {
            HighlightRing.GetComponent<Billboard>().satParent = VisibleSat;
            HighlightRing.transform.position = VisibleSat.transform.position;
        }

    }
}
