using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : Base
{

    public override void Start()
    {
        base.Start();

        fric = 1.0f;
        life = Util.RandInt(20, 40);

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

        if (Util.asteroidCount >= Util.maxAsteroids) Destroy(gameObject);

        else if (Time.time - born > 1f)
        {

            life -= other.gameObject.GetComponent<Base>().force;

            Vector3 scale = transform.localScale;

            if (life <= 0)
            {

                if (scale.x > 0.5f)
                {

                    int n = Random.Range(2, 5);

                    if (Util.RandInt(0, 99) < 5 && Util.PlayerBase().shotLevel < 5) { Util.CreatePowerUp(transform.position); }

                    for (int i = 0; i < n; i++)
                    {

                        float s = (scale.x / Random.Range(2, 5));
                        GameObject go = Util.CreateAsteroid(transform.position, s);

                        go.GetComponent<Base>().velX = Util.Rand(-1f, 1f);
                        go.GetComponent<Base>().velY = Util.Rand(-1f, 1f);
                        go.GetComponent<Base>().force = (int)(force * s);
                        go.GetComponent<Base>().life = 1;

                    }

                    Util.PlaySound("asteroidbreak");

                }

                Destroy(gameObject);

            }

        }
    }

    void OnDestroy()
    {
        Util.asteroidCount--;
    }

}
