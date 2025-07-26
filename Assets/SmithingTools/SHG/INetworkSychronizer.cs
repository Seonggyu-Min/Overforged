
namespace SHG
{
  public interface INetworkSynchronizer<T> where T: INetSynchronizable
  {
    public int NetworkId { get; }
    public void RegisterSynchronizable(T synchronizable);
    public void SendRpc(int sceneId, in string method, object[] args);
    void ReceiveRpc(object[] data);
  }
}
