using UnityEngine;
using System.Collections;

public class Fade : MonoBehaviour {


    private float speed = 2f;

    void Awake()
    {
        GetComponent<Renderer>().enabled = true;
    }

	// Use this for initialization
	void Start () {
        StartCoroutine(FadeOut());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(0.5f);
        float f = 1.5f;
        Material m = GetComponent<Renderer>().material;

        Camera cam = Camera.main;
        float camDist = cam.orthographicSize;

        while (f>0)
        {
            f -= Time.deltaTime*speed;

            f = Mathf.Clamp(f, 0, 999);

            m.SetColor("_EmissionColor", new Color(0.33f,0.75f,1) * f*0.75f);
            m.color = Color.white * f;

            cam.orthographicSize = camDist * (1 - f * 1.5f) + (camDist + 1) * (f * 1.5f);

            yield return new WaitForSeconds(Time.deltaTime);
        }

       // gameObject.SetActive(false);

    }

    public void FadeInNow()
    {
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
      //  yield return new WaitForSeconds(0.5f);
        float f = 0f;
        Material m = GetComponent<Renderer>().material;

        Camera cam = Camera.main;
        float camDist = cam.orthographicSize;

        while (f < 10)
        {
            f += Time.deltaTime * speed*2;

            f = Mathf.Clamp(f, 0, 999);

            m.SetColor("_EmissionColor", new Color(0.33f, 0.75f, 1) *((f * 0.75f)));
            m.color = Color.white * (f);
            cam.orthographicSize = camDist * (1 - f*1.5f) + (camDist + 1) * (f * 1.5f);
            yield return new WaitForSeconds(Time.deltaTime);
        }

  //      gameObject.SetActive(false);

    }
}
