using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MIN
{

    public class TeamIDDebugger : MonoBehaviour
    {
        private void Awake()
        {
            PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(CustomPropertyKeys.TeamColor, out object color);
            Debug.Log($"{color}");
        }

        [ContextMenu("TeamID?")]
        private void Test()
        {
            PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(CustomPropertyKeys.TeamColor, out object color);
            Debug.Log($"{color}");
        }
    }
}
