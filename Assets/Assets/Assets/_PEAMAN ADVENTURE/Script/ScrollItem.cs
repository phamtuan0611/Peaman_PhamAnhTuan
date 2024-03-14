using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollItem : MonoBehaviour, IListener
{
    public int ID = 1;
    public AudioClip sound;
    public GameObject effect;
    public int giveLive = 1;
    public int giveCoin = 100;
    
    bool isCollected = false;
    
    //called by Player
    public void Collect()
    {
        if (isCollected)
            return;

        isCollected = true;

        if (DefaultValue.Instance != null)
        {
            GlobalValue.Scroll++;
            GlobalValue.SetScrollLevelAte(ID);
        }

        SoundManager.PlaySfx(sound);

        if (effect != null)
            SpawnSystemHelper.GetNextObject(effect, true).transform.position = transform.position;

        Menu_GUI.Instance.ScrollCollectAnim(ID);

        GlobalValue.SavedCoins += giveCoin;
        GlobalValue.SaveLives += giveLive;
        //gameObject.SetActive(false);
        Destroy(gameObject);
    }

    public void IPlay()
    {
        CheckCollected();
    }

    void CheckCollected()
    {
        //Debug.LogError("Check star");
        if (GameMode.Instance == null)
            return;

        bool isCollected = GlobalValue.IsScrollLevelAte(ID);

        if (isCollected)
        {
            Menu_GUI.Instance.ScrollCollectAnim(ID, true);
            Destroy(gameObject);
        }
    }

    public void ISuccess()
    {
    }

    public void IPause()
    {
    }

    public void IUnPause()
    {
    }

    public void IGameOver()
    {
    }

    public void IOnRespawn()
    {
   
    }

    public void IOnStopMovingOn()
    {
    } 

    public void IOnStopMovingOff()
    {
    }
}
