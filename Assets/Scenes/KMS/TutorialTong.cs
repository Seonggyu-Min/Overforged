using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTong : MonoBehaviour
{

    public Action OnGet;
    public Action OnAbandon;

    private Transform parent;

    void OnEnable()
    {
        OnAbandon?.Invoke();

    }
    void OnDisable()
    {
        OnGet?.Invoke();
    }
}
