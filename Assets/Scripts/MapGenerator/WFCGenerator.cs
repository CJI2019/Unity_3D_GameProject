using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WFCGenerator : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] Vector3Int mapSize  = new Vector3Int(5, 5, 5);
    [SerializeField] float gridOffset    = 2f; // 타일 간격
    [SerializeField] bool OnNavMeshBuild = false;
    [SerializeField] List<WFCTile> allTiles; // 인스펙터에서 등록할 타일 목록

    DungeonBaker            dungeonBaker;
    GameObject[,,]          spawnedObjects; // 생성된 오브젝트 추적용 배열
    List<WFCTile>[,,]       grid;

    Vector3Int[] directions = {
        Vector3Int.up, Vector3Int.down, 
        Vector3Int.left, Vector3Int.right, 
        Vector3Int.forward, Vector3Int.back 
    };

    void Start()
    {
        dungeonBaker = GetComponent<DungeonBaker>();
        InitializeGrid();
        StartCoroutine(Generate());
    }

    void InitializeGrid()
    {
        grid = new List<WFCTile>[mapSize.x, mapSize.y, mapSize.z];

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                for (int z = 0; z < mapSize.z; z++)
                {
                    InitCoords(new Vector3Int(x, y, z));
                }
            }
        }

        spawnedObjects = new GameObject[mapSize.x, mapSize.y, mapSize.z];

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    void InitCoords(Vector3Int coords)
    {
        grid[coords.x, coords.y, coords.z] = new List<WFCTile>(allTiles);
    }

    IEnumerator Generate()
    {
        int retry = 0, maxRetry = 100;
        while (retry < maxRetry)
        {
            int totalCells = mapSize.x * mapSize.y * mapSize.z;
            int collapsedCount = 0;

            // 첫 엔트로피 셀 임의 지정. 0,0,0 의 셀을 선택한다.
            grid[0,0,0].RemoveAll(tile => tile.tileName == "Wall");   

            while (collapsedCount < totalCells)
            {
                // 엔트로피가 가장 낮은(확정하기 좋은) 셀 찾기
                Vector3Int currentCoords = GetLowestEntropyCell();
                // 만약 찾지 못했거나 이미 다 찼으면 종료
                if (currentCoords == new Vector3Int(-1, -1, -1)) break;
                // 관측(Collapse): 가능한 타일 중 하나를 랜덤으로 선택하여 확정
                CollapseCell(currentCoords);
                // 전파(Propagate): 확정된 셀 때문에 주변 셀들의 가능성 제거
                Propagate(currentCoords);

                collapsedCount++;

                //////////////////////////////////
                // 과정을 눈으로 보기 위해 딜레이
                // DrawMap(); // 맵 그리기
                // yield return new WaitForSeconds(0.005f); 
                //////////////////////////////////
            }
            if (CheckIsolatedIsland())
            {
                Debug.Log("맵 생성 실패: 고립된 섬이 존재합니다. 다시 시도합니다.");
                InitializeGrid();
                ++retry;
            }
            else
            {
                break;
            }
        }

        DrawMap(); // 최종 맵 생성

        if (OnNavMeshBuild)
        {
            yield return null;
            dungeonBaker.BakeMapAsync();
        }

    }

    // 엔트로피가 가장 낮은 셀 좌표 반환 (확정되지 않은 셀 중)
    Vector3Int GetLowestEntropyCell()
    {
        Vector3Int selected = new Vector3Int(-1, -1, -1);
        int minEntropy = int.MaxValue;

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                for (int z = 0; z < mapSize.z; z++)
                {
                    int count = grid[x, y, z].Count;
                    // 1개만 남은건 이미 확정된 것. 1보다 커야 함.
                    if (count > 1 && count < minEntropy)
                    {
                        minEntropy = count;
                        selected = new Vector3Int(x, y, z);
                    }
                }
            }
        }
        return selected;
    }

    void CollapseCell(Vector3Int coords)
    {
        var possibleTiles = grid[coords.x, coords.y, coords.z];
        
        // 맵 범위를 벗어난다면 벗어난 방향으로의 타일 후보는 제거한다.
        List<WFCTile> tempTiles = new List<WFCTile>();
        foreach (var dir in directions)
        {
            Vector3Int neighborCoord = coords + dir;

            if (!IsValidCoord(neighborCoord))
            {
                foreach (var sTile in possibleTiles)
                {
                    if (CheckRoad(sTile, dir))
                    {
                        tempTiles.Add(sTile);
                    }
                }
            }
        }

        foreach (var item in tempTiles)
        {
            possibleTiles.Remove(item);
        }

        if (possibleTiles.Count == 0)
        {
            InitCoords(coords);
            CollapseCell(coords);
            return;
        }

        // 랜덤 선택
        WFCTile selectedTile = possibleTiles[Random.Range(0, possibleTiles.Count)];
        
        possibleTiles.Clear(); 
        possibleTiles.Add(selectedTile); // 해당 셀의 리스트를 선택된 타일 하나로 줄임
    }

    void FindExistTile(List<WFCTile> tileList, string tileName)
    {
        if(tileList.Find(tile => tile.tileName == tileName) != null)
        {
            Debug.Log(tileName + "존재");
        } 
        else
        {
            Debug.Log(tileName + "미존재");
        }
    }

    void Propagate(Vector3Int startCoords)
    {
        // 전파 현재 셀의 상하좌우앞뒤 이웃만 검사

        foreach (var dir in directions)
        {
            Vector3Int neighborCoord = startCoords + dir;

            if (IsValidCoord(neighborCoord))
            {
                UpdateNeighbor(startCoords, neighborCoord, dir);
            }
        }
    }

    // 이웃 셀의 가능성 목록 업데이트
    void UpdateNeighbor(Vector3Int source, Vector3Int target, Vector3Int direction)
    {
        // 이미 확정된(1개만 남은) 셀은 건드리지 않음
        if (grid[target.x, target.y, target.z].Count <= 1) return;

        List<WFCTile> sourceTiles = grid[source.x, source.y, source.z]; // 방금 확정된 타일(들)
        List<WFCTile> targetTiles = grid[target.x, target.y, target.z]; // 검사할 이웃 타일들
        List<WFCTile> toRemove = new List<WFCTile>();

        // 이웃의 모든 가능성(Tile)을 하나씩 검사
        foreach (var tTile in targetTiles)
        {
            bool isCompatible = false;

            // 소스(방금 확정된 곳)의 가능한 타일 중 하나라도 연결 가능하면 생존
            foreach (var sTile in sourceTiles)
            {
                if (CheckSocketCompatibility(sTile, tTile, direction))
                {
                    isCompatible = true;
                    break;
                }
            }

            if (!isCompatible) 
            {
                toRemove.Add(tTile);
            }
        }

        // 불가능한 타일 제거
        foreach (var remove in toRemove)
        {
            targetTiles.Remove(remove);
        }
    }

    // 소켓 문자열 비교 (방향에 따라 반대 소켓을 비교)
    bool CheckSocketCompatibility(WFCTile source, WFCTile target, Vector3Int dir)
    {
        if (dir == Vector3Int.up) return source.up == target.down;
        if (dir == Vector3Int.down) return source.down == target.up;
        if (dir == Vector3Int.left) return source.left == target.right;
        if (dir == Vector3Int.right) return source.right == target.left;
        if (dir == Vector3Int.forward) return source.forward == target.back;
        if (dir == Vector3Int.back) return source.back == target.forward;
        return false;
    }

    bool CheckRoad(WFCTile source, Vector3Int dir)
    {
        if (dir == Vector3Int.up) return source.up == "P";
        if (dir == Vector3Int.down) return source.down == "P";
        if (dir == Vector3Int.left) return source.left == "P";
        if (dir == Vector3Int.right) return source.right == "P";
        if (dir == Vector3Int.forward) return source.forward == "P";
        if (dir == Vector3Int.back) return source.back == "P";
        return false;
    }

    Vector3Int GetOppositeDirection(Vector3Int dir)
    {
        if (dir == Vector3Int.up) return Vector3Int.down;
        if (dir == Vector3Int.down) return Vector3Int.up;
        if (dir == Vector3Int.left) return Vector3Int.right;
        if (dir == Vector3Int.right) return Vector3Int.left;
        if (dir == Vector3Int.forward) return Vector3Int.back;
        if (dir == Vector3Int.back) return Vector3Int.forward;

        Debug.Log("Error: Invalid direction");
        return dir;
    }

    bool IsValidCoord(Vector3Int c)
    {
        return c.x >= 0 && c.x < mapSize.x &&
               c.y >= 0 && c.y < mapSize.y &&
               c.z >= 0 && c.z < mapSize.z;
    }

    void DrawMap()
    {
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                for (int z = 0; z < mapSize.z; z++)
                {
                    var tiles = grid[x, y, z];
                    if (tiles.Count == 1 && spawnedObjects[x, y, z] == null)
                    {
                        // 확정된 타일(혹은 남은 것 중 첫 번째) 생성
                        GameObject go = Instantiate(tiles[0].prefab, 
                            new Vector3(x, y, z) * gridOffset, 
                            Quaternion.identity, transform);
                        spawnedObjects[x, y, z] = go;
                    }
                }
            }
        }
    }

    bool CheckIsolatedIsland()
    {
        bool[,,] visited = new bool[mapSize.x,mapSize.y,mapSize.z];
        int islandCount = 0;

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                for (int z = 0; z < mapSize.z; z++)
                {
                    var tiles = grid[x, y, z];

                    if (!visited[x,y,z] && IsFloor(new Vector3Int(x,y,z)))
                    {
                        ++islandCount;

                        if (islandCount >= 2)
                        {
                            return true;
                        }

                        BFSTileSearch(visited, x, y, z);
                    }
                }
            }
        }

        return false;
    }

    void BFSTileSearch(bool[,,] visited, int x, int y, int z)
    {
        visited[x, y, z] = true;

        Queue<Vector3Int> queue = new Queue<Vector3Int>();
        queue.Enqueue(new Vector3Int(x, y, z));

        while (queue.Count > 0)
        {
            Vector3Int current = queue.Dequeue();
            var currentTiles = grid[current.x, current.y, current.z];
            foreach (var dir in directions)
            {
                Vector3Int neighborCoord = current + dir;

                if (!IsValidCoord(neighborCoord)) continue;
                if(!IsFloor(neighborCoord)) continue;

                var neighborTiles = grid[neighborCoord.x, neighborCoord.y, neighborCoord.z];
                if (visited[neighborCoord.x, neighborCoord.y, neighborCoord.z]) continue;

                //현재 타일에서 이웃 타일로 이동 가능한지 확인
                if (CheckRoad(currentTiles[0],dir) && CheckRoad(neighborTiles[0], GetOppositeDirection(dir)))
                {
                    visited[neighborCoord.x, neighborCoord.y, neighborCoord.z] = true;
                    queue.Enqueue(neighborCoord);
                }
            }
        }
    }

    bool IsFloor(Vector3Int coord)
    {
        var tiles = grid[coord.x, coord.y, coord.z];
        if (tiles.Count == 0) return false;
        return tiles[0].prefab.CompareTag("Floor");
    }

    bool PrintTileNameAndRoad(bool[,,] visited)
    {
        string temp = "";
        string strVisited = "";
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                for (int z = 0; z < mapSize.z; z++)
                {
                    var tiles = grid[x, y, z];
                    temp += tiles[0].prefab.name + "\t";

                    if (!visited[x, y, z] && IsFloor(new Vector3Int(x, y, z)))
                    {
                        strVisited += "1";
                    }
                    else
                    {
                        strVisited += "0";
                    }
                }
            }
            temp += "\n";
            strVisited += "\n";

        }
        Debug.Log(temp);
        Debug.Log("\n");
        Debug.Log(strVisited);
        return false;
    }
}