using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisappearGround : MonoBehaviour
{
    [SerializeField] private float cycleTime;
    [SerializeField] private GameObject ground;

    private void Start()
    {
        StartCoroutine(DisappearCycle());
    }

    private IEnumerator DisappearCycle()
    {
        while (true)
        {
            ground.SetActive(!ground.activeSelf);
            yield return new WaitForSeconds(cycleTime);
        }
    }
}
