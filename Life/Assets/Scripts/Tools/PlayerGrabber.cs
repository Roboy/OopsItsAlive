using UnityEngine;
using System.Collections;
using System;

public class PlayerGrabber : MonoBehaviour
{

    private Cell lastCellCollided;
    private bool isDragging = false;
    private Transform backupParent;
    public GameObject opticalFeedback;

    public KeyCode keyToDrag = KeyCode.Space;
    public float maxDistanceUntilRelease = 1;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(keyToDrag))
        {
            StartDrag();

            if (opticalFeedback)
            {
                opticalFeedback.SetActive(true);
            }
        }

        if (Input.GetKeyUp(keyToDrag))
        {

            if (opticalFeedback)
            {
                opticalFeedback.SetActive(false);
            }
            StopDrag();
        }

        CheckForMaxDistance();

        if(isDragging && lastCellCollided == null)
        {
            StopDrag();
        }
    }

    // IGNORED FOR NOW
    private void CheckForMaxDistance()
    {
        return;


        if (lastCellCollided == null)
            return;
        if (!isDragging)
            return;

        if (Vector3.Distance(transform.position, lastCellCollided.transform.position) > maxDistanceUntilRelease*lastCellCollided.gameObject.transform.localScale.magnitude)
        {
            StopDrag();
            lastCellCollided = null;
        }
    }

    private void StopDrag()
    {
        if (isDragging)
        {
            isDragging = false;

            if (lastCellCollided == null)
                return;

            //lastCellCollided.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
            lastCellCollided.transform.parent = backupParent;
            lastCellCollided =null;
        }
    }

    private void StartDrag()
    {
        if (!isDragging && lastCellCollided != null)
        {
            isDragging = true;
            backupParent = lastCellCollided.transform.parent;
            lastCellCollided.gameObject.transform.parent = transform;

           // lastCellCollided.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }

    void OnTriggerEnter2D(Collider2D hit)
    {
        lastCellCollided = hit.gameObject.GetComponent<Cell>();

        //if (lastCellCollided == null)
        //{
        //    lastCellCollided = hit.gameObject.GetComponent<Cell>();
        //}
    }

    void OnTriggerExit2D(Collider2D hit)
    {

        if (lastCellCollided != null)
        {
            Cell c = hit.gameObject.GetComponent<Cell>();
            if (!c)
                return;
            if (c == lastCellCollided)
            {
                lastCellCollided = null;
                StopDrag();
            }
        }

        //if (lastCellCollided != null)
        //{
        //    Cell c = hit.gameObject.GetComponent<Cell>();
        //    if (c == null)
        //        return;

        //    if (c == lastCellCollided && !isDragging)
        //    {
        //        lastCellCollided = null;
        //    }
        //}
    }


}
