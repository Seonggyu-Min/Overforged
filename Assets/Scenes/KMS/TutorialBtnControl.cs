using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using MIN;
using Firebase.Auth;

public class TutorialBtnControl : MonoBehaviourPunCallbacks
{

    private string uid;

    public void OnclickTutorial()
    {
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
        FirebaseUser user = auth.CurrentUser;
        if (user == null) return;

        uid = user.UserId;

        // 방 설정, 입장 부분
        Hashtable normalProperties = new Hashtable
        {
            { CustomPropertyKeys.MapId, "Tutorial" }
        };
        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 1,
            PublishUserId = true,
            CustomRoomProperties = normalProperties,
            CustomRoomPropertiesForLobby = new string[]
            {
                CustomPropertyKeys.MapId
            }
        };

        PhotonNetwork.CreateRoom(uid, roomOptions);

    }

    public override void OnCreatedRoom()
    {
        if (PhotonNetwork.CurrentRoom.Name != uid) return;
        //플레이어 기본 설정 부분
        Hashtable props = new Hashtable
        {
            { CustomPropertyKeys.CharacterId, 1 },
            { CustomPropertyKeys.TeamColor, 1 }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);

    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (PhotonNetwork.CurrentRoom.Name != uid) return;

        //방 입장 부분
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        string mapId = PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(
            CustomPropertyKeys.MapId, out object mapIdObj) && mapIdObj is string id ? id : "";

        PhotonNetwork.LoadLevel(mapId);

    }


}
