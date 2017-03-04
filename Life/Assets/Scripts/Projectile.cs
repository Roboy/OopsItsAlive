using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

    public enum ProjectileType { friendlyHeal, friendlyDamage, enemyHeal, enemyDamage }
    public ProjectileType projectileType;
    public Vector2 velocity;

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
        //Making sure we stay 2D...
        Rigidbody2D rigidbody = this.GetComponent<Rigidbody2D>();
		rigidbody.transform.position = new Vector3(rigidbody.transform.position.x, rigidbody.transform.position.y, 0);
        //And setting the velocity
        rigidbody.velocity = velocity;
	}
}
