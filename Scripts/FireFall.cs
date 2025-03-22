using UnityEngine;

public class FireFall : MonoBehaviour
{
    private Rigidbody2D rigidbody2D;
    private float force;
    private Animator animator;
    private AnimatorStateInfo stateInfo;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidbody2D = this.gameObject.GetComponent<Rigidbody2D>();
        animator = this.gameObject.GetComponent<Animator>();

        int direction = Random.Range(-1, 2);
        force = Random.Range(180, 260);

        rigidbody2D.AddForceX(force * direction);
        rigidbody2D.AddForceY(force * 0.7f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D target)
    {
        if (target.gameObject.CompareTag("Ground"))
        {
            Destroy(this.gameObject);
        }
    }
}
