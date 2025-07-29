using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

namespace SCR
{
    public class RoomManager : MonoBehaviour
    {
        [SerializeField] SelectToggle characterToggle;
        [SerializeField] SelectToggle teamToggle;
        [SerializeField] Button StartBtn;

        void Awake()
        {
            StartBtn.onClick.AddListener(StartGame);

        }



        public void StartGame()
        {
            ExitGames.Client.Photon.Hashtable playerProperty = new();
            playerProperty["Character"] = characterToggle.SelectIndex;
            playerProperty["Team"] = teamToggle.SelectIndex;
            PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperty);
            Debug.Log("게임 시작");
            PhotonNetwork.LoadLevel("TestGameScene");
        }
    }


}

