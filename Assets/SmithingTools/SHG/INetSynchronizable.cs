
namespace SHG
{
  public interface INetSynchronizable
  {
    public int PlayerNetworkId { get; }
    public bool IsOwner { get; }
    public int SceneId { get; set; }
    public void OnRpc(string method, float latencyInSeconds, object[] args = null);
    void SendRpc(string method, object[] args);
  }  
}
