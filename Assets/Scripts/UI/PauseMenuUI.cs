using UnityEngine;
using UnityEngine.UI;

public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button saveQuitButton;

    private bool _isPaused;

    private void Awake() => pausePanel.SetActive(false);

    private void Start()
    {
        resumeButton.onClick.AddListener(TogglePause);
        saveQuitButton.onClick.AddListener(SaveAndQuit);
    }

    public void TogglePause()
    {
        _isPaused = !_isPaused;
        pausePanel.SetActive(_isPaused);
        Time.timeScale = _isPaused ? 0f : 1f;
    }

    private void SaveAndQuit()
    {
        Time.timeScale = 1f;
        GameManager.Instance?.SaveGame();
        SceneTransitionManager.Instance?.LoadScene("MainMenu");
    }
}
