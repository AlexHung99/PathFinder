using UnityEngine;
using Unity.VisualScripting.Dependencies.Sqlite;
public class BodyTrigger : MonoBehaviour
{
    private bool m_IsGrounded = true; // 確保初始值為 true
    private Renderer objectRenderer;
    private Color originalColor;
    private bool isJumping = false;
    public float jumpHeight = 0.5f; // 稍微彈跳高度
    public float groundHeight = 0f; // 落地高度
    private float jumpSpeed = 3f; // 彈跳速度
    private Vector3 initialPosition;
    private bool playerNearby = false;
    private Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        
        //string databasePath = Application.persistentDataPath + "/mydatabase.db";
        //using (var db = new SQLiteConnection(databasePath, SQLiteOpenFlags.ReadWrite))
        //{
        //    var count = db.CreateTable<UserInfo>();
        //    count = db.CreateTable<RandomEvent>();
        //}

    }
    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer != null)
        {
            originalColor = objectRenderer.material.color;
        }
        initialPosition = transform.position;

        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isJumping)
        {
            // 物件向上彈跳
            transform.position += Vector3.up * jumpSpeed * Time.deltaTime;

            // 檢查是否到達彈跳高度
            if (transform.position.y >= initialPosition.y + jumpHeight)
            {
                isJumping = false;
            }
        }
        else if (!m_IsGrounded)
        {
            // 物件掉落
            transform.position += Vector3.down * jumpSpeed * Time.deltaTime;

            // 檢查是否到達地面
            if (transform.position.y <= groundHeight)
            {
                m_IsGrounded = true;
                transform.position = new Vector3(transform.position.x, groundHeight, transform.position.z);
            }
        }

        // 檢查玩家是否靠近並且按下 F 鍵且物件在地面上
        if (playerNearby && m_IsGrounded && Input.GetKeyDown(KeyCode.F))
        {
            if (rb != null)
            {
                rb.isKinematic = false; // 初始設置為 kinematic
            }
            isJumping = true;
            m_IsGrounded = false;

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.tag + " trigger");
        if (other.tag == "Player")
        {
            Debug.Log("Player enter the body trigger");
            playerNearby = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            playerNearby = false;
        }
    }
}
