using Photon.Pun;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using SHG;

namespace SCR
{
    public class InGameManager : MonoBehaviour
    {
        [SerializeField] private MapManager mapManager;
        public InGameUIManager InGameUIManager { get => inGameUIManager; }
        [SerializeField] private InGameUIManager inGameUIManager;
        private string localSceneLoaded = "LocalSceneLoaded";
        private MapData map;

        private void Awake()
        {
            SetMap();
            SpwanPlayer();
        }

        private void Start()
        {
            SendJoinMessage();
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

            var cameraController = CameraController.Instance; ;
            Debug.Log($"cameraController : {cameraController}");
            cameraController.Player = player.transform;
            cameraController.gameObject.SetActive(true);
        }

        private void SendJoinMessage()
        {
            // 접속했다는 신호를 보냄
            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { localSceneLoaded, true } });
        }

    }
}



