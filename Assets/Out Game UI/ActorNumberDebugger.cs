using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MIN
{
    public class ActorNumberDebugger : MonoBehaviour
    {
        [ContextMenu("ActorNumber")]

        private void ShowActorNumber()
        {
            Debug.Log($"{PhotonNetwork.LocalPlayer.ActorNumber}");
        }
    }
}
