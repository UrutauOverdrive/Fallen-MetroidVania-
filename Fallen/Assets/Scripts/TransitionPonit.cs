using UnityEngine;

public class TransitionPonit : MonoBehaviour
{
    [SerializeField] bool goNextLevel;
    [SerializeField] string levelName;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
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
}
