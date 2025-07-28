using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


namespace MIN
{
    // TODO: ChatBoxItem 풀링
    public class RoomChatPanel : MonoBehaviour
    {
        [SerializeField] private GameObject _chatBoxPrefab;
        [SerializeField] private TMP_InputField _messageInputField;
        [SerializeField] private Transform _chatContentParent;
        [SerializeField] private PhotonView _photonView;


        public void OnClickSendButton()
        {
            string message = _messageInputField.text.Trim();
            if (string.IsNullOrEmpty(_messageInputField.text)) return;

            string username = PhotonNetwork.NickName;

            if (_photonView == null)
            {
                _photonView = PhotonView.Get(this);
            }

            _photonView.RPC(nameof(ReceiveMessage), RpcTarget.All, username, message);

            _messageInputField.text = string.Empty;
        }


        [PunRPC]
        private void ReceiveMessage(string sender, string message)
        {
            GameObject chatBoxObject = Instantiate(_chatBoxPrefab, _chatContentParent);
            ChatBoxItem chatBoxItem = chatBoxObject.GetComponent<ChatBoxItem>();
            chatBoxItem.Init(sender, message);
        }
    }
}
