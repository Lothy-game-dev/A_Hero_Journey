using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeeStinger : MonoBehaviour
{
    #region Component
    private Rigidbody2D rb;
    private Collider2D col;
    #endregion
    #region Public
    public LayerMask PlayerLayer;
    public Vector2 MovingVector;
    public float Damage;
    #endregion
    #region Private
    private bool AlreadyHit;
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        rb.velocity = MovingVector;
        Destroy(gameObject, 2f);
    }

    // Update is called once per frame
    void Update()
    {
        if (!AlreadyHit)
        {
            CheckHit();
        }
    }

    private void CheckHit()
    {
        Collider2D[] cols = Physics2D.OverlapAreaAll(col.bounds.min, col.bounds.max, PlayerLayer);
        if (cols.Length> 0)
        {
            foreach (var plcol in cols)
            {
                if (plcol.GetComponent<Character>()!=null)
                {
                    AlreadyHit = true;
                    plcol.GetComponent<Character>().ReceiveDamage(Damage);
                    Destroy(gameObject);
                    break;
                }
            }
        }
    }
}
