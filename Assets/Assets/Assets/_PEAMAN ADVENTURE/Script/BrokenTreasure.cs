using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenTreasure : MonoBehaviour, ICanTakeDamage, IStandOnEvent
{
    public enum BlockTyle { Destroyable, ChangeSprite }
    public BlockTyle blockTyle;
    public Sprite changeSprite;
    public GameObject destroyFX;
    public Vector2 pushPlayerWhenHitFromAbove = new Vector2(0, 10);
    [Range(0,1)]
    public float volume = 0.6f;
    public AudioClip sound;

    bool isWorked = false;
    
   public void DestroyAndGivePlayerProp()
    {
        TakeDamage(1000, Vector2.zero, GameManager.Instance.Player.gameObject, Vector2.zero);
    }

    public void BoxHit()
    {
        TakeDamage(1000, Vector2.zero, GameManager.Instance.Player.gameObject, Vector2.zero);
        GameManager.Instance.Player.velocity.y = 0;
    }

    #region ICanTakeDamage implementation

    public void TakeDamage(int damage, Vector2 force, GameObject instigator, Vector3 hitPoint)
    {
        if (isWorked)
            return;

        isWorked = true;

        //try spawn random item
        var spawnItem = GetComponent<EnemySpawnItem>();
        if (spawnItem != null)
        {
            spawnItem.SpawnItem();
        }

        GetComponent<Collider2D>().enabled = false;
        
        SoundManager.PlaySfx(sound, volume);
        
        if (blockTyle == BlockTyle.Destroyable)
        {
            if (destroyFX)
                Instantiate(destroyFX, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
        else if(blockTyle == BlockTyle.ChangeSprite)
        {
            GetComponent<SpriteRenderer>().sprite = changeSprite;
        }
    }

    public void StandOnEvent(GameObject instigator)
    {
        BoxHit();
        GameManager.Instance.Player.SetForce(pushPlayerWhenHitFromAbove);
    }

    #endregion
}
