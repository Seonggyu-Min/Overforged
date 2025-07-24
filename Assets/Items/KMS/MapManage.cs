using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class MapManage : MonoBehaviour
{

    void Awake()
    {
        Vector3 spawnPos = new Vector3(Random.Range(0, 5), 1, Random.Range(0, 5));
        PhotonNetwork.Instantiate("TestPlayer", spawnPos, Quaternion.identity);
    }
}
