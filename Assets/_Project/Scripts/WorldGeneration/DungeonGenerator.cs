using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement; // Necesario para el atajo F12

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

    [Header("Prefabs de Puzles Básicos")]
    public GameObject logicDoorPrefab;
    public GameObject[] switchPrefabs;
    public GameObject boxPrefab;
    public int numberOfSwitches = 2;

    [Header("Prefabs de Hackeo")]
    public GameObject sciFiDoorPrefab;
    public GameObject hackingTerminalPrefab;

    [Header("Prefabs de Vagoneta (Puzle Lateral)")]
    public GameObject minecartPrefab;
    [Tooltip("0 = Recto, 1 = Curvo")]
    public GameObject[] railPrefabs;
    public GameObject pressurePlatePrefab;

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

    private Vector2Int minecartPosInt;
    private Vector2Int railPlatePosInt;
    private List<Vector2Int> railPositions = new List<Vector2Int>();
    private bool spawnMinecartRoom = false;

    void Start() { GenerateDungeon(); }

    void Update()
    {
        // NUEVO ATAJO DE TECLADO PARA DESARROLLADORES
        if (Input.GetKeyDown(KeyCode.F12))
        {
            Debug.Log("Saltando al siguiente piso...");
            UIManager.currentFloor++;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            return;
        }

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
        railPositions.Clear();
        exitFound = false;

        int currentFloor = UIManager.currentFloor;

        if (currentFloor == 1) { iterations = 5; enemyCount = 0; numberOfSwitches = Random.Range(1, 3); spawnHackingVault = false; spawnMinecartRoom = false; }
        else if (currentFloor == 2) { iterations = 8; enemyCount = 1; numberOfSwitches = 3; spawnHackingVault = true; spawnMinecartRoom = true; }
        else { iterations = 8 + currentFloor; enemyCount = currentFloor - 1; numberOfSwitches = Mathf.Clamp(2 + (currentFloor / 2), 3, 5); spawnHackingVault = true; spawnMinecartRoom = true; }

        RunRandomWalk();
        PrepareExitBottleneck();

        Vector2Int vaultCenter = Vector2Int.zero;
        if (spawnHackingVault) vaultCenter = BuildHackerVault();
        if (spawnMinecartRoom) BuildMinecartRoom();

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
                    for (int y = -1; y <= 1; y++) floorPositions.Add(currentPosition + new Vector2Int(x, y));
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
                    if (floorPositions.Contains(pos + new Vector2Int(x, y))) { isSolidNorth = false; break; }
                }
                if (!isSolidNorth) break;
            }

            if (isSolidNorth && floorPositions.Contains(pos + Vector2Int.left) && floorPositions.Contains(pos + Vector2Int.right))
                validDoorPositions.Add(pos);
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
            for (int x = -3; x <= 3; x++)
            {
                for (int y = 1; y <= 8; y++)
                {
                    if (floorPositions.Contains(pos + new Vector2Int(x, y))) { isSolidNorth = false; break; }
                }
                if (!isSolidNorth) break;
            }

            if (isSolidNorth && (!exitFound || Vector2Int.Distance(pos, logicDoorPosInt) > 10)) candidates.Add(pos);
        }

        Vector2Int entrance = candidates.Count > 0 ? candidates[Random.Range(0, candidates.Count)] : GetRandomFloorPosition();

        sciFiDoorPosInt = entrance + Vector2Int.up;
        floorPositions.Add(sciFiDoorPosInt);
        terminalPosInt = entrance + new Vector2Int(-1, 1);
        floorPositions.Add(entrance + Vector2Int.left);
        floorPositions.Add(entrance + new Vector2Int(0, 2));
        floorPositions.Add(entrance + new Vector2Int(0, 3));

        Vector2Int vaultCenter = entrance + new Vector2Int(0, 6);
        for (int x = -2; x <= 2; x++)
        {
            for (int y = -2; y <= 2; y++) floorPositions.Add(vaultCenter + new Vector2Int(x, y));
        }
        return vaultCenter;
    }

    private void BuildMinecartRoom()
    {
        List<Vector2Int> candidates = new List<Vector2Int>();
        foreach (Vector2Int pos in floorPositions)
        {
            bool isSolidEast = true;
            // NUEVO: Comprobamos un área gigante (8 de largo x 7 de ancho) para que quepa la sala grande
            for (int x = 1; x <= 8; x++)
            {
                for (int y = -3; y <= 3; y++)
                {
                    if (floorPositions.Contains(pos + new Vector2Int(x, y))) { isSolidEast = false; break; }
                }
                if (!isSolidEast) break;
            }

            if (isSolidEast && (!exitFound || Vector2Int.Distance(pos, logicDoorPosInt) > 8)) candidates.Add(pos);
        }

        if (candidates.Count > 0)
        {
            Vector2Int entrance = candidates[Random.Range(0, candidates.Count)];

            floorPositions.Add(entrance + Vector2Int.right);
            floorPositions.Add(entrance + new Vector2Int(2, 0));

            Vector2Int roomCenter = entrance + new Vector2Int(5, 0);
            // NUEVO: Sala de 7x5 para dejar pasillos alrededor del puzle
            for (int x = -3; x <= 3; x++)
            {
                for (int y = -2; y <= 2; y++) floorPositions.Add(roomCenter + new Vector2Int(x, y));
            }

            minecartPosInt = roomCenter + new Vector2Int(-2, 0); // Vagoneta a la izquierda
            railPlatePosInt = roomCenter + new Vector2Int(2, 0); // Placa a la derecha

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++) railPositions.Add(roomCenter + new Vector2Int(x, y)); // 3x3 centro
            }
        }
        else
        {
            spawnMinecartRoom = false;
        }
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
                if (!floorPositions.Contains(currentPos)) wallTilemap.SetTile(new Vector3Int(x, y, 0), wallTile);
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
                    if (spawnHackingVault && i == 0) spawnPos = vaultCenter + new Vector2Int(0, 2);
                    else spawnPos = GetNorthWallPosition();
                    instantiatePos = new Vector3(spawnPos.x + 0.5f, spawnPos.y + 1.5f, 0f);
                }
                else
                {
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
            activeSwitches = spawnedSwitches;
            activeDoor.requiredSwitches = activeSwitches;
        }

        if (spawnMinecartRoom && minecartPrefab != null && railPrefabs.Length > 0 && pressurePlatePrefab != null)
        {
            GameObject cartObj = Instantiate(minecartPrefab, new Vector3(minecartPosInt.x + 0.5f, minecartPosInt.y + 0.5f, 0), Quaternion.identity);
            Minecart mc = cartObj.GetComponent<Minecart>();
            if (mc != null) mc.startDirection = Vector2.right;

            GameObject plateObj = Instantiate(pressurePlatePrefab, new Vector3(railPlatePosInt.x + 0.5f, railPlatePosInt.y + 0.5f, 0), Quaternion.identity);
            PuzzleSwitch plateSwitch = plateObj.GetComponent<PuzzleSwitch>();

            if (activeDoor != null)
            {
                List<PuzzleSwitch> allSwitches = new List<PuzzleSwitch>(activeSwitches);
                allSwitches.Add(plateSwitch);
                activeSwitches = allSwitches.ToArray();
                activeDoor.requiredSwitches = activeSwitches;
            }

            // NUEVO: Algoritmo para garantizar que el puzle siempre tiene solución
            int layoutIndex = Random.Range(0, 3); // Elegimos 1 de 3 plantillas de caminos garantizados

            foreach (Vector2Int rPos in railPositions)
            {
                // Calculamos en qué fila y columna de la cuadrícula 3x3 estamos
                int localX = rPos.x - (minecartPosInt.x + 2); // Devuelve -1, 0, 1
                int localY = rPos.y - minecartPosInt.y;       // Devuelve -1, 0, 1

                int requiredType = Random.Range(0, 2); // Por defecto, ponemos Recta o Curva al azar para despistar

                // Plantilla 0: Camino en "U" por arriba
                if (layoutIndex == 0)
                {
                    if (localX == -1 && localY == 0) requiredType = 1;      // Curva sube
                    else if (localX == -1 && localY == 1) requiredType = 1; // Curva derecha
                    else if (localX == 0 && localY == 1) requiredType = 0;  // Recta cruza
                    else if (localX == 1 && localY == 1) requiredType = 1;  // Curva baja
                    else if (localX == 1 && localY == 0) requiredType = 1;  // Curva sale
                }
                // Plantilla 1: Camino en "U" por abajo
                else if (layoutIndex == 1)
                {
                    if (localX == -1 && localY == 0) requiredType = 1;       // Curva baja
                    else if (localX == -1 && localY == -1) requiredType = 1; // Curva derecha
                    else if (localX == 0 && localY == -1) requiredType = 0;  // Recta cruza
                    else if (localX == 1 && localY == -1) requiredType = 1;  // Curva sube
                    else if (localX == 1 && localY == 0) requiredType = 1;   // Curva sale
                }
                // Plantilla 2: Camino directo por el centro
                else
                {
                    if (localY == 0) requiredType = 0; // Tres rectas en la fila del medio
                }

                // IMPORTANTE: railPrefabs[0] debe ser tu Raíl Recto, y railPrefabs[1] tu Raíl Curvo.
                GameObject railPrefab = (requiredType == 0) ? railPrefabs[0] : railPrefabs[1];
                GameObject spawnedRail = Instantiate(railPrefab, new Vector3(rPos.x + 0.5f, rPos.y + 0.5f, 0), Quaternion.identity);

                // Rotamos las piezas para que el jugador tenga que descubrir el camino
                int randomRot = Random.Range(0, 4);
                spawnedRail.transform.rotation = Quaternion.Euler(0, 0, randomRot * 90f);

                availablePositions.Remove(rPos);
            }

            availablePositions.Remove(minecartPosInt);
            availablePositions.Remove(railPlatePosInt);
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