using System.Collections;
using UnityEngine;

public class PlayerSkill : MonoBehaviour
{
    public float moveSpeed;
    [SerializeField] private Animator animator;
    private AnimatorStateInfo stateInfo;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = this.gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Fire_I"))
        {
            if (stateInfo.normalizedTime > 0.2f) { transform.Translate(Vector2.right * moveSpeed * Time.deltaTime); }
        }
        if (stateInfo.IsName("Fire_I_End"))
        {
            if (stateInfo.normalizedTime > 0.95f) { Destroy(this.gameObject); }
        }
    }
    IEnumerator DelayDestroy()
    {
        yield return new WaitForSeconds(0.55f);
        animator.SetBool("End", true);
    }

    private void OnTriggerEnter2D(Collider2D target)
    {
        if (target.gameObject.CompareTag("Boss"))
        {
            animator.SetBool("End", true);
            if (stateInfo.IsName("Fire_I_End"))
            {
                if (stateInfo.normalizedTime > 0.95f) { Destroy(this.gameObject); }
            }
        }
    }
}
