using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace SHG
{
  [RequireComponent(typeof(PhotonView))]
  public class NetworkSynchronizer : MonoBehaviour
  {
    const float MS_TO_SEC = 1f / 60f;
    public int NetworkId { get; private set; }
    PhotonView photonView;
    Dictionary<int, INetSynchronizable> synchronizables;

    public void RegisterSynchronizable(INetSynchronizable synchronizable)
    {
      if (!this.synchronizables.TryAdd(
          synchronizable.SceneId, synchronizable)) {
        #if UNITY_EDITOR
        throw (new ApplicationException($"{nameof(RegisterSynchronizable)} synchronizable: {synchronizable}"));
        #endif
      }
    }

    public void SendRpc(int sceneId, in string method, object[] args)
    {
      object[] data = new object[
        args == null ? 3: args.Length + 3];
      data[0] = sceneId;
      data[1] = method;
      data[2] = PhotonNetwork.ServerTimestamp;
      if (args != null) {
        Array.Copy(
        sourceArray: args,
        sourceIndex: 0,
        destinationArray: data,
        destinationIndex: 2,
        length: args.Length);
      }
      this.photonView.RPC(nameof(ReceiveRpc), RpcTarget.Others, data);
    }

    [PunRPC]
    void ReceiveRpc(object[] data)
    {
      int sceneId = (int)data[0];
      string method = (string)data[1];
      float latency = (float)(PhotonNetwork.ServerTimestamp - (int)data[2]) * MS_TO_SEC;
      object[] args = null;
      if (data.Length > 3) {
        args = new object[data.Length - 3];
        Array.Copy(
          sourceArray: data,
          destinationArray: args,
          sourceIndex: 3,
          destinationIndex:0,
          length: data.Length - 3);
      } 
      if (this.synchronizables.TryGetValue(
          sceneId, out INetSynchronizable synchronizable)) {
        synchronizable.OnRpc(method, latency, args);
      }
      else {
        #if UNITY_EDITOR
        throw (new ApplicationException($"{nameof(ReceiveRpc)}: fail to find {nameof(INetSynchronizable)} in {this.synchronizables} for {sceneId}"));
        #endif
      }
    }

    void Awake()
    {
      PhotonNetwork.FetchServerTimestamp();
      this.synchronizables = new ();
      this.photonView = this.GetComponent<PhotonView>();
      this.NetworkId = this.photonView.OwnerActorNr;
    }

    void Start()
    {
    }
  }
}
