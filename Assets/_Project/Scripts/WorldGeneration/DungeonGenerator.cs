using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonGenerator : MonoBehaviour
{
    [Header("Configuración de Tilemaps")]
    public Tilemap floorTilemap;
    public Tilemap wallTilemap;

    [Header("Los Tiles a usar")]
    public TileBase floorTile;
    public TileBase wallTile;

    [Header("Ajustes del Algoritmo")]
    public int iterations = 10;
    public int walkLength = 50;
    public bool startRandomlyEachIteration = true;

    [Header("Prefabs de Entidades")]
    public GameObject playerPrefab;
    public GameObject enemyPrefab;
    public GameObject exitPrefab;
    public int enemyCount = 5;

    [Header("Prefabs de Puzles")]
    public GameObject logicDoorPrefab;
    public GameObject[] switchPrefabs;
    public GameObject boxPrefab;
    public int numberOfSwitches = 2;

    [Header("Prefabs de Hackeo")]
    public GameObject sciFiDoorPrefab;
    public GameObject hackingTerminalPrefab;

    [Header("Prefabs de Botín")]
    public GameObject chestPrefab;
    public int chestCount = 1;

    private HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
    private List<Vector2Int> availablePositions = new List<Vector2Int>();

    private List<GameObject> activeBoxes = new List<GameObject>();
    private List<Vector3> boxStartPositions = new List<Vector3>();
    private PuzzleSwitch[] activeSwitches;
    private LogicDoor activeDoor;

    private Vector2Int exitPosInt;
    private Vector2Int logicDoorPosInt;
    private bool exitFound = false;
    private Vector2Int sciFiDoorPosInt;
    private Vector2Int terminalPosInt;

    private bool spawnHackingVault = false;

    void Start() { GenerateDungeon(); }

    void Update()
    {
        if (HackingTerminal.isTerminalOpen) return;
        if (Input.GetKeyDown(KeyCode.R)) ResetPuzzle();
    }

    private void GenerateDungeon()
    {
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
        floorPositions.Clear();
        availablePositions.Clear();
        activeBoxes.Clear();
        boxStartPositions.Clear();
        exitFound = false;

        int currentFloor = UIManager.currentFloor;

        if (currentFloor == 1)
        {
            iterations = 5;
            enemyCount = 0;
            numberOfSwitches = Random.Range(1, 3);
            spawnHackingVault = false;
        }
        else if (currentFloor == 2)
        {
            iterations = 8;
            enemyCount = 1;
            numberOfSwitches = 3;
            spawnHackingVault = true;
        }
        else
        {
            iterations = 8 + currentFloor;
            enemyCount = currentFloor - 1;
            numberOfSwitches = Mathf.Clamp(2 + (currentFloor / 2), 3, 5);
            spawnHackingVault = true;
        }

        RunRandomWalk();
        PrepareExitBottleneck();

        Vector2Int vaultCenter = Vector2Int.zero;
        if (spawnHackingVault)
        {
            vaultCenter = BuildHackerVault();
        }

        DrawFloorTiles();
        CreateWalls();

        availablePositions = new List<Vector2Int>(floorPositions);
        SpawnEntities(vaultCenter);
    }

    private void RunRandomWalk()
    {
        Vector2Int currentPosition = Vector2Int.zero;
        for (int i = 0; i < iterations; i++)
        {
            for (int j = 0; j < walkLength; j++)
            {
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        floorPositions.Add(currentPosition + new Vector2Int(x, y));
                    }
                }
                currentPosition += GetRandomDirection() * 2;
            }
            currentPosition = startRandomlyEachIteration ? GetRandomFloorPosition() : Vector2Int.zero;
        }
    }

    private void PrepareExitBottleneck()
    {
        List<Vector2Int> validDoorPositions = new List<Vector2Int>();
        foreach (Vector2Int pos in floorPositions)
        {
            bool isSolidNorth = true;
            for (int x = -1; x <= 1; x++)
            {
                for (int y = 1; y <= 3; y++)
                {
                    if (floorPositions.Contains(pos + new Vector2Int(x, y)))
                    {
                        isSolidNorth = false;
                        break;
                    }
                }
                if (!isSolidNorth) break;
            }

            if (isSolidNorth && floorPositions.Contains(pos + Vector2Int.left) && floorPositions.Contains(pos + Vector2Int.right))
            {
                validDoorPositions.Add(pos);
            }
        }

        if (validDoorPositions.Count > 0)
        {
            Vector2Int chosen = validDoorPositions[Random.Range(0, validDoorPositions.Count)];

            logicDoorPosInt = chosen + Vector2Int.up;
            exitPosInt = chosen + new Vector2Int(0, 2);
            exitFound = true;

            floorPositions.Add(logicDoorPosInt);
            floorPositions.Add(exitPosInt);
        }
    }

    private Vector2Int BuildHackerVault()
    {
        List<Vector2Int> candidates = new List<Vector2Int>();
        foreach (Vector2Int pos in floorPositions)
        {
            bool isSolidNorth = true;
            // Aumentamos la zona de búsqueda para asegurarnos de que cabe una sala 5x5
            for (int x = -3; x <= 3; x++)
            {
                for (int y = 1; y <= 8; y++)
                {
                    if (floorPositions.Contains(pos + new Vector2Int(x, y)))
                    {
                        isSolidNorth = false;
                        break;
                    }
                }
                if (!isSolidNorth) break;
            }

            if (isSolidNorth && (!exitFound || Vector2Int.Distance(pos, logicDoorPosInt) > 10))
            {
                candidates.Add(pos);
            }
        }

        Vector2Int entrance = candidates.Count > 0 ? candidates[Random.Range(0, candidates.Count)] : GetRandomFloorPosition();

        sciFiDoorPosInt = entrance + Vector2Int.up;
        floorPositions.Add(sciFiDoorPosInt);

        terminalPosInt = entrance + new Vector2Int(-1, 1);
        floorPositions.Add(entrance + Vector2Int.left);

        // Pasillo de conexión interior (ahora más largo para conectar con la nueva sala)
        floorPositions.Add(entrance + new Vector2Int(0, 2));
        floorPositions.Add(entrance + new Vector2Int(0, 3));

        // NUEVO: Sala acorazada gigante de 5x5 (Centro desplazado a Y+6)
        Vector2Int vaultCenter = entrance + new Vector2Int(0, 6);
        for (int x = -2; x <= 2; x++)
        {
            for (int y = -2; y <= 2; y++)
            {
                floorPositions.Add(vaultCenter + new Vector2Int(x, y));
            }
        }

        return vaultCenter;
    }

    private void DrawFloorTiles()
    {
        foreach (Vector2Int pos in floorPositions) floorTilemap.SetTile(new Vector3Int(pos.x, pos.y, 0), floorTile);
    }

    private void CreateWalls()
    {
        if (floorPositions.Count == 0) return;

        int minX = int.MaxValue; int maxX = int.MinValue;
        int minY = int.MaxValue; int maxY = int.MinValue;

        foreach (Vector2Int pos in floorPositions)
        {
            if (pos.x < minX) minX = pos.x;
            if (pos.x > maxX) maxX = pos.x;
            if (pos.y < minY) minY = pos.y;
            if (pos.y > maxY) maxY = pos.y;
        }

        int padding = 10;
        minX -= padding; maxX += padding;
        minY -= padding; maxY += padding;

        for (int x = minX; x <= maxX; x++)
        {
            for (int y = minY; y <= maxY; y++)
            {
                Vector2Int currentPos = new Vector2Int(x, y);
                if (!floorPositions.Contains(currentPos))
                {
                    wallTilemap.SetTile(new Vector3Int(x, y, 0), wallTile);
                }
            }
        }
    }

    private void SpawnEntities(Vector2Int vaultCenter)
    {
        if (playerPrefab != null && availablePositions.Count > 0)
        {
            Vector2Int spawnPos = availablePositions[0];
            Instantiate(playerPrefab, new Vector3(spawnPos.x + 0.5f, spawnPos.y + 0.5f, 0f), Quaternion.identity);
            availablePositions.Remove(spawnPos);
        }

        if (exitFound)
        {
            Instantiate(exitPrefab, new Vector3(exitPosInt.x + 0.5f, exitPosInt.y + 0.5f, 0f), Quaternion.identity);
            GameObject lDoorObj = Instantiate(logicDoorPrefab, new Vector3(logicDoorPosInt.x + 0.5f, logicDoorPosInt.y + 0.5f, 0f), Quaternion.identity);
            activeDoor = lDoorObj.GetComponent<LogicDoor>();
            availablePositions.Remove(logicDoorPosInt);
            availablePositions.Remove(exitPosInt);
        }

        if (spawnHackingVault)
        {
            GameObject sfDoorObj = Instantiate(sciFiDoorPrefab, new Vector3(sciFiDoorPosInt.x + 0.5f, sciFiDoorPosInt.y + 0.5f, 0f), Quaternion.identity);

            Vector3 visualOffsetPos = new Vector3(terminalPosInt.x + 0.2f, terminalPosInt.y + 0.85f, 0f);
            GameObject terminalObj = Instantiate(hackingTerminalPrefab, visualOffsetPos, Quaternion.identity);

            HackingTerminal terminalScript = terminalObj.GetComponent<HackingTerminal>();
            if (terminalScript != null) terminalScript.sciFiDoor = sfDoorObj;

            availablePositions.Remove(sciFiDoorPosInt);
            availablePositions.Remove(terminalPosInt);
        }

        if (activeDoor != null && switchPrefabs.Length > 0)
        {
            PuzzleSwitch[] spawnedSwitches = new PuzzleSwitch[numberOfSwitches];

            for (int i = 0; i < numberOfSwitches; i++)
            {
                GameObject prefab = switchPrefabs[Random.Range(0, switchPrefabs.Length)];
                PuzzleSwitch switchData = prefab.GetComponent<PuzzleSwitch>();

                Vector2Int spawnPos;
                Vector3 instantiatePos;

                if (switchData.type == PuzzleSwitch.SwitchType.Melee)
                {
                    // NUEVO: Como la sala ahora llega hasta Y+2, colocamos el interruptor en la nueva pared superior
                    if (spawnHackingVault && i == 0) spawnPos = vaultCenter + new Vector2Int(0, 2);
                    else spawnPos = GetNorthWallPosition();

                    instantiatePos = new Vector3(spawnPos.x + 0.5f, spawnPos.y + 1.5f, 0f);
                }
                else
                {
                    // Estos se quedan igual, pero al ser la sala más ancha, tendrán 1 bloque libre a cada lado
                    if (spawnHackingVault && i == 0) spawnPos = vaultCenter + Vector2Int.left;
                    else spawnPos = GetSafeBoxPosition();

                    instantiatePos = new Vector3(spawnPos.x + 0.5f, spawnPos.y + 0.5f, 0f);

                    if (boxPrefab != null)
                    {
                        Vector2Int boxPos = (spawnHackingVault && i == 0) ? vaultCenter + Vector2Int.right : GetSafeBoxPosition();
                        Vector3 fBoxPos = new Vector3(boxPos.x + 0.5f, boxPos.y + 0.5f, 0f);

                        GameObject box = Instantiate(boxPrefab, fBoxPos, Quaternion.identity);
                        activeBoxes.Add(box);
                        boxStartPositions.Add(fBoxPos);
                    }
                }

                GameObject swObj = Instantiate(prefab, instantiatePos, Quaternion.identity);
                spawnedSwitches[i] = swObj.GetComponent<PuzzleSwitch>();
            }

            activeDoor.requiredSwitches = spawnedSwitches;
            activeSwitches = spawnedSwitches;
        }

        if (chestPrefab != null)
        {
            for (int i = 0; i < chestCount; i++)
            {
                if (availablePositions.Count == 0) break;
                Vector2Int spawnPos = GetSafeBoxPosition();
                if (availablePositions.Contains(spawnPos)) availablePositions.Remove(spawnPos);
                Instantiate(chestPrefab, new Vector3(spawnPos.x + 0.5f, spawnPos.y + 0.5f, 0f), Quaternion.identity);
            }
        }

        if (enemyPrefab != null)
        {
            for (int i = 0; i < enemyCount; i++)
            {
                if (availablePositions.Count == 0) break;
                int r = Random.Range(0, availablePositions.Count);
                Instantiate(enemyPrefab, new Vector3(availablePositions[r].x + 0.5f, availablePositions[r].y + 0.5f, 0f), Quaternion.identity);
                availablePositions.RemoveAt(r);
            }
        }
    }

    private Vector2Int GetRandomDirection()
    {
        Vector2Int[] dir = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        return dir[Random.Range(0, dir.Length)];
    }

    private Vector2Int GetRandomFloorPosition()
    {
        List<Vector2Int> list = new List<Vector2Int>(floorPositions);
        return list[Random.Range(0, list.Count)];
    }

    private Vector2Int GetSafeBoxPosition()
    {
        List<Vector2Int> safePos = new List<Vector2Int>();
        foreach (Vector2Int pos in availablePositions)
        {
            if (floorPositions.Contains(pos + Vector2Int.up) && floorPositions.Contains(pos + Vector2Int.down) &&
                floorPositions.Contains(pos + Vector2Int.left) && floorPositions.Contains(pos + Vector2Int.right))
                safePos.Add(pos);
        }
        if (safePos.Count > 0)
        {
            Vector2Int chosen = safePos[Random.Range(0, safePos.Count)];
            availablePositions.Remove(chosen);
            return chosen;
        }
        return availablePositions[Random.Range(0, availablePositions.Count)];
    }

    private Vector2Int GetNorthWallPosition()
    {
        List<Vector2Int> northWallPos = new List<Vector2Int>();
        foreach (Vector2Int pos in availablePositions)
            if (!floorPositions.Contains(pos + Vector2Int.up)) northWallPos.Add(pos);

        if (northWallPos.Count > 0)
        {
            Vector2Int chosen = northWallPos[Random.Range(0, northWallPos.Count)];
            availablePositions.Remove(chosen);
            return chosen;
        }
        return GetSafeBoxPosition();
    }

    private void ResetPuzzle()
    {
        if (activeDoor != null) activeDoor.ResetDoor();
        if (activeSwitches != null)
        {
            foreach (PuzzleSwitch sw in activeSwitches) if (sw != null) sw.ResetSwitch();
        }
        for (int i = 0; i < activeBoxes.Count; i++)
        {
            if (activeBoxes[i] != null)
            {
                activeBoxes[i].transform.position = boxStartPositions[i];
                Rigidbody2D rb = activeBoxes[i].GetComponent<Rigidbody2D>();
                if (rb != null) { rb.linearVelocity = Vector2.zero; rb.angularVelocity = 0f; }
            }
        }
    }
}