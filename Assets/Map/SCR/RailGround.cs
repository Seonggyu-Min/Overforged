using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailGround : MonoBehaviour
{
    [SerializeField] private float cycleTime;
    [SerializeField] private List<GameObject> ground;
    private void Start()
    {
        StartCoroutine(DisappearCycle());
    }

    private IEnumerator DisappearCycle()
    {
        while (true)
        {
            ground[0].SetActive(!ground[0].activeSelf);
            ground[1].SetActive(!ground[1].activeSelf);
            yield return new WaitForSeconds(cycleTime);
        }
    }
}
