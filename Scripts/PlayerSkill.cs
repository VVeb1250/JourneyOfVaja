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
        if (stateInfo.IsName("Vaja_BallSkill"))
        {
            if (stateInfo.normalizedTime > 0.2f) { transform.Translate(Vector2.right * moveSpeed * Time.deltaTime); }
        }
        if (stateInfo.IsName("Vaja_BallSkill_End"))
        {
            transform.Translate(Vector2.zero);
            if (stateInfo.normalizedTime > 0.95f) { Destroy(this.gameObject); }
        }
    }
    IEnumerator DelayDestroy()
    {
        yield return new WaitForSeconds(0.2f);
        animator.SetTrigger("End");
    }

    private void OnTriggerEnter2D(Collider2D target)
    {
        if (target.gameObject.CompareTag("Boss"))
        {
            // transform.Translate(Vector2.zero);
            animator.SetTrigger("End");
            // if (stateInfo.IsName("Fire_I_End (Player)"))
            // {
                // if (stateInfo.normalizedTime > 0.95f) { Destroy(this.gameObject); }
            // }
        }
    }
}
