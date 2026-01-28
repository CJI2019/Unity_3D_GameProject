using UnityEngine;

[System.Serializable]
public class WFCTile
{
    public string tileName;
    public GameObject prefab;
    public float weight = 1f; // 출현 빈도 가중치

    // 소켓 정의 (연결 규칙)
    // 예: "A"(길), "B"(벽), "C"(공기)
    // 3차원이므로 6개 면에 대한 소켓이 필요합니다.
    [Header("Sockets (Up, Down, Left, Right, Forward, Back)")]
    public string up;
    public string down;
    public string left;
    public string right;
    public string forward;
    public string back;
}