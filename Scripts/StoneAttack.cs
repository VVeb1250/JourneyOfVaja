using UnityEngine;

public class StoneAttack : MonoBehaviour
{
    [SerializeField]private float timeDestroy;
    [SerializeField] private float attackSpeed;    
    [SerializeField]private Animator animator;
    [SerializeField] private string animatiorName;
    private AnimatorStateInfo stateInfo;
    private float startTime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startTime = Time.time;
        GameObject animatorObj = GameObject.FindWithTag("StoneMonster");
        if (animatorObj != null )
        {
            animator = animatorObj.GetComponent<Animator>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (attackSpeed != 0)
        {
            transform.Translate(Vector2.right * attackSpeed * Time.deltaTime);
            if (stateInfo.IsName(animatiorName))
            {
                if (stateInfo.normalizedTime > timeDestroy)
                {
                    Destroy(this.gameObject);
                }
            }
        }
        else
        {
            if (stateInfo.normalizedTime > timeDestroy)
            {
                Destroy(this.gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D target)
    {
        if (target.gameObject.CompareTag("Player"))
        {
            Destroy(this.gameObject);
        }
    }
}
