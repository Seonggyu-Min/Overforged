using ExitGames.Client.Photon;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


namespace MIN
{
    public class RoomPanelBehaviour : MonoBehaviourPunCallbacks
    {
        [SerializeField] private TMP_Text _roomNameText;
        [SerializeField] private GameObject _menuPanel;
        [SerializeField] private GameObject _exitButton;

        public override void OnEnable()
        {
            StartCoroutine(RoomNameUpdateRoutine());
            _exitButton.SetActive(true);
            _menuPanel.SetActive(true);
            Debug.Log("OnEnable에서 호출");
            ResetLocalSceneLoadedProperty();
            StartCoroutine(ResetRoutine());
        }

        public override void OnDisable()
        {
            _exitButton.SetActive(false);
            _menuPanel.SetActive(false);
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("OnJoinedRoom에서 호출");
            ResetLocalSceneLoadedProperty();
        }

        private IEnumerator RoomNameUpdateRoutine()
        {
            float timeout = 3f;
            float timer = 0f;

            while (PhotonNetwork.CurrentRoom == null && timer < timeout)
            {
                timer += 0.5f;
                yield return new WaitForSeconds(0.5f);
            }

            _roomNameText.text = PhotonNetwork.CurrentRoom != null
                ? PhotonNetwork.CurrentRoom.Name
                : "방 없음";
        }

        private void ResetLocalSceneLoadedProperty()
        {
            if (PhotonNetwork.InRoom)
            {
                ExitGames.Client.Photon.Hashtable hashtable = new();
                hashtable[CustomPropertyKeys.localSceneLoaded] = false;
                PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);

                Debug.Log("localSceneLoaded가 초기화 성공");
            }
            else
            {
                Debug.Log("방에 있지 않아 localSceneLoaded가 초기화되지 않았습니다.");
            }
        }

        private IEnumerator ResetRoutine()
        {
            yield return new WaitForSeconds(3f);

            ExitGames.Client.Photon.Hashtable hashtable = new();
            hashtable[CustomPropertyKeys.localSceneLoaded] = false;
            PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);

            Debug.Log("localSceneLoaded가 초기화 시도함");
        }

        [ContextMenu("localSceneLoaded?")]
        private void Test()
        {
            if ((bool)PhotonNetwork.LocalPlayer.CustomProperties[CustomPropertyKeys.localSceneLoaded] == true)
            {
                Debug.Log("localSceneLoaded가 true");
            }
            else
            {
                Debug.Log("localSceneLoaded가 false");
            }
        }
    }
}
