using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("ADDP/Enemy AI/Enemy Range Attack")]
public class EnemyRangeAttack : MonoBehaviour {
	public bool allowAimPlayer = true;
    public Transform firePoint;
    public float bulletSpeed = 10;
	public int damage = 30;
	public Projectile bullet;
	public float shootingRate = 2;
	public int multiShoot = 1;
	public float multiShootRate = 0.2f;
    public AudioClip soundAttack;
	float lastShoot = 0;
    int multiShootCounter = 0;
    public bool AllowAction()
    {
        bool allowShoot = Time.time - lastShoot > shootingRate;
        if (allowShoot)
            lastShoot = Time.time;
        return allowShoot;
    }
    
	public void Shoot(bool isFacingRight){
		StartCoroutine (ShootCo (isFacingRight));
	}

	IEnumerator ShootCo(bool isFacingRight){
		
			float shootAngle = 0;
			if (allowAimPlayer)
				shootAngle = AimHelperEnemy.Aim (transform, GameManager.Instance.Player.transform, isFacingRight);
			else
				shootAngle = isFacingRight ? 0 : 180;

			var projectile = SpawnSystemHelper.GetNextObject (bullet.gameObject, false).GetComponent<Projectile> ();
			projectile.transform.position = firePoint.position;
			projectile.transform.rotation = Quaternion.Euler (0, 0, shootAngle);
            projectile.Initialize(gameObject, Vector2.right * (isFacingRight ? 1 : -1), Vector2.one, false, false, damage, bulletSpeed);

            projectile.gameObject.SetActive (true);
            SoundManager.PlaySfx(soundAttack);

        multiShootCounter++;
        if (multiShootCounter < multiShoot)
        {
            yield return new WaitForSeconds(multiShootRate);
            lastShoot = 0;
        }
        else
            multiShootCounter = 0;



    }
}
