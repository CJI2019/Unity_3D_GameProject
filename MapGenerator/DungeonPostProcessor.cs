using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonPostProcessor : MonoBehaviour
{
    // 벽으로 메꿀 때 사용할 프리팹 (WFCGenerator에 있는 Wall 프리팹을 연결하세요)
    public GameObject wallPrefab;

    // '바닥'으로 인식할 태그 이름 (Floor, Room 등)
    // 주의: 바닥 프리팹들에 반드시 이 Tag를 설정해야 합니다.
    public string floorTag = "Floor"; 

    // 생성된 맵 오브젝트들을 담고 있는 3차원 배열 (WFCGenerator에서 받아와야 함)
    private GameObject[,,] mapGrid;
    private Vector3Int mapSize;

    // 메인 함수: WFC 생성이 끝나면 이 함수를 호출하세요.
    public void ProcessMap(GameObject[,,] generatedGrid, Vector3Int size)
    {
        mapGrid = generatedGrid;
        mapSize = size;

        // 1. 모든 바닥 그룹(섬) 찾기
        List<List<Vector3Int>> allRegions = GetAllRegions();

        if (allRegions.Count <= 1) 
        {
            Debug.Log("고립된 섬이 없거나, 맵이 비어있습니다.");
            return;
        }

        // 2. 크기순으로 정렬 (내림차순: 큰 것이 0번 인덱스)
        allRegions.Sort((a, b) => b.Count.CompareTo(a.Count));

        // 3. 가장 큰 0번만 남기고, 나머지(1번부터 끝까지)는 벽으로 교체
        int removedCount = 0;
        for (int i = 1; i < allRegions.Count; i++)
        {
            foreach (Vector3Int coord in allRegions[i])
            {
                ReplaceWithWall(coord);
            }
            removedCount++;
        }

        Debug.Log($"정리 완료: {allRegions[0].Count}칸짜리 메인 룸만 남기고 {removedCount}개의 작은 섬을 삭제했습니다.");
    }

    // --- 내부 로직 (BFS: 너비 우선 탐색) ---

    List<List<Vector3Int>> GetAllRegions()
    {
        List<List<Vector3Int>> regions = new List<List<Vector3Int>>();
        bool[,,] visited = new bool[mapSize.x, mapSize.y, mapSize.z];

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                for (int z = 0; z < mapSize.z; z++)
                {
                    // 방문하지 않았고, '바닥'이라면 새로운 탐색 시작
                    if (!visited[x, y, z] && IsFloor(new Vector3Int(x, y, z)))
                    {
                        List<Vector3Int> newRegion = GetRegionTiles(x, y, z, visited);
                        regions.Add(newRegion);
                    }
                }
            }
        }
        return regions;
    }

    List<Vector3Int> GetRegionTiles(int startX, int startY, int startZ, bool[,,] visited)
    {
        List<Vector3Int> tiles = new List<Vector3Int>();
        Queue<Vector3Int> queue = new Queue<Vector3Int>();
        
        Vector3Int startNode = new Vector3Int(startX, startY, startZ);
        queue.Enqueue(startNode);
        visited[startX, startY, startZ] = true;
        tiles.Add(startNode);

        Vector3Int[] directions = {
            Vector3Int.right, Vector3Int.left,
            Vector3Int.up, Vector3Int.down,
            Vector3Int.forward, Vector3Int.back
        };

        while (queue.Count > 0)
        {
            Vector3Int current = queue.Dequeue();
            
            // ★ 1. 현재 타일의 연결 정보를 가져옵니다.
            GameObject currentObj = mapGrid[current.x, current.y, current.z];
            TileConnectivity currentConn = currentObj.GetComponent<TileConnectivity>();

            foreach (Vector3Int dir in directions)
            {
                Vector3Int neighbor = current + dir;

                // 맵 범위 확인
                if (!IsValid(neighbor)) continue;

                // 방문 여부 확인
                if (visited[neighbor.x, neighbor.y, neighbor.z]) continue;

                // 바닥(Floor) 태그인지 확인
                if (!IsFloor(neighbor)) continue;

                // ★ 2. 여기서 '고립' 여부를 판별합니다!
                // "현재 타일에서 해당 방향(dir)으로 나갈 수 있는가?"를 체크합니다.
                // WFC 규칙상 내 쪽이 뚫려있으면, 상대방 쪽도 무조건 뚫려있으므로(P-P 연결), 내 쪽만 검사하면 됩니다.
                if (currentConn != null && currentConn.CanCross(dir) == false)
                {
                    // 벽으로 막혀있음 -> 연결된 방이 아님 -> 스킵
                    continue;
                }

                // 모든 조건을 통과했으므로 같은 방으로 인정
                visited[neighbor.x, neighbor.y, neighbor.z] = true;
                tiles.Add(neighbor);
                queue.Enqueue(neighbor);
            }
        }
        return tiles;
    }
    // 해당 좌표가 바닥인지 확인 (태그로 구분하거나 이름으로 구분)
    bool IsFloor(Vector3Int coord)
    {
        GameObject obj = mapGrid[coord.x, coord.y, coord.z];
        if (obj == null) return false;
        
        // 방법 1: Tag 비교 (Inspector에서 프리팹 Tag 설정 필수)
        return obj.CompareTag(floorTag);
        
        // 방법 2: 이름에 "Floor"가 포함되는지 비교 (Tag 쓰기 싫을 때)
        // return obj.name.Contains("Floor"); 
    }

    bool IsValid(Vector3Int c)
    {
        return c.x >= 0 && c.x < mapSize.x &&
               c.y >= 0 && c.y < mapSize.y &&
               c.z >= 0 && c.z < mapSize.z;
    }

    void ReplaceWithWall(Vector3Int coord)
    {
        // 기존 바닥 삭제
        Destroy(mapGrid[coord.x, coord.y, coord.z]);

        // // 벽 생성
        // GameObject newWall = Instantiate(wallPrefab, new Vector3(coord.x, coord.y, coord.z) * 2f, Quaternion.identity); // * 2f는 그리드 간격(gridOffset)
        // newWall.transform.parent = this.transform; // 정리용 부모 설정
        
        // // 그리드 데이터 갱신
        // mapGrid[coord.x, coord.y, coord.z] = newWall;
    }
}