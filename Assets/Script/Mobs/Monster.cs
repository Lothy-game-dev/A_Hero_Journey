using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class Monster : MonoBehaviour
{
    #region Component
    protected Rigidbody2D rb;
    protected Animator anim;
    #endregion
    #region Public
    public float MaxHealth;
    public float AttackPoint;
    public float MovingSpeed;
    public int CoinSpawns;
    public GameObject Player;
    public GameObject GroundStanding;
    public GameObject BottomPos;
    public float ChangeDirectionDelay;
    public CharacterHealthBar HealthBar;
    #endregion
    #region Protected
    protected float CurrentHealth;
    protected float DelayAttack;
    protected bool AllowAttack;
    protected float ChangeDirectionTimer;
    protected bool isFacingLeft;
    protected bool isMovingLeft;
    protected float ReceivedDamageReducePercent;
    protected bool AlreadyDead;
    #endregion
    // Start is called before the first frame update
    protected void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        AllowAttack = true;
        ChangeDirectionTimer = ChangeDirectionDelay / 2f;
        CurrentHealth = MaxHealth;
        isFacingLeft = true;
        isMovingLeft = true;
    }

    // Update is called once per frame
    protected void Update()
    {
        if (!AllowAttack)
        {
            if (DelayAttack > 0f)
            {
                DelayAttack -= Time.deltaTime;
            }
            else
            {
                AllowAttack = true;
            }
        }
        if (!AlreadyDead)
        CheckMoving();
        HealthBar.UpdateBar(MaxHealth, CurrentHealth);
        CheckHealth();
    }

    protected abstract void CheckMoving();
    protected abstract void OnReceivingDamageTrigger();
    protected abstract void OnDealingDamageTrigger();

    private void CheckHealth()
    {
        if (CurrentHealth <= 0f)
        {
            if (!AlreadyDead)
            {
                AlreadyDead = true;
                Dead();
            }
        }
    }

    protected void RotateHealthBar()
    {
        HealthBar.transform.localScale = new Vector3(HealthBar.transform.localScale.x, HealthBar.transform.localScale.y, HealthBar.transform.localScale.z);
    }

    private void Dead()
    {
        SpawnCoins();
        StartCoroutine(DeadAnimation());
    }

    private void SpawnCoins()
    {
        // Spawn coins by number
        float FirstCoinAngle = 0;
        float CoinAngleDiff = 0;
        float Radius = 1f;
        if (CoinSpawns == 3)
        {
            // angle from the vector2(0,1): -60, 0, 60
            FirstCoinAngle = -60;
            CoinAngleDiff = 60;
        } else if (CoinSpawns == 4)
        {
            // angle from the vector2(0,1): -75, -25, 25, 75
            FirstCoinAngle = -75;
            CoinAngleDiff = 50;
        } else if (CoinSpawns == 5)
        {
            // angle from the vector2(0,1): -60, -30, 0, 30, 60
            FirstCoinAngle = -60;
            CoinAngleDiff = 30;
        }
        for (int i = 0; i < CoinSpawns; i++)
        {
            float xPos = transform.position.x + Radius * Mathf.Sin(Mathf.Abs(FirstCoinAngle) * Mathf.Deg2Rad) * (FirstCoinAngle < 0 ? -1 : 1);
            float yPos = transform.position.y + Radius * Mathf.Cos(Mathf.Abs(FirstCoinAngle) * Mathf.Deg2Rad);
            Camera.main.GetComponent<ItemSpawner>().SpawnCoinByMonster(transform.position, new Vector2(xPos, yPos));
            FirstCoinAngle += CoinAngleDiff;
        }
    }

    private IEnumerator DeadAnimation()
    {
        // Stop Moving and remove any collider
        GetComponent<Rigidbody2D>().isKinematic = true;
        GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        GetComponent<Collider2D>().enabled = false;
        HealthBar.gameObject.SetActive(false);
        for (int i=0;i<10;i++)
        {
            Color c = GetComponent<SpriteRenderer>().color;
            c.a -= 0.1f;
            GetComponent<SpriteRenderer>().color = c;
            yield return new WaitForSeconds(0.1f);
        }
        Camera.main.GetComponent<MonsterSpawner>().MonsterDead();
        Destroy(gameObject, 1f);
    }

    public void ReceiveDamage(float Damage)
    {
        CurrentHealth -= Damage * (100 - ReceivedDamageReducePercent) / 100f;
        ShowDamageNumber(Damage * (100 - ReceivedDamageReducePercent) / 100f);
        OnReceivingDamageTrigger();
    }

    protected void ShowDamageNumber(float Damage)
    {
        GameObject go = new GameObject();
        go.transform.position = transform.position;
        TextMeshPro tm = go.AddComponent<TextMeshPro>();
        Rigidbody2D rb = go.AddComponent<Rigidbody2D>();
        rb.isKinematic = true;
        tm.color = Color.red;
        tm.sortingOrder = 21;
        tm.fontSize = 3.5f;
        tm.fontStyle = FontStyles.Italic | FontStyles.Bold;
        tm.horizontalAlignment = HorizontalAlignmentOptions.Center;
        tm.verticalAlignment = VerticalAlignmentOptions.Middle;
        tm.text = Damage.ToString();
        rb.velocity = new Vector2(0, 0.4f);
        Destroy(go, 3f);
    }

    protected void ChangeDirectionCheck()
    {
        if (ChangeDirectionTimer > 0f)
        {
            ChangeDirectionTimer -= Time.deltaTime;
        }
        else
        {
            ChangeDirectionTimer = ChangeDirectionDelay;
            isMovingLeft = !isMovingLeft;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (AllowAttack)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                if (collision.gameObject.GetComponent<Character>() != null)
                {
                    DelayAttack = 1f;
                    AllowAttack = false;
                    collision.gameObject.GetComponent<Character>().ReceiveDamage(AttackPoint);
                    OnDealingDamageTrigger();
                }
            }
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground") || collision.gameObject.layer == LayerMask.NameToLayer("UpperGround"))
        {
            GroundStanding = collision.gameObject;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Collision with border will immediately change direction moving
        if (collision.gameObject.layer == LayerMask.NameToLayer("Border"))
        {
            ChangeDirectionTimer = 0f;
        }
    }
}
