using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    [SerializeField] private string targetScene;
    [SerializeField] private string requiredTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(requiredTag)) return;
        SceneTransitionManager.Instance?.LoadScene(targetScene);
    }
}
