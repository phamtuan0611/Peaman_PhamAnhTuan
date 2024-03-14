using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Menu_Gamefinish : MonoBehaviour
{
    public GameObject Buttons;
    public GameObject Next;

    public GameObject ScrollHolder;
    public GameObject BossHolder;
    public GameObject scroll1;
    public GameObject scroll2;
    public GameObject scroll3;

    public AudioClip soundScroll1;
    public AudioClip soundScroll2;
    public AudioClip soundScroll3;

    void Awake()
    {
        Buttons.SetActive(false);
    }

    // Use this for initialization
    IEnumerator Start()
    {

        ScrollHolder.SetActive(LevelMapType.Instance.levelType == LEVELTYPE.Normal);
        BossHolder.SetActive(LevelMapType.Instance.levelType == LEVELTYPE.BossFight);

        Buttons.SetActive(true);
        
        scroll1.SetActive(false);
        scroll2.SetActive(false);
        scroll3.SetActive(false);

        yield return new WaitForSeconds(0.5f);

        if (LevelMapType.Instance.levelType == LEVELTYPE.Normal)
        {
            if (GlobalValue.IsScrollLevelAte(1))
            {
                scroll1.SetActive(true);
                SoundManager.PlaySfx(soundScroll1);
                yield return new WaitForSeconds(1);
            }

            if (GlobalValue.IsScrollLevelAte(2))
            {
                scroll2.SetActive(true);
                SoundManager.PlaySfx(soundScroll2);
                yield return new WaitForSeconds(1);
            }

            if (GlobalValue.IsScrollLevelAte(3))
            {
                scroll3.SetActive(true);
                SoundManager.PlaySfx(soundScroll3);
                yield return new WaitForSeconds(1f);
            }
        }else
            yield return new WaitForSeconds(2f);

        Buttons.SetActive(true);
    }
}
