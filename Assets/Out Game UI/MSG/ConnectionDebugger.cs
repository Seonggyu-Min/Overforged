using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


namespace MIN
{
    public class ConnectionDebugger : MonoBehaviour
    {
        private void Update()
        {
            if (PhotonNetwork.IsConnected)
            {
                Debug.Log($"Connected to Photon");
            }
            else
            {
                Debug.Log("Not connected to Photon");
            }
        }
    }
}
