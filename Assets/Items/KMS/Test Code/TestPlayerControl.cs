using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

using MIN;
using Photon.Pun;
using UnityEngine;

public class TestPlayerControl : MonoBehaviourPun
{

    [SerializeField] public Transform hand;

    public Item current;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            current.Abandon();
            current = null;
        }

    }
}
