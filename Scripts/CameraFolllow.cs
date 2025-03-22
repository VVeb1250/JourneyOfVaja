using UnityEngine;

public class CameraFolllow : MonoBehaviour
{
    public Transform player;
    public Vector3 offset;
    public float raduis;
    private bool fixedCamera;
    public LayerMask layerFixed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(player.position.x + offset.x, transform.position.y, offset.z);

        if (fixedCamera)
        {
            transform.position = new Vector3(39.77f, player.position.y + offset.y, offset.z);
        }
    }

    private void FixedUpdate()
    {
        Collider2D hit = Physics2D.OverlapCircle(player.transform.position, raduis, layerFixed);
        fixedCamera = hit != null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(player.transform.position, raduis);
    }

    private void OnTriggerEnter2D(Collider2D target)
    {
        if (target.gameObject.CompareTag("InvisibleWall"))
        {
            Debug.Log("fixedCamera");
            transform.position = new Vector3(transform.position.x , transform.position.y, transform.position.z);
        }
    }
}
