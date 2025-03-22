using System.Collections;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public float speed;
    private Rigidbody2D rigidbody2D;
    public float hp = 30;
    private Animator animator;
    public GameObject deathEffectAnim;
    public float flipWalk;
    private float startTime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidbody2D = this.gameObject.GetComponent<Rigidbody2D>();
        animator = this.gameObject.GetComponent<Animator>();
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        rigidbody2D.linearVelocity = new Vector2(speed, 0);
        if (hp <= 0)
        {
            StartCoroutine(Death());
        }

        if (Time.time - startTime > flipWalk)
        {
            transform.Rotate(new Vector3(0, 180, 0));
            speed *= -1;
            startTime = Time.time;
        }
    }

     private void OnTriggerEnter2D(Collider2D target)
    {
        if (target.gameObject.CompareTag("Attack"))
        {
            hp -= Random.Range(2, 8);
        }
    }

    IEnumerator Death()
    {
        rigidbody2D.linearVelocity = new Vector2(0, 0);
        animator.SetTrigger("IsDeath");
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        Instantiate(deathEffectAnim, transform.position, transform.rotation);
        Destroy(this.gameObject);
    }
}
