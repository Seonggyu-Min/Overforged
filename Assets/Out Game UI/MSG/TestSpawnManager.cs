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

            

            Vector3 spawnPos = mapData.SpawnPoints[myNum].position;
            GameObject playerobj = PhotonNetwork.Instantiate("Player", spawnPos, Quaternion.identity);
            Player player = playerobj.GetComponent<Player>();

            var cameraController = CameraController.Instance; ;
            Debug.Log($"cameraController : {cameraController}");
            cameraController.Player = player.transform;
            cameraController.gameObject.SetActive(true);
        }
    }
}

