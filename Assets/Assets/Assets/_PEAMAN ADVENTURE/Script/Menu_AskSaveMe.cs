using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;

public class Menu_AskSaveMe : MonoBehaviour
{
    public Text timerTxt;
    public Image timerImage;

    float timer = 3;
    float timerCountDown = 0;

    public Button btnSaveByCoin;
    public Button btnWatchVideoAd;

    float timeStep = 0.02f;
    // Start is called before the first frame update
    void OnEnable()
    {
        if (GlobalValue.SaveLives > 0 || (LevelMapType.Instance && LevelMapType.Instance.playerNoLimitLife))
        {
            if (LevelMapType.Instance && !LevelMapType.Instance.playerNoLimitLife)
                GlobalValue.SaveLives--;
            Continue();
        }
        else
        {
            Time.timeScale = 0;
            btnSaveByCoin.interactable = GlobalValue.SavedCoins >= GameManager.Instance.continueCoinCost;
#if UNITY_ANDROID || UNITY_IOS
            btnWatchVideoAd.interactable = AdsManager.Instance && AdsManager.Instance.isRewardedAdReady();
#else
            btnWatchVideoAd.interactable = false;
            btnWatchVideoAd.gameObject.SetActive(false);
#endif

            if (!btnSaveByCoin.interactable && !btnWatchVideoAd.interactable)
                timerCountDown = 0;
            else
                timerCountDown = timer;

            StartCoroutine(CountingDownCo());
        }
    }

    IEnumerator CountingDownCo()
    {
        while (!GameManager.Instance.isWatchingAd)
        {
            if (timerCountDown > 0)
            {
                timerCountDown -= timeStep;
                timerTxt.text = (int)timerCountDown + "";
                timerImage.fillAmount = Mathf.Clamp01(timerCountDown / timer);
            }
            else
            {
                GameManager.Instance.GameOver(true);
                Time.timeScale = 1;
                MenuManager.Instance.OpenSaveMe(false);
                Destroy(this);      //destroy this script
            }

            yield return null;
        }
    }

    

    public void SaveByCoin()
    {
        SoundManager.Click();
        GlobalValue.SavedCoins -= GameManager.Instance.continueCoinCost;
        Continue();
    }

    public void WatchVideoAd()
    {
        SoundManager.Click();
        AdsManager.AdResult += AdsManager_AdResult;
        AdsManager.Instance.ShowRewardedAds();
        //reset to avoid play Unity video ad when finish game
        AdsManager.Instance.ResetCounter(); 
    }

    private void AdsManager_AdResult(bool isSuccess, int rewarded)
    {
        AdsManager.AdResult -= AdsManager_AdResult;
        if (isSuccess)
        {
            GlobalValue.SaveLives += 1;
            Continue();
        }
    }

    void Continue()
    {
        Time.timeScale = 1;
        GameManager.Instance.Continue();
    }
}
