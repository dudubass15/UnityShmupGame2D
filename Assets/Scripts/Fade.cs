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
        ToBlack();
    }

    void ToBlack()
    {
        var wRate = 1.7f;
        var color = sp.color;
        color.r -= fadeRate * wRate;
        color.g -= fadeRate * wRate;
        color.b -= fadeRate * wRate;
        sp.color = color;
    }

}
