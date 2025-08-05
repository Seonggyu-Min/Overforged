using System.Collections;
using System.Collections.Generic;
using SCR;
using UnityEngine;

public class RespwanCube : MonoBehaviour
{
    public float respawnDelay = 5f; // 리스폰까지의 대기 시간 (초)


    private void OnTriggerEnter(Collider other)
    {
        // 플레이어일때
        if (other.CompareTag("Player"))
        {
            PlayerPhysical player = other.GetComponent<PlayerPhysical>();
            if (!player.IsRespawning)
            {
                Rigidbody playerRb = other.GetComponent<Rigidbody>();
                if (playerRb != null)
                {
                    playerRb.velocity = Vector3.zero; // 현재 속도 초기화
                    playerRb.angularVelocity = Vector3.zero; // 회전 속도 초기화
                }
                StartCoroutine(PlayerRespawn(other.transform));
            }
        }
        else if (other.CompareTag("Tongs"))
        {
            Tongs tongs = other.GetComponent<Tongs>();
            if (!tongs.IsRespawning)
            {
                Rigidbody tongsRb = other.GetComponent<Rigidbody>();
                if (tongsRb != null)
                {
                    tongsRb.velocity = Vector3.zero; // 현재 속도 초기화
                    tongsRb.angularVelocity = Vector3.zero; // 회전 속도 초기화
                }
                StartCoroutine(TongsRespawn(other.transform));
            }
        }
    }

    // 리스폰 코루틴
    IEnumerator PlayerRespawn(Transform playerTransform)
    {
        playerTransform.gameObject.SetActive(false);
        yield return new WaitForSeconds(respawnDelay);
        playerTransform.gameObject.SetActive(true);
        PlayerPhysical player = playerTransform.gameObject.GetComponent<PlayerPhysical>();
        playerTransform.position = player.RespawnPoint;
        player.IsRespawning = false;
    }

    IEnumerator TongsRespawn(Transform tongsTransform)
    {
        tongsTransform.gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);
        tongsTransform.gameObject.SetActive(true);
        Tongs tongs = tongsTransform.gameObject.GetComponent<Tongs>();
        tongsTransform.position = tongs.RespawnPoint;
        tongs.IsRespawning = false;
    }
}
