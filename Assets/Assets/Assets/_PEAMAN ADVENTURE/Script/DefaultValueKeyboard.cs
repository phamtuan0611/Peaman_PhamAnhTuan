using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultValueKeyboard : MonoBehaviour {
	public static DefaultValueKeyboard Instance;

    [Header("KEYBOARD CONTROL")]
    public KeyCode keyMoveLeft;
    public KeyCode keyMoveRight;
    public KeyCode keyMoveDown;
    public KeyCode keyJump;
    //public KeyCode keyMoveLeft2 = KeyCode.LeftArrow;
    //public KeyCode keyMoveRight2 = KeyCode.RightArrow;
    //public KeyCode keyMoveDown2 = KeyCode.DownArrow;
    //public KeyCode keyJump2 = KeyCode.UpArrow;

    public KeyCode keyMelee;
    public KeyCode keyNormalBullet;
    public KeyCode keyPowerBullet;
    public KeyCode keyGod;
    public KeyCode keyPause;
    public KeyCode keyDogge;

    void Awake(){

		if (DefaultValue.Instance) {
			Destroy (gameObject);
			return;
		}

		Instance = this;
	}
}
