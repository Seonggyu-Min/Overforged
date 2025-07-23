using UnityEngine;
using Zenject;

namespace SHG
{
  public class DefaultInstaller : MonoInstaller<DefaultInstaller>
  {
    [SerializeField]
    public ZenjectTestObject prefab;

    public override void InstallBindings()
    {
      this.Container
        .Bind<IGameManager>()
        .To<GameManager>()
        .AsSingle();
    }
  }
}
