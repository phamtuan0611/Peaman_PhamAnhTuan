using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ContinueAdType { rewardedVideo, None}
public class DefaultValue : MonoBehaviour {
    public static DefaultValue Instance;

    public ContinueAdType continueAdType;
    [Header("DEFAULT VALUE")]
    public int defaultLives = 3;
    public int defaultCoin = 100;

    public int normalBulletLimited = 6;
    public bool defaultBulletMax = false;
    public int defaultBullet = 0;

    public int defaultGodItem = 0;
    [Space]
    [Header("SHOP")]
    public int livePrice = 10;
    public int bulletPrice = 10;

    [Header("WATCH VIDEO REWARD")]
    public int rewardedLives = 3;

    [Header("KEYBOARD CONTROL")]
    public KeyCode keyMoveLeft;
    public KeyCode keyMoveRight;
    public KeyCode keyMoveDown;
    public KeyCode keyJump;

    public KeyCode keyNormalBullet;
    public KeyCode keyPause;

    // Use this for initialization
    private void Awake()
    {
        Instance = this;
    }
    void Start () {

        GlobalValue.normalBulletLimited = normalBulletLimited;
        DontDestroyOnLoad(gameObject);
	}

    private void OnDrawGizmos()
    {
        if (defaultBulletMax)
            defaultBullet = int.MaxValue;
    }
}
