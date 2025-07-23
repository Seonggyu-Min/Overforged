using System.Collections;

namespace SHG
{
  using Player = TestPlayer;

  public interface IInteractable
  {
    public bool IsInteractable(Player player);
    public IEnumerator Interact(Player player);
  }
}
