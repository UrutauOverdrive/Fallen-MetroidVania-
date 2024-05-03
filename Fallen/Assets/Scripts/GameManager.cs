using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Vector2 plataformingRespawnPoint;
    public Vector2 respawnPoint;
    [SerializeField] Bench bench;

    public GameObject shade;

    public static GameManager Instance { get; private set; }
    private void Awake()
    {
        SaveData.Instance.Initialize();
        SaveScene();
        bench = FindObjectOfType<Bench>();

        if (PlayerController.Instance != null)
        {
            if (PlayerController.Instance.halfMana)
            {
                SaveData.Instance.LoadShadeData();
                if (SaveData.Instance.sceneWithShade == SceneManager.GetActiveScene().name || SaveData.Instance.sceneWithShade == "")
                {
                    Instantiate(shade, SaveData.Instance.shadePos, SaveData.Instance.shadeRot);
                }
            }
        }
    }    

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            SaveData.Instance.SavePlayerData();
        }
    }

    public void SaveScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SaveData.Instance.sceneNames.Add(currentSceneName);
    }

    public void RespawnPlayer()
    {
        SaveData.Instance.LoadBench();
        if (SaveData.Instance.benchSceneName != null)
        {
            SceneManager.LoadScene(SaveData.Instance.benchSceneName);
        }

       if (SaveData.Instance.benchPos != null)
       {
            respawnPoint = SaveData.Instance.benchPos;
       }
       else
       {
            respawnPoint = plataformingRespawnPoint;
       }

        PlayerController.Instance.transform.position = respawnPoint;

        StartCoroutine(UIManager.Instance.DeactivateDeathScreen());

        PlayerController.Instance.Respawned();
    }
}