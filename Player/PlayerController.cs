using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Transform transformPlayerModel;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float rotationSpeed = 10f;
    [SerializeField] float jumpHeight = 10f;
    [SerializeField] float gravity = -15f;
    [SerializeField] float groundOffset = 0.1f;
    [SerializeField] float groundRadius = 0.5f;
    [SerializeField] float airControlFactor = 0.6f;
    [SerializeField] LayerMask groundLayer;

    PlayerInputManager playerInput;
    CharacterController controller;
    Vector3 moveDir;
    Vector3 lastGroundMoveDir;
    Vector3 airVelocity; // 점프 시 수평 속도 
    float verticalVelocity = 0f;
    bool isJump = false;
    bool triggerJump = false;
    bool isGrounded = true;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInputManager>();
    }

    void Update()
    {
        JumpAndGravity();
        GroundedCheck();
        Move();
    }

    void JumpAndGravity()
    {
        isJump = playerInput.jump;

        if (isGrounded && !triggerJump)
        {
            if (isJump)
            {
                lastGroundMoveDir = moveDir; // 점프 입력 직후 마지막 지상 이동 방향 저장

                float g = gravity;
                verticalVelocity = Mathf.Sqrt(2 * -g * jumpHeight); // 등가속도
                airVelocity.Set(moveDir.x * airControlFactor, 0f, moveDir.z * airControlFactor);
                triggerJump = true;
            }
            else
            {
                verticalVelocity = 0f;
                airVelocity.Set(0f, 0f, 0f);
                lastGroundMoveDir.Set(0f,0f,0f);
            }
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
            isJump = false;
            triggerJump = false;
        }
    }

    void GroundedCheck()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - groundOffset, transform.position.z);
        isGrounded = Physics.CheckSphere(spherePosition, groundRadius, groundLayer, QueryTriggerInteraction.Ignore);
    }

    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.75f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.75f);

        if (isGrounded) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - groundOffset, transform.position.z), groundRadius);
    }

    void Move()
    {
        var camera = playerInput.GetCamera();

        Vector3 look = camera.transform.forward;
        Vector3 right = camera.transform.right;

        var frontMove = look * playerInput.move.y;
        var rightMove = right * playerInput.move.x;
        frontMove.y = 0f; rightMove.y = 0f;

        Vector3 velocity;
        moveDir = (frontMove + rightMove).normalized;
        
        if (isGrounded) {
            velocity = moveDir * moveSpeed;
        }
        else 
        { // 공중 이동
            if(lastGroundMoveDir != Vector3.zero)
            {
                velocity = (lastGroundMoveDir + moveDir * airControlFactor).normalized * moveSpeed;   
            }
            else
            {
                velocity = moveDir * airControlFactor * moveSpeed;
            }
        }

        if(playerInput.move == Vector2.zero)
        {
            lastGroundMoveDir.Set(0f,0f,0f);
        }

        velocity.y = verticalVelocity;

        controller.Move(velocity * Time.deltaTime);

        Vector3 flattenMoveDir = new Vector3(moveDir.x, 0f, moveDir.z);
        if (flattenMoveDir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(flattenMoveDir);
            // 부드럽게 회전 적용
            transformPlayerModel.rotation = Quaternion.Slerp(transformPlayerModel.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}