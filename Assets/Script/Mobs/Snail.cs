using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snail : Monster
{
    #region Component
    #endregion
    #region Public
    #endregion
    #region Private
    private bool isDefenseMode;
    private float DefenseDuration;
    private float DefenseCooldown;
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
        if (!isDefenseMode)
        {
            ChangeDirectionCheck();
        } else
        {
            if (DefenseDuration > 0f)
            {
                DefenseDuration -= Time.deltaTime;
            }
            else
            {
                isDefenseMode = false;
                ReceivedDamageReducePercent = 0;
            }
        }
        DefenseCooldown -= Time.deltaTime;
    }


    protected override void CheckMoving()
    {
        // If not in defense mode, moving normally
        if (!isDefenseMode)
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
            }
            else if (!isFacingLeft && isMovingLeft)
            {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                isFacingLeft = true;
                RotateHealthBar();
            }
            rb.velocity = new Vector2(MovingSpeed * (isMovingLeft ? -1 : 1), rb.velocity.y);
        }
        else
        {
            // In defense mode, stop moving and change animation
            if (Player != null && Player.GetComponent<Character>() != null)
            {
                if (!anim.GetCurrentAnimatorStateInfo(0).IsTag("Defense"))
                {
                    anim.SetTrigger("Defense");
                }
                isMovingLeft = Player.transform.position.x < transform.position.x;
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
                rb.velocity = new Vector2(0, 0);
            }
            else
            {
                isDefenseMode = false;
            }
        }
    }

    protected override void OnDealingDamageTrigger()
    {

    }

    protected override void OnReceivingDamageTrigger()
    {
        // Enter defense mode upon receiving damage
        if (!isDefenseMode && DefenseCooldown <= 0f)
        {
            isDefenseMode = true;
            DefenseDuration = 5f;
            DefenseCooldown = 10f;
            ReceivedDamageReducePercent = 100f;
        }
    }

}
