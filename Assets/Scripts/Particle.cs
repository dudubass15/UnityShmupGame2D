using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : Base
{

    public float colorSpeed = 0.07f;
    public float alphaSpeed = 0.02f;
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
            color.r += colorSpeed * speed;
            color.g += colorSpeed * speed;
            color.b += colorSpeed * speed;
            color.a -= alphaSpeed * speed;
            GetComponent<SpriteRenderer>().color = color;
        }
    }

    public void SetColor(int color = 1)
    {
        Color c = new Color(0, 0, 0);
        if (color == 1) c.r = 255;
        if (color == 2) c.g = 255;
        GetComponent<SpriteRenderer>().color = c;
    }

}
