using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using EditorAttributes;
using Zenject;
using EventData = ExitGames.Client.Photon.EventData;
using SendOptions = ExitGames.Client.Photon.SendOptions;

namespace SHG
{
  public class NetworkEventHandler: INetworkEventHandler, IConnectionCallbacks, IMatchmakingCallbacks, IOnEventCallback
  {
    byte customCode = 101;

    [Inject] DiContainer container;
    [SerializeField]
    GameObject prefab;
    public Action OnNetworkConnected { get; set; }
    public Action OnNetworkDisconnected { get; set; }
    public Action OnJoinedToRoom { get; set; }
    Dictionary<object, byte> codeBySenders;
    Dictionary<byte, object> receiverByCodes;

    public NetworkEventHandler()
    {
      this.codeBySenders = new ();
      this.receiverByCodes = new ();
      PhotonNetwork.AddCallbackTarget(this);
      this.CheckConnection();
    }

    void INetworkEventHandler.Register<T>(T sender)
    {
      if (this.codeBySenders.TryGetValue(sender, out byte code)) {
        #if UNITY_EDITOR
        throw (new ArgumentException($"{sender} is already registered"));
        #endif
        this.receiverByCodes[code] = sender;
      }
      else {
        this.codeBySenders[sender] = this.customCode;
        this.receiverByCodes[this.customCode] = sender;
        Debug.Log($"register {sender} for {this.customCode}");
        this.customCode += 1;
      }
    }

    void INetworkEventHandler.SendEvent<T>(T sender, object[] data)
    {
      if (!this.codeBySenders.TryGetValue(sender, out byte eventCode)) {
        #if UNITY_EDITOR
        throw (new ApplicationException($"{sender} is not registered"));
        #endif
      }
      RaiseEventOptions raiseEventOptions = new RaiseEventOptions
      {
        Receivers = ReceiverGroup.Others,
        CachingOption = EventCaching.AddToRoomCache
      };
      SendOptions sendOptions = new SendOptions
      {
        Reliability = true
      };
      PhotonNetwork.RaiseEvent(
        eventCode, data, raiseEventOptions, sendOptions);
    }

    public void OnEvent(EventData photonEvent)
    {
      if (photonEvent.Code <= 100 || photonEvent.Code >= 200) {
        return ;
      }
      if (!this.receiverByCodes.TryGetValue(
          photonEvent.Code, out object found) ||
        !(found is INetworkEventReciever networkEventReciever)) {
        #if UNITY_EDITOR
        Debug.Log($"{nameof(OnEvent)}: reciever for event code {photonEvent.Code} not registered or not {nameof(INetworkEventReciever)}");
        #endif
        return ;
      }
      networkEventReciever.ReceiveEvent((object[])photonEvent.CustomData);
    }

    void OnSpawnCharcter(EventData photonEvent)
    {
//      if (photonEvent.Code == this.characterCode) {
//        object[] data = (object[]) photonEvent.CustomData;
//        GameObject player = this.container
//          .InstantiatePrefab(this.prefab);
//        player.transform.position = (Vector3)data[0];
//        player.transform.rotation = (Quaternion)data[1];
//        PhotonView photonView = player.GetComponent<PhotonView>();
//        photonView.ViewID = (int) data[2];
//      }
    }

    [Button]
    void CreateTestObject()
    {
      PhotonNetwork.Instantiate(
        "TestObject",
        Vector3.zero,
        Quaternion.identity);
    }

    [Button]
    void ManullyCreateTest()
    {
      GameObject player = this.container.InstantiatePrefab(
        this.prefab);
      PhotonView photonView = player.GetComponent<PhotonView>();

      if (PhotonNetwork.AllocateViewID(photonView)) {
        object[] data = new object[] {
          player.transform.position, player.transform.rotation, photonView.ViewID
        };

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.All,
            CachingOption = EventCaching.AddToRoomCache
        };

        SendOptions sendOptions = new SendOptions
        {
          Reliability = true
        };
        PhotonNetwork.RaiseEvent(
          customCode, data, raiseEventOptions, sendOptions);
      }
      else
      {
        Debug.LogError("Failed to allocate a ViewId.");
        GameObject.Destroy(player);
      }
    }

    #region Development Codes

    void CheckConnection()
    {
      if (!PhotonNetwork.IsConnected) {
        PhotonNetwork.ConnectUsingSettings();
      }
      else {
        Debug.Log("PhotonNetwork IsConnected");
        PhotonNetwork.JoinRandomOrCreateRoom();
      }
    }

    public void OnConnectedToMaster()
    {
      Debug.Log("OnConnectedToMaster");
      this.OnNetworkConnected?.Invoke();
      PhotonNetwork.JoinRandomOrCreateRoom();
    }

    public void OnJoinedRoom()
    {
      Debug.Log("OnJoinedRoom");
      this.OnJoinedToRoom?.Invoke();
    }

    public void OnConnected()
    {
      Debug.Log(nameof(OnConnected));
    }

    public void OnDisconnected(DisconnectCause cause)
    {
      Debug.Log(nameof(OnDisconnected));
      this.OnNetworkDisconnected?.Invoke();
    }

    public void OnRegionListReceived(RegionHandler regionHandler)
    {
      Debug.Log(nameof(OnRegionListReceived));
    }

    public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
    {
      Debug.Log(nameof(OnCustomAuthenticationResponse));
    }

    public void OnCustomAuthenticationFailed(string debugMessage)
    {
      Debug.Log(nameof(OnCustomAuthenticationFailed));
    }

    public void OnFriendListUpdate(List<FriendInfo> friendList)
    {
      Debug.Log(nameof(OnFriendListUpdate));
    }

    public void OnCreatedRoom()
    {
      Debug.Log(nameof(OnCreatedRoom));
    }

    public void OnCreateRoomFailed(short returnCode, string message)
    {
      Debug.Log(nameof(OnCreateRoomFailed));
    }

    public void OnJoinRoomFailed(short returnCode, string message)
    {
      Debug.Log(nameof(OnJoinRoomFailed));
    }

    public void OnJoinRandomFailed(short returnCode, string message)
    {
      Debug.Log(nameof(OnJoinRoomFailed));
    }

    public void OnLeftRoom()
    {
      Debug.Log(nameof(OnLeftRoom));
    }
    #endregion
    }
}
