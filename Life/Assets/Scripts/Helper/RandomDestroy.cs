using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomDestroy : MonoBehaviour
{
    [Range(0, 100)]
    public float probabilityOfSurvive;

    public bool randomPos = false;
    Vector2 minMaxPosX = new Vector2(-32f, 32f);
    Vector2 minMaxPosy = new Vector2(-32f, 32f);

    public bool randomRot = false;



    // Use this for initialization
    void Start()
    {
        if (Random.Range(0, 100) > probabilityOfSurvive)
        {
            Destroy(this.gameObject);
        }
        else
        {
            if (randomPos)
            {
                transform.position = new Vector3(Random.Range(minMaxPosX.x, minMaxPosX.y), Random.Range(minMaxPosy.x, minMaxPosy.y), 0);
            }

            if(randomRot)
            {
                transform.rotation = Quaternion.Euler( new Vector3(0,0, Random.Range(0f, 360f)));
            }

            Destroy(this);
        }
    }

}
