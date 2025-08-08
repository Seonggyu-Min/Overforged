using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;


namespace MIN
{
    public class LogInDebugText : MonoBehaviour
    {
        [Inject] private IFirebaseManager _firebaseManager;
        [SerializeField] private TMP_Text _debugText;
        [SerializeField] private TMP_Text _debugText2;

        private void Update()
        {
            _debugText.text = $"Player Count: {PhotonNetwork.PlayerList.Length}\n" +
                  $"Is Master Client: {PhotonNetwork.IsMasterClient}\n" +
                  $"Is In Room: {PhotonNetwork.InRoom}\n" +
                  $"Is In Lobby : {PhotonNetwork.InLobby}\n" +
                  $"Is Connected: {PhotonNetwork.IsConnected}\n" +
                  $"PhotonNetwork.NetworkClientState : {PhotonNetwork.NetworkClientState.ToString()}";

            if (_firebaseManager.Auth.CurrentUser != null)
            {
                _debugText2.text = $"로그인됨: {_firebaseManager.Auth.CurrentUser.Email}";
            }
            else
            {
                _debugText2.text = "로그인되지 않음";
            }
        }
    }
}
