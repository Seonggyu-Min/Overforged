using Zenject;

namespace SHG
{
  public class TestMonoInstaller : MonoInstaller<TestMonoInstaller>
  {
    public override void InstallBindings()
    {
      this.Container
        .Bind<INetworkEventHandler>()
        .To<NetworkEventHandler>()
        .AsSingle();
      this.Container
        .Bind<INetworkSynchronizer<SmithingToolComponent>>()
        .To<SmithingToolSynchronizer>()
        .AsSingle();
      this.Container
        .Bind<IAudioLibrary>()
        .To<AudioLibrary>()
        .AsSingle(); 
    }
  }
}
