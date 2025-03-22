using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerAttack : MonoBehaviour
{
    private double startTime;
    public PlayerController playerDetials;
    public float timeDestroy;
    public float attackSpeed;
    public string attackName;
    public Animator animator;
    private AnimatorStateInfo stateInfo;
    // private float speed = 0.01f;

    // public Vector2 attackOffset;
    // public Vector3 attackScale;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startTime = Time.time;
        GameObject animatorObject = GameObject.Find("Vaja 1");
        if (animatorObject != null)
        {
            animator = animatorObject.GetComponent<Animator>();
        }
        if (attackSpeed == 0)
        {
            transform.Translate(Vector2.right);
        }
        if (attackName == "SlamDunk") { transform.Rotate(180, 0, 0); }
        
    }

    // Update is called once per frame
    void Update()
    {
        stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Vaja_attack3"))
        {
            transform.Translate(Vector2.right * attackSpeed * Time.deltaTime);
        }
        if (stateInfo.IsName("Vaja_attack3") && stateInfo.normalizedTime > 0.75f)
        {
             Destroy(this.gameObject);
        }
        if (stateInfo.IsName("Vaja_SlamDrunk"))
        {
            transform.Translate(Vector2.down * attackSpeed * Time.deltaTime);
            if (stateInfo.normalizedTime > timeDestroy) { Destroy(this.gameObject); }
        }
        if (stateInfo.normalizedTime > timeDestroy && attackName != "SlamDunk")
        {
            Destroy(this.gameObject);
        }
    }
}
