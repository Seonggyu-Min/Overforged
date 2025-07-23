using System.Collections;

namespace SHG
{
  using Player = TestPlayer;

  public interface IInteractable
  {
    public bool IsInteractable(Player player);
    IEnumerator Interact(Player player);
  }
}
