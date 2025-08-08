using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class DisappearGround : MonoBehaviourPun
{
    [SerializeField] private float cycleTime;
    [SerializeField] private GameObject ground;

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
            StartCoroutine(DisappearCycle());
    }

    private IEnumerator DisappearCycle()
    {
        while (true)
        {
            ground.SetActive(!ground.activeSelf);
            yield return new WaitForSeconds(cycleTime);
        }
    }
}
