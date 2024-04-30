using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Vector2 plataformingRespawnPoint;
    public static GameManager Instance { get; private set; }
    private void Awake()
    {

    }    
}