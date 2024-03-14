using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Controller2D))]
public class Player : MonoBehaviour, ICanTakeDamage, IListener {
    public int ID = 1;
    public Sprite iconImage;
    public Transform centerPoint;

    public delegate void HealthChange(int currentHealth);
    public static event HealthChange healthChangeEvent;

    public bool GodMode;
	[Header("Moving")]
	public float moveSpeed = 3;
	float accelerationTimeAirborne = .2f;
	float accelerationTimeGrounded = .1f;

	[Header("Jump")]
	public float maxJumpHeight = 3;
	public float minJumpHeight = 1;
	public float timeToJumpApex = .4f;
	public int numberOfJumpMax = 1;
	int numberOfJumpLeft;
	public GameObject JumpEffect;
    public GameObject landingFX;

    [Header("Wall Slide")]
    [HideInInspector]  public bool wallSlideJumpUp = false;
    public Vector2 wallJumpClimb;
	public Vector2 wallLeap;
    public LayerMask wallLayer;
    [Tooltip("When look to other side, sliding with this speed")]
    public float wallSlideSpeedHold = 0.15f;
    [Tooltip("When look to other side, sliding with this speed")]
    public float wallSlideSpeedNoHold = 0.5f;
    [Tooltip("When look to other side, sliding with this speed")]
    public float wallSlideSpeedLookOtherSide = 0.3f;
    public float wallStickTime = .25f;
	float timeToWallUnstick;
    public Transform checkWallUp, checkWallDown;
    [HideInInspector] public bool wallSliding;
    int wallDirX;

    [Header("Health")]
    public int maxHealth = 3;
	public int Health{ get; private set;}
	public GameObject HurtEffect;
    public GameObject respawnFX;

    [Header("TAKE DAMAGE")]
    public float rateGetDmg = 0.5f;
    public Color blinkingColor = Color.green;
    [ReadOnly] public bool isBlinking = false;
    public float knockbackForce = 10f;

    [Header("Sound")]
    public AudioClip respawnSound;
	public AudioClip[] jumpSound;
	[Range(0,1)]
	public float jumpSoundVolume = 0.5f;
	public AudioClip landSound;
	[Range(0,1)]
	public float landSoundVolume = 0.5f;
	public AudioClip wallSlideSound;
	[Range(0,1)]
	public float wallSlideSoundVolume = 0.5f;
	public AudioClip[] hurtSound;
	[Range(0,1)]
	public float hurtSoundVolume = 0.5f;
	public AudioClip[] deadSound;
	[Range(0,1)]
	public float deadSoundVolume = 0.5f;
    public AudioClip[] rangeVocalSound;
    [Range(0, 1)]
    public float rangeVocalSoundVolume = 0.8f;
    bool isPlayedLandSound;

    [Header("Option")]
	public bool allowRangeAttack;
	public bool allowSlideWall;

	protected RangeAttack rangeAttack;

	private AudioSource soundFx;

	float gravity;
	float maxJumpVelocity;
	float minJumpVelocity;
	[HideInInspector]
	public Vector3 velocity;
	float velocityXSmoothing;

	[ReadOnly] public bool isFacingRight;
	public Vector2 input;
    bool isDead = false;

    [HideInInspector]
	public Controller2D controller;
	[HideInInspector] public Animator anim;

	public bool isPlaying { get; private set;}
	public bool isFinish { get; set;}
    public bool isGrounded { get { return controller.collisions.below; } }

    void Awake(){
        healthChangeEvent = null;
        controller = GetComponent<Controller2D> ();
		anim = GetComponent<Animator> ();
    }

