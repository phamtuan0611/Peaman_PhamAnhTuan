using UnityEngine;
using System.Collections;
public class RangeAttack : MonoBehaviour {
    public Transform FirePoint;
    [Tooltip("fire projectile after this delay, useful to sync with the animation of firing action")]
	public float fireDelay;
	public float fireRate;
	public bool inverseDirection = false;

    [Header("BOUNCING BULLET")]
    public BouncingBullet bouncingBullet;

    float nextFire = 0;

    public AudioClip soundAttack;


    public bool Fire(bool power)
    {
        if (((DefaultValue.Instance && DefaultValue.Instance.defaultBulletMax) || (GlobalValue.Bullets > 0 || GameManager.Instance.hideGUI)) && Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            if (DefaultValue.Instance && DefaultValue.Instance.defaultBulletMax)
                ;
            else
                GlobalValue.Bullets--;
            StartCoroutine(DelayAttack(fireDelay));
            return true;
        }
        else
            return false;
    }

	IEnumerator DelayAttack(float time){
		yield return new WaitForSeconds (time);

		var direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;

		if (inverseDirection || GameManager.Instance.Player.wallSliding)
			direction *= -1;

        Vector2 firePoint = GameManager.Instance.Player.wallSliding ? (FirePoint.position - Vector3.right * (FirePoint.position.x - GameManager.Instance.Player.transform.position.x)) : FirePoint.position;

        
            var bullet = SpawnSystemHelper.GetNextObject(bouncingBullet.gameObject, true, firePoint);
            bullet.GetComponent<BouncingBullet>().Init(direction, gameObject);
        

        SoundManager.PlaySfx(soundAttack);
    }
}
