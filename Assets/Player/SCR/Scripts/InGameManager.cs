using MIN;
using Photon.Pun;
using SHG;
using System.Collections;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace SCR
{
    public class InGameManager : MonoBehaviour
    {
        //[SerializeField] private CameraController controller;
        [SerializeField] private MapManager mapManager;
        public InGameUIManager InGameUIManager { get => inGameUIManager; }
        [SerializeField] private InGameUIManager inGameUIManager;
        private string localSceneLoaded = "localSceneLoaded";
        private MapData map;

        private void Awake()
        {
            SetMap();
            SpwanPlayer();
        }

        private void Start()
        {
            SendJoinMessage();
            StartCoroutine(SendJoinMessageRoutine());
        }

        // 맵 생성
        private void SetMap()
        {
            Hashtable roomProps = PhotonNetwork.CurrentRoom.CustomProperties;
            if (roomProps.ContainsKey("MapId"))
            {
                map = mapManager.getMap((int)roomProps["MapId"]);
                map.gameObject.SetActive(true);
            }
        }

        // 플레이어 생성
        [ContextMenu("SpawnPlayer")]
        public void SpwanPlayer()
        {
            int myNum = PhotonNetwork.LocalPlayer.ActorNumber - 1;
            Vector3 spawnPos;
            if (myNum < 0 || myNum >= map.SpawnPoints.Count)
            {
                Debug.LogWarning("Invalid player number or spawn points not set up correctly.");
                spawnPos = new Vector3(0f, 1f, 0f);
            }
            else
            {
                spawnPos = map.SpawnPoints[myNum].position;
            }

            GameObject playerobj = PhotonNetwork.Instantiate("Player", spawnPos, Quaternion.identity);
            PlayerPhysical player = playerobj.GetComponent<PlayerPhysical>();
            player.RespawnPoint = spawnPos;

            
            //Debug.Log($"cameraController? : {controller != null}");
            //controller.Player = player.gameObject.transform;
            //controller.gameObject.SetActive(true);
        }

        private void SendJoinMessage()
        {
            // 접속했다는 신호를 보냄
            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { localSceneLoaded, true } });
        }

        private IEnumerator SendJoinMessageRoutine()
        {
            while (!PhotonNetwork.InRoom)
            {
                yield return new WaitForSeconds(0.5f);
                Debug.Log("방에 없음");
            }

            yield return new WaitForSeconds(0.5f);

            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { localSceneLoaded, true } });
            Debug.Log("localSceneLoaded, true 시도");
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



