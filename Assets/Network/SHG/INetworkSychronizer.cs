using UnityEngine;

namespace SHG
{
  public interface INetworkSynchronizer<T> where T: INetworkSynchronizable
  {
    public bool TryFindComponentFromNetworkId<U>(int networId, out U found) where U: Component;
    public void RegisterSynchronizable(T synchronizable);
    public void SendRpc(int sceneId, in string method, object[] args);
    void ReceiveRpc(object[] data);
  }
}
