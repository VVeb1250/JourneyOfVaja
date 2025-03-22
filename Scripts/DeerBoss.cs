using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DeerBoss : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform centerCamera;
    [SerializeField] private float moveSpeed;
    [SerializeField] private GameObject crystalFormGround;
    [SerializeField] private GameObject crystalExplode;
    [SerializeField] private GameObject lightingCrystal;
    [SerializeField] private Vector3 areaOffset;
    [SerializeField] private Vector3 areaSize;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float attackCooldown;
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private float health = 300;
    public Slider hpBar;
    public GameObject hpObj;
    private float attackRate;
    private float direction;
    private bool isAttack = false;
    private bool attacking;
    private bool isDeath = false;
    private Rigidbody2D rigidbody2D;
    private Animator animator;
    private AudioSource audioSource;
    private PlayerController playerDetials;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidbody2D = this.gameObject.GetComponent<Rigidbody2D>();
        animator = this.gameObject.GetComponent<Animator>();
        audioSource = this.gameObject.GetComponent<AudioSource>();
        playerDetials = FindAnyObjectByType<PlayerController>();
        // hpObj = GameObject.Find("BossHpBar");
        if (hpObj != null)
        {
            hpBar = hpObj.GetComponent<Slider>();
        }

        hpBar.maxValue = health;
        hpBar.value = health;

        // GameObject playerObj = GameObject.FindWithTag("Player");
        // if (playerObj !=  null ) { playerTransform = playerObj.GetComponent<Transform>(); }
    }

    // Update is called once per frame
    void Update()
    {
        // if (playerDetials.havedBossBar)
        // {
            // if (hpObj != null) { hpBar = hpObj.GetComponent<Slider>(); }
        // }

        direction = transform.position.x - playerTransform.position.x;
        int playerHp = playerDetials.GetHealth();
        animator.SetInteger("Attack", 0);
        // Debug.Log("pos x = " + playerTransform.position.x + " pos y = " + playerTransform.position.y + " pos z = " + playerTransform.position.z);

        if (health <= 0)
        {
            health = 0;
            isDeath = true;
            animator.SetTrigger("Death");
            rigidbody2D.linearVelocity = new Vector2(0, 0);
            health += 0.1f;
        }
        if (playerHp <= 0)
        {
            rigidbody2D.linearVelocity = new Vector2(0, 0);
            animator.SetBool("Walk", false);
            animator.SetInteger("Attack", 0);
        }

        // CheckAttacked();
        if (!isAttack && !isDeath)
        {
            Movement();
        }
        if (attacking && Time.time > attackRate && !isDeath)
        {
            Attack();
        }

        hpBar.value = health;
    }

    private void Movement()
    {
        if (Vector3.Distance(transform.position, playerTransform.position) >= 25)
        {
            animator.SetBool("Walk", false);
            rigidbody2D.linearVelocity = new Vector2(0, rigidbody2D.linearVelocityY);
        }
        else if (Vector3.Distance(transform.position, playerTransform.position) < 25)
        {
            animator.SetBool("Walk", true);
            rigidbody2D.linearVelocity = new Vector2(moveSpeed * GetLeftRight(), rigidbody2D.linearVelocityY);
            if (direction > 2) { transform.eulerAngles = new Vector2(0, 0); }
            if (direction < -2) { transform.eulerAngles = new Vector2(0, 180); }
        }
    }

    private void FixedUpdate()
    {
        Collider2D attackArea = Physics2D.OverlapBox(transform.position + areaOffset, areaSize, 0, playerLayer);
        attacking = attackArea != null;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + areaOffset, areaSize);
    }

    private void Attack()
    {
        int skillNum = Random.Range(1, 4);
        // audioSource.PlayOneShot(attackSound);
        // int skillNum = 3;
        if (skillNum == 1)
        {
            // Debug.Log("pos x = " + playerTransform.position.x + " pos y = " + playerTransform.position.y + " pos z = " + playerTransform.position.z);
            isAttack = true;
            animator.SetInteger("Attack", 1);
            animator.SetBool("Walk", false);
            rigidbody2D.linearVelocity = new Vector2(0, rigidbody2D.linearVelocity.y);
            // Instantiate(crystal, new Vector3(centerCamera.position.x + 7.2f, centerCamera.position.y - 3f, centerCamera.position.z), Quaternion.identity);
            for (int j = 0; j < 3; j++)
            {
                int spawnOffset = Random.Range(-18, 19);
                Instantiate(crystalFormGround, new Vector3(playerTransform.position.x + spawnOffset, playerTransform.position.y + 2.28f, playerTransform.position.z), Quaternion.identity);
            }
            attackRate = Time.time + attackCooldown;
            // StartCoroutine(SpawnCrystal());
        }
        else if (skillNum == 2)
        {
            audioSource.PlayOneShot(attackSound);
            isAttack = true;
            animator.SetInteger("Attack", 2);
            animator.SetBool("Walk", false);
            rigidbody2D.linearVelocity = new Vector2(0, rigidbody2D.linearVelocity.y);
            Instantiate(crystalExplode, new Vector3(centerCamera.position.x, centerCamera.position.y, 0), Quaternion.identity);
            attackRate = Time.time + attackCooldown + 5;
        }
        else if (skillNum == 3)
        {
            audioSource.PlayOneShot(attackSound);
            isAttack = true;
            animator.SetInteger("Attack", 3);
            animator.SetBool("Walk", false);
            rigidbody2D.linearVelocity = new Vector2(0, rigidbody2D.linearVelocity.y);
            Instantiate(lightingCrystal, new Vector3(playerTransform.position.x, playerTransform.position.y + 4.8f, playerTransform.position.z), Quaternion.identity);
            attackRate = Time.time + attackCooldown + 7;
        }
    }
    private void CheckAttacked()
    {
        // AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        // if (stateInfo.IsName("deerroar"))
        // {
        // if (stateInfo.normalizedTime >= 0.95) { isAttack = false; }
        // }
        // else if (stateInfo.IsName("deerroar_2"))
        // {
        // if (stateInfo.normalizedTime >= 0.95) { isAttack = false; }
        // }
        // else if (stateInfo.IsName("deerroar_3"))
        // {
        // if (stateInfo.normalizedTime >= 0.95) { isAttack = false; }
        // }
        isAttack = false;
    }
    IEnumerator SpawnCrystal()
    {
        // for (int i = 0; i < 5; i++)
        // {
            // int spawnOffset = Random.Range(-9, 8);
            // Debug.Log("pos x = " + playerTransform.position.x + " pos y = " + playerTransform.position.y + " pos z = " + playerTransform.position.z);
            // Debug.Log(playerTransform.position);
            // for (int j = 0; j < 3; j++)
            // {
                // int spawnOffset = Random.Range(-9, 8);
                // Instantiate(crystal, new Vector3(playerTransform.position.x + spawnOffset, -5.95f, playerTransform.position.z), Quaternion.identity);
            // }
            // if (i >= 4) { isAttack = false; }
            yield return new WaitForSeconds(1.65f);
        // }
    }

    private int GetLeftRight()
    {
        if (direction > 0) { return -1; }
        return 1;
    }

    private void OnTriggerEnter2D(Collider2D target) // µÃÇ¨ÊÍºàÁ×èÍ¶Ù¡â¨Á¤Õ
    {
        if (target.gameObject.CompareTag("Attack"))
        {
            health -= Random.Range(4, 10);
            Destroy(target.gameObject);
        }
        if (target.gameObject.CompareTag("SlamDunk"))
        {
            health -= Random.Range(12, 16);
            Destroy(target.gameObject);
        }
        if (target.gameObject.CompareTag("Skill"))
        {
            health -= Random.Range(4, 10);
        }
        if (target.gameObject.CompareTag("LightSkill"))
        {
            health -= Random.Range(9, 19);
        }
        if (target.gameObject.CompareTag("LightExplode"))
        {
            health -= Random.Range(9, 16);
        }
    }
}
