using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

namespace SHG
{
    public class BotSceneEndPopup : MonoBehaviour
    {
        [SerializeField]
        GameObject winPopup;
        [SerializeField]
        GameObject loosePopup;
        [SerializeField]
        Button[] lobbyButtons;

        public void ShowResult(bool isWin)
        {
            if (isWin) {
                this.winPopup.gameObject.SetActive(true);
            }
            else {
                this.loosePopup.gameObject.SetActive(true);
            }
        }

        void Awake()
        {
            foreach (var button in this.lobbyButtons) {
                button.onClick.AddListener(this.OnClickLobby);
            }
            this.winPopup.SetActive(false);
            this.loosePopup.SetActive(false);
            this.gameObject.SetActive(false);
        }

        void OnClickLobby()
        {
            PhotonNetwork.CurrentRoom.IsOpen = true;
            PhotonNetwork.CurrentRoom.IsVisible = true;
            PhotonNetwork.LoadLevel(0);
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}