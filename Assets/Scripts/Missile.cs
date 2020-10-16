using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : Base
{
    float smokeTime = 0f;
    public override void Start()
    {
        base.Start();

        life = 1;
        fric = 1f;
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

        Util.CreateParticle(gameObject);

        if (target) Util.RotateTo(gameObject, target);

        else if (Mathf.Abs(velX) > 1f)
        {

            target = Util.FindClosestTagged(gameObject, targetTag, true);

            GetComponent<Animator>().SetBool("Idle", false);

            if (target)
            {
                Util.PlaySound("s_missilebeep");
                target.GetComponent<Base>().targeted = true;
                Util.CreateTarget(target.transform.position);
            }

        }

        accel += 0.1f;



    }

    void OnDestroy()
    {
        if (target) target.GetComponent<Base>().targeted = false;
    }

}
