using UnityEngine;
using UnityEngine.InputSystem;

public interface IInteractable
{
    void Interact(JenniferInteraction jennifer);
}

public class JenniferInteraction : MonoBehaviour
{
    [SerializeField] private float interactRange = 2f;
    [SerializeField] private LayerMask interactableLayer;

    private JenniferInputActions _input;

    private void Awake() => _input = new JenniferInputActions();

    private void OnEnable()
    {
        _input.Player.Enable();
        _input.Player.Interact.performed += OnInteract;
        _input.Player.Intimidate.performed += OnIntimidate;
    }

    private void OnDisable()
    {
        _input.Player.Interact.performed -= OnInteract;
        _input.Player.Intimidate.performed -= OnIntimidate;
        _input.Player.Disable();
    }

    private void OnInteract(InputAction.CallbackContext ctx)
    {
        if (Physics.Raycast(transform.position + Vector3.up, transform.forward,
            out RaycastHit hit, interactRange, interactableLayer))
        {
            hit.collider.GetComponent<IInteractable>()?.Interact(this);
            AudioManager.Instance?.PlayInteract();
        }
    }

    private void OnIntimidate(InputAction.CallbackContext ctx) =>
        ShopUI.Instance?.OnIntimidatePressed();
}
