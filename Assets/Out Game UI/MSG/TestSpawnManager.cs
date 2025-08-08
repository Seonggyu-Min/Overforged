using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SCR;
using SHG;

namespace MIN
{
    public class TestSpawnManager : MonoBehaviour
    {
        [SerializeField] private MapData mapData;

        private void Start()
        {
            SpwanPlayer();
        }

        [ContextMenu("SpawnPlayer")]
        public void SpwanPlayer()
        {
            int myNum = PhotonNetwork.LocalPlayer.ActorNumber - 1;
            Vector3 spawnPos;

            if (myNum < 0 || myNum >= mapData.SpawnPoints.Count)
            {
                Debug.LogWarning("Invalid player number or spawn points not set up correctly.");
                spawnPos = new Vector3(0f, 1f, 0f);
            }
            else
            {
                spawnPos = mapData.SpawnPoints[myNum].position;
            }

            GameObject playerobj = PhotonNetwork.Instantiate("Player", spawnPos, Quaternion.identity);
            Player player = playerobj.GetComponent<Player>();
            player.OnPlayerInput();

            //var cameraController = CameraController.Instance; ;
            //Debug.Log($"cameraController : {cameraController}");
            //cameraController.Player = player.transform;
            //cameraController.gameObject.SetActive(true);
        }
    }
}

