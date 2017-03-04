using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrabber2 : MonoBehaviour
{

    public KeyCode key = KeyCode.Space;
    bool isDragging = false;
    Cell lastCell;
    int lastLayer;
    public GameObject opticalFeedback;

    void OnTriggerEnter2D(Collider2D hit)
    {
        if (lastCell != null)
            return;

        Cell c = hit.GetComponent<Cell>();
        if (c)
        {
            lastCell = c;
            //Andock(lastCell.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D hit)
    {
        Cell c = hit.GetComponent<Cell>();
        if (c)
        {
            if (lastCell == c)
            {
                Abdock(lastCell.gameObject);
                lastCell = null;
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(key))
        {
            if (lastCell != null)
                Andock(lastCell.gameObject);
            
            if (opticalFeedback)
            {
                opticalFeedback.SetActive(true);
            }
        }

        if (Input.GetKeyUp(key))
        {
            if (lastCell != null)
                Abdock(lastCell.gameObject);
            
            if (opticalFeedback)
            {
                opticalFeedback.SetActive(false);
            }
        }
    }
        
    void Andock(GameObject g)
    {
        g.transform.parent = transform;
        lastLayer = g.layer;
        foreach (Transform child in g.transform)
        {
            //child is your child transform
            child.gameObject.layer = LayerMask.NameToLayer("Distortion");
        }
        g.layer = LayerMask.NameToLayer("Distortion");
    }

    void Abdock(GameObject g)
    {
        foreach (Transform child in g.transform)
        {
            //child is your child transform
            child.gameObject.layer = lastLayer;
        }
        g.layer = lastLayer;
        g.transform.parent = null;
    }


}
