using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GenItem : MonoBehaviour
{

    [SerializeField] ItemDataList itemList;

    [SerializeField] GameObject matitem;
    [SerializeField] Transform hand;

    private MaterialItem m;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F3))
        {
            GameObject go = Instantiate(matitem, hand);
            go.transform.position = hand.position;
            m = go.GetComponent<MaterialItem>();
            m.Data = itemList.list[3];
            m.Ore = OreType.Gold;

        }
        if (Input.GetKeyDown(KeyCode.F4))
        {

            m.Heat();
        }
        if (Input.GetKeyDown(KeyCode.F5))
        {

            m.Cool();
        }
        if (Input.GetKeyDown(KeyCode.F6))
        {

            m.ChangeToNext();
        }

    }
}
