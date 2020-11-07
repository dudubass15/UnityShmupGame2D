using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : Base
{

    public float lifeTime = 4f;
    public GameObject targetGO;
    public GameObject exaustPoint;
    public bool ignoreTagged = false;
    public override void Start()
    {
        base.Start();

        life = 1;
        fric = 1f;
        velX = -0.5f;
        accel = 0.1f;
        down[keyDownInit] = true;
        StartCoroutine(PlaySound());

    }

    IEnumerator PlaySound()
    {
        yield return new WaitForSeconds(0.3f);
        Util.AddSound(gameObject, "s_missile1", volume: 0.1f);
    }

    public override void Update()
    {
        base.Update();
        DestroyOnOut();

        lifeTime += speed < 1f ? 0.01f : 0f;

        float scale = transform.localScale.x;

        if (Time.time - born > lifeTime) base.Die();

        Util.CreateParticle(gameObject, 1f, exaustPoint);

        if (velX * scale > maxSpeed) velX = maxSpeed * scale;
        if (velY * scale > maxSpeed) velY = maxSpeed * scale;

        if (target) Util.RotateTo(gameObject, target, velR * speed);

        else if (Mathf.Abs(velX) > 1f)
        {

            target = Util.FindClosestTagged(gameObject, targetTag, ignoreTagged);

            if (target)
            {
                Util.PlaySound("s_missilebeep");
                target.GetComponent<Base>().targeted = true;
                targetGO = Util.CreateTarget(target.transform.position);
                targetGO.GetComponent<Target>().targetGO = target;
            }

        }

        accel += 0.1f;

    }

    void OnDestroy()
    {
        if (targetGO) Destroy(targetGO);
        if (target) target.GetComponent<Base>().targeted = false;
    }

}
