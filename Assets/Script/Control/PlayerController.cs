using PathFinder.Combat;
using PathFinder.Movement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private NavMeshAgent agent;

    public float rotateSpeedMovement = 0.1f;
    private float rotateVelocity;

    [SerializeField]
    private Camera mainCamera;

    [SerializeField]
    private InputActionAsset inputActions;

    private InputAction moveAction;
    private InputAction rightClickMoveAction;
    //private InputAction jumpAction;

    private Vector2 inputVector;

    private string isWaik = "isWaik";
    private string isJump = "isJump";
    private Animator ani;

    #region 法術相關變數
    public GameObject stoneSlashPrefab;
    public GameObject magicShieldYellowPrefab;
    public GameObject healingPrefab;
    public GameObject buffPrefab;

    private GameObject currentSpellPrefab;
    public GameObject circleObject;
    #endregion 法術相關變數

    // Start is called before the first frame update
    void Start()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        ani = gameObject.GetComponent<Animator>(); // 初始化 Animator

        if (mainCamera == null)
        {
            Debug.LogError("Main Camera is not found.");
        }

        // 獲取移動、右鍵點擊移動和跳躍動作
        var playerActionMap = inputActions.FindActionMap("Player");
        moveAction = playerActionMap.FindAction("Move");
        rightClickMoveAction = playerActionMap.FindAction("RightClickMove");
        //jumpAction = playerActionMap.FindAction("Jump");

        moveAction.Enable();
        rightClickMoveAction.Enable();
        //jumpAction.Enable();

        moveAction.performed += OnMove;
        moveAction.canceled += OnMoveCanceled;
        rightClickMoveAction.performed += OnRightClickMove;
        //jumpAction.performed += OnJump;
    }

    private void OnDestroy()
    {
        moveAction.performed -= OnMove;
        moveAction.canceled -= OnMoveCanceled;
        rightClickMoveAction.performed -= OnRightClickMove;
        //jumpAction.performed -= OnJump;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        inputVector = context.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        inputVector = Vector2.zero;
        ani.ResetTrigger(isWaik); // 重置動畫狀態
    }

    public void OnRightClickMove(InputAction.CallbackContext context)
    {
        RaycastHit hit;

        if (Physics.Raycast(mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue()), out hit, Mathf.Infinity))
        {
            // 檢查目標點是否在 NavMesh 上
            NavMeshHit navMeshHit;
            if (NavMesh.SamplePosition(hit.point, out navMeshHit, 1.0f, NavMesh.AllAreas))
            {
                // 移動到點擊位置
                agent.SetDestination(navMeshHit.position);
                ani.SetTrigger(isWaik);
            }
            else
            {
                Debug.LogWarning("Target point is not on the NavMesh.");
            }
        }
    }

    private void MoveToCursor()
    {
        Ray ray = GetMouseRay();
        RaycastHit hit;
        bool hasHit = Physics.Raycast(ray, out hit);
        if (hasHit)
        {
            GetComponent<Mover>().MoveTo(hit.point);
        }
    }

    private static Ray GetMouseRay()
    {
        return Camera.main.ScreenPointToRay(Input.mousePosition);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        // 跳躍邏輯
        if (agent.isOnNavMesh)
        {
            agent.velocity += Vector3.up * 5f; // 調整跳躍高度
        }
    }

    //public CharacterInfoData characterInfoData;
    // Update is called once per frame
    void Update()
    {
        if (mainCamera == null)
        {
            return;
        }

        InteracWithMovement();
        InteracWithCombat();

    }

    private void InteracWithCombat() //戰鬥互動
    {
        // 檢查滑鼠點擊
        RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
        foreach(RaycastHit hit in hits)
        {
            CombatTarget target =  hit.transform.GetComponent<CombatTarget>();
            if(target == null) continue;

            // 檢查是否點擊到敵人 (CombatTarget) 滑鼠左鍵攻擊
            if (Input.GetMouseButtonDown(0))
            {
                GetComponent<Fighter>().Attact(target);
            }
        }

        // 檢查是否在使用 OffMeshLink
        if (agent.isOnOffMeshLink)
        {
            StartCoroutine(HandleOffMeshLink());
        }


        // 檢查按鍵輸入
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SelectSpell(stoneSlashPrefab);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SelectSpell(magicShieldYellowPrefab);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SelectSpell(healingPrefab);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SelectSpell(buffPrefab);
        }
    }

    private void InteracWithMovement() //移動互動
    {


        // 處理 WSAD 移動
        if (inputVector != Vector2.zero)
        {
            Vector3 move = ConvertInputToCameraDirection(inputVector);
            agent.SetDestination(transform.position + move);
            ani.SetTrigger(isWaik);
        }

        // 檢查是否到達目的地
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
            {
                ani.ResetTrigger(isWaik); // 重置動畫狀態
            }
        }
    }



    #region 法術相關方法
    private void SelectSpell(GameObject spellPrefab)
    {
        currentSpellPrefab = spellPrefab;
        PlaceSpell();
    }

    private void PlaceSpell()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 10f; // 設置一個合適的深度
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);

        // 設置法術的方向
        Vector3 direction = (worldPosition - transform.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(direction);

        // 調整法術位置的高度與角色一致
        worldPosition.y = transform.position.y;

        // 在滑鼠位置施放法術並設置為角色的子物件
        GameObject spellInstance = Instantiate(currentSpellPrefab, worldPosition, rotation);
        spellInstance.transform.SetParent(transform);

        Destroy(spellInstance, 5f); // 5秒後銷毀效果物件
    }
    #endregion 法術相關方法

    private Vector3 ConvertInputToCameraDirection(Vector2 input)
    {
        // 獲取相機的前方向和右方向
        Vector3 cameraForward = mainCamera.transform.forward;
        Vector3 cameraRight = mainCamera.transform.right;

        // 忽略相機的垂直方向
        cameraForward.y = 0;
        cameraRight.y = 0;

        // 正規化方向向量
        cameraForward.Normalize();
        cameraRight.Normalize();

        // 計算相對於相機的移動方向
        Vector3 move = cameraForward * input.y + cameraRight * input.x;
        return move;
    }

    private IEnumerator HandleOffMeshLink()
    {
        // 開始使用 OffMeshLink，觸發跳躍動畫
        ani.SetTrigger(isJump);

        // 獲取 OffMeshLink 資訊
        OffMeshLinkData data = agent.currentOffMeshLinkData;
        Vector3 startPos = agent.transform.position;
        Vector3 endPos = data.endPos + Vector3.up * agent.baseOffset;

        // 設定跳躍高度
        float jumpHeight = 2.0f; // 可以根據需要調整跳躍高度
        float duration = 0.8f; // 跳躍持續時間
        float elapsedTime = 0.0f;

        // 禁用 NavMeshAgent 以手動控制角色位置
        agent.updatePosition = false;

        //參數問題
        while (elapsedTime <= duration)
        {
            float t = elapsedTime / duration;
            float height = Mathf.Sin(Mathf.PI * t) * jumpHeight; // 使用正弦函數模擬拋物線運動
            agent.transform.position = Vector3.Lerp(startPos, endPos, t) + Vector3.up * height;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 手動設置 NavMeshAgent 的位置並重新啟用它
        agent.transform.position = endPos;
        agent.updatePosition = true;
        agent.CompleteOffMeshLink();

        // 重置跳躍動畫
        ani.ResetTrigger(isJump);
    }
}