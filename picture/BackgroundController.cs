using UnityEngine;
using UnityEditor;
using JetBrains.Annotations;

public class BackgroundController : MonoBehaviour
{
    private float[] startPos;
    public GameObject cam;
    private Transform[] Items;
    public Material newMaterial; // Shader ที่ต้องการใช้
    private Renderer[] childRenderers;
    public float pallaxEffect;
    private SpriteRenderer spriteRenderer; // สำหรับเก็บ SpriteRenderer ของ GameObject นี้
    private SpriteRenderer parentSpriteRenderer; // สำหรับเก็บ SpriteRenderer ของตัวแม่

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Items = GetComponentsInChildren<Transform>();
        startPos = new float[Items.Length];
        SetUpItenPos();
        SetSortingLayer();
        SetUpShader();
    }
    private void SetUpItenPos()
    {
        int i = 0;
        foreach (Transform item in Items)
        {
            startPos[i] = item.position.x;
            i++;
        }
    }
    private void SetUpShader()
    {
        // ดึง Renderer ของทุกๆ ลูกใน GameObject นี้
        childRenderers = GetComponentsInChildren<Renderer>();

        // เปลี่ยน Shader ของทุกๆ ลูก
        foreach (Renderer childRenderer in childRenderers)
        {
            if (childRenderer != null)
            {
                // ตั้ง Shader ใหม่ให้กับ Material
                childRenderer.material = newMaterial;
            }
        }
    }
    private void SetSortingLayer()
    {
        // ดึง SpriteRenderer ของ GameObject นี้
        spriteRenderer = GetComponent<SpriteRenderer>();

        // ดึง SpriteRenderer ของตัวแม่ (Parent)
        if (transform.parent != null)
        {
            parentSpriteRenderer = transform.GetComponent<SpriteRenderer>();
            
            // ตรวจสอบว่าตัวแม่มี SpriteRenderer หรือไม่
            if (parentSpriteRenderer != null)
            {
                // ตั้งค่า sortingLayerName ให้กับ GameObject นี้จากตัวแม่
                spriteRenderer.sortingLayerName = parentSpriteRenderer.sortingLayerName;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        int i = 0;
        foreach (Transform item in Items)
        {
            float distance = (item.transform.position.x - cam.transform.position.x) * pallaxEffect;

            item.transform.position = new Vector3(startPos[i] + distance, item.transform.position.y, item.transform.position.z);

            i++;
        }
    }
}