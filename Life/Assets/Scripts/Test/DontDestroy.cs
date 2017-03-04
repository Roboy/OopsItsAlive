using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DontDestroy : MonoBehaviour {

    public string id = "default";
    public static List<string> existing;
    // Use this for initialization
    void Start()
    {

        if (existing == null)
            existing = new List<string>();

        if (existing.Contains(id))
        {
            Destroy(this.gameObject);
        }
        else
        {
            existing.Add(id);

            DontDestroyOnLoad(this.gameObject);
        }
    }
	
}
