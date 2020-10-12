using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smoke : Base
{

    public override void Start()
    {
        base.Start();
        Destroy(gameObject, 0.8f);
    }

    public override void Update()
    {
        base.Update();
    }
}
