using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : Base
{

    public override void Start()
    {
        base.Start();

        fric = 1.0f;
        life = Util.RandInt(30, 50);

    }

    public override void Update()
    {
        base.Update();
        Rect r = Util.Limits();
        Vector2 pos = transform.position;
        if (pos.x < r.xMin - 5f) Destroy(gameObject);
        if (pos.y < r.yMin - 5f) Destroy(gameObject);
        if (pos.y > r.yMax + 5f) Destroy(gameObject);
        if (pos.x > r.xMax + 5f && Time.time - born > 5f) Destroy(gameObject);

    }

    public override void OnCollisionEnter2D(Collision2D other)
    {

        if (Time.time - born > 1f)
        {

            life -= other.gameObject.GetComponent<Base>().force;

            Vector3 scale = transform.lossyScale;

            if (life <= 0)
            {

                if (scale.x > 0.5f)
                {

                    int n = Util.RandInt(4, 7);

                    if (Util.RandInt(0, 99) < 2 && Util.PlayerBase().shotLevel < 5) { Util.CreatePowerUp(transform.position); }

                    for (int i = 0; i < n; i++)
                    {

                        float s = scale.x / (n / 2);
                        GameObject go = Util.CreateAsteroid(transform.position, s);

                        go.GetComponent<Base>().velX = Util.Rand(-1f, 1f);
                        go.GetComponent<Base>().velY = Util.Rand(-1f, 1f);
                        go.GetComponent<Base>().life = (int)(maxLife * s);
                        go.GetComponent<Base>().force = (int)(force * s);

                    }

                    Util.PlaySound("asteroidbreak");

                }

                Destroy(gameObject);

            }

        }
    }

}
