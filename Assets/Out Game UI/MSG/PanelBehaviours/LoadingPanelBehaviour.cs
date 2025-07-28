using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;


namespace MIN
{
    public class LoadingPanelBehaviour : MonoBehaviourPunCallbacks
    {
        [Inject] IOutGameUIManager _outGameUIManager;

        [SerializeField] private TMP_Text _loadingText;


        public override void OnEnable()
        {
            PhotonNetwork.AddCallbackTarget(this);
            Init();
        }

        public override void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        // 디버그용
        private void Update()
        {
            _loadingText.text = PhotonNetwork.NetworkClientState.ToString();
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            if (PhotonNetwork.NetworkClientState == ClientState.Disconnected)
            {
                Debug.Log("연결이 해제되어 다시 연결 중입니다.");
                PhotonNetwork.ConnectUsingSettings();
            }
            else
            {
                Debug.LogWarning($"연결을 재시도 하였으나 현재 상태: {PhotonNetwork.NetworkClientState} 때문에 연결하지 못했습니다.");
            }
        }

        public override void OnJoinedLobby()
        {
            _outGameUIManager.Hide("Loading Panel", () =>
                {
                    _outGameUIManager.Show("Lobby Panel");
                });
        }


        private void Init()
        {
            if (!PhotonNetwork.IsConnected)
            {
                Debug.Log("Photon 연결되지 않음. ConnectUsingSettings 호출");
                PhotonNetwork.ConnectUsingSettings();
            }
            else if (PhotonNetwork.NetworkClientState == ClientState.ConnectedToMasterServer)
            {
                Debug.Log("이미 마스터 서버에 연결되어 있음. JoinLobby 호출");
                PhotonNetwork.JoinLobby();
            }
            // 예외 처리: 연결이 안되어있을 경우 기다리기
            else
            {
                StartCoroutine(Wait());
            }
        }

        private IEnumerator Wait()
        {
            yield return new WaitUntil(() => PhotonNetwork.NetworkClientState == ClientState.ConnectedToMasterServer);
            Debug.Log("마스터 서버에 연결됨. 로비에 입장합니다.");
            PhotonNetwork.JoinLobby();
        }
    }
}
