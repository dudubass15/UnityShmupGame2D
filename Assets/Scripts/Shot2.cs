using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shot2 : Base
{

    public override void Start()
    {
        base.Start();

        // GameObject player = GameObject.Find("Player") as GameObject;
        // float angle = Util.AngleTo(gameObject, player);
        // bool ok = Mathf.Abs(angle) < 90 ? true : false;
        // if (ok) Util.LookTo(gameObject, player);

        force = Util.RandInt(5, 25);
        explosive = false;
        velX = -20.0f;
        fric = 1.00f;
        life = 1;

    }

    public override void Update()
    {
        base.Update();
        DestroyOnOut();
    }

}
