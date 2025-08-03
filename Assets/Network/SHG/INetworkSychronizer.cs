using UnityEngine;

namespace SHG
{
  public interface INetworkSynchronizer<T> where T: INetworkSynchronizable
  {
    public bool TryFindComponentFromNetworkId<U>(int networId, out U found) where U: Component;
    public GameObject GetPlayerObject();
    public bool TryGetGameObjectNetworkId(GameObject gameObject, out int id);
    public void SendRpcToGameObject(GameObject gameObject, in string method, object[] args);
    public void RegisterSynchronizable(T synchronizable);
    public void SendRpc(int sceneId, in string method, object[] args);
    public void SendRpcToMaster(int sceneId, in string method, object[] args);
    void ReceiveRpc(object[] data);
  }
}
