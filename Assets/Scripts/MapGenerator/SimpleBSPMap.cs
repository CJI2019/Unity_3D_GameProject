using System.Collections;
using UnityEngine;

public class SimpleBSPDungeon : MonoBehaviour
{
    [Header("Dungeon Settings")]
    public int mapWidth = 50;       // 전체 맵 너비
    public int mapHeight = 50;      // 전체 맵 깊이 (3D에서는 Z축)
    public int minNodeSize = 10;    // 분할될 공간의 최소 크기
    public int minRoomSize = 6;     // 방의 최소 크기 (Node보다 작아야 함)

    [Header("Prefabs")]
    public GameObject floorPrefab;  // 바닥 프리팹 (1x1 크기 권장)
    public GameObject wallPrefab;   // 벽 프리팹 (1x1 크기 권장)

    private int[,] mapData;         // 0: 벽, 1: 바닥
    private int maxDepth = 0;

    // 공간을 나타내는 노드 클래스
    public class DungeonNode
    {
        public int x, y, width, height;
        public DungeonNode leftNode;
        public DungeonNode rightNode;
        public RectInt roomRect; // 실제 방의 좌표와 크기
        public int depth;

        public DungeonNode(int x, int y, int width, int height,int depth)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.depth = depth;
        }

        public bool IsLeaf() => leftNode == null && rightNode == null;
    }

    void Start()
    {
        GenerateDungeon();
    }

    void Update()
    {
        // 스페이스바를 누르면 재생성 (테스트용)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GenerateDungeon();
        }
    }

    public void GenerateDungeon()
    {
        // 1. 맵 데이터 및 이전 오브젝트 초기화
        mapData = new int[mapWidth, mapHeight];
        foreach (Transform child in transform) Destroy(child.gameObject);

        // 2. 루트 노드 생성 (전체 맵 크기)
        DungeonNode rootNode = new DungeonNode(0, 0, mapWidth, mapHeight,0);

        // 3. 공간 분할 (재귀 함수)
        SplitSpace(rootNode,1);

        // 4. 방 생성 및 복도 연결
        CreateRoomsAndCorridors(rootNode);
        // 5. 맵 데이터를 기반으로 프리팹 생성 (시각화)
        // InstantiateDungeonOptimized();
        StartCoroutine(CreateAndCombineRoutine());
    }

    // 공간 분할 함수
    void SplitSpace(DungeonNode node,int currentDepth)
    {
        // 더 이상 쪼갤 수 없는 크기라면 중단 (종료 조건)
        if (node.width <= minNodeSize * 2 && node.height <= minNodeSize * 2) return;

        // 가로로 자를지 세로로 자를지 랜덤 결정 (50:50)
        // 단, 한쪽이 너무 길쭉하면 강제로 반대 방향으로 자르게 할 수도 있음
        bool splitHorizontally = Random.value > 0.5f;
        
        // 너비가 너무 넓으면 세로로 자르도록 유도
        if (node.width > node.height && node.width / node.height >= 1.25) splitHorizontally = false;
        // 높이가 너무 높으면 가로로 자르도록 유도
        else if (node.height > node.width && node.height / node.width >= 1.25) splitHorizontally = true;

        int max = (splitHorizontally ? node.height : node.width) - minNodeSize;
        
        // 쪼갤 공간이 부족하면 중단
        if (max < minNodeSize) return;

        // 분할 지점 랜덤 설정
        int split = Random.Range(minNodeSize, max);

        if (splitHorizontally)
        {
            // 가로 분할 (위/아래)
            node.leftNode = new DungeonNode(node.x, node.y, node.width, split,currentDepth);
            node.rightNode = new DungeonNode(node.x, node.y + split, node.width, node.height - split,currentDepth);
        }
        else
        {
            // 세로 분할 (좌/우)
            node.leftNode = new DungeonNode(node.x, node.y, split, node.height,currentDepth);
            node.rightNode = new DungeonNode(node.x + split, node.y, node.width - split, node.height,currentDepth);
        }

        // 자식 노드들도 재귀적으로 분할 시도
        SplitSpace(node.leftNode,currentDepth + 1);
        SplitSpace(node.rightNode,currentDepth + 1);

        maxDepth = Mathf.Max(currentDepth,maxDepth);
    }

    // 방과 복도 생성
    void CreateRoomsAndCorridors(DungeonNode node)
    {
        // 리프 노드(자식이 없는 노드)라면 방을 만든다
        if (node.IsLeaf())
        {
            // 구역 내에서 랜덤한 크기와 위치의 방 생성 (여백을 둠)
            int roomW = Random.Range(minRoomSize, node.width - 2);
            int roomH = Random.Range(minRoomSize, node.height - 2);
            int roomX = node.x + Random.Range(1, node.width - roomW - 1);
            int roomY = node.y + Random.Range(1, node.height - roomH - 1);

            node.roomRect = new RectInt(roomX, roomY, roomW, roomH);

            // 맵 데이터에 방 영역을 1(바닥)로 기록
            FillMap(node.roomRect, 1);
        }
        else
        {
            // 자식이 있다면 자식부터 처리
            if (node.leftNode != null) CreateRoomsAndCorridors(node.leftNode);
            if (node.rightNode != null) CreateRoomsAndCorridors(node.rightNode);

            // 두 자식 노드를 연결하는 복도 생성
            if (node.leftNode != null && node.rightNode != null)
            {
                ConnectNodes(node.leftNode, node.rightNode);
            }
        }
    }

    void ConnectNodes(DungeonNode nodeA, DungeonNode nodeB)
    {
        // 각 노드(혹은 그 자식의 방)의 중심점 찾기
        Vector2Int centerA = GetRoomCenter(nodeA);
        Vector2Int centerB = GetRoomCenter(nodeB);

        // 중심점을 잇는 'ㄱ'자 복도 생성
        // 수평 이동 후 수직 이동 (혹은 반대)
        
        // 1. 수평 통로 (X축 이동)
        int xStart = Mathf.Min(centerA.x, centerB.x);
        int xEnd = Mathf.Max(centerA.x, centerB.x);
        for (int x = xStart; x <= xEnd; x++)
        {
            mapData[x, centerA.y] = 1;
            if(centerA.y > 0) mapData[x, centerA.y - 1] = 1;
            if(centerA.y < mapHeight - 1) mapData[x, centerA.y + 1] = 1;
        }

        // 2. 수직 통로 (Y축 이동) - B의 X좌표에서 이동
        int yStart = Mathf.Min(centerA.y, centerB.y);
        int yEnd = Mathf.Max(centerA.y, centerB.y);
        for (int y = yStart; y <= yEnd; y++)
        {
            mapData[centerB.x, y] = 1;
            if(centerB.x > 0) mapData[centerB.x - 1, y] = 1;
            if(centerB.x < mapWidth - 1) mapData[centerB.x + 1, y] = 1;
        }
    }

    // 재귀적으로 내려가서 실제 방의 중심점을 찾아오는 헬퍼 함수
    Vector2Int GetRoomCenter(DungeonNode node)
    {
        if (node.IsLeaf())
        {
            return new Vector2Int((int)node.roomRect.center.x, (int)node.roomRect.center.y);
        }
        else
        {
            // 리프가 아니면 왼쪽 자식의 중심을 대표로 씀 (랜덤하게 오른쪽 써도 됨)
            return GetRoomCenter(node.leftNode);
        }
    }

    void FillMap(RectInt rect, int value)
    {
        for (int x = rect.x; x < rect.x + rect.width; x++)
        {
            for (int y = rect.y; y < rect.y + rect.height; y++)
            {
                mapData[x, y] = value;
            }
        }
    }

    void InstantiateDungeon()
    {
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                // 3D 공간이므로 y좌표는 z축으로 활용
                Vector3 pos = new Vector3(x, 0, y);

                if (mapData[x, y] == 1)
                {
                    Instantiate(floorPrefab, pos, Quaternion.identity, transform);
                }
                else
                {
                    // 벽은 1층 높이로 쌓거나 필요 시 생성
                    // 여기서는 맵 전체를 채우는 벽이 아니라, 그냥 빈 공간을 둘지 벽을 세울지 결정
                    // 보통 로그라이크는 빈 곳을 벽으로 채움
                    Instantiate(wallPrefab, pos + Vector3.up * 0.5f, Quaternion.identity, transform);
                }
            }
        }
    }

    // 최적화된 프리팹 생성
    void InstantiateDungeonOptimized()
    {
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                Vector3 pos = new Vector3(x, 0, y);

                // 1. 해당 타일이 바닥인 경우 -> 무조건 바닥 생성
                if (mapData[x, y] == 1)
                {
                    Instantiate(floorPrefab, pos, Quaternion.identity, transform);
                }
                // 2. 해당 타일이 빈 공간(0)인 경우 -> 주변 확인
                else
                {
                    // 주변에 바닥이 하나라도 붙어있어야 벽을 세운다.
                    if (HasAdjacentFloor(x, y))
                    {
                        // 벽은 바닥보다 약간 높게(0.5f) 배치하여 겹침 방지 및 시각적 구분
                        Instantiate(wallPrefab, pos + Vector3.up * 0.5f, Quaternion.identity, transform);
                    }
                    // 주변에 바닥이 없는 순수한 빈 공간이면 아무것도 생성하지 않음 (내부 벽 제거 효과)
                }
            }
        }
    }
    // --- 최적화된 생성 로직 ---

    IEnumerator CreateAndCombineRoutine()
    {
        // 임시 컨테이너 생성
        GameObject tempFloorParent = new GameObject("TempFloor");
        GameObject tempWallParent = new GameObject("TempWall");
        tempFloorParent.transform.parent = transform;
        tempWallParent.transform.parent = transform;

        // 1. 오브젝트 배치 (비활성 상태로 생성하여 성능 확보)
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                Vector3 pos = new Vector3(x, 0, y);
                if (mapData[x, y] == 1)
                    Instantiate(floorPrefab, pos, Quaternion.identity, tempFloorParent.transform);
                else if (HasAdjacentFloor(x, y))
                    Instantiate(wallPrefab, pos + Vector3.up * 0.5f, Quaternion.identity, tempWallParent.transform);
            }
        }

        // 한 프레임 대기 (생성 완료 보장)
        yield return null;

        // 2. 메쉬 결합
        Combine(tempFloorParent, "CombinedFloor", floorPrefab.GetComponentInChildren<MeshRenderer>().sharedMaterial);
        Combine(tempWallParent, "CombinedWall", wallPrefab.GetComponentInChildren<MeshRenderer>().sharedMaterial);

        // 3. 임시 개별 오브젝트들 삭제
        Destroy(tempFloorParent);
        Destroy(tempWallParent);
    }

    void Combine(GameObject parent, string name, Material mat)
    {
        MeshFilter[] filters = parent.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[filters.Length];

        for (int i = 0; i < filters.Length; i++)
        {
            combine[i].mesh = filters[i].sharedMesh;
            combine[i].transform = filters[i].transform.localToWorldMatrix;
        }

        GameObject combinedObj = new GameObject(name);
        combinedObj.transform.parent = transform;
        
        MeshFilter mf = combinedObj.AddComponent<MeshFilter>();
        mf.mesh = new Mesh();
        mf.mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; // 대량의 정점 허용
        mf.mesh.CombineMeshes(combine);

        MeshRenderer mr = combinedObj.AddComponent<MeshRenderer>();
        mr.sharedMaterial = mat;
        
        // 충돌을 위한 메쉬 콜라이더 추가 (선택 사항)
        combinedObj.AddComponent<MeshCollider>().sharedMesh = mf.mesh;
    }

    bool HasAdjacentFloor(int x, int y)
    {
        int[] dx = { 0, 0, -1, 1, 1, 1, -1, -1 }; // 대각선 포함 8방향 체크 시 더 촘촘함
        int[] dy = { 1, -1, 0, 0, 1, -1, 1, -1 };
        for (int i = 0; i < 8; i++) {
            int nx = x + dx[i], ny = y + dy[i];
            if (nx >= 0 && nx < mapWidth && ny >= 0 && ny < mapHeight && mapData[nx, ny] == 1) return true;
        }
        return false;
    }
}