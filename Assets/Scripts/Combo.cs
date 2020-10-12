using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combo : Base
{
    public override void Start()
    {
        base.Start();
        velY = 2f;
    }

    public override void Update()
    {
        base.Update();
        bool zero = Util.TextAlpha(gameObject, 0.008f);
        if (zero) Destroy(gameObject);
    }
}
