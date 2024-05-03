using UnityEngine;

public class TransitionPonit : MonoBehaviour
{
    [SerializeField] bool goNextLevel;
    [SerializeField] string levelName;

    private void OnTriggerEnter2D(Collider2D _collision)
    {
        if (_collision.CompareTag("Player"))
        {
            CheckShadeData();
            if (goNextLevel)
            {
                SceneController.Instance.NextLevel();
            }
            else
            {
                SceneController.Instance.LoadScene(levelName);
            }
        }
    }

    void CheckShadeData()
    {
        GameObject[] enemyObjects = GameObject.FindGameObjectsWithTag("Enemy");

        for (int i = 0; i < enemyObjects.Length; i++)
        {
            if (enemyObjects[i].GetComponent<Shade>() != null)
            {
                SaveData.Instance.SaveShadeData();
            }
        }
    }
}
