using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flare : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float a = gameObject.GetComponent<SpriteRenderer>().material.color.a;
        if (a > 0f)
        {
            a -= 0.01f;
            Util.Opacity(gameObject, a);
        } else {
            Destroy(gameObject);
        }
    }
}
