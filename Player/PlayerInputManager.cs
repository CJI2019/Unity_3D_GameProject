using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    PlayerInput playerInput;
    public Vector2 move;
    public bool jump = false;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
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
}
