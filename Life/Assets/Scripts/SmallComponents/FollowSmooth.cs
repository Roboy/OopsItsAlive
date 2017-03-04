using UnityEngine;
using System.Collections;
using System;

public class FollowSmooth : MonoBehaviour {

    public float speed = 10;
	public Transform Player;
    public Transform mainTarget;
    public bool followAtZ = false;

	private bool targetSwitched = false;
	private float timeToSwitchBack = 0.0f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        FollowTarget();
		if (targetSwitched && timeToSwitchBack <= 0.0f) {
			mainTarget = Player;
			targetSwitched = false;
		}
	}

    private void FollowTarget()
    {
        if (mainTarget == null)
            return;

        float f = transform.position.z;
		transform.position = Vector3.Lerp(transform.position, mainTarget.position, Time.deltaTime * speed);
        if (!followAtZ)
            transform.position = new Vector3(transform.position.x, transform.position.y, f);

		timeToSwitchBack -= Time.deltaTime;

    }

	public void ChangeTarget(Transform target)
	{
		mainTarget = target;
		targetSwitched = true;
		timeToSwitchBack = 4.0f;
	}

	public void ChangeTarget20Sec(Transform target)
	{
		mainTarget = target;
		targetSwitched = true;
		timeToSwitchBack = 20.0f;
	}

	public void ChangeTarget10Sec(Transform target)
	{
		mainTarget = target;
		targetSwitched = true;
		timeToSwitchBack = 20.0f;
	}
}
