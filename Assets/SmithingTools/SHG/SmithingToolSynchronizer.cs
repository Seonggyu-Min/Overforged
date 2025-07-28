using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace SHG
{
  public class SmithingToolSynchronizer : INetworkSynchronizer<SmithingToolComponent>, INetworkEventReciever
  {
    INetworkEventHandler networkEventHandler;

    const float MS_TO_SEC = 1f / 1000f;
    Dictionary<int, SmithingToolComponent> smithingTools;

    public SmithingToolSynchronizer(INetworkEventHandler networkEventHandler)
    {
      this.networkEventHandler = networkEventHandler;
      this.networkEventHandler.OnNetworkConnected += this.OnNetworkConnected;
      this.networkEventHandler.OnJoinedToRoom += this.OnJoinedToRoom;
      this.networkEventHandler.Register<SmithingToolSynchronizer>(this);
      this.smithingTools = new ();
      var photonObject = new GameObject($"{nameof(SmithingToolSynchronizer)} photonObject");
    }

    public void RegisterSynchronizable(SmithingToolComponent smithingTool)
    {
      if (!this.smithingTools.TryAdd(
          smithingTool.SceneId, smithingTool)) {
        #if UNITY_EDITOR
        throw (new ApplicationException($"{nameof(RegisterSynchronizable)} synchronizable: {smithingTool}"));
        #endif
      }
      smithingTool.OnTransfered += this.OnTranfered;
      smithingTool.OnWorked += this.OnWork;
    }

    void OnWork(SmithingToolComponent component, ToolWorkResult result)
    {
      if (component.IsOwner) {
        this.SendRpc(
          sceneId: component.SceneId,
          method: nameof(SmithingToolComponent.Work),
          args: new object[] {
            result.ConvertToNetworkArguments() }
          ); 
      } 
    }

    void OnTranfered(SmithingToolComponent component, ToolTransferArgs args, ToolTransferResult result)
    {
      if (component.IsOwner) {
        this.SendRpc(
          sceneId: component.SceneId,
          method: nameof(SmithingToolComponent.Transfer),
          args: new object[] { 
            args.ConvertToNetworkArguments(),
            result.ConvertToNetworkArguments()
          }
          );
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
        destinationIndex: 3,
        length: args.Length);
      }
      this.networkEventHandler.SendEvent(this, data);
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
        throw (new ApplicationException($"{nameof(ReceiveRpc)}: fail to find {nameof(INetworkSynchronizable)} in {this.smithingTools} for {sceneId}"));
        #endif
      }
    }

    void OnNetworkConnected()
    {
      PhotonNetwork.FetchServerTimestamp();
    }

    void OnJoinedToRoom()
    {
      int playerId = PhotonNetwork.LocalPlayer.ActorNumber;
      foreach (var smithingTool in this.smithingTools.Values) {
        smithingTool.IsOwner = smithingTool.PlayerNetworkId == playerId;
      }
    }

    public void ReceiveEvent(object[] data)
    {
      int sceneId = (int)data[0];
      string method = (string)data[1];
      int timestamp = (int)data[2];
      //TODO: adjust timestamp overflow 
      float latency = (float)(PhotonNetwork.ServerTimestamp - timestamp) * MS_TO_SEC;
      object[] args = data.Length > 3 ? 
        new object[data.Length - 3]: null;  
      if (data.Length > 3) {
        Array.Copy(
        sourceArray: data,
        sourceIndex: 3,
        destinationArray: args,
        destinationIndex: 0,
        length: args.Length);
      }
      if (this.smithingTools.TryGetValue(
          sceneId, out SmithingToolComponent smithingTool)) {
        smithingTool.OnRpc(
          method: method,
          latencyInSeconds: latency,
          args: args
          );
      }
    }

    public bool TryFindComponentFromNetworkId<U>(int networId, out U found) where U: Component
    {
      PhotonView foundView = PhotonView.Find(networId);
      if (foundView != null) {
        found = foundView.GetComponent<U>();  
        return (found != null);
      }
      found = null;
      return (false);
    }
  }
}
