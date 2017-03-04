using UnityEngine;
using System.Collections;
using System;

public class RoboCell : MonoBehaviour
{

    public enum TargetKind { ALL, NEUTRAL, GOOD, BAD}

    public TargetKind kind = TargetKind.ALL ;
        

    GameObject target;
    public float damage = 20;
    public float speed = 2;

    bool searchingForTargets = false;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Rigidbody2D>().angularVelocity = 0;
        if (target != null)
        {
            MoveTowardsTarget();
        }
        else
        {
            if(!searchingForTargets)
            {
                StartCoroutine(SearchForTargets());
            }
        }
    }

    IEnumerator SearchForTargets()
    {
        searchingForTargets = true;
        while (target == null)
        {
            Cell[] cells = GameObject.FindObjectsOfType<Cell>();
            int lowestIndex = -1;
            int i = 0;
            float distance = 99999;
            foreach(Cell c in cells)
            {
                if (kind == TargetKind.ALL 
                    || (c.cellType == Cell.CELLTYPE.Bad && kind == TargetKind.BAD)
                    || (c.cellType == Cell.CELLTYPE.Good && kind == TargetKind.GOOD)
                    || (c.cellType == Cell.CELLTYPE.Neutral && kind == TargetKind.NEUTRAL))
                {

                    float d = Vector3.Distance(transform.position, c.transform.position);
                    if (d < distance)
                    {
                        distance = d;
                        lowestIndex = i;
                    }
                }
                i++;
            }

            if(lowestIndex!=-1)
            {
                target = cells[lowestIndex].gameObject;
            }

            yield return new WaitForSeconds(1);
        }

        searchingForTargets = false;
    }

    private void MoveTowardsTarget()
    {
        Vector3 dir = target.transform.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        transform.Translate((target.transform.position - transform.position).normalized * Time.deltaTime * speed, Space.World);

    }

    void OnCollisionStay2D(Collision2D hit)
    {
        Cell c = hit.collider.gameObject.GetComponent<Cell>();
        if(c)
        {
            c.ApplyDeathRay(damage, Time.deltaTime);
        }

    }
}
