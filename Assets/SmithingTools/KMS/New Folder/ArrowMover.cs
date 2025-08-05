using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ArrowMover : MonoBehaviour
{
    [SerializeField] ScrollRect scrollRect;
    void Start()
    {
        scrollRect.verticalNormalizedPosition = 1;

    }

    void Update()
    {
        scrollRect.verticalNormalizedPosition -= Time.deltaTime;
        if (scrollRect.verticalNormalizedPosition <= 0)
        {
            scrollRect.verticalNormalizedPosition = 1;
        }

    }
}
