using UnityEngine;
using System.Collections;
using System;
public class RangeLimiter : MonoBehaviour {

    public Vector3 origin = new Vector3(0,0,0);
    public float maxRange = 1000;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 currentPosition = transform.position;
	    if (Vector3.Distance(origin, currentPosition) > maxRange)
        {
            if(Math.Abs(currentPosition.x - origin.x) > maxRange)
            {
                transform.position = new Vector3(maxRange, currentPosition.y, currentPosition.z);
            }
            if (Math.Abs(currentPosition.y - origin.y) > maxRange)
            {
                transform.position = new Vector3(currentPosition.x, maxRange, currentPosition.z);
            }
            if (Math.Abs(currentPosition.z - origin.z) > maxRange)
            {
                transform.position = new Vector3(currentPosition.x, currentPosition.y, maxRange);
            }
        }

	}
}
