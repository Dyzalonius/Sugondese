using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Window : MonoBehaviour
{
    [SerializeField]
    private WindowSize windowSize;

    private Vector2Int screenSize = new Vector2Int();

    private void Awake()
    {
        screenSize.y = (int)windowSize;
        screenSize.x = (int)windowSize / 9 * 16;

        if (Screen.width != screenSize.x || Screen.height != screenSize.y)
        {
            Screen.SetResolution(screenSize.x, screenSize.y, false);
            PlayerPrefs.SetInt("Screenmanager Resolution Width", screenSize.x);
            PlayerPrefs.SetInt("Screenmanager Resolution Height", screenSize.y);
        }
    }

    private enum WindowSize
    {
        p360 = 360,
        p480 = 480,
        p720 = 720,
        p1080 = 1080
    }
}
