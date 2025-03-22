using System.Collections;
using UnityEngine;

public class Destroy : MonoBehaviour
{
    // private Animator animator;
    // public float timeDestroy;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        // if (stateInfo.normalizedTime >= timeDestroy && !stateInfo.IsName("Fire_Bomb_Start"))
        // {
            // Destroy(this.gameObject);
        // }
    }
    private void DestroyObj()
    {
        Destroy(this.gameObject);
    }
    private void SetVisible()
    {
        gameObject.SetActive(false);
    }
}
