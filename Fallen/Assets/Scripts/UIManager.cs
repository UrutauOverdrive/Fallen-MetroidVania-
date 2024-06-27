using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public SceneController sceneController; 

    public static UIManager Instance;

    [SerializeField] GameObject deathScreen;
    public GameObject mapHandler; 

    [SerializeField] GameObject halfMana, fullMana;

    public enum ManaState
    {
        FullMana,
        HalfMana
    }

    public ManaState manaState;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);

        sceneController = GetComponentInChildren<SceneController>();
    }

    public void SwitchMana(ManaState _manaState)
    {
        switch (_manaState)
        {
            case ManaState.FullMana:

                halfMana.SetActive(false);
                fullMana.SetActive(true);

                break;

            case ManaState.HalfMana:

                halfMana.SetActive(true);
                fullMana.SetActive(false);

                break;
        }
        manaState = _manaState;
    }

    public IEnumerator ActivateDeathScreen()
    {
        yield return new WaitForSeconds(0.8f);
        deathScreen.SetActive(true);
    }

    public IEnumerator DeactivateDeathScreen()
    {
        yield return new WaitForSeconds(0.5f);
        deathScreen.SetActive(false);

    }
}