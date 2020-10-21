using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameHandler : MonoBehaviour
{

    AsyncOperation loadingOperation;
    TextMeshPro loader;

    void Start()
    {

        loader = GameObject.Find("Loading").GetComponent<TextMeshPro>();
        loadingOperation = SceneManager.LoadSceneAsync("Main");

        Cursor.visible = false;

    }

    void Update()
    {

        float progressValue = Mathf.Clamp01(loadingOperation.progress / 0.9f);
        string text = Mathf.Round(progressValue * 100) + "%";

        if (loadingOperation.isDone) loader.text = "Ready!";

        else loader.text = text;

    }
}
