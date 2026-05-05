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
    [Tooltip("Añade aquí tanto el Botón como la Palanca")]
    public GameObject[] switchPrefabs;
    public GameObject boxPrefab;
    public int numberOfSwitches = 2;

    private HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
    private List<Vector2Int> availablePositions = new List<Vector2Int>();

    // --- MEMORIA PARA EL RESET DEL PUZLE ---
    private List<GameObject> activeBoxes = new List<GameObject>();
    private List<Vector3> boxStartPositions = new List<Vector3>();
    private PuzzleSwitch[] activeSwitches;
    private LogicDoor activeDoor;

    void Start()
    {
        GenerateDungeon();
    }

    // --- NUEVO: ESCUCHAR LA TECLA R ---
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetPuzzle();
        }
    }

    private void GenerateDungeon()
    {
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
        floorPositions.Clear();
        availablePositions.Clear();

        // Limpiamos la memoria del puzle anterior
        activeBoxes.Clear();
        boxStartPositions.Clear();
        activeSwitches = null;
        activeDoor = null;

        RunRandomWalk();
        DrawFloorTiles();
        CreateWalls();

        availablePositions = new List<Vector2Int>(floorPositions);

        if (playerPrefab != null && availablePositions.Count > 0)
        {
            Vector2Int spawnPos = availablePositions[0];
            Vector3 playerSpawnPos = new Vector3(spawnPos.x + 0.5f, spawnPos.y + 0.5f, 0f);
            Instantiate(playerPrefab, playerSpawnPos, Quaternion.identity);
            availablePositions.Remove(spawnPos);
        }

        SpawnExitAndPuzzle();
        SpawnEnemies();
    }

    private void RunRandomWalk()
    {
        Vector2Int currentPosition = Vector2Int.zero;
        for (int i = 0; i < iterations; i++)
        {
            for (int j = 0; j < walkLength; j++)
            {
                floorPositions.Add(currentPosition);
                floorPositions.Add(currentPosition + Vector2Int.up);
                floorPositions.Add(currentPosition + Vector2Int.right);
                floorPositions.Add(currentPosition + new Vector2Int(1, 1));

                currentPosition += GetRandomDirection();
            }
            currentPosition = startRandomlyEachIteration ? GetRandomFloorPosition() : Vector2Int.zero;
        }
    }

    private Vector2Int GetRandomDirection()
    {
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        return directions[Random.Range(0, directions.Length)];
    }

    private Vector2Int GetRandomFloorPosition()
    {
        List<Vector2Int> list = new List<Vector2Int>(floorPositions);
        return list[Random.Range(0, list.Count)];
    }

    private void DrawFloorTiles()
    {
        foreach (Vector2Int position in floorPositions)
        {
            floorTilemap.SetTile(new Vector3Int(position.x, position.y, 0), floorTile);
        }
    }

    private void CreateWalls()
    {
        HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>();
        foreach (Vector2Int position in floorPositions)
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0) continue;
                    Vector2Int neighbor = new Vector2Int(position.x + x, position.y + y);
                    if (!floorPositions.Contains(neighbor))
                    {
                        wallPositions.Add(neighbor);
                    }
                }
            }
        }

        foreach (Vector2Int position in wallPositions)
        {
            wallTilemap.SetTile(new Vector3Int(position.x, position.y, 0), wallTile);
        }
    }

    private Vector2Int GetSafeBoxPosition()
    {
        List<Vector2Int> safePos = new List<Vector2Int>();
        foreach (Vector2Int pos in availablePositions)
        {
            if (floorPositions.Contains(pos + Vector2Int.up) &&
                floorPositions.Contains(pos + Vector2Int.down) &&
                floorPositions.Contains(pos + Vector2Int.left) &&
                floorPositions.Contains(pos + Vector2Int.right))
            {
                safePos.Add(pos);
            }
        }

        if (safePos.Count > 0)
        {
            int r = Random.Range(0, safePos.Count);
            Vector2Int chosen = safePos[r];
            availablePositions.Remove(chosen);
            return chosen;
        }

        int fallbackIndex = Random.Range(0, availablePositions.Count);
        Vector2Int fallback = availablePositions[fallbackIndex];
        availablePositions.RemoveAt(fallbackIndex);
        return fallback;
    }

    private Vector2Int GetNorthWallPosition()
    {
        List<Vector2Int> northWallPos = new List<Vector2Int>();
        foreach (Vector2Int pos in availablePositions)
        {
            if (!floorPositions.Contains(pos + Vector2Int.up))
            {
                northWallPos.Add(pos);
            }
        }

        if (northWallPos.Count > 0)
        {
            int r = Random.Range(0, northWallPos.Count);
            Vector2Int chosen = northWallPos[r];
            availablePositions.Remove(chosen);
            return chosen;
        }

        return GetSafeBoxPosition();
    }

    private void SpawnExitAndPuzzle()
    {
        if (exitPrefab == null || logicDoorPrefab == null || switchPrefabs.Length == 0) return;

        List<Vector2Int> validDoorPositions = new List<Vector2Int>();
        foreach (Vector2Int floorPos in floorPositions)
        {
            Vector2Int wallAbove = floorPos + Vector2Int.up;
            Vector2Int floorBelow = floorPos + Vector2Int.down;

            if (!floorPositions.Contains(wallAbove) && floorPositions.Contains(floorBelow))
            {
                validDoorPositions.Add(floorPos);
            }
        }

        if (validDoorPositions.Count > 0)
        {
            int randomIndex = Random.Range(0, validDoorPositions.Count);
            Vector2Int doorPosInt = validDoorPositions[randomIndex];
            Vector2Int exitPosInt = doorPosInt + Vector2Int.up;

            Vector2Int[] wallsToForce = {
                doorPosInt + Vector2Int.left,
                doorPosInt + Vector2Int.right,
                exitPosInt + Vector2Int.left,
                exitPosInt + Vector2Int.right
            };

            foreach (Vector2Int w in wallsToForce)
            {
                floorPositions.Remove(w);
                availablePositions.Remove(w);
                floorTilemap.SetTile(new Vector3Int(w.x, w.y, 0), null);
                wallTilemap.SetTile(new Vector3Int(w.x, w.y, 0), wallTile);
            }

            wallTilemap.SetTile(new Vector3Int(exitPosInt.x, exitPosInt.y, 0), null);
            Instantiate(exitPrefab, new Vector3(exitPosInt.x + 0.5f, exitPosInt.y + 0.5f, 0f), Quaternion.identity);

            GameObject doorObj = Instantiate(logicDoorPrefab, new Vector3(doorPosInt.x + 0.5f, doorPosInt.y + 0.5f, 0f), Quaternion.identity);
            LogicDoor logicDoorScript = doorObj.GetComponent<LogicDoor>();

            // GUARDAMOS LA PUERTA EN MEMORIA
            activeDoor = logicDoorScript;

            availablePositions.Remove(doorPosInt);

            PuzzleSwitch[] spawnedSwitches = new PuzzleSwitch[numberOfSwitches];

            for (int i = 0; i < numberOfSwitches; i++)
            {
                GameObject prefabToSpawn = switchPrefabs[Random.Range(0, switchPrefabs.Length)];
                PuzzleSwitch switchData = prefabToSpawn.GetComponent<PuzzleSwitch>();

                Vector2Int spawnPos;
                Vector3 finalInstantiatePos;

                if (switchData.type == PuzzleSwitch.SwitchType.Melee)
                {
                    spawnPos = GetNorthWallPosition();
                    finalInstantiatePos = new Vector3(spawnPos.x + 0.5f, spawnPos.y + 1.5f, 0f);
                }
                else
                {
                    spawnPos = GetSafeBoxPosition();
                    finalInstantiatePos = new Vector3(spawnPos.x + 0.5f, spawnPos.y + 0.5f, 0f);

                    Vector2Int boxPos = GetSafeBoxPosition();
                    if (boxPrefab != null)
                    {
                        Vector3 finalBoxPos = new Vector3(boxPos.x + 0.5f, boxPos.y + 0.5f, 0f);
                        GameObject box = Instantiate(boxPrefab, finalBoxPos, Quaternion.identity);

                        // GUARDAMOS LA CAJA Y SU POSICIÓN ORIGINAL EN MEMORIA
                        activeBoxes.Add(box);
                        boxStartPositions.Add(finalBoxPos);
                    }
                }

                GameObject swObj = Instantiate(prefabToSpawn, finalInstantiatePos, Quaternion.identity);
                spawnedSwitches[i] = swObj.GetComponent<PuzzleSwitch>();
            }

            logicDoorScript.requiredSwitches = spawnedSwitches;

            // GUARDAMOS LOS INTERRUPTORES EN MEMORIA
            activeSwitches = spawnedSwitches;
        }
    }

    private void SpawnEnemies()
    {
        if (enemyPrefab == null) return;
        for (int i = 0; i < enemyCount; i++)
        {
            if (availablePositions.Count == 0) break;
            int randomIndex = Random.Range(0, availablePositions.Count);
            Vector2Int spawnPos = availablePositions[randomIndex];
            Instantiate(enemyPrefab, new Vector3(spawnPos.x + 0.5f, spawnPos.y + 0.5f, 0f), Quaternion.identity);
            availablePositions.RemoveAt(randomIndex);
        }
    }

    // --- NUEVO: FUNCIÓN QUE EJECUTA EL RESET ---
    private void ResetPuzzle()
    {
        // 1. Cerramos la puerta
        if (activeDoor != null)
        {
            activeDoor.ResetDoor();
        }

        // 2. Apagamos todos los interruptores y palancas
        if (activeSwitches != null)
        {
            foreach (PuzzleSwitch sw in activeSwitches)
            {
                if (sw != null) sw.ResetSwitch();
            }
        }

        // 3. Teletransportamos las cajas a su origen y frenamos su inercia
        for (int i = 0; i < activeBoxes.Count; i++)
        {
            if (activeBoxes[i] != null)
            {
                activeBoxes[i].transform.position = boxStartPositions[i];

                Rigidbody2D rb = activeBoxes[i].GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector2.zero; // Frenamos la caja en seco
                    rb.angularVelocity = 0f;
                }
            }
        }

        Debug.Log("¡Puzle Reiniciado!");
    }
}