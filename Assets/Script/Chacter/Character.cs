using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    #region Component
    private Rigidbody2D rb;
    private Animator anim;
    #endregion
    #region Public
    // Input System
    public GameObject LeftInput;
    public GameObject RightInput;
    public GameObject AttackInput;
    public GameObject JumpInput;
    public LayerMask UILayer;
    // Data Value
    public float MaxHealth;
    public float AttackPoint;
    public float MovingSpeed;
    public float JumpForce;
    // UI Components
    public CharacterHealthBar HealthBar;
    public ParticleSystem UpgradeVFX;
    // Hit Box
    public LayerMask EnemyLayer;
    public GameObject HitBoxAttack1;
    public GameObject HitBoxAttack2;
    public GameObject Slash1;
    public GameObject Slash2;
    // Ground Layer
    public GameObject GroundStanding;
    public GameObject BottomPos;
    public GameObject EndingScene;
    #endregion
    #region Private
    private float CurrentHealth;
    private float BonusAttackPercent;
    private bool isUpgraded;
    private float UpgradeTime;
    private bool isAttacking;
    private float AttackWaitTime;
    private bool isJumping;
    private bool isJumpingAnimation;
    private bool isFacingLeft;
    private bool isAttackPattern1;
    private float IdleCooldownTimer;
    private bool isDead;
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        isAttackPattern1 = true;
        // Set Default Stats
        if (MaxHealth == 0)
        {
            MaxHealth = 100;
        }
        if (AttackPoint == 0)
        {
            AttackPoint = 10;
        }
        if (MovingSpeed == 0)
        {
            MovingSpeed = 2;
        }
        if (JumpForce == 0)
        {
            JumpForce = 5;
        }
        CurrentHealth = MaxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        // Check Input from Touch
        if (!isAttacking && !isDead)
        {
            TouchInputCheck();
        } else
        {
            if (AttackWaitTime > 0f)
            {
                AttackWaitTime -= Time.deltaTime;
            }
            else
            {
                isAttacking = false;
            }
        }
        // Check Idle Animation
        if (IdleCooldownTimer < 1f && !isDead)
        {
            IdleCooldownTimer += Time.deltaTime;
        } else
        {
            anim.SetTrigger("Idle");
        }
        // Upgrade Effect
        if (isUpgraded)
        {
            if (UpgradeTime > 0f)
            {
                UpgradeTime -= Time.deltaTime;
            } else
            {
                isUpgraded = false;
                BonusAttackPercent = 0f;
                UpgradeVFX.Stop();
            }
        }
        // Set Health Bar
        HealthBar.UpdateBar(MaxHealth, CurrentHealth);
        CheckHealth();
    }

    private void TouchInputCheck()
    {
        if (Input.touchCount > 0)
        {
            // Get Data for Touches
            List<string> TouchInputs = new();
            for (int i=0;i<Input.touchCount;  i++)
            {
                Touch touch = Input.GetTouch(i);
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(touch.position), new Vector3(0, 0, 1), Mathf.Infinity, UILayer);
                if (hit.collider != null)
                {
                    if (hit.collider.gameObject == LeftInput)
                    {
                        TouchInputs.Add("Left");
                    } 
                    else if (hit.collider.gameObject == RightInput)
                    {
                        TouchInputs.Add("Right");
                    }
                    else if (hit.collider.gameObject == AttackInput)
                    {
                        TouchInputs.Add("Attack");
                    }
                    else if (hit.collider.gameObject == JumpInput)
                    {
                        TouchInputs.Add("Jump");
                    }
                }
            }
            // Set Trans of Touch activated
            UITransparency(TouchInputs);
            // Looping from end to start to check which on is touch later
            // Example: Touch Left before Touch right will result into right move
            bool AlreadyProceedMovement = false;
            for (int i=TouchInputs.Count-1; i>=0; i--)
            {
                // Left Right Move
                if (!AlreadyProceedMovement)
                {
                    if (TouchInputs[i] == "Left" || TouchInputs[i] == "Right")
                    {
                        AlreadyProceedMovement = true;
                        MovingLeftRight(TouchInputs[i] == "Left");
                    }
                }
                if (!isJumping)
                {
                    // Attack 
                    if (!isAttacking)
                    {
                        if (TouchInputs[i] == "Attack")
                        {
                            isAttacking = true;
                            AttackWaitTime = 0.5f;
                            Attack();
                        }
                    }
                    // Jump
                    if (TouchInputs[i] == "Jump" && JumpForce > 0f)
                    {
                        isJumping = true;
                        isJumpingAnimation = true;
                        StartCoroutine(Jump());
                    }
                }
            }
            // Stop Moving Animation and velocity when no moving input called
            if (!AlreadyProceedMovement)
            {
                StopMoving();
            }
        } else
        {
            // Set Data for no touch input
            UITransparency(new List<string>());
            StopAnimation();
        }
    }

    private void CheckHealth()
    {
        // Check Health
        if (CurrentHealth <= 0f && !isDead)
        {
            isDead = true;
            StartCoroutine(LoseAnimation());
        }
    }

    private IEnumerator LoseAnimation()
    {
        // Show Ending Scene
        EndingScene.SetActive(true);
        for (int i=0;i<50;i++)
        {
            Color c = EndingScene.GetComponent<SpriteRenderer>().color;
            c.a += 0.02f;
            EndingScene.GetComponent<SpriteRenderer>().color = c;
            yield return new WaitForSeconds(0.1f);
        }
        // Set Data and Change Scene
        PlayerPrefs.SetInt("Score", Camera.main.GetComponent<ScoreController>().Score);
        PlayerPrefs.SetString("Result", "DEFEAT!");
        SceneManager.LoadSceneAsync(2);
        SceneManager.UnloadSceneAsync(1);
    }

    private void UITransparency(List<string> TouchInputs)
    {
        // If touch contain data then transparency = 1
        // if not trans = 0.5f
        if (TouchInputs.Contains("Left"))
        {
            Color c = LeftInput.GetComponent<Image>().color;
            c.a = 1f;
            LeftInput.GetComponent<Image>().color = c;
        }
        else
        {
            Color c = LeftInput.GetComponent<Image>().color;
            c.a = 0.5f;
            LeftInput.GetComponent<Image>().color = c;
        }
        if (TouchInputs.Contains("Right"))
        {
            Color c = RightInput.GetComponent<Image>().color;
            c.a = 1f;
            RightInput.GetComponent<Image>().color = c;
        }
        else
        {
            Color c = RightInput.GetComponent<Image>().color;
            c.a = 0.5f;
            RightInput.GetComponent<Image>().color = c;
        }
        if (TouchInputs.Contains("Attack"))
        {
            Color c = AttackInput.GetComponent<Image>().color;
            c.a = 1f;
            AttackInput.GetComponent<Image>().color = c;
            Color c1 = AttackInput.transform.GetChild(0).GetComponent<Image>().color;
            c1.a = 1f;
            AttackInput.transform.GetChild(0).GetComponent<Image>().color = c1;
        }
        else
        {
            Color c = AttackInput.GetComponent<Image>().color;
            c.a = 0.5f;
            AttackInput.GetComponent<Image>().color = c;
            Color c1 = AttackInput.transform.GetChild(0).GetComponent<Image>().color;
            c1.a = 0.5f;
            AttackInput.transform.GetChild(0).GetComponent<Image>().color = c1;
        }
        if (TouchInputs.Contains("Jump"))
        {
            Color c = JumpInput.GetComponent<Image>().color;
            c.a = 1f;
            JumpInput.GetComponent<Image>().color = c;
            Color c1 = JumpInput.transform.GetChild(0).GetComponent<Image>().color;
            c1.a = 1f;
            JumpInput.transform.GetChild(0).GetComponent<Image>().color = c1;
        }
        else
        {
            Color c = JumpInput.GetComponent<Image>().color;
            c.a = 0.5f;
            JumpInput.GetComponent<Image>().color = c;
            Color c1 = JumpInput.transform.GetChild(0).GetComponent<Image>().color;
            c1.a = 0.5f;
            JumpInput.transform.GetChild(0).GetComponent<Image>().color = c1;
        }
    }

    private void MovingLeftRight(bool isLeft)
    {
        // Set Timer for Idle animation
        IdleCooldownTimer = 0f;
        // If not moving when jumping then set animator trigger
        if (!isJumping)
        {
            if (!anim.GetCurrentAnimatorStateInfo(0).IsTag("Run"))
            {
                BaseAnimationActivate();
            }
            anim.SetTrigger("Run");
            // Animation speed by moving speed compared to default
            anim.speed = MovingSpeed / 2f;
        }
        // Rotate
        if (isFacingLeft && !isLeft)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            isFacingLeft = false;
        } else if (!isFacingLeft && isLeft)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            isFacingLeft = true;
        }
        // Moving by velocity
        rb.velocity = new Vector2(MovingSpeed * (isLeft ? -1 : 1), rb.velocity.y);
    }

    private void StopMoving()
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
        if (anim.GetCurrentAnimatorStateInfo(0).IsTag("Run"))
            BaseAnimationActivate();
    }

    private void StopAnimation()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsTag("Run"))
            BaseAnimationActivate();
        if (anim.GetCurrentAnimatorStateInfo(0).IsTag("Jump"))
            BaseAnimationActivate();
    }

    private void Attack()
    {
        // Set Idle animation time
        IdleCooldownTimer = 0f;
        // Set Animator to base if the curernt state is not attack
        if (!anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
        BaseAnimationActivate();
        // Rotate between 2 attack patterns
        if (isAttackPattern1)
        {
            anim.SetTrigger("Attack1");
            isAttackPattern1 = false;
            Collider2D[] cols = Physics2D.OverlapAreaAll(HitBoxAttack1.GetComponent<Collider2D>().bounds.min, HitBoxAttack1.GetComponent<Collider2D>().bounds.max, EnemyLayer);
            if (cols.Length > 0)
            {
                foreach (var col in cols)
                {
                    if (col.GetComponent<Boar>()!=null)
                    {
                        col.GetComponent<Boar>().ReceiveDamage(AttackPoint * (100+ BonusAttackPercent)/100f);
                    }
                }
            }
            StartCoroutine(SpawnSlash(1));
        } else
        {
            anim.SetTrigger("Attack2");
            isAttackPattern1 = true;
            Collider2D[] cols = Physics2D.OverlapAreaAll(HitBoxAttack2.GetComponent<Collider2D>().bounds.min, HitBoxAttack2.GetComponent<Collider2D>().bounds.max, EnemyLayer);
            if (cols.Length > 0)
            {
                foreach (var col in cols)
                {
                    if (col.GetComponent<Boar>() != null)
                    {
                        col.GetComponent<Boar>().ReceiveDamage(AttackPoint * (100 + BonusAttackPercent) / 100f);
                    }
                }
            }
            StartCoroutine(SpawnSlash(2));
        }
        rb.velocity = new Vector2(0, 0);
    }

    private IEnumerator SpawnSlash(int SlashNo)
    {
        // Wait for animation and spawn slash
        yield return new WaitForSeconds(10 / 60f);
        GameObject Slash = Instantiate(SlashNo == 1 ? Slash1 : Slash2, SlashNo == 1 ? Slash1.transform.position : Slash2.transform.position, Quaternion.identity);
        Slash.GetComponent<SwordSlash>().IsLeft = isFacingLeft;
        Slash.GetComponent<SwordSlash>().MovingSpeed = 4f;
        Slash.GetComponent<SwordSlash>().Duration = 1f;
        Slash.GetComponent<SwordSlash>().Damage = AttackPoint * (100 + BonusAttackPercent) / 100f * 50 / 100f;
        Slash.SetActive(true);
    }

    private IEnumerator Jump()
    {
        // Jump by jump force
        IdleCooldownTimer = 0f;
        float Modifier = JumpForce / 5f;
        BaseAnimationActivate();
        anim.SetTrigger("Jump");
        anim.speed = 1 / Modifier;
        yield return new WaitForSeconds(10 / 60f * Modifier);
        anim.speed = 1;
        Debug.Log("Jump");
        rb.AddForce(new Vector2(0, JumpForce), ForceMode2D.Impulse);
        isJumpingAnimation = false;
    }

    private void BaseAnimationActivate()
    {
        // Base
        ResetAllTriggers();
        if (!anim.GetCurrentAnimatorStateInfo(0).IsTag("Base"))
        {
            anim.SetTrigger("Base");
        }
        anim.speed = 1;
    }

    private void ResetAllTriggers()
    {
        foreach (var param in anim.parameters)
        {
            if (param.type == AnimatorControllerParameterType.Trigger)
            {
                anim.ResetTrigger(param.name);
            }
        }
    }

    public void ReceiveDamage(float Damage)
    {
        // Receive damage and show number
        CurrentHealth -= Damage;
        ShowDamageNumber(Damage);
    }

    public void RecoverHealth(float value)
    {
        if (CurrentHealth + value > MaxHealth)
        {
            value = MaxHealth - CurrentHealth;
            CurrentHealth = MaxHealth;
        } else
        {
            CurrentHealth += value;
        }
        ShowHealNumber(value);
    }

    public void GainUpgradeBuff()
    {
        // Reset time and bool for buff
        UpgradeTime = 5f;
        isUpgraded = true;
        BonusAttackPercent = 50f;
        UpgradeVFX.Play();
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

    protected void ShowHealNumber(float Heal)
    {
        GameObject go = new GameObject();
        go.transform.position = transform.position;
        TextMeshPro tm = go.AddComponent<TextMeshPro>();
        Rigidbody2D rb = go.AddComponent<Rigidbody2D>();
        rb.isKinematic = true;
        tm.color = Color.green;
        tm.sortingOrder = 21;
        tm.fontSize = 3.5f;
        tm.fontStyle = FontStyles.Italic | FontStyles.Bold;
        tm.horizontalAlignment = HorizontalAlignmentOptions.Center;
        tm.verticalAlignment = VerticalAlignmentOptions.Middle;
        tm.text = Heal.ToString();
        rb.velocity = new Vector2(0, 0.4f);
        Destroy(go, 3f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Set ground standing for the current ground standing on
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground") || collision.gameObject.layer == LayerMask.NameToLayer("UpperGround"))
        {
            GroundStanding = collision.gameObject;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // Complete jump animation when reaching ground
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground") || collision.gameObject.layer == LayerMask.NameToLayer("UpperGround"))
        {
            if (isJumping && !isJumpingAnimation && rb.velocity.y <= 0)
            {
                anim.SetTrigger("JumpDone");
                isJumping = false;
                rb.velocity = new Vector2(rb.velocity.x, 0);
            }
        }
    }
}
