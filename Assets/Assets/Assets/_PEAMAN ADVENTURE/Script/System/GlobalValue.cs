using UnityEngine;
using System.Collections;

public enum BulletFeature { Normal, Explosion, Shocking }

public class GlobalValue : MonoBehaviour
{
    public static bool isFirstOpenMainMenu = true;
    public static int levelPlaying = -1;

    public static string WorldReached = "WorldReached";
    public static string Coins = "Coins";
    public static string Character = "Character";
    public static string ChoosenCharacterID = "choosenCharacterID";
    public static string ChoosenCharacterInstanceID = "ChoosenCharacterInstanceID";
    public static GameObject CharacterPrefab;
    public static int normalBulletLimited = 6;

    public static bool isSound = true;
    public static bool isMusic = true;

    public static int getDartLimited()
    {
       return normalBulletLimited;
    }
    
        public static bool isOpenRateButton
    {
        get { return PlayerPrefs.GetInt("isOpenRateButton", 0) == 1 ? true : false; }
        set { PlayerPrefs.SetInt("isOpenRateButton", value ? 1 : 0); }
    }

    public static bool RemoveAds
    {
        get { return PlayerPrefs.GetInt("RemoveAds", 0) == 1 ? true : false; }
        set { PlayerPrefs.SetInt("RemoveAds", value ? 1 : 0); }
    }

    public static bool isSetDefaultValue
    {
        get { return PlayerPrefs.GetInt("isSetDefaultValue", 0) == 1 ? true : false; }
        set { PlayerPrefs.SetInt("isSetDefaultValue", value ? 1 : 0); }
    }

    public static int SaveLives
    {
        get { return PlayerPrefs.GetInt("SaveLives", 6); }
        set
        {
            int i = Mathf.Max(value, 0);
            PlayerPrefs.SetInt("SaveLives", i);
        }
    }

    public static int SavedCoins
    {
        get { return PlayerPrefs.GetInt(GlobalValue.Coins, DefaultValue.Instance != null ? DefaultValue.Instance.defaultCoin : 99999); }
        set { PlayerPrefs.SetInt(GlobalValue.Coins, value); }
    }

    public static int Bullets
    {
        get {
            int bullets = PlayerPrefs.GetInt("Bullets", 3);
            bullets = Mathf.Clamp(bullets, 0, getDartLimited());
            return bullets; }
        set { PlayerPrefs.SetInt("Bullets", value); }
    }

    public static int storeGod
    {
        get { return PlayerPrefs.GetInt("storeGod", 0); }
        set { PlayerPrefs.SetInt("storeGod", value); }
    }

    public static void SetScrollLevelAte(int scrollID)
    {
        //Debug.LogError("EAT: " + scrollID);
        PlayerPrefs.SetInt("AteScroll" + levelPlaying + scrollID, 1);
    }

    public static bool IsScrollLevelAte(int scrollID)
    {
        //Debug.LogError(scrollID + ":" + (PlayerPrefs.GetInt("AteScroll" + levelPlaying + scrollID, 0) == 1));
        return PlayerPrefs.GetInt("AteScroll" + levelPlaying + scrollID, 0) == 1 ? true : false;
    }

    public static bool IsScrollLevelAte(int level, int scrollID)
    {
        return PlayerPrefs.GetInt("AteScroll" + level + scrollID, 0) == 1 ? true : false;
    }


    public static int Scroll
    {
        get { return PlayerPrefs.GetInt("Scroll", 0); }
        set { PlayerPrefs.SetInt("Scroll", value); }
    }

    public static int LevelHighest
    {
        get { return PlayerPrefs.GetInt("LevelHighest", 1); }
        set { PlayerPrefs.SetInt("LevelHighest", value); }
    }
}
