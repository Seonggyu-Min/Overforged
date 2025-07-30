using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;


namespace SCR
{


    public class InGameManager : MonoBehaviourPunCallbacks
    {
        [SerializeField] private CharacterInfo characters;
        [SerializeField] private List<Color> TeamColor;

        void Start()
        {
            PhotonNetwork.JoinRandomOrCreateRoom();

        }

        public override void OnCreatedRoom()
        {

        }
        public override void OnJoinedRoom()
        {
            Debug.Log("입장 완료");
            PhotonNetwork.LocalPlayer.NickName = $"Player_{PhotonNetwork.LocalPlayer.ActorNumber}";
            SpwanPlayer();
        }

        public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
        {

        }


        public void SpwanPlayer()
        {
            Vector3 spawnPos = new Vector3(0, 0, 0);
            GameObject playerobj = PhotonNetwork.Instantiate("Player", spawnPos, Quaternion.identity);
            Player player = playerobj.GetComponent<Player>();
        }

        public override void OnPlayerEnteredRoom(Photon.Realtime.Player player)
        {
            Debug.Log($"{player.NickName} 입장 완료");
        }

    }
}



