using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


namespace MIN
{
    // TODO: ChatBoxItem 풀링
    //public class GlobalChatPanel : MonoBehaviour, IOnEventCallback
    //{
    //    [SerializeField] private TMP_InputField _messageInputField;
    //    [SerializeField] private Transform _chatContentParent;
    //    [SerializeField] private GameObject _chatBoxPrefab;

    //    private void OnEnable() => PhotonNetwork.AddCallbackTarget(this);

    //    private void OnDisable() => PhotonNetwork.RemoveCallbackTarget(this);


    //    public void OnClickSendButton()
    //    {
    //        string message = _messageInputField.text.Trim();
    //        if (string.IsNullOrEmpty(message)) return;

    //        string sender = PhotonNetwork.NickName;
    //        object content = new object[] { sender, message };

    //        RaiseEventOptions raiseOptions = new RaiseEventOptions
    //        {
    //            Receivers = ReceiverGroup.All,
    //        };

    //        SendOptions sendOptions = new SendOptions
    //        {
    //            Reliability = true,
    //        };

    //        PhotonNetwork.RaiseEvent(PhotonEventCodes.GlobalChatMessage, content, raiseOptions, sendOptions);

    //        _messageInputField.text = string.Empty;
    //    }


    //    public void OnEvent(EventData photonEvent)
    //    {
    //        if (photonEvent.Code == PhotonEventCodes.GlobalChatMessage)
    //        {
    //            object[] data = (object[])photonEvent.CustomData;
    //            string sender = (string)data[0];
    //            string message = (string)data[1];

    //            GameObject chatBoxObj = Instantiate(_chatBoxPrefab, _chatContentParent);
    //            ChatBoxItem chatBoxItem = chatBoxObj.GetComponent<ChatBoxItem>();
    //            chatBoxItem.Init(sender, message);
    //        }
    //    }
    //}
}
