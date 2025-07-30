using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class CharacterIndicator : MonoBehaviourPun
{
    [SerializeField] private GameObject indicatorPrefab; // UI 프리팹
    private GameObject currentIndicator;

    [SerializeField]
    private Vector3 offset = new Vector3(0, 0, 0); // 캐릭터 발 밑 위치

    void Start()
    {
        if (photonView.IsMine)
        {
            if (indicatorPrefab != null)
            {
                currentIndicator = Instantiate(indicatorPrefab, transform);
            }
        }
    }

    void LateUpdate()
    {
        if (photonView.IsMine && currentIndicator != null)
        {
            currentIndicator.transform.position = transform.position + offset;
        }
    }

    // 삭제 타이밍 언제?
    void OnDestroy()
    {
        if (photonView.IsMine && currentIndicator != null) // UI가 생성된 경우에만
        {
            Destroy(currentIndicator);
        }
    }
}
