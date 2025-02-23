using System.Collections;
using UnityEngine;

public class CoconutAttack : MonoBehaviour
{
    [SerializeField] private float attackSpeed;
    [SerializeField] private float timeDestory;
    [SerializeField] private Transform playerPos;
    private Animator animator;
    private AnimatorStateInfo stateInfo;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null) { playerPos = playerObj.GetComponent<Transform>(); }
    }

    // Update is called once per frame
    void Update()
    {
        stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        transform.Translate(Vector2.right * attackSpeed * Time.deltaTime);

        if (stateInfo.IsName("Fire_I_End"))
        {
            if (stateInfo.normalizedTime > 0.95) { Destroy(this.gameObject); }
        }

        StartCoroutine(DelayDestory(timeDestory));
    }
    public IEnumerator DelayDestory(float timeDestroy)
    {
        yield return new WaitForSeconds(timeDestroy);
        animator.SetBool("End", true);
    }
    private void OnTriggerEnter2D(Collider2D target)
    {
        if (target.gameObject.CompareTag("Player"))
        {
            transform.Translate(Vector2.zero);
            animator.SetBool("End", true);
        }
    }
}
