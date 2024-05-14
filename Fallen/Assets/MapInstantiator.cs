using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapInstantiator : MonoBehaviour
{
    public GameObject[] rooms; // Prefabs de salas de mapa
    public GameObject transitionDoorPrefab; // Prefab de porta de transição
    public GameObject rewardPrefab; // Prefab de recompensa

    void InstantiateMap(int[,] map)
    {
        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                if (map[x, y] == 1)
                {
                    // Instancia uma sala
                    Instantiate(rooms[Random.Range(0, rooms.Length)], new Vector3(x, y, 0), Quaternion.identity);
                }
                else
                {
                    // Instancia outro tipo de célula (por exemplo, corredor)
                }
            }
        }

        // Instancia portas de transição e recompensas em salas específicas
    }
}
