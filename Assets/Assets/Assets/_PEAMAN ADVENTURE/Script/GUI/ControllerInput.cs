using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ControllerInput : MonoBehaviour, IListener {
    public static ControllerInput Instance;
    public GameObject rangeAttack;

    public GameObject PLATFORM, RUNNER;

	Player Player;

    [Header("Button")]
    public GameObject btnJump;
    public GameObject btnRange;

    private void OnEnable()
    {
        if (GameManager.Instance != null)
            StopMove();
    }

    public void TurnJump(bool isOn)
    {
        btnJump.SetActive(isOn);
    }

    public void TurnRange(bool isOn)
    {
        btnRange.SetActive(isOn);
    }

    private void Awake()
    {
        Instance = this;
    }

    void Start () {
        
        Player = FindObjectOfType<Player> ();
		if(Player==null)
			Debug.LogError("There are no Player character on scene");

        PLATFORM.SetActive(LevelMapType.Instance.controllerType == CONTROLLER.PLATFORM);
        RUNNER.SetActive(LevelMapType.Instance.controllerType == CONTROLLER.RUNNER);
    }

	void Update(){

        if (Input.GetKeyDown(DefaultValue.Instance == null ? DefaultValueKeyboard.Instance.keyPause : DefaultValue.Instance.keyPause))
            MenuManager.Instance.Pause();

        if (isMovingRight)
            MoveRight();
        else if (isMovingLeft)
            MoveLeft();

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.R))
            MenuManager.Instance.RestartGame();
    }

    bool isMovingLeft, isMovingRight;
	
	public void MoveLeft(){
        if (GameManager.Instance.State == GameManager.GameState.Playing)
        {
            Player.MoveLeft();
            isMovingLeft = true;
        }
	}

	public void MoveRight(){
        if (GameManager.Instance.State == GameManager.GameState.Playing)
        {
            Player.MoveRight();
            isMovingRight = true;
        }
	}

	public void FallDown(){
		if (GameManager.Instance.State == GameManager.GameState.Playing)
			Player.FallDown ();
	}


	public void StopMove(){
        if (GameManager.Instance.State == GameManager.GameState.Playing)
        {
            Player.StopMove();
            isMovingLeft = false;
            isMovingRight = false;
        }
	}

	public void Jump (){
		if (GameManager.Instance.State == GameManager.GameState.Playing)
			Player.Jump ();
	}

	public void JumpOff(){
		if (GameManager.Instance.State == GameManager.GameState.Playing)
			Player.JumpOff ();
	}

	public void RangeAttack(bool power){
        if (GameManager.Instance.State == GameManager.GameState.Playing)
        {
            Player.RangeAttack(power);
        }
	}

    public void NoHoldingNormalBullet()
    {
    
    }

    public void IPlay()
    {

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
