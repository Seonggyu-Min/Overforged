
namespace SHG
{
  public interface INetworkSynchronizable
  {
    public int PlayerNetworkId { get; }
    public bool IsOwner { get; }
    public int SceneId { get; }
    public void OnRpc(string method, float latencyInSeconds, object[] args = null);
  }  
}
