using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameUIInitializer
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void OnStart()
    {
        // Set up scene load event listener
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Initialize for currently active scene
        InitializeForScene(SceneManager.GetActiveScene());
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeForScene(scene);
    }

    private static void InitializeForScene(Scene scene)
    {
        // 1. Ensure GameUIController singleton exists
        if (GameUIController.Instance == null)
        {
            GameObject managerGo = new GameObject("GameUIManager", typeof(GameUIController));
            Object.DontDestroyOnLoad(managerGo);
        }

        // 2. Fix the silent AudioSource bug (change initial volume 0 -> 1.0)
        AudioSource[] audioSources = Object.FindObjectsOfType<AudioSource>(true);
        foreach (var source in audioSources)
        {
            // If the audio source is configured with 0 volume in the scene, we set it to 1
            // so that it plays sound, scaled by AudioListener.volume.
            if (source.volume == 0)
            {
                source.volume = 1f;
            }
        }

        // Apply saved global volume setting
        float savedVolume = PlayerPrefs.GetFloat("GameVolume", 0.8f);
        AudioListener.volume = savedVolume;

        // 3. Find the best canvas to mount our UI enhancements
        Canvas targetCanvas = FindActiveCanvas();
        if (targetCanvas != null)
        {
            GameUIController.Instance.SetupUIForScene(scene.name, targetCanvas);
        }
        else
        {
            Debug.LogWarning("[GameUIInitializer] No suitable active Canvas found in scene: " + scene.name);
        }
    }

    private static Canvas FindActiveCanvas()
    {
        // Prefer a Canvas that is active and has GraphicRaycaster (ready for UI interactions)
        Canvas[] canvases = Object.FindObjectsOfType<Canvas>();
        foreach (var canvas in canvases)
        {
            if (canvas.gameObject.activeInHierarchy && canvas.GetComponent<UnityEngine.UI.GraphicRaycaster>() != null)
            {
                // In levels, we have Canvas and Canvas1. Prefer the one named "Canvas" or the one with higher sorting order/main layout.
                if (canvas.name == "Canvas")
                {
                    return canvas;
                }
            }
        }

        // Fallback to the first active canvas
        foreach (var canvas in canvases)
            if (canvas.gameObject.activeInHierarchy)
                return canvas;

        return null;
    }
}
