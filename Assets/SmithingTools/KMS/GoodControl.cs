using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoodControl : MonoBehaviour
{
    void Update()
    {
        transform.forward = Camera.main.transform.forward;
    }

    public void OnAnimEnd()
    {
        Destroy(transform.root.gameObject);
    }
}
