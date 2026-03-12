using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button quitButton;

    private void Start()
    {
        newGameButton.onClick.AddListener(NewGame);
        continueButton.onClick.AddListener(Continue);
        quitButton.onClick.AddListener(Quit);

        bool hasSave = File.Exists(
            Path.Combine(Application.persistentDataPath, "jennifer_save.json"));
        continueButton.interactable = hasSave;
    }

    private void NewGame()
    {
        SaveSystem.Save(new SaveData());
        SceneTransitionManager.Instance?.LoadScene("City");
    }

    private void Continue() =>
        SceneTransitionManager.Instance?.LoadScene("City");

    private void Quit() => Application.Quit();
}
