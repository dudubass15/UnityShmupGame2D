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
        velX = 20.0f;
        fric = 1.00f;
        life = 1;

    }

    public override void Update()
    {
        base.Update();
        DestroyOnOut();
    }

}
