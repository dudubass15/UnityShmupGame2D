using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shot : Base
{

    public override void Start()
    {
        base.Start();

        force = Util.RandInt(10, 25);
        explosive = false;
        velX = 35.0f;
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
        if (tag == other.gameObject.tag) return;
        base.OnCollisionEnter2D(other);
        Util.CreateFragment(transform.position);
        CameraShake.Shake(0.1f, 0.05f);
    }

}
