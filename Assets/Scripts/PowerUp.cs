using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : Base
{

    public override void Start()
    {
        base.Start();
        velX = -1.0f;
        fric = 1f;
    }

    public override void Update()
    {
        base.Update();
    }

    public override void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            Destroy(gameObject);
            return;
        }
    }

}
