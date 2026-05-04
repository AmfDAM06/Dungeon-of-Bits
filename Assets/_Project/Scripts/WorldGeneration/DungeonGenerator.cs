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

    [Header("Prefabs a Instanciar")]
    public GameObject playerPrefab;
    public GameObject enemyPrefab;
    public GameObject exitPrefab; // <--- Referencia para la puerta
    public int enemyCount = 5;

    // Memoria de la hormiga excavadora
    private HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();

    void Start()
    {
        GenerateDungeon();
    }

    private void GenerateDungeon()
    {
        // 1. Limpiamos mapas
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
        floorPositions.Clear();

        // 2. Ejecutamos el Random Walk
        RunRandomWalk();

        // 3. Dibujamos los tiles de suelo
        DrawFloorTiles();

        // 4. Calculamos y dibujamos las paredes sólidas
        CreateWalls();

        // 5. Spawneamos al jugador en el centro de la primera casilla de suelo asegurada
        if (playerPrefab != null && floorPositions.Count > 0)
        {
            Vector2Int[] floorArray = new Vector2Int[floorPositions.Count];
            floorPositions.CopyTo(floorArray);

            // Le sumamos 0.5f para que el jugador aparezca en el centro de la baldosa y no se atasque
            Vector3 playerSpawnPos = new Vector3(floorArray[0].x + 0.5f, floorArray[0].y + 0.5f, 0f);
            Instantiate(playerPrefab, playerSpawnPos, Quaternion.identity);
        }

        // 6. Spawneamos a los enemigos
        SpawnEnemies();

        // 7. Spawneamos la puerta de salida incrustada en una pared superior
        SpawnExit();
    }

    private void RunRandomWalk()
    {
        Vector2Int currentPosition = Vector2Int.zero;

        for (int i = 0; i < iterations; i++)
        {
            for (int j = 0; j < walkLength; j++)
            {
                floorPositions.Add(currentPosition);
                currentPosition += GetRandomDirection();
            }

            if (startRandomlyEachIteration)
            {
                currentPosition = GetRandomFloorPosition();
            }
            else
            {
                currentPosition = Vector2Int.zero;
            }
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

                    // Si el vecino NO tiene suelo, es un borde. Ahí va una pared.
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

    private void SpawnEnemies()
    {
        if (enemyPrefab == null) return;

        List<Vector2Int> availablePositions = new List<Vector2Int>(floorPositions);

        if (floorPositions.Count > 0)
        {
            Vector2Int[] floorArray = new Vector2Int[floorPositions.Count];
            floorPositions.CopyTo(floorArray);
            availablePositions.Remove(floorArray[0]);
        }

        for (int i = 0; i < enemyCount; i++)
        {
            if (availablePositions.Count == 0) break;

            int randomIndex = Random.Range(0, availablePositions.Count);
            Vector2Int spawnPos = availablePositions[randomIndex];

            // Enemigos también centrados con el 0.5f
            Instantiate(enemyPrefab, new Vector3(spawnPos.x + 0.5f, spawnPos.y + 0.5f, 0f), Quaternion.identity);

            availablePositions.RemoveAt(randomIndex);
        }
    }

    // --- MAGIA DE LA PUERTA ---
    private void SpawnExit()
    {
        if (exitPrefab == null) return;

        List<Vector2Int> topWalls = new List<Vector2Int>();

        // Escaneamos el mapa en busca de paredes que estén justo ARRIBA de un suelo
        foreach (Vector2Int floorPos in floorPositions)
        {
            Vector2Int potentialTopWall = floorPos + Vector2Int.up; // Miramos la baldosa de arriba

            // Si la casilla de arriba no es suelo, es una pared segura.
            if (!floorPositions.Contains(potentialTopWall))
            {
                topWalls.Add(potentialTopWall);
            }
        }

        // Si hemos encontrado alguna pared superior candidata...
        if (topWalls.Count > 0)
        {
            // Elegimos una al azar
            int randomIndex = Random.Range(0, topWalls.Count);
            Vector2Int exitPos = topWalls[randomIndex];

            // 1. Borramos la pared de piedra para que el jugador pueda entrar al hueco
            wallTilemap.SetTile(new Vector3Int(exitPos.x, exitPos.y, 0), null);

            // 2. Instanciamos la puerta visualmente en ese hueco (centrada con 0.5f)
            Instantiate(exitPrefab, new Vector3(exitPos.x + 0.5f, exitPos.y + 0.5f, 0f), Quaternion.identity);
        }
    }
}