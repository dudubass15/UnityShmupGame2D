using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Text.RegularExpressions;

public static class Util
{

    public static int level = 1;
    public static int score = 0;
    public static int goCounter = 0;
    public static string camName = "Camera";

    public static int RandInt(int min, int max)
    {
        System.Random rand = new System.Random();
        return rand.Next(min, max);
    }

    public static void Larry(int n = -1)
    {
        if (n < 0) n = (int)Util.Rand(0, 14);
        Util.PlaySound(string.Format("larry{0}", n));
    }

    public static void Combo(int n = -1)
    {
        if (n < 0) n = (int)Util.Rand(0, 26);
        Util.PlaySound(string.Format("combo{0}", n));
    }

    public static void Victory(int n = -1)
    {
        if (n < 0) n = (int)Util.Rand(0, 2);
        Util.PlaySound(string.Format("victory{0}", n));
    }

    public static void ComboBreaker(int n = -1)
    {
        if (n < 0) n = (int)Util.Rand(0, 1);
        Util.PlaySound(string.Format("combobreak{0}", n));
    }

    public static void LarryWow()
    {
        int[] nums = { 0, 1, 8 };
        System.Random rnd = new System.Random();
        Util.PlaySound(string.Format("larry{0}", nums[rnd.Next(3)]));
    }

    public static void AddSound(GameObject go, string name, float volume = 0.5f, bool loop = false)
    {
        AudioSource audio = go.AddComponent<AudioSource>();
        audio.clip = (AudioClip)Resources.Load(name);
        audio.volume = volume;
        audio.loop = loop;
        goCounter += 1;
        audio.Play();
    }

    public static void PlaySound(string name, bool loop = false, float volume = 1.0f, float pbSpeed = 1f)
    {

        if (Util.FindText(name, "combo")) { volume = 20f; }
        if (Util.FindText(name, "explosion")) { volume = 0.3f; }

        switch (name)
        {
            case "hit": volume = 0.1f; break;
            case "music": loop = true; break;
            case "laser": volume = 0.05f; break;
            case "s_missile": volume = 0.5f; break;
            case "s_engineon": volume = 0.3f; pbSpeed = 3f; break;
        }

        GameObject m = new GameObject(string.Format("{0:D8}_Sound", goCounter));
        AudioSource audio = m.AddComponent<AudioSource>();
        audio.clip = (AudioClip)Resources.Load(name);
        audio.pitch = pbSpeed;
        audio.volume = volume;
        audio.loop = loop;
        goCounter += 1;
        audio.Play();

        if (!loop)
        {
            MonoBehaviour.Destroy(m, audio.clip.length);
        }
    }

    public static VideoPlayer PlayVideo(string resName)
    {
        GameObject camera = GameObject.Find(camName);
        VideoPlayer vp = camera.AddComponent<VideoPlayer>();
        vp.name = string.Format("{0:D8}_Video", goCounter);
        vp.aspectRatio = VideoAspectRatio.FitVertically;
        vp.renderMode = VideoRenderMode.CameraFarPlane;
        vp.clip = (VideoClip)Resources.Load(resName);
        vp.playOnAwake = true;
        goCounter += 1;
        vp.Prepare();
        vp.Play();

        return vp;
    }

