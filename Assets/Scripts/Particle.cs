using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : Base
{

    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();
        Color color = GetComponent<SpriteRenderer>().color;
        if (color.a <= 0) Destroy(gameObject);
        else
        {
            color.r += 0.05f;
            color.g += 0.05f;
            color.b += 0.05f;
            color.a -= 0.01f;
            GetComponent<SpriteRenderer>().color = color;
        }
    }

}
