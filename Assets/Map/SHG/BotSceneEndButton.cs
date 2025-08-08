using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

namespace SHG
{
    public class BotSceneEndButton : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        Button button;

        void Awake()
        {
            this.button.onClick.AddListener(this.OnClickEnd);
        }

        void OnClickEnd()
        {
            PhotonNetwork.CurrentRoom.IsOpen = true;
            PhotonNetwork.CurrentRoom.IsVisible = true;
            PhotonNetwork.LoadLevel(0);
        }
    }
}