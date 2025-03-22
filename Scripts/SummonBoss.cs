using System.Collections;
using UnityEngine;

public class SummonBoss : MonoBehaviour
{
    public GameObject boss;
    public GameObject effectSummon;
    private Animator animator;
    private int checkNum;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = effectSummon.GetComponent<Animator>();
        checkNum = 0;
        boss.transform.eulerAngles = new Vector2(0, 180);

        StartCoroutine(Summon());
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = boss.transform.position;
    }

    public GameObject InstantiateObj()
    {
        return boss;
    }
    IEnumerator Summon()
    {
        yield return new WaitForSeconds(2.5f);
        Instantiate(effectSummon);
        if (checkNum == 0)
        {
            Instantiate(boss);
            checkNum++;
        }
    }
}
