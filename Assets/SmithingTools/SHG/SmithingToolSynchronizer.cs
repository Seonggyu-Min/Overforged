using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace SHG
{
  public class SmithingToolSynchronizer : INetworkSynchronizer<SmithingToolComponent>
  {
    INetworkEventHandler networkEventHandler;

    const float MS_TO_SEC = 1f / 60f;
    public int NetworkId { get; set; }
    PhotonView photonView;
    Dictionary<int, SmithingToolComponent> smithingTools;

    public SmithingToolSynchronizer(INetworkEventHandler networkEventHandler)
    {
      this.networkEventHandler = networkEventHandler;
      this.networkEventHandler.OnNetworkConnected += this.OnNetworkConnected;
      this.networkEventHandler.OnJoinedToRoom += this.OnJoinedToRoom;
      this.smithingTools = new ();
      var photonObject = new GameObject($"{nameof(SmithingToolSynchronizer)} photonObject");
      this.photonView = photonObject.AddComponent<PhotonView>();
    }

    public void RegisterSynchronizable(SmithingToolComponent smithingTool)
    {
      if (!this.smithingTools.TryAdd(
          smithingTool.SceneId, smithingTool)) {
        #if UNITY_EDITOR
        throw (new ApplicationException($"{nameof(RegisterSynchronizable)} synchronizable: {smithingTool}"));
        #endif
      }
    }

    void OnNetworkConnected()
    {
      PhotonNetwork.FetchServerTimestamp();
    }

    void OnJoinedToRoom()
    {
      this.NetworkId = this.photonView.OwnerActorNr;
      foreach (var smithingTool in this.smithingTools.Values) {
        smithingTool.IsOwner = smithingTool.PlayerNetworkId == this.NetworkId;
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
    public void ReceiveRpc(object[] data)
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
      if (this.smithingTools.TryGetValue(
          sceneId, out SmithingToolComponent smithingTool)) {
        smithingTool.OnRpc(method, latency, args);
      }
      else {
        #if UNITY_EDITOR
        throw (new ApplicationException($"{nameof(ReceiveRpc)}: fail to find {nameof(INetSynchronizable)} in {this.smithingTools} for {sceneId}"));
        #endif
      }
    }
  }
}
