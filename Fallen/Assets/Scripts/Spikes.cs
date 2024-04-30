using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (_other.CompareTag("Player"))
        {
            StartCoroutine(RespawnPoint());
        }
    }

    IEnumerator RespawnPoint()
    {
        PlayerController.Instance.pState.cutscene = true;
        PlayerController.Instance.pState.invincible = true;
        PlayerController.Instance.rb.velocity = Vector2.zero;
        Time.timeScale = 0;
        StartCoroutine(UIManager.Instance.sceneController.LoadLevel());
        PlayerController.Instance.TakeDamage(1);
        yield return new WaitForSeconds(1);
        PlayerController.Instance.transform.position = GameManager.Instance.plataformingRespawnPoint;
        StartCoroutine(UIManager.Instance.sceneController.LoadLevel());
        yield return new WaitForSeconds(3);
        PlayerController.Instance.pState.cutscene = false;
        PlayerController.Instance.pState.invincible = false;
        Time.timeScale = 1;

    }
}
