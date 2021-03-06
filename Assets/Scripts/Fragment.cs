using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fragment : Base
{

    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();

        float flat = velX * 10;
        if (flat > origScaleX) transform.localScale = new Vector3(flat, origScaleX, 1);

        Color color = GetComponent<SpriteRenderer>().color;
        if (velX < 1f) color.a = velX; else color.a = 1f;
        if (color.a <= 0.01f) Destroy(gameObject);

        else GetComponent<SpriteRenderer>().color = color;

    }

}
