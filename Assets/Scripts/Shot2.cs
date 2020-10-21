using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shot2 : Base
{

    public override void Start()
    {
        base.Start();

        force = Util.RandInt(5, 25);
        explosive = false;
        velX = -7.0f;
        fric = 1.00f;
        life = 1;

    }

    public override void Update()
    {
        base.Update();
        DestroyOnOut();
    }

    public override void OnCollisionEnter2D(Collision2D other)
    {
        Util.CreateFragment(transform.position);
        CameraShake.Shake(0.1f, 0.05f);
        base.OnCollisionEnter2D(other);
    }

}
