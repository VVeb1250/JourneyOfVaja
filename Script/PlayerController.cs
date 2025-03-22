using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    // import คลาสที่จำเป็น
    private UIManager uiManager;
    // ตั้งค่าตัวละครต่างๆ (การเคลื่อนที่)
    [Header("ตั้งค่าตัวละครต่างๆ (การเคลื่อนที่)")]
    [SerializeField] private float speed = 15f; // ความเร็วที่ใช้วิ่ง
    [SerializeField] private float jumpSpeed = 20f; // ความเร็วเมื่อกระโดด
    [SerializeField] private float maxSpeed = 50f; // ความเร็วสูงสุดเมื่อมีสิ่งอื่นมากระทำด้วย
    [SerializeField] private float jumpPower = 75; // ความแรงในการกระโดด
    [SerializeField] private float knockbackForce; // แรงกระเด็นเมื่อถูกโจมตี
    private bool left, right;

    // ตั้งค่าตัวละครต่างๆ (ความสามารถ)
    [Header("ตั้งค่าตัวละครต่างๆ (ความสามารถ)")]
    [SerializeField] private float attackRate = 0.1f; // ความเร็วโจมตี
    private float nextattackRate = 0.0f; // อัตราการโจมตีในครั้งต่อไป
    [SerializeField] private float dodgeCooldown = 0.25f; // อัตราการกระโดดหลบ
    private float nextDodge = 0f; // อัตราการกระโดดหลบ
    [SerializeField] private int healt = 100; // เลือดตัวละคร
    [SerializeField] private GameObject attackArea; // ขอบเขตการโจมตี
    [SerializeField] private GameObject slamDunkAttackArea; // ขอบเขตการโจมตี Slamdunk
    [SerializeField] private GameObject slamDunkEffect; // effect ตอน slamDunk ถึงพื้น
    [SerializeField] private GameObject ballSkill; // skill ตัวละคร
    [SerializeField] private GameObject lightSkill;
    [SerializeField] private GameObject lightExplodeSkill;
    [SerializeField] private float cooldownSkill;
    private float skillRate;

    // การโจมตีของตัวละคร
    [Header("การโจมตีต่างๆ(คอมโบ)")]
    [SerializeField] private float comboTimeWindow = 0.5f; // เวลาที่รอให้กดปุ่มโจมตีครั้งต่อไป
    [SerializeField] private float airAttackAllow = 5f;
    [SerializeField] private float slamDrunkAllow = 10f;
    private int comboCounter = 0;
    private float lastAttackTime = 0f;
    private bool isAttackingAnimation = false;
    private bool isAttacking = false;
    private bool isSlamDrunkAttack = false;
    private bool isAirAttack = false;
    private bool havedAirAttack = false;
    private float slashOffset_X ,slashOffset_Y, slashRotate_Y, slashRotate_Z;
    
    // บูลลีนเช็คการเคลื่อนไหวตัวละคร
    private readonly float jumpRate = 0.1f; // เช็คการกระโดด
    private float nextJump = 0.0f; // เช็คการกระโดด
    private bool isDodge = false; // เช็คการหลบ
    private bool isDoubleJump = true; // เช็คการกระโดดเบิ้ล
    private bool isKnockback = false; // เช็คการถูกกระเด็น

    // เงื่อนไขเพิ่มเติม (เช็คการอยู่บนพื้น)
    [Header("กำหนดพื้นที่เช็คการอยู่บนพื้น")]
    [SerializeField] private Vector3 footOffset = new(-0.06f, 0, 0);
    [SerializeField] private Vector2 footArea = new(1.4f, 0.35f);
    [SerializeField] private LayerMask groundLayerMask; // เลเยอร์พื้น
    private bool isGrounded; // ตรวจสอบว่าอยู่บนพื้น

    // ทำ idle animation
    private int randomIdle;
    public float idleTimeMin = 5f; // เวลาน้อยสุดที่ idle จะเกิด
    public float idleTimeMax = 10f; // เวลามากสุดที่ idle จะเกิด
    private float idleTime; // เวลาในการสุ่ม
    private bool isMoving = false; // เช็คว่าไม่เคลื่อนที่เพื่อทำ idle animation
    // ทำ grab animation
    [Header("ทำ grab animation")]
    public Transform ledgeCheck;
    public Transform wallCheck;
    private Vector2 placeGrabing;
    [SerializeField] private float wallCheckDistance = 1.5f;
    private bool isTouchingWall, isTouchingLedge, isGrabing;

    // Hp Bar
    [Header("Hp Bar")]
    [SerializeField] private Slider hpBar;
    [SerializeField] private GameObject healEffect;
    [SerializeField] private Text potionUI;
    [SerializeField] private GameObject bossBar;
    public bool havedBossBar = false;
    private int numPotion = 3;

    // ตรวจตำแหน่งตัวละคร เทียบกับ Enemy
    [Header("Enemy position")]
    [SerializeField] private Transform[] enemiesTransform;
    [SerializeField] private Transform enemyTransform; // ตำแหน่ง Enemy ที่ใกล้ที่สุด (หลายตัว)
    private Transform nearestEnemy;
    private float playerPos;
    private float enemyPos;

    // ทำฟังก์ชั่นเสริม
    private Rigidbody2D rigidBody2D; 
    private readonly Physics2D physics2D;
    private CoconutAttack coconutAtk;
    private UIManager uIManager;
    private DeerBoss bossDetials;
    Animator animator;

    // ตรวจสอบสถานะตัวละคร
    private bool isDeath = false;
    private bool isSkill = false;
    private bool isHealed = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidBody2D = this.gameObject.GetComponent<Rigidbody2D>();
        animator = this.gameObject.GetComponent<Animator>();
        uIManager = FindAnyObjectByType<UIManager>();
        bossDetials = FindAnyObjectByType<DeerBoss>();
        GameObject sliderObj = GameObject.FindWithTag("Slider");
        if (sliderObj != null ) { hpBar = sliderObj.GetComponent<Slider>(); }

        animator.SetBool("isDead", false);
        hpBar.maxValue = healt;
        hpBar.value = healt;
        SetRandomIdleTime();
    }

    // Update is called once per frame
    void Update()
    {
        if (uIManager != null && uIManager.isResting)
        { // ถ้ากำลังพักผ่อนอยู่ ให้หยุดการเคลื่อนที่ทั้งหมด
            animator.SetFloat("speed", 0);
            animator.SetBool("attack", false);
            checkAndDoIdle();
            return;
        }
        if (uIManager != null && uIManager.isPausing) { return; }

        // ส่วนอัพเดทค่าต่างๆ
        animator.SetBool("isGround", isGrounded);
        animator.SetFloat("Y", rigidBody2D.linearVelocity.y);
        NearestEnemySelected();

        isAttackingAnimation = animator.GetCurrentAnimatorStateInfo(0).IsName("Vaja_attack1") ||
                               animator.GetCurrentAnimatorStateInfo(0).IsName("Vaja_attack2") ||
                               animator.GetCurrentAnimatorStateInfo(0).IsName("Vaja_attack3");

        if (isGrounded) { havedAirAttack = false; }
        if (isHealed) { StartCoroutine(resetHealed()); }

        // ถ้ากำลังจับผนัง, ดอดจ์, หยุดการเคลื่อนไหวทั้งหมด
        if (!isGrabing && !isDodge && !isAttacking && !isSlamDrunkAttack && !isAirAttack && !isKnockback && !isDeath && !isSkill) {
            if (!isAttackingAnimation){
                Movement();
                Jumping();
            }

            Dodge();
            Skill();
            Healing();

            if (isGrounded && !(comboCounter == 3)) { Attack(); }
            // is not ground condition
            else { SlamDrunkAttack();} // ให้ความสำคัญกับ SlamDrunkAttack ก่อน
            if (!isAttacking && !isSlamDrunkAttack && !isGrounded) { AirAttack(); }
        }
        if (isDodge) { CheckResetDodge(); }
        if (isAttacking && CheckHalfAttacking()) { EndAttack(); }
        if (Time.time - lastAttackTime > comboTimeWindow + attackRate && !isAttacking) { ResetCombo(); } // รีเซ็ต Combo ถ้าไม่กดปุ่มภายในเวลาที่กำหนด
        if (isAirAttack) { CheckResetAirAttack(); }
        if (isSlamDrunkAttack) { CheckResetSlamDrunkAttack(); }
        if (isGrabing) { GrabingLedge(); } // อ่านจาก FixedUpdate()
        if ( !isMoving && CheckIdle() ) { animator.SetInteger("IdleState", 0); };
        if (isKnockback) { checkKnockBack(); } // เช็ค KnockBack
        if (isSkill) { CheckSkilled(); }

        if (healt <= 0) { Death(); }

        hpBar.value = healt;
    }

    // การอัพเดทฟิสิกส์ด้วย FixedUpdate()
    private void FixedUpdate() {
        // ดึงตำแหน่งของ Player, Enemy
        playerPos = transform.position.x;
        enemyPos = enemyTransform.position.x;
        // อัพเดทการแตะพื้น
        Collider2D hitCollider = Physics2D.OverlapBox(transform.position + footOffset, footArea, 0, groundLayerMask);
        isGrounded = hitCollider != null;
        // อัพเดทการ grab ผนัง
        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, groundLayerMask);
        isTouchingLedge = Physics2D.Raycast(ledgeCheck.position, transform.right, wallCheckDistance, groundLayerMask);
        // ทำแอคชั่น grab
        if (isTouchingWall && !isTouchingLedge && !isGrabing && !isGrounded && !isDodge && !isSlamDrunkAttack) {
            isGrabing = true;
            animator.SetBool("isGrabing", true);
            placeGrabing = transform.position;
        }
    }

    // การทำให้เห็นตัวขอบ
    private void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position + footOffset, footArea);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(ledgeCheck.position, new Vector3(ledgeCheck.position.x + wallCheckDistance, ledgeCheck.position.y, ledgeCheck.position.z));
        Gizmos.DrawLine(ledgeCheck.position, new Vector3(ledgeCheck.position.x - wallCheckDistance, ledgeCheck.position.y, ledgeCheck.position.z));
        Gizmos.color = Color.green;
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y, wallCheck.position.z));
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x - wallCheckDistance, wallCheck.position.y, wallCheck.position.z));
    }
    private void checkAndDoIdle() {
        // ตัวละครหยุด
        if (isMoving)
        {
            isMoving = false;
            SetRandomIdleTime(); // สุ่มเวลา idle ใหม่
        }
        // ลดเวลา idle ลง
        idleTime -= Time.deltaTime;
        if (idleTime <= 0)
        {
            // เมื่อถึงเวลา idle ที่สุ่มได้แล้ว ให้เปลี่ยน animation idle
            RandomIdleAnimation();
            SetRandomIdleTime(); // สุ่มเวลาต่อไป
        }
    }

    // ฟังก์ชั่น Handmade
    private void Movement() {
        animator.SetFloat("speed", Mathf.Abs(Input.GetAxis("Horizontal")));
        // การเคลื่อนที่ของตัวละคร
        if (Input.GetAxis("Horizontal") < -0.1f) {
            // ตัวละครเคลื่อนที่ไปทางซ้าย
            isMoving = true;
            left = true;
            right = false;
            transform.Translate(Vector2.right * (speed * Time.deltaTime));
            transform.eulerAngles = new Vector2(0, 180);
        } else if (Input.GetAxis("Horizontal") > 0.1f) {
            // ตัวละครเคลื่อนที่ไปทางขวา
            isMoving = true;
            left = false;
            right = true;
            transform.Translate(Vector2.right * (speed * Time.deltaTime));
            transform.eulerAngles = new Vector2(0, 0);
        } else {
            checkAndDoIdle();
        }
    }
    private void Jumping() {
        // ทำแอคชั่นกระโดด
        if ((Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.X)) && (isGrounded || isDoubleJump) && Time.time >= nextJump) {
            if (isGrounded) { isDoubleJump = true; } 
            else { isDoubleJump = false; }
            nextJump = Time.time + jumpRate;
            rigidBody2D.AddForce(jumpSpeed * (Vector2.up * jumpPower)); // กระโดด
        }
    }
    private void GrabingLedge() {
        transform.position = placeGrabing;
        rigidBody2D.linearVelocity = new Vector2(0, 0);
        // ตรวจสอบว่าแอนิเมชั่นที่กำลังเล่นเสร็จแล้วหรือไม่
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Vaja grabing") && stateInfo.normalizedTime >= 1.0f) {
            // แอนิเมชั่นเล่นเสร็จแล้ว
            animator.SetBool("isGrabing", false);
            isGrabing = false;
            transform.position = new Vector2(transform.position.x + (2f * GetLeftRight()), transform.position.y + 3f);
        }
    }
    private void Dodge() {
        if ((Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.C)) && Time.time >= nextDodge && !Input.GetMouseButton(0)) {
            isDodge = true;
            animator.SetBool("dodge", true);
            ResetCombo(); // reset combo when dodge
            rigidBody2D.linearVelocity = new Vector2(0, 0.5f);
            rigidBody2D.linearVelocity = new Vector2(jumpPower * GetLeftRight() / 2.75f, jumpPower / 10f);
        }
    }
    private void CheckResetDodge() {
        // ตรวจสอบว่าแอนิเมชั่นที่กำลังเล่นเสร็จแล้วหรือไม่
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Vaja_dodge") && stateInfo.normalizedTime >= 1.0f ) { ResetDodge(); } 
        else if (stateInfo.IsName("Vaja_airdodge") && isGrounded) { ResetDodge(); }
    }
    private void ResetDodge() {
        // แอนิเมชั่นเล่นเสร็จแล้ว
        isDodge = false;
        animator.SetBool("dodge", false);
        // ตั้งเวลาคูลดาวน์ (เมื่ออนิเมชั่นเสร็จแล้ว)
        nextDodge = Time.time + dodgeCooldown;
    }
    private void Attack() {
        // ตรวจสอบการกดปุ่มโจมตี
        if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Z)) && Time.time >= nextattackRate && !Input.GetKey(KeyCode.LeftShift)) {
            if (Time.time - lastAttackTime <= comboTimeWindow + attackRate) {
                comboCounter++;
            } else {
                comboCounter = 1;
            }

            lastAttackTime = Time.time;
            nextattackRate = Time.time + attackRate;
            isAttacking = true;
            animator.SetBool("attack", true);

            // เล่น Animation ตาม Combo
            animator.SetInteger("Combo", comboCounter);
            if (comboCounter == 1) {
                slashOffset_X = 3.5f; slashOffset_Y = 2.95f;
                slashRotate_Y = 0; slashRotate_Z = 0;
                rigidBody2D.AddForce(speed * (GetLeftRight() *(Vector2.right * (jumpPower / 2.5f))));
                GenAttackArea(3.75f, 1.75f, attackArea);
            } if (comboCounter == 2) {
                slashOffset_X = 3.5f; slashOffset_Y = 2.95f;
                slashRotate_Y = 180; slashRotate_Z = 45;
                rigidBody2D.AddForce(speed * (GetLeftRight() *(Vector2.right * (jumpPower / 5))));
                GenAttackArea(2.75f, 1.75f, attackArea);
            } if (comboCounter == 3) {
                rigidBody2D.AddForce(speed * (GetLeftRight() *(Vector2.right * (jumpPower / 1))));
                GenAttackArea(3.75f, 1.75f, attackArea);
            }
        }
    }
    private bool CheckHalfAttacking() {
        // ตรวจสอบว่าแอนิเมชั่นที่กำลังเล่นเสร็จแล้วหรือไม่
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (animator.IsInTransition(0)) { return false; }
        if (comboCounter == 1 && stateInfo.IsName("Vaja_attack1") && stateInfo.normalizedTime >= 0.7f ) { return true; } 
        if (comboCounter == 2 && stateInfo.IsName("Vaja_attack2") && stateInfo.normalizedTime >= 0.7f ) { return true; } 
        if (comboCounter == 3 && stateInfo.IsName("Vaja_attack3") && stateInfo.normalizedTime >= 0.7f ) { return true; }
        return false;
    }
    private void EndAttack() {
        // ฟังก์ชันที่เรียกจาก Animation Event เมื่อจบ Animation โจมตี
        isAttacking = false;
        animator.SetBool("attack", false);
        lastAttackTime = Time.time;
        nextattackRate = Time.time + attackRate;
        if (comboCounter >= 3) { ResetCombo(); }  // สมมติว่า Combo สูงสุดคือ 3 ครั้ง
    }
    private void ResetCombo() {
        isAttacking = false;
        animator.SetBool("attack", false);
        comboCounter = 0;
        animator.SetInteger("Combo", 0);
    }
    private void AirAttack() {
        if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Z)) && !havedAirAttack && Mathf.Abs(rigidBody2D.linearVelocity.y) >= airAttackAllow) {
            placeGrabing = transform.position;
            isAirAttack = true;
            havedAirAttack = true;
            animator.SetBool("isAirAttack", true);
            GenAttackArea(2.75f, 1.75f, attackArea);
            transform.position = placeGrabing;
            rigidBody2D.linearVelocity = new Vector2(0, 0);
        }
    }
    private void CheckResetAirAttack() {
        // ตรวจสอบว่าแอนิเมชั่นที่กำลังเล่นเสร็จแล้วหรือไม่
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        transform.position = placeGrabing;
        rigidBody2D.linearVelocity = new Vector2(0, 0);
        // if (animator.IsInTransition(0)) { return; }
        if (stateInfo.IsName("Vaja_airAttack") && stateInfo.normalizedTime >= 0.75f) { // take from attack2
            // แอนิเมชั่นเล่นเสร็จแล้ว
            isAirAttack = false;
            animator.SetBool("isAirAttack", false);
        } else if (isGrounded) {
            // อยู่พื้น
            isAirAttack = false;
            animator.SetBool("isAirAttack", false);
        }
    }
    private void SlamDrunkAttack() {
        // ตรวจสอบการกดปุ่มโจมตี
        if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Z)) && (Input.GetAxis("Vertical") <= -0.01) && !havedAirAttack && Mathf.Abs(rigidBody2D.linearVelocity.y) >= slamDrunkAllow) {
            placeGrabing = transform.position;
            isSlamDrunkAttack = true;
            havedAirAttack = true;
            animator.SetBool("isSlamDrunkAttack", true);
            GenAttackArea(0.25f, 0, slamDunkAttackArea);
            // Instantiate(slamDunkEffect, transform.position, transform.rotation);
            if (rigidBody2D.linearVelocity.y >= 0) {
                rigidBody2D.AddForce( Vector2.up * (jumpPower  / 2) );
            } else {
                rigidBody2D.AddForce( Mathf.Abs(rigidBody2D.linearVelocity.y) * (Vector2.up * jumpPower) );
            }
            // transform.position = placeGrabing;
            // rigidBody2D.linearVelocity = new Vector2(0, 0);
        }
    }
    private void CheckResetSlamDrunkAttack() {
        // ตรวจสอบว่าแอนิเมชั่นที่กำลังเล่นเสร็จแล้วหรือไม่
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        // transform.position = placeGrabing;
        // rigidBody2D.linearVelocity = new Vector2(0, 0);
        // if (animator.IsInTransition(0)) { return; }
        if (stateInfo.IsName("Vaja_SlamDrunk") && stateInfo.normalizedTime >= 0.75f) {
            // แอนิเมชั่นเล่นเสร็จแล้ว
            isSlamDrunkAttack = false;
            animator.SetBool("isSlamDrunkAttack", false);
            rigidBody2D.linearVelocity = new Vector2(0, jumpPower * 0.65f * -1f);
            // if (animator.IsInTransition(0)) { Instantiate(slamDunkEffect, transform.position, Quaternion.identity); }
            // if (isGrounded) { Instantiate(slamDunkEffect, new Vector3(transform.position.x + (0.35f * GetLeftRight()), transform.position.y, transform.position.z), transform.rotation); }
            // Instantiate(slamDunkEffect, new Vector3(transform.position.x + (0.35f * GetLeftRight()), transform.position.y, transform.position.z), transform.rotation);
        }
        // else if (stateInfo.IsName("Vaja_SlamDrunk") && stateInfo.normalizedTime >= 0.75f)
        // {
            // if (animator.IsInTransition(0)) { Instantiate(slamDunkEffect, transform.position, Quaternion.identity); }
        // }
    }
    private bool CheckIdle() {
        // ตรวจสอบว่าแอนิเมชั่นที่กำลังเล่นเสร็จแล้วหรือไม่
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Vaja_Idle")) { return true; } 
        if (stateInfo.IsName("Vaja_Idle2")) { return true; } 
        if (stateInfo.IsName("Vaja_Idle3")) { return true; }
        return false;
    }
    private void SetRandomIdleTime() {
        // สุ่มเวลา idle ในช่วงที่กำหนด
        idleTime = UnityEngine.Random.Range(idleTimeMin, idleTimeMax);
    }
    private void RandomIdleAnimation() {
        // เปลี่ยน animation idle อย่างสุ่ม (สมมุติว่าใน Animator มีหลาย state เช่น "Idle1", "Idle2", "Idle3")
        int newrandomIdle = UnityEngine.Random.Range(1, 4); // จะสุ่มให้เลือกจาก "Idle1", "Idle2", "Idle3"
        if (newrandomIdle == randomIdle) { 
            animator.SetInteger("IdleState", 0);
            return; 
        }
        randomIdle = newrandomIdle;
        animator.SetInteger("IdleState", randomIdle);  // ตั้งค่าไปยัง Animator
    }
    private void Skill()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (Input.GetKeyDown(KeyCode.E) && Time.time > skillRate && isGrounded)
        {
            isSkill = true;
            animator.SetBool("isSkillCasting", true);
            Instantiate(ballSkill, new Vector3(transform.position.x + (2.15f * GetLeftRight()), transform.position.y + 5f, transform.position.z), transform.rotation);
            skillRate = Time.time + cooldownSkill;
        }
        if (Input.GetKeyDown(KeyCode.Q) && Time.time > skillRate && isGrounded)
        {
            isSkill = true;
            animator.SetBool("isSkillCasting", true);
            Instantiate(lightSkill, new Vector3(enemyTransform.position.x, enemyTransform.position.y + 12f, enemyTransform.position.z), Quaternion.identity);
            skillRate = Time.time + cooldownSkill;
        }
        if (Input.GetKeyDown(KeyCode.R) && Time.time > skillRate && isGrounded)
        {
            isSkill = true;
            animator.SetBool("isSkillCasting", true);
            Instantiate(lightExplodeSkill, new Vector3(enemyTransform.position.x, enemyTransform.position.y + 7f, enemyTransform.position.z), Quaternion.identity);
            skillRate = Time.time + cooldownSkill;
        }
    }

    private void CheckSkilled()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Vaja_Skill_Hold"))
        {
            if (stateInfo.normalizedTime > 0.95)
            {
                isSkill = false;
                animator.SetBool("isSkillCasting", false);
            }
        }
    }
    private void Healing()
    {
        if (Input.GetKey(KeyCode.H) && numPotion > 0 && !isHealed)
        {
            Instantiate(healEffect, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
            healt += 25;
            numPotion -= 1;
            potionUI.text = "x " + numPotion.ToString();
            isHealed = true;
        }
    }
    private void GenAttackArea(float offsetX, float offsetY, GameObject areaAtk) // สร้างขอบเขตการโจมตี
    {
        Instantiate(areaAtk, new Vector3(transform.position.x + (offsetX * GetLeftRight()), transform.position.y + offsetY, transform.position.z), transform.rotation);
    }

    private void checkKnockBack()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Vaja_getup"))
        {
            if (stateInfo.normalizedTime > 0.95) { isKnockback = false; }
        }
    }
    private void Death()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        isDeath = true;
        healt = 0;
        // animator.SetTrigger("dead");
        // animator.ResetTrigger("gotAttack");
        animator.SetBool("isDead", true);
        rigidBody2D.linearVelocity = new Vector2(0, 0);
        // if (stateInfo.IsName("Vaja_dead"))
        // {
            // if (stateInfo.normalizedTime > 0.95) { healt += 1; animator.SetBool("isDead", false); }
        // }
    }
    private void GenSlamDunkEffect()
    {
        Instantiate(slamDunkEffect, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
    }
    private void GenSlash(GameObject slashEffect)
    {
        Quaternion playerRotate = transform.rotation;
        float playerRotate_Y = playerRotate.eulerAngles.y;
        Quaternion slashRotate = Quaternion.Euler(0, playerRotate_Y + slashRotate_Y, slashRotate_Z);
        Instantiate(slashEffect, new Vector3(transform.position.x + (slashOffset_X * GetLeftRight()), transform.position.y + slashOffset_Y, transform.position.z), slashRotate);
    }
    private void NearestEnemySelected()
    {
        float minDistance = Mathf.Infinity;
        foreach (Transform enemy in enemiesTransform)
        {
            float distance = Vector3.Distance(transform.position, enemy.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestEnemy =  enemy;
            }
        }
        enemyTransform = nearestEnemy;
    }

    IEnumerator resetHealed()
    {
        yield return new WaitForSeconds(1.5f);
        isHealed = false;
    } 
    public int GetHealth()
    {
        return healt;
    }

    public void setHealth(int health)
    {
        this.healt = health;
    }
    public void setNumPotion(int numPotion)
    {
        this.numPotion = numPotion;
    }
    public bool GetIsGrounded() { return isGrounded; }
    private int GetLeftRight() {
        if (left) {
            return -1;
        } return 1;
    }

    private void OnTriggerEnter2D(Collider2D target)
    {
        // การโจมตีของ StoneMonster
        if (target.gameObject.CompareTag("StoneMonsAtk_1") || target.gameObject.CompareTag("StoneMonsAtk_3") && !isDodge && !isKnockback)
        {
            isKnockback = true;
            animator.SetTrigger("gotAttack");
            if (playerPos > enemyPos) { rigidBody2D.AddForceX(knockbackForce * 1.8f, ForceMode2D.Impulse); }
            else if (playerPos < enemyPos) { rigidBody2D.AddForceX(knockbackForce * -1.8f, ForceMode2D.Impulse); }
            healt -= UnityEngine.Random.Range(3, 8);
            if (gameObject.CompareTag("StoneMonsAtk_1")) { Destroy(target.gameObject); }
        }
        if (target.gameObject.CompareTag("StoneMonsAtk_2") && !isDodge && !isKnockback)
        {
            isKnockback = true;
            animator.SetTrigger("gotAttack");
            if (playerPos > enemyPos) { rigidBody2D.AddForce(new Vector2(knockbackForce * 1.2f, knockbackForce * 1.95f), ForceMode2D.Impulse); }
            else if (playerPos < enemyPos) { rigidBody2D.AddForce(new Vector2(knockbackForce * -1.2f, knockbackForce * 1.95f), ForceMode2D.Impulse); }
            healt -= UnityEngine.Random.Range(3, 8);
            Destroy(target.gameObject);
        }
        if (target.CompareTag("CoconutAtk") && !isDodge && !isKnockback)
        {
            isKnockback = true;
            animator.SetTrigger("gotAttack");
            if (playerPos > enemyPos) { rigidBody2D.AddForceX(knockbackForce * 1.15f, ForceMode2D.Impulse); }
            else if (playerPos < enemyPos) { rigidBody2D.AddForceX(knockbackForce * -1.15f, ForceMode2D.Impulse); }
            healt -= UnityEngine.Random.Range(3, 8);
        }
        if (target.gameObject.CompareTag("DeerAttack") && !isDodge && !isKnockback)
        {
            isKnockback = true;
            animator.SetTrigger("gotAttack");
            rigidBody2D.AddForce(new Vector2(knockbackForce * 1.25f * GetLeftRight(), knockbackForce * 1.85f), ForceMode2D.Impulse);
            healt -= UnityEngine.Random.Range(5, 13);
        } 
        if (target.CompareTag("CheckBar") && !havedBossBar)
        {
            bossBar.SetActive(true);
            havedBossBar = true;
        }
    }
}
 
 