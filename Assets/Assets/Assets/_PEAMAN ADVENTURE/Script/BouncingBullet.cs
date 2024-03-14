using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class BouncingBullet : MonoBehaviour
{
    Controller2D controller;
    protected Vector3 velocity;
    protected float velocityXSmoothing = 0;
    public float gravity = 20;
    public float moveSpeed = 5;
    public float bounceForce = 5;
    float bouncingForce;
    public float reduceForcePerImpact = 0.5f;
    public GameObject bouncingFX, destroyFX;
    Vector2 _direction = Vector2.right;

    public LayerMask damageLayer;
    bool allowHitOwner = false;
    GameObject owner;

    void Start()
    {
        controller = GetComponent<Controller2D>();
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    public void Init(Vector2 dir, GameObject _owner)
    {
        Reset();

        _direction = dir;
        owner = _owner;
        Invoke("AllowHitOwner", 0.2f);
    }

    private void Reset()
    {
        velocity = Vector2.zero;
        allowHitOwner = false;
        bouncingForce = bounceForce;
    }

    void AllowHitOwner()
    {
        allowHitOwner = true;
    }

    void Flip()
    {
        _direction.x = _direction.x * -1;
    }

    private void LateUpdate()
    {

        float targetVelocityX = _direction.x * moveSpeed;
        float finalGravity = gravity;

        velocity.x = targetVelocityX;

        velocity.y += -finalGravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime, false);

        if (controller.collisions.above)
        {
            velocity.y = 0;
        }

        CheckContactEnemy();

        if (controller.collisions.below)
        {
            if (!allowHitOwner && controller.collisions.ClosestHit.collider.gameObject == owner)
                return;

            Jump();
            if (bouncingFX)
                SpawnSystemHelper.GetNextObject(bouncingFX, true, controller.collisions.ClosestHit.point);

            //allowHitOwner = true;
        }

        if (controller.collisions.right || controller.collisions.left)
        {
            if (!allowHitOwner && controller.collisions.ClosestHit.collider.gameObject == owner)
                return;

            Flip();
            if (bouncingFX)
                SpawnSystemHelper.GetNextObject(bouncingFX, true, controller.collisions.ClosestHit.point);
        }

       

    }

    void CheckContactEnemy()
    {
        var hit = Physics2D.BoxCast(controller.boxcollider.bounds.center, controller.boxcollider.bounds.size * 0.9f, 0, Vector2.zero,0, damageLayer);
        if (hit)
        {
            if (!allowHitOwner && hit.collider.gameObject == owner)
                return;

            var contactEventObj = (ICanTakeDamage)hit.collider.gameObject.GetComponent(typeof(ICanTakeDamage));
            if (contactEventObj != null)
            {
                contactEventObj.TakeDamage(1, Vector2.zero, gameObject, controller.collisions.ClosestHit.point);
                gameObject.SetActive(false);

                if (destroyFX)
                    SpawnSystemHelper.GetNextObject(destroyFX, true, transform.position);
            }
        }
    }

    void Jump()
    {
        bouncingForce -= reduceForcePerImpact;
        if (bouncingForce <= 0)
            gameObject.SetActive(false);
        else
            velocity.y = bouncingForce;
    }
}