	void Start() {

        CameraFollow.Instance.manualControl = true;
		gravity = -(2 * maxJumpHeight) / Mathf.Pow (timeToJumpApex, 2);
		maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
		minJumpVelocity = Mathf.Sqrt (2 * Mathf.Abs (gravity) * minJumpHeight);

		isFacingRight = transform.localScale.x > 0;
		Health = maxHealth;
        //healthChangeEvent(Health);
        numberOfJumpLeft = numberOfJumpMax;

		rangeAttack = GetComponent<RangeAttack> ();
		soundFx = gameObject.AddComponent<AudioSource> ();
		soundFx.loop = true;
		soundFx.playOnAwake = false;
		soundFx.clip = wallSlideSound;
		soundFx.volume = wallSlideSoundVolume;

        godAudioSource = gameObject.AddComponent<AudioSource>();
        godAudioSource.clip = godSoundKeep;
        godAudioSource.Play();
        godAudioSource.loop = true;
        godAudioSource.volume = 0;
    }

    bool allowCheckWall = true;
    bool firstContactWall = true;
    bool allowGrapNextWall = false;
    void CheckWall()
    {
        wallSliding = false;

        if (controller.collisions.ClosestHit.collider != null && (controller.collisions.ClosestHit.collider.gameObject.GetComponent<Bridge>() || controller.collisions.ClosestHit.collider.gameObject.GetComponent<SimpleGravityObject>()))
            return;

        if (!allowCheckWall)
            return;
        
        if (controller.collisions.left)
            wallDirX = -1;
        else if (controller.collisions.right)
            wallDirX = 1;
        else
            wallDirX = 0;
        
        if (wallDirX!=0 &&(timeToWallUnstick > 0 || (allowSlideWall && ((controller.collisions.left && !isFacingRight && (input.x == -1 || allowGrapNextWall /* || firstContactWall*/))
            || (controller.collisions.right && isFacingRight && (input.x == 1 || allowGrapNextWall/* || firstContactWall*/))) && (!controller.collisions.below && velocity.y < 0 && firstContactWall)/* && (input.x == wallDirX)*/)))
        {

           var hitUp = Physics2D.Raycast(checkWallUp.position, wallDirX == 1 ? Vector2.right : Vector2.left, 0.5f, wallLayer);
            var hitDown = Physics2D.Raycast(checkWallDown.position, wallDirX == 1 ? Vector2.right : Vector2.left, 0.5f, wallLayer);

            if (hitUp && hitUp.collider.GetComponent<Block>())
                return;
            if (hitDown && hitDown.collider.GetComponent<Block>())
                return;

            if (hitUp && hitDown)     //check up and down contact wall or not
            {
            
                wallSliding = true;
                
                firstContactWall = false;
                if (!soundFx.isPlaying)
                    soundFx.Play();     //play the sliding sound
                
                if (timeToWallUnstick > 0)
                {
                  
                    velocityXSmoothing = 0;
                    //velocity.x = 0;

                    if (input.x != wallDirX)
                    {
                        if (input.x == 0)
                        {
                            timeToWallUnstick -= Time.deltaTime;
                            if (timeToWallUnstick <= 0)
                            {
                              
                                wallSliding = false;
                                Invoke("AllowCheckWall", 0.2f);
                                Flip();
                            }

                            if (velocity.y < -wallSlideSpeedNoHold)
                            {
                              
                                velocity.y = -wallSlideSpeedNoHold;
                            }
                        }
                        else
                        {
                            //velocity.y = 0;
                           
                            timeToWallUnstick = wallStickTime;      //
                            //wallSlidingHoldPosition = true;
                            if (velocity.y < -wallSlideSpeedLookOtherSide)
                            {
                                velocity.y = -wallSlideSpeedLookOtherSide;
                            }
                        }
                    }
                    else
                    {
                       
                        timeToWallUnstick = wallStickTime;
                        if (velocity.y < -wallSlideSpeedHold)
                        {
                            velocity.y = -wallSlideSpeedHold;
                        }
                    }
                }
                else
                {
                    timeToWallUnstick = wallStickTime;
                }
            }
            else
            {
                wallSliding = false;
                allowCheckWall = true;
                firstContactWall = true;
                timeToWallUnstick = 0;
            }
        }
        else
        {
            if (soundFx.isPlaying)
                soundFx.Stop();
        }

        if ((!controller.collisions.left && controller.collisions.faceDir == -1) || (!controller.collisions.right && controller.collisions.faceDir == 1))
        {
            wallSliding = false;
            allowCheckWall = true;
            firstContactWall = true;
        }  
    }

