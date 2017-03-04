using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceField : MonoBehaviour
{

    List<Rigidbody2D> ridgs = new List<Rigidbody2D>();
    public float force = 10;

    // Use this for initialization
    void Start()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, 4);
     //   GetComponent<Renderer>().material.mainTextureScale = new Vector2(50, 20); // new Vector2(-transform.localScale.x*0.5f, 1);
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Rigidbody2D r in ridgs)
        {
            if(r!=null)
                r.AddForce(new Vector2(transform.right.x, transform.right.y) * Time.deltaTime * force);
        }
    }


    void OnTriggerEnter2D(Collider2D hit)
    {
        Rigidbody2D rig = hit.gameObject.GetComponent<Rigidbody2D>();
        if (rig != null)
        {
            ridgs.Add(rig);
        }
    }

    void OnTriggerExit2D(Collider2D hit)
    {

        Rigidbody2D rig = hit.gameObject.GetComponent<Rigidbody2D>();
        if (rig != null)
        {

            rig.velocity = Vector2.zero;
            rig.angularVelocity = 0;
            ridgs.Remove(rig);
        }
    }
}
