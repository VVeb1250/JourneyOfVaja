using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BossScript : MonoBehaviour
{
    public Transform player;
    private Rigidbody2D rigidbody2D;
    private Animator animator;
    public float walkSpeed;
    public float runSpeed;
    public float forceDodge;
    public Vector3 footOffset;
    public Vector2 footArea;
    public Vector3 dodgedOffset;
    public float dodgedRaduis;
    public LayerMask playerLayer;
    public LayerMask playerHit;
    private bool attack;
    private bool dodged;
    private float startTime;
    private float timeSkill2;
    private float timeEvade;
    public GameObject hitArea;
    public GameObject effectSkill2;
    public GameObject areaSkill2;
    public GameObject deathEffect;
    public Vector3 effectPos;
    public Vector3 offset;
    public float hp = 250.0f;
    public Slider hpSlider;
    private bool isDeath = false;
    private bool isAttack = false;
    private bool isDodged = false;
    private bool isInstantiated = false;
    private bool isSkill2 = false;
    private AnimatorStateInfo stateInfo;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidbody2D = this.gameObject.GetComponent<Rigidbody2D>();
        animator = this.gameObject.GetComponent<Animator>();
        startTime = Time.time;
        timeSkill2 = Time.time;
        timeEvade = Time.time;
        GameObject playerObject = GameObject.FindWithTag("Player");
        GameObject sliderObject = GameObject.FindWithTag("BarBoss");

        if (playerObject != null)
        {
            player = playerObject.GetComponent<Transform>();
        }
        if (sliderObject != null)
        {
            hpSlider = sliderObject.GetComponent<Slider>();
        }

        hpSlider.maxValue = hp;
        hpSlider.value = hp;
    }

    // Update is called once per frame
    void Update()
    {
        stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (hp <= 0)
        {
            hp = 0;
            isDeath = true;
            animator.SetTrigger("death");
            hp += 0.1f;
            isInstantiated = false;
        }
        // Debug.Log(isDeath);

        if (Vector3.Distance(transform.position, player.position) < 10 && Vector3.Distance(transform.position, player.position) >= 7 && !isDeath && !isAttack && !isSkill2)
        {
            animator.SetBool("walk", true);
            animator.SetBool("run", false);
            if (transform.position.x - player.position.x > 0)
            {
                dodgedAttack();
                transform.eulerAngles = new Vector2(0, 180);
                rigidbody2D.linearVelocity = new Vector2(walkSpeed * (-1), 0);
            }
            if (transform.position.x - player.position.x < 0)
            {
                dodgedAttack();
                transform.eulerAngles = new Vector2(0, 0);
                rigidbody2D.linearVelocity = new Vector2(walkSpeed, 0);
            }
        }
        if (Vector3.Distance(transform.position, player.position) < 7 && Vector3.Distance(transform.position, player.position) > 3.5 && !isDeath && !isAttack && !isSkill2)
        {
            animator.SetBool("walk", false);
            animator.SetBool("run", true);
            if (transform.position.x - player.position.x > 0)
            {
                dodgedAttack();
                transform.eulerAngles = new Vector2(0, 180);
                rigidbody2D.linearVelocity = new Vector2(runSpeed * (-1), 0);
            }
            if (transform.position.x - player.position.x < 0)
            {
                dodgedAttack();
                transform.eulerAngles = new Vector2(0, 0);
                rigidbody2D.linearVelocity = new Vector2(runSpeed, 0);
            }
        }
        if (Vector3.Distance(transform.position, player.position) <= 3.5 && !isDeath && !isAttack && !isSkill2)
        {
            animator.SetBool("walk", true);
            animator.SetBool("run", false);
            if (transform.position.x - player.position.x > 0)
            {
                dodgedAttack();
                transform.eulerAngles = new Vector2(0, 180);
                rigidbody2D.linearVelocity = new Vector2(walkSpeed * (-1), 0);
            }
            if (transform.position.x - player.position.x < 0)
            {
                dodgedAttack();
                transform.eulerAngles = new Vector2(0, 0);
                rigidbody2D.linearVelocity = new Vector2(walkSpeed, 0);
            }
        }
        if (Vector3.Distance(transform.position, player.position) >= 10 && !isDeath)
        {
            animator.SetBool("walk", false);
            animator.SetBool("run", false);
        }

        hpSlider.value = hp;

        if (Time.time - timeSkill2 > Random.Range(11, 15) && !isDeath && !isAttack && !isDodged)
        {
            isInstantiated = false;
            isSkill2 = true;
            rigidbody2D.linearVelocity = new Vector2 (0, 0);
            animator.SetTrigger("skill_2");
            timeSkill2 = Time.time;
        }

        if (attack && Time.time - startTime > 5 && !isDeath && !isSkill2 && !isDodged)
        {
            isInstantiated = false;
            isAttack = true;
            rigidbody2D.linearVelocity = new Vector2(0, 0);
            animator.SetTrigger("skill_1");
            startTime = Time.time;
        }

        if (stateInfo.IsName("skill_1") && !isSkill2)
        {
            if (stateInfo.normalizedTime > 0.95f)
            {
                isAttack = false;
            }
            if (stateInfo.normalizedTime > 0.35f && !isInstantiated)
            {
                Instantiate(hitArea, transform.position + offset, transform.rotation);
                isInstantiated = true;
                
            }
        }
        if (stateInfo.IsName("skill_2") && !isAttack)
        {
            if (stateInfo.normalizedTime > 0.95f)
            {
                isSkill2 = false;
            }
            if (stateInfo.normalizedTime > 0.5f && !isInstantiated)
            {
                Instantiate(effectSkill2, effectPos, transform.rotation);
                Instantiate(areaSkill2, new Vector3(effectPos.x, effectPos.y + 1.55f, effectPos.z), transform.rotation);
                isInstantiated = true;
            }
        }
        if (isDeath)
        {
            if (stateInfo.IsName("death") && stateInfo.normalizedTime > 0.7f && !isInstantiated)
            {
                Instantiate(deathEffect, new Vector3(transform.position.x, transform.position.y + 2.5f, transform.position.z), transform.rotation);
                isInstantiated = true;                
            }
            if (stateInfo.IsName("death") && stateInfo.normalizedTime > 0.95f)
            {
                Destroy(this.gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D target)
    {
        if (target.gameObject.CompareTag("Attack") && isDodged)
        {
            hp -= Random.Range(2, 8);
        }
    }

    IEnumerator DelayDodged()
    {
        yield return new WaitForSeconds(2.75f);
        isDodged = false;
    }

    private void FixedUpdate()
    {
        Collider2D attackArea = Physics2D.OverlapBox(transform.position + footOffset, footArea, 0, playerLayer);
        attack = attackArea != null;
        Collider2D dodgedArea = Physics2D.OverlapCircle(transform.position + dodgedOffset, dodgedRaduis, playerHit);
        dodged = dodgedArea != null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position + footOffset, footArea);
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position + dodgedOffset, dodgedRaduis);
    }

    private void dodgedAttack()
    {
        if (!isAttack && !isSkill2 && !(stateInfo.IsName("Skill_1") || stateInfo.IsName("Skill_2")))
        {
            if (dodged && !isDodged && Time.time - timeEvade > 0.8)
            {
                // Debug.Log("doged Attack");
                if (transform.position.x - player.position.x > 0)
                {
                    transform.eulerAngles = new Vector2(0, 0);
                    rigidbody2D.AddForce(new Vector2(forceDodge, 0), ForceMode2D.Impulse);
                    rigidbody2D.linearVelocity = new Vector2(forceDodge, 0);
                }
                if (transform.position.x - player.position.x < 0)
                {
                    transform.eulerAngles = new Vector2(0, 180);
                    rigidbody2D.AddForce(new Vector2(forceDodge * -1, 0), ForceMode2D.Impulse);
                    rigidbody2D.linearVelocity = new Vector2(forceDodge * -1, 0);
                }
                isDodged = true;
                animator.SetTrigger("evade_1");
                timeEvade = Time.time;
                StartCoroutine(DelayDodged());
            }
        }
    }
}
