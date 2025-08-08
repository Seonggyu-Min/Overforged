using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using MIN;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;

namespace SHG
{
    public class BotSceneButton : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        Button button;
        string roomName;

        void Awake()
        {
            button.onClick.AddListener(this.OnClickStart);
        }

        void OnClickStart()
        {
            var properties = new Hashtable {
            { CustomPropertyKeys.MapId, "BotScene" }
            };
            var roomOptions = new RoomOptions
            {
                MaxPlayers = 1,
                IsOpen = false,
                IsVisible = false,
                CustomRoomProperties = new Hashtable {
                    { CustomPropertyKeys.MapId, "BotScene" }
                },
                CustomRoomPropertiesForLobby = new string[] {
                    CustomPropertyKeys.MapId
                }
            };
            this.roomName = System.Guid.NewGuid().ToString();
            PhotonNetwork.CreateRoom(this.roomName, roomOptions);
        }

        public override void OnCreatedRoom()
        {
            if (PhotonNetwork.CurrentRoom.Name != this.roomName) return;
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.LoadLevel("BotScene");
        }
    }
}