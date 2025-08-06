using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorialmsg : MonoBehaviour
{

    [SerializeField] CanvasGroup group;

    public void ValueChanged(float val)
    {
        group.alpha = val;
    }

}
