using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailCube : MonoBehaviour
{
    // 무빙 워커가 플레이어를 이동시킬 속도
    [SerializeField] float moveSpeed = 2f;

    private Vector3 moveDirection; // 이동시킬 방향 (예: Vector3.right, Vector3.left 등)
    [SerializeField] float centerSnapSpeed = 5f; // 중앙으로 이동하는 속도 (높을수록 빠르게 스냅)
    private List<Transform> snappingTargets = new List<Transform>();

    void Start()
    {
        moveDirection = transform.forward;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!snappingTargets.Contains(collision.transform))
        {
            snappingTargets.Add(collision.transform); // 리스트에 추가
            StartCoroutine(SnapToCenter(collision.transform));
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        Rigidbody objectRidid = collision.rigidbody;
        if (!snappingTargets.Contains(collision.transform))
        {
            if (objectRidid != null)
            {
                objectRidid.velocity = moveDirection * moveSpeed + Vector3.up * objectRidid.velocity.y;
            }
        }

    }

    private void OnCollisionExit(Collision collision)
    {
        Rigidbody objectRidid = collision.rigidbody;

        if (objectRidid != null)
        {
            objectRidid.velocity = moveDirection * 0 + Vector3.up * 0;
        }
    }

    private IEnumerator SnapToCenter(Transform targetTransform)
    {
        // 무빙 워커의 중앙 XZ 평면 (Y축은 유지)
        Vector3 targetCenter = new Vector3(transform.position.x, targetTransform.position.y, transform.position.z);

        while (Vector3.Distance(targetTransform.position, targetCenter) > 0.05f) // 일정 거리 이하로 가까워지면 멈춤
        {
            // 중앙 방향으로 이동
            targetTransform.position = Vector3.Lerp(targetTransform.position, targetCenter, Time.deltaTime * centerSnapSpeed);
            yield return null; // 다음 프레임까지 대기
        }
        // 최종적으로 중앙에 스냅
        targetTransform.position = targetCenter;
        if (snappingTargets.Contains(targetTransform))
        {
            snappingTargets.Remove(targetTransform);
        }
    }

    // 무빙 워커의 이동 방향을 에디터에서 시각화하기 위함
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.TransformDirection(moveDirection) * 1f);
    }
}
