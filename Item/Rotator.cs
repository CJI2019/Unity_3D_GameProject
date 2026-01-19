using UnityEngine;

public class Rotator : MonoBehaviour
{
    [Tooltip("초당 회전 각도를 의미한다.")]
    [SerializeField] float rotateAngle = 360f;
    [SerializeField] Vector3 rotationAxis = Vector3.up;
    void Update()
    {
        transform.Rotate(rotationAxis * rotateAngle * Time.deltaTime);
    }
}
