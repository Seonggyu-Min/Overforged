using Zenject;

namespace SHG
{
  public class DefaultInstaller : MonoInstaller<DefaultInstaller>
  {
    public override void InstallBindings()
    {
      this.Container
        .Bind<IGameManager>()
        .To<GameManager>()
        .AsSingle();
    }
  }
}
