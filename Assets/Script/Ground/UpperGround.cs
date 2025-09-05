using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpperGround : MonoBehaviour
{
    #region Public
    public GameObject SurfacePos;
    public LayerMask MonsterLayer;
    public GameObject Character;
    #endregion
    // Start is called before the first frame update
    void Awake()
    {
    }

    // Update is called once per frame
    void Update()
    {
        CheckAllMonstersAndPlayer();
    }

    private void CheckAllMonstersAndPlayer()
    {
        // Check if character's bottom position is below the surface of the ground, then ignores collision
        if (Character!=null && Character.GetComponent<Character>()!=null && Character.GetComponent<Character>().BottomPos!=null)
        {
            if (Character.GetComponent<Character>().BottomPos.transform.position.y < SurfacePos.transform.position.y)
            {
                if (!Physics2D.GetIgnoreCollision(GetComponent<Collider2D>(), Character.GetComponent<Collider2D>()))
                    Physics2D.IgnoreCollision(GetComponent<Collider2D>(), Character.GetComponent<Collider2D>(), true);
            }
            else
            {
                if (Physics2D.GetIgnoreCollision(GetComponent<Collider2D>(), Character.GetComponent<Collider2D>()))
                    Physics2D.IgnoreCollision(GetComponent<Collider2D>(), Character.GetComponent<Collider2D>(), false);
            }
        }
        // Check if any monster's bottom position inside the collider's area is below the surface of the ground, then ignores collision
        Collider2D[] cols = Physics2D.OverlapAreaAll(GetComponent<Collider2D>().bounds.min, GetComponent<Collider2D>().bounds.max, MonsterLayer);
        if (cols.Length > 0f)
        {
            foreach (var col in cols)
            {
                if (col.GetComponent<Monster>() != null && col.GetComponent<Monster>().BottomPos != null)
                {
                    if (col.GetComponent<Monster>().BottomPos.transform.position.y < SurfacePos.transform.position.y)
                    {
                        if (!Physics2D.GetIgnoreCollision(GetComponent<Collider2D>(), col))
                            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), col, true);
                    }
                    else
                    {
                        if (Physics2D.GetIgnoreCollision(GetComponent<Collider2D>(), col))
                            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), col, false);
                    }
                }
            }
        }
    }
}
