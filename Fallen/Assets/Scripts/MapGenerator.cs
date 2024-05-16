using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public int width;
    public int height;
    public float fillPercent;
    public int smoothingIterations;

    private int[,] map;

    void Start()
    {
        GenerateMap();
    }

    void GenerateMap()
    {
        map = new int[width, height];
        RandomFillMap();

        for (int i = 0; i < smoothingIterations; i++)
        {
            SmoothMap();
        }

        // Aqui você pode instanciar prefabs para cada célula do mapa, dependendo do valor na matriz 'map'
    }

    void RandomFillMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                map[x, y] = (Random.Range(0f, 1f) < fillPercent) ? 1 : 0;
            }
        }
    }

    void SmoothMap()
    {
        // Implemente a lógica de suavização aqui

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int surroundingWallCount = GetSurroundingWallCount(x, y);

                if (surroundingWallCount > 4)
                {
                    map[x, y] = 1; // Define a célula como preenchida
                }
                else if (surroundingWallCount < 4)
                {
                    map[x, y] = 0; // Define a célula como vazia
                }
            }
        }
    }

    int GetSurroundingWallCount(int gridX, int gridY)
    {
        int wallCount = 0;
        for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
        {
            for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
            {
                // Certifique-se de que a célula vizinha está dentro dos limites do mapa
                if (neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height)
                {
                    // Ignore a célula atual
                    if (neighbourX != gridX || neighbourY != gridY)
                    {
                        // Se a célula vizinha estiver preenchida, aumente o contador
                        wallCount += map[neighbourX, neighbourY];
                    }
                }
                else
                {
                    // Considere células fora dos limites como preenchidas
                    wallCount++;
                }
            }
        }
        return wallCount;
    }

}
