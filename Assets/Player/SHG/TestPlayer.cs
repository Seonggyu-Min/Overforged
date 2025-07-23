using UnityEngine;

namespace SHG
{
  [RequireComponent(typeof(Rigidbody))]
  public class TestPlayer : MonoBehaviour
  {

    [SerializeField] [Range(1f, 3f)]
    float interactRadius;
    [SerializeField] [Range(1f, 10f)]
    float interactRange; 
    Coroutine interactRoutine;

    bool IsTryingInteract()
    {
      return (Input.GetKeyDown(KeyCode.E));
    }

    bool TryFindInteratable(out IInteractable interactable)
    {
      bool isHit = Physics.SphereCast(
        origin: this.transform.position,
        radius: this.interactRadius,
        direction: this.transform.forward,
        hitInfo: out RaycastHit hitInfo,
        maxDistance: this.interactRange);
      if (!isHit) {
        interactable = null;
        return (false);
      } 
      interactable = hitInfo.collider.GetComponent<IInteractable>();
      return (interactable != null);
    }

    void Update()
    {
      var movingInput = this.GetInput();
      if (movingInput != Vector2.zero) {
        this.Move(movingInput);
      }
      if (this.IsTryingInteract() &&
        this.TryFindInteratable(out IInteractable interactable) &&
        interactable.IsInteractable(this)) {
        this.Interact(interactable);
      }
    }

    void Interact(IInteractable interactable)
    {
      if (this.interactRoutine != null) {
        this.StopCoroutine(this.interactRoutine);
      }
      this.interactRoutine = this.StartCoroutine(
        interactable.Interact(this));
    }

    #region Test code
    Rigidbody rb;
    [SerializeField] [Range(1f, 5f)]
    float movingSpeed;

    void Move(in Vector2 input)
    {
      this.rb.velocity = input * this.movingSpeed; 
    }

    Vector2 GetInput()
    {
      return (
        new Vector2(
          x: Input.GetAxis("Horizontal"),
          y: Input.GetAxis("Vertical")
          ).normalized);
    }

    void Awake()
    {
      this.rb = this.GetComponent<Rigidbody>();
    }
    #endregion
  }
}
