using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapTransition : MonoBehaviour
{
    private MapManager mapManager;

    void Start()
    {
        mapManager = Object.FindAnyObjectByType<MapManager>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            LoadNextMap();
        }
    }

    void LoadNextMap()
    {
        if (mapManager.availableMaps.Count > 0)
        {
            // Carrega o pr�ximo mapa dispon�vel
            SceneManager.LoadScene(mapManager.availableMaps[0].name);
            mapManager.availableMaps.RemoveAt(0);
        }
        else
        {
            // Implemente o c�digo para o fim do jogo
        }
    }
}
