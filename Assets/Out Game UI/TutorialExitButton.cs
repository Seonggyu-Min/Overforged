using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;


namespace MIN
{
    public class TutorialExitButton : MonoBehaviour
    {
        [ContextMenu("Exit")]
        public void OnClickExitButton()
        {
            PhotonNetwork.LeaveRoom();  // 방이 아니라 로비로 가기 위해 LeaveRoom
            PhotonNetwork.JoinLobby();
            PhotonNetwork.LoadLevel(0); // 아웃게임씬으로
        }
    }
}
