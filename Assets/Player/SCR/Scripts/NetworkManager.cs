using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
namespace SCR
{
    public class NetworkManager : MonoBehaviourPunCallbacks
    {
        private void Start()
        {
            PhotonNetwork.ConnectUsingSettings(); // 접속 시도 요청
        }

        public override void OnConnected()
        {
            base.OnConnected();
            Debug.Log("연결");

        }

        public override void OnConnectedToMaster()
        {
            base.OnConnectedToMaster();
            Debug.Log("마스터 연결");

            // if (_loadingPanel.activeSelf)
            //     _loadingPanel.SetActive(false);
            // else PhotonNetwork.JoinLobby();
        }

        public override void OnJoinedRoom()
        {

        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            base.OnDisconnected(cause);
            PhotonNetwork.ConnectUsingSettings();

        }
    }
}