    public static Vector2 RandomPosition()
    {
        float y = UnityEngine.Random.Range(Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).y, Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height)).y);
        float x = UnityEngine.Random.Range(Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).x, Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0)).x);

        return new Vector2(x, y);
    }

    public static Rect ScreenInfo(GameObject go)
    {
        float camDistance = Vector3.Distance(go.transform.position, Camera.main.transform.position);

        Vector2 LD = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, camDistance));
        Vector2 RT = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, camDistance));

        return new Rect(LD.x, RT.x, LD.y, RT.y);

    }

    public static bool[] OutOfScreen(GameObject go)
    {

        float camDistance = Vector3.Distance(go.transform.position, Camera.main.transform.position);

        Vector2 LD = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, camDistance));
        Vector2 RT = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, camDistance));

        bool[] inf = { false, false, false, false, false, false };

        float[] pos = { LD.x, RT.x, LD.y, RT.y };

        Vector2 p = go.transform.position;

        if (p.x < pos[0]) inf[0] = true;
        if (p.x > pos[1]) inf[1] = true;

        if (p.y < pos[2]) inf[2] = true;
        if (p.y > pos[3]) inf[3] = true;

        return inf;

    }

    public static bool HasText(Collision2D co, string txt, string s = @"({0})$")
    {
        string name = co.gameObject.name;
        string p = string.Format(s, txt);
        Match r = Regex.Match(name, p);
        return r.Success;
    }

    public static Rect Limits()
    {

        Vector2 TR = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, Camera.main.pixelHeight));
        Vector2 BL = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, Camera.main.pixelWidth));

        return new Rect(BL.x, BL.y, TR.x - BL.x, TR.y - BL.y);

    }

    public static void Message(string text)
    {
        GameObject msg = GameObject.Find("Message") as GameObject;
        TextMeshPro paper = msg.GetComponent<TextMeshPro>();
        paper.text = text;
    }

    public static void Opacity(GameObject go, float alpha)
    {
        Color color = go.GetComponent<SpriteRenderer>().material.color;
        color.a = alpha;
        go.GetComponent<SpriteRenderer>().material.color = color;
    }

    public static bool SpriteAlpha(GameObject go, float alpha)
    {

        Color color = go.GetComponent<SpriteRenderer>().color;
        color.a -= alpha;
        if (color.a <= 0) return true;
        go.GetComponent<SpriteRenderer>().color = color;
        return false;

    }

    public static bool TextAlpha(GameObject go, float alpha)
    {

        Color color = go.GetComponent<TextMesh>().color;
        color.a -= alpha;
        if (color.a <= 0) return true;
        go.GetComponent<TextMesh>().color = color;
        return false;

    }

    public static Quaternion AngleToQuarternion(float rot)
    {
        return Quaternion.AngleAxis(rot * Mathf.Rad2Deg, Vector3.forward);
    }

    public static GameObject CreatePlayer(Vector2 pos, Quaternion rot = default(Quaternion))
    {
        GameObject obj = Resources.Load("Player") as GameObject;
        GameObject go = MonoBehaviour.Instantiate(obj, pos, rot);
        go.name = string.Format("Player", goCounter);
        goCounter += 1;
        return go;
    }

    public static GameObject CreateShot(Vector2 pos, Quaternion rot = default(Quaternion))
    {
        GameObject obj = Resources.Load("Shot") as GameObject;
        GameObject go = MonoBehaviour.Instantiate(obj, pos, rot);
        go.name = string.Format("{0:D8}_Shot", goCounter);
        goCounter += 1;
        return go;
    }

    public static GameObject CreatePowerUp(Vector2 pos, Quaternion rot = default(Quaternion))
    {
        GameObject obj = Resources.Load("PowerUp") as GameObject;
        GameObject go = MonoBehaviour.Instantiate(obj, pos, rot);
        go.name = string.Format("{0:D8}_PowerUp", goCounter);
        goCounter += 1;
        return go;
    }

    public static GameObject CreateMissile(Vector2 pos, Quaternion rot = default(Quaternion))
    {
        GameObject obj = Resources.Load("Missile") as GameObject;
        GameObject go = MonoBehaviour.Instantiate(obj, pos, rot);
        go.name = string.Format("{0:D8}_Missile", goCounter);
        goCounter += 1;
        return go;
    }

    public static GameObject CreateShot2(Vector2 pos, Quaternion rot = default(Quaternion))
    {
        GameObject obj = Resources.Load("Shot2") as GameObject;
        GameObject go = MonoBehaviour.Instantiate(obj, pos, rot);
        go.name = string.Format("{0:D8}_Shot2", goCounter);
        goCounter += 1;
        return go;
    }

    public static GameObject CreateFlare(Vector2 pos, Quaternion rot = default(Quaternion))
    {
        GameObject obj = Resources.Load("Flare") as GameObject;
        GameObject go = MonoBehaviour.Instantiate(obj, pos, rot);
        go.name = string.Format("{0:D8}_Flare", goCounter);
        goCounter += 1;
        return go;
    }

    public static GameObject CreateEnemy1(Vector2 pos, Quaternion rot = default(Quaternion))
    {
        GameObject obj = Resources.Load("Enemy1") as GameObject;
        GameObject go = MonoBehaviour.Instantiate(obj, pos, rot);
        go.name = string.Format("{0:D8}_Enemy1", goCounter);
        goCounter += 1;
        return go;
    }

    public static GameObject CreateExplosion(Vector2 pos, Quaternion rot = default(Quaternion))
    {
        GameObject obj = Resources.Load("Explode") as GameObject;
        GameObject go = MonoBehaviour.Instantiate(obj, pos, rot);
        go.name = string.Format("{0:D8}_Explosion", goCounter);
        goCounter += 1;
        return go;
    }

    public static GameObject CreateSmoke(Vector2 pos, Quaternion rot = default(Quaternion), float vx = 0)
    {
        GameObject obj = Resources.Load("Smoke") as GameObject;
        GameObject go = MonoBehaviour.Instantiate(obj, pos, rot);
        go.name = string.Format("{0:D8}_Smoke", goCounter);
        float scale = Util.Rand(0.3f, 2.0f);
        float speed = Util.Rand(10f, 30f);
        go.transform.localScale = new Vector3(scale, scale, 0f);
        go.GetComponent<Base>().velX = speed * (vx > 0 ? -1 : 1);
        go.GetComponent<Base>().velY = Util.Rand(-0.5f, 0.5f);
        go.GetComponent<Base>().fric = 0.9f;
        goCounter += 1;
        return go;
    }

    public static GameObject CreateTarget(Vector2 pos)
    {
        Quaternion rot = Quaternion.Euler(0, 0, 0);
        GameObject obj = Resources.Load("Target") as GameObject;
        GameObject go = MonoBehaviour.Instantiate(obj, pos, rot);
        go.name = string.Format("{0:D8}_Target", goCounter);
        goCounter += 1;
        return go;
    }

    public static GameObject CreateStar(bool first = false)
    {
        float px = 0f;
        Rect l = Util.Limits();
        float py = Util.Rand(l.yMin, l.yMax);
        if (first) px = Util.Rand(l.xMin, l.xMax);
        else px = l.xMax + Util.RandInt(0, 3);
        Vector2 pos = new Vector2(px, py);
        GameObject obj = Resources.Load("Star") as GameObject;
        GameObject go = MonoBehaviour.Instantiate(obj, pos, Quaternion.identity);
        go.name = string.Format("{0:D8}_Star", goCounter);
        goCounter += 1;
        return go;
    }

    public static GameObject CreateAsteroid(Vector2 pos, float scale = 1.0f, Quaternion rot = default(Quaternion))
    {
        GameObject obj = Resources.Load("Asteroid") as GameObject;
        GameObject go = MonoBehaviour.Instantiate(obj, pos, rot);
        if (scale < 1f) go.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("aste-piece") as Sprite;
        go.transform.localScale = new Vector3(scale, scale, 1);
        go.name = string.Format("{0:D8}_Asteroid", goCounter);
        goCounter += 1;
        return go;
    }

    public static GameObject CreateDamage(Vector2 pos, string text, Quaternion rot = default(Quaternion))
    {
        GameObject obj = Resources.Load("Damage") as GameObject;
        GameObject go = MonoBehaviour.Instantiate(obj, pos, rot);
        go.name = string.Format("{0:D8}_Damage", goCounter);
        go.GetComponent<TextMesh>().text = text;
        goCounter += 1;
        return go;
    }

    public static GameObject CreateCombo(string text, Quaternion rot = default(Quaternion))
    {
        GameObject obj = Resources.Load("Combo") as GameObject;
        Rect limits = Util.Limits();
        Vector2 pos = new Vector2(0, limits.yMin + 0.3f);
        GameObject go = MonoBehaviour.Instantiate(obj, pos, rot);
        go.name = string.Format("{0:D8}_Combo", goCounter);
        go.GetComponent<TextMesh>().text = text;
        goCounter += 1;
        return go;
    }

    public static GameObject CreateAlert(string text)
    {
        Rect limits = Util.Limits();
        Vector2 pos = new Vector2(0, limits.yMin);
        GameObject obj = Resources.Load("Alert") as GameObject;
        GameObject go = MonoBehaviour.Instantiate(obj, pos, Quaternion.identity);
        go.name = string.Format("{0:D8}_Alert", goCounter);
        go.GetComponent<TextMesh>().text = text;
        goCounter += 1;
        return go;
    }

    public static GameObject CreateLevelUp(string text, Quaternion rot = default(Quaternion))
    {
        GameObject obj = Resources.Load("LevelUp") as GameObject;
        Rect limits = Util.Limits();
        Vector2 pos = new Vector2(0, -0.7f);
        GameObject go = MonoBehaviour.Instantiate(obj, pos, rot);
        go.name = string.Format("{0:D8}_LevelUp", goCounter);
        go.GetComponent<TextMesh>().text = text;
        goCounter += 1;
        return go;
    }

    public static void SetText(GameObject go, string text)
    {
        go.GetComponent<TextMesh>().text = text;
    }

    public static GameObject CreateLife(Vector2 pos, Quaternion rot = default(Quaternion))
    {
        GameObject obj = Resources.Load("Life") as GameObject;
        GameObject go = MonoBehaviour.Instantiate(obj, pos, rot);
        go.name = string.Format("{0:D8}_Life", goCounter);
        goCounter += 1;
        return go;
    }

    public static TextMesh CreateText(int fontSize = 16)
    {
        string name = string.Format("{0:D8}_Text", goCounter);

        GameObject go = new GameObject(name);
        TextMesh text = go.AddComponent<TextMesh>();
        text.font = Resources.Load("PressStart2P", typeof(Font)) as Font;
        go.GetComponent<MeshRenderer>().material = text.font.material;
        text.transform.localScale = new Vector3(0.2f, 0.2f, 1);
        text.fontSize = fontSize;
        goCounter += 1;

        return text;
    }

    public static void TextUpdate(TextMesh textMesh, Vector2 pos, string text)
    {
        textMesh.transform.SetPositionAndRotation(pos, default(Quaternion));
        textMesh.text = string.Format("{0}", text);
    }

    public static float Rand(float min, float max)
    {
        return UnityEngine.Random.Range(min, max);
    }

    public static GameObject CreateLifeBar(Vector2 pos, Quaternion rot = default(Quaternion))
    {
        GameObject obj = Resources.Load("LifeBar") as GameObject;
        GameObject go = MonoBehaviour.Instantiate(obj, pos, rot);
        go.name = string.Format("{0:D8}_LifeBar", goCounter);
        go.GetComponent<Slider>().value = 1f;
        goCounter += 1;
        return go;
    }

    public static GameObject FindClosestTagged(GameObject me, string tag, bool ignoreTargeted = false)
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag(tag);
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = me.transform.position;
        foreach (GameObject go in gos)
        {
            if (go.name == me.name) continue;
            if (go.GetComponent<Base>().owner == me.name) continue;
            if (ignoreTargeted && go.GetComponent<Base>().targeted) continue;
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }
        return closest;
    }

    public static bool FindText(string text, string find)
    {
        text = string.Format(@"(?i){0}", text);
        return Regex.Match(text, find).Success;
    }

    public static void LookTo(GameObject a, GameObject b, float add = -90)
    {
        a.transform.Rotate(0, 0, AngleTo(a, b, add));
    }

    public static float AngleTo(GameObject a, GameObject b, float add = -90)
    {
        Transform target = b.GetComponent<Transform>();
        Vector3 relative = a.transform.InverseTransformPoint(target.position);
        return (Mathf.Atan2(relative.x, relative.y) * Mathf.Rad2Deg) + add;
    }

    public static Base PlayerBase()
    {
        return GameObject.Find("Player").GetComponent<Base>();
    }

    public static void RotateTo(GameObject a, GameObject b, float speed = 0.5f)
    {
        Vector3 myLocation = a.transform.position;
        Vector3 targetLocation = b.transform.position;
        targetLocation.z = myLocation.z;

        Vector3 vectorToTarget = targetLocation - myLocation;
        Vector3 rotatedVectorToTarget = Quaternion.Euler(0, 0, 90) * vectorToTarget;

        Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: rotatedVectorToTarget);

        a.transform.rotation = Quaternion.RotateTowards(a.transform.rotation, targetRotation, speed * a.transform.localScale.x);
    }

    public static void IgnoreCollision(GameObject a, GameObject b)
    {
        Physics2D.IgnoreCollision(a.GetComponent<Collider2D>(), b.GetComponent<Collider2D>());
    }

}
