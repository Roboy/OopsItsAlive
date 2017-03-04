using UnityEngine;
using System.Collections;
using System;

public class LaserBeamCoolLookingEffect : MonoBehaviour
{
    public Projectile.ProjectileType beamType = Projectile.ProjectileType.friendlyHeal;
    public float rayPower = 20;
    public float speed = -3;
    public float distance = 5;
    Material mat;
    LineRenderer myLineRenderer;
    public GameObject particles;

    // Use this for initialization
    void Start()
    {


        mat = GetComponent<Renderer>().material;
        myLineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleRayAndSetSize();
        BeamAnimation();
    }

    private void HandleRayAndSetSize()
    {
        Vector3 offset =- transform.right * 0.25f;
        RaycastHit2D hit = Physics2D.Raycast(transform.position - offset, transform.up * distance,distance);

        RaycastHit2D hit2 = Physics2D.Raycast(transform.position + offset, transform.up * distance, distance);

        if (hit2.collider != null)
        {
            offset = offset * -1;
            hit = hit2;
        }
        if (hit.collider != null)
        {
            //Debug.Log("Collides with " + hit.collider.name);
            myLineRenderer.SetPosition(1, hit.distance * Vector3.up);
            mat.SetTextureScale("_MainTex", new Vector2(hit.distance * 0.4f, 1));

            // Set Particle effect at position

            particles.transform.position = hit.point + (new Vector2( offset.x, offset.y));

            ApplyEffectOnHit(hit);
        }
        else
        {
            mat.SetTextureScale("_MainTex", new Vector2(0.4f * distance, 1));
            myLineRenderer.SetPosition(1, distance * Vector3.up);


            particles.transform.localPosition = distance * Vector3.up;
        }
    }

    private void ApplyEffectOnHit(RaycastHit2D hit)
    {
        Cell cell = hit.collider.gameObject.GetComponent<Cell>();
        if (cell != null)
        {
            if (beamType == Projectile.ProjectileType.friendlyHeal || beamType == Projectile.ProjectileType.enemyHeal)
            {
                cell.ApplyHealRay(rayPower, Time.deltaTime);
            }

            if (beamType == Projectile.ProjectileType.friendlyDamage || beamType == Projectile.ProjectileType.enemyDamage)
            {
                cell.ApplyDeathRay(rayPower, Time.deltaTime);
            }
        }

        RoboCell roboCell = hit.collider.gameObject.GetComponent<RoboCell>();
        if(roboCell)
        {
            Destroy(hit.collider.gameObject);
        }
    }

    private void BeamAnimation()
    {
        Vector2 tmpOffset = mat.GetTextureOffset("_MainTex") + Vector2.right * Time.deltaTime * speed;
        mat.SetTextureOffset("_MainTex", tmpOffset);
    }
}
