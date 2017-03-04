using UnityEngine;
using System.Collections;

public class DestroyAfterTime : MonoBehaviour {

    public float timeToLive = 20;

	// Use this for initialization
	void Start () {
        StartCoroutine(DestroyMe());
	}
	
    IEnumerator DestroyMe()
    {
        yield return new WaitForSeconds(timeToLive);
        Destroy(this.gameObject);
    }
}
