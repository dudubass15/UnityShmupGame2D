using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fade : MonoBehaviour
{

    public float fadeRate = .01f;
    SpriteRenderer sp;

    void Start()
    {
        sp = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        var color = sp.color;
        color.a -= fadeRate;
        if (color.a > 0f)
            sp.color = color;
        else
            Destroy(gameObject);
    }

}
