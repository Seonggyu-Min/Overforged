using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayerControl : MonoBehaviour
{
    private Rigidbody rigid;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }


    // Update is called once per frame
    void Update()
    {

    }
    void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 movedist = transform.forward * v * 0.1f;
        rigid.MovePosition(rigid.position + movedist);

        rigid.rotation = rigid.rotation * Quaternion.Euler(0, h, 0);
    }
}
