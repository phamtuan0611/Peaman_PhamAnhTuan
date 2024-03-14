using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
public class HealthBar : MonoBehaviour {
	public GameObject healthIcon;
    List<Image> hearthIcons = new List<Image>();


    private void Start()
    {
        hearthIcons.Add(healthIcon.GetComponent<Image>());
        for (int i = 0; i < GameManager.Instance.Player.maxHealth; i++)
        {
            if (i > 0)
            {
                hearthIcons.Add( Instantiate(healthIcon, transform).GetComponent<Image>());
            }
        }

        Player.healthChangeEvent += Player_healthChangeEvent;
    }

    private void Player_healthChangeEvent(int currentHealth)
    {
        for(int i = 0; i < hearthIcons.Count; i++)
        {
            hearthIcons[i].color = (i < currentHealth) ? Color.white : Color.black;
        }
    }

}
