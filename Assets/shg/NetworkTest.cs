using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using EditorAttributes;
using Zenject;
using EventData = ExitGames.Client.Photon.EventData;
using SendOptions = ExitGames.Client.Photon.SendOptions;

namespace SHG
{
  public class NetworkTest : MonoBehaviourPunCallbacks, IOnEventCallback
  {
    byte customCode = 100;

    [Inject] DiContainer container;
    //[Inject]
    //ZenjectTestObject.Factory testFactory;

    [SerializeField]
    GameObject prefab;

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
        Destroy(player);
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

    public override void OnEnable()
    {
      base.OnEnable(); 
      PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable()
    {
      base.OnDisable();
      PhotonNetwork.RemoveCallbackTarget(this);
    }

    void Start()
    {
      this.CheckConnection();
    }

    void CheckConnection()
    {
      if (!PhotonNetwork.IsConnected) {
        PhotonNetwork.ConnectUsingSettings();
      }
      else {
        Debug.Log("PhotonNetwork IsConnected");
        this.CreateOrJoinRandomRoom();
      }
    }

    public override void OnConnectedToMaster()
    {
      Debug.Log("OnConnectedToMaster");
      this.CreateOrJoinRandomRoom();
    }

    void CreateOrJoinRandomRoom()
    {
      PhotonNetwork.JoinRandomOrCreateRoom();
    }

    public override void OnJoinedRoom()
    {
      Debug.Log("OnJoinedRoom");
    }
    #endregion
  }

}
