using System.Collections;
//using UnityEditor.Rendering;
using UnityEngine;

public class CrystalFall : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float attackSpeed;
    [SerializeField] private float timeDestroy;
    private Rigidbody2D rigidbody2D;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        // GameObject animatorObj = GameObject.Find("GenkiCharge (Clone)");
        // if (animatorObj != null ) { animator =  animatorObj.GetComponent<Animator>(); }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.right * -attackSpeed * Time.deltaTime);
        transform.Translate(Vector2.down * attackSpeed * Time.deltaTime);
        // AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        // if (stateInfo.IsName("DeerGenkiElectric"))
        // {
        // transform.Translate(Vector2.right * -attackSpeed * Time.deltaTime);
        // transform.Translate(Vector2.down * attackSpeed * Time.deltaTime);
        // }
        // if (transform.rotation.y == 180 && transform.rotation.z != 45) { rigidbody2D.AddForce(new Vector2(attackSpeed, attackSpeed * Random.Range(-3.5f, 0f))); }
        // if (transform.rotation.z == 45) { rigidbody2D.AddForce(new Vector2(0, attackSpeed * -0.45f)); }
        // if (transform.rotation.y == 0 && transform.rotation.z != 45) { rigidbody2D.AddForce(new Vector2(attackSpeed * -1, attackSpeed * Random.Range(-3.5f, 0f))); }
        // rigidbody2D.linearVelocity = new Vector2(attackSpeed, attackSpeed);
    }
}
