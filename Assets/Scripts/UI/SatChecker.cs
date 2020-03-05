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
    private bool _initialised = false;

    private bool HighlightVisibile = false;

    public GameObject GetVisibleSat() { return VisibleSat; }

    public void Init()
    {
        //satellite layer is layer 8
        satLayer = LayerMask.GetMask("Satellite");

        //instantiate highlight
        HighlightRing = Instantiate(HighlightRingPrefab, Vector3.zero, Quaternion.identity);
        float initScale = GameObject.Find("Earth").GetComponent<Earth>().GetScale();
        SetHightlightRingScale(initScale * 0.5f);
        HighlightVisibile = false;
        HighlightRing.SetActive(false);

        _initialised = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (_initialised)
        {
            CheckForSat();
        }

    }

    public void SetHightlightRingScale(float scale)
    {
        HighlightRing.transform.localScale = new Vector3(scale, scale, scale);
    }

    private void CheckForSat()
    {
        Ray checkRay = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        if (Physics.Raycast(checkRay, out hit, Mathf.Infinity, satLayer))
        {
            Debug.DrawLine(transform.position, hit.point, Color.red);

            if (VisibleSat != hit.transform.gameObject || VisibleSat == null)
            {
                //float distance = hit.distance;
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

            if (HighlightVisibile == true)
            {
                HighlightVisibile = false;
                //hide the UI
                if (satUI.UIVisible) {satUI.HideUI();}

                HighlightRing.SetActive(false) ;
                VisibleSat = null;                
            }

        }
    }


    private void Highlight()
    {
        HighlightRing.GetComponent<Billboard>().satParent = VisibleSat;
        HighlightRing.transform.position = VisibleSat.transform.position;
        HighlightVisibile = true;
        HighlightRing.SetActive(true);
    }
}
