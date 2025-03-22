using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StoneMonster : MonoBehaviour
{
    [Header("�������͹������Ф�")]
    [SerializeField] private float walkSpeed;

    [Header("��õ�駤�����բͧ StoneMonster")]
    [SerializeField] private Vector3 attackOffset; 
    [SerializeField] private Vector2 attackSize; // ���ҧ�ͺࢵ��Ǩ�ͺ �������ⴹ������
    [SerializeField] private float attackForce; // �ç�������㹡�����բͧ skill 1, skill 2
    [SerializeField] private GameObject attackArea; // �ͺࢵ������բͧ skill 1 
    [SerializeField] private GameObject attack2_Area; // �ͺࢵ������� skill 2
    [SerializeField] private GameObject crytals_Skill3; // Crytals �ͧ skill 3

    [Header("���ҧᶺ���ʹ")]
    [SerializeField] private float maxHealth = 150;
    [SerializeField] private Image fillHpBar;
    [SerializeField] private Transform hpBarTransform;
    [SerializeField] private float currentHealth;

    [Header("���§")]
    [SerializeField] private AudioClip punchSound;
    [SerializeField] private AudioClip crystalSound;

    // ���������Ѻ Check ʶҹе�ҧ �
    [SerializeField] private LayerMask playerLayer;
    private float distance;
    private float direction;
    private bool isAttack = false; 
    private bool attacking;
    private bool isDeath = false;
    private float startTime;

    // ���������Ѻ�֧ Component
    private Rigidbody2D rigidbody2D;
    private Animator animator;
    private AudioSource audioSource;
    private AnimatorStateInfo stateInfo;

    [Header("�֧�����Ţͧ Vaja")]
    [SerializeField] private Transform player;
    public PlayerController playerDetials;
    public int playerHp;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        GameObject playerObj = GameObject.FindWithTag("Player");
        GameObject sliderObj = GameObject.FindWithTag("BarBoss");
        if (playerObj != null )
        {
            player = playerObj.GetComponent<Transform>();
            playerDetials = playerObj.GetComponent<PlayerController>();
        }

        playerHp = playerDetials.GetHealth();
        currentHealth = maxHealth;
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        hpBarTransform.rotation = Quaternion.identity;
        distance = Vector3.Distance(transform.position, player.position);
        direction = transform.position.x - player.position.x;
        stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        playerHp = playerDetials.GetHealth();
        animator.SetInteger("Attack", 0);

        // StoneMonster ���
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            isDeath = true;
            animator.SetTrigger("isDeath");
            rigidbody2D.linearVelocity = new Vector2(0, 0);
            StartCoroutine(WaitForNextLevel());
            currentHealth += 0.1f;
        }
        if (playerHp <= 0)
        {
            rigidbody2D.linearVelocity = new Vector2(0, 0);
            animator.SetBool("Standing", true);
            animator.SetBool("Walk", false);
        }

        if (!isAttack && !stateInfo.IsName("StoneMonsterAttack1") && !stateInfo.IsName("StoneMonsterAttack2") && !stateInfo.IsName("StoneMonsterAttack3") && !isDeath && playerHp > 0) { movement(); }
        if (attacking && Time.time - startTime > 5.5f && !isDeath && playerHp > 0) { isAttack = true; attacked(); } // ���շء � 5.5 �Թҷ� ����� Player �����㹢ͺࢵ

        fillHpBar.fillAmount = currentHealth / maxHealth;
    }

    private void FixedUpdate()
    {
        // ���ҧ�ͺࢵ������������� player �����ⴹ
        Collider2D attack = Physics2D.OverlapBox(transform.position + attackOffset, attackSize, 0, playerLayer);
        attacking = attack != null;
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + attackOffset, attackSize);
    }

    private void movement()
    {
        if (distance <= 27.5)
        {
            animator.SetBool("Standing", true);
        }
        if (distance <= 25)
        {
            animator.SetBool("Walk", true);
            if (direction > 5.5f)
            {
                transform.eulerAngles = new Vector2(0, 0);
                rigidbody2D.linearVelocity = new Vector2(walkSpeed * -1, rigidbody2D.linearVelocityY);
            }
            else if (direction < -5.5f)
            {
                transform.eulerAngles = new Vector2(0, 180);
                rigidbody2D.linearVelocity = new Vector2(walkSpeed, rigidbody2D.linearVelocityY);
            }
        }
        if (distance > 25)
        {
            animator.SetBool("Walk", false);
        }
    }

    private void attacked()
    {
        int skillNumber = Random.Range(1, 4);
        // int skillNumber = 2;
        if (isAttack && skillNumber == 1)
        {
            animator.SetBool("Walk", false);
            animator.SetInteger("Attack", 1);
            isAttack = false;
            Instantiate(attackArea, new Vector3(transform.position.x, transform.position.y + 1.65f, transform.position.z), transform.rotation);
            if (stateInfo.normalizedTime > 0.45f)
            {
                addForceAtk(0.8f);
            }
        }
        else if (isAttack && skillNumber == 2)
        {
            animator.SetBool("Walk", false);
            animator.SetInteger("Attack", 2);
            Instantiate(attack2_Area, new Vector3(transform.position.x, transform.position.y + 1.15f, transform.position.z), transform.rotation);
            addForceAtk(0.75f);
            isAttack = false;
        }
        else if (isAttack && skillNumber == 3)
        {
            audioSource.PlayOneShot(punchSound);
            audioSource.PlayOneShot(crystalSound);
            animator.SetBool("Walk", false);
            animator.SetInteger("Attack", 3);
            isAttack = false;
            rigidbody2D.linearVelocity = new Vector2(0, 0);
        }
        startTime = Time.time;
    }

    private void addForceAtk(float multipleForce) // method ��� StoneMonster �������� (multipleForce �����������ҡ������ç���˹ �������¡��Ѻ skill 1 �Ѻ skill 2)
    {
        if (direction > 0)
        {
            rigidbody2D.AddForceX(attackForce * -1 * multipleForce, ForceMode2D.Impulse);
        }
        if (direction < 0)
        {
            rigidbody2D.AddForceX(attackForce * multipleForce, ForceMode2D.Impulse);
        }
    }

    private void SpanwCrytal() // ���ҧ Crytals �ͧ skill 3
    {
        if (direction > 0) { Instantiate(crytals_Skill3, new Vector3(transform.position.x + 1.1f, transform.position.y + 6.5f, transform.position.z), transform.rotation); }
        if (direction < 0) { Instantiate(crytals_Skill3, new Vector3(transform.position.x - 1.1f, transform.position.y + 6.5f, transform.position.z), transform.rotation); }
    }
    IEnumerator WaitForNextLevel()
    {
        yield return new WaitForSeconds(1.5f);
        SceneController.instance.NextLevel();
    }
    private void OnTriggerEnter2D(Collider2D target) // ��Ǩ�ͺ����Ͷ١����
    {
        if (target.gameObject.CompareTag("Attack"))
        {
            currentHealth -= Random.Range(4, 10);
            Destroy(target.gameObject);
        }
        if (target.gameObject.CompareTag("SlamDunk"))
        {
            currentHealth -= Random.Range(12, 16);
            Destroy(target.gameObject);
        }
        if (target.gameObject.CompareTag("Skill"))
        {
            currentHealth -= Random.Range(4, 10);
        }
        if (target.gameObject.CompareTag("LightSkill"))
        {
            currentHealth -= Random.Range(9, 19);
        }
        if (target.gameObject.CompareTag("LightExplode"))
        {
            currentHealth -= Random.Range(9, 16);
        }
    }
}
