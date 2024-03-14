using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BtnDartUI : MonoBehaviour
{
    public Text bulletTxt;
    public Image image;
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnBtnPress);
    }

    void OnBtnPress()
    {
        ControllerInput.Instance.RangeAttack(false);
    }

    private void Update()
    {
        bulletTxt.text = GlobalValue.Bullets + "";
        bulletTxt.color = GlobalValue.Bullets == GlobalValue.getDartLimited() ? Color.red : Color.white;
    }
}
