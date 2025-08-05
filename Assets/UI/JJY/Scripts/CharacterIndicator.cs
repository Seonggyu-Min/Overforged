using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class CharacterIndicator : MonoBehaviourPun
{
    [SerializeField] private GameObject indicatorPrefab; // UI 프리팹

    void Start()
    {
        ShowIndicator();
    }
    void ShowIndicator()
    {
        if (photonView.IsMine)
        {
            if (indicatorPrefab != null)
            {
                indicatorPrefab.SetActive(true);
            }
        }
    }

    // void LateUpdate()
    // {
    //     if (photonView.IsMine && currentIndicator != null)
    //     {
    //         currentIndicator.transform.position = transform.position + offset;
    //     }
    // }

    // 삭제 타이밍 언제?
    void HideIndicator()
    {
        if (photonView.IsMine)
        {
            if (indicatorPrefab != null)
            {
                indicatorPrefab.SetActive(false);
            }
        }
    }
}
