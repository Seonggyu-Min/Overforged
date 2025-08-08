//#define LOCAL_TEST
using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using EventData = ExitGames.Client.Photon.EventData;
using SendOptions = ExitGames.Client.Photon.SendOptions;

namespace SHG
{
  public class NetworkEventHandler: INetworkEventHandler, IOnEventCallback, IConnectionCallbacks
  {
    byte customCode = 101;

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
      #if LOCAL_TEST
      PhotonNetwork.ConnectUsingSettings(); 
      #endif
    }

    void CreateRoom()
    {
      PhotonNetwork.CreateRoom("SHGTest");
    }

    void INetworkEventHandler.Register<T>(T sender)
    {
      if (this.codeBySenders.TryGetValue(sender, out byte code)) {
        #if UNITY_EDITOR
        throw (new ArgumentException($"{sender} is already registered"));
        #else
        this.receiverByCodes[code] = sender;
        #endif
      }
      else {
        this.codeBySenders[sender] = this.customCode;
        this.receiverByCodes[this.customCode] = sender;
        this.customCode += 1;
      }
    }

    void INetworkEventHandler.SendEvent<T>(T sender, object[] data, INetworkEventHandler.EventReceiver receiver)
    {
      if (!this.codeBySenders.TryGetValue(sender, out byte eventCode)) {
        #if UNITY_EDITOR
        throw (new ApplicationException($"{sender} is not registered"));
        #endif
      }
      RaiseEventOptions raiseEventOptions = new RaiseEventOptions {
        Receivers = receiver switch {
          INetworkEventHandler.EventReceiver.Master => 
            Photon.Realtime.ReceiverGroup.MasterClient,
          INetworkEventHandler.EventReceiver.All => 
           Photon.Realtime.ReceiverGroup.All,
          INetworkEventHandler.EventReceiver.Others => 
           Photon.Realtime.ReceiverGroup.Others,
          _ => throw (new ArgumentException($"{nameof(INetworkEventHandler.SendEvent)}: invalid {nameof(receiver)}: {receiver}"))
        },
        CachingOption = EventCaching.AddToRoomCache
      };
      SendOptions sendOptions = new SendOptions {
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

    public void OnConnected()
    {
    }

    public void OnConnectedToMaster()
    {
      #if LOCAL_TEST
      this.CreateRoom();
      Debug.LogWarning("CreateRoom");
      #endif
    }

    public void OnDisconnected(DisconnectCause cause)
    {
    }

    public void OnRegionListReceived(RegionHandler regionHandler)
    {
    }

    public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
    {
    }

    public void OnCustomAuthenticationFailed(string debugMessage)
    {
    }
    }
}
