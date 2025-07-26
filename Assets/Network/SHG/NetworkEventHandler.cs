using System;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using EditorAttributes;
using Zenject;
using EventData = ExitGames.Client.Photon.EventData;
using SendOptions = ExitGames.Client.Photon.SendOptions;
using System.Collections.Generic;

namespace SHG
{
  public class NetworkEventHandler : IConnectionCallbacks, INetworkEventHandler, IMatchmakingCallbacks
  {
    byte customCode = 100;

    [Inject] DiContainer container;

    [SerializeField]
    GameObject prefab;

    public Action OnNetworkConnected { get; set; }
    public Action OnNetworkDisconnected { get; set; }
    public Action OnJoinedToRoom { get; set; }

    public NetworkEventHandler()
    {
      PhotonNetwork.AddCallbackTarget(this);
      this.CheckConnection();
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

    public void OnEvent(EventData photonEvent)
    {
      if (photonEvent.Code == this.customCode) {
        object[] data = (object[]) photonEvent.CustomData;
        GameObject player = this.container
          .InstantiatePrefab(this.prefab);
        player.transform.position = (Vector3)data[0];
        player.transform.rotation = (Quaternion)data[1];
        PhotonView photonView = player.GetComponent<PhotonView>();
        photonView.ViewID = (int) data[2];
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
