using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameUIController : MonoBehaviour
{
    public static GameUIController Instance { get; private set; }

    private GameObject pausePanel;
    private GameObject nosButton;
    private TextMeshProUGUI nosButtonText;
    private Image nosButtonImage;
    private bool isInLevel;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetupUIForScene(string sceneName, Canvas canvas)
    {
        CleanupLevelUI();
        if (canvas == null) return;

        if (sceneName == "Level1" || sceneName == "Level2")
        {
            SetupLevelUI(canvas);
        }
        else if (sceneName == "WinScene")
        {
            SetupWinSceneUI(canvas);
        }
    }

    #region Level UI Setup (Pause System)
    private void SetupLevelUI(Canvas canvas)
    {
        // Create Pause Button in Top Left corner
        GameObject pauseBtn = CreateButton(canvas.transform, "PauseButton", "II", new Vector2(55, 55), new Vector2(45, -45), () =>
        {
            PauseGame();
        });

        // Anchor it to Top-Left
        RectTransform btnRect = pauseBtn.GetComponent<RectTransform>();
        btnRect.anchorMin = new Vector2(0, 1);
        btnRect.anchorMax = new Vector2(0, 1);
        btnRect.pivot = new Vector2(0, 1);

        // Customize the text size inside the Pause Button
        TextMeshProUGUI btnTxt = pauseBtn.GetComponentInChildren<TextMeshProUGUI>();
        if (btnTxt != null)
        {
            btnTxt.fontSize = 24;
            btnTxt.fontStyle = FontStyles.Bold;
        }

        // Create Pause Panel (Hidden initially)
        CreatePausePanel(canvas.transform);

        // Create NOS Button
        isInLevel = true;
        CreateNosButton(canvas.transform);
        StartCoroutine(UpdateNosButtonRoutine());
    }

    private void CreatePausePanel(Transform parent)
    {
        // Panel Root
        pausePanel = CreatePanel(parent, "PausePanel", new Vector2(400, 300), new Color(0.08f, 0.09f, 0.13f, 0.98f));
        pausePanel.SetActive(false);

        // Neon cyan top header strip
        CreatePanel(pausePanel.transform, "HeaderStrip", new Vector2(400, 6), new Color(0.00f, 0.88f, 1.00f, 1f))
            .GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 147);

        // Title
        CreateText(pausePanel.transform, "GAME DIHENTIKAN", 24, new Color(0.00f, 0.88f, 1.00f, 1f), TextAlignmentOptions.Center, new Vector2(0, 85));

        // Resume Button
        CreateButton(pausePanel.transform, "ResumeButton", "LANJUTKAN", new Vector2(220, 50), new Vector2(0, 10), () =>
        {
            ResumeGame();
        });

        // Main Menu Button
        CreateButton(pausePanel.transform, "MainMenuButton", "MENU UTAMA", new Vector2(220, 50), new Vector2(0, -60), () =>
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainMenu");
        });
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;
        if (pausePanel != null)
        {
            StopAllCoroutines();
            StartCoroutine(ScaleOpen(pausePanel.transform));
        }
    }

    private void ResumeGame()
    {
        Time.timeScale = 1f;
        if (pausePanel != null)
        {
            StopAllCoroutines();
            StartCoroutine(ScaleClose(pausePanel.transform));
        }
        if (isInLevel)
        {
            StartCoroutine(UpdateNosButtonRoutine());
        }
    }
    #endregion

    #region NOS System
    private void CleanupLevelUI()
    {
        isInLevel = false;
        StopAllCoroutines();
        nosButton = null;
        nosButtonText = null;
        nosButtonImage = null;
    }

    private void CreateNosButton(Transform parent)
    {
        nosButton = CreateButton(parent, "NOSButton", "NOS", new Vector2(80, 80), new Vector2(-30, 0), () =>
        {
            CarController car = FindObjectOfType<CarController>();
            if (car != null) car.ActivateNos();
        });

        RectTransform rect = nosButton.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(1, 0.5f);
        rect.anchorMax = new Vector2(1, 0.5f);
        rect.pivot = new Vector2(1, 0.5f);

        nosButtonText = nosButton.GetComponentInChildren<TextMeshProUGUI>();
        nosButtonText.fontSize = 22;

        nosButtonImage = nosButton.GetComponent<Image>();
    }

    private IEnumerator UpdateNosButtonRoutine()
    {
        while (isInLevel)
        {
            CarController car = FindObjectOfType<CarController>();
            if (car != null && nosButtonImage != null && nosButtonText != null)
            {
                if (car.IsNosActive())
                {
                    nosButtonText.text = "NOS";
                    nosButtonText.color = Color.white;
                    nosButtonImage.color = new Color(1f, 0.2f, 0.2f, 1f);
                    nosButton.transform.localScale = Vector3.one * 1.1f;
                }
                else if (car.IsNosReady())
                {
                    nosButtonText.text = "NOS";
                    nosButtonText.color = Color.white;
                    nosButtonImage.color = new Color(1f, 0.6f, 0f, 1f);
                    nosButton.transform.localScale = Vector3.one;
                }
                else
                {
                    float remaining = car.GetCooldownRemaining();
                    nosButtonText.text = Mathf.CeilToInt(remaining).ToString() + "s";
                    nosButtonText.color = new Color(0.6f, 0.6f, 0.6f, 0.8f);
                    nosButtonImage.color = new Color(0.2f, 0.2f, 0.25f, 0.6f);
                    nosButton.transform.localScale = Vector3.one;
                }
            }
            yield return new WaitForSecondsRealtime(0.15f);
        }
    }
    #endregion

    #region Win Scene UI Setup
    private void SetupWinSceneUI(Canvas canvas)
    {
        string completedLevel = PlayerPrefs.GetString("CompletedLevel", "");

        Color neonCyan = new Color(0.00f, 0.88f, 1.00f, 1f);

        // Dark overlay background (stretch to fill canvas)
        GameObject bg = CreatePanel(canvas.transform, "WinBackground", Vector2.zero, new Color(0.05f, 0.06f, 0.10f, 0.95f));
        RectTransform bgRect = bg.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;

        // YOU WIN Title
        CreateText(canvas.transform, "YOU WIN", 48, new Color(1f, 0.84f, 0f, 1f), TextAlignmentOptions.Center, new Vector2(0, 120));

        // Subtitle: completed level
        CreateText(canvas.transform, "LEVEL " + (completedLevel == "Level2" ? "2" : "1") + " SELESAI!", 22, neonCyan, TextAlignmentOptions.Center, new Vector2(0, 70));

        // Neon header strip below title
        CreatePanel(canvas.transform, "WinHeaderStrip", new Vector2(300, 4), neonCyan)
            .GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 40);

        // Continue to Level 2 button (only if just finished Level 1)
        if (completedLevel == "Level1")
        {
            CreateButton(canvas.transform, "NextLevelButton", "LANJUT LEVEL DUA", new Vector2(280, 55), new Vector2(0, -20), () =>
            {
                SceneManager.LoadScene("Level2");
            });
        }

        // Back to Main Menu button
        CreateButton(canvas.transform, "BackToMenuButton", "BALIK KE MAIN MENU", new Vector2(280, 55), new Vector2(0, completedLevel == "Level1" ? -90 : -40), () =>
        {
            SceneManager.LoadScene("MainMenu");
        });
    }
    #endregion

    #region UI Utility Helper Methods
    private GameObject CreatePanel(Transform parent, string name, Vector2 size, Color color)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        go.transform.SetParent(parent, false);
        
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.sizeDelta = size;
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;

        Image img = go.GetComponent<Image>();
        img.color = color;

        return go;
    }

    private TextMeshProUGUI CreateText(Transform parent, string textContent, int fontSize, Color color, TextAlignmentOptions alignment, Vector2 position, Vector2? size = null)
    {
        GameObject go = new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
        go.transform.SetParent(parent, false);

        RectTransform rect = go.GetComponent<RectTransform>();
        if (size.HasValue)
        {
            rect.sizeDelta = size.Value;
        }
        else
        {
            rect.sizeDelta = new Vector2(parent.GetComponent<RectTransform>().sizeDelta.x - 40f, 40f);
        }
        rect.anchoredPosition = position;
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);

        TextMeshProUGUI text = go.GetComponent<TextMeshProUGUI>();
        text.text = textContent;
        text.fontSize = fontSize;
        text.color = color;
        text.alignment = alignment;

        return text;
    }

    private GameObject CreateButton(Transform parent, string name, string labelText, Vector2 size, Vector2 position, System.Action onClickAction)
    {
        GameObject buttonGo = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        buttonGo.transform.SetParent(parent, false);

        RectTransform rect = buttonGo.GetComponent<RectTransform>();
        rect.sizeDelta = size;
        rect.anchoredPosition = position;
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);

        Image img = buttonGo.GetComponent<Image>();
        img.color = new Color(0.14f, 0.15f, 0.20f, 1f);

        Button btn = buttonGo.GetComponent<Button>();
        btn.targetGraphic = img;
        
        ColorBlock colors = btn.colors;
        colors.normalColor = new Color(0.14f, 0.15f, 0.20f, 1f);
        colors.highlightedColor = new Color(0.00f, 0.88f, 1.00f, 1f); // Neon cyan
        colors.pressedColor = new Color(0.00f, 0.70f, 0.80f, 1f);
        colors.selectedColor = new Color(0.14f, 0.15f, 0.20f, 1f);
        btn.colors = colors;

        btn.onClick.AddListener(() => onClickAction?.Invoke());

        // Label
        TextMeshProUGUI txt = CreateText(buttonGo.transform, labelText, 16, Color.white, TextAlignmentOptions.Center, Vector2.zero, size);
        RectTransform txtRect = txt.GetComponent<RectTransform>();
        txtRect.anchorMin = Vector2.zero;
        txtRect.anchorMax = Vector2.one;
        txtRect.sizeDelta = Vector2.zero;
        txtRect.anchoredPosition = Vector2.zero;
        txt.fontStyle = FontStyles.Bold;

        return buttonGo;
    }

    private GameObject CreateSlider(Transform parent, string name, Vector2 size, Vector2 position, float minValue, float maxValue, float currentValue, System.Action<float> onValueChangeAction)
    {
        GameObject sliderGo = new GameObject(name, typeof(RectTransform), typeof(Slider));
        sliderGo.transform.SetParent(parent, false);
        
        RectTransform sliderRect = sliderGo.GetComponent<RectTransform>();
        sliderRect.sizeDelta = size;
        sliderRect.anchoredPosition = position;
        sliderRect.anchorMin = new Vector2(0.5f, 0.5f);
        sliderRect.anchorMax = new Vector2(0.5f, 0.5f);
        sliderRect.pivot = new Vector2(0.5f, 0.5f);

        Slider slider = sliderGo.GetComponent<Slider>();

        // Background
        GameObject bgGo = new GameObject("Background", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        bgGo.transform.SetParent(sliderGo.transform, false);
        RectTransform bgRect = bgGo.GetComponent<RectTransform>();
        bgRect.anchorMin = new Vector2(0, 0.35f);
        bgRect.anchorMax = new Vector2(1, 0.65f);
        bgRect.sizeDelta = Vector2.zero;
        bgRect.anchoredPosition = Vector2.zero;
        bgGo.GetComponent<Image>().color = new Color(0.22f, 0.24f, 0.30f, 1f);

        // Fill Area
        GameObject fillAreaGo = new GameObject("Fill Area", typeof(RectTransform));
        fillAreaGo.transform.SetParent(sliderGo.transform, false);
        RectTransform fillAreaRect = fillAreaGo.GetComponent<RectTransform>();
        fillAreaRect.anchorMin = new Vector2(0, 0.35f);
        fillAreaRect.anchorMax = new Vector2(1, 0.65f);
        fillAreaRect.sizeDelta = Vector2.zero;
        fillAreaRect.anchoredPosition = Vector2.zero;

        // Fill
        GameObject fillGo = new GameObject("Fill", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        fillGo.transform.SetParent(fillAreaGo.transform, false);
        RectTransform fillRect = fillGo.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.sizeDelta = Vector2.zero;
        fillRect.anchoredPosition = Vector2.zero;
        fillGo.GetComponent<Image>().color = new Color(0.00f, 0.88f, 1.00f, 1f); // Neon cyan

        // Handle Slide Area
        GameObject handleAreaGo = new GameObject("Handle Slide Area", typeof(RectTransform));
        handleAreaGo.transform.SetParent(sliderGo.transform, false);
        RectTransform handleAreaRect = handleAreaGo.GetComponent<RectTransform>();
        handleAreaRect.anchorMin = Vector2.zero;
        handleAreaRect.anchorMax = Vector2.one;
        handleAreaRect.sizeDelta = Vector2.zero;
        handleAreaRect.anchoredPosition = Vector2.zero;

        // Handle
        GameObject handleGo = new GameObject("Handle", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        handleGo.transform.SetParent(handleAreaGo.transform, false);
        RectTransform handleRect = handleGo.GetComponent<RectTransform>();
        handleRect.sizeDelta = new Vector2(24, 24);
        handleRect.anchoredPosition = Vector2.zero;
        handleGo.GetComponent<Image>().color = Color.white;

        // Setup Slider references
        slider.fillRect = fillRect;
        slider.handleRect = handleRect;
        slider.targetGraphic = handleGo.GetComponent<Image>();
        slider.minValue = minValue;
        slider.maxValue = maxValue;
        slider.value = currentValue;
        slider.onValueChanged.AddListener((val) => onValueChangeAction?.Invoke(val));

        return sliderGo;
    }
    #endregion

    #region Animations
    private IEnumerator ScaleOpen(Transform target)
    {
        target.gameObject.SetActive(true);
        target.localScale = Vector3.zero;
        float elapsed = 0f;
        float duration = 0.25f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;
            // Back out ease
            t = Mathf.Sin(t * Mathf.PI * 0.5f);
            target.localScale = Vector3.one * t;
            yield return null;
        }
        target.localScale = Vector3.one;
    }

    private IEnumerator ScaleClose(Transform target)
    {
        float elapsed = 0f;
        float duration = 0.2f;
        Vector3 startScale = target.localScale;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;
            t = t * t; // Ease in
            target.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
            yield return null;
        }
        target.localScale = Vector3.zero;
        target.gameObject.SetActive(false);
    }
    #endregion
}
