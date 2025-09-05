using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    #region Public
    public string Type;
    public GameObject Player;
    public LayerMask PlayerLayer;
    public ParticleSystem CoinVFX;
    public bool CoinSpawnedByMonster;
    public Vector2 FinalPos;
    #endregion
    #region Private
    private ItemSpawner Spawner;
    private bool DisableItemCheck;
    private bool StopCoinMoving;
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        Spawner = Camera.main.GetComponent<ItemSpawner>();
        // Check if it is Coin spawned by monster, it will have different movement
        if (CoinSpawnedByMonster)
        {
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
            GetComponent<Rigidbody2D>().velocity = new Vector2(FinalPos.x - transform.position.x, FinalPos.y - transform.position.y) * 2f;
            DisableItemCheck = true;
            StopCoinMoving = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!DisableItemCheck)
        {
            ItemCheck();
        } else
        {
            CheckFinalPos();
        }
    }

    private void CheckFinalPos()
    {
        // Check if the pos near the final pos designated
        if (Vector2.Distance(transform.position, FinalPos) < 0.1f && !StopCoinMoving)
        {
            StopCoinMoving = true;
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionY;
            GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            GetComponent<Collider2D>().enabled = true;
            DisableItemCheck = false;
        }
    }

    private void ItemCheck()
    {
        // Check collided with player
        if (Player != null && Physics2D.OverlapCircleAll(transform.position, GetComponent<CircleCollider2D>().radius * transform.localScale.x, PlayerLayer).Length > 0)
        {
            DisableItemCheck = true;
            Color c = GetComponent<SpriteRenderer>().color;
            c.a = 0f;
            GetComponent<SpriteRenderer>().color = c;
            if (Type == "Coin")
            {
                CoinVFX.Play();
                Camera.main.GetComponent<ScoreController>().AddScore();
            } else if (Type == "Heal")
            {
                Player.GetComponent<Character>().RecoverHealth(Player.GetComponent<Character>().MaxHealth * 20 / 100f);
            } else if (Type == "Upgrade")
            {
                Player.GetComponent<Character>().GainUpgradeBuff();
            }
            Camera.main.GetComponent<ItemSpawner>().RemoveItem();
            Destroy(gameObject, 1f);
        }
    }
}
