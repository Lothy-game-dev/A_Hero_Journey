using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bee : Monster
{
    #region Component
    #endregion
    #region Public
    public GameObject Stinger;
    public GameObject StingerSpawnPos;
    #endregion
    #region Private
    private bool isAttacking;
    private float AttackCooldown;
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
        if (!isAttacking)
        {
            ChangeDirectionCheck();
            if (AttackCooldown < 0)
            {
                CheckAttack();
            }
        }
        AttackCooldown -= Time.deltaTime;
    }


    protected override void CheckMoving()
    {
        // If not attacking, moving normally
        if (!isAttacking)
        {
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
            // If attacking, facing the player
            if (Player != null && Player.GetComponent<Character>() != null)
            {
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
        }
    }

    private IEnumerator Attack()
    {
        // Attack Pattern
        isAttacking = true;
        AttackCooldown = 3f;
        anim.SetTrigger("Attack");
        // Wait for animation
        yield return new WaitForSeconds(30/60f);
        // Spawn stinger to deal damage
        GameObject Sting = Instantiate(Stinger, StingerSpawnPos.transform.position, Quaternion.identity);
        // Angle of the bee to the player
        float Angle = Vector2.Angle(new Vector2(0, 1), Player.transform.position - transform.position);
        Vector2 MovingVector = Player.transform.position - transform.position;
        // Set data for stinger
        Sting.transform.rotation = Quaternion.Euler(0, 0, Angle * (isMovingLeft ? 1 : -1));
        Sting.GetComponent<BeeStinger>().Damage = AttackPoint;
        Sting.GetComponent<BeeStinger>().MovingVector = MovingVector / MovingVector.magnitude * 2f;
        Sting.SetActive(true);
        yield return new WaitForSeconds(30 / 60f);
        isAttacking = false;
    }

    private void CheckAttack()
    {
        // Check condition to be able to attack player
        if (Player != null && Player.GetComponent<Character>() != null && Vector2.Distance(Player.transform.position,transform.position) < 2f)
        {
            StartCoroutine(Attack());
        }
    }

    protected override void OnDealingDamageTrigger()
    {

    }

    protected override void OnReceivingDamageTrigger()
    {

    }
}
