using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GenItem : MonoBehaviour
{
    [SerializeField] ProductSprites ps;


    void Start()
    {
        ps.Init();
    }
}
