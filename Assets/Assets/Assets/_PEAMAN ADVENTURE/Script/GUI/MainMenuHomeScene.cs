using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;

public class MainMenuHomeScene : MonoBehaviour {
	public static MainMenuHomeScene Instance;

	public GameObject StartMenu;
	public GameObject WorldsChoose;
	public GameObject LoadingScreen;
	public GameObject Shop;
    public GameObject Settings;
    public GameObject removeAdBut;

	SoundManager soundManager;

    [Header("Sound and Music")]
    public Image soundImage;
    public Image musicImage;
    public Sprite soundImageOn, soundImageOff, musicImageOn, musicImageOff;

    void Awake(){
		Instance = this;
		soundManager = FindObjectOfType<SoundManager> ();
    }
    
	void Start () {
        if (!GlobalValue.isSetDefaultValue)
        {
            GlobalValue.isSetDefaultValue = true;
            if (DefaultValue.Instance)
            {
                GlobalValue.Bullets = DefaultValue.Instance.defaultBulletMax ? int.MaxValue : DefaultValue.Instance.defaultBullet;
                GlobalValue.storeGod = DefaultValue.Instance.defaultGodItem;
               GlobalValue.SaveLives = DefaultValue.Instance.defaultLives;
            }
        }

        StartMenu.SetActive(false);
        WorldsChoose.SetActive (false);
		LoadingScreen.SetActive (false);
        Shop.SetActive (false);
        Settings.SetActive(false);
        SoundManager.PlayMusic(SoundManager.Instance.musicsMenu);

        SoundManager.PlayGameMusic();
        StartMenu.SetActive(true);

        soundManager = FindObjectOfType<SoundManager>();

        soundImage.sprite = GlobalValue.isSound ? soundImageOn : soundImageOff;
        musicImage.sprite = GlobalValue.isMusic ? musicImageOn : musicImageOff;
        if (!GlobalValue.isSound)
            SoundManager.SoundVolume = 0;
        if (!GlobalValue.isMusic)
            SoundManager.MusicVolume = 0;
    }

    #region Music and Sound
    public void TurnSound()
    {
        GlobalValue.isSound = !GlobalValue.isSound;
        soundImage.sprite = GlobalValue.isSound ? soundImageOn : soundImageOff;

        SoundManager.SoundVolume = GlobalValue.isSound ? 1 : 0;
    }

    public void TurnMusic()
    {
        GlobalValue.isMusic = !GlobalValue.isMusic;
        musicImage.sprite = GlobalValue.isMusic ? musicImageOn : musicImageOff;

        SoundManager.MusicVolume = GlobalValue.isMusic ? SoundManager.Instance.musicsGameVolume : 0;
    }
    #endregion

    void Update()
    {
        removeAdBut.gameObject.SetActive(!GlobalValue.RemoveAds);
    }

    public void OpenStoreLink()
    {
        SoundManager.Click();
        GameMode.Instance.OpenStoreLink();
    }

    public void OpenGooglePlayLink()
    {
        SoundManager.Click();
        GameMode.Instance.OpenGooglePlayLink();
    }

#if UNITY_WEBGL
    [DllImport("__Internal")]
    private static extern void openPage(string url);
#endif

    public void RemoveAds()
    {
#if UNITY_PURCHASING
        if (Purchaser.Instance)
        {
            Purchaser.Instance.BuyRemoveAds();
        }
#endif
    }

    public void OpenSettings(bool open)
    {
        Settings.SetActive(open);
        StartMenu.SetActive(!open);
    }

  //  public void OpenWorld(int world){
		//WorldsChoose.SetActive (false);
		//LevelsChoose.SetActive (true);

		//for (int i = 0; i < WorldLevel.Length; i++) {
		//	if (i == (world - 1)) {
		//		WorldLevel [i].SetActive (true);
		//	} else
		//		WorldLevel [i].SetActive (false);
		//}

		//SoundManager.PlaySfx (soundManager.soundClick);
  //      SoundManager.PlayMusic(soundManager.musicLevelChoose);
  //  }

	public void OpenWorldChoose(){
        StartMenu.SetActive(false);
        WorldsChoose.SetActive (true);

        SoundManager.Click();
        SoundManager.PlayMusic(soundManager.musicWorldChoose);
    }

	public void OpenStartMenu(){
        StartMenu.SetActive(true);
        WorldsChoose.SetActive (false);

        SoundManager.Click();
        SoundManager.PlayMusic(soundManager.musicsMenu);
    }

	public void OpenShop(bool open) {
        SoundManager.Click();
        Shop.SetActive (open);
        StartMenu.SetActive(!open);
	}

    public void LoadScene(string name)
    {
        WorldsChoose.SetActive(false);
        //SceneManager.LoadSceneAsync(name);
        LoadingScreen.SetActive(true);
        StartCoroutine(LoadAsynchronously(name));
    }

    [Header("LOADING PROGRESS")]
    public Slider slider;
    public Text progressText;
    IEnumerator LoadAsynchronously(string name)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(name);
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            if (slider != null)
                slider.value = progress;
            if (progressText != null)
                progressText.text = (int) progress * 100f + "%";
            //			Debug.LogError (progress);
            yield return null;
        }
    }

    public void Exit(){
		Application.Quit ();
	}
}
