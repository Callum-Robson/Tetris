using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PrefabOverrider : MonoBehaviour
{
    public GameObject[] prefabs = new GameObject[12];
    // Start is called before the first frame update
    void Start()
    {
        foreach(GameObject prefab in prefabs)
        {
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);   //Instantiate<GameObject>(prefab);
            instance.name = prefab.name;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
