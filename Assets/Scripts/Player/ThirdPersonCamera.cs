using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCamera : MonoBehaviour
{
    [SerializeField] InputActionReference lookActionReference;
    [SerializeField] Transform target;          // 따라갈 플레이어
    [SerializeField] float distance = 5.0f;     // 플레이어와 카메라 거리
    [SerializeField] float sensitivity = 2.0f;  // 마우스 감도
    [SerializeField] float minY = -20f;         // 최소 Pitch
    [SerializeField] float maxY = 80f;          // 최대 Pitch
    float rotX = 0f;          // Yaw (좌우)
    float rotY = 0f;          // Pitch (상하)

    InputAction lookAction;

    void OnEnable()
    {
        lookAction = lookActionReference.action;
        lookAction.Enable();
    }

    void OnDisable()
    {
        lookAction.Disable();        
    }

    void LateUpdate()
    {
        CameraUpdate();
    }

    void CameraUpdate()
    {
        // 마우스 입력값 읽기 (Vector2)
        Vector2 lookInput = lookAction.ReadValue<Vector2>();

        rotX += lookInput.x * sensitivity * Time.deltaTime;
        rotY -= lookInput.y * sensitivity * Time.deltaTime;

        rotY = Mathf.Clamp(rotY, minY, maxY);

        // 회전 적용
        Quaternion rotation = Quaternion.Euler(rotY, rotX, 0);
        Vector3 position = target.position - rotation * Vector3.forward * distance;

        transform.rotation = rotation;
        transform.position = position;
    }
}
