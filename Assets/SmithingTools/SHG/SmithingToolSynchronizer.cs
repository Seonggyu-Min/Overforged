using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using SCR;
using MIN;

namespace SHG
{
  public class SmithingToolSynchronizer : INetworkSynchronizer<SmithingToolComponent>, INetworkEventReciever
  {
    INetworkEventHandler networkEventHandler;
    public bool IsLocal { get; private set; }
    const float MS_TO_SEC = 1f / 1000f;
    Dictionary<int, SmithingToolComponent> smithingTools;
    GameObject playerObject;
    HashSet<int> userTools = new();

    public SmithingToolSynchronizer(INetworkEventHandler networkEventHandler)
    {
      this.networkEventHandler = networkEventHandler;
      this.networkEventHandler.OnNetworkConnected += this.OnNetworkConnected;
      this.networkEventHandler.OnJoinedToRoom += this.OnJoinedToRoom;
      this.networkEventHandler.Register<SmithingToolSynchronizer>(this);
      this.smithingTools = new();
      var photonObject = new GameObject($"{nameof(SmithingToolSynchronizer)} photonObject");
      this.IsLocal = (SceneManager.GetActiveScene().name == "BotScene");
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
            if (BotContext.Instance != null)
            {
                BotContext.Instance.AddTool(smithingTool);
            }
      int playerId = PhotonNetwork.LocalPlayer.ActorNumber;
      if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(
        CustomPropertyKeys.TeamColor, out object teamProperty) &&
        int.TryParse(teamProperty.ToString(), out int teamNumber)) {
        playerId = teamNumber;
      }
      if (this.IsLocal) {
        smithingTool.IsOwner = true;
        if (smithingTool is TableComponent table) {
          table.IsLocal = true;
        }
      }
      else if (smithingTool is DropOffTableComponent dropOffTable ||
        smithingTool is ConveyorBeltHopper conveyorBelt) {
        smithingTool.IsOwner = PhotonNetwork.LocalPlayer.IsMasterClient;
      }
      else {
        smithingTool.IsOwner = smithingTool.PlayerNetworkId == playerId;
      }
    }

    void OnWork(SmithingToolComponent component, ToolWorkResult result)
    {
    //  bool isOwner = false;
    //  var photonView = component.GetComponent<PhotonView>();
    //  if (photonView != null && photonView.IsMine)
    //  {
    //    isOwner = true;
    //  }
    //  else if  ((component is DropOffTableComponent dropOffTable ||
    //    component is ConveyorBeltHopper conveyorBelt  ) && component.IsOwner) {
    //    isOwner = true;
    //  }
    //  if (isOwner) {
    //    this.SendRpc(
    //      sceneId: component.SceneId,
    //      method: nameof(SmithingToolComponent.Work),
    //      args: new object[] {
    //        result.ConvertToNetworkArguments() }
    //      );
    //  }
    }

    void OnTranfered(SmithingToolComponent component, ToolTransferArgs args, ToolTransferResult result)
    {
      //bool isOwner = false;
      //var photonView = component.GetComponent<PhotonView>();
      //if (photonView != null && photonView.IsMine)
      //{
      //  isOwner = true;
      //}
      //else if  ((component is DropOffTableComponent dropOffTable ||
      //  component is ConveyorBeltHopper conveyorBelt  ) && component.IsOwner) {
      //  isOwner = true;
      //}
      //if (isOwner) {
      ////if (component.IsOwner) {
      //  this.SendRpc(
      //    sceneId: component.SceneId,
      //    method: nameof(SmithingToolComponent.Transfer),
      //    args: new object[] {
      //      args.ConvertToNetworkArguments(),
      //      result.ConvertToNetworkArguments()
      //    });
      //}
    }

    public void SendRpc(int sceneId, in string method, object[] args)
    {
      this.SendEvent(
        sceneId: sceneId,
        method: method,
        args: args,
        receiver: INetworkEventHandler.EventReceiver.Others);
    }

    public void SendRpcToMaster(int sceneId, in string method, object[] args)
    {
      this.SendEvent(
        sceneId: sceneId,
        method: method,
        args: args,
        receiver: INetworkEventHandler.EventReceiver.Master);
    }

    void SendEvent(int sceneId, in string method, object[] args, INetworkEventHandler.EventReceiver receiver)
    {
      object[] data = new object[
        args == null ? 3 : args.Length + 3];
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
          destinationIndex: 0,
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
      //      int playerId = PhotonNetwork.LocalPlayer.ActorNumber;
      //      foreach (var smithingTool in this.smithingTools.Values) {
      //        smithingTool.IsOwner = smithingTool.PlayerNetworkId == playerId;
      //      }evelop
    }

    public void ReceiveEvent(object[] data)
    {
      int sceneId = (int)data[0];
      string method = (string)data[1];
      int timestamp = (int)data[2];
      //TODO: adjust timestamp overflow 
      float latency = (float)(PhotonNetwork.ServerTimestamp - timestamp) * MS_TO_SEC;
      object[] args = data.Length > 3 ?
        new object[data.Length - 3] : null;
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

    public bool TryFindComponentFromNetworkId<U>(int networId, out U found) where U : Component
    {
      PhotonView foundView = PhotonView.Find(networId);
      if (foundView != null) {
        found = foundView.GetComponent<U>();
        return (found != null);
      }
      #if UNITY_EDITOR
      Debug.LogError($"{nameof(TryFindComponentFromNetworkId)} fail to find {networId}");
      #endif
      found = null;
      return (false);
    }

    public GameObject GetPlayerObject()
    {
      if (this.playerObject != null) {
        return (this.playerObject);
      }
      foreach (var gameObject in GameObject.FindGameObjectsWithTag("Player")) {
        PhotonView photonView = gameObject.GetComponent<PhotonView>();
        if (photonView != null && photonView.IsMine) {
          this.playerObject = gameObject;
          return (this.playerObject);
        }
      }
      #if UNITY_EDITOR
      Debug.LogError($"{nameof(GetPlayerObject)} Fail to find Player object by tags");
      #endif
      return (null);
    }

    public void SendRpcToGameObject(GameObject gameObject, in string method, object[] args)
    {
      PhotonView photonView = gameObject.GetComponent<PhotonView>();
      if (photonView != null) {
        photonView.RPC( methodName: method, target: RpcTarget.Others, parameters: args);
      }
      else {
        #if UNITY_EDITOR
        throw (new ApplicationException($"{nameof(SendRpcToGameObject)}: Fail to Get {nameof(PhotonView)} from {gameObject}"));
        #endif
      }
    }

    public bool TryGetGameObjectNetworkId(GameObject gameObject, out int id)
    {
      PhotonView photonView = gameObject.GetComponent<PhotonView>();
      if (photonView != null) {
        id = photonView.ViewID;
        return (true); 
      }
      id = -1;
      return (false);
    }
  }
}
