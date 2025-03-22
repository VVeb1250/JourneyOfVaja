using System.Collections;
using UnityEngine;

public class DeerAttack : MonoBehaviour
{
    [SerializeField] private float timeDestroy;
    [SerializeField] private float attackSpeed;
    // [SerializeField] private PolygonCollider2D collider;
    private AnimatorStateInfo stateInfo;
    private Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = this.gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("DeerCrystal"))
        {
            if (stateInfo.normalizedTime > timeDestroy) { Destroy(this.gameObject); }
        }
        if (stateInfo.IsName("DeerCrystal_2")) { StartCoroutine(DelayDestroy(3.25f)); }
    }

    private void OnTriggerEnter2D(Collider2D target)
    {
        // if (target.gameObject.CompareTag("Player")) { collider.enabled = false; }
    }
    IEnumerator DelayDestroy(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(this.gameObject);
    }
}
