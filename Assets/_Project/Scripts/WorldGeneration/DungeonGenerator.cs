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

    [Header("Prefabs de Botín")] // NUEVO
    public GameObject chestPrefab;
    public int chestCount = 1; // Cuántos cofres aparecen por nivel

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
        // Ignorar el reinicio si el jugador está hackeando
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

        // --- SISTEMA DE DIFICULTAD ---
        int currentFloor = UIManager.currentFloor;

        if (currentFloor == 1)
        {
            iterations = 5; // Mapa pequeño
            enemyCount = 0; // Sin enemigos
            numberOfSwitches = Random.Range(1, 3); // 1 o 2 switches
            spawnHackingVault = false; // Sin terminal de hackeo
        }
        else if (currentFloor == 2)
        {
            iterations = 8; // Mapa mediano
            enemyCount = 1; // 1 enemigo introductorio
            numberOfSwitches = 3;
            spawnHackingVault = true; // Empieza el hackeo
        }
        else
        {
            iterations = 8 + currentFloor; // Crece cada nivel
            enemyCount = currentFloor - 1; // Más enemigos
            numberOfSwitches = Mathf.Clamp(2 + (currentFloor / 2), 3, 5); // Tope en 5
            spawnHackingVault = true;
        }

        // 1. DIBUJAR LOS PLANOS (Topología)
        RunRandomWalk();
        PrepareExitBottleneck();

        Vector2Int vaultCenter = Vector2Int.zero;
        if (spawnHackingVault)
        {
            vaultCenter = BuildHackerVault();
        }

        // 2. CONSTRUIR MAPA FÍSICO 
        DrawFloorTiles();
        CreateWalls();

        // 3. LIMPIAR MUROS
        CleanDoorsWalls();

        // 4. COLOCAR OBJETOS
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
                floorPositions.Add(currentPosition);
                floorPositions.Add(currentPosition + Vector2Int.up);
                floorPositions.Add(currentPosition + Vector2Int.right);
                floorPositions.Add(currentPosition + new Vector2Int(1, 1));
                currentPosition += GetRandomDirection();
            }
            currentPosition = startRandomlyEachIteration ? GetRandomFloorPosition() : Vector2Int.zero;
        }
    }

    private void PrepareExitBottleneck()
    {
        List<Vector2Int> validDoorPositions = new List<Vector2Int>();
        foreach (Vector2Int pos in floorPositions)
        {
            if (!floorPositions.Contains(pos + Vector2Int.up) && floorPositions.Contains(pos + Vector2Int.down))
                validDoorPositions.Add(pos);
        }

        if (validDoorPositions.Count > 0)
        {
            logicDoorPosInt = validDoorPositions[Random.Range(0, validDoorPositions.Count)];
            exitPosInt = logicDoorPosInt + Vector2Int.up;
            exitFound = true;

            Vector2Int[] wallsToForce = { logicDoorPosInt + Vector2Int.left, logicDoorPosInt + Vector2Int.right, exitPosInt + Vector2Int.left, exitPosInt + Vector2Int.right };
            foreach (Vector2Int w in wallsToForce) floorPositions.Remove(w);

            floorPositions.Add(exitPosInt);
        }
    }

    private Vector2Int BuildHackerVault()
    {
        Vector2Int entrance = Vector2Int.zero;
        bool found = false;

        List<Vector2Int> candidates = new List<Vector2Int>(floorPositions);
        int startIndex = Random.Range(0, candidates.Count);

        for (int i = 0; i < candidates.Count; i++)
        {
            Vector2Int pos = candidates[(startIndex + i) % candidates.Count];

            if (exitFound && Vector2Int.Distance(pos, logicDoorPosInt) < 8) continue;

            bool isAreaClear = true;
            for (int x = -3; x <= 3; x++)
            {
                for (int y = 1; y <= 7; y++)
                {
                    if (floorPositions.Contains(pos + new Vector2Int(x, y)))
                    {
                        isAreaClear = false;
                        break;
                    }
                }
                if (!isAreaClear) break;
            }

            if (isAreaClear)
            {
                entrance = pos;
                found = true;
                break;
            }
        }

        if (!found) entrance = GetRandomFloorPosition();

        for (int x = -3; x <= 3; x++)
        {
            for (int y = 1; y <= 7; y++) floorPositions.Remove(entrance + new Vector2Int(x, y));
        }

        for (int x = -2; x <= 2; x++)
        {
            for (int y = 2; y <= 6; y++) floorPositions.Add(entrance + new Vector2Int(x, y));
        }

        sciFiDoorPosInt = entrance + Vector2Int.up;
        floorPositions.Add(sciFiDoorPosInt);

        terminalPosInt = entrance + Vector2Int.right;
        floorPositions.Add(terminalPosInt);

        return entrance + new Vector2Int(0, 4);
    }

    private void DrawFloorTiles()
    {
        foreach (Vector2Int pos in floorPositions) floorTilemap.SetTile(new Vector3Int(pos.x, pos.y, 0), floorTile);
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
                    if (!floorPositions.Contains(neighbor)) wallPositions.Add(neighbor);
                }
            }
        }
        foreach (Vector2Int pos in wallPositions) wallTilemap.SetTile(new Vector3Int(pos.x, pos.y, 0), wallTile);
    }

    private void CleanDoorsWalls()
    {
        if (exitFound) wallTilemap.SetTile(new Vector3Int(exitPosInt.x, exitPosInt.y, 0), null);
        if (spawnHackingVault) wallTilemap.SetTile(new Vector3Int(sciFiDoorPosInt.x, sciFiDoorPosInt.y, 0), null);
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
            GameObject terminalObj = Instantiate(hackingTerminalPrefab, new Vector3(terminalPosInt.x + 0.5f, terminalPosInt.y + 0.5f, 0f), Quaternion.identity);

            HackingTerminal terminalScript = terminalObj.GetComponent<HackingTerminal>();
            if (terminalScript != null) terminalScript.sciFiDoor = sfDoorObj;
            availablePositions.Remove(sciFiDoorPosInt);
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
                    if (spawnHackingVault && i == 0) spawnPos = vaultCenter + new Vector2Int(0, 2);
                    else spawnPos = GetNorthWallPosition();

                    instantiatePos = new Vector3(spawnPos.x + 0.5f, spawnPos.y + 1.5f, 0f);
                }
                else
                {
                    if (spawnHackingVault && i == 0) spawnPos = vaultCenter;
                    else spawnPos = GetSafeBoxPosition();

                    instantiatePos = new Vector3(spawnPos.x + 0.5f, spawnPos.y + 0.5f, 0f);

                    if (boxPrefab != null)
                    {
                        Vector2Int boxPos = (spawnHackingVault && i == 0) ? vaultCenter + Vector2Int.down : GetSafeBoxPosition();
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

        // --- NUEVO: COLOCAR COFRES DE BOTÍN ---
        if (chestPrefab != null)
        {
            for (int i = 0; i < chestCount; i++)
            {
                if (availablePositions.Count == 0) break;
                // Elegimos una posición libre al azar para el cofre
                int r = Random.Range(0, availablePositions.Count);
                Vector2Int spawnPos = availablePositions[r];

                Instantiate(chestPrefab, new Vector3(spawnPos.x + 0.5f, spawnPos.y + 0.5f, 0f), Quaternion.identity);
                availablePositions.RemoveAt(r);
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