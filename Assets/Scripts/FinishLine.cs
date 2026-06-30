using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishLine : MonoBehaviour
{
    public string nextScene;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerPrefs.SetString("CompletedLevel", SceneManager.GetActiveScene().name);
            SceneManager.LoadScene("WinScene");
        }
    }
}