using UnityEngine;

public class BombArea : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject fireFall;
    private bool isInstantiated = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject animatorObj = GameObject.FindWithTag("FireBomb");
        if (animatorObj != null)
        {
            animator = animatorObj.GetComponent<Animator>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Fire_Bomb_End") && fireFall != null)
        {
            if (stateInfo.normalizedTime > 0.7f)
            {
                transform.position = new Vector3(-3.65f, -0.15f, -4.157f);
                transform.localScale = new Vector3(10.8f, 10.8f, 10.8f);

                if (!isInstantiated)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        Instantiate(fireFall, new Vector3 (transform.position.x, transform.position.y + 2.75f, transform.position.z), transform.rotation);
                        new WaitForSeconds(0.2f);
                    }
                    isInstantiated = true;
                }
            }
            if (stateInfo.normalizedTime > 0.95f)
            {
                Destroy(this.gameObject);
            }
        }
        else if (stateInfo.IsName("Fire_Bomb_End"))
        {
            if (stateInfo.normalizedTime > 0.95f)
            {
                Destroy(this.gameObject);
            }
        }
    }
}
