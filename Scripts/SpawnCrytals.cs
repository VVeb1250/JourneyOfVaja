using System.Collections;
using UnityEngine;

public class SpawnCrytals : MonoBehaviour
{
    [SerializeField] private float upSpeed;
    [SerializeField] private float timeDestroy;
    private Animator animator;
    private Animator monsAnimator;
    private Rigidbody2D rigidbody2D;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = this.gameObject.GetComponent<Animator>();
        GameObject monsterObj = GameObject.FindWithTag("Boss");
        if (monsterObj != null)
        {
            rigidbody2D = monsterObj.GetComponent<Rigidbody2D>();
            monsAnimator = monsterObj.GetComponent<Animator>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("SpawnCrytals"))
        {
            monsAnimator.SetBool("Walk", false);
            rigidbody2D.linearVelocity = new Vector2(0, 0);
        }
    }

    private void DestroyObj()
    {
        Destroy(this.gameObject);
    }
}
