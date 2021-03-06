using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : Base
{
    public GameObject targetGO;
    public override void Start()
    {
        base.Start();
        Util.SetScale(gameObject, 10);
        Util.SetAlpha(gameObject, 0f);
    }

    public override void Update()
    {
        base.Update();
        Util.SpriteFadeIn(gameObject, 0.05f);
        Util.ScaleDown(gameObject, 0.1f, 0.5f);
        transform.Rotate(Vector3.forward * transform.localScale.x);
        if (targetGO) transform.position = targetGO.transform.position;
        else Destroy(gameObject);
    }
}
