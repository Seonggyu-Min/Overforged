using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconFollow : MonoBehaviour
{
    public Transform target; // 머리위 빈 오브젝트
    public Vector3 offset = Vector3.up * 3f;

    void LateUpdate()
    {
        if (target == null) return;

        Debug.Log("바라보기");
        transform.position = target.position + offset;
        // transform.forward = Camera.main.transform.forward;
    }
}
