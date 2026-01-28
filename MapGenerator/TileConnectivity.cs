using UnityEngine;

public class TileConnectivity : MonoBehaviour
{
    // 각 방향으로 이동이 가능한지 여부 (True: 뚫림, False: 막힘)
    public bool up;
    public bool down;
    public bool left;
    public bool right;
    public bool forward;
    public bool back;

    // 방향 벡터를 입력받아 이동 가능 여부를 반환하는 함수
    public bool CanCross(Vector3Int direction)
    {
        if (direction == Vector3Int.up) return up;
        if (direction == Vector3Int.down) return down;
        if (direction == Vector3Int.left) return left;
        if (direction == Vector3Int.right) return right;
        if (direction == Vector3Int.forward) return forward;
        if (direction == Vector3Int.back) return back;
        return false;
    }

    public bool CheckWall(Vector3Int direction)
    {
        if (direction == Vector3Int.up) return down;
        if (direction == Vector3Int.down) return up;
        if (direction == Vector3Int.left) return right;
        if (direction == Vector3Int.right) return left;
        if (direction == Vector3Int.forward) return back;
        if (direction == Vector3Int.back) return forward;
        return false;
    }
}