using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Player : Base
{

    bool danger = false;
    GameObject healthBar;
    GameObject hackingBar;
    GameObject timeStopBar;
    public float lastShot = 0f;
    public bool lowLife = false;
    public GameObject bulletPoint;
    public GameObject exaustPoint;
    public float shotInterval = 0.10f;

    bool slowmoAux = false;
    bool slowmoOverload = false;

    float missilesLast;
    int missiles = 0;

    // SHIP HACKING
    bool hacked = false;
    bool hacking = false;
    float hackingAux = 0f;
    float hackingTime = 10f;
    float hackingSpeed = 0.1f;

    public override void Start()
    {
        base.Start();

        fric = 0.92f;
        accel = 45.0f;
        SetupControl();
        missileCount = 10;
        missilesLast = Time.time;
        Util.PlaySound("s_engineon");

        healthBar = Util.CreateLifeBar(transform.position);
        hackingBar = Util.CreateLifeBar(transform.position, new Color(80f, 0f, 255f));
        timeStopBar = Util.CreateLifeBar(transform.position, new Color(0f, 80f, 255f));

    }

    double lastX = 0;
    double lastY = 0;
    string hackingName;
    AudioSource hackingSound;
    public override void Update()
    {

        base.Update();

        if (Time.time - missilesLast > 0.5f)
        {
            if (missiles > 5) Util.Larry(3); // hot fury
            missiles = 0;
        }

        // HEALTH BAR
        Vector2 pos = transform.position;
        healthBar.GetComponent<LifeBar>().SetSize(life / maxLife);
        healthBar.transform.position = new Vector3(pos.x, pos.y + 0.5f, 0);

        // FREEZE TIME BAR
        timeStopBar.GetComponent<LifeBar>().SetSize(Util.slowmoPower / Util.slowmoMaxPower);
        timeStopBar.transform.position = new Vector3(pos.x, pos.y + 0.7f, 0);
        bool slowmoFull = (Util.slowmoPower == Util.slowmoMaxPower);
        if (slowmoFull) timeStopBar.SetActive(false);
        else timeStopBar.SetActive(true);

        // HACKING BAR
        hackingBar.GetComponent<LifeBar>().SetSize(hackingAux / hackingTime);
        hackingBar.transform.position = new Vector3(pos.x, pos.y + 0.9f, 0);
        if (hacking) hackingBar.SetActive(true);
        else hackingBar.SetActive(false);

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

            // Time Freeze

            if (down[6])
            {
                if (!slowmoOverload)
                {
                    if (Util.slowmoPower > 0)
                    {
                        Util.slowmoPower -= Util.slowmoDischarge;
                        if (slowmoAux) SlowMoIn();
                    }
                    else
                    {
                        slowmoOverload = true;
                        Util.slowmoPower = 0;
                        SlowMoOut();
                    }
                    HoloTrail();
                }
                else SlowMoReload();
            }

            else
            {
                if (!slowmoAux) SlowMoOut();
                else { SlowMoReload(); }
                slowmoOverload = true;
            }

            // TODO: Ship Hacking

            GameObject[] ships = GameObject.FindGameObjectsWithTag("Enemy");

            hacking = false;
            l.enabled = false;
            GameObject hShip = null;

            if (ships.Length > 0)
            {
                foreach (GameObject s in ships)
                {
                    float dist = Util.Dist(gameObject, s);
                    if (dist < 10)
                    {
                        hShip = s;
                        hacking = true;
                        l.enabled = true;
                        lp[0] = transform.position;
                        lp[1] = s.transform.position;
                        l.SetPositions(lp.ToArray());
                        if (hackingName != s.name) { hackingName = s.name; hackingAux = 0f; }
                    }
                }
            }

            if (!hacked)
            {
                if (hacking)
                {
                    if (!hacked)
                    {
                        if (hackingAux < hackingTime)
                        {
                            hackingAux += hackingSpeed * Util.speed;
                            if (!hackingSound) hackingSound = Util.PlaySound("glitch", volume: 0.5f);
                        }
                        else
                        {
                            if (!hShip.GetComponent<Base>().isHacked)
                            {

                                var l = Util.Limits();
                                var s = hShip.transform.localScale;
                                hShip.GetComponent<Base>().HoloTrail(0.02f);
                                float rndY = UnityEngine.Random.Range(0, l.yMax);

                                hShip.transform.position = new Vector3(l.xMin * 1.2f, rndY, 0f);
                                hShip.transform.localScale = new Vector3(s.x * -1f, s.y, s.z);
                                hShip.GetComponent<Base>().isHacked = true;
                                Util.PlaySound("hacked", volume: 8f);
                                Util.CreateAlert("Enemy Hacked!");
                                hackingAux = hackingTime;
                                hShip.tag = "Player";
                                hacked = true;

                            }
                        }
                    }
                }
            }

            if (!hacking)
            {
                if (hackingSound)
                {
                    hackingSound.Stop();
                    hackingSound = null;
                }
                hackingAux = 0f;
                hacked = false;
            }

            transform.rotation = Util.AngleToQuarternion(rot * speed);

            // destroy hacked enemies
            if (down[8])
            {
                GameObject[] hackeds = GameObject.FindGameObjectsWithTag("Player");
                foreach (var h in hackeds)
                {
                    if (h.GetComponent<Base>().isHacked)
                        h.GetComponent<Base>().life = 0f;
                }
            }

            // UDP ONLINE

            string recv = UDPClient.GetLastRecv();

            if (recv != "")
            {
                if (Util.EnemyLength() > 0)
                {

                    string[] ev = recv.Split(':');
                    int _UID = int.Parse(ev[0]);

                    GameObject go = Util.GetEnemyByUID(_UID);
                    if (!go) go = Util.SetEnemyUID(_UID);

                    switch (ev[1])
                    {

                        case "move":

                            float _px = float.Parse(ev[2]);
                            float _py = float.Parse(ev[3]);
                            float _pr = float.Parse(ev[4]);

                            go.GetComponent<Base>().life = float.Parse(ev[5]);
                            go.transform.rotation = Quaternion.Euler(0, 0, _pr);
                            go.transform.position = new Vector3(_px * -1, _py, 0);

                            break;

                        case "down":

                            if (int.Parse(ev[2]) == 4) go.GetComponent<Enemy1>().Shot();
                            if (int.Parse(ev[2]) == 5) go.GetComponent<Enemy1>().Missile(true);

                            break;

                    }

                }
            }

            double[] p = RoundedPosition();

            if (p[0] != lastX || p[1] != lastY)
            {
                UDPClient.sendUDP(string.Format("{0}:move:{1}:{2}:{3}:{4}", UID, p[0], p[1], p[2], life));
                lastX = p[0];
                lastY = p[1];
            }

            // END ONLINE

        }
        else
        {
            Util.CreateExplosion(transform.position);
            Util.LarryWow();
            Destroy(gameObject);
        }

    }

    double[] RoundedPosition()
    {
        double px = Math.Round(transform.position.x, 2);
        double py = Math.Round(transform.position.y, 2);
        double pr = Math.Round(transform.rotation.z, 2);
        return new double[] { px, py, pr };
    }

    void SlowMoIn()
    {
        Util.PlaySound("s_slowmo_in");
        Util.music.pitch = 0.5f;
        Util.speed = 0.1f;
        slowmoAux = false;
    }

    void SlowMoOut()
    {
        Util.PlaySound("s_slowmo_out");
        Util.music.pitch = 1.0f;
        slowmoAux = true;
    }

    void SlowMoReload()
    {
        if (slowmoOverload && Util.slowmoPower == Util.slowmoMaxPower) slowmoOverload = false;
        if (Util.slowmoPower < Util.slowmoMaxPower) Util.slowmoPower += Util.slowmoRecharge;
        if (Util.slowmoPower > Util.slowmoMaxPower) Util.slowmoPower = Util.slowmoMaxPower;
        if (Util.speed < 1.0f) Util.speed += Util.speed / 10f;
        else if (Util.speed > 1.0f) Util.speed = 1.0f;
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

        UDPClient.sendUDP(string.Format("{0}:down:{1}", UID, 4));

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
            missilesLast = Time.time;
            missiles += 1;

            UDPClient.sendUDP(string.Format("{0}:down:{1}", UID, 5));

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

        if (other.gameObject.tag == "Missile" && UnityEngine.Random.Range(0, 9) > 7) Util.Larry(15); // should avoid mines

        if (other.gameObject.tag == "PowerUp")
        {
            Util.PlaySound("s_powerup");
            Util.Larry(9); // powers up

            if (missileCount < 99)

                missileCount++;

            shotLevel++;

        }

        else base.OnCollisionEnter2D(other);

    }

    void OnDestroy()
    {
        Destroy(healthBar);
        Destroy(hackingBar);
        Destroy(timeStopBar);
        if (hackingSound)
        {
            hackingSound.Stop();
            hackingSound = null;
        }
    }

}
