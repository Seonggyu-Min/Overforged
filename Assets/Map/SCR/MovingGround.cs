using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingGround : MonoBehaviour
{
    [SerializeField] private List<Transform> movePoints; // 발판이 이동할 지점들
    [SerializeField] private float moveSpeed = 3f; // 발판의 이동 속도
    [SerializeField] private float waitTimeAtPoint = 5f; // 각 지점에 도착했을 때 대기 시간

    private Vector3 nextTarget; // 발판이 다음에 이동할 목표 지점
    private int index;

    private void Start()
    {
        if (movePoints.Count < 2)
        {
            Debug.LogError("movePoint가 2개 이상 설정되지 않았습니다!");
            enabled = false;
            return;
        }
        index = 0;
        transform.position = movePoints[index].position;
        index++;
        nextTarget = movePoints[index].position;

        StartCoroutine(MovePlatform());
    }

    private IEnumerator MovePlatform()
    {
        while (true)
        {
            while (Vector3.Distance(transform.position, nextTarget) > 0.05f)
            {
                transform.position = Vector3.MoveTowards(transform.position, nextTarget, moveSpeed * Time.deltaTime);
                yield return null;
            }

            transform.position = nextTarget;

            yield return new WaitForSeconds(waitTimeAtPoint);
            index++;
            if (index > movePoints.Count - 1) index = 0;
            nextTarget = movePoints[index].position;

        }
    }

    // 발판이 트리거 영역에 다른 콜라이더가 들어왔을 때
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Item") || other.CompareTag("Tongs"))
            other.transform.SetParent(transform);
    }

    // 발판이 트리거 영역에서 다른 콜라이더가 나갔을 때
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Item") || other.CompareTag("Tongs"))
            other.transform.SetParent(null);
    }

}
