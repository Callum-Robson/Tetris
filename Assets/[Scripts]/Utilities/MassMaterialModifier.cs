using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MassMaterialModifier : MonoBehaviour
{
    public Material[] materials;
    [Header("Only check one box, will default to set by percentage if both are checked")]
    [Header("To set all material's saturation to the same value. Range is 0.0 to 1.0")]
    public float newSaturation;
    public bool setByValue;
    [Header("To change all saturation values by percentage.")]
    public float saturationChangePercent;
    public bool setByPercent;
    // Start is called before the first frame update
    void Start()
    {
        string[] oldValues = new string[materials.Length];
        int i = 0;
        if (setByPercent)
        {
            setByValue = false;
        }
        foreach (Material mat in materials)
        {
            Color oldColor = mat.color;
            oldValues[i] = oldColor.r.ToString() + "," + oldColor.g.ToString() + "," + oldColor.b.ToString() +  ",";
            i++;
            float hue, sat, val;
            Color.RGBToHSV(oldColor, out hue, out sat, out val);
            if (setByValue)
            {
                //mat.color = Color.HSVToRGB(hue, newSaturation, val);
            }
            else if (setByPercent)
            {
                //mat.color = Color.HSVToRGB(hue, sat * saturationChangePercent, val);
            }
        }
        BackUpOldValues(oldValues);
    }

    private void BackUpOldValues(string[] values)
    {
        string path = "Assets/Resources/MatColorsBackupCSV_test.txt";

        //Write some text to the test.txt file

        StreamWriter writer = new StreamWriter(path, true);

        string csv = "";

        foreach (string str in values)
        {
            csv += str;
        }

        writer.WriteLine(csv);

        writer.Close();
    }

}
