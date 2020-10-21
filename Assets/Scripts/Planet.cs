using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : Base
{

    float minVel = 0.005f;
    float maxVel = 0.100f;
    public override void Start()
    {
        base.Start();

        fric = 1.0f;
        velX = -Util.Rand(minVel, maxVel);

        float scale = velX * 20f;

        Color c = GetComponent<SpriteRenderer>().color;
        if (scale > 0.5f) c.a = scale; else c.a = 0.5f;
        GetComponent<SpriteRenderer>().color = c;

        transform.localScale = new Vector3(scale, scale, 1);
    }

    public override void Update()
    {
        base.Update();
        Rect l = Util.Limits();
        if (transform.position.x < l.xMin)
        {
            if (!GetComponent<Renderer>().isVisible)
            {
                Util.CreatePlanet();
                Destroy(gameObject);
            }
        }
    }

}
