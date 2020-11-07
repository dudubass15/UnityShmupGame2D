using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Base : MonoBehaviour
{

    // out of screen
    public bool[] oos = { false, false, false, false };

    // movement
    public float maxSpeed = 50f;
    public float accel = 20.0f;
    public float speed = 1.0f;
    public float fric = 0.89f;
    public float velX = 0.0f;
    public float velY = 0.0f;
    public float velR = 0.0f;
    public float rot = 0.0f;

    // controls
    public bool[] down;
    public string[] acts;
    public KeyCode[] keys;
    public int keyNum = 8;
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
    public float comboTime = 1f;
    public GameObject lifeBar;
    public int shotLevel = 1;
    public int lifeLost = 0;
    public GameObject target;
    public string[] evades;
    public bool targeted = false;
    public string targetTag;
    public int keyDownInit = 3;
    public int missileCount = 1;

    public float origScaleX;

    public virtual void Start()
    {
        CreateLine();
        life = maxLife;
        born = Time.time;
        down = new bool[keyNum];
        keys = new KeyCode[keyNum];
        origScaleX = transform.localScale.x;
        for (int i = 0; i < keyNum; i++) down[i] = false;
        acts = new string[] { "UP", "DOWN", "LEFT", "RIGHT", "FIRE1", "FIRE2", "FIRE3", "FIRE4", "FIRE5" };
    }

    public virtual void Update()
    {

        if (Main.paused) return;

        lp = new List<Vector3> { transform.position, transform.position };
        if (name == "Player") speed = Util.speed == 0.1f ? 0.5f : Util.speed;
        else speed = Util.speed;
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

    public virtual void Die(Collision2D other = null)
    {

        if (gameObject.tag != "Projectile")
        {

            if (other != null)
            {
                Base ob = other.gameObject.GetComponent<Base>();
                Base mb = gameObject.GetComponent<Base>();

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
                    if (Util.RandInt(0, 99) > 95)
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

        if (down[0]) velY += accel * Time.deltaTime * speed;
        if (down[1]) velY -= accel * Time.deltaTime * speed;
        if (down[2]) velX -= accel * Time.deltaTime * speed;
        if (down[3]) velX += accel * Time.deltaTime * speed;

        if (Time.time - lastKill > comboTime) combo = 1;

        float mx = velX * Time.deltaTime * speed;
        float my = velY * Time.deltaTime * speed;

        transform.Translate(new Vector2(mx, my));

        oos = Util.OutOfScreen(gameObject);

        velX *= fric;
        velY *= fric;

    }
    public virtual void GameControl()
    {
        for (int i = 0; i < keyNum; i++)

            down[i] = Input.GetKey(keys[i]);

        XboxController();
    }

    void XboxController()
    {
        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");
        if (v > 0.9f) down[1] = true; else down[1] = down[1];
        if (h > 0.9f) down[3] = true; else down[3] = down[3];
        if (v < -0.9f) down[0] = true; else down[0] = down[0];
        if (h < -0.9f) down[2] = true; else down[2] = down[2];
        if (Input.GetKey(KeyCode.Joystick1Button2)) down[4] = true; else down[4] = down[4];
        if (Input.GetKey(KeyCode.Joystick1Button1)) down[5] = true; else down[5] = down[5];
        if (Input.GetKey(KeyCode.Joystick1Button4)) down[6] = true; else down[6] = down[6];
        if (Input.GetKey(KeyCode.Joystick1Button3)) down[7] = true; else down[7] = down[7];
        if (Input.GetKeyDown(KeyCode.Joystick1Button7)) { if (Main.paused) Util.speed = 0; else Util.speed = 1; Main.paused = !Main.paused; }
    }

    public virtual void SetupControl()
    {
        keys[0] = KeyCode.W;
        keys[1] = KeyCode.S;
        keys[2] = KeyCode.A;
        keys[3] = KeyCode.D;
        keys[4] = KeyCode.I;
        keys[5] = KeyCode.O;
        keys[6] = KeyCode.P;
        keys[7] = KeyCode.U;
    }

    public virtual void CustomControl()
    {

        if (pressed < keyNum)
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

                // lp[0] = transform.position;
                // lp[1] = closest.transform.position;

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
