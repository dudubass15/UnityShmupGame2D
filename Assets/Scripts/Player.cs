using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Player : Base
{

    bool danger = false;
    public float lastShot = 0f;
    public bool lowLife = false;
    public float shotInterval = 0.10f;

    public override void Start()
    {
        base.Start();
        fric = 0.92f;
        accel = 45.0f;
        SetupControl();
        lifeBar = Resources.Load("Life") as GameObject;
        lifeBar = Instantiate(lifeBar);
        Util.PlaySound("s_engineon");

    }

    public override void Update()
    {

        base.Update();

        Vector2 pos = transform.position;
        Vector2 tPos = new Vector2(pos.x + 0.3f, pos.y + 0.6f);
        lifeBar.transform.position = tPos;
        lifeBar.GetComponent<TextMesh>().text = string.Format("{0}", life);

        GameControl();

        if (life > 0)
        {

            Bounce();

            rot = velY / 20;

            if (down[4])
            {
                if (Time.time - lastShot > shotInterval)
                {
                    lastShot = Time.time;
                    Shot();
                }
            }

            if (down[5])
            {
                if (Time.time - lastShot > shotInterval)
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
                Util.PlaySound("s_danger");
                danger = true;
            }

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
            float a = rot + Util.Rand(-0.05f, 0.05f);
            if (i == 0) rt = Util.AngleToQuarternion(a);
            else rt = Util.AngleToQuarternion(rot + (r * s));

            GameObject go = Util.CreateShot(pos, rt);
            go.GetComponent<Base>().owner = gameObject.name;
            Util.IgnoreCollision(gameObject, go);

        }

    }

    void Missile()
    {
        if (GetComponent<Base>().missileCount > 0)
        {

            GameObject go = Util.CreateMissile(ShotPosition(), transform.rotation);
            go.GetComponent<Base>().owner = gameObject.name;
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
        Vector2 pos = transform.position;
        // pos.x = pos.x + 0.6f;
        return pos;
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

}
