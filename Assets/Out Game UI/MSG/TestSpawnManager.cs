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
        private void Start()
        {
            SpwanPlayer();
        }

        [ContextMenu("SpawnPlayer")]
        public void SpwanPlayer()
        {
            Vector3 spawnPos = new Vector3(0, 0.5f, 0);
            GameObject playerobj = PhotonNetwork.Instantiate("Player", spawnPos, Quaternion.identity);
            Player player = playerobj.GetComponent<Player>();

            var cameraController = CameraController.Instance; ;
            Debug.Log($"cameraController : {cameraController}");
            cameraController.Player = player.transform;
            cameraController.gameObject.SetActive(true);
        }
    }
}

