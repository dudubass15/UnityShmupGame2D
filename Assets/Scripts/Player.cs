using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Player : Base
{

    bool danger = false;
    bool advised = false;
    bool warning = false;
    GameObject healthBar;
    public float lastShot = 0f;
    public bool lowLife = false;
    public GameObject bulletPoint;
    public GameObject exaustPoint;
    public float shotInterval = 0.10f;

    bool slowmoAux = false;
    public override void Start()
    {
        base.Start();
        fric = 0.92f;
        accel = 45.0f;
        SetupControl();
        Util.PlaySound("s_engineon");

        healthBar = Util.CreateLifeBar(transform.position);

    }

    public override void Update()
    {

        base.Update();

        Vector2 pos = transform.position;
        healthBar.GetComponent<LifeBar>().SetSize(life / maxLife);
        healthBar.transform.position = new Vector3(pos.x, pos.y + 0.5f, 0);

        Util.CreateParticle(gameObject, 2f, exaustPoint);

        GameControl();

        if (life > 0)
        {

            Bounce();

            rot = velY / 30;

            if (down[4])
            {
                if (Time.time - lastShot > shotInterval / speed)
                {
                    lastShot = Time.time;
                    Shot();
                }
            }

            if (down[5])
            {
                if (Time.time - lastShot > shotInterval / speed)
                {
                    lastShot = Time.time;
                    Missile();
                }
            }

            if (life < 30 && !lowLife)
            {
                Util.Larry(18);
                lowLife = true;
            }

            if (life < 10 && !danger)
            {
                Util.CreateCombo("DANGER: LOW LIFE!");
                danger = true;
            }

            GameObject[] missiles = GameObject.FindGameObjectsWithTag("Missile");

            if (Util.speed < 1.0f && Util.speed > 0.1f && !slowmoAux)
            {
                Util.PlaySound("s_slowmo_out");
                slowmoAux = true;
            }

            if (Util.speed < 1.0f) Util.speed += Util.speed / 10f;
            else Util.speed = 1.0f;
            warning = false;
            speed = 1.0f;

            foreach (GameObject g in missiles)
            {

                if (g.GetComponent<Base>().owner == name) continue;

                if (Util.Dist(gameObject, g) < 10)
                {
                    if (Util.AngleTo(g, gameObject) < 15)
                    {
                        Util.speed = 0.1f;
                        warning = true;
                        break;
                    }
                }
            }

            if (warning && !advised)
            {
                Util.PlaySound("s_slowmo_in");
                Util.PlaySound("s_danger");
                slowmoAux = false;
                advised = true;
            }

            if (!warning) advised = false;

            transform.rotation = Util.AngleToQuarternion(rot);

        }
        else
        {
            Util.CreateExplosion(transform.position);
            Util.LarryWow();
            Destroy(gameObject);
        }

    }

    void Shot()
    {

        int s = 1;
        float r = 0.05f;
        Util.PlaySound("laser");

        for (int i = 0; i < shotLevel; i++)
        {

            s = s * -1;
            Vector2 pos = ShotPosition();
            Quaternion rt = Quaternion.Euler(0, 0, 0);

            if (i > 0) pos.y += (r * s) * 1f;
            float a = rot + Util.Rand(-0.02f, 0.02f);
            if (i == 0) rt = Util.AngleToQuarternion(a);
            else rt = Util.AngleToQuarternion(rot + (r * s));

            GameObject go = Util.CreateShot(pos, rt);
            Util.IgnoreCollision(gameObject, go);
            go.GetComponent<Base>().owner = gameObject.name;
            GameObject[] shots = GameObject.FindGameObjectsWithTag("Shot");
            foreach (GameObject g in shots) Util.IgnoreCollision(gameObject, g);

        }

    }

    void Missile()
    {
        if (GetComponent<Base>().missileCount > 0)
        {

            GameObject go = Util.CreateMissile(ShotPosition(), transform.rotation);
            go.GetComponent<Base>().owner = gameObject.name;
            go.GetComponent<Missile>().ignoreTagged = true;
            go.GetComponent<Base>().targetTag = "Enemy";
            Util.IgnoreCollision(gameObject, go);
            GetComponent<Base>().missileCount--;

        }
        else
        {
            Util.PlaySound("s_failure");
            Util.CreateCombo("NO MISSILES");
        }

    }

    public override void Combo()
    {
        Util.CreateCombo(string.Format("COMBO x{0}", combo));
        Util.Combo(combo);
        kills = 0;
    }

    Vector2 ShotPosition(int r = 0)
    {
        return bulletPoint.transform.position;
    }

    public override void OnCollisionEnter2D(Collision2D other)
    {

        if (other.gameObject.tag == "PowerUp")
        {
            Util.PlaySound("s_powerup");
            shotLevel++;
        }

        else base.OnCollisionEnter2D(other);

    }

    void OnDestroy()
    {
        Destroy(healthBar);
    }

}
