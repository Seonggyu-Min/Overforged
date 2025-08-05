using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tongs : MonoBehaviour
{
    public Vector3 RespawnPoint;
    public bool IsRespawning;

    private void Start()
    {
        IsRespawning = false;
        RespawnPoint = transform.position;
    }
}
