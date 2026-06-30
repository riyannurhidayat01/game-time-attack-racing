using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private TextMeshProUGUI volumeText;

    private void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat("GameVolume", 0.8f);
        AudioListener.volume = savedVolume;
        if (volumeSlider != null)
        {
            volumeSlider.value = savedVolume;
        }
        UpdateVolumeText(savedVolume);
    }

    private void UpdateVolumeText(float volume)
    {
        if (volumeText != null)
        {
            volumeText.text = Mathf.RoundToInt(volume * 100f) + "%";
        }
    }

    public void ToggleSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(!settingsPanel.activeSelf);
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Level1");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("GameVolume", volume);
        PlayerPrefs.Save();
        UpdateVolumeText(volume);
    }
}