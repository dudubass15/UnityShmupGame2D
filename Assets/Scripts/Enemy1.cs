using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy1 : Base
{

    int btnNum = 0;
    float moveTime = 0f;
    float lastTime = 0f;
    float lastShot = 0f;
    GameObject healthBar;
    float shotInterval = 0.3f;
    public GameObject bulletPoint;
    public GameObject exaustPoint;

    public override void Start()
    {
        base.Start();
        fric = 0.92f;
        accel = Util.Rand(10f, 20f);

        healthBar = Util.CreateLifeBar(transform.position);

        evades = new string[] { "Player", "Missile", "Asteroid", "Enemy" };

    }

    public override void Update()
    {
        base.Update();
        Bounce();

        transform.rotation = Quaternion.identity;

        Vector2 pos = transform.position;
        healthBar.GetComponent<LifeBar>().SetSize(life / maxLife);
        healthBar.transform.position = new Vector3(pos.x, pos.y + 0.5f, 0);

        Util.CreateParticle(gameObject, 2f, exaustPoint);

        if (life > 0)
        {

            rot = velY / 20;

            Rect limits = Util.Limits();

            foreach (string evade in evades) Evade(evade);

            if (Time.time - lastTime >= moveTime)
            {

                btnNum = UnityEngine.Random.Range(0, down.Length + (int)(300 / Util.level));
                for (int i = 0; i < down.Length; i++) down[i] = false;
                moveTime = UnityEngine.Random.Range(0.3f, 0.7f);

                if (pos.x < limits.xMin / 2f) btnNum = 3;
                if (pos.x > limits.xMax - 2f) btnNum = 2;
                if (pos.y > limits.yMax - 2f) btnNum = 1;
                if (pos.y < limits.yMin + 2f) btnNum = 0;

                if (down.Length > btnNum) down[btnNum] = true;

                lastTime = Time.time;

            }

            GameObject player = GameObject.Find("Player") as GameObject;
            down[4] = (player && Util.AngleTo(gameObject, player) < 1f);

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
                moveTime = 0;
                Missile();
            }

            transform.rotation = Util.AngleToQuarternion(rot * -1);

        }

    }

    void Shot()
    {

        GameObject go = Util.CreateShot2(ShotPosition(), transform.rotation);
        go.GetComponent<Base>().owner = gameObject.name;
        Util.PlaySound("laser", false, 0.05f);
        Util.IgnoreCollision(gameObject, go);

    }

    void Missile()
    {
        if (GetComponent<Base>().missileCount > 0)
        {

            GameObject go = Util.CreateMissile(ShotPosition(), transform.rotation);
            go.GetComponent<Base>().owner = gameObject.name;
            go.transform.localScale = new Vector3(-1, 1, 1);
            go.GetComponent<Base>().targetTag = "Player";
            go.GetComponent<Base>().keyDownInit = 2;
            Util.IgnoreCollision(gameObject, go);
            GetComponent<Base>().missileCount--;

        }

    }

    Vector2 ShotPosition()
    {
        return bulletPoint.transform.position;
    }

    public override void OnCollisionEnter2D(Collision2D other)
    {
        base.OnCollisionEnter2D(other);
    }

    void OnDestroy()
    {
        Destroy(healthBar);
    }

}
