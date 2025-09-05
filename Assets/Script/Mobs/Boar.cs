using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boar : Monster
{
    #region Component
    #endregion
    #region Public
    #endregion
    #region Private
    private bool isAttackMode;
    #endregion
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
        if (!isAttackMode)
        {
            ChangeDirectionCheck();
        }
    }

    protected override void CheckMoving()
    {
        // Moving normally
        if (!isAttackMode)
        {
            if (!anim.GetCurrentAnimatorStateInfo(0).IsTag("Walk"))
            {
                anim.SetTrigger("Walk");
            }
            if (isFacingLeft && !isMovingLeft)
            {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                isFacingLeft = false;
                RotateHealthBar();
            } else if (!isFacingLeft && isMovingLeft)
            {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                isFacingLeft = true;
                RotateHealthBar();
            }
            rb.velocity = new Vector2(MovingSpeed * (isMovingLeft ? -1 : 1), rb.velocity.y);
        } else
        {
            // When in attack mode, moving faster and moving toward the player
            if (Player!=null && Player.GetComponent<Character>()!=null && Player.GetComponent<Character>().GroundStanding == GroundStanding)
            {
                if (!anim.GetCurrentAnimatorStateInfo(0).IsTag("Run"))
                {
                    anim.SetTrigger("Run");
                }
                // Allow rotating when distance from player is big enough
                if (Mathf.Abs(Player.transform.position.x - transform.position.x) >= 0.5f)
                {
                    isMovingLeft = Player.transform.position.x < transform.position.x;
                }
                if (isFacingLeft && !isMovingLeft)
                {
                    transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                    isFacingLeft = false;
                    RotateHealthBar();
                }
                else if (!isFacingLeft && isMovingLeft)
                {
                    transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                    isFacingLeft = true;
                    RotateHealthBar();
                }
                rb.velocity = new Vector2(MovingSpeed * 2 * (isMovingLeft ? -1 : 1), rb.velocity.y);
            } else
            {
                isAttackMode = false;
            }
        }
    }

    protected override void OnReceivingDamageTrigger()
    {
        // Enter attack mode after receiving damage
        isAttackMode = true;
    }

    protected override void OnDealingDamageTrigger()
    {
        // Exit attack mode after dealing damage
        isAttackMode = false;
        ChangeDirectionTimer = 0f;
    }
}
