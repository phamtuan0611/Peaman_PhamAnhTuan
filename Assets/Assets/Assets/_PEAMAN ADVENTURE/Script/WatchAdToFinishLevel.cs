using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WatchAdToFinishLevel : MonoBehaviour
{
    public GameObject buttonVideo;

    // Update is called once per frame
    void Update()
    {
        buttonVideo.SetActive(AdsManager.Instance && AdsManager.Instance.isRewardedAdReady());
    }

    public void WatchAd()
    {
        AdsManager.AdResult += AdsManager_AdResult;

        AdsManager.Instance.ShowRewardedAds();
    }

    private void AdsManager_AdResult(bool isSuccess, int rewarded)
    {
        AdsManager.AdResult -= AdsManager_AdResult;
        if (isSuccess)
        {
            MenuManager.Instance.NextLevel();
        }
    }
}
