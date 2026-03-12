using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance { get; private set; }

    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 0.5f;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        if (fadeImage != null)
            DontDestroyOnLoad(fadeImage.transform.root.gameObject);
    }

    public void LoadScene(string sceneName) =>
        StartCoroutine(FadeAndLoad(sceneName));

    private IEnumerator FadeAndLoad(string sceneName)
    {
        yield return Fade(0f, 1f);
        var op = SceneManager.LoadSceneAsync(sceneName);
        while (op != null && !op.isDone) yield return null;
        yield return Fade(1f, 0f);
    }

    private IEnumerator Fade(float from, float to)
    {
        float elapsed = 0f;
        Color c = fadeImage.color;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(from, to, elapsed / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }
        c.a = to;
        fadeImage.color = c;
    }
}
