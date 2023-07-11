using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LoadMaterialColors : MonoBehaviour
{
    public int numberOfColors;
    [Header("Must be in same order as colors in txt document")]
    public Material[] materials;

    // Start is called before the first frame update
    void Start()
    {
        string path = "Assets/Resources/MatColorsLoad.txt";
        StreamReader reader = new StreamReader(path);
        
        while (!reader.EndOfStream)
        {
            string line = reader.ReadLine();
            string[] values = line.Split(',');

            int valueIterator = 0;
            int colorIterator = 0;

            string r = "";
            string g = ""; 
            string b = "";

            // Process the values
            foreach (string value in values)
            {
                if (valueIterator == 0)
                {
                    r = value;
                }

                else if (valueIterator == 1)
                {
                    g = value;
                }

                else if (valueIterator == 2)
                {
                    b = value;
                }



                valueIterator++;

                if (valueIterator > 2)
                {
                    materials[colorIterator].color = new Color(float.Parse(r), float.Parse(g), float.Parse(b));
                    //Debug.Log("Material color #" + colorIterator + " = " + r + ", " + g + ", " + b);
                    valueIterator = 0;
                    colorIterator++;
                }
            }
        }
    }
}


/*
 *     
 *      
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 */