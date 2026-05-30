using UnityEngine;
using UnityEngine.SceneManagement;

public class Obstacle : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerPrefs.SetString("CurrentLevel", SceneManager.GetActiveScene().name);

            SceneManager.LoadScene("GameOver");
        }
    }
}