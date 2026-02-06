using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    [SerializeField] bool isCursorLocak = true;
    PlayerInput playerInput;
    public Vector2 move;
    public bool jump = false;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        SetCursorLockMode(isCursorLocak);
    }

    public void OnMove(InputValue value)
    {
        move = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        jump = value.isPressed;
    }

    public Camera GetCamera()
    {
        return playerInput.camera;
    }

    public void SetCursorLockMode(bool isLock)
    {
        Cursor.lockState = isLock ? CursorLockMode.Locked : CursorLockMode.None; // 마우스 고정
        Cursor.visible = !isLock;                   // 커서 숨기기
    }
}
