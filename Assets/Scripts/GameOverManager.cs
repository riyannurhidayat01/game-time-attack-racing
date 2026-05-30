using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public void RetryGame()
    {
        string currentLevel = PlayerPrefs.GetString("CurrentLevel");

        SceneManager.LoadScene(currentLevel);
    }
}