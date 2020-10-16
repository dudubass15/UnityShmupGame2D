using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : Base
{
    public override void Start()
    {
        base.Start();

        fric = 1.0f;
        velX = -Util.Rand(1f, 10f);
        transform.localScale = new Vector3(velX, velX, 1);

        Color color = GetComponent<SpriteRenderer>().color;
        color.a = (velX / 10) * -1;
        GetComponent<SpriteRenderer>().color = color;

    }

    public override void Update()
    {
        base.Update();
        Rect l = Util.Limits();
        // float flat = ((velX * 10) / 2) * -1;
        // if (flat > origScaleX) transform.localScale = new Vector3(flat, origScaleX, 1);
        if (transform.position.x < l.xMin)
        {
            Util.CreateStar();
            Destroy(gameObject);
        }

    }
}
