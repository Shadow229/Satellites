using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SatSearch : MonoBehaviour
{
    public TLE_Reader tle_reader;
    public TLEFile tleFile;

    private List<string> availableSats;

    private string CurrentAutoComp = "";


    public TMP_InputField InputField;
    public TextMeshProUGUI InputFeldText;
    public TextMeshProUGUI autoCompText;

    private bool SatSearchFound = false;

    private string defaultMessage = "";



    private void Awake()
    {
        InputField.onValidateInput += delegate (string input, int charIndex, char addedChar) { return SetToUpper(addedChar); };
    }

    public char SetToUpper(char c)
    {
        string str = c.ToString().ToUpper();
        char[] chars = str.ToCharArray();
        return chars[0];
    }


    public void GetAvailableSats(string a_str)
    {
        autoCompText.text = defaultMessage;
        availableSats = tle_reader.GenerateSatNames(tleFile.TrackedTLEFiles[tleFile.listIndex]);

        UpdateAutoComplete(a_str);
    }

    public void UpdateAutoComplete(string a_str)
    {

        string oldString = CurrentAutoComp;

        if (!string.IsNullOrEmpty(a_str) && a_str.Length > oldString.Length)
        {
            List<string> found = availableSats.FindAll(w => w.StartsWith(a_str));
            if (found.Count > 0)
            {
                //myText = found[0];
                InputFeldText.color = Color.black;
                autoCompText.text = found[0];
                Debug.Log(found[0]);
                Debug.Log(found.Count);
                SatSearchFound = true;
            }
            else
            {
                autoCompText.text = "";
                InputFeldText.color = Color.red;
                SatSearchFound = false;
            }
        }
        else if (string.IsNullOrEmpty(a_str))
        {
            autoCompText.text = defaultMessage;
            SatSearchFound = false;
        }
    }

    public void SetSatellite(string a_str)
    {
        if (SatSearchFound == true || InputField.text == "")
        {
            //set the auto complete
            InputField.text = autoCompText.text;

            //pass the sat value to the TLE reader for loadout
            tle_reader.SelectSatellite(InputField.text);
        }
    }


}