    void AllowCheckWall()
    {
        allowCheckWall = true;
        firstContactWall = true;
    }

    void Update() {
        
        if (isFrozen)
            return;

        HandleInput();
		HandleAnimation ();

        float targetVelocityX = input.x * moveSpeed * mulSpeedc;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);

        velocity.y += gravity * Time.deltaTime;

        if (controller.collisions.below && !isPlayedLandSound) {
            isPlayedLandSound = true;
			SoundManager.PlaySfx (landSound, landSoundVolume);
            if (landingFX)
                SpawnSystemHelper.GetNextObject(landingFX, true).transform.position = transform.position;
		} else if (!controller.collisions.below && isPlayedLandSound)
			isPlayedLandSound = false;

        if (controller.collisions.above)
        {
            CheckBlock();
        }
    }

   void CheckBelow()
    {
        if (controller.collisions.ClosestHit.collider != null)
        {
            var standObj = (IStandOnEvent)controller.collisions.ClosestHit.collider.gameObject.GetComponent(typeof(IStandOnEvent));
            if (standObj != null)
                standObj.StandOnEvent(gameObject);
        }
    }

    void CheckBlock()
    {
        Block isBlock;
        BrokenTreasure isTreasureBlock;
        var bound = controller.boxcollider.bounds;

        //check middle
        var hit = Physics2D.Raycast(new Vector2((bound.min.x + bound.max.x) / 2f, bound.max.y), Vector2.up, 0.5f, 1 << LayerMask.NameToLayer("Platform"));

        if (hit)
        {
            isBlock = hit.collider.gameObject.GetComponent<Block>();
            if (isBlock)
            {
                isBlock.BoxHit();
                //return;
            }

            isTreasureBlock = hit.collider.gameObject.GetComponent<BrokenTreasure>();
            if (isTreasureBlock)
            {
                isTreasureBlock.BoxHit();
                //return;
            }
        }

        //check left
        hit = Physics2D.Raycast(new Vector2(bound.min.x, bound.max.y), Vector2.up, 0.5f, 1 << LayerMask.NameToLayer("Platform"));
        if (hit)
        {
            isBlock = hit.collider.gameObject.GetComponent<Block>();
            if (isBlock)
            {
                isBlock.BoxHit();
            }

            isTreasureBlock = hit.collider.gameObject.GetComponent<BrokenTreasure>();
            if (isTreasureBlock)
            {
                isTreasureBlock.BoxHit();
            }
        }

        hit = Physics2D.Raycast(new Vector2(bound.max.x, bound.max.y), Vector2.up, 0.5f, 1 << LayerMask.NameToLayer("Platform"));
        if (hit)
        {
            isBlock = hit.collider.gameObject.GetComponent<Block>();
            if (isBlock)
            {
                isBlock.BoxHit();
            }

            isTreasureBlock = hit.collider.gameObject.GetComponent<BrokenTreasure>();
            if (isTreasureBlock)
            {
                isTreasureBlock.BoxHit();
            }
        }
    }

	void LateUpdate(){
        if (isFrozen)
            return;
        if (!isDead)
            CheckWall();

        if (wallSliding)
            velocity.x = 0;

        if ((controller.raycastOrigins.bottomLeft.x < CameraFollow.Instance._min.x && velocity.x<0) || (controller.raycastOrigins.bottomRight.x > CameraFollow.Instance._max.x && velocity.x > 0))
            velocity.x = 0;

        if (controller.raycastOrigins.bottomLeft.y < CameraFollow.Instance._min.y)
            GameManager.Instance.GameOver();

        controller.Move (velocity * Time.deltaTime, input);
        
        if (controller.collisions.above || controller.collisions.below) {
			velocity.y = 0;
		}

        if (controller.collisions.below)
        {
            timeToWallUnstick = 0;
            numberOfJumpLeft = 0;
            firstContactWall = true;
            allowGrapNextWall = false;
            AllowCheckWall();
            CheckBelow();       //check the object below if it have Stand on event
            controller.collisionMask = controller.collisionMask & ~(1 << LayerMask.NameToLayer("Enemies")); ;
            
        }
        else {
            controller.collisionMask |= (1 << LayerMask.NameToLayer("Enemies"));
        }

        if (!isDead)
            CameraFollow.Instance.DoFollowPlayer();
    }

    public void PausePlayer(bool pause)
    {
        StopMove();
        isPlaying = !pause;
    }

    public bool isFrozen { get; set; }  //player will be frozen
    public void Frozen(bool is_enable)
    {
        input = Vector2.zero;
        velocity = Vector2.zero;
        isFrozen = is_enable;
        anim.enabled = !is_enable;
    }

    private void HandleInput(){
		if (Input.GetKey (DefaultValue.Instance == null ? DefaultValueKeyboard.Instance.keyMoveLeft : DefaultValue.Instance.keyMoveLeft) || Input.GetKey(KeyCode.LeftArrow))
			MoveLeft ();
		else if (Input.GetKey (DefaultValue.Instance == null ? DefaultValueKeyboard.Instance.keyMoveRight : DefaultValue.Instance.keyMoveRight) || Input.GetKey(KeyCode.RightArrow))
			MoveRight ();
		else if(Input.GetKeyUp (DefaultValue.Instance==null? DefaultValueKeyboard.Instance.keyMoveLeft: DefaultValue.Instance.keyMoveLeft) || Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp (/*KeyCode.D*/DefaultValue.Instance == null ? DefaultValueKeyboard.Instance.keyMoveRight : DefaultValue.Instance.keyMoveRight) || Input.GetKeyUp(KeyCode.RightArrow))
			StopMove ();

        if (Input.GetKeyDown(DefaultValue.Instance == null ? DefaultValueKeyboard.Instance.keyMoveDown : DefaultValue.Instance.keyMoveDown) || Input.GetKeyDown(KeyCode.DownArrow))
            FallDown();

        if (Input.GetKeyUp(DefaultValue.Instance == null ? DefaultValueKeyboard.Instance.keyMoveDown : DefaultValue.Instance.keyMoveDown) || Input.GetKeyUp(KeyCode.DownArrow))
            StopMove();


        if (Input.GetKeyDown (DefaultValue.Instance == null ? DefaultValueKeyboard.Instance.keyJump : DefaultValue.Instance.keyJump) || Input.GetKeyDown(KeyCode.UpArrow)) {
			Jump ();
		}

		if (Input.GetKeyUp (DefaultValue.Instance == null ? DefaultValueKeyboard.Instance.keyJump : DefaultValue.Instance.keyJump) || Input.GetKeyUp(KeyCode.UpArrow)) {
			JumpOff ();
		}

        if (Input.GetKeyDown(DefaultValue.Instance == null ? DefaultValueKeyboard.Instance.keyNormalBullet : DefaultValue.Instance.keyNormalBullet))
        {
            RangeAttack(false);
        }
    }

	private void Flip(){
        if (wallSliding)
            return;
        
        transform.localScale = new Vector3 (transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
		isFacingRight = transform.localScale.x > 0;
	}


	public void MoveLeft(){
		if (isPlaying) {
			input = new Vector2 (-1, 0);
			if (isFacingRight)
				Flip ();
		}
	}


	public void MoveRight(){
		if (isPlaying) {
			input = new Vector2 (1, 0);
			if (!isFacingRight)
				Flip ();
		}
	}


	public void StopMove(){
		input = Vector2.zero;
	}

	public void FallDown(){
		input = new Vector2 (0, -1);
	}


    public void Jump()
    {
        if (!isPlaying)
            return;

        if (wallSliding)
        {
            allowCheckWall = false;
            wallSliding = false;
            numberOfJumpLeft = 0;
            timeToWallUnstick = 0;

            if (wallDirX == input.x && wallSlideJumpUp)
            {
                velocity.x = -wallDirX * wallJumpClimb.x;
                velocity.y = wallJumpClimb.y;
                Invoke("AllowCheckWall", 0.35f);
            }
            //else if (input.x == 0)
            //{
            //    velocity.x = -wallDirX * wallJumpOff.x;
            //    velocity.y = wallJumpOff.y;
            //    Flip();
            //    Invoke("AllowCheckWall", 0.1f);
            //}
            else
            {
                velocity.x = -wallDirX * wallLeap.x;
                velocity.y = wallLeap.y;
                Flip();
                allowGrapNextWall = true;
                Invoke("AllowCheckWall", 0.05f);
            }
            SoundManager.PlaySfx(jumpSound, jumpSoundVolume);
        }
        else if (controller.collisions.below)
        {
            velocity.y = maxJumpVelocity;

            if (JumpEffect)
                SpawnSystemHelper.GetNextObject(JumpEffect, true).transform.position = transform.position;

            SoundManager.PlaySfx(jumpSound, jumpSoundVolume);
            numberOfJumpLeft = numberOfJumpMax;
        }
        else
        {
            numberOfJumpLeft--;
            if (numberOfJumpLeft > 0)
            {
                anim.SetTrigger("doubleJump");
                velocity.y = minJumpVelocity;

                if (JumpEffect)
                    SpawnSystemHelper.GetNextObject(JumpEffect, true).transform.position = transform.position;
                SoundManager.PlaySfx(jumpSound, jumpSoundVolume);
            }
        }
    }
    
	public void JumpOff(){
		if (velocity.y > minJumpVelocity) {
			velocity.y = minJumpVelocity;
		}
	}

    public void RangeAttack(bool power){

		if (!isPlaying)
			return;
		
		if (allowRangeAttack && rangeAttack!=null) {

			if (rangeAttack.Fire (power)) {
				anim.SetTrigger ("range_attack");
                SoundManager.PlaySfx(rangeVocalSound, rangeVocalSoundVolume);
            }
		}
	}


    public void SetForce(Vector2 force, bool springPush = false)
    {
        if (!springPush && isBlinking)
            return;

        if (!springPush && GodMode)
            return;

        if (springPush)
        {
            numberOfJumpLeft = numberOfJumpMax;
        }

        velocity = (Vector3)force;
    }

	public void AddForce(Vector2 force){
		velocity += (Vector3) force;
	}


	public void RespawnAt(Vector2 pos){
		transform.position = pos;
        if (respawnFX)
            Instantiate(respawnFX, pos, respawnFX.transform.rotation);
		isPlaying = true;
        isDead = false;
        Health = maxHealth;
        //if(healthChangeEvent!=null)
        healthChangeEvent(Health);
        SoundManager.PlaySfx(respawnSound, 0.8f);
        godAudioSource.volume = 0;
        GodMode = false;

        mulSpeedc = 1;

        ResetAnimation ();

        controller.HandlePhysic = true;

        StartCoroutine(BlinkingCo(1.5f));
	}

	void HandleAnimation(){
		//set animation state
		anim.SetFloat ("speed", Mathf.Abs(velocity.x));
		anim.SetFloat ("height_speed", velocity.y);
		anim.SetBool ("isGrounded", controller.collisions.below);
		anim.SetBool ("isWall", wallSliding);
    }

	void ResetAnimation(){
		anim.SetFloat ("speed", 0);
		anim.SetFloat ("height_speed", 0);
		anim.SetBool ("isGrounded", true);
		anim.SetBool ("isWall", false);
		anim.SetTrigger ("reset");
	}

	public void GameFinish(){
		StopMove ();
		isPlaying = false;
		anim.SetTrigger ("finish");
	}

    public void TakeDamage(int damage, Vector2 force, GameObject instigator, Vector3 hitPoint)
    {
        if (!isPlaying || isBlinking)
            return;

        if (GodMode)
        {
          
            if (instigator.gameObject.layer == LayerMask.NameToLayer("Enemies"))
            {
                if (Time.time > (lastGodDamage + godDamageRate))
                {
                    lastGodDamage = Time.time;
                    var _damage = (ICanTakeDamage)instigator.GetComponent(typeof(ICanTakeDamage));
                    if (_damage != null)
                        _damage.TakeDamage(godmodeDamage, Vector2.zero, gameObject, Vector2.zero);        //kill the enemy right away while in godmode
                }
            }

            return;
        }
		
		SoundManager.PlaySfx (hurtSound, hurtSoundVolume);
        if (HurtEffect)
            SpawnSystemHelper.GetNextObject(HurtEffect, true).transform.position = hitPoint == Vector3.zero ? instigator.transform.position : hitPoint;

        Health--;
        healthChangeEvent(Health);
        if (Health <= 0)
            GameManager.Instance.GameOver();
        else
        {
            anim.SetTrigger("hurt");
            StartCoroutine(BlinkingCo(rateGetDmg));
        }

        
        if (instigator != null)
        {
            int dirKnockBack = (instigator.transform.position.x > transform.position.x) ? -1 : 1;
            SetForce(new Vector2(knockbackForce * dirKnockBack, 0));
        }
    }

    IEnumerator BlinkingCo(float time)
    {
        isBlinking = true;
        int blink = (int)(time * 0.5f / 0.1f);
        for (int i = 0; i < blink; i++)
        {
            imageCharacterSprite.color = godBlinkColor;
            yield return new WaitForSeconds(0.1f);
            imageCharacterSprite.color = Color.white;
            yield return new WaitForSeconds(0.1f);
        }

        imageCharacterSprite.color = Color.white;
        isBlinking = false;
    }

	public void GiveHealth(int hearthToGive, GameObject instigator){
		Health = Mathf.Min (Health + hearthToGive, maxHealth);
        healthChangeEvent(Health);
    }
    
    public void Kill()
    {
        if (isPlaying)
        {
            isPlaying = false;
            wallSliding = false;
            isDead = true;
            StopAllCoroutines();
            StopMove();
            SoundManager.PlaySfx(deadSound, deadSoundVolume);
            soundFx.Stop(); //stop the sliding wall sound if it's playing
            anim.SetTrigger("dead");
            SetForce(new Vector2(0, 7f));
            Health = 0;
            healthChangeEvent(Health);
            imageCharacterSprite.color = Color.white;
            allowCheckWall = true;
            firstContactWall = true;
            godAudioSource.volume = 0;
            GodMode = false;
            mulSpeedc = 1;
        }
    }

    #region GOD MODE

    public enum GodType
    {
        Blinking, FX

    }

    public enum GodObstacles { Through, GetKill}
    [Header("GOD MODE")]

    public SpriteRenderer imageCharacterSprite;     //the Image of the character
    public Color godBlinkColor = new Color(0.2f, .2f, .2f, 1f);     //blink colour
    public GodObstacles godObstacles;
    
    public float godDamageRate = 0.5f;
    float lastGodDamage;
    public AudioClip godSoundKeep;
    AudioSource godAudioSource;

    [Header("GOD DEFAULT")]
    public GodType godEffectType;
    public  float godTimer = 7;     //active the God timer in the given time
  public int godmodeDamage = 50;
    
    public void InitGodmode()
    {
        if (GodMode)
            return;

        StartCoroutine(GodmodeCo());
    }

    IEnumerator GodmodeCo()
    {

        GodMode = true;
        godAudioSource.volume = 1;
        if (godEffectType == GodType.Blinking)
        {
            int blink = (int)(godTimer * 0.5f / 0.1f);
            for (int i = 0; i < blink; i++)
            {
                imageCharacterSprite.color = godBlinkColor;
                yield return new WaitForSeconds(0.1f);
                imageCharacterSprite.color = Color.white;
                yield return new WaitForSeconds(0.1f);
            }

            for (int i = 0; i < 6; i++)
            {
                imageCharacterSprite.color = godBlinkColor;
                yield return new WaitForSeconds(0.3f);
                imageCharacterSprite.color = Color.white;
                yield return new WaitForSeconds(0.3f);
            }

            imageCharacterSprite.color = Color.white;
        }
        else
        {
            yield return new WaitForSeconds(godTimer);
        }

        godAudioSource.volume = 0;
        GodMode = false;

    }

    #endregion
    [HideInInspector]
    public float mulSpeedc = 1;
    float XXspeed, XXtime;

    public void SpeedBoost(float Xspeed, float time, bool allowEffect)
    {
        XXspeed = Xspeed;
        XXtime = time;
        StartCoroutine(SpeedBoostCo(allowEffect));
    }


    IEnumerator SpeedBoostCo(bool allowEffect)
    {
       
        mulSpeedc = XXspeed;
        
        yield return new WaitForSeconds(XXtime);
        mulSpeedc = 1;
    }

    #region TELEPORT
    public void Teleport(Transform newPos, float timer)
    {
        StartCoroutine(TeleportCo(newPos, timer));
    }


    IEnumerator TeleportCo(Transform newPos, float timer)
    {
        StopMove();
        isPlaying = false;
        Color color = imageCharacterSprite.color;

        float transparentSpeed = 3;
        float alpha = 1;
        while (alpha > 0)
        {
            alpha -= (Time.deltaTime * transparentSpeed);
            
            color.a = Mathf.Clamp01(alpha);
            imageCharacterSprite.color = color;
            yield return null;
        }

        transform.position = newPos.position;
        yield return new WaitForSeconds(timer);

        isPlaying = true;
        yield return null;
        isPlaying = false;

        alpha = 0;
        while (alpha < 1)
        {
            alpha += (Time.deltaTime * transparentSpeed);
            color.a = Mathf.Clamp01(alpha);
            imageCharacterSprite.color = color;
            yield return null;
        }

        color.a = 1;
        imageCharacterSprite.color = Color.white;

        isPlaying = true;
    }
    #endregion
    
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            Gizmos.DrawRay(checkWallUp.position, Vector2.right * 0.5f);
            Gizmos.DrawRay(checkWallDown.position, Vector2.right * 0.5f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isPlaying)
            return;

        var isTriggerEvent = collision.GetComponent<TriggerEvent>();
        if (isTriggerEvent != null)
            isTriggerEvent.OnContactPlayer();

        var scrollType = collision.GetComponent<ScrollItem>();
        if (scrollType)
            scrollType.Collect();

        if (collision.CompareTag("Checkpoint"))
        {
            var hitGround = Physics2D.Raycast(collision.transform.position, Vector2.down, 100, GameManager.Instance.groundLayer);
            
            if (hitGround)
                GameManager.Instance.SaveCheckPoint(hitGround.point);
            else
                GameManager.Instance.SaveCheckPoint(collision.transform.position);

        }

        if (collision.CompareTag("GodItem"))
        {
            Destroy(collision.gameObject);
            InitGodmode();
        }

        if (!GodMode && collision.CompareTag("DeadZone"))
            GameManager.Instance.GameOver();

        //if (GodMode)
        //{
        //    var damage = (ICanTakeDamage)collision.GetComponent(typeof(ICanTakeDamage));
        //    if (damage!=null)
        //        damage.TakeDamage(godmodeDamage,Vector2.zero, gameObject, collision.transform.position);
        //}

        if (GodMode)
        {
                if (collision.gameObject.layer == LayerMask.NameToLayer("Enemies"))
                {
                    if (Time.time > (lastGodDamage + godDamageRate))
                    {
                        lastGodDamage = Time.time;
                        var _damage = (ICanTakeDamage)collision.GetComponent(typeof(ICanTakeDamage));
                        if (_damage != null)
                            _damage.TakeDamage(godmodeDamage, Vector2.zero, gameObject, Vector2.zero);        //kill the enemy right away while in godmode
                    }
                }
            }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!isPlaying)
            return;

        var itemType = collision.GetComponent<ItemType>();
        if (itemType)
            itemType.Collect();
    }

    public void IPlay()
    {
        isPlaying = true;
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
        Kill();
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
