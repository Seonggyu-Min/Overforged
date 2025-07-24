using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


public class Launcher : MonoBehaviourPunCallbacks
{
    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    void Start()
    {
        Connect();

    }
    public void Connect()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            Debug.Log("connect using settings"); //1
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("on connected to master"); //2
        PhotonNetwork.JoinRoom("MyRoom");
    }

    public void JoinMap()
    {
        PhotonNetwork.LoadLevel("TestScene");
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("on disconected");
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("on join room failed");
        PhotonNetwork.CreateRoom("MyRoom", new RoomOptions());
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("on joined room"); //4
        
        
    }
}
