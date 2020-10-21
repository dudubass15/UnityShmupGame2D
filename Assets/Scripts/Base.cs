using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Base : MonoBehaviour
{

    // out of screen
    public bool[] oos = { false, false, false, false };

    // movement
    public float accel = 20.0f;
    public float speed = 0.0f;
    public float fric = 0.89f;
    public float velX = 0.0f;
    public float velY = 0.0f;
    public float velR = 0.0f;
    public float rot = 0.0f;

    // controls
    public bool[] down;
    public string[] acts;
    public KeyCode[] keys;
    public int KeyNum = 6;
    public int pressed = 0;

    // color
    public float alpha = 1.0f;

    // gameplay
    public float born;
    public string owner;
    public int combo = 1;
    public int score = 0;
    public int kills = 0;
    public TextMesh label;
    public LineRenderer l;
    public List<Vector3> lp;
    public int force = 100;
    public float life = 1f;
    public float maxLife = 100f;
    public bool dead = false;
    public float lastKill = 0;
    public bool collide = true;
    public bool evading = false;
    public bool explosive = true;
    public float safeDistance = 2f;
    public float comboTime = 3f;
    public GameObject lifeBar;
    public int shotLevel = 1;
    public int lifeLost = 0;
    public GameObject target;
    public string[] evades;
    public bool targeted = false;
    public string targetTag;
    public int keyDownInit = 3;
    public int missileCount = 10;

    public float origScaleX;

    public virtual void Start()
    {
        CreateLine();
        life = maxLife;
        born = Time.time;
        down = new bool[KeyNum];
        keys = new KeyCode[KeyNum];
        origScaleX = transform.localScale.x;
        for (int i = 0; i < KeyNum; i++) down[i] = false;
        acts = new string[] { "UP", "DOWN", "LEFT", "RIGHT", "FIRE1", "FIRE2" };
    }

    public virtual void Update()
    {

        lp = new List<Vector3> { transform.position, transform.position };
        Move();

    }

    void CreateLine()
    {
        l = gameObject.AddComponent<LineRenderer>();
        lp = new List<Vector3>();
        lp.Add(transform.position);
        lp.Add(transform.position);
        l.useWorldSpace = true;
        l.startWidth = 0.03f;
        l.endWidth = 0.03f;
    }

    public virtual void Die(Collision2D other)
    {

        if (gameObject.tag != "Projectile")
        {

            Base mb = gameObject.GetComponent<Base>();
            Base ob = other.gameObject.GetComponent<Base>();

            if (gameObject.name == "Player")
            {
                Util.Larry(1);
                if (mb.combo > 1)
                {
                    int n = ob.combo < 10 ? 0 : 1;
                    Util.ComboBreaker(n);
                }
            }
            else if (gameObject.tag == "Enemy")
            {
                if (Util.RandInt(0, 9) > 5)
                {
                    Util.LarryWow();
                }
            }

            if (ob.owner != owner)
            {

                GameObject go = GameObject.Find(ob.owner);

                if (go)
                {
                    Base bs = go.GetComponent<Base>() as Base;

                    if (Time.time - bs.lastKill < comboTime)
                    { bs.combo++; bs.Combo(); }

                    bs.lastKill = Time.time;
                    bs.kills += 1;
                }

            }

        }

        if (explosive)
        {
            Util.CreateExplosion(transform.position);
            CameraShake.Shake(0.25f, 0.5f);
        }
        if (lifeBar) Destroy(lifeBar);
        if (label) Destroy(label);
        Destroy(gameObject);

    }

    public virtual void Combo() { }

    public virtual void Move()
    {

        velY *= fric;
        velX *= fric;

        oos = Util.OutOfScreen(gameObject);

        if (down[0]) velY += accel * Time.deltaTime;
        if (down[1]) velY -= accel * Time.deltaTime;
        if (down[2]) velX -= accel * Time.deltaTime;
        if (down[3]) velX += accel * Time.deltaTime;

        if (Time.time - lastKill > comboTime) combo = 1;

        transform.Translate(new Vector2(velX * Time.deltaTime, velY * Time.deltaTime));

    }
    public virtual void GameControl()
    {
        for (int i = 0; i < KeyNum; i++)
        {
            if (Input.GetKey(keys[i])) down[i] = true; else down[i] = false;
        }
    }

    public virtual void SetupControl()
    {
        keys[0] = KeyCode.W;
        keys[1] = KeyCode.S;
        keys[2] = KeyCode.A;
        keys[3] = KeyCode.D;
        keys[4] = KeyCode.I;
        keys[5] = KeyCode.O;
    }

    public virtual void CustomControl()
    {

        if (pressed < KeyNum)
        {

            Util.Message(string.Format("Press button to {0}...", acts[pressed]));

            foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
            {

                bool exists = false;

                if (Input.GetKeyDown(kcode))
                {

                    foreach (KeyCode k in keys)
                    {
                        if (kcode == k)
                        {
                            exists = true;
                            break;
                        }
                    }

                    if (!exists)
                    {
                        keys[pressed] = kcode;
                        pressed += 1;
                    }

                }
            }

        }

        else Util.Message("");

    }

    public virtual void Bounce(float force = 2f)
    {

        if (oos[0]) velX += force;
        if (oos[1]) velX -= force;
        if (oos[2]) velY += force;
        if (oos[3]) velY -= force;

    }

    public virtual void DestroyOnOut()
    {
        if (oos[0] || oos[1] || oos[2] || oos[3])
        {
            Destroy(gameObject);
        }
    }

    public virtual void OnCollisionEnter2D(Collision2D other)
    {

        string otag = other.gameObject.tag;
        string oname = other.gameObject.name;

        Base data = other.gameObject.GetComponent<Base>();

        if (tag != "Shot" && Time.time - born < 1f) return;

        if (tag == otag || otag == "PowerUp")
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), other.gameObject.GetComponent<Collider2D>());
            return;
        }

        if (other.gameObject.name != owner && gameObject.name != data.owner)
        {

            int f = data.force;
            Vector2 pos = transform.position;
            string popup = string.Format("{0}", f);
            GameObject own = GameObject.Find(owner);
            if (explosive) Util.CreateDamage(pos, popup);
            if (own) own.GetComponent<Base>().score += force;

            data.score += score * data.combo;
            lifeLost += data.force;
            Util.PlaySound("hit");
            life -= data.force;

        }

        if (life <= 0) Die(other);

    }

    public virtual bool Evade(string tag, bool evad = false)
    {

        GameObject closest = Util.FindClosestTagged(gameObject, tag);

        if (closest)
        {


            float dd = Vector2.Distance(transform.position, closest.transform.position);

            if (dd < safeDistance)
            {

                Vector2 dir = closest.transform.position - transform.position;
                for (int i = 0; i < down.Length; i++) down[i] = false;
                float a = Vector2.Angle(Vector2.right, dir);

                evad = true;

                lp[0] = transform.position;
                lp[1] = closest.transform.position;

                float dx = transform.position.x - closest.transform.position.x;
                float dy = transform.position.y - closest.transform.position.y;

                if (dx > 0) down[3] = true;
                if (dx < 0) down[2] = true;
                if (dy > 0) down[0] = true;
                if (dy < 0) down[1] = true;

            }

        }

        // l.SetPositions(lp.ToArray());

        return evad;

    }

}
