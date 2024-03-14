using UnityEngine;
using System.Collections;

public class Block : MonoBehaviour, ICanTakeDamage
{
    public enum BlockTyle { Destroyable, Rocky, Hidden }
    public BlockTyle blockTyle;
    public LayerMask enemiesLayer;

    public int maxHit = 1;
    public float pushEnemyUp = 7f;
    public float sizeDetectEnemies = 0.25f;
    public int pointToAdd = 100;

    public float offsetCheckEnemyY = 0.1f;

    [Header("Destroyable")]
    public GameObject DestroyEffect;

    public Sprite imageBlockStatic;

    [Header("Sound")]
    public AudioClip soundDestroy;
    public AudioClip soundHit;
    [Range(0, 1)]
    public float soundDestroyVolume = 0.5f;

    Animator anim;
    SpriteRenderer spriteRenderer;
    Sprite oldSprite;
    int currentHitLeft;

    [ReadOnly] public bool isShowed = false;

    // Use this for initialization
    void Start()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        oldSprite = spriteRenderer.sprite;
        currentHitLeft = Mathf.Clamp(maxHit, 1, int.MaxValue);

        spriteRenderer.enabled = blockTyle != BlockTyle.Hidden;

    }

    public void BoxHit()
    {
        if (isWaitNextHit)
            return;


        if (currentHitLeft <= 0)
            return;

        StartCoroutine(BoxHitCo());
    }

    bool isWaitNextHit = false;

    IEnumerator BoxHitCo()
    {
        isWaitNextHit = true;

        CheckEnemiesOnTop();

        anim.SetTrigger("hit");

        var spawnItem = GetComponent<EnemySpawnItem>();
        if (spawnItem != null)
        {
            spawnItem.SpawnItem();
        }

        currentHitLeft--;
        if (currentHitLeft > 0)
        {
            yield return null;
            isWaitNextHit = false;
            yield break;
        }

        if (blockTyle == BlockTyle.Destroyable)
        {
            if (DestroyEffect != null)
                SpawnSystemHelper.GetNextObject(DestroyEffect, true, transform.position);



            isWaitNextHit = false;
            SoundManager.PlaySfx(soundDestroy);
            Destroy(gameObject);
        }
        else if (blockTyle == BlockTyle.Rocky || blockTyle == BlockTyle.Hidden)
        {
            spriteRenderer.enabled = true;
            spriteRenderer.sprite = imageBlockStatic;
            isShowed = true;
            SoundManager.PlaySfx(soundHit);
        }

       
        yield return null;
        isWaitNextHit = false;
    }

    void CheckEnemiesOnTop()
    {
        //check if any enemies on top? kill them
        var hits = Physics2D.CircleCastAll(transform.position + Vector3.up * offsetCheckEnemyY, sizeDetectEnemies, Vector2.zero, 0, 1 << LayerMask.NameToLayer("Enemies"));
        if (hits.Length > 0)
        {
            foreach (var hit in hits)
            {
                if (hit.collider.gameObject.GetComponent<Block>() == null)
                {

                    var damage = (ICanTakeDamage)hit.collider.gameObject.GetComponent(typeof(ICanTakeDamage));
                    if (damage != null)
                        damage.TakeDamage(10000, Vector2.up * pushEnemyUp, gameObject, Vector2.zero); //kill it right away
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        if (Application.isPlaying)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * offsetCheckEnemyY, sizeDetectEnemies);


        GetComponent<SpriteRenderer>().enabled = blockTyle != BlockTyle.Hidden;
        if (blockTyle == BlockTyle.Hidden)
        {
            Gizmos.color = new Color(1, 1, 1, 0.3f);
            Gizmos.DrawCube(transform.position, GetComponent<BoxCollider2D>().size);
        }
    }

    public void TakeDamage(int damage, Vector2 force, GameObject instigator, Vector3 hitPoint)
    {
        BoxHit();
    }
}
