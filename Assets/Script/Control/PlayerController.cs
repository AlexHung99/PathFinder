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

    #region �k�N�����ܼ�
    public GameObject stoneSlashPrefab;
    public GameObject magicShieldYellowPrefab;
    public GameObject healingPrefab;
    public GameObject buffPrefab;

    private GameObject currentSpellPrefab;
    public GameObject circleObject;
    #endregion �k�N�����ܼ�

    // Start is called before the first frame update
    void Start()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        ani = gameObject.GetComponent<Animator>(); // ��l�� Animator

        if (mainCamera == null)
        {
            Debug.LogError("Main Camera is not found.");
        }

        // ������ʡB�k���I�����ʩM���D�ʧ@
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
        ani.ResetTrigger(isWaik); // ���m�ʵe���A
    }

    public void OnRightClickMove(InputAction.CallbackContext context)
    {
        RaycastHit hit;

        if (Physics.Raycast(mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue()), out hit, Mathf.Infinity))
        {
            // �ˬd�ؼ��I�O�_�b NavMesh �W
            NavMeshHit navMeshHit;
            if (NavMesh.SamplePosition(hit.point, out navMeshHit, 1.0f, NavMesh.AllAreas))
            {
                // ���ʨ��I����m
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
        // ���D�޿�
        if (agent.isOnNavMesh)
        {
            agent.velocity += Vector3.up * 5f; // �վ���D����
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

    private void InteracWithCombat() //�԰�����
    {
        // �ˬd�ƹ��I��
        RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
        foreach(RaycastHit hit in hits)
        {
            CombatTarget target =  hit.transform.GetComponent<CombatTarget>();
            if(target == null) continue;

            // �ˬd�O�_�I����ĤH (CombatTarget) �ƹ��������
            if (Input.GetMouseButtonDown(0))
            {
                GetComponent<Fighter>().Attact(target);
            }
        }

        // �ˬd�O�_�b�ϥ� OffMeshLink
        if (agent.isOnOffMeshLink)
        {
            StartCoroutine(HandleOffMeshLink());
        }


        // �ˬd�����J
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

    private void InteracWithMovement() //���ʤ���
    {


        // �B�z WSAD ����
        if (inputVector != Vector2.zero)
        {
            Vector3 move = ConvertInputToCameraDirection(inputVector);
            agent.SetDestination(transform.position + move);
            ani.SetTrigger(isWaik);
        }

        // �ˬd�O�_��F�ت��a
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
            {
                ani.ResetTrigger(isWaik); // ���m�ʵe���A
            }
        }
    }



    #region �k�N������k
    private void SelectSpell(GameObject spellPrefab)
    {
        currentSpellPrefab = spellPrefab;
        PlaceSpell();
    }

    private void PlaceSpell()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 10f; // �]�m�@�ӦX�A���`��
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);

        // �]�m�k�N����V
        Vector3 direction = (worldPosition - transform.position).normalized;
        Quaternion rotation = Quaternion.LookRotation(direction);

        // �վ�k�N��m�����׻P����@�P
        worldPosition.y = transform.position.y;

        // �b�ƹ���m�I��k�N�ó]�m�����⪺�l����
        GameObject spellInstance = Instantiate(currentSpellPrefab, worldPosition, rotation);
        spellInstance.transform.SetParent(transform);

        Destroy(spellInstance, 5f); // 5���P���ĪG����
    }
    #endregion �k�N������k

    private Vector3 ConvertInputToCameraDirection(Vector2 input)
    {
        // ����۾����e��V�M�k��V
        Vector3 cameraForward = mainCamera.transform.forward;
        Vector3 cameraRight = mainCamera.transform.right;

        // �����۾���������V
        cameraForward.y = 0;
        cameraRight.y = 0;

        // ���W�Ƥ�V�V�q
        cameraForward.Normalize();
        cameraRight.Normalize();

        // �p��۹��۾������ʤ�V
        Vector3 move = cameraForward * input.y + cameraRight * input.x;
        return move;
    }

    private IEnumerator HandleOffMeshLink()
    {
        // �}�l�ϥ� OffMeshLink�AĲ�o���D�ʵe
        ani.SetTrigger(isJump);

        // ��� OffMeshLink ��T
        OffMeshLinkData data = agent.currentOffMeshLinkData;
        Vector3 startPos = agent.transform.position;
        Vector3 endPos = data.endPos + Vector3.up * agent.baseOffset;

        // �]�w���D����
        float jumpHeight = 2.0f; // �i�H�ھڻݭn�վ���D����
        float duration = 0.8f; // ���D����ɶ�
        float elapsedTime = 0.0f;

        // �T�� NavMeshAgent �H��ʱ�����m
        agent.updatePosition = false;

        //�Ѽư��D
        while (elapsedTime <= duration)
        {
            float t = elapsedTime / duration;
            float height = Mathf.Sin(Mathf.PI * t) * jumpHeight; // �ϥΥ�����Ƽ����ߪ��u�B��
            agent.transform.position = Vector3.Lerp(startPos, endPos, t) + Vector3.up * height;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // ��ʳ]�m NavMeshAgent ����m�í��s�ҥΥ�
        agent.transform.position = endPos;
        agent.updatePosition = true;
        agent.CompleteOffMeshLink();

        // ���m���D�ʵe
        ani.ResetTrigger(isJump);
    }
}