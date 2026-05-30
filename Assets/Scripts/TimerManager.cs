using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class TimerManager : MonoBehaviour
{
    public float timeRemaining = 60f;
    public TextMeshProUGUI timerText;

    void Start()
    {
        PlayerPrefs.SetString("CurrentLevel", SceneManager.GetActiveScene().name);
    }

    void Update()
    {
        timeRemaining -= Time.deltaTime;

        timerText.text = Mathf.Ceil(timeRemaining).ToString();

        if (timeRemaining <= 0)
        {
            SceneManager.LoadScene("GameOver");
        }
    }
}