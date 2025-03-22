using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CoconutMonster : MonoBehaviour
{
    [Header("ตั้งค่าตัวละคร")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float health;

    [Header("Component ของ Player")]
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask playerLayer;
    public PlayerController playerDetials;
    public int playerHp;

    [Header("ตั้งค่าขอบเขตการโจมตี")]
    [SerializeField] private float attackArea;
    [SerializeField] private Vector3 attackOffset;

    [Header("วัตถุที่จะให้ยิงออกไป")]
    [SerializeField] private GameObject shootObject;

    // ตัวแปรสำหรับโจมตี
    private bool attacking;
    private bool isAttack = false;
    private bool isDeath = false;
    private float direction;

    [Header("ตั้งค่า Skill")]
    [SerializeField] private float cooldownSkill;
    private float skillRate;

    // Component ตัวละคร
    // private Slider healthBar;
    private Rigidbody2D rigidbody2D;
    [SerializeField] private Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = this.gameObject.GetComponent<Animator>();
        rigidbody2D = this.gameObject.GetComponent<Rigidbody2D>();
        // GameObject sliderObj = GameObject.FindWithTag("BarBoss");
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            // player = playerObj.GetComponent<Transform>();
            // playerDetials = playerObj.GetComponent<PlayerController>();
        }
        // if (sliderObj != null ) { healthBar =  sliderObj.GetComponent<Slider>(); }

        playerHp = playerDetials.GetHealth();
        // healthBar.maxValue = health;
        // healthBar.value = health;
    }

    // Update is called once per frame
    void Update()
    {
        direction = transform.position.x - player.position.x; // ตรวจว่าตัวละครอยู่ฝั่งไหน
        playerHp = playerDetials.GetHealth();

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
        }

        if (!isAttack && !isDeath) { transform.Rotate(new Vector3(0, 0, 0), Space.Self); } // หมุนกลับเมื่อไม่ได้โจมตี
        if (Vector3.Distance(transform.position, player.position) >= 27.5)
        {
            rigidbody2D.linearVelocity = new Vector2(0, 0);
        }

        if (Vector3.Distance(transform.position, player.position) < 27.5 && !isAttack && playerHp > 0 && !isDeath) { Movement(); }

        if (attacking && Time.time > skillRate && playerHp > 0 && !isDeath)
        {
            Attack();
            skillRate = Time.time + cooldownSkill;
        }

        // healthBar.value = health;

        // CheckAttacked();
    }

    private void FixedUpdate()
    {
        // สร้างขอบเขตสำหรับตรวจาสอบว่าจะโจมตี
        Collider2D attack = Physics2D.OverlapCircle(transform.position + attackOffset, attackArea, playerLayer);
        attacking = attack != null;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + attackOffset, attackArea);
    }
    private void Movement()
    {
        if (direction > 4.5)
        {
            transform.eulerAngles = new Vector2(0, 0);
            rigidbody2D.linearVelocity = new Vector2(moveSpeed * GetLeftRigth(), rigidbody2D.linearVelocityY);
        }
        else if (direction < -4.5)
        {
            transform.eulerAngles = new Vector2(0, 180);
            rigidbody2D.linearVelocity = new Vector2(moveSpeed * GetLeftRigth(), rigidbody2D.linearVelocityY);
        }
    }
    private int GetLeftRigth()
    {
        if (direction > 0) { return -1; }
        return 1;
    }
    private void Attack()
    {
        // Debug.Log("Attack");
        int numAttack = 1;
        isAttack = true;
        transform.Rotate(new Vector3(0, 0, 13.75f), Space.Self);
        rigidbody2D.linearVelocity = new Vector2(0, 0);
        if (numAttack == 1) { StartCoroutine(MonsterShoot()); }
        
        
    }
    private void CheckAttacked()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Attack"))
        {
            if (stateInfo.normalizedTime > 0.95) { isAttack = false; }
        }
    }
    private void Shooted() // ยิง shootObject เรียกใช้จาก animation
    {
        if (direction > 1.75)
        {
            transform.eulerAngles = new Vector2(0, 0);
            Instantiate(shootObject, new Vector3(transform.position.x - 1.2f, transform.position.y + 1.75f, transform.position.z), Quaternion.Euler(0, 180, -14.5f));
        }
        else if (direction < -1.75)
        {
            transform.eulerAngles = new Vector2(0, 180);
            Instantiate(shootObject, new Vector3(transform.position.x + 1.2f, transform.position.y + 1.75f, transform.position.z), Quaternion.Euler(0, 0, -14.5f));
        }
        else
        {
            transform.eulerAngles = new Vector2(0, 180);
            Instantiate(shootObject, new Vector3(transform.position.x + 1.2f, transform.position.y + 1.75f, transform.position.z), Quaternion.Euler(0, 0, -45f));
        }
    }
    IEnumerator MonsterShoot() // ยิง shootObject ออกไปโดยยิง 1 ครั้งและ delay 1.15 s
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        for (int i = 0; i < 5; i++)
        {
            if (playerHp <= 0) { break; }
            // if (direction > 1.75) { transform.eulerAngles = new Vector2(0, 0); }
            // else if (direction < -1.75) { transform.eulerAngles = new Vector2(0, 180); }
            animator.SetTrigger("Attack");
            if (i >= 4) { isAttack = false; }
            yield return new WaitForSeconds(1.15f);
        }
    }
    private void OnTriggerEnter2D(Collider2D target) // ตรวจสอบเมื่อถูกโจมคี
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
