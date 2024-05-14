using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    public List<GameObject> mapPrefabs;
    public List<GameObject> availableMaps;

    void Start()
    {
        availableMaps = new List<GameObject>(mapPrefabs);
        ShuffleMaps();
    }

    void ShuffleMaps()
    {
        for (int i = 0; i < availableMaps.Count; i++)
        {
            int randomIndex = Random.Range(i, availableMaps.Count);
            GameObject temp = availableMaps[randomIndex];
            availableMaps[randomIndex] = availableMaps[i];
            availableMaps[i] = temp;
        }
    }
}
