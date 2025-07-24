using System;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;


namespace SCR
{
    [Serializable]
    public struct Character
    {
        public string name;
        public float walkSpeed;
        public float dashForce;
        public float workSpeed;
    }

    public class InGameManager : MonoBehaviourPunCallbacks
    {
        [SerializeField] private List<Character> characters;
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
            int CharNum = 0;
            int TeamNum = 0;
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Character", out object character))
            {
                CharNum = (int)character;
            }
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("Team", out object team))
            {
                TeamNum = (int)team;
            }
            SpwanPlayer(CharNum, TeamNum);

        }

        public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
        {

        }


        public void SpwanPlayer(int character, int team)
        {
            Vector3 spawnPos = new Vector3(0, 0, 0);
            GameObject playerobj = PhotonNetwork.Instantiate(characters[character].name, spawnPos, Quaternion.identity);
            Player player = playerobj.GetComponent<Player>();
            player.SetCharacter(characters[character]);
            player.SetTeam(team, TeamColor[team]);
        }

        public override void OnPlayerEnteredRoom(Photon.Realtime.Player player)
        {
            Debug.Log($"{player.NickName} 입장 완료");
        }

    }
}



