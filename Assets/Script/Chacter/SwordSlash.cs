using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordSlash : MonoBehaviour
{
    #region Component
    private Rigidbody2D rb;
    private Collider2D col;
    private SpriteRenderer ren;
    #endregion
    #region Public
    public LayerMask EnemyLayer;
    public float MovingSpeed;
    public bool IsLeft;
    public float Duration;
    public float Damage;
    #endregion
    #region Private
    private List<GameObject> AlreadyHit;
    private float AlreadyHitResetTimer;
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        ren =  GetComponent<SpriteRenderer>();
        AlreadyHit = new();
        if (IsLeft)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        Color c = ren.color;
        c.a = 0f;
        ren.color = c;
        rb.velocity = new Vector2(MovingSpeed * (IsLeft ? -1 : 1), 0);
        Destroy(gameObject, Duration);
        // Start animation
        StartCoroutine(AnimationSlash());
    }

    // Update is called once per frame
    void Update()
    {
        CheckDamage();
        // Reset already hit to allow multiple hits
        if (AlreadyHitResetTimer > 0f)
        {
            AlreadyHitResetTimer -= Time.deltaTime;
        } else
        {
            AlreadyHitResetTimer = 0.2f;
            AlreadyHit = new();
        }
    }

    private IEnumerator AnimationSlash()
    {
        for (int i=0;i<10;i++)
        {
            Color c = ren.color;
            c.a += 0.1f;
            ren.color = c;
            yield return new WaitForSeconds(0.02f);
        }
    }

    private void CheckDamage()
    {
        // Check Damage
        Collider2D[] cols = Physics2D.OverlapAreaAll(col.bounds.min, col.bounds.max, EnemyLayer);
        if (cols.Length > 0)
        {
            foreach (var enemy in cols)
            {
                // Prevent hitting enemy too often
                if (enemy.GetComponent<Monster>()!=null && !AlreadyHit.Contains(enemy.gameObject))
                {
                    AlreadyHit.Add(enemy.gameObject);
                    enemy.GetComponent<Monster>().ReceiveDamage(Damage);
                }
            }
        }
    }
}
