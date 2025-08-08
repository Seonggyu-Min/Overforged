using MIN;
using Photon.Pun;
using SHG;
using System.Collections;
using System.Collections.Generic;
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
        private List<int> teams;
        private MapData map;

        private void Awake()
        {
            
        }

        private void Start()
        {
            StartCoroutine(SendJoinMessageRoutine());
        }

        public void SetTeam(List<Photon.Realtime.Player> players)
        {
            SetMap();
            SpwanPlayer();

            teams = new();
            foreach (var p in players)
            {
                Hashtable playerProps = p.CustomProperties;
                if (!p.CustomProperties.TryGetValue(CustomPropertyKeys.TeamColor, out object teamcolor))
                {
                    Debug.LogWarning($"플레이어 {p.NickName}의 팀 색상이 설정되지 않았습니다.");
                    continue;
                }
                int teamNum = int.Parse(teamcolor.ToString());
                if (teams.Find(n => n == teamNum) == 0)
                {
                    teams.Add(teamNum);
                }
            }
            SetTeamMap();
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

        private void SetTeamMap()
        {
            map.SetTeam(teams);
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



        private IEnumerator SendJoinMessageRoutine()
        {
            while (!PhotonNetwork.InRoom)
            {
                yield return new WaitForSeconds(0.5f);
                Debug.Log("방에 없음");
            }

            yield return new WaitForSeconds(0.5f);

            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { CustomPropertyKeys.localSceneLoaded, true } });
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



