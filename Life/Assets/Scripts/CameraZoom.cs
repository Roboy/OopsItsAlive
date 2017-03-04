using UnityEngine;
using System.Collections;
using System;

public class CameraZoom : MonoBehaviour {

    public float speed = 1;

    public Vector2 minMax = new Vector2(1, 10);

    public KeyCode zoomIn = KeyCode.E;
    public KeyCode zoomOut = KeyCode.Q;

    private Camera cam;
	
    void Start()
    {
        cam = GetComponent<Camera>();
        cam.orthographicSize = 5f;
    }

	// Update is called once per frame
	void Update () {
        HandleZoomInput();
	}

    private void HandleZoomInput()
    {
        if (Input.GetKey(zoomIn))
        {
            cam.orthographicSize += Time.deltaTime * speed;

            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minMax.x, minMax.y);
        }
        else
            if (Input.GetKey(zoomOut))
        {
            cam.orthographicSize -= Time.deltaTime * speed;

            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minMax.x, minMax.y);
        }
    }
}
