using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : Base
{
    float minVel = 0.1f;
    float maxVel = 1.0f;
    public override void Start()
    {
        base.Start();

        fric = 1.0f;
        velX = -Util.Rand(minVel, maxVel);
        transform.localScale = new Vector3(velX * 5f, velX * 5f, 1);

        Color color = GetComponent<SpriteRenderer>().color;
        color.a = (velX * -1);
        GetComponent<SpriteRenderer>().color = color;

    }

    public override void Update()
    {
        base.Update();
        Rect l = Util.Limits();
        // float flat = (((velX * -1) - minVel) * 100 / maxVel);
        // if (flat > origScaleX) transform.localScale = new Vector3(flat, origScaleX, 1);
        if (transform.position.x < l.xMin)
        {
            Util.CreateStar();
            Destroy(gameObject);
        }

    }
}
