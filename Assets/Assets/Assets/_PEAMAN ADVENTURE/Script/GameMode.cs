using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMode : MonoBehaviour {
    public static GameMode Instance;
    public bool showInfor = true;
    public int setFPS = 60;
    public bool setScreenResolution = true;
    public Vector2 screenResolution = new Vector2(1280, 720);
    float deltaTime = 0.0f;

    [Header("Open link")]
    public string androidLink;
    public string iosLink;

    [Header("TEST LEVEL")]
    public bool showPassLevelButton = false;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start(){
		Application.targetFrameRate = setFPS;
        if (setScreenResolution)
            Screen.SetResolution((int)screenResolution.x, (int)screenResolution.y, true);
    }

    void OnGUI()
    {
        if (showInfor)
        {
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
            int w = Screen.width, h = Screen.height;

            GUIStyle style = new GUIStyle();

            Rect rect = new Rect(0, 0, w, h * 2 / 100);
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = h * 2 / 100;
            style.normal.textColor = new Color(1f, 1f, 1f, 1.0f);
            float msec = deltaTime * 1000.0f;
            float fps = 1.0f / deltaTime;
            string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);


            GUI.Label(rect, text, style);

            Rect rect2 = new Rect(250, 0, w, h * 2 / 100);
            GUI.Label(rect2, Screen.currentResolution.width + "x" + Screen.currentResolution.height, style);
        }
    }

    public void OpenStoreLink()
    {
        //SoundManager.PlaySfx(soundManager.soundClick);
#if UNITY_ANDROID
        Application.OpenURL(androidLink);
#elif UNITY_IOS
        Application.OpenURL(iosLink);
#endif

        if (!GlobalValue.isOpenRateButton)
        {
            SoundManager.PlaySfx(SoundManager.Instance.soundPurchased);
            GlobalValue.isOpenRateButton = true;
        }
        //finishGame.SetActive(false);
    }

    public void OpenGooglePlayLink()
    {
        //SoundManager.PlaySfx(soundManager.soundClick);
#if !UNITY_WEBGL
        Application.OpenURL(androidLink);
#else
        openPage(androidLink);
#endif
    }

#if UNITY_WEBGL
    [DllImport("__Internal")]
    private static extern void openPage(string url);
#endif
}
