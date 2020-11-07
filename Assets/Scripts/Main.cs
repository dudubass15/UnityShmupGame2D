using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Video;

public class Main : MonoBehaviour
{
    private GameObject[] asteroids;
    private GameObject[] enemies;
    public int numEnemies = 0;
    private int lifeCount = 0;
    private float started = 0;
    private GameObject player;
    private Base bsPlayer;
    int nextAsteroidTime = 0;
    float startDelay = 2f;
    float asteroided = 0;
    int starCount = 200;

    string[] alerts = { "CONTROLS: W S A D I O P", "ASTEROIDS KEEP SECRETS... DESTROY ALL THEM!" };
    int alertsTotal = 2;
    int alertCount = 0;
    GameObject alert;
    public static TimeSpan ts;
    public static bool paused = false;

    void Start()
    {

        for (int i = 0; i < starCount; i++) Util.CreateStar(true);

        Util.music = Util.PlaySound("top-gear-1");

        float f = Util.Rand(0, 10);
        int n = f > 5 ? 19 : 20;

        started = Time.time;

        Util.CreatePlanet();

        Util.Larry(n);

    }

    public CameraShake cameraShake;
    void Update()
    {
        CreatePlayer();

        if (!Util.music)

            Util.music = Util.PlaySound("top-gear-2", true);

        if (Time.time - started > startDelay) CreateEnemies();
        if (Time.time - started > startDelay) CreateAsteroid();

        ts = TimeSpan.FromSeconds(Time.time - started);
        string combo = string.Format("x{0:D2}", bsPlayer.combo);
        string time = string.Format("{0:00}:{1:00}", ts.Minutes, ts.Seconds);

        string infos = "Ships: {1:D2} Level: {0:D2} Missiles: {2:D2} Score: {3:D12} {5} Time: {4}";
        Util.Message(string.Format(infos, Util.level, lifeCount, bsPlayer.missileCount, bsPlayer.score, time, combo));

        if (!alert && alertCount < alertsTotal)
        {
            alert = Util.CreateAlert(alerts[alertCount]);
            alertCount++;
        }

        Rect limits = Util.Limits();
        GameObject message = GameObject.Find("Message");
        message.transform.position = new Vector3(0, limits.yMax - 0.5f, 0);

        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();

    }

    void CreatePlayer()
    {

        player = GameObject.Find("Player");

        if (!player)
        {
            lifeCount += 1;
            Util.speed = 1.0f;
            Util.music.pitch = 1.0f;
            Rect rect = Util.Limits();
            Vector2 pos = new Vector2(rect.x - 1f, 0);
            player = Util.CreatePlayer(pos, Quaternion.identity);
        }

        bsPlayer = player.GetComponent<Base>();

    }

    void CreateAsteroid()
    {
        if ((Time.time - asteroided) > nextAsteroidTime)
        {
            Rect l = Util.Limits();
            nextAsteroidTime = Util.RandInt(1, 5);
            float x = l.xMax + Util.RandInt(1, 5);
            float y = UnityEngine.Random.Range(l.yMin, l.yMax);

            Vector2 p = new Vector2(x, y);
            GameObject ast = Util.CreateAsteroid(p);
            ast.GetComponent<Base>().velX = -Util.Rand(0.5f, 1.5f);

            asteroided = Time.time;
        }
    }

    void CreateEnemies()
    {

        if (numEnemies != Util.level)
        {

            numEnemies = Util.level;
            enemies = new GameObject[numEnemies];

            for (int i = 0; i < numEnemies; i++)
            {
                Rect rect = Util.Limits();
                float r1 = Util.Rand(0.1f, 2.0f);
                float r2 = Util.Rand(rect.yMin, rect.yMax);
                Vector2 pos = new Vector2(rect.xMax + r1, r2);
                enemies[i] = Util.CreateEnemy1(pos, Quaternion.identity);
                string s = string.Format("Spaceship{0}", UnityEngine.Random.Range(1, 4));
                enemies[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(s) as Sprite;

            }

        }
        else
        {

            int n = 0;
            for (int i = 0; i < enemies.Length; i++)
            {
                if (!enemies[i]) n++;
            }

            if (n == numEnemies)
            {

                Util.level++;

                if (bsPlayer.life < 100) bsPlayer.life += ((bsPlayer.maxLife - bsPlayer.life) * 10 / 100);
                if (bsPlayer.lifeLost == 0) Util.Victory(); else Util.Larry(21);
                Util.CreateLevelUp(string.Format("LEVEL {0:D3}", Util.level));
                Util.PlaySound("s_levelup");
                bsPlayer.lifeLost = 0;

            }

        }

    }

}
