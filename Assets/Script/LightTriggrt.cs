using UnityEngine;

public class LightTriggrt : MonoBehaviour
{
    private bool playerNearby = false;
    private Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // 檢查玩家是否靠近並且按下 F 鍵
        if (playerNearby && Input.GetKeyDown(KeyCode.F))
        {
            if (animator != null)
            {
                animator.SetTrigger("isPush");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
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